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
import { DiseaseRoutingModule } from './disease-routing.module';
import { CoreModule } from 'src/@core/core.module';
// Components
import { DiseasesComponent } from './disease/diseases/diseases.component';
import { CountryDiseaseIncidentsComponent } from './country-disease-incident/country-disease-incidents/country-disease-incidents.component';
import { DiseaseTestingProtocolsComponent } from './disease-testing-protocol/disease-testing-protocols/disease-testing-protocols.component';
import { AddEditDiseaseComponent } from './disease/add-edit-disease/add-edit-disease.component';
import { AddEditDiseaseTestingProtocolComponent } from './disease-testing-protocol/add-edit-disease-testing-protocol/add-edit-disease-testing-protocol.component';
import { AddEditCountryDiseaseIncidentComponent } from './country-disease-incident/add-edit-country-disease-incident/add-edit-country-disease-incident.component';


@NgModule({
  declarations: [
  DiseasesComponent,
  CountryDiseaseIncidentsComponent,
  DiseaseTestingProtocolsComponent,
  AddEditDiseaseComponent,
  AddEditDiseaseTestingProtocolComponent,
  AddEditCountryDiseaseIncidentComponent],
  imports: [
    CoreModule,
    CommonModule,
    DiseaseRoutingModule,
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
export class DiseaseModule {}
