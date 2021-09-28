import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// Components
import { InstrumentsComponent } from './instrument/instruments/instruments.component';
import { ProductsComponent } from './product/products/products.component';
import { ProductDetailsComponent } from './product/product-details/product-details.component';
import { CountryProductPricesComponent } from './country-product-price/country-product-prices/country-product-prices.component';
import { RegionProductPricesComponent } from './region-product-price/region-product-prices/region-product-prices.component';
import { LaboratoryProductPricesComponent } from './laboratory-product-price/laboratory-product-prices/laboratory-product-prices.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'products',
    pathMatch: 'full'
  },
  {
    path: 'instruments',
    component: InstrumentsComponent
  },
  {
    path: 'products',
    component: ProductsComponent
  },
  {
    path: 'products/details/:productId',
    component: ProductDetailsComponent
  },
  {
    path: 'country-product-prices',
    component: CountryProductPricesComponent
  },
  {
    path: 'region-product-prices',
    component: RegionProductPricesComponent
  },
  {
    path: 'laboratory-product-prices',
    component: LaboratoryProductPricesComponent
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProductRoutingModule { }
