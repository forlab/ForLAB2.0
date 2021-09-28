export class ConfigurationDto {
  id: number = null;
  // User Managment
  numOfDaysToChangePassword: number = null;
  accountLoginAttempts: number = null;
  userPhotosize: number = null;
  passwordExpiryTime: number = null;
  timeToSessionTimeOut: number = null;
  attachmentsMaxSize: number = null;
  timesCountBeforePasswordReuse: number = null;
}