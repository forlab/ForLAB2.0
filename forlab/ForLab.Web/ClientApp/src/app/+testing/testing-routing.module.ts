import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// Components
import { TestsComponent } from './test/tests/tests.component';
import { TestingProtocolsComponent } from './testing-protocol/testing-protocols/testing-protocols.component';
import { TestDetailsComponent } from './test/test-details/test-details.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'testings',
    pathMatch: 'full'
  },
  {
    path: 'tests',
    component: TestsComponent
  },
  {
    path: 'tests/details/:testId',
    component: TestDetailsComponent
  },
  {
    path: 'testing-protocols',
    component: TestingProtocolsComponent
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TestingRoutingModule { }
