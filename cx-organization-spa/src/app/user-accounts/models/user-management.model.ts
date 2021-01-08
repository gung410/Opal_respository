import { EntityStatus } from 'app-models/entity-status.model';
import { Identity } from 'app-models/identity.model';
import { LoginServiceClaim } from 'app-models/login-service-claims.model';
import { UserType } from 'app-models/user-type.model';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { SystemRole } from 'app/core/models/system-role';
import { ExportOption } from 'app/reports/models/export-option';
import { AppConstant } from 'app/shared/app.constant';
import { findIndexCommon } from 'app/shared/constants/common.const';
import { UserRoleEnum } from 'app/shared/constants/user-roles.enum';
import * as _ from 'lodash';

// tslint:disable:max-classes-per-file
export class UserManagement {
  static hasLearnerRole(systemRoles: UserType[] | undefined): boolean {
    if (!systemRoles) {
      return false;
    }

    return (
      systemRoles.findIndex((role: SystemRole) => {
        return role.identity.extId === UserRoleEnum.Learner;
      }) > findIndexCommon.notFound
    );
  }

  static hasSystemAdminRole(systemRoles: UserType[] | undefined): boolean {
    if (!systemRoles) {
      return false;
    }

    return (
      systemRoles.findIndex((role: SystemRole) => {
        return role.identity.extId === UserRoleEnum.OverallSystemAdministrator;
      }) > findIndexCommon.notFound
    );
  }

  static hasUserAccountAdminRole(systemRoles: UserType[] | undefined): boolean {
    if (!systemRoles) {
      return false;
    }

    return (
      systemRoles.findIndex((role: SystemRole) => {
        return role.identity.extId === UserRoleEnum.UserAccountAdministrator;
      }) > findIndexCommon.notFound
    );
  }

  departmentName?: string;
  departmentId?: number;
  groups?: any[];
  firstName?: string;
  mobileCountryCode?: number;
  mobileNumber?: string;
  ssn?: string;
  gender?: number;
  dateOfBirth?: any;
  emailAddress?: string;
  tag?: string;
  identity?: Identity;
  dynamicAttributes?: any[];
  entityStatus?: EntityStatus;
  roles?: any[];
  systemRoles?: UserType[];
  personnelGroups?: any[];
  careerPaths?: any[];
  idpLocked?: boolean;
  resetOtp?: string;
  otpValue?: string;
  created?: any;
  loginServiceClaims?: LoginServiceClaim[];
  forceLoginAgain?: boolean;
  learningFrameworks?: any[];
  developmentalRoles?: any[];
  jsonDynamicAttributes?: Partial<JsonDynamicAttributes>;
  avatarUrl?: string;
  constructor(data?: Partial<UserManagement>) {
    if (!data) {
      return;
    }
    this.departmentId = data.departmentId;
    this.departmentName = data.departmentName;
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
    this.personnelGroups = data.personnelGroups;
    this.careerPaths = data.careerPaths;
    this.idpLocked = data.idpLocked;
    this.resetOtp = data.resetOtp;
    this.otpValue = data.otpValue;
    this.created = data.created;
    this.loginServiceClaims = data.loginServiceClaims;
    this.forceLoginAgain = data.forceLoginAgain;
    this.learningFrameworks = data.learningFrameworks;
    this.developmentalRoles = data.developmentalRoles;
    this.jsonDynamicAttributes = data.jsonDynamicAttributes;
  }
}

export class UserManagementQueryModel {
  parentDepartmentId?: number[];
  userIds?: number[];
  userArchetypes?: string[];
  extIds?: string[];
  usertypeIds?: number[];
  loginServiceClaims?: string[];
  userEntityStatuses?: string[];
  getUserGroups?: boolean;
  userGroupIds?: number[];
  filterOnParentHd?: boolean;
  getRoles: boolean;
  getDeapartments: boolean;
  pageIndex?: number;
  pageSize?: number;
  orderBy?: string;
  searchKey?: string;
  emails?: string[];
  jsonDynamicData?: string[];
  getLoginServiceClaims?: boolean;
  externallyMastered?: boolean;
  multiUserTypefilters?: string[][];
  ageRanges: any[];
  exportOption?: ExportOption;
  createdAfter: string;
  createdBefore: string;
  expirationDateAfter: string;
  expirationDateBefore: string;
  orgUnittypeIds: any[];
  filterOnSubDepartment: boolean;
  multiUserTypeExtIdFilters: string[][];
  isCrossOrganizationalUnit: boolean;

  constructor(data?: Partial<UserManagementQueryModel>) {
    if (!data) {
      this.pageIndex = 1;
      this.pageSize = AppConstant.ItemPerPage;
      this.multiUserTypefilters = [];
      this.multiUserTypefilters.push([]);
      this.multiUserTypefilters.push([]);
      this.jsonDynamicData = [];
      this.ageRanges = [];
      this.orgUnittypeIds = [];
      this.userEntityStatuses = [];
      this.multiUserTypeExtIdFilters = [];
      this.multiUserTypeExtIdFilters.push([]);
      this.multiUserTypeExtIdFilters.push([]);

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
    this.userGroupIds = data.userGroupIds ? data.userGroupIds : undefined;
    this.getUserGroups =
      data.getUserGroups !== null ? data.getUserGroups : undefined;
    this.filterOnParentHd =
      data.filterOnParentHd !== null ? data.filterOnParentHd : undefined;
    this.getRoles = data.getRoles !== null ? data.getRoles : undefined;
    this.getDeapartments =
      data.getDeapartments !== null ? data.getDeapartments : undefined;
    this.pageIndex = data.pageIndex !== undefined ? data.pageIndex : 1;
    this.pageSize =
      data.pageSize !== undefined ? data.pageSize : AppConstant.ItemPerPage;
    this.orderBy = data.orderBy ? data.orderBy : '';
    this.getLoginServiceClaims = data.getLoginServiceClaims
      ? data.getLoginServiceClaims
      : undefined;
    this.searchKey = data.searchKey ? data.searchKey : '';
    this.emails = data.emails ? data.emails : [];
    this.jsonDynamicData = data.jsonDynamicData ? data.jsonDynamicData : [];
    this.externallyMastered =
      data.externallyMastered !== null ? data.externallyMastered : undefined;
    if (data.multiUserTypefilters) {
      this.multiUserTypefilters = data.multiUserTypefilters;
    } else {
      this.multiUserTypefilters = [];
      this.multiUserTypefilters.push([]);
      this.multiUserTypefilters.push([]);
    }
    this.ageRanges = data.ageRanges ? data.ageRanges : [];
    this.exportOption = data.exportOption ? data.exportOption : undefined;
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
    this.multiUserTypeExtIdFilters = data.multiUserTypeExtIdFilters
      ? data.multiUserTypeExtIdFilters
      : [];
    if (data.multiUserTypeExtIdFilters) {
      this.multiUserTypeExtIdFilters = data.multiUserTypeExtIdFilters;
    } else {
      this.multiUserTypeExtIdFilters = [];
      this.multiUserTypeExtIdFilters.push([]);
      this.multiUserTypeExtIdFilters.push([]);
    }
    this.isCrossOrganizationalUnit = data.isCrossOrganizationalUnit;
  }

  preProcessSpecialFields?(): UserManagementQueryModel {
    const result = _.cloneDeep(this);
    if (result.createdAfter) {
      result.createdAfter = DateTimeUtil.surveyToDateLocalTime(
        result.createdAfter
      ).toISOString();
    }
    if (result.createdBefore) {
      result.createdBefore = DateTimeUtil.getEndDate(
        DateTimeUtil.surveyToDateLocalTime(result.createdBefore)
      ).toISOString();
    }
    if (result.expirationDateAfter) {
      result.expirationDateAfter = DateTimeUtil.surveyToDateLocalTime(
        result.expirationDateAfter
      ).toISOString();
    }
    if (result.expirationDateBefore) {
      result.expirationDateBefore = DateTimeUtil.getEndDate(
        DateTimeUtil.surveyToDateLocalTime(result.expirationDateBefore)
      ).toISOString();
    }

    return result;
  }
}

export class PagingResponseModel<T> {
  hasMoreData?: boolean;
  items?: T[];
  pageIndex?: number;
  pageSize?: number;
  totalItems?: number;
  constructor(data?: Partial<PagingResponseModel<T>>) {
    if (!data) {
      return;
    }
    this.hasMoreData = data.hasMoreData;
    this.items = data.items;
    this.pageIndex = data.pageIndex;
    this.pageSize = data.pageSize;
    this.totalItems = data.totalItems;
  }
}

export class JsonDynamicAttributes {
  signupReason: any;
  finishOnBoarding: any;
  identityType: any;
  dateJoinedMinistry: any;
  designation: any;
  portfolio: any;
  roleSpecificProficiencies: any;
  teachingSubjects: any;
  teachingLevels: any;
  jobFamilies: any;
  cocurricularActivities: any;
  professionalInterestArea: any;
  teachingCourseOfStudy: any;
  titleSalutation: any;
  avatarUrl: string;
  manualOrganizationUnitType: any;
  manualOrganizationUnitAddress: string;
  manualOrganizationUnitZone: any;
  manualOrganizationUnitCluster: any;
  manualOrganizationUnitName: string;
  isFromExistingPlaceOfWork: any;
  notificationPreference: any;
  holdsSupervisoryRole: any;
  personalStorageSize: any;
  isStorageUnlimited: any;
}
