import { BaseEntityDto } from '../common/BaseEntityDto';
import { BaseFilter } from '../common/BaseFilter';
import { UserCountrySubscriptionDto, UserRegionSubscriptionDto, UserLaboratorySubscriptionDto } from './UserSubscription';

export class UserDto extends BaseEntityDto {
    id?: number;
    firstName: string = null; 
    lastName: string = null;
    address: string = null;
    personalImagePath?: string; 
    ip?: string = null;
    changePassword?: boolean;
    callingCode: string = null;
    jobTitle: string = null;
    status: string = null;
    electronicSignature: string = null;
    nextPasswordExpiryDate: string = null;
    emailVerifiedDate: string = null;
    phoneNumber: string = null; 
    email: string = null;
    password: string = null;
    emailConfirmed: boolean = null;
    phoneNumberConfirmed: boolean = null;
    
    userRoles?: string[] = [];
    regionId: number = null;
    userSubscriptionLevelId: number = null;
    userCountrySubscriptionDtos: UserCountrySubscriptionDto[] = [];
    userRegionSubscriptionDtos: UserRegionSubscriptionDto[] = [];
    userLaboratorySubscriptionDtos: UserLaboratorySubscriptionDto[] = [];

    // UI
    userSubscriptionLevelName: string = null;
    reomveProfileImage: boolean = false;
}

export class UserDetailsDto extends BaseEntityDto {
    id?: number;
    firstName: string = null; 
    lastName: string = null;
    address: string = null;
    personalImagePath?: string; 
    ip?: string = null;
    changePassword?: boolean;
    callingCode: string = null;
    jobTitle: string = null;
    status: string = null;
    electronicSignature: string = null;
    nextPasswordExpiryDate: string = null;
    emailVerifiedDate: string = null;
    phoneNumber: string = null; 
    email: string = null;
    emailConfirmed: boolean = null;
    phoneNumberConfirmed: boolean = null;

    userRoles?: string[] = [];

    userSubscriptionLevelId: number = null;
    userCountrySubscriptionDtos: UserCountrySubscriptionDto[] = [];
    userRegionSubscriptionDtos: UserRegionSubscriptionDto[] = [];
    userLaboratorySubscriptionDtos: UserLaboratorySubscriptionDto[] = [];
}

export class UserFilterDto extends BaseFilter{
    name?: string = null;
    email?: string = null;
    phoneNumber: string = null; 
    roleId?: number = null;
    jobTitle: string = null;
    status: string = null;
    userSubscriptionLevelId: number = null;
}



