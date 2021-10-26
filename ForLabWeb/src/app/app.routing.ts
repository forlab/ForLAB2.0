/**
 * Created by griga on 7/11/16.
 */

import { Routes, RouterModule } from "@angular/router";
import { MainLayoutComponent } from "./shared/layout/app-layouts/main-layout.component";
import { AuthLayoutComponent } from "./shared/layout/app-layouts/auth-layout.component";
import { ModuleWithProviders } from "@angular/core";
import { AuthGuard } from "./guards/auth-guard.service";

export const routes: Routes = [
  {
    path: "",
    component: AuthLayoutComponent,
    loadChildren: "app/+auth/auth.module#AuthModule",
  },
  {
    path: "",
    component: MainLayoutComponent,
    data: { pageTitle: "Home" },
    children: [
      {
        path: "",
        redirectTo: "Dashboard",
        pathMatch: "full",
      },
      {
        path: "home",
        loadChildren: "app/+home/home.module#HomeModule",
        data: { pageTitle: "Home" },
      },
      {
        path: "Dashboard",
        loadChildren: "app/+Dashboard/Dashboard.module#DashboardModule",
        data: { pageTitle: "Dashboard" },
      },
      {
        path: "Managedata",
        loadChildren: "app/+Managedata/Managedata.module#managedataModule",
        data: { pageTitle: "Manage Data" },
      },
      {
        path: "ConstructMorbidity",
        loadChildren:
          "app/+ConstructMorbidity/ConstructMorbidity.module#ConstructMorbidityModule",
        data: { pageTitle: "Construct Morbidity" },
      },
      {
        path: "ConductForecast",
        loadChildren:
          "app/+ConductForecast/ConductForecast.module#ConductForecastModule",
        data: { pageTitle: "Conduct Forecast" },
      },
      {
        path: "ImportData",
        loadChildren: "app/+ImportData/ImportData.module#ImportDataModule",
        data: { pageTitle: "Import Data" },
      },

      // {path: 'dashboard', loadChildren: 'app/+dashboard/dashboard.module#DashboardModule',data:{pageTitle: 'Dashboard'}},
      // {path: 'smartadmin', loadChildren: 'app/+smartadmin-intel/smartadmin-intel.module#SmartadminIntelModule',data:{pageTitle: 'Smartadmin'}},
      // {path: 'app-views', loadChildren: 'app/+app-views/app-views.module#AppViewsModule',data:{pageTitle: 'App Views'}},
      // {path: 'calendar', loadChildren: 'app/+calendar/calendar.module#CalendarModule',data:{pageTitle: 'Calendar'}},
      // {path: 'e-commerce', loadChildren: 'app/+e-commerce/e-commerce.module#ECommerceModule',data:{pageTitle: 'E-commerce'}},
      // {path: 'forms', loadChildren: 'app/+forms/forms-showcase.module#FormsShowcaseModule',data:{pageTitle: 'Forms'}},
      // {path: 'graphs', loadChildren: 'app/+graphs/graphs-showcase.module#GraphsShowcaseModule',data:{pageTitle: 'Graphs'}},
      // {path: 'maps', loadChildren: 'app/+maps/maps.module#MapsModule',data:{pageTitle: 'Maps'}},
      {
        path: "miscellaneous",
        loadChildren:
          "app/+miscellaneous/miscellaneous.module#MiscellaneousModule",
        data: { pageTitle: "Miscellaneous" },
      },
      {
        path: "SearchProduct",
        loadChildren:
          "app/+SearchProduct/SearchProduct.module#SearchProductModule",
        data: { pageTitle: "SearchProduct" },
      },
      {
        path: "SearchSite",
        loadChildren: "app/+SearchSite/SearchSite.module#SearchSiteModule",
        data: { pageTitle: "SearchSite" },
      },

      // {path: 'Site', loadChildren: 'app/+Site/Site.module#SiteModule',data:{pageTitle: 'Site'}},
      {
        path: "Demographic",
        loadChildren: "app/+Demographic/Demographic.module#DemograhicModule",
        data: { pageTitle: "Demographic" },
      },
      {
        path: "Consumption",
        loadChildren: "app/+Consumption/Consumption.module#ConsumptionModule",
        data: { pageTitle: "Consumption" },
      },
      {
        path: "ServiceStatistic",
        loadChildren:
          "app/+ServiceStatistic/ServiceStatistic.module#ServiceModule",
        data: { pageTitle: "Service Statistic" },
      },
      {
        path: "Report",
        loadChildren: "app/+Report/Report.module#ReportModule",
        data: { pageTitle: "Report" },
      },
      {
        path: "CopyDefaultData",
        loadChildren:
          "app/+copydefaultdata/copydefaultdata.module#copydefaultdataModule",
        data: { pageTitle: "Import Default Data" },
      },
      {
        path: "cmspage",
        loadChildren: "app/+cmspagenew/cmspagenew.module#cmspagenewModule",
        data: { pageTitle: "CMS Page" },
      },
      {
        path: "Approvemasterdata",
        loadChildren:
          "app/+pendingapprovalentry/pendingapprovalentry.module#PendingapprovalModule",
        data: { pageTitle: "CMS Page" },
      },
    ],

    canActivate: [AuthGuard],
  },
];

export const routing: ModuleWithProviders = RouterModule.forRoot(routes, {
  useHash: true,
});
