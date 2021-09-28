import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// Components
import { CountriesComponent } from './country/countries/countries.component';
import { AddEditCountryComponent } from './country/add-edit-country/add-edit-country.component';
import { StaticLookupsComponent } from './static-lookups/static-lookups.component';
import { RegionsComponent } from './region/regions/regions.component';
import { LaboratoryCategoriesComponent } from './laboratory-category/laboratory-categories/laboratory-categories.component';
import { LaboratoryLevelsComponent } from './laboratory-level/laboratory-levels/laboratory-levels.component';
import { PatientGroupsComponent } from './patient-group/patient-groups/patient-groups.component';
import { TestingAreasComponent } from './testing-area/testing-areas/testing-areas.component';
import { NgxPermissionsGuard } from 'ngx-permissions';
import { AuthGuard } from 'src/@core/services/auth.guard';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'static-lookups',
    pathMatch: 'full'
  },
  {
    path: 'static-lookups',
    component: StaticLookupsComponent
  },
  {
    path: 'countries',
    component: CountriesComponent
  },
  {
    path: 'countries/add',
    component: AddEditCountryComponent,
    canActivate: [AuthGuard, NgxPermissionsGuard], 
		data: { permissions: { only: ['SuperAdmin'], redirectTo: '/' } },
  },
  {
    path: 'countries/update/:countryId',
    component: AddEditCountryComponent,
    canActivate: [AuthGuard, NgxPermissionsGuard], 
		data: { permissions: { only: ['SuperAdmin'], redirectTo: '/' } },
  },
  {
    path: 'regions',
    component: RegionsComponent
  },
  {
    path: 'laboratory-categories',
    component: LaboratoryCategoriesComponent
  },
  {
    path: 'laboratory-levels',
    component: LaboratoryLevelsComponent
  },
  {
    path: 'patient-groups',
    component: PatientGroupsComponent
  },
  {
    path: 'testing-areas',
    component: TestingAreasComponent
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LookupRoutingModule { }
