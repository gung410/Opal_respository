import { DateUtils, MAX_INT, Utils } from '@opal20/infrastructure';
import { SystemRoleEnum, UserInfoModel } from '../../share/models/user-info.model';

import { CAM_PERMISSIONS } from '../../share/permission-keys/cam-permission-key';
import { ClassRun } from './classrun.model';
import { CourseContentItemModel } from './course-content-item.model';
import { CoursePlanningCycle } from './course-planning-cycle.model';
import { CourseStatus } from '../../share/models/course-status.enum';
import { CourseType } from './course-type.model';
import { LMM_PERMISSIONS } from '@opal20/domain-api/share/permission-keys/lmm-permission-key';
import { MetadataId } from './../../tagging/models/metadata-id-enum';
import { PlaceOfWorkType } from './place-of-word-type.model';
import { PrerequisiteCertificateType } from './prerequisite-ecertificate.model';
import { RegistrationMethod } from './registration-method.model';

export enum ContentStatus {
  Draft = 'Draft',
  Published = 'Published',
  Unpublished = 'Unpublished',
  Expired = 'Expired',
  PendingApproval = 'PendingApproval',
  Approved = 'Approved',
  Rejected = 'Rejected'
}

export interface ICourse {
  id?: string;

  // Overview Info
  courseCode: string;
  courseName: string;
  thumbnailUrl: string;
  description: string;
  durationMinutes: number;
  durationHours: number;
  pdActivityType: string;
  learningMode: string;
  externalCode: string | null;
  courseOutlineStructure: string;
  courseObjective: string;
  categoryIds: string[];

  // Provider Info
  ownerDivisionIds: number[];
  ownerBranchIds: number[];
  partnerOrganisationIds: number[];
  moeOfficerId: string;
  moeOfficerPhoneNumber: string;
  moeOfficerEmail: string;
  notionalCost: number;
  courseFee: number;
  trainingAgency: string[];
  otherTrainingAgencyReason: string[];
  nieAcademicGroups: string[];

  // Copyright
  allowPersonalDownload: boolean;
  allowNonCommerInMOEReuseWithModification: boolean;
  allowNonCommerReuseWithModification: boolean;
  allowNonCommerInMoeReuseWithoutModification: boolean;
  allowNonCommerReuseWithoutModification: boolean;
  copyrightOwner: string;
  acknowledgementAndCredit: string;
  remarks: string;

  // Target Audience
  maximumPlacesPerSchool: number;
  registrationMethod?: RegistrationMethod;
  numOfSchoolLeader: number;
  numOfSeniorOrLeadTeacher: number;
  numOfMiddleManagement: number;
  numOfBeginningTeacher: number;
  numOfExperiencedTeacher: number;
  placeOfWork: PlaceOfWorkType;
  prerequisiteCourseIds: string[];
  applicableDivisionIds: number[];
  applicableBranchIds: number[];
  applicableZoneIds: number[];
  applicableClusterIds: number[];
  applicableSchoolIds: number[];
  trackIds: string[];
  developmentalRoleIds: string[];
  teachingLevels: string[];
  teachingSubjectIds: string[];
  teachingCourseStudyIds: string[];
  cocurricularActivityIds: string[];
  jobFamily: string[];
  easSubstantiveGradeBandingIds: string[];

  // Metadata
  courseLevel?: string;
  pdAreaThemeId?: string;
  serviceSchemeIds: string[];
  subjectAreaIds: string[];
  learningFrameworkIds: string[];
  learningDimensionIds: string[];
  learningAreaIds: string[];
  learningSubAreaIds: string[];
  teacherOutcomeIds: string[];
  metadataKeys: string[];

  // Course Planning
  natureOfCourse: string;
  numOfPlannedClass: number;
  numOfSessionPerClass: number;
  numOfHoursPerSession: number;
  numOfMinutesPerSession: number;
  minParticipantPerClass: number;
  maxParticipantPerClass: number;
  planningPublishDate?: Date;
  planningArchiveDate?: Date;
  willArchiveCommunity: boolean;
  courseType: CourseType;
  pdActivityPeriods: string[];
  maxReLearningTimes: number;
  startDate?: Date;
  expiredDate?: Date;

  // Evaluation And ECertificate
  preCourseEvaluationFormId: string;
  postCourseEvaluationFormId: string;
  eCertificateTemplateId: string;
  eCertificatePrerequisite: string;
  courseNameInECertificate: string;

  // Administration
  firstAdministratorId: string;
  secondAdministratorId: string;
  primaryApprovingOfficerId: string;
  alternativeApprovingOfficerId: string;
  collaborativeContentCreatorIds: string[];
  courseFacilitatorIds: string[];
  courseCoFacilitatorIds: string[];

  // System
  version: string;
  status: CourseStatus;
  contentStatus: ContentStatus;
  isDeleted: boolean;
  changedBy: string;
  createdBy: string;
  externalId: string;
  createdDate: Date;
  changedDate?: Date;
  submittedDate?: Date;
  submittedContentDate?: Date;
  approvalDate?: Date;
  approvalContentDate?: Date;
  publishedContentDate?: Date;
  hasContent?: boolean;
  isMigrated: boolean;
  coursePlanningCycleId?: string;
  verifiedDate?: Date;
  archiveDate?: Date;
  archivedBy?: string;
  hasFullRight?: boolean;
}

export class Course implements ICourse {
  public static optionalProps: (keyof ICourse)[] = ['hasContent'];
  public static allowedThumbnailExtensions = ['jpeg', 'jpg', 'gif', 'png', 'svg'];

  public id?: string;

  // Overview Info
  public courseCode: string;
  public courseName: string = '';
  public thumbnailUrl: string;
  public description: string = '';
  public durationMinutes: number = 0;
  public durationHours: number = 0;
  public pdActivityType: string;
  public learningMode: string;
  public externalCode: string | null;
  public courseOutlineStructure: string = '';
  public courseObjective: string = '';
  public categoryIds: string[] = [];
  public contentStatus: ContentStatus = ContentStatus.Draft;

  // Provider Info
  public ownerDivisionIds: number[] = [];
  public ownerBranchIds: number[] = [];
  public partnerOrganisationIds: number[] = [];
  public moeOfficerId: string;
  public moeOfficerPhoneNumber: string = '';
  public moeOfficerEmail: string = '';
  public notionalCost: number = 0;
  public courseFee: number = 0;
  public trainingAgency: string[] = [];
  public otherTrainingAgencyReason: string[] = [];
  public nieAcademicGroups: string[] = [];
  // Metadata
  public courseLevel?: string = null;
  public pdAreaThemeId?: string = null;
  public serviceSchemeIds: string[] = [];
  public subjectAreaIds: string[] = [];
  public learningFrameworkIds: string[] = [];
  public learningDimensionIds: string[] = [];
  public learningAreaIds: string[] = [];
  public learningSubAreaIds: string[] = [];
  public teacherOutcomeIds: string[] = [];
  public metadataKeys: string[] = [];

  // Copyright
  public allowPersonalDownload: boolean = false;
  public allowNonCommerInMOEReuseWithModification: boolean = false;
  public allowNonCommerReuseWithModification: boolean = false;
  public allowNonCommerInMoeReuseWithoutModification: boolean = false;
  public allowNonCommerReuseWithoutModification: boolean = false;
  public copyrightOwner: string = '';
  public acknowledgementAndCredit: string = '';
  public remarks: string = '';

  // Target Audience
  public maximumPlacesPerSchool: number;
  public numOfSchoolLeader: number;
  public numOfSeniorOrLeadTeacher: number;
  public numOfMiddleManagement: number;
  public numOfBeginningTeacher: number;
  public numOfExperiencedTeacher: number;
  public placeOfWork: PlaceOfWorkType = PlaceOfWorkType.ApplicableForEveryone;
  public prerequisiteCourseIds: string[] = [];
  public applicableDivisionIds: number[] = [];
  public applicableBranchIds: number[] = [];
  public applicableZoneIds: number[] = [];
  public applicableClusterIds: number[] = [];
  public applicableSchoolIds: number[] = [];
  public trackIds: string[] = [];
  public developmentalRoleIds: string[] = [];
  public teachingLevels: string[] = [];
  public teachingSubjectIds: string[] = [];
  public teachingCourseStudyIds: string[] = [];
  public cocurricularActivityIds: string[] = [];
  public jobFamily: string[] = [];
  public easSubstantiveGradeBandingIds: string[] = [];
  public registrationMethod?: RegistrationMethod = null;

  // Course Planning
  public natureOfCourse: string;
  public numOfPlannedClass: number = 0;
  public numOfSessionPerClass: number = 0;
  public get numOfHoursPerClass(): number {
    return this.numOfSessionPerClass * (this.numOfHoursPerSession + this.numOfMinutesPerSession / 60);
  }
  public numOfHoursPerSession: number = 0;
  public numOfMinutesPerSession: number = 0;
  public get totalHoursAttendWithinYear(): number {
    return Utils.round(this.numOfHoursPerClass * this.numOfPlannedClass, 2);
  }
  public minParticipantPerClass: number;
  public maxParticipantPerClass: number;
  public planningPublishDate?: Date;
  public planningArchiveDate?: Date;
  public willArchiveCommunity: boolean = true;
  public courseType: CourseType = CourseType.New;
  public pdActivityPeriods: string[] = [];
  public maxReLearningTimes: number = MAX_INT;
  public startDate: Date | undefined = undefined;
  public expiredDate: Date | undefined = undefined;
  // Evaluation And ECertificate
  public preCourseEvaluationFormId: string;
  public postCourseEvaluationFormId: string;
  public eCertificateTemplateId: string;
  public eCertificatePrerequisite: string = PrerequisiteCertificateType.CompletionCourseEvaluation;
  public courseNameInECertificate: string = '';

  // Administration
  public firstAdministratorId: string;
  public secondAdministratorId: string;
  public primaryApprovingOfficerId: string;
  public alternativeApprovingOfficerId: string;
  public collaborativeContentCreatorIds: string[] = [];
  public courseFacilitatorIds: string[] = [];
  public courseCoFacilitatorIds: string[] = [];

  // System
  public version: string;
  public status: CourseStatus = CourseStatus.Draft;
  public isDeleted: boolean;
  public changedBy: string;
  public createdBy: string;
  public externalId: string;
  public createdDate: Date = new Date();
  public changedDate?: Date;
  public submittedDate?: Date;
  public submittedContentDate?: Date;
  public approvalDate?: Date;
  public approvalContentDate?: Date;
  public publishedContentDate?: Date;
  public hasContent?: boolean;
  public coursePlanningCycleId?: string;
  public verifiedDate?: Date;
  public isMigrated: boolean = false;
  public archiveDate?: Date;
  public archivedBy?: string;
  public hasFullRight: boolean = false;

  public get fullTitle(): string {
    return `${this.courseName} ${this.courseCode ? `(${this.courseCode})` : ``}`;
  }

  public static haveViewCoursesPermission(user: UserInfoModel): boolean {
    return (
      this.haveCreateCoursePermission(user) ||
      this.haveApproveCoursePermission(user) ||
      user.hasPermissionPrefix(CAM_PERMISSIONS.ViewCourseList)
    );
  }

  public static haveCreateCoursePermission(user: UserInfoModel): boolean {
    return (
      user.hasAdministratorRoles() ||
      user.hasRole(SystemRoleEnum.CourseContentCreator) ||
      user.hasPermissionPrefix(CAM_PERMISSIONS.CreateEditCourse)
    );
  }

  public static haveApproveCoursePermission(user: UserInfoModel): boolean {
    return (
      user.hasAdministratorRoles() ||
      user.hasRole(SystemRoleEnum.MOEHQContentApprovingOfficer, SystemRoleEnum.SchoolContentApprovingOfficer) ||
      user.hasPermissionPrefix(CAM_PERMISSIONS.CourseApproval)
    );
  }

  public static canViewArchivalCourses(user: UserInfoModel): boolean {
    return user.hasAdministratorRoles() || user.hasRole(SystemRoleEnum.CourseContentCreator);
  }

  public static canViewCoursesStatisticalReport(user: UserInfoModel): boolean {
    return Course.haveApproveCoursePermission(user) || user.hasAdministratorRoles() || user.hasRole(SystemRoleEnum.CourseContentCreator);
  }

  public static createForPlanningCycle(planningCycleId: string | null): Course {
    const result = new Course();
    result.coursePlanningCycleId = planningCycleId;
    return result;
  }

  constructor(data?: ICourse) {
    if (data == null) {
      return;
    }

    this.id = data.id;

    // Overview Info
    this.courseCode = data.courseCode != null ? data.courseCode : this.courseCode;
    this.courseName = data.courseName != null ? data.courseName : this.courseName;
    this.thumbnailUrl = data.thumbnailUrl != null ? data.thumbnailUrl : this.thumbnailUrl;
    this.description = data.description != null ? data.description : this.description;
    this.durationMinutes = data.durationMinutes != null ? data.durationMinutes : this.durationMinutes;
    this.durationHours = data.durationHours != null ? data.durationHours : this.durationHours;
    this.pdActivityType = data.pdActivityType != null ? data.pdActivityType : this.pdActivityType;
    this.learningMode = data.learningMode != null ? data.learningMode : this.learningMode;
    this.externalCode = data.externalCode != null ? data.externalCode : this.externalCode;
    this.courseOutlineStructure = data.courseOutlineStructure != null ? data.courseOutlineStructure : this.courseOutlineStructure;
    this.courseObjective = data.courseObjective != null ? data.courseObjective : this.courseObjective;
    this.categoryIds = Utils.defaultIfNull(data.categoryIds, []).map(p => p.toLowerCase());
    this.contentStatus = data.contentStatus != null ? data.contentStatus : this.contentStatus;

    // Provider Info
    this.ownerDivisionIds = Utils.defaultIfNull(data.ownerDivisionIds, []);
    this.ownerBranchIds = Utils.defaultIfNull(data.ownerBranchIds, []);
    this.partnerOrganisationIds = Utils.defaultIfNull(data.partnerOrganisationIds, []);
    this.moeOfficerId = data.moeOfficerId != null ? data.moeOfficerId : this.moeOfficerId;
    this.moeOfficerPhoneNumber = data.moeOfficerPhoneNumber != null ? data.moeOfficerPhoneNumber : this.moeOfficerPhoneNumber;
    this.moeOfficerEmail = data.moeOfficerEmail != null ? data.moeOfficerEmail : this.moeOfficerEmail;
    this.notionalCost = data.notionalCost != null ? data.notionalCost : this.notionalCost;
    this.courseFee = data.courseFee != null ? data.courseFee : this.courseFee;
    this.trainingAgency = Utils.defaultIfNull(data.trainingAgency, []);
    this.otherTrainingAgencyReason = Utils.defaultIfNull(data.otherTrainingAgencyReason, []);
    this.nieAcademicGroups = Utils.defaultIfNull(data.nieAcademicGroups, []);

    // Metadata
    this.courseLevel = data.courseLevel != null ? data.courseLevel.toLowerCase() : this.courseLevel;
    this.pdAreaThemeId = data.pdAreaThemeId != null ? data.pdAreaThemeId.toLowerCase() : this.pdAreaThemeId;
    this.serviceSchemeIds = Utils.defaultIfNull(data.serviceSchemeIds, []).map(p => p.toLowerCase());
    this.subjectAreaIds = Utils.defaultIfNull(data.subjectAreaIds, []).map(p => p.toLowerCase());
    this.learningFrameworkIds = Utils.defaultIfNull(data.learningFrameworkIds, []).map(p => p.toLowerCase());
    this.learningDimensionIds = Utils.defaultIfNull(data.learningDimensionIds, []).map(p => p.toLowerCase());
    this.learningAreaIds = Utils.defaultIfNull(data.learningAreaIds, []).map(p => p.toLowerCase());
    this.learningSubAreaIds = Utils.defaultIfNull(data.learningSubAreaIds, []).map(p => p.toLowerCase());
    this.teacherOutcomeIds = Utils.defaultIfNull(data.teacherOutcomeIds, []).map(p => p.toLowerCase());
    this.metadataKeys = data.metadataKeys != null ? data.metadataKeys : [];

    // Copyright
    this.allowPersonalDownload = data.allowPersonalDownload != null ? data.allowPersonalDownload : this.allowPersonalDownload;
    this.allowNonCommerInMOEReuseWithModification =
      data.allowNonCommerInMOEReuseWithModification != null
        ? data.allowNonCommerInMOEReuseWithModification
        : this.allowNonCommerInMOEReuseWithModification;
    this.allowNonCommerReuseWithModification =
      data.allowNonCommerReuseWithModification != null
        ? data.allowNonCommerReuseWithModification
        : this.allowNonCommerReuseWithModification;
    this.allowNonCommerInMoeReuseWithoutModification =
      data.allowNonCommerInMoeReuseWithoutModification != null
        ? data.allowNonCommerInMoeReuseWithoutModification
        : this.allowNonCommerInMoeReuseWithoutModification;
    this.allowNonCommerReuseWithoutModification =
      data.allowNonCommerReuseWithoutModification != null
        ? data.allowNonCommerReuseWithoutModification
        : this.allowNonCommerReuseWithoutModification;
    this.copyrightOwner = data.copyrightOwner != null ? data.copyrightOwner : this.copyrightOwner;
    this.acknowledgementAndCredit = data.acknowledgementAndCredit != null ? data.acknowledgementAndCredit : this.acknowledgementAndCredit;
    this.remarks = data.remarks != null ? data.remarks : this.remarks;

    // Target Audience
    this.maximumPlacesPerSchool = data.maximumPlacesPerSchool != null ? data.maximumPlacesPerSchool : this.maximumPlacesPerSchool;
    this.numOfSchoolLeader = data.numOfSchoolLeader != null ? data.numOfSchoolLeader : this.numOfSchoolLeader;
    this.numOfSeniorOrLeadTeacher = data.numOfSeniorOrLeadTeacher != null ? data.numOfSeniorOrLeadTeacher : this.numOfSeniorOrLeadTeacher;
    this.numOfMiddleManagement = data.numOfMiddleManagement != null ? data.numOfMiddleManagement : this.numOfMiddleManagement;
    this.numOfBeginningTeacher = data.numOfBeginningTeacher != null ? data.numOfBeginningTeacher : this.numOfBeginningTeacher;
    this.numOfExperiencedTeacher = data.numOfExperiencedTeacher != null ? data.numOfExperiencedTeacher : this.numOfExperiencedTeacher;
    this.registrationMethod = data.registrationMethod != null ? data.registrationMethod : this.registrationMethod;
    this.placeOfWork = Utils.defaultIfNull(data.placeOfWork, PlaceOfWorkType.ApplicableForEveryone);
    this.prerequisiteCourseIds = Utils.defaultIfNull(data.prerequisiteCourseIds, []);
    this.applicableDivisionIds = Utils.defaultIfNull(data.applicableDivisionIds, []);
    this.applicableBranchIds = Utils.defaultIfNull(data.applicableBranchIds, []);
    this.applicableZoneIds = Utils.defaultIfNull(data.applicableZoneIds, []);
    this.applicableClusterIds = Utils.defaultIfNull(data.applicableClusterIds, []);
    this.applicableSchoolIds = Utils.defaultIfNull(data.applicableSchoolIds, []);
    this.trackIds = Utils.defaultIfNull(data.trackIds, []).map(p => p.toLowerCase());
    this.developmentalRoleIds = Utils.defaultIfNull(data.developmentalRoleIds, []).map(p => p.toLowerCase());
    this.teachingLevels = Utils.defaultIfNull(data.teachingLevels, []).map(p => p.toLowerCase());
    this.teachingSubjectIds = Utils.defaultIfNull(data.teachingSubjectIds, []).map(p => p.toLowerCase());
    this.teachingCourseStudyIds = Utils.defaultIfNull(data.teachingCourseStudyIds, []).map(p => p.toLowerCase());
    this.cocurricularActivityIds = Utils.defaultIfNull(data.cocurricularActivityIds, []).map(p => p.toLowerCase());
    this.jobFamily = Utils.defaultIfNull(data.jobFamily, []).map(p => p.toLowerCase());
    this.easSubstantiveGradeBandingIds = Utils.defaultIfNull(data.easSubstantiveGradeBandingIds, []).map(p => p.toLowerCase());

    // Course Planning
    this.natureOfCourse = data.natureOfCourse != null ? data.natureOfCourse : this.natureOfCourse;
    this.numOfPlannedClass = data.numOfPlannedClass != null ? data.numOfPlannedClass : this.numOfPlannedClass;
    this.numOfSessionPerClass = data.numOfSessionPerClass != null ? data.numOfSessionPerClass : this.numOfSessionPerClass;
    this.numOfHoursPerSession = data.numOfHoursPerSession != null ? data.numOfHoursPerSession : this.numOfHoursPerSession;
    this.numOfMinutesPerSession = data.numOfMinutesPerSession != null ? data.numOfMinutesPerSession : this.numOfMinutesPerSession;
    this.minParticipantPerClass = data.minParticipantPerClass != null ? data.minParticipantPerClass : this.minParticipantPerClass;
    this.maxParticipantPerClass = data.maxParticipantPerClass != null ? data.maxParticipantPerClass : this.maxParticipantPerClass;
    this.maxReLearningTimes = data.maxReLearningTimes == null || data.maxReLearningTimes === 0 ? MAX_INT : data.maxReLearningTimes;
    this.planningPublishDate = data.planningPublishDate != null ? new Date(data.planningPublishDate) : this.planningPublishDate;
    this.planningArchiveDate = data.planningArchiveDate != null ? new Date(data.planningArchiveDate) : this.planningArchiveDate;
    this.willArchiveCommunity = data.willArchiveCommunity != null ? data.willArchiveCommunity : this.willArchiveCommunity;
    this.startDate = data.startDate != null ? new Date(data.startDate) : this.startDate;
    this.expiredDate = data.expiredDate != null ? new Date(data.expiredDate) : this.expiredDate;
    this.courseType = Utils.defaultIfNull(data.courseType, CourseType.New);
    this.pdActivityPeriods = Utils.defaultIfNull(data.pdActivityPeriods, []);

    // Evaluation And ECertificate
    this.preCourseEvaluationFormId =
      data.preCourseEvaluationFormId != null ? data.preCourseEvaluationFormId : this.preCourseEvaluationFormId;
    this.postCourseEvaluationFormId =
      data.postCourseEvaluationFormId != null ? data.postCourseEvaluationFormId : this.postCourseEvaluationFormId;
    this.eCertificateTemplateId = data.eCertificateTemplateId != null ? data.eCertificateTemplateId : this.eCertificateTemplateId;
    this.eCertificatePrerequisite = data.eCertificatePrerequisite != null ? data.eCertificatePrerequisite : this.eCertificatePrerequisite;
    this.courseNameInECertificate = data.courseNameInECertificate != null ? data.courseNameInECertificate : this.courseNameInECertificate;

    // Administration
    this.firstAdministratorId = data.firstAdministratorId != null ? data.firstAdministratorId : this.firstAdministratorId;
    this.secondAdministratorId = data.secondAdministratorId != null ? data.secondAdministratorId : this.secondAdministratorId;
    this.primaryApprovingOfficerId =
      data.primaryApprovingOfficerId != null ? data.primaryApprovingOfficerId : this.primaryApprovingOfficerId;
    this.alternativeApprovingOfficerId =
      data.alternativeApprovingOfficerId != null ? data.alternativeApprovingOfficerId : this.alternativeApprovingOfficerId;
    this.collaborativeContentCreatorIds = Utils.defaultIfNull(data.collaborativeContentCreatorIds, []);
    this.courseFacilitatorIds = Utils.defaultIfNull(data.courseFacilitatorIds, []);
    this.courseCoFacilitatorIds = Utils.defaultIfNull(data.courseCoFacilitatorIds, []);

    // System
    this.version = data.version != null ? data.version : this.version;
    this.status = data.status != null ? data.status : this.status;
    this.contentStatus = data.contentStatus != null ? data.contentStatus : this.contentStatus;
    this.isDeleted = data.isDeleted != null ? data.isDeleted : this.isDeleted;
    this.changedBy = data.changedBy != null ? data.changedBy : this.changedBy;
    this.createdBy = data.createdBy != null ? data.createdBy : this.createdBy;
    this.externalId = data.externalId != null ? data.externalId : this.externalId;
    this.createdDate = data.createdDate != null ? new Date(data.createdDate) : this.createdDate;
    this.changedDate = data.changedDate != null ? new Date(data.changedDate) : this.changedDate;
    this.submittedDate = data.submittedDate != null ? new Date(data.submittedDate) : this.submittedDate;
    this.submittedContentDate = data.submittedContentDate != null ? new Date(data.submittedContentDate) : this.submittedContentDate;
    this.approvalDate = data.approvalDate != null ? new Date(data.approvalDate) : this.approvalDate;
    this.approvalContentDate = data.approvalContentDate != null ? new Date(data.approvalContentDate) : this.approvalContentDate;
    this.publishedContentDate = data.publishedContentDate != null ? new Date(data.publishedContentDate) : this.publishedContentDate;
    this.isMigrated = data.isMigrated != null ? data.isMigrated : this.isMigrated;
    this.hasContent = data.hasContent != null ? data.hasContent : this.hasContent;
    this.coursePlanningCycleId = data.coursePlanningCycleId != null ? data.coursePlanningCycleId : this.coursePlanningCycleId;
    this.verifiedDate = data.verifiedDate != null ? data.verifiedDate : this.verifiedDate;
    this.archiveDate = data.archiveDate != null ? data.archiveDate : this.archiveDate;
    this.archivedBy = data.archivedBy != null ? data.archivedBy : this.archivedBy;
    this.hasFullRight = data.hasFullRight != null ? data.hasFullRight : this.hasFullRight;
  }

  public getAllMetadataTagIds(): string[] {
    return []
      .concat(this.pdActivityType ? [this.pdActivityType] : [])
      .concat(this.courseLevel ? [this.courseLevel] : [])
      .concat(this.pdAreaThemeId ? [this.pdAreaThemeId] : [])
      .concat(this.serviceSchemeIds)
      .concat(this.subjectAreaIds)
      .concat(this.learningFrameworkIds)
      .concat(this.learningDimensionIds)
      .concat(this.learningAreaIds)
      .concat(this.learningSubAreaIds)
      .concat(this.teacherOutcomeIds)
      .concat(this.trackIds)
      .concat(this.categoryIds)
      .concat(this.jobFamily)
      .concat(this.teachingCourseStudyIds)
      .concat(this.teachingLevels)
      .concat(this.teachingSubjectIds)
      .concat(this.cocurricularActivityIds)
      .concat(this.pdActivityPeriods)
      .concat(this.developmentalRoleIds)
      .concat(this.learningMode ? [this.learningMode] : [])
      .concat(this.natureOfCourse ? [this.natureOfCourse] : []);
  }

  public getAllUserIds(): string[] {
    return []
      .concat(this.firstAdministratorId ? [this.firstAdministratorId] : [])
      .concat(this.secondAdministratorId ? [this.secondAdministratorId] : [])
      .concat(this.primaryApprovingOfficerId ? [this.primaryApprovingOfficerId] : [])
      .concat(this.alternativeApprovingOfficerId ? [this.alternativeApprovingOfficerId] : [])
      .concat(this.courseFacilitatorIds ? this.courseFacilitatorIds : [])
      .concat(this.courseCoFacilitatorIds ? this.courseCoFacilitatorIds : [])
      .concat(this.collaborativeContentCreatorIds ? this.collaborativeContentCreatorIds : [])
      .concat(this.moeOfficerId ? [this.moeOfficerId] : []);
  }

  public getAdminstratorIds(): string[] {
    return []
      .concat(this.firstAdministratorId ? [this.firstAdministratorId] : [])
      .concat(this.secondAdministratorId ? [this.secondAdministratorId] : []);
  }

  public getTargetParticipantDepartmentIds(): number[] {
    return [].concat(
      this.applicableDivisionIds,
      this.applicableBranchIds,
      this.applicableZoneIds,
      this.applicableClusterIds,
      this.applicableSchoolIds
    );
  }

  public getAllDepartmentIds(currentUser?: UserInfoModel): number[] {
    return Utils.distinct(
      []
        .concat(this.ownerDivisionIds ? this.ownerDivisionIds : [])
        .concat(this.ownerBranchIds ? this.ownerBranchIds : [])
        .concat(this.partnerOrganisationIds ? this.partnerOrganisationIds : [])
        .concat(this.applicableDivisionIds ? this.applicableDivisionIds : [])
        .concat(this.applicableBranchIds ? this.applicableBranchIds : [])
        .concat(this.applicableZoneIds ? this.applicableZoneIds : [])
        .concat(this.applicableClusterIds ? this.applicableClusterIds : [])
        .concat(this.applicableSchoolIds ? this.applicableSchoolIds : [])
        .concat(currentUser && currentUser.departmentId ? [currentUser.departmentId] : [])
    );
  }

  public isResubmit(): boolean {
    return this.approvalDate && (this.status === CourseStatus.Draft || this.status === CourseStatus.PendingApproval);
  }

  public isContentResubmit(): boolean {
    return this.approvalContentDate && (this.contentStatus === ContentStatus.Draft || this.contentStatus === ContentStatus.PendingApproval);
  }

  public afterApproving(): boolean {
    return (
      this.status === CourseStatus.Approved ||
      this.status === CourseStatus.PlanningCycleVerified ||
      this.status === CourseStatus.PlanningCycleCompleted ||
      this.status === CourseStatus.Published ||
      this.status === CourseStatus.Unpublished ||
      this.status === CourseStatus.Completed ||
      this.status === CourseStatus.Archived
    );
  }

  public afterPublishing(): boolean {
    return (
      this.status === CourseStatus.Published ||
      this.status === CourseStatus.Unpublished ||
      this.status === CourseStatus.Completed ||
      this.status === CourseStatus.Archived
    );
  }

  public canViewContent(): boolean {
    return this.afterApproving();
  }

  public canViewClassRun(): boolean {
    return (this.isMigrated || this.afterApproving()) && !this.isMicrolearning();
  }

  public canCreateClassRun(): boolean {
    return !this.isMicrolearning() && !this.isArchived();
  }

  public hasCreateClassRunPermission(user: UserInfoModel): boolean {
    return this.hasAdministrationPermission(user) || user.hasPermissionPrefix(CAM_PERMISSIONS.CreateEditClassRun);
  }

  public hasOwnerPermission(user: UserInfoModel): boolean {
    return this.createdBy === user.id || this.hasFullRight;
  }

  public hasApprovalPermission(user: UserInfoModel): boolean {
    return this.primaryApprovingOfficerId === user.id || this.alternativeApprovingOfficerId === user.id || this.hasFullRight;
  }

  public hasViewCourseDetailPermission(user: UserInfoModel): boolean {
    return (
      this.hasContentCreatorPermission(user) ||
      this.hasApprovalPermission(user) ||
      this.hasAdministrationPermission(user) ||
      user.hasPermissionPrefix(CAM_PERMISSIONS.ViewCourseDetail)
    );
  }

  public hasViewClassRunsPermission(user: UserInfoModel): boolean {
    return (
      this.hasApprovalPermission(user) ||
      this.hasContentCreatorPermission(user) ||
      this.hasAdministrationPermission(user) ||
      user.hasPermissionPrefix(CAM_PERMISSIONS.ViewClassRunList)
    );
  }

  public hasViewSessionsPermission(user: UserInfoModel): boolean {
    return (
      this.hasApprovalPermission(user) ||
      this.hasContentCreatorPermission(user) ||
      this.hasAdministrationPermission(user) ||
      user.hasPermissionPrefix(CAM_PERMISSIONS.ViewSessionList)
    );
  }

  public hasFacilitatorsPermission(user: UserInfoModel): boolean {
    return this.courseFacilitatorIds.includes(user.id) || this.courseCoFacilitatorIds.includes(user.id) || this.hasFullRight;
  }

  public hasAdministrationPermission(user: UserInfoModel): boolean {
    return this.firstAdministratorId === user.id || this.secondAdministratorId === user.id || this.hasFullRight;
  }

  public hasViewSessionPermission(user: UserInfoModel): boolean {
    return this.hasContentCreatorPermission(user) || this.hasAdministrationPermission(user) || this.hasFacilitatorsPermission(user);
  }

  public hasCollaborativeContentCreatorPermission(user: UserInfoModel): boolean {
    return this.collaborativeContentCreatorIds.includes(user.id) || this.hasFullRight;
  }

  public hasCreateEditCoursePermission(user: UserInfoModel): boolean {
    return this.hasContentCreatorPermission(user) || user.hasPermissionPrefix(CAM_PERMISSIONS.CreateEditCourse);
  }

  public hasContentCreatorPermission(user: UserInfoModel): boolean {
    return this.hasCollaborativeContentCreatorPermission(user) || this.hasOwnerPermission(user) || this.hasFullRight;
  }

  public hasApproveRejectCourseContentPermission(user: UserInfoModel): boolean {
    return user.hasPermissionPrefix(LMM_PERMISSIONS.ApproveRejectCourseContent);
  }

  public hasPublishCourseContentPermission(user: UserInfoModel): boolean {
    return user.hasPermissionPrefix(LMM_PERMISSIONS.PublishCourseContent);
  }

  public hasViewPostCourseEvaluationPermission(user: UserInfoModel): boolean {
    return user.hasPermissionPrefix(LMM_PERMISSIONS.ViewPostCourseEvaluation);
  }

  public canApproveRejectCourseContent(): boolean {
    return this.contentStatus === ContentStatus.PendingApproval;
  }

  public canCustomizeContentForClassRun(user: UserInfoModel, classRun: ClassRun): boolean {
    return (
      (this.hasContentCreatorPermission(user) ||
        this.hasFacilitatorsPermission(user) ||
        this.hasAdministrationPermission(user) ||
        this.hasApprovalPermission(user) ||
        classRun.hasFacilitatorPermission(user, this)) &&
      !this.isArchived()
    );
  }

  public canActivateCourseCriteria(user: UserInfoModel): boolean {
    return (this.hasContentCreatorPermission(user) || this.hasAdministrationPermission(user)) && !this.isArchived();
  }

  public canActivateCourseAutomate(user: UserInfoModel): boolean {
    return (
      (this.hasContentCreatorPermission(user) || this.hasAdministrationPermission(user) || user.hasAdministratorRoles()) &&
      !this.isArchived()
    );
  }

  public canByPassCA(): boolean {
    return this.learningMode === MetadataId.ELearning && this.registrationMethod === RegistrationMethod.Public;
  }

  public canExportParticipant(user: UserInfoModel): boolean {
    return (
      (this.hasOwnerPermission(user) || (this.hasAdministrationPermission(user) && this.status === CourseStatus.Published)) &&
      !this.isArchived()
    );
  }

  public canAddParticipants(user: UserInfoModel): boolean {
    return !this.isMicrolearning() && this.hasAdministrationPermission(user) && !this.isArchived();
  }

  public canViewFieldOfCourseInPlanningCycle(): boolean {
    return this.status === CourseStatus.PlanningCycleVerified;
  }

  public communityCreated(): boolean {
    return this.status === CourseStatus.Published;
  }

  public isMicrolearning(): boolean {
    return this.pdActivityType === MetadataId.Microlearning;
  }

  public isELearning(): boolean {
    return this.learningMode === MetadataId.ELearning;
  }

  public isPlanningVerificationRequired(): boolean {
    return this.coursePlanningCycleId != null && (this.verifiedDate == null || this.status === CourseStatus.VerificationRejected);
  }

  public isCompletingCourseForPlanning(): boolean {
    return (
      this.coursePlanningCycleId != null &&
      this.verifiedDate != null &&
      (this.status === CourseStatus.PlanningCycleVerified || this.status === CourseStatus.PlanningCycleCompleted)
    );
  }

  public isPublishedOnce(): boolean {
    return this.courseCode != null && this.courseCode !== '';
  }

  public isStatusValidToEdit(): boolean {
    return (
      this.status === CourseStatus.Draft ||
      this.status === CourseStatus.Rejected ||
      this.status === CourseStatus.VerificationRejected ||
      this.status === CourseStatus.Unpublished ||
      this.status === CourseStatus.PendingApproval ||
      this.status === CourseStatus.PlanningCycleVerified ||
      this.status === CourseStatus.PlanningCycleCompleted ||
      this.status === CourseStatus.Approved
    );
  }

  /**
   * Check is editable for field which can only edit when course hasn't published and started
   */
  public isEditableBeforeStarted(): boolean {
    return this.isStatusValidToEdit() && (!this.started() || this.isMigrated || !this.isPublishedOnce());
  }

  /**
   * Check is editable for field still can edit even when course has started
   */
  public isEditableEvenClassStarted(): boolean {
    return this.isStatusValidToEdit();
  }

  public canPublishContent(user: UserInfoModel): boolean {
    return (
      user &&
      this.hasContentCreatorPermission(user) &&
      (this.contentStatus === ContentStatus.Approved || this.contentStatus === ContentStatus.Unpublished) &&
      !this.isArchived()
    );
  }

  public canUnpublishContent(user: UserInfoModel): boolean {
    return (
      user &&
      this.hasContentCreatorPermission(user) &&
      this.contentStatus === ContentStatus.Published &&
      (!this.isMicrolearning() || this.status !== CourseStatus.Published) &&
      !this.isArchived()
    );
  }

  public isContentEditable(): boolean {
    return this.contentStatus !== ContentStatus.Published && this.status !== CourseStatus.Completed && !this.isArchived();
  }

  public canUserEditContent(user: UserInfoModel): boolean {
    return (this.hasContentCreatorPermission(user) || this.hasFacilitatorsPermission(user)) && this.isContentEditable();
  }

  public isPlanningPendingApproval(): boolean {
    return this.status === CourseStatus.PendingApproval && this.coursePlanningCycleId != null;
  }

  public isPlanningPendingVerify(): boolean {
    return this.status === CourseStatus.Approved && this.coursePlanningCycleId != null;
  }

  public canPublishCourse(courseContents?: CourseContentItemModel[]): boolean {
    return (
      ((this.status === CourseStatus.Approved && !this.isPlanningVerificationRequired()) ||
        this.status === CourseStatus.Unpublished ||
        this.status === CourseStatus.PlanningCycleCompleted) &&
      (!this.isMicrolearning() || (courseContents && courseContents.length > 0) || this.hasContent)
    );
  }

  public hasPublishCoursePermission(user: UserInfoModel): boolean {
    return this.hasContentCreatorPermission(user) || user.hasPermissionPrefix(CAM_PERMISSIONS.PublishUnpublishCourse);
  }

  public canUnpublishCourse(user: UserInfoModel): boolean {
    return this.status === CourseStatus.Published && user && this.hasContentCreatorPermission(user);
  }

  public isArchived(): boolean {
    return this.status === CourseStatus.Archived;
  }

  public canImportParticipant(user: UserInfoModel): boolean {
    return this.status === CourseStatus.Published && this.hasAdministrationPermission(user) && !this.isMicrolearning();
  }

  public canVerifyCourse(user: UserInfoModel): boolean {
    return this.status === CourseStatus.Approved && this.verifiedDate == null && CoursePlanningCycle.canVerifyCourse(user);
  }

  public canViewStatisticalReport(user: UserInfoModel): boolean {
    return this.hasApprovalPermission(user) || this.hasContentCreatorPermission(user) || this.hasAdministrationPermission(user);
  }

  public canDeleteCourse(currentUser: UserInfoModel): boolean {
    return (
      this.isPublishedOnce() === false &&
      this.isMigrated === false &&
      this.status === CourseStatus.Draft &&
      this.hasOwnerPermission(currentUser)
    );
  }

  public canTransferOwnershipCourse(currentUser: UserInfoModel): boolean {
    return !this.isArchived() && this.hasOwnerPermission(currentUser);
  }

  public canArchiveCourse(user: UserInfoModel): boolean {
    return (
      this.hasOwnerPermission(user) &&
      [CourseStatus.Approved, CourseStatus.Rejected, CourseStatus.Unpublished, CourseStatus.Draft].includes(this.status) &&
      !this.isPendingPlanningCycleVerified()
    );
  }

  public canBeApproved(): boolean {
    return this.status === CourseStatus.PendingApproval;
  }

  public isPendingPlanningCycleVerified(): boolean {
    return this.coursePlanningCycleId != null && this.status === CourseStatus.Approved;
  }

  public started(): boolean {
    return this.startDate != null && DateUtils.removeTime(new Date()) >= DateUtils.removeTime(this.startDate);
  }
}
