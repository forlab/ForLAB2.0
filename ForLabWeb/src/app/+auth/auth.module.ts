import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { routing } from "./auth.routing";
import { AuthComponent } from "./auth.component";
import { FormsModule } from "@angular/forms";
import { APIwithActionService } from "../shared/APIwithAction.service";
import { GlobalAPIService } from "../shared/GlobalAPI.service";
import { SmartadminModule } from "../shared/smartadmin.module";
import { ReactiveFormsModule } from "@angular/forms";
import { VerifylinkComponent } from "./verifylink/verifylink.component";
import { ResetpasswordComponent } from "./resetpassword/resetpassword.component";
import { SharedequalModule } from "../shared/Equalvalidateshared.module";
import { NgxLoadingModule } from "ngx-loading";
import { LandingpagenewComponent } from "./landingpagenew/landingpagenew.component";
import { LoginModalComponent } from "./loginmodal/loginmodal.component";
import { RegisterModalComponent } from "./registermodal/registermodal.component";
import { BsDropdownModule } from "ngx-bootstrap";
import { ForgotpwdModalComponent } from "./forgotpwdmodal/forgotpwdmodal.component";
@NgModule({
  imports: [
    CommonModule,
    SharedequalModule,
    ReactiveFormsModule,
    routing,
    FormsModule,
    NgxLoadingModule.forRoot({}),
    SmartadminModule,
    BsDropdownModule.forRoot(),
  ],
  declarations: [
    AuthComponent,
    LoginModalComponent,
    RegisterModalComponent,
    ForgotpwdModalComponent,
    VerifylinkComponent,
    ResetpasswordComponent,
    LandingpagenewComponent,
  ],
  entryComponents: [AuthComponent],
  providers: [APIwithActionService, GlobalAPIService],
})
export class AuthModule {}
