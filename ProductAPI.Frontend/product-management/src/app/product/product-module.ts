import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

import { ProductRoutingModule } from './product-routing-module';
import { ProductListComponent } from './components/product-list/product-list';
import { ProductService } from './services/product';

@NgModule({
  declarations: [
    ProductListComponent
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    ProductRoutingModule
  ],
  providers: [
    ProductService
  ]
})
export class ProductModule { }
