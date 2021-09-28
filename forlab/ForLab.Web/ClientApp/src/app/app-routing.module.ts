import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { NgxPermissionsGuard } from 'ngx-permissions';
import { AuthGuard } from '../@core/services/auth.guard';

const routes: Routes = [
  {
    path: 'dashboard',
    canLoad: [AuthGuard],
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./dashboard/dashboard.module').then(m => m.DashboardModule)
  },
  {
    path: 'authentication',
    loadChildren: () =>
      import('./authentication/authentication.module').then(
        m => m.AuthenticationModule
      )
  },
  {
    path: 'lookup',
    canLoad: [AuthGuard],
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./+lookup/lookup.module').then(
        m => m.LookupModule
      )
  },
  {
    path: 'user',
    canLoad: [AuthGuard],
    canActivate: [AuthGuard], 
    loadChildren: () =>
      import('./+user/user.module').then(
        m => m.UserModule
      )
  },
  {
    path: 'vendor',
    canLoad: [AuthGuard],
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./+vendor/vendor.module').then(
        m => m.VendorModule
      )
  },
  {
    path: 'disease',
    canLoad: [AuthGuard],
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./+disease/disease.module').then(
        m => m.DiseaseModule
      )
  },
  {
    path: 'configuration',
    canLoad: [AuthGuard],
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./+configuration/configuration.module').then(
        m => m.ConfigurationModule
      )
  },
  {
    path: 'product',
    canLoad: [AuthGuard],
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./+product/product.module').then(
        m => m.ProductModule
      )
  },
  {
    path: 'laboratory',
    canLoad: [AuthGuard],
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./+laboratory/laboratory.module').then(
        m => m.LaboratoryModule
      )
  },
  {
    path: 'testing',
    canLoad: [AuthGuard],
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./+testing/testing.module').then(
        m => m.TestingModule
      )
  },
  {
    path: 'CMS',
    canLoad: [AuthGuard],
    canActivate: [AuthGuard, NgxPermissionsGuard], 
		data: { permissions: { only: ['SuperAdmin'], redirectTo: '/' } },
    loadChildren: () =>
      import('./+CMS/CMS.module').then(
        m => m.CMSModule
      )
  },
  {
    path: 'program',
    canLoad: [AuthGuard],
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./+program/program.module').then(
        m => m.ProgramModule
      )
  },
  {
    path: 'forecasting',
    canLoad: [AuthGuard],
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./+forecasting/forecasting.module').then(
        m => m.ForecastingModule
      )
  },
  {
    path: 'import-feature',
    canLoad: [AuthGuard],
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./+import-feature/import-feature.module').then(
        m => m.ImportFeatureModule
      )
  },
  {path: '', redirectTo: '/dashboard/main', pathMatch: 'full'},
  {path: '**', redirectTo: '/dashboard/main', pathMatch: 'full'}
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
