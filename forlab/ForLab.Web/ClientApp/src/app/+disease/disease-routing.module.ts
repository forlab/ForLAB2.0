import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// Components
import { DiseasesComponent } from './disease/diseases/diseases.component';
import { CountryDiseaseIncidentsComponent } from './country-disease-incident/country-disease-incidents/country-disease-incidents.component';
import { DiseaseTestingProtocolsComponent } from './disease-testing-protocol/disease-testing-protocols/disease-testing-protocols.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'diseases',
    pathMatch: 'full'
  },
  {
    path: 'diseases',
    component: DiseasesComponent
  },
  {
    path: 'country-disease-incidents',
    component: CountryDiseaseIncidentsComponent
  },
  {
    path: 'disease-testing-protocols',
    component: DiseaseTestingProtocolsComponent
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DiseaseRoutingModule { }
