import { PDCatalogueConstant } from './pd-catalogue.enum';

export enum TrackedUserFieldEnum {
  DepartmentName = 'departmentName',

  FirstName = 'firstName',
  MobileCountryCode = 'mobileCountryCode',
  MobileNumber = 'mobileNumber',
  Ssn = 'ssn',
  Gender = 'gender',
  DateOfBirth = 'dateOfBirth',
  EmailAddress = 'emailAddress',

  SystemRoles = 'systemRoles',
  PersonnelGroups = 'personnelGroups',
  CareerPaths = 'careerPaths',

  ActiveDate = 'entityStatus.activeDate',
  ExpirationDate = 'entityStatus.expirationDate',

  DevelopmentalRoles = 'developmentalRoles',
  LearningFrameworks = 'learningFrameworks',
  Salutation = 'jsonDynamicAttributes.titleSalutation',
  Portfolio = 'jsonDynamicAttributes.portfolio',
  TeachingStudy = 'jsonDynamicAttributes.teachingCourseOfStudy',
  TeachingLevels = 'jsonDynamicAttributes.teachingLevels',
  TeachingSubjects = 'jsonDynamicAttributes.teachingSubjects',
  CocurricularActivities = 'jsonDynamicAttributes.cocurricularActivities',
  JobFamily = 'jsonDynamicAttributes.jobFamily',
  Designation = 'jsonDynamicAttributes.designation',
  RoleSpecificProficiencies = 'jsonDynamicAttributes.roleSpecificProficiencies',
  ProfessionalInterestArea = 'jsonDynamicAttributes.professionalInterestArea',
  personalStorageSize = 'jsonDynamicAttributes.personalStorageSize',

  ReportingOfficer = 'reportingOfficer',

  EntityStatus = 'entityStatusId'
}

// tslint:disable:variable-name
export const UserFieldNameConstant = {
  [TrackedUserFieldEnum.DepartmentName]:
    'User_Account_Page.Audit_History.User_Fields.Department_Name',
  [TrackedUserFieldEnum.FirstName]:
    'User_Account_Page.Audit_History.User_Fields.First_Name',
  [TrackedUserFieldEnum.MobileCountryCode]:
    'User_Account_Page.Audit_History.User_Fields.Mobile_Code',
  [TrackedUserFieldEnum.MobileNumber]:
    'User_Account_Page.Audit_History.User_Fields.Mobile_Number',
  [TrackedUserFieldEnum.Ssn]: 'User_Account_Page.Audit_History.User_Fields.SSN',
  [TrackedUserFieldEnum.Gender]:
    'User_Account_Page.Audit_History.User_Fields.Gender',
  [TrackedUserFieldEnum.DateOfBirth]:
    'User_Account_Page.Audit_History.User_Fields.Date_of_Birth',
  [TrackedUserFieldEnum.EmailAddress]:
    'User_Account_Page.Audit_History.User_Fields.Email',
  [TrackedUserFieldEnum.SystemRoles]:
    'User_Account_Page.Audit_History.User_Fields.System_Roles',
  [TrackedUserFieldEnum.PersonnelGroups]:
    'User_Account_Page.Audit_History.User_Fields.Personnel_Groups',
  [TrackedUserFieldEnum.CareerPaths]:
    'User_Account_Page.Audit_History.User_Fields.Career_Paths',
  [TrackedUserFieldEnum.EntityStatus]:
    'User_Account_Page.Audit_History.User_Fields.Status',

  [TrackedUserFieldEnum.ActiveDate]:
    'User_Account_Page.Audit_History.User_Fields.Activation_Date',
  [TrackedUserFieldEnum.ExpirationDate]:
    'User_Account_Page.Audit_History.User_Fields.Expiration_Date',
  [TrackedUserFieldEnum.DevelopmentalRoles]:
    'User_Account_Page.Audit_History.User_Fields.Developmental_Roles',
  [TrackedUserFieldEnum.LearningFrameworks]:
    'User_Account_Page.Audit_History.User_Fields.Learning_Frameworks',
  [TrackedUserFieldEnum.Salutation]:
    'User_Account_Page.Audit_History.User_Fields.Salutation',
  [TrackedUserFieldEnum.Portfolio]:
    'User_Account_Page.Audit_History.User_Fields.Portfolio',
  [TrackedUserFieldEnum.TeachingStudy]:
    'User_Account_Page.Audit_History.User_Fields.Teaching_Study',
  [TrackedUserFieldEnum.TeachingLevels]:
    'User_Account_Page.Audit_History.User_Fields.Teaching_Level',
  [TrackedUserFieldEnum.TeachingSubjects]:
    'User_Account_Page.Audit_History.User_Fields.Teaching_Subject',
  [TrackedUserFieldEnum.CocurricularActivities]:
    'User_Account_Page.Audit_History.User_Fields.Cocurricular_Activity',
  [TrackedUserFieldEnum.JobFamily]:
    'User_Account_Page.Audit_History.User_Fields.Job_Family',
  [TrackedUserFieldEnum.Designation]:
    'User_Account_Page.Audit_History.User_Fields.Designation',
  [TrackedUserFieldEnum.ProfessionalInterestArea]:
    'User_Account_Page.Audit_History.User_Fields.Areas_Professional_Interest',
  [TrackedUserFieldEnum.RoleSpecificProficiencies]:
    'User_Account_Page.Audit_History.User_Fields.Role_Specific_Proficiency',
  [TrackedUserFieldEnum.personalStorageSize]:
    'User_Account_Page.Audit_History.User_Fields.Personal_Space_Limitation'
};

export const TrackedUserUpdatedProps = [
  TrackedUserFieldEnum.DepartmentName,
  TrackedUserFieldEnum.FirstName,
  TrackedUserFieldEnum.MobileCountryCode,
  TrackedUserFieldEnum.MobileNumber,
  // TrackedUserFieldEnum.Ssn,  REMOVE SSN in the audit log.
  TrackedUserFieldEnum.Gender,
  TrackedUserFieldEnum.DateOfBirth,
  TrackedUserFieldEnum.EmailAddress,
  TrackedUserFieldEnum.SystemRoles,
  TrackedUserFieldEnum.PersonnelGroups,
  TrackedUserFieldEnum.CareerPaths,
  TrackedUserFieldEnum.personalStorageSize,

  TrackedUserFieldEnum.ActiveDate,
  TrackedUserFieldEnum.ExpirationDate,
  TrackedUserFieldEnum.DevelopmentalRoles,
  TrackedUserFieldEnum.LearningFrameworks,
  TrackedUserFieldEnum.Salutation,
  TrackedUserFieldEnum.Portfolio,
  TrackedUserFieldEnum.TeachingStudy,
  TrackedUserFieldEnum.TeachingLevels,
  TrackedUserFieldEnum.TeachingSubjects,
  TrackedUserFieldEnum.CocurricularActivities,
  TrackedUserFieldEnum.JobFamily,
  TrackedUserFieldEnum.ProfessionalInterestArea,
  TrackedUserFieldEnum.Designation,
  TrackedUserFieldEnum.RoleSpecificProficiencies
];

export enum GenderEnum {
  Male = 0,
  Female = 1
}

export enum EntityStatusEnum {
  Unknown = 0,
  Active = 1,
  Inactive = 2,
  Deactive = 3,
  Pending = 4,
  PendingApproval1st = 5,
  PendingApproval2nd = 6,
  PendingApproval3rd = 7,
  New = 8,
  Locked = 9,
  Hidden = 10,
  Rejected = 11,
  Archived = 12,
  All = 99
}

export const EntityStatusConstant = {
  [EntityStatusEnum.Unknown]: 'User_Account_Page.User_Status.Unknown',
  [EntityStatusEnum.Active]: 'User_Account_Page.User_Status.Active',
  [EntityStatusEnum.Inactive]: 'User_Account_Page.User_Status.Suspended',
  [EntityStatusEnum.Deactive]: 'User_Account_Page.User_Status.Deleted',
  [EntityStatusEnum.Pending]: 'User_Account_Page.User_Status.Pending',
  [EntityStatusEnum.PendingApproval1st]:
    'User_Account_Page.User_Status.Pending_1st_Level_Approval',
  [EntityStatusEnum.PendingApproval2nd]:
    'User_Account_Page.User_Status.Pending_2nd_Level_Approval',
  [EntityStatusEnum.PendingApproval3rd]:
    'User_Account_Page.User_Status.Pending_3rd_Level_Approval',
  [EntityStatusEnum.New]: 'User_Account_Page.User_Status.New',
  [EntityStatusEnum.All]: 'User_Account_Page.User_Status.All',
  [EntityStatusEnum.Locked]: 'User_Account_Page.User_Status.Locked'
};

export enum DatahubEventActionType {
  UserCreated = 'cx-organization-api.crud.created.employee',
  UserUpdated = 'cx-organization-api.crud.updated.employee',
  EntityStatusChanged = 'cx-organization-api.crud.entitystatus_changed.employee',
  ApprovalGroupCreated = 'cx-organization-api.crud.user_membership_created.approvalgroup',
  ApprovalGroupDeleted = 'cx-organization-api.crud.user_membership_deleted.approvalgroup',
  LoginFailed = 'cxid.system_warn.locked.user'
}

export enum AuditActionType {
  AccountCreated,
  InfoUpdated,
  EntityStatusChanged,
  PrimaryApprovalGroupChanged,
  AlternateApprovalGroupChanged,
  LoginFailed
}

export const ListOfMetadatas = [
  TrackedUserFieldEnum.Portfolio,
  TrackedUserFieldEnum.TeachingStudy,
  TrackedUserFieldEnum.TeachingLevels,
  TrackedUserFieldEnum.TeachingSubjects,
  TrackedUserFieldEnum.CocurricularActivities,
  TrackedUserFieldEnum.JobFamily,
  TrackedUserFieldEnum.ProfessionalInterestArea,
  TrackedUserFieldEnum.Designation,
  TrackedUserFieldEnum.RoleSpecificProficiencies
];

export const METADATA_ID = {
  [TrackedUserFieldEnum.Portfolio]: PDCatalogueConstant.Portfolio.id,
  [TrackedUserFieldEnum.TeachingStudy]:
    PDCatalogueConstant.TeachingCourseOfStudy.id,
  [TrackedUserFieldEnum.TeachingLevels]: PDCatalogueConstant.TeachingLevels.id,
  [TrackedUserFieldEnum.TeachingSubjects]:
    PDCatalogueConstant.TeachingSubjects.id,
  [TrackedUserFieldEnum.CocurricularActivities]:
    PDCatalogueConstant.CoCurricularActivity.id,
  [TrackedUserFieldEnum.JobFamily]: PDCatalogueConstant.JobFamily.id,
  [TrackedUserFieldEnum.ProfessionalInterestArea]:
    PDCatalogueConstant.AreasOfProfessionalInterest.id,
  [TrackedUserFieldEnum.Designation]: PDCatalogueConstant.Designation.id,
  [TrackedUserFieldEnum.RoleSpecificProficiencies]:
    PDCatalogueConstant.RoleSpecificProficiencies.id
};

export const EntityStatusMapping: Map<string, string> = new Map<string, string>(
  [
    // tslint:disable: max-line-length
    // From Pending Status
    [
      `${EntityStatusEnum.PendingApproval1st}-${EntityStatusEnum.PendingApproval2nd}`,
      `User_Account_Page.Audit_History.ActivateApprove1st`
    ],
    [
      `${EntityStatusEnum.PendingApproval1st}-${EntityStatusEnum.Rejected}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Reject1st`
    ],
    [
      `${EntityStatusEnum.PendingApproval2nd}-${EntityStatusEnum.New}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Approve2nd`
    ],
    [
      `${EntityStatusEnum.PendingApproval2nd}-${EntityStatusEnum.Rejected}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Reject2nd`
    ],
    [
      `${EntityStatusEnum.PendingApproval2nd}-${EntityStatusEnum.PendingApproval3rd}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.RequestSpecialApproval`
    ],
    [
      `${EntityStatusEnum.PendingApproval3rd}-${EntityStatusEnum.New}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Approve3rd`
    ],
    [
      `${EntityStatusEnum.PendingApproval3rd}-${EntityStatusEnum.Rejected}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Reject3rd`
    ],
    // From Active Status
    [
      `${EntityStatusEnum.Active}-${EntityStatusEnum.Inactive}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Suspend`
    ],
    [
      `${EntityStatusEnum.Active}-${EntityStatusEnum.Deactive}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Deactive`
    ],
    [
      `${EntityStatusEnum.Active}-${EntityStatusEnum.Archived}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Archive`
    ],
    // From New Status
    [
      `${EntityStatusEnum.New}-${EntityStatusEnum.Inactive}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Suspend`
    ],
    [
      `${EntityStatusEnum.New}-${EntityStatusEnum.Deactive}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Deactive`
    ],
    [
      `${EntityStatusEnum.New}-${EntityStatusEnum.Active}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.FirstLogin`
    ],
    [
      `${EntityStatusEnum.New}-${EntityStatusEnum.Archived}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Archive`
    ],
    // From Suspended Status (Inactive)
    [
      `${EntityStatusEnum.Inactive}-${EntityStatusEnum.Deactive}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Deactive`
    ],
    [
      `${EntityStatusEnum.Inactive}-${EntityStatusEnum.Active}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Activate`
    ],
    [
      `${EntityStatusEnum.Inactive}-${EntityStatusEnum.Archived}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Archive`
    ],
    // From Archived Status
    [
      `${EntityStatusEnum.Archived}-${EntityStatusEnum.Active}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Unarchive`
    ],
    [
      `${EntityStatusEnum.Archived}-${EntityStatusEnum.Inactive}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Unarchive`
    ],
    [
      `${EntityStatusEnum.Archived}-${EntityStatusEnum.New}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Unarchive`
    ],
    [
      `${EntityStatusEnum.Archived}-${EntityStatusEnum.Deactive}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Deactive`
    ],
    // From Delete Status
    [
      `${EntityStatusEnum.Deactive}-${EntityStatusEnum.Active}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Activate`
    ],
    // From Locked Status
    [
      `${EntityStatusEnum.Locked}-${EntityStatusEnum.Active}`,
      `User_Account_Page.Audit_History.EntityStatusChanged.Activate`
    ]
  ]
);
