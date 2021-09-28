import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartsModule as chartjsModule } from 'ng2-charts';
import { NgxEchartsModule } from 'ngx-echarts';
import { MorrisJsModule } from 'angular-morris-js';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
// Angualr Matrial
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatMenuModule } from '@angular/material/menu';
import { MatTableModule } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
// Routing
import { UserRoutingModule } from './user-routing.module';
import { CoreModule } from 'src/@core/core.module';
// Components
import { AddEditUserComponent } from './add-edit-user/add-edit-user.component';
import { UsersComponent } from './users/users.component';
import { ProfileComponent } from './profile/profile.component';
import { UserSubscriptionsComponent } from './user-subscriptions/user-subscriptions.component';
import { ChangeUserStatusComponent } from './users/change-user-status/change-user-status.component';

@NgModule({
  declarations: [
    AddEditUserComponent,
    UsersComponent,
    ProfileComponent,
    UserSubscriptionsComponent,
    ChangeUserStatusComponent
  ],
  imports: [
    CoreModule,
    CommonModule,
    UserRoutingModule,
    chartjsModule,
    NgxEchartsModule,
    MorrisJsModule,
    MatIconModule,
    MatButtonModule,
    MatPaginatorModule,
    MatSortModule,
    MatMenuModule,
    MatTableModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatInputModule,
    NgxDatatableModule,
    MatExpansionModule,
    MatTooltipModule
  ]
})
export class UserModule { }