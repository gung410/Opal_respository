export enum UserRoleEnum {
  DivisionAdmin = 'divisionadmin',
  BranchAdmin = 'branchadmin',
  SchoolAdmin = 'schooladmin',
  CourseContentCreator = 'coursecontentcreator',
  CourseAdmin = 'courseadmin',
  CourseFacilitator = 'coursefacilitator',
  WttgrePresentative = 'wttgrepresentative',
  DigitalContentApprovingOfficer = 'digitalcontentapprovingofficer',
  ContentCreator = 'contentcreator',
  ApprovingOfficer = 'approvingofficer',
  ReportingOfficer = 'reportingofficer',
  Learner = 'learner',
  Guest = 'guest',
  SuperAdministrator = 'superadministrator',
  OverallSystemAdministrator = 'overallsystemadministrator',
  UserAccountAdministrator = 'useraccountadministrator',
  SchoolStaffDeveloper = 'schooltrainingcoordinator',
  DivisionalLearningCoordinator = 'divisiontrainingcoordinator'
}

// tslint:disable-next-line:variable-name
export const UserRoleGroups = {
  Administrators: [
    UserRoleEnum.OverallSystemAdministrator.toString(),
    UserRoleEnum.UserAccountAdministrator.toString(),
    UserRoleEnum.DivisionAdmin.toString(),
    UserRoleEnum.BranchAdmin.toString(),
    UserRoleEnum.SchoolAdmin.toString()
  ],
  DivisionalLearningCoordinatorOrSchoolStaffDeveloper: [
    UserRoleEnum.DivisionalLearningCoordinator.toString(),
    UserRoleEnum.SchoolStaffDeveloper.toString()
  ]
};
