import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { ChartsModule as chartjsModule } from 'ng2-charts';
import { NgxEchartsModule } from 'ngx-echarts';
import { MorrisJsModule } from 'angular-morris-js';
// Angualr Matrial
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatMenuModule } from '@angular/material/menu';
import { MatTableModule } from '@angular/material/table';
// Routing
import { DashboardRoutingModule } from './dashboard-routing.module';
import { CoreModule } from 'src/@core/core.module';
// Components
import { MainComponent } from './main/main.component';
import { ForecastDashboardComponent } from './forecast-dashboard/forecast-dashboard.component';
import { ForecastNumbersComponent } from './forecast-dashboard/forecast-numbers/forecast-numbers.component';

@NgModule({
  declarations: [MainComponent, ForecastDashboardComponent, ForecastNumbersComponent],
  imports: [
    CoreModule,
    CommonModule,
    DashboardRoutingModule,
    chartjsModule,
    NgxEchartsModule,
    MorrisJsModule,
    PerfectScrollbarModule,
    MatIconModule,
    MatButtonModule,
    MatPaginatorModule,
    MatSortModule,
    MatMenuModule,
    MatInputModule,
    MatFormFieldModule,
    MatTableModule
  ]
})
export class DashboardModule {}
