-- Loại bỏ phần mở rộng PostgreSQL

-- Tạo bảng User
CREATE TABLE [User] (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name NVARCHAR(100) NOT NULL,
    email NVARCHAR(255) UNIQUE NOT NULL,
    sodienthoai NVARCHAR(20) UNIQUE NOT NULL,
    password NVARCHAR(255) NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);

-- Tạo bảng Role
CREATE TABLE [Role] (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name NVARCHAR(50) NOT NULL,
    description NVARCHAR(MAX),
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);

-- Tạo bảng UserRole
CREATE TABLE [UserRole] (
    user_id UNIQUEIDENTIFIER,
    role_id UNIQUEIDENTIFIER,
    PRIMARY KEY (user_id, role_id),
    FOREIGN KEY (user_id) REFERENCES [User](id),
    FOREIGN KEY (role_id) REFERENCES [Role](id)
);

-- Tạo bảng Category
CREATE TABLE [Category] (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name NVARCHAR(100) NOT NULL,
    description NVARCHAR(MAX),
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);

-- Tạo bảng Product
CREATE TABLE [Product] (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name NVARCHAR(200) NOT NULL,
    description NVARCHAR(MAX),
    stock_price DECIMAL(10,2) NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    stock_quantity INT NOT NULL,
    category_id UNIQUEIDENTIFIER REFERENCES [Category](id),
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);

-- Tạo bảng DiscountCode
CREATE TABLE [DiscountCode] (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    code NVARCHAR(20) UNIQUE NOT NULL,
    discount_percentage DECIMAL(5,2) NOT NULL,
    max_uses INT NOT NULL,
    uses_count INT DEFAULT 0,
    min_order_value DECIMAL(10,2),
    start_date DATE NOT NULL,
    end_date DATE NOT NULL
);

-- Tạo bảng Order
CREATE TABLE [Order] (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    user_id UNIQUEIDENTIFIER REFERENCES [User](id),
    status NVARCHAR(20) CHECK (status IN (
        'pending',
        'processing',
        'shipped',
        'delivered',
        'cancelled'
    )),
    total_amount DECIMAL(10,2) NOT NULL,
    discount_code_id UNIQUEIDENTIFIER REFERENCES [DiscountCode](id),
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);

-- Tạo bảng OrderDetail
CREATE TABLE [OrderDetail] (
    order_id UNIQUEIDENTIFIER,
    product_id UNIQUEIDENTIFIER,
    quantity INT,
    price_at_purchase DECIMAL(10,2),
    PRIMARY KEY (order_id, product_id),
    FOREIGN KEY (order_id) REFERENCES [Order](id),
    FOREIGN KEY (product_id) REFERENCES [Product](id)
);

-- Tạo bảng Address
CREATE TABLE [Address] (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    user_id UNIQUEIDENTIFIER REFERENCES [User](id) NOT NULL,
    street NVARCHAR(255) NOT NULL,
    city NVARCHAR(100) NOT NULL,
    postal_code NVARCHAR(20) NOT NULL,
    is_default BIT DEFAULT 0,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);

-- Bảng master: PaymentMethod
CREATE TABLE [PaymentMethod] (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    code NVARCHAR(30) UNIQUE NOT NULL,
    name NVARCHAR(100) NOT NULL,
    provider NVARCHAR(50),
    fee_percent DECIMAL(5,2) DEFAULT 0,
    is_active BIT DEFAULT 1,
    created_at DATETIME DEFAULT GETDATE()
);

-- Tạo bảng Payment
CREATE TABLE [Payment] (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    order_id UNIQUEIDENTIFIER REFERENCES [Order](id) NOT NULL,
    payment_method_id UNIQUEIDENTIFIER REFERENCES [PaymentMethod](id) NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    status NVARCHAR(20) CHECK (status IN (
        'pending',
        'processing',
        'success',
        'failed',
        'cancelled',
        'refunded'
    )) DEFAULT 'pending',
    provider_txn_id NVARCHAR(255),
    provider_fee DECIMAL(10,2) DEFAULT 0,
    metadata NVARCHAR(MAX),
    paid_at DATETIME,
    idempotency_key UNIQUEIDENTIFIER UNIQUE,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);

-- Tạo bảng Shipping
CREATE TABLE [Shipping] (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    order_id UNIQUEIDENTIFIER REFERENCES [Order](id) NOT NULL,
    address_id UNIQUEIDENTIFIER REFERENCES [Address](id) NOT NULL,
    status NVARCHAR(20) CHECK (status IN (
        'pending',
        'shipping',
        'delivered'
    )),
    shipper_id UNIQUEIDENTIFIER REFERENCES [User](id),
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);

-- Tạo bảng Promotion
CREATE TABLE [Promotion] (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name NVARCHAR(100) NOT NULL,
    description NVARCHAR(MAX),
    discount_percentage DECIMAL(5,2) NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);

-- Tạo bảng Review
CREATE TABLE [Review] (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    product_id UNIQUEIDENTIFIER REFERENCES [Product](id) NOT NULL,
    user_id UNIQUEIDENTIFIER REFERENCES [User](id) NOT NULL,
    rating INT CHECK (rating BETWEEN 1 AND 5),
    comment NVARCHAR(MAX),
    created_at DATETIME DEFAULT GETDATE()
);

-- Tạo bảng Wishlist
CREATE TABLE [Wishlist] (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    user_id UNIQUEIDENTIFIER REFERENCES [User](id) NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);

-- Tạo bảng Cart
CREATE TABLE [Cart] (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    user_id UNIQUEIDENTIFIER REFERENCES [User](id) NOT NULL UNIQUE,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);

-- Bảng trung gian CartItem (N:M)
CREATE TABLE [CartItem] (
    cart_id UNIQUEIDENTIFIER REFERENCES [Cart](id),
    product_id UNIQUEIDENTIFIER REFERENCES [Product](id),
    quantity INT NOT NULL,
    PRIMARY KEY (cart_id, product_id)
);

-- Bảng trung gian PromotionProduct (N:M)
CREATE TABLE [PromotionProduct] (
    promotion_id UNIQUEIDENTIFIER REFERENCES [Promotion](id),
    product_id UNIQUEIDENTIFIER REFERENCES [Product](id),
    PRIMARY KEY (promotion_id, product_id)
);

-- Tạo bảng Permission
CREATE TABLE [Permission] (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name NVARCHAR(50) NOT NULL,
    description NVARCHAR(MAX),
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);

-- Bảng trung gian RolePermission (N:M)
CREATE TABLE [RolePermission] (
    role_id UNIQUEIDENTIFIER REFERENCES [Role](id),
    permission_id UNIQUEIDENTIFIER REFERENCES [Permission](id),
    PRIMARY KEY (role_id, permission_id)
);


