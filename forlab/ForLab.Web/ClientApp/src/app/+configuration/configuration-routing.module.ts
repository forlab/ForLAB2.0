import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// Components
import { ConfigurationComponent } from './configuration/configuration.component';
import { ConfigurationAuditsComponent } from './configuration-audits/configuration-audits.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: '',
    pathMatch: 'full'
  },
  {
    path: 'edit',
    component: ConfigurationComponent
  },
  {
    path: 'audits',
    component: ConfigurationAuditsComponent
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ConfigurationRoutingModule { }
