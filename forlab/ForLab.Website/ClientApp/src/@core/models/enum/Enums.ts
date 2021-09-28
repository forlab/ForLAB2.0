//#region Application
export enum ApplicationRolesEnum {
  SuperAdmin = 1,
  CountryLevel = 2,
  RegionLevel = 3,
  LaboratoryLevel = 4,
  ViewOnlyLevel = 5
}
export enum UserSubscriptionLevelsEnum {
  CountryLevel = 1,
  RegionLevel = 2,
  LaboratoryLevel = 3,
  ViewOnlyLevel = 4
}
export enum UserStatusEnum {
  Active = 1,
  NotActive = 2,
  Locked = 3
}
export enum UserTransactionTypesEnum {
  NameChanging = 1,
  IsActiveChanging = 2,
  EmailChanging = 3,
  PasswordChanging = 4,
  PhoneChanging = 5,
  AddressChanging = 6,
  AccountLock = 7,
  Login = 8,
  Logout = 9,
  ForgetPassword = 10,
  ResetPassword = 11,
}
export enum ActionOfAuditEnum {
  Create = 1,
  Update = 2,
  Activate = 3,
  Deactivate = 4,
  CreateFromDuplicate = 5
}
//#endregion

//#region Static Lookups
export enum CalculationPeriodEnum {
  OneYear = 1,
  TwoYears = 2,
}
export enum ContinentEnum {
  Asia = 1,
  Africa = 2,
  NorthAmerica = 3,
  SouthAmerica = 4,
  Antarctica = 5,
  Europe = 6,
  Australia = 7
}
export enum ControlRequirementUnitEnum {
  Daily = 1,
  Weekly = 2,
  Monthly = 3
}
export enum CountryPeriodEnum {
  Weekly = 1,
  Monthly = 2,
  Quarterly = 3,
  Annualy = 4
}
export enum EntityTypeEnum {
}
export enum ForecastConsumableUsagePeriodEnum {
  Daily = 1,
  Weekly = 2,
  Monthly = 3,
  Quarterly = 4,
  Annualy = 5
}
export enum ForecastInfoLevelEnum {
  Country = 1,
  Region = 2,
  Laboratory = 3
}
export enum ForecastInfoStatusEnum {
  Open = 1,
  Closed = 2
}
export enum ForecastMethodologyEnum {
  Service = 1,
  Consumption = 2,
  DempgraphicMorbidity = 3
}
export enum ProductBasicUnitEnum {
  KG = 1,
  GM = 2,
  CM = 3
}
export enum ProductTypeEnum {
  Reagents = 1,
  QualityControl = 2,
  Consumables = 3,
  Durables = 4,
  Calibrators = 5
}
export enum ReagentSystemEnum {
  Open = 1,
  Closed = 2,
  PartiallyOpen = 3
}
export enum ScopeOfTheForecastEnum {
  National = 1,
  Global = 2,
  ProgramBased = 3
}
export enum ThroughPutUnitEnum {
  Hourly = 1,
  Daily = 2,
}
export enum VariableTypeEnum {
  Numeric = 1,
  Percentage = 2
}
//#endregion

//#region General
export enum DaysOfWeek {
  Saturday,
  Sunday,
  Monday,
  Tuesday,
  Wednesday,
  Thursday,
  Friday
}
//#endregion