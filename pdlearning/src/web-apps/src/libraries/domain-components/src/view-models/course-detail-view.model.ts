import {
  ADMINISTRATOR_ROLES,
  BaseUserInfo,
  ContentStatus,
  Course,
  CoursePlanningCycle,
  CourseStatus,
  CourseType,
  DepartmentInfoModel,
  ECertificateTemplateModel,
  FormModel,
  FormSurveyType,
  MetadataId,
  MetadataTagGroupCode,
  MetadataTagModel,
  MetadataTagType,
  NOT_APPLICABLE_ITEM_DISPLAY_TEXT,
  OrganizationUnitLevelEnum,
  OtherTrainingAgencyReasonType,
  PlaceOfWorkType,
  PrerequisiteCertificateType,
  ROLE_TO_PERMISSIONS,
  RegistrationMethod,
  SystemRoleEnum,
  TrainingAgencyType,
  UserInfoModel
} from '@opal20/domain-api';
import { MAX_INT, Utils } from '@opal20/infrastructure';
import { Observable, combineLatest, of } from 'rxjs';

import { IOpalSelectDefaultItem } from '@opal20/common-components';
import { buildNieAcademicGroupSelectItems } from '../models/nie-academic-group-select-item.model';
import { buildPrerequisiteEcertificateTypeSelectItems } from '../models/prerequisite-ecertificate-type-select-item.model';
import { buildRegistrationMethodSelectItems } from '../models/registration-method-select-item.model';
import { map } from 'rxjs/operators';

export class CourseDetailViewModel {
  //#region Metadata constant for filter
  public static readonly ORDERED_COURSE_LEVELS: Dictionary<number> = {
    ['Emergent']: 1,
    ['Proficient']: 2,
    ['Accomplished']: 3,
    ['Leading']: 4,
    ['Not Applicable']: 5
  };

  public static readonly create = create;

  public static preCourseFormSurveyType = FormSurveyType.PreCourse;
  public static postCourseFormSurveyType = FormSurveyType.PostCourse;
  public static get formsSurveyTypes(): FormSurveyType[] {
    return [this.preCourseFormSurveyType, this.postCourseFormSurveyType];
  }

  public static courseAdministratorItemsRoles = [SystemRoleEnum.CourseAdministrator, ...ADMINISTRATOR_ROLES];
  public static get courseAdministratorItemsPermissions(): string[] {
    return ROLE_TO_PERMISSIONS[SystemRoleEnum.CourseAdministrator];
  }
  public static courseFacilitatorItemsRoles = [SystemRoleEnum.CourseFacilitator, ...ADMINISTRATOR_ROLES];
  public static get courseFacilitatorItemsPermissions(): string[] {
    return ROLE_TO_PERMISSIONS[SystemRoleEnum.CourseFacilitator];
  }
  public static collaborativeContentCreatorItemsRoles = [SystemRoleEnum.CourseContentCreator, ...ADMINISTRATOR_ROLES];
  public static get collaborativeContentCreatorItemPermissions(): string[] {
    return ROLE_TO_PERMISSIONS[SystemRoleEnum.CourseContentCreator];
  }

  public courseData: Course = new Course();
  public originCourseData: Course = new Course();
  public get allowedThumbnailExtensions(): string[] {
    return Course.allowedThumbnailExtensions;
  }

  //#region User dropdown items
  public courseAdministratorItems: BaseUserInfo[] = [];
  public courseAdministrator2ndItems: BaseUserInfo[] = [];
  public courseApprovingOfficerItems: BaseUserInfo[] = [];
  public courseApprovingOfficer2ndItems: BaseUserInfo[] = [];
  public courseFacilitatorItems: BaseUserInfo[] = [];
  public prerequisiteCertificates: IOpalSelectDefaultItem<string>[] = [];
  public courseCoFacilitatorItems: BaseUserInfo[] = [];
  public collaborativeContentCreatorItems: BaseUserInfo[] = [];
  public currentUser = Utils.cloneDeep(UserInfoModel.getMyUserInfo());
  public _moeOfficerItems: UserInfoModel[] = [];
  public get moeOfficerItems(): UserInfoModel[] {
    return this._moeOfficerItems;
  }
  public set moeOfficerItems(v: UserInfoModel[]) {
    if (Utils.isDifferent(this._moeOfficerItems, v)) {
      this._moeOfficerItems = v;
      this.usersDic = { ...this.usersDic, ...Utils.toDictionary(v, p => p.id) };
    }
  }

  public registrationMethods: IOpalSelectDefaultItem<string>[] = [];
  public nieAcademicGroupItems: IOpalSelectDefaultItem<string>[] = [];

  //#endregion

  //#region Evaluation Forms
  public preCourseFormItems: FormModel[] = [];
  public postCourseFormItems: FormModel[] = [];
  public preCourseFormsDic: Dictionary<FormModel> = {};
  public postCourseFormsDic: Dictionary<FormModel> = {};

  public _eCertificateTemplateItems: ECertificateTemplateModel[] = [];
  public get eCertificateTemplateItems(): ECertificateTemplateModel[] {
    return this._eCertificateTemplateItems;
  }
  public set eCertificateTemplateItems(v: ECertificateTemplateModel[]) {
    if (Utils.isDifferent(this._eCertificateTemplateItems, v)) {
      this.setECertificateTemplates(v);
    }
  }

  public eCertificateTemplateDic: Dictionary<ECertificateTemplateModel> = {};
  public selectedECertificateTemplate: ECertificateTemplateModel;

  public prerequisiteCertificateDic: Dictionary<IOpalSelectDefaultItem<string>> = {};
  //#endregion
  public registrationMethodDic: Dictionary<IOpalSelectDefaultItem<string>> = {};
  public nieAcademicGroupsDic: Dictionary<IOpalSelectDefaultItem<string>> = {};

  //#region Metadata dropdown items
  public metadataTagsDic: Dictionary<MetadataTagModel> = {};
  public metadataTagsDicByGroupCode: Dictionary<MetadataTagModel[]> = {};
  public pdTypeSelectItems: MetadataTagModel[] = [];
  public categorySelectItems: MetadataTagModel[] = [];
  public modeOfLearningSelectItems: MetadataTagModel[] = [];
  public courseLevelSelectItems: MetadataTagModel[] = [];
  public serviceSchemeSelectItems: MetadataTagModel[] = [];
  public _trainingAgencyOther: string;
  public _localTrainingAgency: string[] = [];
  public _localOtherTrainingAgencyReason: string[] = [];
  public _otherTrainingAgencyReasonOther: string;

  public _pdAreaThemeSelectItems: MetadataTagModel[] = [];
  public get pdAreaThemeSelectItems(): MetadataTagModel[] {
    return this._pdAreaThemeSelectItems;
  }
  public set pdAreaThemeSelectItems(v: MetadataTagModel[]) {
    if (Utils.isEqual(this._pdAreaThemeSelectItems, v)) {
      return;
    }
    this._pdAreaThemeSelectItems = v;
    this.pdAreaThemeId = Utils.rightJoinSingle(this.pdAreaThemeId, this.pdAreaThemeSelectItems.map(p => p.tagId));
  }

  public _tracksTreeValues: string[] = [];
  public get tracksTreeValues(): string[] {
    return this._tracksTreeValues;
  }
  public set tracksTreeValues(v: string[]) {
    if (this._tracksTreeValues !== v) {
      this._tracksTreeValues = v;
      this.trackIds = this._tracksTreeValues.filter(p => this.metadataTagsDic[p].groupCode !== MetadataTagGroupCode.SERVICE_SCHEMES);
      this.selectedTracksTree = this.buildSelectedTracksTree();
    }
  }

  public selectedTracksTree: MetadataTagModel[] = [];

  public _tracksTree: MetadataTagModel[] = [];
  public get tracksTree(): MetadataTagModel[] {
    return this._tracksTree;
  }
  public set tracksTree(v: MetadataTagModel[]) {
    this._tracksTree = v;
    this.tracksTreeValues = MetadataTagModel.filterIdsInTree(this._tracksTreeValues, this.tracksTree);
  }

  public _teachingCourseStudysTreeValues: string[] = [];
  public get teachingCourseStudysTreeValues(): string[] {
    return this._teachingCourseStudysTreeValues;
  }
  public set teachingCourseStudysTreeValues(v: string[]) {
    if (this._teachingCourseStudysTreeValues !== v) {
      this._teachingCourseStudysTreeValues = v;
      this.teachingCourseStudyIds = this._teachingCourseStudysTreeValues.filter(
        p => this.metadataTagsDic[p].groupCode !== MetadataTagGroupCode.SERVICE_SCHEMES
      );
      this.selectedTeachingCourseStudysTree = this.buildSelectedTeachingCourseStudysTree();
    }
  }

  public selectedTeachingCourseStudysTree: MetadataTagModel[] = [];

  public _teachingCourseStudysTree: MetadataTagModel[] = [];
  public get teachingCourseStudysTree(): MetadataTagModel[] {
    return this._teachingCourseStudysTree;
  }
  public set teachingCourseStudysTree(v: MetadataTagModel[]) {
    this._teachingCourseStudysTree = v;
    this.teachingCourseStudysTreeValues = MetadataTagModel.filterIdsInTree(
      this._teachingCourseStudysTreeValues,
      this.teachingCourseStudysTree
    );
  }

  public _teachingLevelsTreeValues: string[] = [];
  public get teachingLevelsTreeValues(): string[] {
    return this._teachingLevelsTreeValues;
  }
  public set teachingLevelsTreeValues(v: string[]) {
    if (this._teachingLevelsTreeValues !== v) {
      this._teachingLevelsTreeValues = v;
      this.teachingLevels = this._teachingLevelsTreeValues.filter(
        p => this.metadataTagsDic[p].groupCode !== MetadataTagGroupCode.SERVICE_SCHEMES
      );
      this.selectedTeachingLevelsTree = this.buildSelectedTeachingLevelsTree();
    }
  }

  public selectedTeachingLevelsTree: MetadataTagModel[] = [];

  public _teachingLevelsTree: MetadataTagModel[] = [];
  public get teachingLevelsTree(): MetadataTagModel[] {
    return this._teachingLevelsTree;
  }
  public set teachingLevelsTree(v: MetadataTagModel[]) {
    this._teachingLevelsTree = v;
    this.teachingLevelsTreeValues = MetadataTagModel.filterIdsInTree(this._teachingLevelsTreeValues, this.teachingLevelsTree);
  }

  public _jobFamilysTreeValues: string[] = [];
  public get jobFamilysTreeValues(): string[] {
    return this._jobFamilysTreeValues;
  }
  public set jobFamilysTreeValues(v: string[]) {
    if (this._jobFamilysTreeValues !== v) {
      this._jobFamilysTreeValues = v;
      this.jobFamily = this._jobFamilysTreeValues.filter(p => this.metadataTagsDic[p].groupCode !== MetadataTagGroupCode.SERVICE_SCHEMES);
      this.selectedJobFamilysTree = this.buildSelectedJobFamilysTree();
    }
  }

  public selectedJobFamilysTree: MetadataTagModel[] = [];

  public _jobFamilysTree: MetadataTagModel[] = [];
  public get jobFamilysTree(): MetadataTagModel[] {
    return this._jobFamilysTree;
  }
  public set jobFamilysTree(v: MetadataTagModel[]) {
    this._jobFamilysTree = v;
    this.jobFamilysTreeValues = MetadataTagModel.filterIdsInTree(this._jobFamilysTreeValues, this.jobFamilysTree);
  }

  public _developmentalRolesTreeValues: string[] = [];
  public get developmentalRolesTreeValues(): string[] {
    return this._developmentalRolesTreeValues;
  }
  public set developmentalRolesTreeValues(v: string[]) {
    if (this._developmentalRolesTreeValues !== v) {
      this._developmentalRolesTreeValues = v;
      this.developmentalRoleIds = this._developmentalRolesTreeValues.filter(
        p => this.metadataTagsDic[p].groupCode !== MetadataTagGroupCode.SERVICE_SCHEMES
      );
      this.selectedDevelopmentalRolesTree = this.buildSelectedDevelopmentalRolesTree();
    }
  }

  public selectedDevelopmentalRolesTree: MetadataTagModel[] = [];

  public _developmentalRolesTree: MetadataTagModel[] = [];
  public get developmentalRolesTree(): MetadataTagModel[] {
    return this._developmentalRolesTree;
  }
  public set developmentalRolesTree(v: MetadataTagModel[]) {
    this._developmentalRolesTree = v;
    this.developmentalRolesTreeValues = MetadataTagModel.filterIdsInTree(this._developmentalRolesTreeValues, this.developmentalRolesTree);
  }

  public _easSubstantiveGradeBandingTreeValues: string[] = [];
  public get easSubstantiveGradeBandingTreeValues(): string[] {
    return this._easSubstantiveGradeBandingTreeValues;
  }
  public set easSubstantiveGradeBandingTreeValues(v: string[]) {
    if (this._easSubstantiveGradeBandingTreeValues !== v) {
      this._easSubstantiveGradeBandingTreeValues = v;
      this.easSubstantiveGradeBandingIds = this._easSubstantiveGradeBandingTreeValues.filter(
        p => this.metadataTagsDic[p].groupCode !== MetadataTagGroupCode.SERVICE_SCHEMES
      );
      this.selectedEasSubstantiveGradeBandingTree = this.buildSelectedEasSubstantiveGradeBandingTree();
    }
  }

  public selectedEasSubstantiveGradeBandingTree: MetadataTagModel[] = [];

  public _easSubstantiveGradeBandingTree: MetadataTagModel[] = [];
  public get easSubstantiveGradeBandingTree(): MetadataTagModel[] {
    return this._easSubstantiveGradeBandingTree;
  }
  public set easSubstantiveGradeBandingTree(v: MetadataTagModel[]) {
    this._easSubstantiveGradeBandingTree = v;
    this.easSubstantiveGradeBandingTreeValues = MetadataTagModel.filterIdsInTree(
      this._easSubstantiveGradeBandingTreeValues,
      this.easSubstantiveGradeBandingTree
    );
  }

  public _subjectAreaTreeValues: string[] = [];
  public get subjectAreaTreeValues(): string[] {
    return this._subjectAreaTreeValues;
  }
  public set subjectAreaTreeValues(v: string[]) {
    if (this._subjectAreaTreeValues !== v) {
      this._subjectAreaTreeValues = v;
      this.subjectAreaIds = this._subjectAreaTreeValues.filter(
        p => this.metadataTagsDic[p].groupCode !== MetadataTagGroupCode.SERVICE_SCHEMES
      );
      this.selectedSubjectAreaTree = this.buildSelectedSubjectAreaTree();
    }
  }

  public selectedSubjectAreaTree: MetadataTagModel[] = [];

  public _subjectAreaTree: MetadataTagModel[] = [];
  public get subjectAreaTree(): MetadataTagModel[] {
    return this._subjectAreaTree;
  }
  public set subjectAreaTree(v: MetadataTagModel[]) {
    this._subjectAreaTree = v;
    this.subjectAreaTreeValues = MetadataTagModel.filterIdsInTree(this._subjectAreaTreeValues, this.subjectAreaTree);
  }

  public _learningFrameworkTreeValues: string[] = [];
  public get learningFrameworkTreeValues(): string[] {
    return this._learningFrameworkTreeValues;
  }
  public set learningFrameworkTreeValues(v: string[]) {
    if (this._learningFrameworkTreeValues !== v) {
      this._learningFrameworkTreeValues = v;
      this.learningFrameworkIds = this._learningFrameworkTreeValues.filter(
        p => this.metadataTagsDic[p].groupCode !== MetadataTagGroupCode.SERVICE_SCHEMES
      );
      this.selectedLearningFrameworkTree = this.buildSelectedLearningFrameworkTree();
    }
  }

  public selectedLearningFrameworkTree: MetadataTagModel[] = [];

  public _learningFrameworkTree: MetadataTagModel[] = [];
  public get learningFrameworkTree(): MetadataTagModel[] {
    return this._learningFrameworkTree;
  }
  public set learningFrameworkTree(v: MetadataTagModel[]) {
    this._learningFrameworkTree = v;
    this.learningFrameworkTreeValues = MetadataTagModel.filterIdsInTree(this._learningFrameworkTreeValues, this.learningFrameworkTree);
  }

  public _learningDimensionAreaTreeValues: string[] = [];
  public get learningDimensionAreaTreeValues(): string[] {
    return this._learningDimensionAreaTreeValues;
  }
  public set learningDimensionAreaTreeValues(v: string[]) {
    if (this._learningDimensionAreaTreeValues !== v) {
      this._learningDimensionAreaTreeValues = v;
      const learningDimensionAreas = this._learningDimensionAreaTreeValues.filter(
        p => this.metadataTagsDic[p].groupCode !== MetadataTagGroupCode.LEARNING_FXS
      );

      const learningDimensionAreaDic = Utils.toDictionary(learningDimensionAreas);

      const flatedTagsTree = MetadataTagModel.flatTree(this.learningDimensionAreaTree);

      const learningDimensionIds: string[] = [];
      const learningAreaIds: string[] = [];
      const learningSubAreaIds: string[] = [];

      flatedTagsTree.forEach(item => {
        // Item is checked
        if (learningDimensionAreaDic[item.tagId] != null) {
          // Level 1: Learning Framework
          // Level 2: Learning Dimension
          // Level 3: Learning Area
          // Level 4: Learning Sub Area
          if (item.level === 2) {
            learningDimensionIds.push(item.tagId);
          } else if (item.level === 3) {
            learningAreaIds.push(item.tagId);
          } else if (item.level === 4) {
            learningSubAreaIds.push(item.tagId);
          }
        }
      });

      this.learningDimensionIds = learningDimensionIds;
      this.learningAreaIds = learningAreaIds;
      this.learningSubAreaIds = learningSubAreaIds;

      this.selectedLearningDimensionAreaTree = this.buildSelectedLearningDimensionAreaTree();
    }
  }

  public selectedLearningDimensionAreaTree: MetadataTagModel[] = [];

  public _learningDimensionAreaTree: MetadataTagModel[] = [];
  public get learningDimensionAreaTree(): MetadataTagModel[] {
    return this._learningDimensionAreaTree;
  }
  public set learningDimensionAreaTree(v: MetadataTagModel[]) {
    this._learningDimensionAreaTree = v;
    this.learningDimensionAreaTreeValues = MetadataTagModel.filterIdsInTree(
      this._learningDimensionAreaTreeValues,
      this.learningDimensionAreaTree
    );
  }

  public _teacherOutcomeTreeValues: string[] = [];
  public get teacherOutcomeTreeValues(): string[] {
    return this._teacherOutcomeTreeValues;
  }
  public set teacherOutcomeTreeValues(v: string[]) {
    if (this._teacherOutcomeTreeValues !== v) {
      this._teacherOutcomeTreeValues = v;
      this.teacherOutcomeIds = this._teacherOutcomeTreeValues.filter(
        p => this.metadataTagsDic[p].groupCode !== MetadataTagGroupCode.LEARNING_FXS
      );
      this.selectedTeacherOutcomeTree = this.buildSelectedTeacherOutcomeTree();
    }
  }
  public selectedTeacherOutcomeTree: MetadataTagModel[] = [];

  public _teacherOutcomeTree: MetadataTagModel[] = [];
  public get teacherOutcomeTree(): MetadataTagModel[] {
    return this._teacherOutcomeTree;
  }
  public set teacherOutcomeTree(v: MetadataTagModel[]) {
    this._teacherOutcomeTree = v;
    this.teacherOutcomeTreeValues = MetadataTagModel.filterIdsInTree(this._teacherOutcomeTreeValues, this.teacherOutcomeTree);
  }

  public _natureCourseSelectItems: MetadataTagModel[] = [];
  public get natureCourseSelectItems(): MetadataTagModel[] {
    return this._natureCourseSelectItems;
  }
  public set natureCourseSelectItems(v: MetadataTagModel[]) {
    if (Utils.isEqual(this._natureCourseSelectItems, v)) {
      return;
    }
    this._natureCourseSelectItems = v;
    this.natureOfCourse = Utils.rightJoinSingleBy(this.natureOfCourse, this.natureCourseSelectItems, p => p, p => p.tagId);
  }

  public _teachingSubjectSelectItems: MetadataTagModel[] = [];
  public get teachingSubjectSelectItems(): MetadataTagModel[] {
    return this._teachingSubjectSelectItems;
  }
  public set teachingSubjectSelectItems(v: MetadataTagModel[]) {
    if (Utils.isEqual(this._teachingSubjectSelectItems, v)) {
      return;
    }
    this._teachingSubjectSelectItems = v;
    this.teachingSubjectIds = Utils.rightJoin(this.teachingSubjectIds, this.teachingSubjectSelectItems.map(p => p.tagId));
  }

  public _cocurricularActivitySelectItems: MetadataTagModel[] = [];
  public get cocurricularActivitySelectItems(): MetadataTagModel[] {
    return this._cocurricularActivitySelectItems;
  }
  public set cocurricularActivitySelectItems(v: MetadataTagModel[]) {
    if (Utils.isEqual(this._cocurricularActivitySelectItems, v)) {
      return;
    }
    this._cocurricularActivitySelectItems = v;
    this.cocurricularActivityIds = Utils.rightJoin(this.cocurricularActivityIds, this.cocurricularActivitySelectItems.map(p => p.tagId));
  }

  public _pdActivityPeriodSelectItems: MetadataTagModel[] = [];
  public get pdActivityPeriodSelectItems(): MetadataTagModel[] {
    return this._pdActivityPeriodSelectItems;
  }
  public set pdActivityPeriodSelectItems(v: MetadataTagModel[]) {
    if (Utils.isEqual(this._pdActivityPeriodSelectItems, v)) {
      return;
    }
    this._pdActivityPeriodSelectItems = v;
    this.pdActivityPeriods = Utils.rightJoin(this.pdActivityPeriods, this.pdActivityPeriodSelectItems.map(p => p.tagId));
  }

  public _teacherOutcomeSelectItems: MetadataTagModel[] = [];
  public get teacherOutcomeSelectItems(): MetadataTagModel[] {
    return this._teacherOutcomeSelectItems;
  }
  public set teacherOutcomeSelectItems(v: MetadataTagModel[]) {
    if (Utils.isEqual(this._teacherOutcomeSelectItems, v)) {
      return;
    }
    this._teacherOutcomeSelectItems = v;
    this.teacherOutcomeIds = Utils.rightJoin(this.teacherOutcomeIds, this.teacherOutcomeSelectItems.map(p => p.tagId));
  }
  //#endregion

  //#region  Department Items
  public _departmentsDic: Dictionary<DepartmentInfoModel> = {};
  public get departmentsDic(): Dictionary<DepartmentInfoModel> {
    return this._departmentsDic;
  }
  public set departmentsDic(v: Dictionary<DepartmentInfoModel>) {
    if (Utils.isEqual(this._departmentsDic, v)) {
      return;
    }
    this._departmentsDic = v;
  }
  public _divisionSelectItems: DepartmentInfoModel[] = [];
  public get divisionSelectItems(): DepartmentInfoModel[] {
    return this._divisionSelectItems;
  }
  public set divisionSelectItems(v: DepartmentInfoModel[]) {
    if (Utils.isEqual(this._divisionSelectItems, v)) {
      return;
    }
    this._divisionSelectItems = v;
  }
  public _ownerDivisionSelectItems: DepartmentInfoModel[] = [];
  public get ownerDivisionSelectItems(): DepartmentInfoModel[] {
    return this._ownerDivisionSelectItems;
  }
  public set ownerDivisionSelectItems(v: DepartmentInfoModel[]) {
    if (Utils.isEqual(this._ownerDivisionSelectItems, v)) {
      return;
    }
    this._ownerDivisionSelectItems = v;
  }
  public _partneringSelectItems: DepartmentInfoModel[] = [];
  public get partneringSelectItems(): DepartmentInfoModel[] {
    return this._partneringSelectItems;
  }
  public set partneringSelectItems(v: DepartmentInfoModel[]) {
    if (Utils.isEqual(this._partneringSelectItems, v)) {
      return;
    }
    this._partneringSelectItems = v;
  }
  public _branchSelectItems: DepartmentInfoModel[] = [];
  public get branchSelectItems(): DepartmentInfoModel[] {
    return this._branchSelectItems;
  }
  public set branchSelectItems(v: DepartmentInfoModel[]) {
    if (Utils.isEqual(this._branchSelectItems, v)) {
      return;
    }
    this._branchSelectItems = v;
    this.applicableBranchIds = Utils.rightJoinBy(this.applicableBranchIds, this.branchSelectItems, p => p, p => p.id);
  }

  public _ownerBranchSelectItems: DepartmentInfoModel[] = [];
  public get ownerBranchSelectItems(): DepartmentInfoModel[] {
    return this._ownerBranchSelectItems;
  }
  public set ownerBranchSelectItems(v: DepartmentInfoModel[]) {
    if (Utils.isEqual(this._ownerBranchSelectItems, v)) {
      return;
    }
    this._ownerBranchSelectItems = v;
    this.ownerBranchIds = Utils.rightJoinBy(this.ownerBranchIds, this.ownerBranchSelectItems, p => p, p => p.id);
  }
  public _zoneSelectItems: DepartmentInfoModel[] = [];
  public get zoneSelectItems(): DepartmentInfoModel[] {
    return this._zoneSelectItems;
  }
  public set zoneSelectItems(v: DepartmentInfoModel[]) {
    if (Utils.isEqual(this._zoneSelectItems, v)) {
      return;
    }
    this._zoneSelectItems = v;
    this.applicableZoneIds = Utils.rightJoinBy(this.applicableZoneIds, this._zoneSelectItems, p => p, p => p.id);
  }
  public _clusterSelectItems: DepartmentInfoModel[] = [];
  public get clusterSelectItems(): DepartmentInfoModel[] {
    return this._clusterSelectItems;
  }
  public set clusterSelectItems(v: DepartmentInfoModel[]) {
    if (Utils.isEqual(this._clusterSelectItems, v)) {
      return;
    }
    this._clusterSelectItems = v;
    this.applicableClusterIds = Utils.rightJoinBy(this.applicableClusterIds, this.clusterSelectItems, p => p, p => p.id);
  }
  public _schoolSelectItems: DepartmentInfoModel[] = [];
  public get schoolSelectItems(): DepartmentInfoModel[] {
    return this._schoolSelectItems;
  }
  public set schoolSelectItems(v: DepartmentInfoModel[]) {
    if (Utils.isEqual(this._schoolSelectItems, v)) {
      return;
    }
    this._schoolSelectItems = v;
    this.applicableSchoolIds = Utils.rightJoinBy(this.applicableSchoolIds, this.schoolSelectItems, p => p, p => p.id);
  }
  //#endregion

  //#region  Others
  public _prerequisiteCourseSelectItems: Course[] = [];
  public get prerequisiteCourseSelectItems(): Course[] {
    return this._prerequisiteCourseSelectItems;
  }
  public set prerequisiteCourseSelectItems(v: Course[]) {
    if (Utils.isEqual(this._prerequisiteCourseSelectItems, v)) {
      return;
    }
    this._prerequisiteCourseSelectItems = v;
  }

  public get isArchived(): boolean {
    return this.courseData.isArchived();
  }
  public get archiveDate(): Date | null {
    return this.courseData.archiveDate;
  }
  public get archivedBy(): string | null {
    return this.courseData.archivedBy;
  }

  public get isMicrolearning(): boolean {
    return this.courseData.isMicrolearning();
  }
  public get isPlanningVerificationRequired(): boolean {
    return this.courseData.isPlanningVerificationRequired();
  }
  public get isCompletingCourseForPlanning(): boolean {
    return this.courseData.isCompletingCourseForPlanning();
  }
  public _checkTrainingAgencyOther: boolean;
  public get checkTrainingAgencyOther(): boolean {
    return this._checkTrainingAgencyOther;
  }
  public set checkTrainingAgencyOther(v: boolean) {
    this._checkTrainingAgencyOther = v;
  }
  public _checkMaxReLearningTimesOther: boolean;
  public get checkMaxReLearningTimesOther(): boolean {
    return this._checkMaxReLearningTimesOther;
  }
  public set checkMaxReLearningTimesOther(v: boolean) {
    if (Utils.isEqual(this._checkMaxReLearningTimesOther, v)) {
      return;
    }
    this._checkMaxReLearningTimesOther = v;
    this._checkMaxReLearningTimesUnlimited = false;
    this._maxReLearningTimesOther = 1;
    this.courseData.maxReLearningTimes = 1;
  }
  public _maxReLearningTimesOther: number;
  public get maxReLearningTimesOther(): number {
    return this._maxReLearningTimesOther;
  }
  public set maxReLearningTimesOther(v: number) {
    if (Utils.isEqual(this._maxReLearningTimesOther, v)) {
      return;
    }
    this._maxReLearningTimesOther = v;
    if (v != null && v !== 0) {
      this.courseData.maxReLearningTimes = this._maxReLearningTimesOther;
    }
  }
  public _checkMaxReLearningTimesUnlimited: boolean;
  public get checkMaxReLearningTimesUnlimited(): boolean {
    return this._checkMaxReLearningTimesUnlimited;
  }
  public set checkMaxReLearningTimesUnlimited(v: boolean) {
    if (Utils.isEqual(this._checkMaxReLearningTimesUnlimited, v)) {
      return;
    }
    this._checkMaxReLearningTimesUnlimited = v;
    this._checkMaxReLearningTimesOther = false;
    this._maxReLearningTimesOther = null;
    this.courseData.maxReLearningTimes = MAX_INT;
  }
  public _checkOtherTrainingAgencyReasonOther: boolean;
  public get checkOtherTrainingAgencyReasonOther(): boolean {
    return this._checkOtherTrainingAgencyReasonOther;
  }
  public set checkOtherTrainingAgencyReasonOther(v: boolean) {
    if (Utils.isEqual(this._checkOtherTrainingAgencyReasonOther, v)) {
      return;
    }
    this._checkOtherTrainingAgencyReasonOther = v;
  }

  public get isPublishedOnce(): boolean {
    return this.courseData.isPublishedOnce();
  }

  public get isEditable(): boolean {
    return this.courseData.isEditableBeforeStarted() || this.courseData.isEditableEvenClassStarted();
  }

  public static allUserItemsRoles(departmentsDic: Dictionary<DepartmentInfoModel>): SystemRoleEnum[] {
    return [].concat(
      this.courseAdministratorItemsRoles,
      this.courseApprovingOfficerItemsRoles(departmentsDic),
      this.courseFacilitatorItemsRoles,
      this.collaborativeContentCreatorItemsRoles
    );
  }

  public static courseApprovingOfficerItemsRoles(departmentsDic: Dictionary<DepartmentInfoModel>): SystemRoleEnum[] {
    const currentUser = UserInfoModel.getMyUserInfo();
    if (
      currentUser &&
      currentUser.departmentId &&
      departmentsDic[currentUser.departmentId] &&
      departmentsDic[currentUser.departmentId].departmentType === OrganizationUnitLevelEnum.School
    ) {
      return departmentsDic[currentUser.departmentId].isPartnering
        ? [
            SystemRoleEnum.SystemAdministrator,
            SystemRoleEnum.SchoolAdministrator,
            SystemRoleEnum.SchoolContentApprovingOfficer,
            SystemRoleEnum.MOEHQContentApprovingOfficer,
            SystemRoleEnum.DivisionAdministrator,
            SystemRoleEnum.BranchAdministrator
          ]
        : [SystemRoleEnum.SystemAdministrator, SystemRoleEnum.SchoolAdministrator, SystemRoleEnum.SchoolContentApprovingOfficer];
    }
    return [
      SystemRoleEnum.MOEHQContentApprovingOfficer,
      SystemRoleEnum.DivisionAdministrator,
      SystemRoleEnum.BranchAdministrator,
      SystemRoleEnum.SystemAdministrator
    ];
  }

  constructor(
    course?: Course,
    public usersDic: Dictionary<UserInfoModel> = {},
    public metadataTags: MetadataTagModel[] = [],
    public departments: DepartmentInfoModel[] = [],
    public prerequisiteCoursesDic: Dictionary<Course> = {},
    public forms: FormModel[] = [],
    public eCertificate?: ECertificateTemplateModel,
    public coursePlanningCycle?: CoursePlanningCycle
  ) {
    this.updateCourseData(new Course());
    if (course) {
      this.updateCourseData(course);
      this.processTrainingAgencyOtherCheckedStates();
      this.processMaxReLearningTimesCheckedStates();
      this._initTrainingAgency(course.trainingAgency);
      this._initOtherTrainingAgencyReason(course.otherTrainingAgencyReason);
    }

    this.setMetadataTagsDicInfo(metadataTags);

    this.departmentsDic = Utils.toDictionary(this.departments, p => p.id);

    //#region Course evaluation forms
    this.prerequisiteCertificates = buildPrerequisiteEcertificateTypeSelectItems();
    this.prerequisiteCertificateDic = Utils.toDictionary(this.prerequisiteCertificates, p => p.value);
    this.setCourseEvaluation(forms);
    this.selectedECertificateTemplate = eCertificate;
    if (eCertificate) {
      this.setECertificateTemplates([eCertificate]);
    }
    //#endregion

    //#region Target Audience dropdown items
    this.registrationMethods = buildRegistrationMethodSelectItems();
    this.registrationMethodDic = Utils.toDictionary(this.registrationMethods, p => p.value);
    //#endregion

    //#region NIE Academic Group dropdown items
    this.nieAcademicGroupItems = buildNieAcademicGroupSelectItems();
    this.nieAcademicGroupsDic = Utils.toDictionary(this.nieAcademicGroupItems, p => p.value);
    //#endregion

    //#region Metadata dropdown items
    this.pdTypeSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TYPES], []);
    this.categorySelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_CATEGORIES], []);
    this.modeOfLearningSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_MODES], []);
    this.courseLevelSelectItems = Utils.orderBy(
      Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.COURSE_LEVELS], []),
      p => CourseDetailViewModel.ORDERED_COURSE_LEVELS[p.displayText] || MAX_INT
    );
    this.serviceSchemeSelectItems = Utils.defaultIfNull(
      Utils.rightJoinBy(
        Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.SERVICE_SCHEMES], []),
        Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TAXONOMY], []),
        x => x.tagId,
        x => x.parentTagId
      ),
      []
    );
    this._subjectAreaTree = this.buildSubjectAreaTree();
    this._subjectAreaTreeValues = this.buildSubjectAreaTreeValues();
    this.selectedSubjectAreaTree = this.buildSelectedSubjectAreaTree();

    this._learningFrameworkTree = this.buildLearningFrameworkTree();
    this._learningFrameworkTreeValues = this.buildLearningFrameworkTreeValues();
    this.selectedLearningFrameworkTree = this.buildSelectedLearningFrameworkTree();

    this._learningDimensionAreaTree = this.buildLearningDimensionAreaTree();
    this._learningDimensionAreaTreeValues = this.buildLearningDimensionAreaTreeValues();
    this.selectedLearningDimensionAreaTree = this.buildSelectedLearningDimensionAreaTree();

    this._teacherOutcomeTree = this.buildTeacherOutcomeTree();
    this._teacherOutcomeTreeValues = this.buildTeacherOutcomeTreeValues();
    this.selectedTeacherOutcomeTree = this.buildSelectedTeacherOutcomeTree();

    this._natureCourseSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_NATURES], []);
    this._tracksTree = this.buildTracksTree();
    this._tracksTreeValues = this.buildTracksTreeValues();
    this.selectedTracksTree = this.buildSelectedTracksTree();

    this._teachingLevelsTree = this.buildTeachingLevelsTree();
    this._teachingLevelsTreeValues = this.buildTeachingLevelsTreeValues();
    this.selectedTeachingLevelsTree = this.buildSelectedTeachingLevelsTree();

    this._developmentalRolesTree = this.buildDevelopmentalRolesTree();
    this._developmentalRolesTreeValues = this.buildDevelopmentalRolesTreeValues();
    this.selectedDevelopmentalRolesTree = this.buildSelectedDevelopmentalRolesTree();

    this._teachingSubjectSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.TEACHING_SUBJECTS], []);

    this._jobFamilysTree = this.buildJobFamilysTree();
    this._jobFamilysTreeValues = this.buildJobFamilysTreeValues();
    this.selectedJobFamilysTree = this.buildSelectedJobFamilysTree();

    this._teachingCourseStudysTree = this.buildTeachingCourseStudysTree();
    this._teachingCourseStudysTreeValues = this.buildTeachingCourseStudysTreeValues();
    this.selectedTeachingCourseStudysTree = this.buildSelectedTeachingCourseStudysTree();

    this._cocurricularActivitySelectItems = Utils.defaultIfNull(
      this.metadataTagsDicByGroupCode[MetadataTagGroupCode.CO_CURRICULAR_ACTIVITIES],
      []
    );

    this._easSubstantiveGradeBandingTree = this.buildEasSubstantiveGradeBandingTree();
    this._easSubstantiveGradeBandingTreeValues = this.buildEasSubstantiveGradeBandingTreeValues();
    this.selectedEasSubstantiveGradeBandingTree = this.buildSelectedEasSubstantiveGradeBandingTree();
    this.pdActivityPeriodSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PERIOD_PD_ACTIVITY], []);
    //#endregion

    //#region Department List Item
    this._divisionSelectItems = this.departments.filter(p => p.departmentType === OrganizationUnitLevelEnum.Division);
    this._ownerDivisionSelectItems = this._divisionSelectItems;
    this._partneringSelectItems = this.departments.filter(p => this.partnerOrganisationIds.includes(p.id));
    this._branchSelectItems = this.buildBranchSelectItems();
    this._ownerBranchSelectItems = this.buildOwnerBranchSelectItems();
    this._zoneSelectItems = this.buildZoneSelectItems();
    this._clusterSelectItems = this.buildClusterSelectItems();
    this._schoolSelectItems = this.buildSchoolSelectItems();
    //#endregion
  }

  //#endregion

  //#region Overview Info Tab

  public get status(): CourseStatus {
    return this.courseData.status;
  }

  public set status(status: CourseStatus) {
    this.courseData.status = status;
  }

  public get contentStatus(): ContentStatus {
    return this.courseData.contentStatus;
  }

  public set contentStatus(contentStatus: ContentStatus) {
    this.courseData.contentStatus = contentStatus;
  }

  public get createBy(): string {
    return this.courseData.createdBy;
  }

  public set createBy(createBy: string) {
    this.courseData.createdBy = createBy;
  }

  public get courseName(): string {
    return this.courseData.courseName;
  }
  public set courseName(name: string) {
    this.courseData.courseName = name;
  }
  public get durationHours(): number {
    return this.courseData.durationHours;
  }
  public set durationHours(durationHours: number) {
    this.courseData.durationHours = durationHours;
  }
  public get durationMinutes(): number {
    return this.courseData.durationMinutes;
  }
  public set durationMinutes(durationMinutes: number) {
    this.courseData.durationMinutes = durationMinutes;
  }
  public get pdActivityType(): string {
    return this.courseData.pdActivityType;
  }
  public set pdActivityType(pdActivityType: string) {
    if (Utils.isEqual(this.courseData.pdActivityType, pdActivityType)) {
      return;
    }
    this.courseData.pdActivityType = pdActivityType;
    if (this.isMicrolearning) {
      this.clearDataHiddenFieldWhenMicrolearning();
    } else {
      this.setDefaultValueForNotMicrolearning();
    }
  }
  public get categoryIds(): string[] {
    return this.courseData.categoryIds;
  }
  public set categoryIds(categoryIds: string[]) {
    this.courseData.categoryIds = categoryIds;
  }
  public get learningMode(): string {
    return this.courseData.learningMode;
  }
  public set learningMode(learningMode: string) {
    this.courseData.learningMode = learningMode;
  }
  public get courseCode(): string {
    return this.courseData.courseCode;
  }
  public set courseCode(courseCode: string) {
    this.courseData.courseCode = courseCode;
  }
  public get externalCode(): string | null {
    return this.courseData.externalCode;
  }
  public set externalCode(externalCode: string | null) {
    this.courseData.externalCode = externalCode;
  }
  public get courseLevel(): string {
    return this.courseData.courseLevel;
  }
  public set courseLevel(courseLevel: string) {
    this.courseData.courseLevel = courseLevel;
  }
  public get courseOutlineStructure(): string {
    return this.courseData.courseOutlineStructure;
  }
  public set courseOutlineStructure(courseOutlineStructure: string) {
    this.courseData.courseOutlineStructure = courseOutlineStructure;
  }
  public get courseObjective(): string {
    return this.courseData.courseObjective;
  }
  public set courseObjective(courseObjective: string) {
    this.courseData.courseObjective = courseObjective;
  }
  public get description(): string {
    return this.courseData.description;
  }
  public set description(description: string) {
    this.courseData.description = description;
  }
  public get thumbnailUrl(): string {
    return this.courseData.thumbnailUrl;
  }
  public set thumbnailUrl(v: string) {
    this.courseData.thumbnailUrl = v;
  }
  //#endregion

  //#region Provider Info
  public get trainingAgency(): string[] {
    return this.courseData.trainingAgency;
  }

  public set trainingAgency(trainingAgency: string[]) {
    this.courseData.trainingAgency = trainingAgency;
    if (!this.trainingAgencyContains(TrainingAgencyType.NIE)) {
      this.nieAcademicGroups = [];
    }
  }

  public get trainingAgencyOther(): string {
    return this._trainingAgencyOther;
  }
  public set trainingAgencyOther(trainingAgencyOther: string) {
    this._trainingAgencyOther = trainingAgencyOther;
    this.updateTrainingAgency();
  }

  public get otherTrainingAgencyReason(): string[] {
    return this.courseData.otherTrainingAgencyReason;
  }
  public set otherTrainingAgencyReason(otherTrainingAgencyReason: string[]) {
    this.courseData.otherTrainingAgencyReason = otherTrainingAgencyReason;
  }

  public get otherTrainingAgencyReasonOther(): string {
    return this._otherTrainingAgencyReasonOther;
  }
  public set otherTrainingAgencyReasonOther(otherTrainingAgencyReasonOther: string) {
    this._otherTrainingAgencyReasonOther = otherTrainingAgencyReasonOther;
    this.changeOtherTrainingAgencyReason();
  }

  public get ownerDivisionIds(): number[] {
    return this.courseData.ownerDivisionIds;
  }
  public set ownerDivisionIds(ownerDivisionIds: number[]) {
    this.courseData.ownerDivisionIds = ownerDivisionIds;
  }
  public get ownerBranchIds(): number[] {
    return this.courseData.ownerBranchIds;
  }
  public set ownerBranchIds(ownerBranchIds: number[]) {
    this.courseData.ownerBranchIds = ownerBranchIds;
  }
  public get partnerOrganisationIds(): number[] {
    return this.courseData.partnerOrganisationIds;
  }
  public set partnerOrganisationIds(partnerOrganisationIds: number[]) {
    this.courseData.partnerOrganisationIds = partnerOrganisationIds;
  }
  public get moeOfficerId(): string {
    return this.courseData.moeOfficerId;
  }
  public set moeOfficerId(moeOfficerId: string) {
    this.courseData = this.setMoeOfficerId(this.courseData, moeOfficerId);
  }
  public get moeOfficerPhoneNumber(): string {
    return this.courseData && this.courseData.moeOfficerPhoneNumber
      ? this.courseData.moeOfficerPhoneNumber
      : Utils.cloneDeep(this.currentUser.phone);
  }
  public set moeOfficerPhoneNumber(moeOfficerPhoneNumber: string) {
    this.courseData.moeOfficerPhoneNumber = moeOfficerPhoneNumber;
  }
  public get moeOfficerEmail(): string {
    return this.courseData && this.courseData.moeOfficerEmail;
  }
  public set moeOfficerEmail(moeOfficerEmail: string) {
    this.courseData.moeOfficerEmail = moeOfficerEmail;
  }
  public get notionalCost(): number {
    return this.courseData.notionalCost;
  }
  public set notionalCost(notionalCost: number) {
    this.courseData.notionalCost = notionalCost;
    if (this.courseData.notionalCost !== 0) {
      this.courseData.courseFee = 0;
    }
  }
  public get courseFee(): number {
    return this.courseData.courseFee;
  }
  public set courseFee(courseFee: number) {
    this.courseData.courseFee = courseFee;
    if (this.courseData.courseFee !== 0) {
      this.courseData.notionalCost = 0;
    }
  }

  public get nieAcademicGroups(): string[] {
    return this.courseData.nieAcademicGroups;
  }
  public set nieAcademicGroups(nieAcademicGroups: string[]) {
    this.courseData.nieAcademicGroups = nieAcademicGroups;
  }
  //#endregion

  //#region Metadata
  public get serviceSchemeIds(): string[] {
    return this.courseData.serviceSchemeIds;
  }
  public set serviceSchemeIds(serviceSchemeIds: string[]) {
    this.courseData.serviceSchemeIds = serviceSchemeIds;
    this.subjectAreaTree = this.buildSubjectAreaTree();
    this.learningFrameworkTree = this.buildLearningFrameworkTree();
    this.developmentalRolesTree = this.buildDevelopmentalRolesTree();
    this.easSubstantiveGradeBandingTree = this.buildEasSubstantiveGradeBandingTree();
  }
  public get subjectAreaIds(): string[] {
    return this.courseData.subjectAreaIds;
  }
  public set subjectAreaIds(subjectAreaIds: string[]) {
    this.courseData.subjectAreaIds = subjectAreaIds;
    this.pdAreaThemeSelectItems = this.subjectAreaIds.map(p => this.metadataTagsDic[p]).filter(p => p != null && p.codingScheme != null);
  }
  public get pdAreaThemeId(): string {
    return this.courseData.pdAreaThemeId;
  }
  public set pdAreaThemeId(pdAreaThemeId: string) {
    this.courseData.pdAreaThemeId = pdAreaThemeId;
  }
  public get learningFrameworkIds(): string[] {
    return this.courseData.learningFrameworkIds;
  }
  public set learningFrameworkIds(learningFrameworkIds: string[]) {
    this.courseData.learningFrameworkIds = learningFrameworkIds;
    this.learningDimensionAreaTree = this.buildLearningDimensionAreaTree();
    this.teacherOutcomeTree = this.buildTeacherOutcomeTree();
  }
  public get learningDimensionIds(): string[] {
    return this.courseData.learningDimensionIds;
  }
  public set learningDimensionIds(learningDimensionIds: string[]) {
    this.courseData.learningDimensionIds = learningDimensionIds;
  }
  public get learningAreaIds(): string[] {
    return this.courseData.learningAreaIds;
  }
  public set learningAreaIds(learningAreaIds: string[]) {
    this.courseData.learningAreaIds = learningAreaIds;
  }
  public get learningSubAreaIds(): string[] {
    return this.courseData.learningSubAreaIds;
  }
  public set learningSubAreaIds(learningSubAreaIds: string[]) {
    this.courseData.learningSubAreaIds = learningSubAreaIds;
  }
  public get learningDimensionAreas(): string[] {
    const learningAreas = this.learningAreaIds.concat(this.learningSubAreaIds);
    return this.learningDimensionIds.concat(learningAreas);
  }
  public get metadataKeys(): string[] {
    return this.courseData.metadataKeys;
  }
  public set metadataKeys(metadataKeys: string[]) {
    this.courseData.metadataKeys = metadataKeys;
  }
  public get teacherOutcomeIds(): string[] {
    return this.courseData.teacherOutcomeIds;
  }
  public set teacherOutcomeIds(teacherOutcomeIds: string[]) {
    this.courseData.teacherOutcomeIds = teacherOutcomeIds;
  }
  //#endregion

  //#region Copyrights
  public get allowPersonalDownload(): boolean {
    return this.courseData.allowPersonalDownload;
  }
  public setAllowPersonalDownload(allowPersonalDownload: boolean): void {
    this.clearLicencePermission();
    this.courseData.allowPersonalDownload = allowPersonalDownload;
  }
  public get allowNonCommerInMOEReuseWithModification(): boolean {
    return this.courseData.allowNonCommerInMOEReuseWithModification;
  }
  public setAllowNonCommerInMOEReuseWithModification(allowNonCommerInMOEReuseWithModification: boolean): void {
    this.clearLicencePermission();
    this.courseData.allowNonCommerInMOEReuseWithModification = allowNonCommerInMOEReuseWithModification;
  }
  public get allowNonCommerReuseWithModification(): boolean {
    return this.courseData.allowNonCommerReuseWithModification;
  }
  public setAllowNonCommerReuseWithModification(allowNonCommerReuseWithModification: boolean): void {
    this.clearLicencePermission();
    this.courseData.allowNonCommerReuseWithModification = allowNonCommerReuseWithModification;
  }
  public get allowNonCommerInMoeReuseWithoutModification(): boolean {
    return this.courseData.allowNonCommerInMoeReuseWithoutModification;
  }
  public setAllowNonCommerInMoeReuseWithoutModification(allowNonCommerInMoeReuseWithoutModification: boolean): void {
    this.clearLicencePermission();
    this.courseData.allowNonCommerInMoeReuseWithoutModification = allowNonCommerInMoeReuseWithoutModification;
  }
  public get allowNonCommerReuseWithoutModification(): boolean {
    return this.courseData.allowNonCommerReuseWithoutModification;
  }
  public setAllowNonCommerReuseWithoutModification(allowNonCommerReuseWithoutModification: boolean): void {
    this.clearLicencePermission();
    this.courseData.allowNonCommerReuseWithoutModification = allowNonCommerReuseWithoutModification;
  }
  public get copyrightOwner(): string {
    return this.courseData.copyrightOwner;
  }
  public set copyrightOwner(copyrightOwner: string) {
    this.courseData.copyrightOwner = copyrightOwner;
  }
  public get acknowledgementAndCredit(): string {
    return this.courseData.acknowledgementAndCredit;
  }
  public set acknowledgementAndCredit(acknowledgementAndCredit: string) {
    this.courseData.acknowledgementAndCredit = acknowledgementAndCredit;
  }
  public get remarks(): string {
    return this.courseData.remarks;
  }
  public set remarks(remarks: string) {
    this.courseData.remarks = remarks;
  }
  //#endregion

  //#region Target Audience
  public get maximumPlacesPerSchool(): number {
    return this.courseData.maximumPlacesPerSchool;
  }
  public set maximumPlacesPerSchool(maximumPlacesPerSchool: number) {
    this.courseData.maximumPlacesPerSchool = maximumPlacesPerSchool;
  }
  public get prerequisiteCourseIds(): string[] {
    return this.courseData.prerequisiteCourseIds;
  }
  public set prerequisiteCourseIds(prerequisiteCourseIds: string[]) {
    this.courseData.prerequisiteCourseIds = prerequisiteCourseIds;
  }
  public get numOfSchoolLeader(): number {
    return this.courseData.numOfSchoolLeader;
  }
  public set numOfSchoolLeader(numOfSchoolLeader: number) {
    this.courseData.numOfSchoolLeader = numOfSchoolLeader;
  }
  public get numOfSeniorOrLeadTeacher(): number {
    return this.courseData.numOfSeniorOrLeadTeacher;
  }
  public set numOfSeniorOrLeadTeacher(numOfSeniorOrLeadTeacher: number) {
    this.courseData.numOfSeniorOrLeadTeacher = numOfSeniorOrLeadTeacher;
  }
  public get numOfMiddleManagement(): number {
    return this.courseData.numOfMiddleManagement;
  }
  public set numOfMiddleManagement(numOfMiddleManagement: number) {
    this.courseData.numOfMiddleManagement = numOfMiddleManagement;
  }
  public get numOfBeginningTeacher(): number {
    return this.courseData.numOfBeginningTeacher;
  }
  public set numOfBeginningTeacher(numOfBeginningTeacher: number) {
    this.courseData.numOfBeginningTeacher = numOfBeginningTeacher;
  }
  public get numOfExperiencedTeacher(): number {
    return this.courseData.numOfExperiencedTeacher;
  }
  public set numOfExperiencedTeacher(numOfExperiencedTeacher: number) {
    this.courseData.numOfExperiencedTeacher = numOfExperiencedTeacher;
  }
  public get placeOfWork(): PlaceOfWorkType {
    return this.courseData.placeOfWork;
  }
  public set placeOfWork(placeOfWork: PlaceOfWorkType) {
    this.courseData.placeOfWork = placeOfWork;
  }
  public get applicableDivisionIds(): number[] {
    return this.courseData.applicableDivisionIds;
  }
  public set applicableDivisionIds(applicableDivisionIds: number[]) {
    this.courseData.applicableDivisionIds = applicableDivisionIds;
  }
  public get applicableBranchIds(): number[] {
    return this.courseData.applicableBranchIds;
  }
  public set applicableBranchIds(applicableBranchIds: number[]) {
    this.courseData.applicableBranchIds = applicableBranchIds;
  }

  public get applicableZoneIds(): number[] {
    return this.courseData.applicableZoneIds;
  }
  public set applicableZoneIds(applicableZoneIds: number[]) {
    this.courseData.applicableZoneIds = applicableZoneIds;
  }
  public get applicableClusterIds(): number[] {
    return this.courseData.applicableClusterIds;
  }
  public set applicableClusterIds(applicableClusterIds: number[]) {
    this.courseData.applicableClusterIds = applicableClusterIds;
  }
  public get applicableSchoolIds(): number[] {
    return this.courseData.applicableSchoolIds;
  }
  public set applicableSchoolIds(applicableSchoolIds: number[]) {
    this.courseData.applicableSchoolIds = applicableSchoolIds;
  }
  public get trackIds(): string[] {
    return this.courseData.trackIds;
  }
  public set trackIds(trackIds: string[]) {
    this.courseData.trackIds = trackIds;
  }
  public get developmentalRoleIds(): string[] {
    return this.courseData.developmentalRoleIds;
  }
  public set developmentalRoleIds(developmentalRoleIds: string[]) {
    this.courseData.developmentalRoleIds = developmentalRoleIds;
  }
  public get teachingLevels(): string[] {
    return this.courseData.teachingLevels;
  }
  public set teachingLevels(teachingLevels: string[]) {
    this.courseData.teachingLevels = teachingLevels;
  }
  public get teachingSubjectIds(): string[] {
    return this.courseData.teachingSubjectIds;
  }
  public set teachingSubjectIds(teachingSubjectIds: string[]) {
    this.courseData.teachingSubjectIds = teachingSubjectIds;
  }
  public get teachingCourseStudyIds(): string[] {
    return this.courseData.teachingCourseStudyIds;
  }
  public set teachingCourseStudyIds(teachingCourseStudyIds: string[]) {
    this.courseData.teachingCourseStudyIds = teachingCourseStudyIds;
  }
  public get cocurricularActivityIds(): string[] {
    return this.courseData.cocurricularActivityIds;
  }
  public set cocurricularActivityIds(cocurricularActivityIds: string[]) {
    this.courseData.cocurricularActivityIds = cocurricularActivityIds;
  }
  public get jobFamily(): string[] {
    return this.courseData.jobFamily;
  }
  public set jobFamily(jobFamily: string[]) {
    this.courseData.jobFamily = jobFamily;
  }
  public get easSubstantiveGradeBandingIds(): string[] {
    return this.courseData.easSubstantiveGradeBandingIds;
  }
  public set easSubstantiveGradeBandingIds(easSubstantiveGradeBandingIds: string[]) {
    this.courseData.easSubstantiveGradeBandingIds = easSubstantiveGradeBandingIds;
  }
  public get displayMaximumOfPlacesPerSchool(): boolean {
    return this.pdActivityType === MetadataId.Conference_Seminar_Forum;
  }
  public get registrationMethod(): RegistrationMethod {
    return this.courseData.registrationMethod;
  }
  public set registrationMethod(registrationMethod: RegistrationMethod) {
    this.courseData.registrationMethod = registrationMethod;
  }
  //#endregion

  //#region Course Planning
  public get natureOfCourse(): string {
    return this.courseData.natureOfCourse;
  }
  public set natureOfCourse(natureOfCourse: string) {
    this.courseData.natureOfCourse = natureOfCourse;
  }
  public get numOfPlannedClass(): number {
    return this.courseData.numOfPlannedClass;
  }
  public set numOfPlannedClass(numOfPlannedClass: number) {
    this.courseData.numOfPlannedClass = numOfPlannedClass;
  }
  public get numOfSessionPerClass(): number {
    return this.courseData.numOfSessionPerClass;
  }
  public set numOfSessionPerClass(numOfSessionPerClass: number) {
    this.courseData.numOfSessionPerClass = numOfSessionPerClass;
  }
  public get numOfHoursPerClass(): number {
    return this.courseData.numOfHoursPerClass;
  }
  public get numOfHoursPerSession(): number {
    return this.courseData.numOfHoursPerSession;
  }
  public set numOfHoursPerSession(categnumOfHoursPerSessionoryIds: number) {
    this.courseData.numOfHoursPerSession = categnumOfHoursPerSessionoryIds;
  }
  public get numOfMinutesPerSession(): number {
    return this.courseData.numOfMinutesPerSession;
  }
  public set numOfMinutesPerSession(numOfMinutesPerSession: number) {
    this.courseData.numOfMinutesPerSession = numOfMinutesPerSession;
  }
  public get totalHoursAttendWithinYear(): number {
    return this.courseData.totalHoursAttendWithinYear;
  }
  public get minParticipantPerClass(): number {
    return this.courseData.minParticipantPerClass;
  }
  public set minParticipantPerClass(minParticipantPerClass: number) {
    this.courseData.minParticipantPerClass = minParticipantPerClass;
  }
  public get maxParticipantPerClass(): number {
    return this.courseData.maxParticipantPerClass;
  }
  public set maxParticipantPerClass(maxParticipantPerClass: number) {
    this.courseData.maxParticipantPerClass = maxParticipantPerClass;
  }
  public get planningPublishDate(): Date {
    return this.courseData.planningPublishDate;
  }
  public set planningPublishDate(planningPublishDate: Date) {
    this.courseData.planningPublishDate = planningPublishDate;
  }
  public get planningArchiveDate(): Date {
    return this.courseData.planningArchiveDate;
  }
  public set planningArchiveDate(planningArchiveDate: Date) {
    this.courseData.planningArchiveDate = planningArchiveDate;
  }
  public get willArchiveCommunity(): boolean {
    return this.courseData.willArchiveCommunity;
  }
  public set willArchiveCommunity(willArchiveCommunity: boolean) {
    this.courseData.willArchiveCommunity = willArchiveCommunity;
  }
  public get startDate(): Date {
    return this.courseData.startDate;
  }
  public set startDate(startDate: Date) {
    this.courseData.startDate = startDate;
  }
  public get expiredDate(): Date {
    return this.courseData.expiredDate;
  }
  public set expiredDate(expiredDate: Date) {
    this.courseData.expiredDate = expiredDate;
  }
  public get maxReLearningTimes(): number {
    return this.courseData.maxReLearningTimes;
  }
  public set maxReLearningTimes(maxReLearningTimes: number) {
    this.courseData.maxReLearningTimes = maxReLearningTimes;
  }
  public get courseType(): CourseType {
    return this.courseData.courseType;
  }
  public set courseType(courseType: CourseType) {
    this.courseData.courseType = courseType;
  }
  public get pdActivityPeriods(): string[] {
    return this.courseData.pdActivityPeriods;
  }
  public set pdActivityPeriods(pdActivityPeriods: string[]) {
    this.courseData.pdActivityPeriods = pdActivityPeriods;
  }
  //#endregion

  //#region Evaluation And ECertificate
  public get preCourseEvaluationFormId(): string {
    return this.courseData.preCourseEvaluationFormId;
  }
  public set preCourseEvaluationFormId(preCourseEvaluationFormId: string) {
    this.courseData.preCourseEvaluationFormId = preCourseEvaluationFormId;
  }
  public get postCourseEvaluationFormId(): string {
    return this.courseData.postCourseEvaluationFormId;
  }
  public set postCourseEvaluationFormId(postCourseEvaluationFormId: string) {
    this.courseData.postCourseEvaluationFormId = postCourseEvaluationFormId;
  }
  public get eCertificateTemplateId(): string {
    return this.courseData.eCertificateTemplateId;
  }
  public set eCertificateTemplateId(eCertificateTemplateId: string) {
    this.courseData.eCertificateTemplateId = eCertificateTemplateId;
  }
  public get eCertificatePrerequisite(): string {
    return this.courseData.eCertificatePrerequisite;
  }
  public set eCertificatePrerequisite(eCertificatePrerequisite: string) {
    this.courseData.eCertificatePrerequisite = eCertificatePrerequisite;
  }

  public get courseNameInECertificate(): string {
    return this.courseData.courseNameInECertificate;
  }

  public set courseNameInECertificate(courseNameInECertificate: string) {
    this.courseData.courseNameInECertificate = courseNameInECertificate;
  }
  //#endregion

  //#region Administration
  public get firstAdministratorId(): string {
    return this.courseData.firstAdministratorId;
  }
  public set firstAdministratorId(firstAdministratorId: string) {
    this.courseData.firstAdministratorId = firstAdministratorId;
  }
  public get secondAdministratorId(): string {
    return this.courseData.secondAdministratorId;
  }
  public set secondAdministratorId(secondAdministratorId: string) {
    this.courseData.secondAdministratorId = secondAdministratorId;
  }
  public get primaryApprovingOfficerId(): string {
    return this.courseData.primaryApprovingOfficerId;
  }
  public set primaryApprovingOfficerId(primaryApprovingOfficerId: string) {
    this.courseData.primaryApprovingOfficerId = primaryApprovingOfficerId;
  }
  public get alternativeApprovingOfficerId(): string {
    return this.courseData.alternativeApprovingOfficerId;
  }
  public set alternativeApprovingOfficerId(alternativeApprovingOfficerId: string) {
    this.courseData.alternativeApprovingOfficerId = alternativeApprovingOfficerId;
  }
  public get collaborativeContentCreatorIds(): string[] {
    return this.courseData.collaborativeContentCreatorIds;
  }
  public set collaborativeContentCreatorIds(collaborativeContentCreatorIds: string[]) {
    this.courseData.collaborativeContentCreatorIds = collaborativeContentCreatorIds;
  }
  public get courseFacilitatorId(): string | null {
    return this.courseData.courseFacilitatorIds.length === 0 ? null : this.courseData.courseFacilitatorIds[0];
  }
  public set courseFacilitatorId(courseFacilitatorId: string | null) {
    this.courseData.courseFacilitatorIds = courseFacilitatorId ? [courseFacilitatorId] : [];
  }

  public get courseCoFacilitatorId(): string | null {
    return this.courseData.courseCoFacilitatorIds.length === 0 ? null : this.courseData.courseCoFacilitatorIds[0];
  }
  public set courseCoFacilitatorId(courseCoFacilitatorId: string | null) {
    this.courseData.courseCoFacilitatorIds = courseCoFacilitatorId ? [courseCoFacilitatorId] : [];
  }

  //#endregion

  public updateCourseData(course: Course): void {
    if (course.id == null && course.moeOfficerId == null) {
      course = this.setMoeOfficerId(course, this.currentUser.extId);
    }
    this.moeOfficerItems = this.usersDic[course.moeOfficerId]
      ? this.moeOfficerItems.concat(this.usersDic[course.moeOfficerId])
      : this.moeOfficerItems;
    this.originCourseData = Utils.cloneDeep(course);
    this.courseData = Utils.cloneDeep(course);
  }

  public setCourseEvaluation(items: FormModel[]): void {
    this.preCourseFormItems = items.filter(_ => _.surveyType === CourseDetailViewModel.preCourseFormSurveyType);
    this.postCourseFormItems = items.filter(_ => _.surveyType === CourseDetailViewModel.postCourseFormSurveyType);
    this.preCourseFormsDic = Utils.toDictionary(this.preCourseFormItems, p => p.id);
    this.postCourseFormsDic = Utils.toDictionary(this.postCourseFormItems, p => p.id);
  }

  public setECertificateTemplates(items: ECertificateTemplateModel[]): void {
    this._eCertificateTemplateItems = items;
    this.eCertificateTemplateDic = Utils.toDictionary(this.eCertificateTemplateItems, p => p.id);
  }

  public updateDepartments(data: DepartmentInfoModel[]): void {
    this.divisionSelectItems = this.divisionSelectItems.map(p => this.departmentsDic[p.id]).filter(p => p != null);
    this.branchSelectItems = this.branchSelectItems.map(p => this.departmentsDic[p.id]).filter(p => p != null);
    this.clusterSelectItems = this.clusterSelectItems.map(p => this.departmentsDic[p.id]).filter(p => p != null);
    this.schoolSelectItems = this.schoolSelectItems.map(p => this.departmentsDic[p.id]).filter(p => p != null);
    this.zoneSelectItems = this.zoneSelectItems.map(p => this.departmentsDic[p.id]).filter(p => p != null);
  }

  public onChangeOtherTrainingAgencyReason(check: boolean, otherTrainingAgencyReason: OtherTrainingAgencyReasonType): void {
    if (check) {
      this._localOtherTrainingAgencyReason = Utils.uniq((this._localOtherTrainingAgencyReason || []).concat([otherTrainingAgencyReason]));
    } else {
      this._localOtherTrainingAgencyReason = this._localOtherTrainingAgencyReason
        ? this._localOtherTrainingAgencyReason.filter(i => i !== otherTrainingAgencyReason)
        : [];
    }

    this.changeOtherTrainingAgencyReason();
  }

  public hasCheckOtherTrainingAgencyReason(otherTrainingAgencyReason: OtherTrainingAgencyReasonType): boolean {
    return (
      this.courseData.otherTrainingAgencyReason &&
      this.courseData.otherTrainingAgencyReason.findIndex(i => i === otherTrainingAgencyReason) !== -1
    );
  }

  public onTrainingAgencyCheckedChange(checked: boolean, value: TrainingAgencyType): void {
    if (checked) {
      this._localTrainingAgency = Utils.uniq(this._localTrainingAgency).concat([value]);
    } else {
      this._localTrainingAgency = this._localTrainingAgency.filter(i => i !== value);
    }

    this.updateTrainingAgency();
  }

  public onMaxReLearningTimesCheckedChange(checked: boolean, value: number): void {
    this.maxReLearningTimes = value;
  }

  public trainingAgencyContains(trainingAgency: TrainingAgencyType | string): boolean {
    return this.courseData.trainingAgency && this.courseData.trainingAgency.findIndex(i => i === trainingAgency) >= 0;
  }

  public setPlaceOfWork(value: PlaceOfWorkType): void {
    this.placeOfWork = value;

    if (this.placeOfWork === PlaceOfWorkType.ApplicableForEveryone) {
      this.applicableDivisionIds = null;
      this.applicableBranchIds = null;
      this.applicableZoneIds = null;
      this.applicableClusterIds = null;
      this.applicableSchoolIds = null;
    }
  }

  public setDefaultValueForNotMicrolearning(): void {
    this.registrationMethod = this.registrationMethod == null ? RegistrationMethod.Restricted : this.registrationMethod;
    this.eCertificatePrerequisite =
      this.eCertificatePrerequisite == null ? PrerequisiteCertificateType.CompletionCourseEvaluation : this.eCertificatePrerequisite;
  }

  public clearDataHiddenFieldWhenMicrolearning(): void {
    this.learningMode = MetadataId.ELearning;
    this.checkMaxReLearningTimesUnlimited = true;
    this.externalCode = null;
    this.durationHours = null;
    this.firstAdministratorId = null;
    this.secondAdministratorId = null;
    this.courseFacilitatorId = null;
    this.numOfPlannedClass = null;
    this.numOfSessionPerClass = null;
    this.numOfHoursPerSession = null;
    this.planningPublishDate = null;
    this.minParticipantPerClass = null;
    this.maxParticipantPerClass = null;
    this.postCourseEvaluationFormId = null;
    this.eCertificateTemplateId = null;
    this.eCertificatePrerequisite = null;
    this.otherTrainingAgencyReason = null;
    this.moeOfficerId = null;
    this.moeOfficerPhoneNumber = null;
    this.moeOfficerEmail = null;
    this.registrationMethod = null;
  }

  public canViewFundingAndSubsidy(): boolean {
    return this.categoryIds.includes(MetadataId.Funding_And_Subsidy);
  }

  public dataHasChanged(): boolean {
    return Utils.isDifferent(this.originCourseData, this.courseData);
  }

  public buildCourseLevelSelectItems(courseLevels: MetadataTagModel[], orderedCourseLevelDislayTexts: string[]): MetadataTagModel[] {
    if (courseLevels === undefined) {
      return [];
    }
    const courseLevelsDict = Utils.toDictionary(courseLevels, p => p.displayText);
    return orderedCourseLevelDislayTexts.map(p => courseLevelsDict[p]);
  }

  public getNotApplicableItemDisplayText(): string {
    return NOT_APPLICABLE_ITEM_DISPLAY_TEXT;
  }

  public getMetadataTagChilds(item: MetadataTagModel): Observable<MetadataTagModel[]> {
    if (item.childs == null) {
      return of([]);
    }
    return of(item.childs);
  }

  public isMetadataTagHasChilds(item: MetadataTagModel): boolean {
    return item.childs != null && item.childs.length > 0;
  }

  public hasOnlyOneServiceSchemesChecked(): boolean {
    return this.serviceSchemeIds && this.serviceSchemeIds.length === 1;
  }

  public serviceSchemesContains(tagId: MetadataId): boolean {
    return (
      this.serviceSchemeIds && this.serviceSchemeIds.find(p => this.metadataTagsDic[p] && this.metadataTagsDic[p].tagId === tagId) != null
    );
  }

  /**
   * fieldName: property name of Course, same as formControlName
   */

  // I will refactor code group case
  public isViewOnlyForField(fieldName: string, isViewMode: boolean = false): boolean {
    if (isViewMode) {
      return true;
    }

    switch (fieldName) {
      case 'thumbnailUrl':
      case 'otherTrainingAgencyReason':
      case 'ownerDivisionIds':
      case 'ownerBranchIds':
      case 'partnerOrganisationIds':
      case 'moeOfficerId':
      case 'moeOfficerPhoneNumber':
      case 'expiredDate':
      case 'planningArchiveDate':
      case 'pdActivityPeriods':
      case 'minParticipantPerClass':
      case 'maxParticipantPerClass':
      case 'maxReLearningTimes':
      case 'firstAdministratorId':
      case 'secondAdministratorId':
      case 'primaryApprovingOfficerId':
      case 'alternativeApprovingOfficerId':
      case 'courseFacilitatorId':
      case 'courseCoFacilitatorId':
      case 'moeOfficerEmail':
      case 'trainingAgency':
        return this.courseData.isCompletingCourseForPlanning() || !this.courseData.isEditableEvenClassStarted();
      case 'copyrightOwner':
      case 'acknowledgementAndCredit':
      case 'remarks':
      case 'notionalCost':
      case 'courseFee':
      case 'eCertificateTemplateId':
      case 'collaborativeContentCreatorIds':
      case 'registrationMethod':
      case 'allowPersonalDownload':
      case 'allowNonCommerInMOEReuseWithModification':
      case 'allowNonCommerReuseWithModification':
      case 'allowNonCommerInMoeReuseWithoutModification':
      case 'allowNonCommerReuseWithoutModification':
        return !this.courseData.isEditableEvenClassStarted();
      case 'courseName':
      case 'categoryIds':
      case 'learningMode':
      case 'externalCode':
      case 'pdActivityType':
      case 'courseObjective':
      case 'nieAcademicGroups':
      case 'serviceSchemeIds':
      case 'learningFrameworkIds':
      case 'subjectAreaIds':
      case 'pdAreaThemeId':
      case 'courseLevel':
      case 'applicableDivisionIds':
      case 'applicableBranchIds':
      case 'applicableZoneIds':
      case 'applicableClusterIds':
      case 'applicableSchoolIds':
      case 'jobFamily':
      case 'cocurricularActivityIds':
      case 'easSubstantiveGradeBandingIds':
      case 'numOfPlannedClass':
      case 'numOfSessionPerClass':
      case 'numOfHoursPerSession':
      case 'numOfMinutesPerSession':
      case 'planningPublishDate':
      case 'startDate':
      case 'trackIds':
      case 'developmentalRoleIds':
      case 'teachingLevels':
      case 'teachingCourseStudyIds':
      case 'metadataKeys':
      case 'numOfSchoolLeader':
      case 'numOfSeniorOrLeadTeacher':
      case 'numOfMiddleManagement':
      case 'numOfBeginningTeacher':
      case 'numOfExperiencedTeacher':
        return this.courseData.isCompletingCourseForPlanning() || !this.courseData.isEditableBeforeStarted();
      case 'durationMinutes':
      case 'durationHours':
      case 'eCertificatePrerequisite':
      case 'courseOutlineStructure':
      case 'description':
      case 'placeOfWork':
      case 'maximumPlacesPerSchool':
      case 'prerequisiteCourseIds':
      case 'teachingSubjectIds':
      case 'natureOfCourse':
      case 'postCourseEvaluationFormId':
        return !this.courseData.isEditableBeforeStarted();
      default:
        return true;
    }
  }

  //#region Departments
  private buildBranchSelectItems(): DepartmentInfoModel[] {
    return this.departments.filter(p => p.departmentType === OrganizationUnitLevelEnum.Branch);
  }
  private buildOwnerBranchSelectItems(): DepartmentInfoModel[] {
    return this.departments.filter(p => p.departmentType === OrganizationUnitLevelEnum.Branch);
  }
  private buildClusterSelectItems(): DepartmentInfoModel[] {
    return this.departments.filter(p => p.departmentType === OrganizationUnitLevelEnum.Cluster);
  }
  private buildZoneSelectItems(): DepartmentInfoModel[] {
    const schoolDivision = this.departments.find(p => p.isSchoolDivision());
    return schoolDivision == null
      ? []
      : this.departments.filter(p => p.departmentType === OrganizationUnitLevelEnum.Branch && p.parentDepartmentId === schoolDivision.id);
  }
  private buildSchoolSelectItems(): DepartmentInfoModel[] {
    return this.departments.filter(p => p.departmentType === OrganizationUnitLevelEnum.School);
    // return Utils.rightJoinBy(
    //   this.departments.filter(p => p.departmentType === OrganizationUnitLevelEnum.School),
    //   this.applicableClusterIds,
    //   p => p.parentDepartmentId,
    //   p => p
    // );
  }
  //#endregion

  private changeOtherTrainingAgencyReason(): void {
    this.courseData.otherTrainingAgencyReason = this._localOtherTrainingAgencyReason
      ? this._localOtherTrainingAgencyReason.concat(
          this.checkOtherTrainingAgencyReasonOther && this._otherTrainingAgencyReasonOther ? [this._otherTrainingAgencyReasonOther] : []
        )
      : [];
  }

  private updateTrainingAgency(): void {
    this.courseData.trainingAgency = this._localTrainingAgency
      ? this._localTrainingAgency.concat(this.checkTrainingAgencyOther && this._trainingAgencyOther ? [this._trainingAgencyOther] : [])
      : [];
  }

  private processTrainingAgencyOtherCheckedStates(): void {
    this.checkTrainingAgencyOther = false;
    this.checkOtherTrainingAgencyReasonOther = false;

    if (this._localTrainingAgency) {
      this._localTrainingAgency.forEach(i => {
        if (!TrainingAgencyType[i]) {
          this.checkTrainingAgencyOther = true;
          this._trainingAgencyOther = i;
        }
      });
    }

    if (this._localOtherTrainingAgencyReason) {
      this._localOtherTrainingAgencyReason.forEach(i => {
        if (!OtherTrainingAgencyReasonType[i]) {
          this.checkOtherTrainingAgencyReasonOther = true;
          this._otherTrainingAgencyReasonOther = i;
        }
      });
    }
  }

  private processMaxReLearningTimesCheckedStates(): void {
    this._checkMaxReLearningTimesOther = this.courseData.maxReLearningTimes !== MAX_INT;
    this._checkMaxReLearningTimesUnlimited = !this._checkMaxReLearningTimesOther;
    if (this._checkMaxReLearningTimesOther === true) {
      this._maxReLearningTimesOther = this.courseData.maxReLearningTimes;
    }
  }

  //#region Metadata

  /**
   * The list include subject level 1,2,3
   */

  private buildEasSubstantiveGradeBandingTree(): MetadataTagModel[] {
    if (this.serviceSchemeIds.length === 0) {
      return [];
    }
    const serviceSchemesDic = Utils.toDictionary(
      this.serviceSchemeIds.filter(p => p === MetadataId.ExecutiveAndAdministrativeStaff),
      p => p
    );
    return MetadataTagModel.buildTree(this.metadataTags, p => serviceSchemesDic[p.tagId] != null, [
      p => p.groupCode === MetadataTagGroupCode.LEARNING_FXS
    ]).filter(p => p.childs != null && p.childs.length > 0);
  }

  private buildSelectedEasSubstantiveGradeBandingTree(): MetadataTagModel[] {
    return MetadataTagModel.filterSelectedTreeNodes(this.easSubstantiveGradeBandingTree, this.easSubstantiveGradeBandingIds);
  }

  private buildEasSubstantiveGradeBandingTreeValues(): string[] {
    return Utils.clone(this.easSubstantiveGradeBandingIds);
  }

  private buildTracksTree(): MetadataTagModel[] {
    return MetadataTagModel.buildTree(this.metadataTags, null, [p => p.groupCode === MetadataTagGroupCode.TRACKS]).filter(
      p => p.childs != null && p.childs.length > 0
    );
  }

  private buildSelectedTracksTree(): MetadataTagModel[] {
    return MetadataTagModel.filterSelectedTreeNodes(this.tracksTree, this.trackIds);
  }

  private buildTracksTreeValues(): string[] {
    return Utils.clone(this.trackIds);
  }

  private buildTeachingCourseStudysTree(): MetadataTagModel[] {
    return MetadataTagModel.buildTree(this.metadataTags, null, [p => p.groupCode === MetadataTagGroupCode.COURSES_OF_STUDY]).filter(
      p => p.childs != null && p.childs.length > 0
    );
  }

  private buildSelectedTeachingCourseStudysTree(): MetadataTagModel[] {
    return MetadataTagModel.filterSelectedTreeNodes(this.teachingCourseStudysTree, this.teachingCourseStudyIds);
  }

  private buildTeachingCourseStudysTreeValues(): string[] {
    return Utils.clone(this.teachingCourseStudyIds);
  }

  private buildJobFamilysTree(): MetadataTagModel[] {
    return MetadataTagModel.buildTree(this.metadataTags, null, [p => p.groupCode === MetadataTagGroupCode.JOB_FAMILIES]).filter(
      p => p.childs != null && p.childs.length > 0
    );
  }

  private buildSelectedJobFamilysTree(): MetadataTagModel[] {
    return MetadataTagModel.filterSelectedTreeNodes(this.jobFamilysTree, this.jobFamily);
  }

  private buildJobFamilysTreeValues(): string[] {
    return Utils.clone(this.jobFamily);
  }

  private buildTeachingLevelsTree(): MetadataTagModel[] {
    return MetadataTagModel.buildTree(this.metadataTags, null, [p => p.groupCode === MetadataTagGroupCode.TEACHING_LEVELS]).filter(
      p => p.childs != null && p.childs.length > 0
    );
  }

  private buildSelectedTeachingLevelsTree(): MetadataTagModel[] {
    return MetadataTagModel.filterSelectedTreeNodes(this.teachingLevelsTree, this.teachingLevels);
  }

  private buildTeachingLevelsTreeValues(): string[] {
    return Utils.clone(this.teachingLevels);
  }

  private buildDevelopmentalRolesTree(): MetadataTagModel[] {
    if (this.serviceSchemeIds.length === 0) {
      return [];
    }
    const serviceSchemesDic = Utils.toDictionary(this.serviceSchemeIds, p => p);
    return MetadataTagModel.buildTree(this.metadataTags, p => serviceSchemesDic[p.tagId] != null, [
      p => p.groupCode === MetadataTagGroupCode.DEVROLES
    ]).filter(p => p.childs != null && p.childs.length > 0);
  }

  private buildSelectedDevelopmentalRolesTree(): MetadataTagModel[] {
    return MetadataTagModel.filterSelectedTreeNodes(this.developmentalRolesTree, this.developmentalRoleIds);
  }

  private buildDevelopmentalRolesTreeValues(): string[] {
    return Utils.clone(this.developmentalRoleIds);
  }

  private buildSubjectAreaTree(): MetadataTagModel[] {
    if (this.serviceSchemeIds.length === 0) {
      return [];
    }
    const serviceSchemesDic = Utils.toDictionary(this.serviceSchemeIds, p => p);
    return MetadataTagModel.buildTree(this.metadataTags, p => serviceSchemesDic[p.tagId] != null, [
      p => p.groupCode === MetadataTagGroupCode.PDO_TAXONOMY && p.codingScheme != null
    ]).filter(p => p.childs != null && p.childs.length > 0);
  }

  private buildSelectedSubjectAreaTree(): MetadataTagModel[] {
    return MetadataTagModel.filterSelectedTreeNodes(this.subjectAreaTree, this.subjectAreaIds);
  }

  private buildSubjectAreaTreeValues(): string[] {
    return Utils.clone(this.subjectAreaIds);
  }

  private buildLearningFrameworkTree(): MetadataTagModel[] {
    if (this.serviceSchemeIds.length === 0) {
      return [];
    }
    const serviceSchemesDic = Utils.toDictionary(this.serviceSchemeIds, p => p);
    return MetadataTagModel.buildTree(
      this.metadataTags,
      p => serviceSchemesDic[p.tagId] != null,
      [p => p.groupCode === MetadataTagGroupCode.LEARNING_FXS],
      true
    ).filter(p => p.childs != null && p.childs.length > 0);
  }

  private buildSelectedLearningFrameworkTree(): MetadataTagModel[] {
    return MetadataTagModel.filterSelectedTreeNodes(this.learningFrameworkTree, this.learningFrameworkIds);
  }

  private buildLearningFrameworkTreeValues(): string[] {
    return Utils.clone(this.learningFrameworkIds);
  }

  private buildLearningDimensionAreaTree(): MetadataTagModel[] {
    if (this.learningFrameworkIds.length === 0) {
      return [];
    }
    const learningFrameworkDic = Utils.toDictionary(this.learningFrameworkIds, p => p);
    return MetadataTagModel.buildTree(
      this.metadataTags,
      p => learningFrameworkDic[p.tagId] != null,
      [p => p.type !== MetadataTagType.TeacherOutcome],
      true
    ).filter(p => p.childs != null && p.childs.length > 0);
  }

  private buildSelectedLearningDimensionAreaTree(): MetadataTagModel[] {
    return MetadataTagModel.filterSelectedTreeNodes(this.learningDimensionAreaTree, this.learningDimensionAreas);
  }

  private buildLearningDimensionAreaTreeValues(): string[] {
    return Utils.clone(this.learningDimensionAreas);
  }

  private buildTeacherOutcomeTree(): MetadataTagModel[] {
    if (this.learningFrameworkIds.length === 0) {
      return [];
    }
    const learningFrameworkDic = Utils.toDictionary(this.learningFrameworkIds, p => p);
    return MetadataTagModel.buildTree(
      this.metadataTags,
      p => learningFrameworkDic[p.tagId] != null,
      [p => p.type === MetadataTagType.TeacherOutcome],
      true
    ).filter(p => p.childs != null && p.childs.length > 0);
  }

  private buildSelectedTeacherOutcomeTree(): MetadataTagModel[] {
    return MetadataTagModel.filterSelectedTreeNodes(this.teacherOutcomeTree, this.teacherOutcomeIds);
  }

  private buildTeacherOutcomeTreeValues(): string[] {
    return Utils.clone(this.teacherOutcomeIds);
  }

  private setMetadataTagsDicInfo(metadataTags: MetadataTagModel[]): void {
    this.metadataTagsDic = Utils.toDictionary(metadataTags, p => p.tagId);
    this.metadataTagsDicByGroupCode = Utils.toDictionaryGroupBy(
      metadataTags.filter(p => p.groupCode != null),
      p => p.groupCode,
      items => Utils.orderBy(items, p => p.displayText)
    );
  }

  private clearLicencePermission(): void {
    this.courseData.allowPersonalDownload = false;
    this.courseData.allowNonCommerInMOEReuseWithModification = false;
    this.courseData.allowNonCommerReuseWithModification = false;
    this.courseData.allowNonCommerInMoeReuseWithoutModification = false;
    this.courseData.allowNonCommerReuseWithoutModification = false;
  }

  private setMoeOfficerId(course: Course, moeOfficerId: string): Course {
    return Utils.clone(course, p => {
      p.moeOfficerId = moeOfficerId;
      const item = this.moeOfficerItems.find(i => i.extId === moeOfficerId);
      p.moeOfficerPhoneNumber = item && item.phone ? item.phone : p.moeOfficerPhoneNumber;
      p.moeOfficerEmail = item ? item.emails : p.moeOfficerEmail;
    });
  }

  private _initTrainingAgency(allItems: string[]): void {
    const trainingAgencyTypeValuesDic = Utils.toDictionary(Object.keys(TrainingAgencyType));
    const otherTrainingAgencyVal = allItems.find(p => trainingAgencyTypeValuesDic[p] == null);
    this._localTrainingAgency = allItems.filter(p => trainingAgencyTypeValuesDic[p] != null);
    if (otherTrainingAgencyVal != null && otherTrainingAgencyVal !== '') {
      this._checkTrainingAgencyOther = true;
      this._trainingAgencyOther = otherTrainingAgencyVal;
    }
  }

  private _initOtherTrainingAgencyReason(allItems: string[]): void {
    const otherTrainingAgencyReasonTypeValuesDic = Utils.toDictionary(Object.keys(OtherTrainingAgencyReasonType));
    const otherTrainingAgencyReasonVal = allItems.find(p => otherTrainingAgencyReasonTypeValuesDic[p] == null);
    this._localOtherTrainingAgencyReason = allItems.filter(p => otherTrainingAgencyReasonTypeValuesDic[p] != null);
    if (otherTrainingAgencyReasonVal != null && otherTrainingAgencyReasonVal !== '') {
      this._checkOtherTrainingAgencyReasonOther = true;
      this._otherTrainingAgencyReasonOther = otherTrainingAgencyReasonVal;
    }
  }
  //#endregion
}

function create(
  getUsersByIdsFn: (ids: string[]) => Observable<UserInfoModel[]>,
  getCourseByIdsFn: (ids: string[]) => Observable<Course[]>,
  getOrganisationUnitIdsFn: (ids: number[]) => Observable<DepartmentInfoModel[]>,
  getCoursePlanningCycleByIdFn: (coursePlanningCycleId: string) => Observable<CoursePlanningCycle>,
  getECertificateTemplateFn: (id: string) => Observable<ECertificateTemplateModel>,
  course?: Course,
  metadataTags: MetadataTagModel[] = [],
  forms: FormModel[] = [],
  coursePlanningCycleId?: string
): Observable<CourseDetailViewModel> {
  const courseData = course == null ? new Course() : Utils.clone(course);
  const currentUser = UserInfoModel.getMyUserInfo();
  if (coursePlanningCycleId != null) {
    courseData.coursePlanningCycleId = coursePlanningCycleId;
  }

  const coursePlanningCycleObs =
    courseData && courseData.coursePlanningCycleId && getCoursePlanningCycleByIdFn
      ? getCoursePlanningCycleByIdFn(courseData.coursePlanningCycleId)
      : of(null);

  const ecertificateTemplateObs =
    courseData && courseData.eCertificateTemplateId && getECertificateTemplateFn
      ? getECertificateTemplateFn(courseData.eCertificateTemplateId)
      : of(null);

  if (course == null) {
    return combineLatest(coursePlanningCycleObs, ecertificateTemplateObs).pipe(
      map(
        ([coursePlanningCycle, ecertificateTemplate]) =>
          new CourseDetailViewModel(courseData, {}, metadataTags, [], {}, forms, ecertificateTemplate, coursePlanningCycle)
      )
    );
  }

  return combineLatest(
    course.getAllUserIds().length === 0 ? of([]) : getUsersByIdsFn(course.getAllUserIds()),
    course.prerequisiteCourseIds.length === 0 ? of([]) : getCourseByIdsFn(course.prerequisiteCourseIds),
    course.getAllDepartmentIds(currentUser).length === 0 ? of([]) : getOrganisationUnitIdsFn(course.getAllDepartmentIds(currentUser)),
    coursePlanningCycleObs,
    ecertificateTemplateObs
  ).pipe(
    map(
      ([users, prerequisiteCourses, departments, coursePlanningCycle, ecertificateTemplate]) =>
        new CourseDetailViewModel(
          courseData,
          Utils.toDictionary(users.concat(currentUser), p => p.id),
          metadataTags,
          departments,
          Utils.toDictionary(prerequisiteCourses, p => p.id),
          forms,
          ecertificateTemplate,
          coursePlanningCycle
        )
    )
  );
}
