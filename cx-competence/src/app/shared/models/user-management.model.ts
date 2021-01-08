import { AppConstant } from '../app.constant';
import { Identity } from './common.model';
import { EntityStatus } from './entity-status.model';
import { LoginServiceClaim } from './login-service-claims.model';
export class UserManagement {
  public departmentName?: string;
  public departmentAddress?: string;
  public departmentId?: number;
  public groups?: any[];
  public firstName?: string;
  public mobileCountryCode?: number;
  public mobileNumber?: string;
  public ssn?: string;
  public gender?: number;
  public dateOfBirth?: any;
  public emailAddress?: string;
  public tag?: string;
  public identity?: Identity;
  public dynamicAttributes?: any[];
  public entityStatus?: EntityStatus;
  public roles?: any[];
  public systemRoles?: any[];
  public personnelGroups?: any[];
  public experienceCategorys?: any[];
  public careerPaths?: any[];
  public developmentalRoles?: any[];
  public idpLocked?: boolean;
  public resetOtp?: string;
  public otpValue?: string;
  public created?: any;
  public loginServiceClaims?: LoginServiceClaim[];
  public forceLoginAgain?: boolean;
  public jsonDynamicAttributes?: any;
  constructor(data?: Partial<UserManagement>) {
    if (!data) {
      return;
    }
    this.departmentId = data.departmentId;
    this.departmentName = data.departmentName;
    this.departmentAddress = data.departmentAddress;
    this.groups = data.groups;
    this.firstName = data.firstName;
    this.mobileCountryCode = data.mobileCountryCode;
    this.mobileNumber = data.mobileNumber;
    this.ssn = data.ssn;
    this.gender = data.gender;
    this.dateOfBirth = data.dateOfBirth;
    this.emailAddress = data.emailAddress;
    this.tag = data.tag;
    this.identity = data.identity;
    this.dynamicAttributes = data.dynamicAttributes;
    this.entityStatus = data.entityStatus;
    this.roles = data.roles;
    this.systemRoles = data.systemRoles;
    this.experienceCategorys = data.experienceCategorys;
    this.personnelGroups = data.personnelGroups;
    this.developmentalRoles = data.developmentalRoles;
    this.careerPaths = data.careerPaths;
    this.idpLocked = data.idpLocked;
    this.resetOtp = data.resetOtp;
    this.otpValue = data.otpValue;
    this.created = data.created;
    this.loginServiceClaims = data.loginServiceClaims;
    this.forceLoginAgain = data.forceLoginAgain;
    this.jsonDynamicAttributes = data.jsonDynamicAttributes;
  }
}
export class UserManagementQueryModel {
  public parentDepartmentId?: number[];
  public userIds?: number[];
  public userArchetypes?: string[];
  public extIds?: string[];
  public usertypeIds?: number[];
  public loginServiceClaims?: string[];
  public userEntityStatuses?: string[];
  public getUserGroups: boolean;
  public filterOnParentHd?: boolean;
  public getRoles: boolean;
  public getDeapartments: boolean;
  public pageIndex?: number;
  public pageSize?: number;
  public orderBy?: string;
  public searchKey?: string;
  public jsonDynamicData?: string[];
  public getLoginServiceClaims?: boolean;
  public externallyMastered?: boolean;
  public multiUserTypefilters?: string[][];
  public ageRanges: any[];
  public exportFields: any;
  public exportFileName: string;
  public createdAfter: string;
  public createdBefore: string;
  public expirationDateAfter: string;
  public expirationDateBefore: string;
  public orgUnittypeIds: any[];
  public filterOnSubDepartment: boolean;

  constructor(data?: Partial<UserManagementQueryModel>) {
    if (!data) {
      this.pageIndex = 1;
      this.pageSize = AppConstant.ItemPerPage;
      return;
    }
    this.parentDepartmentId = data.parentDepartmentId
      ? data.parentDepartmentId
      : [];
    this.userIds = data.userIds ? data.userIds : [];
    this.userArchetypes = data.userArchetypes ? data.userArchetypes : [];
    this.extIds = data.extIds ? data.extIds : [];
    this.usertypeIds = data.usertypeIds ? data.usertypeIds : [];
    this.loginServiceClaims = data.loginServiceClaims
      ? data.loginServiceClaims
      : [];
    this.userEntityStatuses = data.userEntityStatuses
      ? data.userEntityStatuses
      : [];
    this.getUserGroups =
      data.getUserGroups !== null ? data.getUserGroups : undefined;
    this.filterOnParentHd =
      data.filterOnParentHd !== null ? data.filterOnParentHd : undefined;
    this.getRoles = data.getRoles !== null ? data.getRoles : undefined;
    this.getDeapartments =
      data.getDeapartments !== null ? data.getDeapartments : undefined;
    this.pageIndex = data.pageIndex ? data.pageIndex : 1;
    this.pageSize = data.pageSize ? data.pageSize : AppConstant.ItemPerPage;
    this.orderBy = data.orderBy ? data.orderBy : '';
    this.getLoginServiceClaims = data.getLoginServiceClaims
      ? data.getLoginServiceClaims
      : undefined;
    this.searchKey = data.searchKey ? data.searchKey : '';
    this.jsonDynamicData = data.jsonDynamicData ? data.jsonDynamicData : [];
    this.externallyMastered =
      data.externallyMastered !== null ? data.externallyMastered : undefined;
    this.multiUserTypefilters = data.multiUserTypefilters
      ? data.multiUserTypefilters
      : [];
    this.ageRanges = data.ageRanges ? data.ageRanges : [];
    this.exportFields = data.exportFields ? data.exportFields : undefined;
    this.exportFileName = data.exportFileName ? data.exportFileName : undefined;
    this.createdAfter = data.createdAfter ? data.createdAfter : undefined;
    this.createdBefore = data.createdBefore ? data.createdBefore : undefined;
    this.expirationDateAfter = data.expirationDateAfter
      ? data.expirationDateAfter
      : undefined;
    this.expirationDateBefore = data.expirationDateBefore
      ? data.expirationDateBefore
      : undefined;
    this.orgUnittypeIds = data.orgUnittypeIds ? data.orgUnittypeIds : undefined;
    this.filterOnSubDepartment = data.filterOnSubDepartment
      ? data.filterOnSubDepartment
      : undefined;
  }
}

export class PagingResponse {
  public hasMoreData?: boolean;
  public pageIndex?: number;
  public pageSize?: number;
  public totalItems?: number;
  constructor(data?: Partial<PagingResponse>) {
    if (!data) {
      return;
    }
    this.hasMoreData = data.hasMoreData;
    this.pageIndex = data.pageIndex ? data.pageIndex : 0;
    this.pageSize = data.pageSize ? data.pageSize : 0;
    this.totalItems = data.totalItems ? data.totalItems : 0;
  }
}

export class PagingResponseModel<T> extends PagingResponse {
  public items?: T[];
  constructor(data?: Partial<PagingResponseModel<T>>) {
    super(data);
    if (!data) {
      return;
    }
    this.items = data.items;
  }
}
