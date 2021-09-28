import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartsModule as chartjsModule } from 'ng2-charts';
import { NgxEchartsModule } from 'ngx-echarts';
import { MorrisJsModule } from 'angular-morris-js';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
// Angualr Matrial
import { MatListModule } from '@angular/material/list';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatMenuModule } from '@angular/material/menu';
import { MatTableModule } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
// Routing
import { ProductRoutingModule } from './product-routing.module';
import { CoreModule } from 'src/@core/core.module';
// Components
import { AddEditInstrumentComponent } from './instrument/add-edit-instrument/add-edit-instrument.component';
import { InstrumentsComponent } from './instrument/instruments/instruments.component';
import { ProductsComponent } from './product/products/products.component';
import { AddEditProductComponent } from './product/add-edit-product/add-edit-product.component';
import { ProductDetailsComponent } from './product/product-details/product-details.component';
import { CountryProductPricesComponent } from './country-product-price/country-product-prices/country-product-prices.component';
import { AddEditCountryProductPriceComponent } from './country-product-price/add-edit-country-product-price/add-edit-country-product-price.component';
import { AddEditRegionProductPriceComponent } from './region-product-price/add-edit-region-product-price/add-edit-region-product-price.component';
import { RegionProductPricesComponent } from './region-product-price/region-product-prices/region-product-prices.component';
import { LaboratoryProductPricesComponent } from './laboratory-product-price/laboratory-product-prices/laboratory-product-prices.component';
import { AddEditLaboratoryProductPriceComponent } from './laboratory-product-price/add-edit-laboratory-product-price/add-edit-laboratory-product-price.component';
import { ProductUsageModule } from './product-usage.module';

@NgModule({
  declarations: [
    
  AddEditInstrumentComponent,
    
  InstrumentsComponent,
    
  ProductsComponent,
    
  AddEditProductComponent,
    
  ProductDetailsComponent,
    
  CountryProductPricesComponent,
    
  AddEditCountryProductPriceComponent,
    
  AddEditRegionProductPriceComponent,
    
  RegionProductPricesComponent,
    
  LaboratoryProductPricesComponent,
    
  AddEditLaboratoryProductPriceComponent],
  imports: [
    CoreModule,
    CommonModule,
    ProductRoutingModule,
    chartjsModule,
    NgxEchartsModule,
    MorrisJsModule,
    MatIconModule,
    MatButtonModule,
    MatPaginatorModule,
    MatSortModule,
    MatMenuModule,
    MatTableModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatInputModule,
    NgxDatatableModule,
    MatTooltipModule,
    MatExpansionModule,
    MatTabsModule,
    MatListModule,
    ProductUsageModule,
  ]
})
export class ProductModule { }
