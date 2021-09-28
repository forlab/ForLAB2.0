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
import { TestingRoutingModule } from './testing-routing.module';
import { CoreModule } from 'src/@core/core.module';
import { ImportFeatureModule } from 'src/app/+import-feature/import-feature.module';
// Components
import { AddEditTestComponent } from './test/add-edit-test/add-edit-test.component';
import { TestsComponent } from './test/tests/tests.component';
import { TestingProtocolsComponent } from './testing-protocol/testing-protocols/testing-protocols.component';
import { AddEditTestingProtocolComponent } from './testing-protocol/add-edit-testing-protocol/add-edit-testing-protocol.component';
import { TestDetailsComponent } from './test/test-details/test-details.component';
import { ProductUsageModule } from '../+product/product-usage.module';

@NgModule({
  declarations: [
  AddEditTestComponent,
  TestsComponent,
  TestingProtocolsComponent,
  AddEditTestingProtocolComponent,
  TestDetailsComponent],
  imports: [
    ImportFeatureModule,
    CoreModule,
    CommonModule,
    TestingRoutingModule,
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
    MatTooltipModule,
    ProductUsageModule
  ]
})
export class TestingModule {}
