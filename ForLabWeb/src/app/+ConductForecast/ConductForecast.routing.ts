
import { Routes, RouterModule } from "@angular/router";

import { ForecastComparisonComponent } from "./ForecastComparison/ForecastComparison.component";
import { ConductForecastListComponent } from "./ConductForecastList/ConductForecastList.component";
import { ForecastReportComponent } from "./ForecastReport/ForecastReport.component";

export const routes: Routes = [
  {
    path: '',
    component: ConductForecastListComponent,
  },
  {
    path: 'CostsComparison',
    component: ForecastComparisonComponent
  },
  {
    path: 'ForecastReport/:id',
    component: ForecastReportComponent
  }

];


export const routing = RouterModule.forChild(routes);

