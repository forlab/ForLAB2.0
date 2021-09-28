import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// Components
import { AddEditUserComponent } from './add-edit-user/add-edit-user.component';
import { UsersComponent } from './users/users.component';
import { ProfileComponent } from './profile/profile.component';
import { AuthGuard } from 'src/@core/services/auth.guard';
import { NgxPermissionsGuard } from 'ngx-permissions';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'users',
    pathMatch: 'full',
    canActivate: [AuthGuard, NgxPermissionsGuard], 
		data: { permissions: { only: ['SuperAdmin'], redirectTo: '/' } },
  },
  {
    path: 'profile',
    component: ProfileComponent
  },
  {
    path: 'users',
    component: UsersComponent,
    canActivate: [AuthGuard, NgxPermissionsGuard], 
		data: { permissions: { only: ['SuperAdmin'], redirectTo: '/' } },
  },
  // {
  //   path: 'add',
  //   component: AddEditUserComponent
  // },
  {
    path: 'update/:userId',
    component: AddEditUserComponent,
    canActivate: [AuthGuard, NgxPermissionsGuard], 
		data: { permissions: { only: ['SuperAdmin'], redirectTo: '/' } },
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserRoutingModule { }
