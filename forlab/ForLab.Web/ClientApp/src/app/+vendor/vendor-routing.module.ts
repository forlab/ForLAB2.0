import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// Components
import { VendorsComponent } from './vendor/vendors/vendors.component';
import { VendorContactsComponent } from './vendor-contact/vendor-contacts/vendor-contacts.component';
import { VendorDetailsComponent } from './vendor/vendor-details/vendor-details.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'vendors',
    pathMatch: 'full'
  },
  {
    path: 'vendors',
    component: VendorsComponent
  },
  {
    path: 'vendors/details/:vendorId',
    component: VendorDetailsComponent
  },
  {
    path: 'vendor-contacts',
    component: VendorContactsComponent
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class VendorRoutingModule { }
