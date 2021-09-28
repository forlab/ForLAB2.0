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
// Google Maps
import { AgmCoreModule } from '@agm/core';
import { GooglePlaceModule } from "ngx-google-places-autocomplete";
import { GoogleMapsKey } from 'src/@core/config';
// Routing
import { LookupRoutingModule } from './lookup-routing.module';
import { CoreModule } from 'src/@core/core.module';
// Components
import { CountriesComponent } from './country/countries/countries.component';
import { AddEditCountryComponent } from './country/add-edit-country/add-edit-country.component';
import { StaticLookupsComponent } from './static-lookups/static-lookups.component';
import { RegionsComponent } from './region/regions/regions.component';
import { AddEditRegionComponent } from './region/add-edit-region/add-edit-region.component';
import { LaboratoryCategoriesComponent } from './laboratory-category/laboratory-categories/laboratory-categories.component';
import { AddEditLaboratoryCategoryComponent } from './laboratory-category/add-edit-laboratory-category/add-edit-laboratory-category.component';
import { AddEditLaboratoryLevelComponent } from './laboratory-level/add-edit-laboratory-level/add-edit-laboratory-level.component';
import { LaboratoryLevelsComponent } from './laboratory-level/laboratory-levels/laboratory-levels.component';
import { PatientGroupsComponent } from './patient-group/patient-groups/patient-groups.component';
import { AddEditPatientGroupComponent } from './patient-group/add-edit-patient-group/add-edit-patient-group.component';
import { AddEditTestingAreaComponent } from './testing-area/add-edit-testing-area/add-edit-testing-area.component';
import { TestingAreasComponent } from './testing-area/testing-areas/testing-areas.component';

@NgModule({
  declarations: [
    CountriesComponent,
    AddEditCountryComponent,
    StaticLookupsComponent,
    RegionsComponent,
    AddEditRegionComponent,
    LaboratoryCategoriesComponent,
    AddEditLaboratoryCategoryComponent,
    AddEditLaboratoryLevelComponent,
    LaboratoryLevelsComponent,
    PatientGroupsComponent,
    AddEditPatientGroupComponent,
    AddEditTestingAreaComponent,
    TestingAreasComponent,
  ],
  imports: [
    // Google Maps
    GooglePlaceModule,
    AgmCoreModule.forRoot({
      apiKey: GoogleMapsKey,
      libraries: ["places"]
    }),
    CoreModule,
    CommonModule,
    LookupRoutingModule,
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
  ]
})
export class LookupModule { }
