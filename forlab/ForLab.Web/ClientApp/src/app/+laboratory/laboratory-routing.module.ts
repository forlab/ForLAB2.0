import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// Components
import { LaboratoryWorkingDaysComponent } from './laboratory-working-day/laboratory-working-days/laboratory-working-days.component';
import { LaboratoryInstrumentsComponent } from './laboratory-instrument/laboratory-instruments/laboratory-instruments.component';
import { LaboratoryPatientStatisticsComponent } from './laboratory-patient-statistic/laboratory-patient-statistics/laboratory-patient-statistics.component';
import { LaboratoryConsumptionsComponent } from './laboratory-consumption/laboratory-consumptions/laboratory-consumptions.component';
import { LaboratoryTestServicesComponent } from './laboratory-test-service/laboratory-test-services/laboratory-test-services.component';
import { LaboratoriesComponent } from './laboratory/laboratories/laboratories.component';
import { LaboratoryDetailsComponent } from './laboratory/laboratory-details/laboratory-details.component';
import { AddEditLaboratoryComponent } from './laboratory/add-edit-laboratory/add-edit-laboratory.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: '',
    pathMatch: 'full'
  },
  {
    path: 'laboratories',
    component: LaboratoriesComponent
  },
  {
    path: 'laboratories/add',
    component: AddEditLaboratoryComponent
  },
  {
    path: 'laboratories/update/:laboratoryId',
    component: AddEditLaboratoryComponent
  },
  {
    path: 'laboratories/details/:laboratoryId',
    component: LaboratoryDetailsComponent
  },
  {
    path: 'laboratory-working-days',
    component: LaboratoryWorkingDaysComponent
  },
  {
    path: 'laboratory-instruments',
    component: LaboratoryInstrumentsComponent
  },
  {
    path: 'laboratory-patient-statistics',
    component: LaboratoryPatientStatisticsComponent
  },
  {
    path: 'laboratory-consumptions',
    component: LaboratoryConsumptionsComponent
  },
  {
    path: 'laboratory-test-services',
    component: LaboratoryTestServicesComponent
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LaboratoryRoutingModule { }
