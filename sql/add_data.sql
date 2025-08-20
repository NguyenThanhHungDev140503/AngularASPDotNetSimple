-- Tùy chọn: USE <TenDatabase>;
SET NOCOUNT ON;
BEGIN TRY
    BEGIN TRAN;

    ------------------------------------------------------------
    -- 1) Khai báo 5 category mẫu (id sẽ tự NEWID() theo schema)
    ------------------------------------------------------------
    DECLARE @srcCats TABLE (
        ordinal     int         PRIMARY KEY,
        name        nvarchar(100),
        description nvarchar(MAX)
    );

    INSERT INTO @srcCats (ordinal, name, description)
    VALUES
        (1, N'Giày thể thao', N'Các dòng sneaker chạy/bóng rổ/đi hằng ngày'),
        (2, N'Dép & Sandal', N'Dép quai hậu, sandal đi biển, dép tiện dụng'),
        (3, N'Bốt',          N'Bốt cổ thấp/cao, da, thời trang & đi mưa'),
        (4, N'Giày da',      N'Giày tây/oxford/loafer cho công sở & sự kiện'),
        (5, N'Phụ kiện',     N'Dây giày, lót giày, chăm sóc và vệ sinh');

    -- Chỉ chèn category nếu chưa tồn tại (theo name)
    INSERT INTO dbo.[Category] (name, description)
    SELECT s.name, s.description
    FROM @srcCats s
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.[Category] c WHERE c.name = s.name
    );

    -- Lấy lại id của 5 category theo tên để map khi chèn product
    DECLARE @cats TABLE (
        ordinal int PRIMARY KEY,
        id      uniqueidentifier,
        name    nvarchar(100)
    );

    INSERT INTO @cats(ordinal, id, name)
    SELECT s.ordinal, c.id, c.name
    FROM @srcCats s
    JOIN dbo.[Category] c ON c.name = s.name;

    ------------------------------------------------------------
    -- 2) Sinh 100 product mẫu và gán đều vào 5 category
    --    - stock_price < price (giá vốn < giá bán)
    --    - stock_quantity phân bố 1..100
    ------------------------------------------------------------
    ;WITH nums AS (
        SELECT 1 AS n
        UNION ALL
        SELECT n + 1 FROM nums WHERE n < 100
    )
    INSERT INTO dbo.[Product] (name, description, stock_price, price, stock_quantity, category_id)
    SELECT
        -- Tên: Product 001..100 (kèm tên category cho dễ nhìn)
        CONCAT(N'Product ', RIGHT(CONCAT('000', n), 3), N' - ', cat.name) AS name,
        CONCAT(N'Sản phẩm demo #', n, N' thuộc ', cat.name)              AS description,

        -- Giá vốn: 30..149 (đơn vị tùy bạn, có 2 chữ số thập phân)
        CAST(((n * 37) % 120 + 30) AS DECIMAL(10,2))                      AS stock_price,

        -- Giá bán: tăng theo tỷ lệ 1.20..1.39 lần giá vốn, làm tròn 2 số
        CAST(
            (( (n * 37) % 120 + 30) * (1.20 + ((n % 20) * 0.01)))
            AS DECIMAL(10,2)
        )                                                                 AS price,

        -- Tồn kho: 1..100
        ((n * 11) % 100) + 1                                              AS stock_quantity,

        -- Phân bổ category vòng tròn 1..5
        cat.id                                                            AS category_id
    FROM nums
    JOIN @cats AS cat
      ON cat.ordinal = ((nums.n - 1) % 5) + 1
    OPTION (MAXRECURSION 100);

    COMMIT TRAN;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRAN;
    THROW;
END CATCH;
