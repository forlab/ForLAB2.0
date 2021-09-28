import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartsModule as chartjsModule } from 'ng2-charts';
import { NgxEchartsModule } from 'ngx-echarts';
import { MorrisJsModule } from 'angular-morris-js';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
// Angualr Matrial
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
import { VendorRoutingModule } from './vendor-routing.module';
import { CoreModule } from 'src/@core/core.module';
// Components
import { VendorsComponent } from './vendor/vendors/vendors.component';
import { AddEditVendorComponent } from './vendor/add-edit-vendor/add-edit-vendor.component';
import { AddEditVendorContactComponent } from './vendor-contact/add-edit-vendor-contact/add-edit-vendor-contact.component';
import { VendorContactsComponent } from './vendor-contact/vendor-contacts/vendor-contacts.component';
import { VendorDetailsComponent } from './vendor/vendor-details/vendor-details.component';

@NgModule({
  declarations: [
    VendorsComponent, 
    AddEditVendorComponent, 
    AddEditVendorContactComponent, 
    VendorContactsComponent,
    VendorDetailsComponent, 
  ],
  imports: [
    CoreModule,
    CommonModule,
    VendorRoutingModule,
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
    MatExpansionModule,
    MatTooltipModule
  ]
})
export class VendorModule {}
