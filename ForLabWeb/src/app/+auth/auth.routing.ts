import { Routes, RouterModule } from "@angular/router";
import { AuthComponent } from "./auth.component";
import { VerifylinkComponent } from "./verifylink/verifylink.component";
import { ForgotComponent } from "./+forgot/forgot.component";
import { ResetpasswordComponent } from "./resetpassword/resetpassword.component";
import { LandingpagenewComponent } from "./landingpagenew/landingpagenew.component";
export const routes: Routes = [
  {
    path: "",
    component: AuthComponent,
    children: [
      {
        path: "",
        redirectTo: "landingpage",
        pathMatch: "full",
      },

      {
        path: "landingpage",
        component: LandingpagenewComponent,
      },
      {
        path: "verifylink/:id",
        component: VerifylinkComponent,
      },
      {
        path: "resetpassword/:id",
        component: ResetpasswordComponent,
      },
    ],
  },
];

export const routing = RouterModule.forChild(routes);
