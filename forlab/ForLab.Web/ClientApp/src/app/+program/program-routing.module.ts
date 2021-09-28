import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// Components
import { ProgramsComponent } from './program/programs/programs.component';
import { AddProgramComponent } from './program/add-program/add-program.component';
import { ProgramDetailsComponent } from './program/program-details/program-details.component';
import { ProgramTestsComponent } from './program-test/program-tests/program-tests.component';
import { PatientAssumptionsComponent } from './patient-assumption/patient-assumptions/patient-assumptions.component';
import { ProductAssumptionsComponent } from './product-assumption/product-assumptions/product-assumptions.component';
import { TestingAssumptionsComponent } from './testing-assumption/testing-assumptions/testing-assumptions.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'programs',
    pathMatch: 'full'
  },
  {
    path: 'programs',
    component: ProgramsComponent
  },
  {
    path: 'programs/add',
    component: AddProgramComponent
  },
  {
    path: 'programs/details/:programId',
    component: ProgramDetailsComponent
  },
  {
    path: 'program-tests',
    component: ProgramTestsComponent
  },
  {
    path: 'patient-assumptions',
    component: PatientAssumptionsComponent
  },
  {
    path: 'product-assumptions',
    component: ProductAssumptionsComponent
  },
  {
    path: 'testing-assumptions',
    component: TestingAssumptionsComponent
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProgramRoutingModule { }
