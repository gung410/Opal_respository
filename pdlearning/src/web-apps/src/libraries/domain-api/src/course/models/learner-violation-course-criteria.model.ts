import {
  CourseCriteriaLearnerViolationAccountType,
  ICourseCriteriaLearnerViolationAccountType
} from './course-criteria-learner-violation-account-type.model';
import { CourseCriteriaLearnerViolationField, CourseCriteriaLearnerViolationType } from './learner-violation-course-criteria-type.model';
import {
  CourseCriteriaLearnerViolationPlaceOfWork,
  ICourseCriteriaLearnerViolationPlaceOfWork
} from './course-criteria-learner-violation-place-of-work.model';
import {
  CourseCriteriaLearnerViolationPreRequisiteCourse,
  ICourseCriteriaLearnerViolationPreRequisiteCourse
} from './course-criteria-learner-violation-pre-requisite-course.model';
import {
  CourseCriteriaLearnerViolationTaggingMetadata,
  ICourseCriteriaLearnerViolationTaggingMetadata
} from './course-criteria-learner-violation-tagging-metadata.model';
import { ILearnerCourseCriteria, LearnerCourseCriteria } from './learner-course-criteria.model';

import { Course } from './course.model';
import { DepartmentInfoModel } from '../../organization/models/department-info.model';
import { DepartmentLevelModel } from '../../organization/models/department-level.model';
import { MetadataTagModel } from '../../tagging/models/metadata-tag.model';
import { TypeOfOrganization } from '../../share/models/type-of-organization.model';
import { Utils } from '@opal20/infrastructure';

export interface ILearnerViolationCourseCriteria {
  courseId: string;
  isViolated: boolean;
  learnerCriteria: ILearnerCourseCriteria;
  accountType: ICourseCriteriaLearnerViolationAccountType;
  serviceSchemes: ICourseCriteriaLearnerViolationTaggingMetadata[];
  tracks: ICourseCriteriaLearnerViolationTaggingMetadata[];
  devRoles: ICourseCriteriaLearnerViolationTaggingMetadata[];
  teachingLevels: ICourseCriteriaLearnerViolationTaggingMetadata[];
  teachingCourseOfStudy: ICourseCriteriaLearnerViolationTaggingMetadata[];
  jobFamily: ICourseCriteriaLearnerViolationTaggingMetadata[];
  coCurricularActivity: ICourseCriteriaLearnerViolationTaggingMetadata[];
  subGradeBanding: ICourseCriteriaLearnerViolationTaggingMetadata[];
  placeOfWork: ICourseCriteriaLearnerViolationPlaceOfWork;
  preRequisiteCourses: ICourseCriteriaLearnerViolationPreRequisiteCourse[];
}

export class LearnerViolationCourseCriteria implements ILearnerViolationCourseCriteria {
  public courseId: string;
  public isViolated: boolean = false;
  public learnerCriteria: LearnerCourseCriteria = new LearnerCourseCriteria();
  public accountType: CourseCriteriaLearnerViolationAccountType = new CourseCriteriaLearnerViolationAccountType();
  public serviceSchemes: CourseCriteriaLearnerViolationTaggingMetadata[] = [];
  public tracks: CourseCriteriaLearnerViolationTaggingMetadata[] = [];
  public devRoles: CourseCriteriaLearnerViolationTaggingMetadata[] = [];
  public teachingLevels: CourseCriteriaLearnerViolationTaggingMetadata[] = [];
  public teachingCourseOfStudy: CourseCriteriaLearnerViolationTaggingMetadata[] = [];
  public jobFamily: CourseCriteriaLearnerViolationTaggingMetadata[] = [];
  public coCurricularActivity: CourseCriteriaLearnerViolationTaggingMetadata[] = [];
  public subGradeBanding: CourseCriteriaLearnerViolationTaggingMetadata[] = [];
  public placeOfWork: CourseCriteriaLearnerViolationPlaceOfWork | null;
  public preRequisiteCourses: CourseCriteriaLearnerViolationPreRequisiteCourse[] = [];
  constructor(data?: ILearnerViolationCourseCriteria) {
    if (data != null) {
      this.courseId = data.courseId;
      this.isViolated = data.isViolated;
      this.learnerCriteria = data.learnerCriteria != null ? new LearnerCourseCriteria(data.learnerCriteria) : this.learnerCriteria;
      this.accountType = data.accountType != null ? new CourseCriteriaLearnerViolationAccountType(data.accountType) : this.accountType;
      this.serviceSchemes =
        data.serviceSchemes != null ? data.serviceSchemes.map(p => new CourseCriteriaLearnerViolationTaggingMetadata(p)) : [];
      this.tracks = data.tracks != null ? data.tracks.map(p => new CourseCriteriaLearnerViolationTaggingMetadata(p)) : [];
      this.devRoles = data.devRoles != null ? data.devRoles.map(p => new CourseCriteriaLearnerViolationTaggingMetadata(p)) : [];
      this.teachingLevels =
        data.teachingLevels != null ? data.teachingLevels.map(p => new CourseCriteriaLearnerViolationTaggingMetadata(p)) : [];
      this.teachingCourseOfStudy =
        data.teachingCourseOfStudy != null ? data.teachingCourseOfStudy.map(p => new CourseCriteriaLearnerViolationTaggingMetadata(p)) : [];
      this.jobFamily = data.jobFamily != null ? data.jobFamily.map(p => new CourseCriteriaLearnerViolationTaggingMetadata(p)) : [];
      this.coCurricularActivity =
        data.coCurricularActivity != null ? data.coCurricularActivity.map(p => new CourseCriteriaLearnerViolationTaggingMetadata(p)) : [];
      this.subGradeBanding =
        data.subGradeBanding != null ? data.subGradeBanding.map(p => new CourseCriteriaLearnerViolationTaggingMetadata(p)) : [];
      this.placeOfWork = data.placeOfWork ? new CourseCriteriaLearnerViolationPlaceOfWork(data.placeOfWork) : null;
      this.preRequisiteCourses =
        data.preRequisiteCourses != null ? data.preRequisiteCourses.map(p => new CourseCriteriaLearnerViolationPreRequisiteCourse(p)) : [];
    }
  }

  public checkFieldViolated(criteriaField: CourseCriteriaLearnerViolationField): boolean {
    switch (criteriaField) {
      case CourseCriteriaLearnerViolationField.AccountType:
        return this.accountType && this.accountType.isCourseCriteriaViolated();
      case CourseCriteriaLearnerViolationField.ServiceSchemes:
        return this.serviceSchemes && this.serviceSchemes.filter(p => p.isCourseCriteriaViolated()).length > 0;
      case CourseCriteriaLearnerViolationField.Tracks:
        return this.tracks && this.tracks.filter(p => p.isCourseCriteriaViolated()).length > 0;
      case CourseCriteriaLearnerViolationField.DevRoles:
        return this.devRoles && this.devRoles.filter(p => p.isCourseCriteriaViolated()).length > 0;
      case CourseCriteriaLearnerViolationField.TeachingLevels:
        return this.teachingLevels && this.teachingLevels.filter(p => p.isCourseCriteriaViolated()).length > 0;
      case CourseCriteriaLearnerViolationField.TeachingCourseOfStudy:
        return this.teachingCourseOfStudy && this.teachingCourseOfStudy.filter(p => p.isCourseCriteriaViolated()).length > 0;
      case CourseCriteriaLearnerViolationField.JobFamily:
        return this.jobFamily && this.jobFamily.filter(p => p.isCourseCriteriaViolated()).length > 0;
      case CourseCriteriaLearnerViolationField.CoCurricularActivity:
        return this.coCurricularActivity && this.coCurricularActivity.filter(p => p.isCourseCriteriaViolated()).length > 0;
      case CourseCriteriaLearnerViolationField.SubGradeBanding:
        return this.subGradeBanding && this.subGradeBanding.filter(p => p.isCourseCriteriaViolated()).length > 0;
      case CourseCriteriaLearnerViolationField.PlaceOfWork:
        return this.placeOfWork && this.placeOfWork.isCourseCriteriaViolated();
      case CourseCriteriaLearnerViolationField.PreRequisiteCourses:
        return this.preRequisiteCourses && this.preRequisiteCourses.filter(p => p.isCourseCriteriaViolated()).length > 0;
    }
  }

  public getLearnerCriteriaFieldDisplayText(
    criteriaField: CourseCriteriaLearnerViolationField,
    allMetadataDic: Dictionary<MetadataTagModel>,
    allOrganizationUnitTypeDic: Dictionary<TypeOfOrganization>,
    allOrganizationLevelDic: Dictionary<DepartmentLevelModel>,
    learnerDepartmentsDic: Dictionary<DepartmentInfoModel>
  ): string {
    if (this.learnerCriteria == null) {
      return '';
    }
    switch (criteriaField) {
      case CourseCriteriaLearnerViolationField.ServiceSchemes:
        return this.getMetadataDisplayText(this.learnerCriteria.serviceSchemes, allMetadataDic);
      case CourseCriteriaLearnerViolationField.Tracks:
        return this.getMetadataDisplayText(this.learnerCriteria.tracks, allMetadataDic);
      case CourseCriteriaLearnerViolationField.DevRoles:
        return this.getMetadataDisplayText(this.learnerCriteria.devRoles, allMetadataDic);
      case CourseCriteriaLearnerViolationField.TeachingLevels:
        return this.getMetadataDisplayText(this.learnerCriteria.teachingLevels, allMetadataDic);
      case CourseCriteriaLearnerViolationField.TeachingCourseOfStudy:
        return this.getMetadataDisplayText(this.learnerCriteria.teachingCourseOfStudy, allMetadataDic);
      case CourseCriteriaLearnerViolationField.JobFamily:
        return this.getMetadataDisplayText(this.learnerCriteria.jobFamily, allMetadataDic);
      case CourseCriteriaLearnerViolationField.CoCurricularActivity:
        return this.getMetadataDisplayText(this.learnerCriteria.coCurricularActivity, allMetadataDic);
      case CourseCriteriaLearnerViolationField.SubGradeBanding:
        return this.getMetadataDisplayText(this.learnerCriteria.coCurricularActivity, allMetadataDic);
      case CourseCriteriaLearnerViolationField.PlaceOfWork:
        return this.getLearnerCriteriaDisplayTextForPlaceOfWork(allOrganizationUnitTypeDic, allOrganizationLevelDic, learnerDepartmentsDic);
      case CourseCriteriaLearnerViolationField.AccountType:
        return this.learnerCriteria.accountType;
    }
  }

  public getViolatedFieldDisplayText(
    criteriaField: CourseCriteriaLearnerViolationField,
    allMetadataDic: Dictionary<MetadataTagModel>,
    prerequisiteCoursesDic: Dictionary<Course>,
    allOrganizationUnitTypeDic: Dictionary<TypeOfOrganization>,
    allOrganizationLevelDic: Dictionary<DepartmentLevelModel>,
    specificDepartmentsDic: Dictionary<DepartmentInfoModel>
  ): string {
    switch (criteriaField) {
      case CourseCriteriaLearnerViolationField.ServiceSchemes:
        return this.getMetadataDisplayText(
          this.serviceSchemes.map(p => p.tagId),
          allMetadataDic,
          this.serviceSchemes.filter(p => p.isCourseCriteriaViolated()).map(p => p.violationType)
        );
      case CourseCriteriaLearnerViolationField.Tracks:
        return this.getMetadataDisplayText(
          this.tracks.map(p => p.tagId),
          allMetadataDic,
          this.tracks.filter(p => p.isCourseCriteriaViolated()).map(p => p.violationType)
        );
      case CourseCriteriaLearnerViolationField.DevRoles:
        return this.getMetadataDisplayText(
          this.devRoles.map(p => p.tagId),
          allMetadataDic,
          this.devRoles.filter(p => p.isCourseCriteriaViolated()).map(p => p.violationType)
        );
      case CourseCriteriaLearnerViolationField.TeachingLevels:
        return this.getMetadataDisplayText(
          this.teachingLevels.map(p => p.tagId),
          allMetadataDic,
          this.teachingLevels.filter(p => p.isCourseCriteriaViolated()).map(p => p.violationType)
        );
      case CourseCriteriaLearnerViolationField.TeachingCourseOfStudy:
        return this.getMetadataDisplayText(
          this.teachingCourseOfStudy.map(p => p.tagId),
          allMetadataDic,
          this.teachingCourseOfStudy.filter(p => p.isCourseCriteriaViolated()).map(p => p.violationType)
        );
      case CourseCriteriaLearnerViolationField.JobFamily:
        return this.getMetadataDisplayText(
          this.jobFamily.map(p => p.tagId),
          allMetadataDic,
          this.jobFamily.filter(p => p.isCourseCriteriaViolated()).map(p => p.violationType)
        );
      case CourseCriteriaLearnerViolationField.CoCurricularActivity:
        return this.getMetadataDisplayText(
          this.coCurricularActivity.map(p => p.tagId),
          allMetadataDic,
          this.coCurricularActivity.filter(p => p.isCourseCriteriaViolated()).map(p => p.violationType)
        );
      case CourseCriteriaLearnerViolationField.SubGradeBanding:
        return this.getMetadataDisplayText(
          this.coCurricularActivity.map(p => p.tagId),
          allMetadataDic,
          this.coCurricularActivity.filter(p => p.isCourseCriteriaViolated()).map(p => p.violationType)
        );
      case CourseCriteriaLearnerViolationField.PreRequisiteCourses:
        return this.getViolatedRequisiteCoursesDisplayText(prerequisiteCoursesDic);
      case CourseCriteriaLearnerViolationField.PlaceOfWork:
        return this.getViolatedPlaceOfWorkDisplayText(allOrganizationUnitTypeDic, allOrganizationLevelDic, specificDepartmentsDic);
      case CourseCriteriaLearnerViolationField.AccountType:
        return this.accountType.accountType;
    }
  }

  public getLearnerViolationCourseCriteriaReasonDisplayText(criteriaField: CourseCriteriaLearnerViolationField): string {
    switch (criteriaField) {
      case CourseCriteriaLearnerViolationField.AccountType:
        return this.getReasonDisplayText([this.accountType.violationType]);
      case CourseCriteriaLearnerViolationField.ServiceSchemes:
        return this.getReasonDisplayText(this.serviceSchemes.map(p => p.violationType));
      case CourseCriteriaLearnerViolationField.Tracks:
        return this.getReasonDisplayText(this.tracks.map(p => p.violationType));
      case CourseCriteriaLearnerViolationField.DevRoles:
        return this.getReasonDisplayText(this.devRoles.map(p => p.violationType));
      case CourseCriteriaLearnerViolationField.TeachingLevels:
        return this.getReasonDisplayText(this.teachingLevels.map(p => p.violationType));
      case CourseCriteriaLearnerViolationField.TeachingCourseOfStudy:
        return this.getReasonDisplayText(this.teachingCourseOfStudy.map(p => p.violationType));
      case CourseCriteriaLearnerViolationField.JobFamily:
        return this.getReasonDisplayText(this.jobFamily.map(p => p.violationType));
      case CourseCriteriaLearnerViolationField.CoCurricularActivity:
        return this.getReasonDisplayText(this.coCurricularActivity.map(p => p.violationType));
      case CourseCriteriaLearnerViolationField.SubGradeBanding:
        return this.getReasonDisplayText(this.coCurricularActivity.map(p => p.violationType));
      case CourseCriteriaLearnerViolationField.PreRequisiteCourses:
        return this.getReasonDisplayText(this.preRequisiteCourses.map(p => p.violationType));
      case CourseCriteriaLearnerViolationField.PlaceOfWork:
        return this.getReasonDisplayText(this.placeOfWork.getPlaceOfWorkViolatedTypes());
    }
  }

  public getViolatedRequisiteCoursesDisplayText(registerCourseCriteriaDic: Dictionary<Course>): string {
    return this.preRequisiteCourses
      .map(_ => registerCourseCriteriaDic[_.courseId])
      .filter(p => p != null)
      .map(p => p.courseName)
      .join(', ');
  }

  public getLearnerCriteriaDisplayTextForPlaceOfWork(
    allOrganizationUnitTypeDic: Dictionary<TypeOfOrganization>,
    allOrganizationLevelDic: Dictionary<DepartmentLevelModel>,
    learnerDepartmentsDic: Dictionary<DepartmentInfoModel>
  ): string {
    if (this.placeOfWork.isDepartmentUnitTypesViolated()) {
      return allOrganizationUnitTypeDic[this.learnerCriteria.department.departmentUnitTypeId]
        ? allOrganizationUnitTypeDic[this.learnerCriteria.department.departmentUnitTypeId].displayText
        : 'n/a';
    } else if (this.placeOfWork.isDepartmentLevelTypesViolated()) {
      return allOrganizationLevelDic[this.learnerCriteria.department.departmentLevelTypeId]
        ? allOrganizationLevelDic[this.learnerCriteria.department.departmentLevelTypeId].departmentLevelName
        : 'n/a';
    } else if (this.placeOfWork.isSpecificDepartmentsViolated()) {
      return learnerDepartmentsDic[this.learnerCriteria.department.departmentId]
        ? learnerDepartmentsDic[this.learnerCriteria.department.departmentId].departmentName
        : 'n/a';
    } else {
      return '';
    }
  }

  public getViolatedPlaceOfWorkDisplayText(
    allOrganizationUnitTypeDic: Dictionary<TypeOfOrganization>,
    allOrganizationLevelDic: Dictionary<DepartmentLevelModel>,
    specificDepartmentsDic: Dictionary<DepartmentInfoModel>
  ): string {
    const criteriaViolationType = this.placeOfWork.getPlaceOfWorkViolatedTypes();
    if (
      criteriaViolationType.length > 0 &&
      criteriaViolationType.filter(p => p !== CourseCriteriaLearnerViolationType.NotViolate).length === 0
    ) {
      return 'Matched';
    }
    if (this.placeOfWork.isDepartmentUnitTypesViolated()) {
      return this.placeOfWork.getDepartmentUnitTypesDisplayText(allOrganizationUnitTypeDic);
    } else if (this.placeOfWork.isDepartmentLevelTypesViolated()) {
      return this.placeOfWork.getDepartmentLevelsDisplayText(allOrganizationLevelDic);
    } else if (this.placeOfWork.isSpecificDepartmentsViolated()) {
      this.placeOfWork.getSpecificDepartmentssDisplayText(specificDepartmentsDic);
    } else {
      return '';
    }
  }

  public getMetadataDisplayText(
    tagIds: string[],
    registerAllMetadataDic: Dictionary<MetadataTagModel>,
    missingViolationType?: CourseCriteriaLearnerViolationType[]
  ): string {
    if (missingViolationType != null && missingViolationType.length === 0) {
      return 'Matched';
    }

    const joinTagsDisplay = tagIds
      .map(p => registerAllMetadataDic[p])
      .filter(p => p != null)
      .map(p => p.displayText)
      .join(', ');
    return joinTagsDisplay ? joinTagsDisplay : 'n/a';
  }

  public getReasonDisplayText(violationTypes: CourseCriteriaLearnerViolationType[]): string {
    return violationTypes.filter(p => p === CourseCriteriaLearnerViolationType.Missing).length > 0
      ? 'Missing'
      : 'Exceed maximum participant';
  }

  public getCriteriaDisplayTitle(criteriaField: CourseCriteriaLearnerViolationField): string {
    switch (criteriaField) {
      case CourseCriteriaLearnerViolationField.AccountType:
        return 'Account Type';
      case CourseCriteriaLearnerViolationField.ServiceSchemes:
        return 'Service Scheme';
      case CourseCriteriaLearnerViolationField.Tracks:
        return 'Tracks';
      case CourseCriteriaLearnerViolationField.DevRoles:
        return 'Developmental Role';
      case CourseCriteriaLearnerViolationField.TeachingLevels:
        return 'Teaching Level';
      case CourseCriteriaLearnerViolationField.TeachingCourseOfStudy:
        return 'Teaching Course of Study';
      case CourseCriteriaLearnerViolationField.JobFamily:
        return 'Job Family';
      case CourseCriteriaLearnerViolationField.CoCurricularActivity:
        return 'Co-curricular Activity';
      case CourseCriteriaLearnerViolationField.SubGradeBanding:
        return 'Sub-Grade Banding';
      case CourseCriteriaLearnerViolationField.PreRequisiteCourses:
        return 'Pre-requisite Courses';
      case CourseCriteriaLearnerViolationField.PlaceOfWork:
        return 'Place of Work';
    }
  }

  public getAllRelatedMetadataIds(): string[] {
    return Utils.distinct(
      [].concat(
        this.learnerCriteria.serviceSchemes,
        this.learnerCriteria.tracks,
        this.learnerCriteria.devRoles,
        this.learnerCriteria.teachingLevels,
        this.learnerCriteria.teachingCourseOfStudy,
        this.learnerCriteria.jobFamily,
        this.learnerCriteria.coCurricularActivity,
        this.learnerCriteria.subGradeBanding,
        this.serviceSchemes.map(_ => _.tagId),
        this.tracks.map(_ => _.tagId),
        this.devRoles.map(_ => _.tagId),
        this.teachingLevels.map(_ => _.tagId),
        this.teachingCourseOfStudy.map(_ => _.tagId),
        this.jobFamily.map(_ => _.tagId),
        this.coCurricularActivity.map(_ => _.tagId),
        this.subGradeBanding.map(_ => _.tagId)
      )
    );
  }

  public getAllViolationPreRequisiteCourseIds(): string[] {
    return this.preRequisiteCourses.filter(_ => _.violationType !== CourseCriteriaLearnerViolationType.NotViolate).map(p => p.courseId);
  }
}
