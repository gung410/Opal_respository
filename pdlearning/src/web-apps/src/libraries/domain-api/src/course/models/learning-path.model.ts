import { ILearningPathCourseModel, LearningPathCourseModel } from './learning-path-course-model';
import { SystemRoleEnum, UserInfoModel } from './../../share/models/user-info.model';

import { LMM_PERMISSIONS } from '@opal20/domain-api/share/permission-keys/lmm-permission-key';
export enum LearningPathStatus {
  Published = 'Published',
  Unpublished = 'Unpublished'
}

export interface ILearningPathModel {
  id?: string;
  title: string;
  createdDate: Date;
  createdBy?: string;
  status?: LearningPathStatus;
  thumbnailUrl?: string;
  listCourses: ILearningPathCourseModel[];
  description?: string;

  // Metadata
  courseLevelIds?: string[];
  pdAreaThemeIds?: string[];
  serviceSchemeIds?: string[];
  subjectAreaIds?: string[];
  learningFrameworkIds?: string[];
  learningDimensionIds?: string[];
  learningAreaIds?: string[];
  learningSubAreaIds?: string[];
  teacherOutcomeIds?: string[];
  categoryIds?: string[];
  teachingCourseStudyIds?: string[];
  teachingLevels?: string[];
  metadataKeys?: string[];
}

export class LearningPathModel implements ILearningPathModel {
  public id?: string;
  public title: string = '';
  public createdDate: Date = new Date();
  public createdBy?: string;
  public status?: LearningPathStatus = LearningPathStatus.Unpublished;
  public thumbnailUrl?: string;
  public listCourses: LearningPathCourseModel[] = [];

  // Metadata
  public courseLevelIds?: string[] = [];
  public pdAreaThemeIds?: string[] = [];
  public serviceSchemeIds: string[] = [];
  public subjectAreaIds: string[] = [];
  public learningFrameworkIds: string[] = [];
  public learningDimensionIds: string[] = [];
  public learningAreaIds: string[] = [];
  public learningSubAreaIds: string[] = [];
  public metadataKeys: string[] = [];

  public static hasCreateLearningPathPermission(user: UserInfoModel): boolean {
    return (
      user.hasPermissionPrefix(LMM_PERMISSIONS.CreateEditPublishUnpublishLP) ||
      user.hasAdministratorRoles() ||
      user.hasRole(SystemRoleEnum.CourseContentCreator)
    );
  }

  public static hasViewLearningPathDetailPermission(user: UserInfoModel): boolean {
    return user.hasPermissionPrefix(LMM_PERMISSIONS.ViewLearningPathDetail);
  }

  constructor(data?: ILearningPathModel) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.title = data.title;
    this.createdDate = new Date(data.createdDate);
    this.createdBy = data.createdBy;
    this.thumbnailUrl = data.thumbnailUrl;
    this.listCourses = data.listCourses ? data.listCourses.map(p => new LearningPathCourseModel(p)) : [];
    this.status = data.status;

    // Metadata
    this.courseLevelIds = data.courseLevelIds;
    this.pdAreaThemeIds = data.pdAreaThemeIds;
    this.serviceSchemeIds = data.serviceSchemeIds;
    this.subjectAreaIds = data.subjectAreaIds;
    this.learningFrameworkIds = data.learningFrameworkIds;
    this.learningDimensionIds = data.learningDimensionIds;
    this.learningAreaIds = data.learningAreaIds;
    this.learningSubAreaIds = data.learningSubAreaIds;
    this.metadataKeys = data.metadataKeys;
  }

  public getAllMetadataTagIds(): string[] {
    return []
      .concat(this.courseLevelIds)
      .concat(this.pdAreaThemeIds)
      .concat(this.serviceSchemeIds)
      .concat(this.subjectAreaIds)
      .concat(this.learningFrameworkIds)
      .concat(this.learningDimensionIds)
      .concat(this.learningAreaIds)
      .concat(this.learningSubAreaIds);
  }

  public hasEditPublishUnpublishPermission(user: UserInfoModel): boolean {
    return user.hasPermissionPrefix(LMM_PERMISSIONS.CreateEditPublishUnpublishLP);
  }

  public hasViewCopyHyperLinkLearningPathButtonPermission(user: UserInfoModel): boolean {
    return user.hasPermissionPrefix(LMM_PERMISSIONS.CopyHyperLinkLearningPathButton);
  }

  public hasOwnerPermission(user: UserInfoModel): boolean {
    return this.createdBy === user.id || user.hasAdministratorRoles();
  }

  public hasPopulateMetadataLearningPathPermission(user: UserInfoModel): boolean {
    return (
      user.hasRole(SystemRoleEnum.CourseContentCreator) ||
      this.hasOwnerPermission(user) ||
      user.hasPermissionPrefix(LMM_PERMISSIONS.PopulateMetadataLearningPath)
    );
  }

  public isPublished(): boolean {
    return this.status === LearningPathStatus.Published;
  }
}
