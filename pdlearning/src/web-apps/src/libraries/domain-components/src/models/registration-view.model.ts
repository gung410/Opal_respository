import {
  AttendanceRatioOfPresentInfo,
  Course,
  CourseCriteriaLearnerViolationField,
  DepartmentInfoModel,
  DepartmentLevelModel,
  Designation,
  IRegistration,
  MetadataTagModel,
  NoOfAssignmentDoneInfo,
  PublicUserInfo,
  Registration,
  TypeOfOrganization
} from '@opal20/domain-api';

import { CourseCriteriaRegistrationViolationDetailItemViewModel } from './course-criteria-registration-violation-detail-item-view.model';
import { IGridDataItem } from '@opal20/infrastructure';

export interface IRegistrationViewModel extends IRegistration {
  selected: boolean;
  register: PublicUserInfo;
  noOfAssignmentDone?: NoOfAssignmentDoneInfo;
  attendanceRatioOfPresent?: AttendanceRatioOfPresentInfo;
  registerAllMetadataDic: Dictionary<MetadataTagModel>;
  registerDesginationDic: Dictionary<Designation>;
  registerCourseCriteriaDic: Dictionary<Course>;
  registerOrganizationUnitTypeDic: Dictionary<TypeOfOrganization>;
  registerOrganizationLevelDic: Dictionary<DepartmentLevelModel>;
  registerDepartmentsDic: Dictionary<DepartmentInfoModel>;
  course?: Course;
}

// @dynamic
export class RegistrationViewModel extends Registration implements IGridDataItem, IRegistrationViewModel {
  public selected: boolean = false;
  public register: PublicUserInfo = new PublicUserInfo();
  public noOfAssignmentDone?: NoOfAssignmentDoneInfo;
  public attendanceRatioOfPresent?: AttendanceRatioOfPresentInfo;
  public designationDisplayText: string = '';
  public teachingSubjectsDisplayText: string = '';
  public teachingLevelDisplayText: string = '';
  public accountTypeDisplayText: string = '';
  public serviceChemeDisplayText: string = '';
  public developmentalRolesDisplayText: string = '';
  public get courseNameDisplayText(): string {
    return this.getCourseNameDisplayText();
  }
  public course?: Course;

  public registerDesginationDic: Dictionary<Designation> = {};
  public registerAllMetadataDic: Dictionary<MetadataTagModel> = {};
  public registerOrganizationUnitTypeDic: Dictionary<TypeOfOrganization> = {};
  public registerOrganizationLevelDic: Dictionary<DepartmentLevelModel> = {};
  public registerDepartmentsDic: Dictionary<DepartmentInfoModel> = {};
  public registerCourseCriteriaDic: Dictionary<Course> = {};

  public static createFromModel(
    registration: Registration,
    register: PublicUserInfo,
    checkAll: boolean = false,
    selecteds: Dictionary<boolean> | null = {},
    registerAllMetadataDic: Dictionary<MetadataTagModel>,
    registerDesginationDic: Dictionary<Designation>,
    registerCourseCriteriaDic: Dictionary<Course>,
    registerOrganizationUnitTypeDic: Dictionary<TypeOfOrganization>,
    registerOrganizationLevelDic: Dictionary<DepartmentLevelModel>,
    registerDepartmentsDic: Dictionary<DepartmentInfoModel>,
    noOfAssignmentDone?: NoOfAssignmentDoneInfo,
    attendanceRatioOfPresent?: AttendanceRatioOfPresentInfo,
    course?: Course
  ): RegistrationViewModel {
    return new RegistrationViewModel({
      ...registration,
      selected: checkAll || selecteds[registration.id],
      register: register,
      registerAllMetadataDic: registerAllMetadataDic,
      noOfAssignmentDone: noOfAssignmentDone,
      attendanceRatioOfPresent: attendanceRatioOfPresent,
      registerDesginationDic: registerDesginationDic,
      registerCourseCriteriaDic: registerCourseCriteriaDic,
      registerOrganizationUnitTypeDic: registerOrganizationUnitTypeDic,
      registerOrganizationLevelDic: registerOrganizationLevelDic,
      registerDepartmentsDic: registerDepartmentsDic,
      course: course
    });
  }

  constructor(data?: IRegistrationViewModel) {
    super(data);
    if (data != null) {
      this.selected = data.selected;
      this.register = data.register;
      this.noOfAssignmentDone = data.noOfAssignmentDone;
      this.attendanceRatioOfPresent = data.attendanceRatioOfPresent;
      this.registerAllMetadataDic = data.registerAllMetadataDic;
      this.registerDesginationDic = data.registerDesginationDic;
      this.registerOrganizationUnitTypeDic = data.registerOrganizationUnitTypeDic;
      this.registerOrganizationLevelDic = data.registerOrganizationLevelDic;
      this.registerDepartmentsDic = data.registerDepartmentsDic;
      this.registerCourseCriteriaDic = data.registerCourseCriteriaDic;
      this.course = data.course;
      this.designationDisplayText = this.getDesignationDisplayText();
      this.teachingLevelDisplayText = this.getTeachingLevelDisplayText();
      this.teachingSubjectsDisplayText = this.getTeachingSubjectsDisplayText();
      this.accountTypeDisplayText = this.getAccountTypeDisplayText();
      this.serviceChemeDisplayText = this.getServiceSchemeDisplayText();
      this.developmentalRolesDisplayText = this.getDevelopmentalRolesDisplayText();
      this.learningCompletedDate = data.learningCompletedDate;
    }
  }

  public getDesignationDisplayText(): string {
    if (this.register == null) {
      return '';
    }
    return this.register.getDesignationDisplayText(this.registerDesginationDic);
  }

  public getAccountTypeDisplayText(): string {
    if (this.register == null) {
      return '';
    }
    return this.register.getAccountTypeDisplayText();
  }

  public getTeachingLevelDisplayText(): string {
    if (this.register == null) {
      return '';
    }
    return this.register.teachingLevels
      .map(p => this.registerAllMetadataDic[p])
      .filter(p => p != null)
      .map(p => p.displayText)
      .join(', ');
  }

  public isRegistrationViolatedCourseCriteria(): boolean {
    return this.courseCriteriaViolation != null && !this.courseCriteriaOverrided && this.courseCriteriaViolation.isViolated;
  }

  public getTeachingSubjectsDisplayText(): string {
    if (this.register == null) {
      return '';
    }
    return []
      .concat(this.register.teachingSubjects, this.register.jobFamilies)
      .map(p => this.registerAllMetadataDic[p])
      .filter(p => p != null)
      .map(p => p.displayText)
      .join(', ');
  }

  public getDevelopmentalRolesDisplayText(): string {
    if (this.register == null) {
      return '';
    }
    return this.register.developmentRoles
      .map(p => this.registerAllMetadataDic[p])
      .filter(p => p != null)
      .map(p => p.displayText)
      .join(', ');
  }

  public getServiceSchemeDisplayText(): string {
    if (this.register == null) {
      return '';
    }
    const serviceScheme = this.registerAllMetadataDic[this.register.serviceScheme];
    return serviceScheme ? serviceScheme.displayText : '';
  }

  public getCourseNameDisplayText(): string {
    return this.course != null ? this.course.courseName : 'N/A';
  }

  public buildCourseCriteriaRegistrationViolationDetailItems(): CourseCriteriaRegistrationViolationDetailItemViewModel[] {
    const courseCriteriaVM: CourseCriteriaRegistrationViolationDetailItemViewModel[] = [];

    if (this.isLearnerViolatedByCriteriaField(CourseCriteriaLearnerViolationField.AccountType)) {
      courseCriteriaVM.push(this.courseCriteriaVM(CourseCriteriaLearnerViolationField.AccountType));
    }
    if (this.isLearnerViolatedByCriteriaField(CourseCriteriaLearnerViolationField.ServiceSchemes)) {
      courseCriteriaVM.push(this.courseCriteriaVM(CourseCriteriaLearnerViolationField.ServiceSchemes));
    }
    if (this.isLearnerViolatedByCriteriaField(CourseCriteriaLearnerViolationField.Tracks)) {
      courseCriteriaVM.push(this.courseCriteriaVM(CourseCriteriaLearnerViolationField.Tracks));
    }
    if (this.isLearnerViolatedByCriteriaField(CourseCriteriaLearnerViolationField.DevRoles)) {
      courseCriteriaVM.push(this.courseCriteriaVM(CourseCriteriaLearnerViolationField.DevRoles));
    }
    if (this.isLearnerViolatedByCriteriaField(CourseCriteriaLearnerViolationField.TeachingLevels)) {
      courseCriteriaVM.push(this.courseCriteriaVM(CourseCriteriaLearnerViolationField.TeachingLevels));
    }
    if (this.isLearnerViolatedByCriteriaField(CourseCriteriaLearnerViolationField.TeachingCourseOfStudy)) {
      courseCriteriaVM.push(this.courseCriteriaVM(CourseCriteriaLearnerViolationField.TeachingCourseOfStudy));
    }
    if (this.isLearnerViolatedByCriteriaField(CourseCriteriaLearnerViolationField.JobFamily)) {
      courseCriteriaVM.push(this.courseCriteriaVM(CourseCriteriaLearnerViolationField.JobFamily));
    }
    if (this.isLearnerViolatedByCriteriaField(CourseCriteriaLearnerViolationField.CoCurricularActivity)) {
      courseCriteriaVM.push(this.courseCriteriaVM(CourseCriteriaLearnerViolationField.CoCurricularActivity));
    }
    if (this.isLearnerViolatedByCriteriaField(CourseCriteriaLearnerViolationField.SubGradeBanding)) {
      courseCriteriaVM.push(this.courseCriteriaVM(CourseCriteriaLearnerViolationField.SubGradeBanding));
    }
    if (this.isLearnerViolatedByCriteriaField(CourseCriteriaLearnerViolationField.PlaceOfWork)) {
      courseCriteriaVM.push(this.courseCriteriaVM(CourseCriteriaLearnerViolationField.PlaceOfWork));
    }
    if (this.isLearnerViolatedByCriteriaField(CourseCriteriaLearnerViolationField.PreRequisiteCourses)) {
      courseCriteriaVM.push(this.courseCriteriaVM(CourseCriteriaLearnerViolationField.PreRequisiteCourses));
    }
    return courseCriteriaVM;
  }

  public courseCriteriaVM(criteriaField: CourseCriteriaLearnerViolationField): CourseCriteriaRegistrationViolationDetailItemViewModel {
    return CourseCriteriaRegistrationViolationDetailItemViewModel.create(
      this.getCriteriaDisplayTitle(CourseCriteriaLearnerViolationField[criteriaField]),
      this.getLearnerCriteriaDisplayTextByCriteriaField(CourseCriteriaLearnerViolationField[criteriaField]),
      this.getLearnerViolationCourseCriteriaDisplayTextByCriteriaField(CourseCriteriaLearnerViolationField[criteriaField]),
      this.getLearnerViolationCourseCriteriaReasonDisplayTextByCriteriaField(CourseCriteriaLearnerViolationField[criteriaField])
    );
  }

  public isLearnerViolatedByCriteriaField(criteriafield: CourseCriteriaLearnerViolationField): boolean {
    if (this.courseCriteriaViolation == null) {
      return false;
    }

    return this.courseCriteriaViolation.checkFieldViolated(criteriafield);
  }

  public getLearnerViolationCourseCriteriaDisplayTextByCriteriaField(criteriafield: string): string {
    if (this.courseCriteriaViolation == null) {
      return '';
    }
    return this.courseCriteriaViolation.getViolatedFieldDisplayText(
      CourseCriteriaLearnerViolationField[criteriafield],
      this.registerAllMetadataDic,
      this.registerCourseCriteriaDic,
      this.registerOrganizationUnitTypeDic,
      this.registerOrganizationLevelDic,
      this.registerDepartmentsDic
    );
  }

  public getCriteriaDisplayTitle(criteriafield: string): string {
    if (this.courseCriteriaViolation == null) {
      return '';
    }
    return this.courseCriteriaViolation.getCriteriaDisplayTitle(CourseCriteriaLearnerViolationField[criteriafield]);
  }

  public getLearnerCriteriaDisplayTextByCriteriaField(criteriafield: string): string {
    if (this.courseCriteriaViolation == null) {
      return '';
    }
    return this.courseCriteriaViolation.getLearnerCriteriaFieldDisplayText(
      CourseCriteriaLearnerViolationField[criteriafield],
      this.registerAllMetadataDic,
      this.registerOrganizationUnitTypeDic,
      this.registerOrganizationLevelDic,
      this.registerDepartmentsDic
    );
  }

  public getLearnerViolationCourseCriteriaReasonDisplayTextByCriteriaField(criteriafield: string): string {
    if (this.courseCriteriaViolation == null) {
      return '';
    }
    return this.courseCriteriaViolation.getLearnerViolationCourseCriteriaReasonDisplayText(
      CourseCriteriaLearnerViolationField[criteriafield]
    );
  }
}
