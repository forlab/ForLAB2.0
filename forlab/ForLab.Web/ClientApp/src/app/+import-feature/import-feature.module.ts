import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartsModule as chartjsModule } from 'ng2-charts';
import { NgxEchartsModule } from 'ngx-echarts';
import { MorrisJsModule } from 'angular-morris-js';
// Lib
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
import { TableVirtualScrollModule } from 'ng-table-virtual-scroll';
import { ScrollingModule } from '@angular/cdk/scrolling';
// Routing
import { ImportFeatureRoutingModule } from './import-feature-routing.module';
import { CoreModule } from 'src/@core/core.module';
// Components
import { ImportRootComponent } from './import-root/import-root.component';
import { ImportPatientGroupsComponent } from './import-patient-groups/import-patient-groups.component';
import { ImportLaboratoryLevelsComponent } from './import-laboratory-levels/import-laboratory-levels.component';
import { ImportLaboratoryCategoriesComponent } from './import-laboratory-categories/import-laboratory-categories.component';
import { ImportTestingAreasComponent } from './import-testing-areas/import-testing-areas.component';
import { ImportCountriesComponent } from './import-countries/import-countries.component';
import { ImportRegionsComponent } from './import-regions/import-regions.component';
import { ImportVendorsComponent } from './import-vendors/import-vendors.component';
import { ImportLaboratoriesComponent } from './import-laboratories/import-laboratories.component';
import { ImportTestsComponent } from './import-tests/import-tests.component';
import { ImportInstrumentsComponent } from './import-instruments/import-instruments.component';
import { ImportProductsComponent } from './import-products/import-products.component';
import { ImportCountryDiseaseIncidentsComponent } from './import-country-disease-incidents/import-country-disease-incidents.component';
import { ImportProgramsComponent } from './import-programs/import-programs.component';
import { ImportDiseasesComponent } from './import-diseases/import-diseases.component';
import { ImportVendorContactsComponent } from './import-vendor-contacts/import-vendor-contacts.component';
import { ImportLaboratoryConsumptionsComponent } from './import-laboratory-consumptions/import-laboratory-consumptions.component';
import { ImportLaboratoryInstrumentsComponent } from './import-laboratory-instruments/import-laboratory-instruments.component';
import { ImportLaboratoryPatientStatisticsComponent } from './import-laboratory-patient-statistics/import-laboratory-patient-statistics.component';
import { ImportLaboratoryTestServicesComponent } from './import-laboratory-test-services/import-laboratory-test-services.component';
import { ImportCountryProductPricesComponent } from './import-country-product-prices/import-country-product-prices.component';
import { ImportRegionProductPricesComponent } from './import-region-product-prices/import-region-product-prices.component';
import { ImportLaboratoryProductPricesComponent } from './import-laboratory-product-prices/import-laboratory-product-prices.component';
import { ImportProductUsageComponent } from './import-product-usage/import-product-usage.component';
// Want to Export
import { ImportTestingAssumptionParametersComponent } from './import-testing-assumption-parameters/import-testing-assumption-parameters.component';
import { ImportPatientAssumptionParametersComponent } from './import-patient-assumption-parameters/import-patient-assumption-parameters.component';
import { ImportProductAssumptionParametersComponent } from './import-product-assumption-parameters/import-product-assumption-parameters.component';
import { ImportProgramTestsComponent } from './import-program-tests/import-program-tests.component';
import { PeriodMonthsPopupComponent } from './import-program-tests/period-months-popup/period-months-popup.component';

@NgModule({
  declarations: [
    ImportRootComponent,
    ImportPatientGroupsComponent,
    ImportLaboratoryLevelsComponent,
    ImportLaboratoryCategoriesComponent,
    ImportTestingAreasComponent,
    ImportCountriesComponent,
    ImportRegionsComponent,
    ImportVendorsComponent,
    ImportLaboratoriesComponent,
    ImportTestsComponent,
    ImportInstrumentsComponent,
    ImportProductsComponent,
    ImportCountryDiseaseIncidentsComponent,
    ImportProgramsComponent,
    ImportDiseasesComponent,
    ImportVendorContactsComponent,
    ImportLaboratoryConsumptionsComponent,
    ImportLaboratoryInstrumentsComponent,
    ImportLaboratoryPatientStatisticsComponent,
    ImportLaboratoryTestServicesComponent,
    ImportCountryProductPricesComponent,
    ImportRegionProductPricesComponent,
    ImportLaboratoryProductPricesComponent,
    ImportProductUsageComponent,
    // export
    ImportTestingAssumptionParametersComponent,
    ImportPatientAssumptionParametersComponent,
    ImportProductAssumptionParametersComponent,
    ImportProgramTestsComponent,
    PeriodMonthsPopupComponent,
  ],
  imports: [
    CoreModule,
    CommonModule,
    ImportFeatureRoutingModule,
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
    // Virtual Scrolling
    TableVirtualScrollModule,
    ScrollingModule
  ],
  exports: [ // add component that you want to use outside the import module
    ImportTestingAssumptionParametersComponent,
    ImportPatientAssumptionParametersComponent,
    ImportProductAssumptionParametersComponent,
    ImportProgramTestsComponent,
    // Virtual Scrolling
    TableVirtualScrollModule,
    ScrollingModule
  ]
})
export class ImportFeatureModule { }
