import { BaseUserInfo, DepartmentInfoModel, MetadataTagModel, UserInfoModel } from '@opal20/domain-api';

import { Utils } from '@opal20/infrastructure';

export class CommunityMetadataListingModel {
  public pdTypeSelectItems: MetadataTagModel[] = [];
  public categorySelectItems: MetadataTagModel[] = [];
  public departmentsDic: Dictionary<DepartmentInfoModel> = {};
  public ownerDivisionSelectItems: DepartmentInfoModel[] = [];
  public ownerBranchSelectItems: DepartmentInfoModel[] = [];
  public moeOfficerDic: Dictionary<UserInfoModel> = {};
  public _moeOfficerItems: UserInfoModel[] = [];
  public subjectSelectItems: MetadataTagModel[];
  public divisionSelectItems: DepartmentInfoModel[] = [];
  public branchSelectItems: DepartmentInfoModel[] = [];
  public zoneSelectItems: DepartmentInfoModel[] = [];
  public clusterSelectItems: DepartmentInfoModel[] = [];
  public schoolSelectItems: DepartmentInfoModel[] = [];
  public trackSelectItems: MetadataTagModel[] = [];
  public metadataTagsDic: Dictionary<MetadataTagModel> = {};
  public metadataTagsDicByGroupCode: Dictionary<MetadataTagModel[]> = {};
  public serviceSchemeSelectItems: MetadataTagModel[] = [];
  public learningFrameworksSelectItems: MetadataTagModel[];
  public learningDimensionSelectItems: MetadataTagModel[];
  public learningAreaSelectItems: MetadataTagModel[];
  public learningSubAreaSelectItems: MetadataTagModel[];
  public developmentalRoleSelectItems: MetadataTagModel[];
  public teachingLevelsSelectItems: MetadataTagModel[];
  public teachingCourseStudySelectItems: MetadataTagModel[];
  public modeOfLearningSelectItems: MetadataTagModel[] = [];
  public usersDic: Dictionary<BaseUserInfo> = {};
  private _moderatorItems: BaseUserInfo[] = [];
  private _coModeratorItems: BaseUserInfo[] = [];
  public get coModeratorItems(): BaseUserInfo[] {
    return this._coModeratorItems;
  }
  public set coModeratorItems(v: BaseUserInfo[]) {
    if (Utils.isDifferent(this._coModeratorItems, v)) {
      this._coModeratorItems = v;
      this.usersDic = { ...this.usersDic, ...Utils.toDictionary(v, p => p.id) };
    }
  }
  public get moderatorItems(): BaseUserInfo[] {
    return this._moderatorItems;
  }
  public set moderatorItems(v: BaseUserInfo[]) {
    if (Utils.isDifferent(this._moderatorItems, v)) {
      this._moderatorItems = v;
      this.usersDic = { ...this.usersDic, ...Utils.toDictionary(v, p => p.id) };
    }
  }
  public get moeOfficerItems(): UserInfoModel[] {
    return this._moeOfficerItems;
  }
  public set moeOfficerItems(v: UserInfoModel[]) {
    if (Utils.isDifferent(this._moeOfficerItems, v)) {
      this._moeOfficerItems = v;
      this.moeOfficerDic = { ...this.moeOfficerDic, ...Utils.toDictionary(v, p => p.id) };
    }
  }
}
