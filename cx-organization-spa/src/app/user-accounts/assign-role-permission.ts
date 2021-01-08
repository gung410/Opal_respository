enum UserSystemRole {
  DivisionAdmin = 'divisionadmin',
  BranchAdmin = 'branchadmin',
  SchoolAdmin = 'schooladmin',
  DivisionLearningCoordinator = 'divisiontrainingcoordinator',
  CourseContentCreator = 'coursecontentcreator',
  CourseApprovingOfficer = 'reportingofficer',
  Learner = 'learner',
  OverallSystemAdministrator = 'overallsystemadministrator',
  SchoolStaffDeveloper = 'schooltrainingcoordinator',
  UserAccountAdministrator = 'useraccountadministrator',
  ContentCreator = 'contentcreator',
  SchoolContentApprovingOfficer = 'schoolcontentapprovingofficer',
  CourseAdmin = 'courseadmin',
  CourseFacilitator = 'coursefacilitator',
  OpjApprovingOfficer = 'approvingofficer',
  MOEHQContentApprovingOfficer = 'MOEHQcontentapprovingofficer',
  WebPageEditor = 'webpageeditor',
  CoursePlanningCoordinator = 'courseplanningcoordinator'
}

// tslint:disable:variable-name
export const AssignRolePermission = {
  overallsystemadministrator: [
    UserSystemRole.OverallSystemAdministrator,
    UserSystemRole.UserAccountAdministrator,
    UserSystemRole.DivisionAdmin,
    UserSystemRole.BranchAdmin,
    UserSystemRole.SchoolAdmin,
    UserSystemRole.DivisionLearningCoordinator,
    UserSystemRole.SchoolStaffDeveloper,
    UserSystemRole.CourseAdmin,
    UserSystemRole.CourseFacilitator,
    UserSystemRole.CoursePlanningCoordinator,
    UserSystemRole.CourseContentCreator,
    UserSystemRole.ContentCreator,
    // UserSystemRole.WebPageEditor,
    UserSystemRole.MOEHQContentApprovingOfficer,
    UserSystemRole.SchoolContentApprovingOfficer,
    UserSystemRole.OpjApprovingOfficer,
    UserSystemRole.CourseApprovingOfficer,
    UserSystemRole.Learner
  ],
  useraccountadministrator: [
    UserSystemRole.DivisionAdmin,
    UserSystemRole.BranchAdmin,
    UserSystemRole.SchoolAdmin,
    UserSystemRole.DivisionLearningCoordinator,
    UserSystemRole.SchoolStaffDeveloper,
    UserSystemRole.CourseAdmin,
    UserSystemRole.CourseFacilitator,
    UserSystemRole.CoursePlanningCoordinator,
    UserSystemRole.CourseContentCreator,
    UserSystemRole.ContentCreator,
    // UserSystemRole.WebPageEditor,
    UserSystemRole.MOEHQContentApprovingOfficer,
    UserSystemRole.SchoolContentApprovingOfficer,
    UserSystemRole.OpjApprovingOfficer,
    UserSystemRole.CourseApprovingOfficer,
    UserSystemRole.Learner
  ],
  divisionadmin: [
    UserSystemRole.DivisionLearningCoordinator,
    UserSystemRole.CourseAdmin,
    UserSystemRole.CourseFacilitator,
    UserSystemRole.CourseContentCreator,
    UserSystemRole.ContentCreator,
    // UserSystemRole.WebPageEditor,
    UserSystemRole.MOEHQContentApprovingOfficer,
    UserSystemRole.OpjApprovingOfficer,
    UserSystemRole.CourseApprovingOfficer,
    UserSystemRole.Learner
  ],
  branchadmin: [
    UserSystemRole.DivisionLearningCoordinator,
    UserSystemRole.CourseAdmin,
    UserSystemRole.CourseFacilitator,
    UserSystemRole.CourseContentCreator,
    UserSystemRole.ContentCreator,
    // UserSystemRole.WebPageEditor,
    UserSystemRole.MOEHQContentApprovingOfficer,
    UserSystemRole.CourseApprovingOfficer,
    UserSystemRole.Learner
  ],
  schooladmin: [
    UserSystemRole.SchoolStaffDeveloper,
    UserSystemRole.CourseAdmin,
    UserSystemRole.CourseFacilitator,
    UserSystemRole.CourseContentCreator,
    UserSystemRole.SchoolContentApprovingOfficer,
    UserSystemRole.OpjApprovingOfficer,
    UserSystemRole.CourseApprovingOfficer,
    UserSystemRole.Learner
  ],
  divisiontrainingcoordinator: [
    UserSystemRole.CourseAdmin,
    UserSystemRole.CourseFacilitator,
    UserSystemRole.CourseContentCreator,
    UserSystemRole.ContentCreator,
    // UserSystemRole.WebPageEditor,
    UserSystemRole.MOEHQContentApprovingOfficer,
    UserSystemRole.CourseApprovingOfficer,
    UserSystemRole.Learner
  ],
  schooltrainingcoordinator: [
    UserSystemRole.CourseAdmin,
    UserSystemRole.CourseFacilitator,
    UserSystemRole.CourseContentCreator,
    UserSystemRole.SchoolContentApprovingOfficer,
    UserSystemRole.CourseApprovingOfficer
  ]
};

export const AssignRoleInOrgPermission = {
  dataowner: [
    UserSystemRole.OverallSystemAdministrator,
    UserSystemRole.UserAccountAdministrator,
    UserSystemRole.CourseApprovingOfficer,
    UserSystemRole.CourseContentCreator,
    UserSystemRole.Learner,
    UserSystemRole.ContentCreator,
    UserSystemRole.MOEHQContentApprovingOfficer,
    UserSystemRole.CourseAdmin,
    UserSystemRole.CourseFacilitator,
    UserSystemRole.CoursePlanningCoordinator,
    UserSystemRole.WebPageEditor
  ],
  ministry: [
    UserSystemRole.CourseApprovingOfficer,
    UserSystemRole.CourseContentCreator,
    UserSystemRole.Learner,
    UserSystemRole.ContentCreator,
    UserSystemRole.MOEHQContentApprovingOfficer,
    UserSystemRole.CourseAdmin,
    UserSystemRole.CourseFacilitator,
    UserSystemRole.CoursePlanningCoordinator,
    UserSystemRole.WebPageEditor
  ],
  division: [
    UserSystemRole.DivisionAdmin,
    UserSystemRole.DivisionLearningCoordinator,
    UserSystemRole.CourseApprovingOfficer,
    UserSystemRole.CourseContentCreator,
    UserSystemRole.Learner,
    UserSystemRole.ContentCreator,
    UserSystemRole.MOEHQContentApprovingOfficer,
    UserSystemRole.OpjApprovingOfficer,
    UserSystemRole.CourseAdmin,
    UserSystemRole.CourseFacilitator,
    UserSystemRole.CoursePlanningCoordinator,
    UserSystemRole.WebPageEditor
  ],
  branch: [
    UserSystemRole.BranchAdmin,
    UserSystemRole.CourseApprovingOfficer,
    UserSystemRole.CourseContentCreator,
    UserSystemRole.Learner,
    UserSystemRole.ContentCreator,
    UserSystemRole.MOEHQContentApprovingOfficer,
    UserSystemRole.CourseAdmin,
    UserSystemRole.CourseFacilitator,
    UserSystemRole.CoursePlanningCoordinator,
    UserSystemRole.WebPageEditor
  ],
  school: [
    UserSystemRole.SchoolAdmin,
    UserSystemRole.SchoolStaffDeveloper,
    UserSystemRole.CourseApprovingOfficer,
    UserSystemRole.CourseContentCreator,
    UserSystemRole.Learner,
    UserSystemRole.ContentCreator,
    UserSystemRole.SchoolContentApprovingOfficer,
    UserSystemRole.OpjApprovingOfficer,
    UserSystemRole.CourseAdmin,
    UserSystemRole.CourseFacilitator,
    UserSystemRole.CoursePlanningCoordinator,
    UserSystemRole.WebPageEditor
  ],
  default: [
    UserSystemRole.CourseApprovingOfficer,
    UserSystemRole.CourseContentCreator,
    UserSystemRole.Learner,
    UserSystemRole.ContentCreator,
    UserSystemRole.SchoolContentApprovingOfficer,
    UserSystemRole.CourseAdmin,
    UserSystemRole.CourseFacilitator,
    UserSystemRole.CoursePlanningCoordinator,
    UserSystemRole.WebPageEditor
  ]
};
