import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartsModule as chartjsModule } from 'ng2-charts';
import { NgxEchartsModule } from 'ngx-echarts';
import { MorrisJsModule } from 'angular-morris-js';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
// Libs
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
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
// Google Maps
import { AgmCoreModule } from '@agm/core';
import { GooglePlaceModule } from "ngx-google-places-autocomplete";
import { GoogleMapsKey } from 'src/@core/config';
// Routing
import { LaboratoryRoutingModule } from './laboratory-routing.module';
import { CoreModule } from 'src/@core/core.module';
// Components
import { LaboratoryWorkingDaysComponent } from './laboratory-working-day/laboratory-working-days/laboratory-working-days.component';
import { AddEditLaboratoryWorkingDayComponent } from './laboratory-working-day/add-edit-laboratory-working-day/add-edit-laboratory-working-day.component';
import { AddEditLaboratoryInstrumentComponent } from './laboratory-instrument/add-edit-laboratory-instrument/add-edit-laboratory-instrument.component';
import { LaboratoryInstrumentsComponent } from './laboratory-instrument/laboratory-instruments/laboratory-instruments.component';
import { LaboratoryPatientStatisticsComponent } from './laboratory-patient-statistic/laboratory-patient-statistics/laboratory-patient-statistics.component';
import { AddEditLaboratoryPatientStatisticComponent } from './laboratory-patient-statistic/add-edit-laboratory-patient-statistic/add-edit-laboratory-patient-statistic.component';
import { AddEditLaboratoryConsumptionComponent } from './laboratory-consumption/add-edit-laboratory-consumption/add-edit-laboratory-consumption.component';
import { LaboratoryConsumptionsComponent } from './laboratory-consumption/laboratory-consumptions/laboratory-consumptions.component';
import { LaboratoryTestServicesComponent } from './laboratory-test-service/laboratory-test-services/laboratory-test-services.component';
import { AddEditLaboratoryTestServiceComponent } from './laboratory-test-service/add-edit-laboratory-test-service/add-edit-laboratory-test-service.component';
import { AddEditLaboratoryComponent } from './laboratory/add-edit-laboratory/add-edit-laboratory.component';
import { LaboratoriesComponent } from './laboratory/laboratories/laboratories.component';
import { LaboratoryDetailsComponent } from './laboratory/laboratory-details/laboratory-details.component';

@NgModule({
  declarations: [
  LaboratoryWorkingDaysComponent,
  AddEditLaboratoryWorkingDayComponent,
  AddEditLaboratoryInstrumentComponent,
  LaboratoryInstrumentsComponent,
  LaboratoryPatientStatisticsComponent,
  AddEditLaboratoryPatientStatisticComponent,
  AddEditLaboratoryConsumptionComponent,
  LaboratoryConsumptionsComponent,
  LaboratoryTestServicesComponent,
  AddEditLaboratoryTestServiceComponent,
  AddEditLaboratoryComponent, 
  LaboratoriesComponent, 
  LaboratoryDetailsComponent,],
  imports: [
    // Google Maps
    GooglePlaceModule,
    AgmCoreModule.forRoot({
      apiKey: GoogleMapsKey,
      libraries: ["places"]
    }),
    CoreModule,
    CommonModule,
    LaboratoryRoutingModule,
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
    NgxMaterialTimepickerModule
  ]
})
export class LaboratoryModule { }
