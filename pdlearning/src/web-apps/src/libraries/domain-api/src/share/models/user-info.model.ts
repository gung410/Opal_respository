import { Guid, Utils } from '@opal20/infrastructure';
import { IEntityStatusModel, IIdentityModel } from './identity.model';

import { Designation } from './designation.model';
import { JobFamily } from './job-family.model';
import { TeachingLevel } from './teaching-level.model';
import { TeachingSubject } from './teaching-subject.model';
export enum SystemRoleEnum {
  DivisionAdministrator = 'divisionadmin',
  BranchAdministrator = 'branchadmin',
  SchoolAdministrator = 'schooladmin',
  OPJApprovingOfficer = 'approvingofficer',
  CourseApprovingOfficer = 'reportingofficer',
  Learner = 'learner',
  SystemAdministrator = 'overallsystemadministrator',
  SchoolContentApprovingOfficer = 'schoolcontentapprovingofficer',
  MOEHQContentApprovingOfficer = 'MOEHQcontentapprovingofficer',
  WebPageEditor = 'webpageeditor',
  CoursePlanningCoordinator = 'courseplanningcoordinator',
  CourseContentCreator = 'coursecontentcreator',
  CourseAdministrator = 'courseadmin',
  CourseFacilitator = 'coursefacilitator',
  ContentCreator = 'contentcreator',
  SchoolStaffDeveloper = 'schooltrainingcoordinator',
  UserAccount = 'useraccountadministrator',
  DivisionTrainingCoordinator = 'divisiontrainingcoordinator'
}

export const ADMINISTRATOR_ROLES: Array<SystemRoleEnum> = [
  SystemRoleEnum.SystemAdministrator,
  SystemRoleEnum.DivisionAdministrator,
  SystemRoleEnum.BranchAdministrator,
  SystemRoleEnum.SchoolAdministrator
];

export interface IJsonDynamicAttributesModel {
  avatarUrl?: string;
}

export interface ISystemRolesModel {
  identity: IIdentityModel;
}

export interface IApprovingOfficerGroup {
  departmentId: number;
  userId: number;
  userIdentity: IUserIdentity;
  name: string;
  type: ApprovalGroupType;
}

export enum ApprovalGroupType {
  PrimaryApprovalGroup = 'PrimaryApprovalGroup',
  AlternativeApprovalGroup = 'AlternativeApprovalGroup'
}

export interface IUserIdentity {
  extId: string;
  ownerId: number;
  customerId: number;
  archetype: string;
  id: number;
}

export interface IBaseUserInfo {
  id: string;
  firstName?: string;
  lastName?: string;
  fullName?: string;
  avatarUrl?: string;
  emailAddress: string;
  userCxId?: string;
  departmentId?: number;
  departmentName?: string;
}

export class BaseUserInfo {
  public firstName: string;
  public lastName: string;
  public avatarUrl: string;
  public emailAddress: string;
  public departmentId: number;
  public departmentName: string;

  public get id(): string {
    return this.userCxId;
  }

  public get fullName(): string {
    if (this._fullName != null) {
      return this._fullName;
    }
    return `${this.firstName || ''} ${this.lastName || ''}`;
  }

  protected userCxId: string = '';
  private _fullName: string;

  constructor(data?: IBaseUserInfo) {
    if (data != null) {
      this.firstName = data.firstName;
      this.firstName = data.firstName;
      this.userCxId = data.userCxId.toLowerCase();
      this.avatarUrl = data.avatarUrl;
      this.emailAddress = data.emailAddress;
      this.departmentId = data.departmentId;
      this.departmentName = data.departmentName;
      this._fullName = data.fullName;
    }
  }
}

export interface IUserInfoModel extends IBaseUserInfo {
  departmentId: number;
  departmentName: string;
  identity: IIdentityModel;
  entityStatus: IEntityStatusModel;
  firstName: string;
  lastName: string;
  fullName?: string;
  emailAddress: string;
  systemRoles: ISystemRolesModel[];
  groups: IApprovingOfficerGroup[];
  jsonDynamicAttributes: IJsonDynamicAttributesModel;
}

export class UserInfoModel {
  public get fullName(): string {
    if (this._fullName != null) {
      return this._fullName;
    }
    return `${this.firstName || ''} ${this.lastName || ''}`;
  }

  public get avatarUrl(): string {
    return this.jsonDynamicAttributes ? this.jsonDynamicAttributes.avatarUrl : '';
  }
  public get emails(): string {
    return this.emailAddress;
  }
  public get id(): string {
    return this.extId;
  }

  public extId: string;
  public departmentId: number;
  public departmentName: string;
  public firstName: string;
  public lastName: string;
  public emailAddress: string;
  public phone: string = '';
  public systemRoles: SystemRoleEnum[] = [];
  public groups: IApprovingOfficerGroup[] = [];
  public jsonDynamicAttributes: IJsonDynamicAttributesModel = {};
  public permissionDic: IPermissionDictionary;

  private _fullName?: string;

  public static getPermissionDic(): IPermissionDictionary {
    if (AppGlobal.permissions == null) {
      return {};
    }
    return (
      AppGlobal.permissions.reduce((acc: { [actionKey: string]: IModulePermission }, value: IModulePermission) => {
        if (value.action) {
          acc[value.action] = value;
        }
        return acc;
      }, {}) || {}
    );
  }

  public static getMyUserInfo(): UserInfoModel {
    return new UserInfoModel({
      id: AppGlobal.user.extId.toLocaleLowerCase(),
      emailAddress: AppGlobal.user.emails,
      identity: { extId: AppGlobal.user.extId.toLocaleLowerCase(), id: AppGlobal.user.extId.toLocaleLowerCase() },
      firstName: '',
      lastName: '',
      fullName: AppGlobal.user.fullName,
      jsonDynamicAttributes: {
        avatarUrl: AppGlobal.user.avatarUrl
      },
      departmentId: AppGlobal.user.departmentId,
      departmentName: AppGlobal.user.departmentName,
      entityStatus: { externallyMastered: false },
      systemRoles: AppGlobal.user.systemRoles,
      groups: AppGlobal.user.approvingOfficerGroups ? AppGlobal.user.approvingOfficerGroups : AppGlobal.user.groups
    });
  }

  constructor(data?: IUserInfoModel) {
    if (data != null) {
      this.extId = data.identity ? data.identity.extId.toLowerCase() : '';
      this.firstName = data.firstName;
      this.lastName = data.lastName;
      this._fullName = data.fullName;
      this.emailAddress = data.emailAddress;
      this.departmentId = data.departmentId;
      this.departmentName = data.departmentName;
      this.systemRoles = data.systemRoles ? data.systemRoles.map(x => <SystemRoleEnum>x.identity.extId) : [];
      this.jsonDynamicAttributes = data.jsonDynamicAttributes;
      this.groups = data.groups ? data.groups : [];
      this.permissionDic = UserInfoModel.getPermissionDic();
    }
  }

  public hasRole(...roles: SystemRoleEnum[]): boolean {
    return Utils.includesAny(this.systemRoles, roles);
  }

  public hasOnlyRole(role: SystemRoleEnum): boolean {
    return Utils.includesAny(this.systemRoles, [role]) && this.systemRoles.length === 1;
  }

  public hasPermission(permissionKey: string): boolean {
    const permission: IModulePermission = this.permissionDic[permissionKey];

    return permission ? permission.grantedType === 'Allow' : false;
  }

  public hasPermissionPrefix(permissionKeyPrefix: string): boolean {
    const userPermissionKeys = Object.keys(this.permissionDic);
    for (let i = 0; i < userPermissionKeys.length; i++) {
      const userPermissionKey = userPermissionKeys[i];
      if (userPermissionKey.startsWith(permissionKeyPrefix)) {
        return this.hasPermission(userPermissionKey);
      }
    }
    return false;
  }

  public hasPermissions(permissionKeys: string[]): boolean {
    return permissionKeys.every(key => this.hasPermission(key));
  }

  public hasAdministratorRoles(): boolean {
    return this.hasRole(...ADMINISTRATOR_ROLES);
  }

  public hasValue(term: string, filterFn: (item: UserInfoModel) => string): boolean {
    const value = filterFn(this);
    return value && value.toLowerCase().includes(term.toLowerCase());
  }
}

export interface IPublicUserInfo extends IBaseUserInfo {
  fullName: string;
  userCxId: string;
  avatarUrl?: string;
  lastLoginDate?: Date;
  departmentId: number;
  emailAddress: string;
  departmentName: string;
  designation?: string;
  teachingSubjects?: string[];
  teachingLevels?: string[];
  jobFamilies?: string[];
  typeOfOrganization?: string;
  organizationAddress?: string;
  portfolios?: string[];
  serviceScheme?: string;
  teachingCourseOfStudy?: string[];
  coCurricularActivities?: string[];
  areasOfProfessionalInterest?: string[];
  roleSpecificProficiencies?: string[];
  notificationPreferences?: string[];
  isFollowing?: boolean;
  developmentRoles?: string[];
}

export class PublicUserInfo {
  public fullName: string = '';
  public userCxId: string = '';
  public avatarUrl?: string;
  public lastLoginDate?: Date;
  public departmentId: number;
  public emailAddress: string;
  public departmentName: string;
  public designation?: string;
  public teachingSubjects?: string[] = [];
  public teachingLevels?: string[] = [];
  public jobFamilies?: string[] = [];
  public typeOfOrganization?: string;
  public organizationAddress?: string;
  public portfolios?: string[] = [];
  public serviceScheme?: string;
  public teachingCourseOfStudy?: string[] = [];
  public coCurricularActivities?: string[] = [];
  public areasOfProfessionalInterest?: string[] = [];
  public roleSpecificProficiencies?: string[] = [];
  public notificationPreferences?: string[] = [];
  public isFollowing?: boolean;
  public developmentRoles?: string[] = [];

  public get extId(): string {
    return this.userCxId;
  }
  public get id(): string {
    return this.userCxId;
  }
  public get isHRMS(): boolean {
    return !Guid.isGuid(this.designation);
  }
  public get isNonHRMS(): boolean {
    return Guid.isGuid(this.designation);
  }

  constructor(data?: IPublicUserInfo) {
    if (data != null) {
      this.fullName = data.fullName;
      this.userCxId = data.userCxId.toLowerCase();
      this.avatarUrl = data.avatarUrl;
      this.lastLoginDate = data.lastLoginDate;
      this.departmentId = data.departmentId;
      this.emailAddress = data.emailAddress;
      this.departmentName = data.departmentName;
      this.designation = data.designation;
      this.teachingSubjects = data.teachingSubjects ? data.teachingSubjects : [];
      this.teachingLevels = data.teachingLevels ? data.teachingLevels : [];
      this.jobFamilies = data.jobFamilies ? data.jobFamilies : [];
      this.typeOfOrganization = data.typeOfOrganization;
      this.organizationAddress = data.organizationAddress;
      this.portfolios = data.portfolios ? data.portfolios : [];
      this.serviceScheme = data.serviceScheme;
      this.teachingCourseOfStudy = data.teachingCourseOfStudy ? data.teachingCourseOfStudy : [];
      this.coCurricularActivities = data.coCurricularActivities ? data.coCurricularActivities : [];
      this.areasOfProfessionalInterest = data.areasOfProfessionalInterest ? data.areasOfProfessionalInterest : [];
      this.roleSpecificProficiencies = data.roleSpecificProficiencies ? data.roleSpecificProficiencies : [];
      this.notificationPreferences = data.notificationPreferences ? data.notificationPreferences : [];
      this.developmentRoles = data.developmentRoles ? data.developmentRoles : [];
    }
  }

  public getAllMetadataIds(): string[] {
    return [].concat(
      this.teachingSubjects,
      this.jobFamilies,
      this.teachingLevels,
      this.teachingCourseOfStudy,
      this.coCurricularActivities,
      this.serviceScheme != null ? [this.serviceScheme] : [],
      this.developmentRoles
    );
  }

  public getDesignationDisplayText(allDesignationsDic: Dictionary<Designation>): string {
    return this.isNonHRMS && this.designation && allDesignationsDic[this.designation]
      ? allDesignationsDic[this.designation].displayText
      : this.designation;
  }

  public getJobFalimyDisplayText(jobFamilyDic: Dictionary<JobFamily>): string[] {
    return this.jobFamilies && jobFamilyDic && Utils.countDictionaryKey(jobFamilyDic)
      ? Object.values(jobFamilyDic)
          .filter(jobFamily => this.jobFamilies.includes(jobFamily.id))
          .map(jobFamily => jobFamily.displayText)
      : [];
  }

  public getTeachingSubjectDisplayText(teachingSubjectDic: Dictionary<TeachingSubject>): string[] {
    return this.teachingSubjects && teachingSubjectDic && Utils.countDictionaryKey(teachingSubjectDic)
      ? Object.values(teachingSubjectDic)
          .filter(teachingSubject => this.teachingSubjects.includes(teachingSubject.id))
          .map(teachingSubject => teachingSubject.displayText)
      : [];
  }

  public getTeachingLevelDisplayText(teachingLevelDic: Dictionary<TeachingLevel>): string[] {
    return this.teachingLevels && teachingLevelDic && Utils.countDictionaryKey(teachingLevelDic)
      ? Object.values(teachingLevelDic)
          .filter(teachingLevel => this.teachingLevels.includes(teachingLevel.id))
          .map(teachingLevel => teachingLevel.displayText)
      : [];
  }

  public getAccountTypeDisplayText(): string {
    return this.isHRMS ? 'HRMS' : 'External';
  }
}
