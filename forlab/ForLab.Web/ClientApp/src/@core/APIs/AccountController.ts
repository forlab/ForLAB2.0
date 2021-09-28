
import { BaseURL } from '../config';

export const AccountController = {
  Login: BaseURL + `/api/Account/Login`,
  ResetPassword: BaseURL + `/api/Account/ResetPassword`,
  ForgetPassword: BaseURL + `/api/Account/ForgetPassword`,
  UpdateLoggedInUserImage: BaseURL + `/api/Account/UpdateLoggedInUserImage`,
  UpdateUserProfile: BaseURL + `/api/Account/UpdateUserProfile`,
  ChangePassword: BaseURL + `/api/Account/ChangePassword`,
  GetLoggedInUserProfile: BaseURL + `/api/Account/GetLoggedInUserProfile`,
  RefreshToken: BaseURL + `/api/Account/RefreshToken`,
  CheckSessionExpiryDate: BaseURL + `/api/Account/CheckSessionExpiryDate`,
  ConfirmEmailAddress: BaseURL + `/api/Account/ConfirmEmailAddress`,
}