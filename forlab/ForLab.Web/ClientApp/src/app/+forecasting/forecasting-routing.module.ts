import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// Components
import { ForecastsComponent } from './forecasts/forecasts.component';
import { AddForecastComponent } from './add-forecast/add-forecast.component';
import { ForecastDetailsComponent } from './forecast-details/forecast-details.component';
import { EditForecastComponent } from './edit-forecast/edit-forecast.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'forecasts',
    pathMatch: 'full'
  },
  {
    path: 'forecasts',
    component: ForecastsComponent
  },
  {
    path: 'forecasts/add',
    component: AddForecastComponent
  },
  {
    path: 'forecasts/update/:forecastInfoId',
    component: EditForecastComponent
  },
  {
    path: 'forecasts/details/:forecastInfoId',
    component: ForecastDetailsComponent
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ForecastingRoutingModule { }
