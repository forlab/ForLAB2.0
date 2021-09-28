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
import { MatStepperModule } from '@angular/material/stepper';
import { MatTabsModule } from '@angular/material/tabs';
// Routing
import { ForecastingRoutingModule } from './forecasting-routing.module';
import { CoreModule } from 'src/@core/core.module';
import { ImportFeatureModule } from 'src/app/+import-feature/import-feature.module';
// Components
import { AddForecastComponent } from './add-forecast/add-forecast.component';
import { ForecastsComponent } from './forecasts/forecasts.component';
import { ForecastTestsComponent } from './shared/forecast-tests/forecast-tests.component';
import { ForecastInstrumentsComponent } from './shared/forecast-instruments/forecast-instruments.component';
import { ForecastMorbidityProgramsComponent } from './shared/forecast-morbidity-programs/forecast-morbidity-programs.component';
import { ForecastDetailsComponent } from './forecast-details/forecast-details.component';
import { EditForecastComponent } from './edit-forecast/edit-forecast.component';
import { ForecastPatientGroupsComponent } from './shared/forecast-patient-groups/forecast-patient-groups.component';
import { ForecastPatientAssumptionsComponent } from './shared/forecast-patient-assumptions/forecast-patient-assumptions.component';
import { ForecastProductAssumptionsComponent } from './shared/forecast-product-assumptions/forecast-product-assumptions.component';
import { ForecastTestingAssumptionsComponent } from './shared/forecast-testing-assumptions/forecast-testing-assumptions.component';
import { ForecastTargetBasesComponent } from './shared/forecast-target-bases/forecast-target-bases.component';
import { ForecastWhoBasesComponent } from './shared/forecast-who-bases/forecast-who-bases.component';
import { ForecastTestingProtocolMonthsComponent } from './shared/forecast-testing-protocol-months/forecast-testing-protocol-months.component';
import { HistoicalConsumptionsComponent } from './shared/histoical-consumptions/histoical-consumptions.component';
import { HistoicalServiceDataComponent } from './shared/histoical-service-data/histoical-service-data.component';
import { ForecastChildListComponent } from './forecast-details/forecast-child-list/forecast-child-list.component';
import { ForecastChartComponent } from './forecast-details/forecast-chart/forecast-chart.component';
import { ResultChartComponent } from './forecast-details/result-chart/result-chart.component';

@NgModule({
  declarations: [
  AddForecastComponent,
  ForecastsComponent,
  ForecastTestsComponent,
  ForecastInstrumentsComponent,
  ForecastMorbidityProgramsComponent,
  ForecastDetailsComponent,
  EditForecastComponent,
  ForecastPatientGroupsComponent,
  ForecastPatientAssumptionsComponent,
  ForecastProductAssumptionsComponent,
  ForecastTestingAssumptionsComponent,
  ForecastTargetBasesComponent,
  ForecastWhoBasesComponent,
  ForecastTestingProtocolMonthsComponent,
  HistoicalConsumptionsComponent,
  HistoicalServiceDataComponent,
  ForecastChildListComponent,
  ForecastChartComponent,
  ResultChartComponent],
  imports: [
    ImportFeatureModule,
    CoreModule,
    CommonModule,
    ForecastingRoutingModule,
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
    MatStepperModule,
    MatTabsModule
  ]
})
export class ForecastingModule {}
