
import { BaseURL } from '../config';

export const UserSubscriptionsController = {
  GetAllUserCountrySubscriptions:  BaseURL + `/api/UserSubscriptions/GetAllUserCountrySubscriptions`,
  GetAllUserRegionSubscriptions: BaseURL + `/api/UserSubscriptions/GetAllUserRegionSubscriptions`,
  GetAllUserLaboratorySubscriptions: BaseURL + `/api/UserSubscriptions/GetAllUserLaboratorySubscriptions`,
  GetUserCountriesAsDrp: BaseURL + `/api/UserSubscriptions/GetUserCountriesAsDrp`,
  GetUserRegionsAsDrp: BaseURL + `/api/UserSubscriptions/GetUserRegionsAsDrp`,
  GetUserLaboratoriesAsDrp: BaseURL + `/api/UserSubscriptions/GetUserLaboratoriesAsDrp`,
}