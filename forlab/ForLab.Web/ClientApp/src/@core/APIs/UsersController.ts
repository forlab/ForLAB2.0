
import { BaseURL } from '../config';

export const UsersController = {
  GetAllUsers:  BaseURL + `/api/Users/GetAllUsers`,
  GetAllUsersAsDrp: BaseURL + `/api/Users/GetAllUsersAsDrp`,
  GetUserDetails: BaseURL + `/api/Users/GetUserDetails`,
  CreateUser: BaseURL + `/api/Users/CreateUser`,
  Register: BaseURL + `/api/Users/Register`,
  UpdateUser: BaseURL + `/api/Users/UpdateUser`,
  GetAllRoles: BaseURL + `/api/Users/GetAllRoles`,
  UpdateUserStatus: BaseURL + `/api/Users/UpdateUserStatus`,
  MandatoryChangePassword: BaseURL + `/api/Users/MandatoryChangePassword`,
  ExportUsers: BaseURL + `/api/Users/ExportUsers`,
}