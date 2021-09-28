import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// Components
import { MainComponent } from './main/main.component';
import { ForecastDashboardComponent } from './forecast-dashboard/forecast-dashboard.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'main',
    pathMatch: 'full'
  },
  {
    path: 'main',
    component: MainComponent
  },
  {
    path: 'forecast-dashboard',
    component: ForecastDashboardComponent
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule {}
