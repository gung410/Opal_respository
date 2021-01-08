import {
  MetadataTagGroupCode,
  MetadataTagModel,
  NOT_APPLICABLE_ITEM_DISPLAY_TEXT,
  OrganizationRepository,
  OrganizationUnitLevelEnum,
  TaggingRepository,
  UserRepository
} from '@opal20/domain-api';

import { BehaviorSubject } from 'rxjs';
import { CommunityMetadataListingModel } from '../models/community-metadata-listing.model';
import { CommunityMetadataViewModel } from '../models/community-metadata.model';
import { Injectable } from '@angular/core';
import { Utils } from '@opal20/infrastructure';

@Injectable({ providedIn: 'root' })
export class CommunityMetadataBuilderService {
  protected subject: BehaviorSubject<CommunityMetadataListingModel>;

  protected allMetadata: MetadataTagModel[] = [];
  protected metadataListingModel: CommunityMetadataListingModel;

  constructor(
    protected taggingRepository: TaggingRepository,
    private userRepository: UserRepository,
    private organizationRepository: OrganizationRepository
  ) {
    this.subject = new BehaviorSubject<CommunityMetadataListingModel>(new CommunityMetadataListingModel());
  }

  public buildMetadataListingSubject(metadataModel?: CommunityMetadataViewModel): Promise<BehaviorSubject<CommunityMetadataListingModel>> {
    return new Promise((resolve, reject) => {
      if (this.allMetadata && this.allMetadata.length) {
        return resolve(this.subject);
      }
      this.taggingRepository.loadAllMetaDataTags().subscribe(metadata => {
        this.allMetadata = metadata;
        this.metadataListingModel = new CommunityMetadataListingModel();
        this.buildStaticSelectItems(this.allMetadata);
        this.loadMOEOfficers();
        this.loadOrganizations();
        // Build on-demand for the model was provided
        if (metadataModel) {
          if (!Utils.isEmpty(metadataModel.serviceSchemeIds)) {
            this.buildSubjectSelectItems(metadataModel.serviceSchemeIds);
            this.buildLearningFrameworksSelectItems(metadataModel.serviceSchemeIds);
            this.buildDevelopmentalRoleSelectItems(metadataModel.serviceSchemeIds);
          }

          if (!Utils.isEmpty(metadataModel.learningFrameworkIds)) {
            this.buildLearningDimensionSelectItems(metadataModel.learningFrameworkIds);
          }

          if (!Utils.isEmpty(metadataModel.learningDimensionIds)) {
            this.buildLearningAreaSelectItems(metadataModel.learningDimensionIds);
          }

          if (!Utils.isEmpty(metadataModel.learningAreaIds)) {
            this.buildLearningSubAreaSelectItems(metadataModel.learningAreaIds);
          }

          const userIds = []
            .concat(metadataModel.moeOfficerId ? [metadataModel.moeOfficerId] : [])
            .concat(metadataModel.moderatorId ? [metadataModel.moderatorId] : [])
            .concat(metadataModel.coModeratorId ? [metadataModel.coModeratorId] : []);
          if (userIds.length > 0) {
            this.loadUsersDic(userIds);
          }
        }

        this.subject.next(this.metadataListingModel);
        return resolve(this.subject);
      });
    });
  }

  public buildSubjectSelectItems(serviceSchemeIds: string[]): MetadataTagModel[] {
    this.metadataListingModel.subjectSelectItems = Utils.rightJoinBy(
      Utils.defaultIfNull(this.metadataListingModel.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TAXONOMY], []).filter(
        p => p.codingScheme != null
      ),
      serviceSchemeIds,
      p => p.parentTagId,
      p => p
    );
    this.subject.next(this.metadataListingModel);
    return this.metadataListingModel.subjectSelectItems;
  }

  public buildLearningFrameworksSelectItems(serviceSchemeIds: string[]): MetadataTagModel[] {
    this.metadataListingModel.learningFrameworksSelectItems = Utils.rightJoinBy(
      Utils.defaultIfNull(this.metadataListingModel.metadataTagsDicByGroupCode[MetadataTagGroupCode.LEARNING_FXS], []),
      serviceSchemeIds,
      p => p.parentTagId,
      p => p
    );
    this.subject.next(this.metadataListingModel);
    return this.metadataListingModel.learningFrameworksSelectItems;
  }

  public buildLearningDimensionSelectItems(learningFrameworkIds: string[]): MetadataTagModel[] {
    this.metadataListingModel.learningDimensionSelectItems = Utils.rightJoinBy(
      this.allMetadata,
      learningFrameworkIds,
      p => p.parentTagId,
      p => p
    );
    this.subject.next(this.metadataListingModel);
    return this.metadataListingModel.learningDimensionSelectItems;
  }

  public buildLearningAreaSelectItems(learningDimensionIds: string[]): MetadataTagModel[] {
    this.metadataListingModel.learningAreaSelectItems = Utils.rightJoinBy(
      this.allMetadata,
      learningDimensionIds,
      p => p.parentTagId,
      p => p
    );
    this.subject.next(this.metadataListingModel);

    return this.metadataListingModel.learningAreaSelectItems;
  }

  public buildLearningSubAreaSelectItems(learningAreaIds: string[]): MetadataTagModel[] {
    this.metadataListingModel.learningSubAreaSelectItems = Utils.rightJoinBy(this.allMetadata, learningAreaIds, p => p.parentTagId, p => p);
    this.subject.next(this.metadataListingModel);
    return this.metadataListingModel.learningSubAreaSelectItems;
  }

  public buildDevelopmentalRoleSelectItems(serviceSchemeIds: string[]): MetadataTagModel[] {
    this.metadataListingModel.developmentalRoleSelectItems = Utils.rightJoinBy(
      Utils.defaultIfNull(this.metadataListingModel.metadataTagsDicByGroupCode[MetadataTagGroupCode.DEVROLES], []),
      serviceSchemeIds,
      p => p.parentTagId,
      p => p
    );
    return this.metadataListingModel.developmentalRoleSelectItems;
  }

  public loadMOEOfficers(): void {
    this.metadataListingModel.moeOfficerItems = [];
    this.metadataListingModel.moeOfficerDic = {};

    this.userRepository.loadMOEOfficers([], true).subscribe(officers => {
      this.metadataListingModel.moeOfficerItems = officers;
      this.metadataListingModel.moeOfficerDic = Utils.toDictionary(officers, p => p.id);

      this.subject.next(this.metadataListingModel);
    });
  }

  public loadUsersDic(userIds: string[]): void {
    this.metadataListingModel.usersDic = {};
    this.userRepository
      .loadBaseUserInfoList(
        {
          extIds: userIds,
          pageSize: 0,
          pageIndex: 0,
          entityStatuses: ['All']
        },
        true
      )
      .subscribe(users => {
        this.metadataListingModel.usersDic = Utils.toDictionary(users, p => p.id);
        this.subject.next(this.metadataListingModel);
      });
  }

  public loadOrganizations(): void {
    this.organizationRepository
      .loadDepartmentInfoList({
        departmentId: 1,
        includeChildren: true,
        includeDepartmentType: true,
        getParentDepartmentId: true
      })
      .subscribe(result => {
        this.metadataListingModel.departmentsDic = Utils.toDictionary(result, department => department.id);
        this.metadataListingModel.ownerDivisionSelectItems = result.filter(
          department => department.departmentType === OrganizationUnitLevelEnum.Division
        );
        this.metadataListingModel.ownerBranchSelectItems = result.filter(
          department => department.departmentType === OrganizationUnitLevelEnum.Branch
        );
        this.subject.next(this.metadataListingModel);
      });
  }

  private buildStaticSelectItems(metadataTags: MetadataTagModel[]): void {
    this.metadataListingModel.metadataTagsDic = Utils.toDictionary(metadataTags, p => p.tagId);
    this.metadataListingModel.metadataTagsDicByGroupCode = Utils.toDictionaryGroupBy(
      metadataTags.filter(p => p.groupCode != null),
      p => p.groupCode
    );

    this.metadataListingModel.pdTypeSelectItems = Utils.defaultIfNull(
      this.metadataListingModel.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TYPES],
      []
    );
    this.metadataListingModel.categorySelectItems = Utils.defaultIfNull(
      this.metadataListingModel.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_CATEGORIES],
      []
    );
    this.metadataListingModel.modeOfLearningSelectItems = Utils.defaultIfNull(
      this.metadataListingModel.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_MODES],
      []
    );
    this.metadataListingModel.serviceSchemeSelectItems = Utils.defaultIfNull(
      Utils.rightJoinBy(
        Utils.defaultIfNull(this.metadataListingModel.metadataTagsDicByGroupCode[MetadataTagGroupCode.SERVICE_SCHEMES], []),
        Utils.defaultIfNull(this.metadataListingModel.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TAXONOMY], []),
        x => x.tagId,
        x => x.parentTagId
      ),
      []
    );

    this.metadataListingModel.trackSelectItems = this.buildTrackSelectItems(
      Utils.defaultIfNull(this.metadataListingModel.metadataTagsDicByGroupCode[MetadataTagGroupCode.TRACKS], []),
      NOT_APPLICABLE_ITEM_DISPLAY_TEXT
    );

    this.metadataListingModel.teachingLevelsSelectItems = Utils.defaultIfNull(
      this.metadataListingModel.metadataTagsDicByGroupCode[MetadataTagGroupCode.TEACHING_LEVELS],
      []
    );

    this.metadataListingModel.teachingCourseStudySelectItems = Utils.distinctBy(
      Utils.defaultIfNull(this.metadataListingModel.metadataTagsDicByGroupCode[MetadataTagGroupCode.COURSES_OF_STUDY], []),
      p => p.displayText
    );
  }

  private buildTrackSelectItems(tracks: MetadataTagModel[], target: string): MetadataTagModel[] {
    return Utils.moveItemsToTheEndOfList(tracks, p => p.displayText === target);
  }
}
