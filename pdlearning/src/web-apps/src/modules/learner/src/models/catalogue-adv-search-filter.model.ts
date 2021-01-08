import {
  AttachmentType,
  CommunityType,
  Course,
  MetadataCodingScheme,
  MetadataId,
  MetadataTagGroupCode,
  MetadataTagModel,
  SelectDataModel
} from '@opal20/domain-api';

import { Utils } from '@opal20/infrastructure';

export class AdvanceSearchCatalogModel {
  public get allowedThumbnailExtensions(): string[] {
    return Course.allowedThumbnailExtensions;
  }

  //#region Metadata dropdown items
  public metadataTagsDic: Dictionary<MetadataTagModel> = {};
  public metadataTagsDicByGroupCode: Dictionary<MetadataTagModel[]> = {};
  public pdTypeSelectItems: MetadataTagModel[] = [];
  public categorySelectItems: MetadataTagModel[] = [];
  public attachmentTypeSelectItems: SelectDataModel[];
  public communityTypeSelectItems: SelectDataModel[];
  public modeOfLearningSelectItems: MetadataTagModel[] = [];
  public courseLevelSelectItems: MetadataTagModel[] = [];
  // public serviceSchemeSelectItems: MetadataTagModel[] = [];
  protected _serviceSchemeSelectItems: MetadataTagModel[] = [];
  public get serviceSchemeSelectItems(): MetadataTagModel[] {
    return this._serviceSchemeSelectItems;
  }
  public set serviceSchemeSelectItems(v: MetadataTagModel[]) {
    v.map(item => {
      const serviceSchemeWithSubject = this.buildSubjectSelectItems([item.tagId]);
      item.childs = serviceSchemeWithSubject;
      return item;
    });
    this._serviceSchemeSelectItems = v;
  }

  protected _subjectSelectItems: MetadataTagModel[] = [];
  public get subjectSelectItems(): MetadataTagModel[] {
    return this._subjectSelectItems;
  }
  public set subjectSelectItems(v: MetadataTagModel[]) {
    this._subjectSelectItems = v;
    this.subjectAreaIds = Utils.rightJoin(this.subjectAreaIds, this.subjectSelectItems.map(p => p.tagId));
  }

  protected _learningFrameworksSelectItems: MetadataTagModel[] = [];
  public get learningFrameworksSelectItems(): MetadataTagModel[] {
    return this._learningFrameworksSelectItems;
  }
  public set learningFrameworksSelectItems(v: MetadataTagModel[]) {
    this._learningFrameworksSelectItems = v;
    this.learningDimensionSelectItems = this.buildLearningDimensionSelectItems();
    this.learningFrameworkIds = Utils.rightJoin(this.learningFrameworkIds, this.learningFrameworksSelectItems.map(p => p.tagId));
  }
  protected _learningDimensionSelectItems: MetadataTagModel[] = [];
  public get learningDimensionSelectItems(): MetadataTagModel[] {
    return this._learningDimensionSelectItems;
  }
  public set learningDimensionSelectItems(v: MetadataTagModel[]) {
    this._learningDimensionSelectItems = v;
    this.learningAreaSelectItems = this.buildLearningAreaSelectItems();
    this.learningDimensionIds = Utils.rightJoin(this.learningDimensionIds, this.learningDimensionSelectItems.map(p => p.tagId));
  }
  protected _learningAreaSelectItems: MetadataTagModel[] = [];
  public get learningAreaSelectItems(): MetadataTagModel[] {
    return this._learningAreaSelectItems;
  }
  public set learningAreaSelectItems(v: MetadataTagModel[]) {
    this._learningAreaSelectItems = v;
    this.learningSubAreaSelectItems = this.buildLearningSubAreaSelectItems();
    this.learningAreaIds = Utils.rightJoin(this.learningAreaIds, this.learningAreaSelectItems.map(p => p.tagId));
  }
  protected _learningSubAreaSelectItems: MetadataTagModel[] = [];
  public get learningSubAreaSelectItems(): MetadataTagModel[] {
    return this._learningSubAreaSelectItems;
  }
  public set learningSubAreaSelectItems(v: MetadataTagModel[]) {
    this._learningSubAreaSelectItems = v;
    this.learningSubAreaIds = Utils.rightJoin(this.learningSubAreaIds, this.learningSubAreaSelectItems.map(p => p.tagId));
  }

  protected _natureCourseSelectItems: MetadataTagModel[] = [];
  public get natureCourseSelectItems(): MetadataTagModel[] {
    return this._natureCourseSelectItems;
  }
  public set natureCourseSelectItems(v: MetadataTagModel[]) {
    this._natureCourseSelectItems = v;
    this.natureOfCourse = Utils.rightJoinSingleBy(this.natureOfCourse, this.natureCourseSelectItems, p => p, p => p.tagId);
  }

  protected _developmentalRoleSelectItems: MetadataTagModel[] = [];
  public get developmentalRoleSelectItems(): MetadataTagModel[] {
    return this._developmentalRoleSelectItems;
  }
  public set developmentalRoleSelectItems(v: MetadataTagModel[]) {
    this._developmentalRoleSelectItems = v;
    this.developmentalRoleIds = Utils.rightJoin(this.developmentalRoleIds, this.developmentalRoleSelectItems.map(p => p.tagId));
  }
  protected _teachingLevelsSelectItems: MetadataTagModel[] = [];
  public get teachingLevelsSelectItems(): MetadataTagModel[] {
    return this._teachingLevelsSelectItems;
  }
  public set teachingLevelsSelectItems(v: MetadataTagModel[]) {
    this._teachingLevelsSelectItems = v;
    this.teachingLevels = Utils.rightJoin(this.teachingLevels, this.teachingLevelsSelectItems.map(p => p.tagId));
  }
  protected _teachingSubjectSelectItems: MetadataTagModel[] = [];
  public get teachingSubjectSelectItems(): MetadataTagModel[] {
    return this._teachingSubjectSelectItems;
  }
  public set teachingSubjectSelectItems(v: MetadataTagModel[]) {
    this._teachingSubjectSelectItems = v;
    this.teachingSubjectIds = Utils.rightJoin(this.teachingSubjectIds, this.teachingSubjectSelectItems.map(p => p.tagId));
  }
  //#endregion

  //#region Overview Info Tab
  protected _pdActivityType: string;
  public get pdActivityType(): string {
    return this._pdActivityType;
  }
  public set pdActivityType(pdActivityType: string) {
    this._pdActivityType = pdActivityType;
  }

  protected _categoryIds: string[] = [];
  public get categoryIds(): string[] {
    return this._categoryIds;
  }
  public set categoryIds(categoryIds: string[]) {
    this._categoryIds = categoryIds;
  }

  protected _attachmentType: string;
  public get attachmentType(): string {
    return this._attachmentType;
  }
  public set attachmentType(attachmentType: string) {
    this._attachmentType = attachmentType;
  }

  protected _communitiesType: string[] = [];
  public get communitiesType(): string[] {
    return this._communitiesType;
  }
  public set communitiesType(communitiesType: string[]) {
    this._communitiesType = communitiesType;
  }

  protected _learningMode: string;
  public get learningMode(): string {
    return this._learningMode;
  }
  public set learningMode(learningMode: string) {
    this._learningMode = learningMode;
  }

  protected _courseLevel: string;
  public get courseLevel(): string {
    return this._courseLevel;
  }
  public set courseLevel(courseLevel: string) {
    this._courseLevel = courseLevel;
  }
  //#endregion

  //#region Metadata
  protected _serviceSchemeIds: string[] = [];
  public get serviceSchemeIds(): string[] {
    return this._serviceSchemeIds;
  }
  public set serviceSchemeIds(serviceSchemeIds: string[]) {
    this._serviceSchemeIds = serviceSchemeIds;
    this.subjectSelectItems = this.buildSubjectSelectItems(this._serviceSchemeIds);
    this.learningFrameworksSelectItems = this.buildLearningFrameworksSelectItems();
    this.developmentalRoleSelectItems = this.buildDevelopmentalRoleSelectItems();
  }

  protected _subjectAreaIds: string[] = [];
  public get subjectAreaIds(): string[] {
    return this._subjectAreaIds;
  }
  public set subjectAreaIds(subjectAreaIds: string[]) {
    this._subjectAreaIds = subjectAreaIds;
  }

  protected _learningFrameworkIds: string[] = [];
  public get learningFrameworkIds(): string[] {
    return this._learningFrameworkIds;
  }
  public set learningFrameworkIds(learningFrameworkIds: string[]) {
    this._learningFrameworkIds = learningFrameworkIds;
    this.learningDimensionSelectItems = this.buildLearningDimensionSelectItems();
  }

  protected _learningDimensionIds: string[] = [];
  public get learningDimensionIds(): string[] {
    return this._learningDimensionIds;
  }
  public set learningDimensionIds(learningDimensionIds: string[]) {
    this._learningDimensionIds = learningDimensionIds;
    this.learningAreaSelectItems = this.buildLearningAreaSelectItems();
  }

  protected _learningAreaIds: string[] = [];
  public get learningAreaIds(): string[] {
    return this._learningAreaIds;
  }
  public set learningAreaIds(learningAreaIds: string[]) {
    this._learningAreaIds = learningAreaIds;
    this.learningSubAreaSelectItems = this.buildLearningSubAreaSelectItems();
  }

  protected _learningSubAreaIds: string[] = [];
  public get learningSubAreaIds(): string[] {
    return this._learningSubAreaIds;
  }
  public set learningSubAreaIds(learningSubAreaIds: string[]) {
    this._learningSubAreaIds = learningSubAreaIds;
  }

  //#endregion

  //#region Course Registration Conditions
  protected _developmentalRoleIds: string[] = [];
  public get developmentalRoleIds(): string[] {
    return this._developmentalRoleIds;
  }
  public set developmentalRoleIds(developmentalRoleIds: string[]) {
    this._developmentalRoleIds = developmentalRoleIds;
  }

  protected _teachingLevels: string[] = [];
  public get teachingLevels(): string[] {
    return this._teachingLevels;
  }
  public set teachingLevels(teachingLevels: string[]) {
    this._teachingLevels = teachingLevels;
  }

  protected _teachingSubjectIds: string[] = [];

  public get teachingSubjectIds(): string[] {
    return this._teachingSubjectIds;
  }
  public set teachingSubjectIds(teachingSubjectIds: string[]) {
    this._teachingSubjectIds = teachingSubjectIds;
  }

  protected _teachingCourseStudyIds: string[] = [];
  public get teachingCourseStudyIds(): string[] {
    return this._teachingCourseStudyIds;
  }
  public set teachingCourseStudyIds(teachingCourseStudyIds: string[]) {
    this._teachingCourseStudyIds = teachingCourseStudyIds;
  }
  //#endregion

  //#region Course Planning
  protected _natureOfCourse: string;
  public get natureOfCourse(): string {
    return this._natureOfCourse;
  }
  public set natureOfCourse(natureOfCourse: string) {
    this._natureOfCourse = natureOfCourse;
  }
  //#endregion

  constructor(public metadataTags: MetadataTagModel[] = []) {
    this.setMetadataTagsDicInfo(metadataTags);

    //#region Metadata dropdown items
    this.pdTypeSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TYPES], []);
    this.categorySelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_CATEGORIES], []);
    this.attachmentTypeSelectItems = this.buildAttachmentTypeSelectItems();
    this.communityTypeSelectItems = this.buildcommunitiesTypeSelectItems();
    this.modeOfLearningSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_MODES], []);
    this.courseLevelSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.COURSE_LEVELS], []);
    this.serviceSchemeSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.SERVICE_SCHEMES], []);
    this._subjectSelectItems = this.buildSubjectSelectItems(this.serviceSchemeIds);
    this._learningFrameworksSelectItems = this.buildLearningFrameworksSelectItems();
    this._learningDimensionSelectItems = this.buildLearningDimensionSelectItems();
    this._learningAreaSelectItems = this.buildLearningAreaSelectItems();
    this._learningSubAreaSelectItems = this.buildLearningSubAreaSelectItems();
    this._natureCourseSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_NATURES], []);
    this._developmentalRoleSelectItems = this.buildDevelopmentalRoleSelectItems();
    this._teachingLevelsSelectItems = Utils.orderBy(
      Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.TEACHING_LEVELS], []),
      p => p.displayText
    );
    this._teachingSubjectSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.TEACHING_SUBJECTS], []);
    //#endregion

    this.removeUselessFillterData();
  }

  public clone(): AdvanceSearchCatalogModel {
    const cloneOne = new AdvanceSearchCatalogModel(this.metadataTags);

    cloneOne._pdActivityType = this._pdActivityType;
    cloneOne._learningFrameworksSelectItems = this._learningFrameworksSelectItems;
    cloneOne._learningMode = this._learningMode;
    cloneOne._categoryIds = this._categoryIds;
    cloneOne._serviceSchemeIds = this._serviceSchemeIds;
    cloneOne._developmentalRoleIds = this._developmentalRoleIds;
    cloneOne._teachingLevels = this._teachingLevels;
    cloneOne._courseLevel = this._courseLevel;
    cloneOne._subjectAreaIds = this._subjectAreaIds;
    cloneOne._learningFrameworkIds = this._learningFrameworkIds;
    cloneOne._learningDimensionIds = this._learningDimensionIds;
    cloneOne._learningAreaIds = this._learningAreaIds;
    cloneOne._natureOfCourse = this._natureOfCourse;
    cloneOne._learningSubAreaIds = this._learningSubAreaIds;
    cloneOne._attachmentType = this._attachmentType;
    cloneOne._communitiesType = this._communitiesType;

    return cloneOne;
  }

  public isMicrolearning(): boolean {
    return this.pdActivityType === MetadataId.Microlearning;
  }

  public updateMetadataTags(metadataTags: MetadataTagModel[]): void {
    this.setMetadataTagsDicInfo(metadataTags);

    this.pdTypeSelectItems = this.pdTypeSelectItems.map(p => this.metadataTagsDic[p.tagId]).filter(p => p != null);
    this.categorySelectItems = this.categorySelectItems.map(p => this.metadataTagsDic[p.tagId]).filter(p => p != null);
    this.modeOfLearningSelectItems = this.modeOfLearningSelectItems.map(p => this.metadataTagsDic[p.tagId]).filter(p => p != null);
    this.courseLevelSelectItems = this.courseLevelSelectItems.map(p => this.metadataTagsDic[p.tagId]).filter(p => p != null);
    this.serviceSchemeSelectItems = this.serviceSchemeSelectItems.map(p => this.metadataTagsDic[p.tagId]).filter(p => p != null);

    this.subjectSelectItems = this.subjectSelectItems.map(p => this.metadataTagsDic[p.tagId]).filter(p => p != null);
    this.learningFrameworksSelectItems = this.learningFrameworksSelectItems.map(p => this.metadataTagsDic[p.tagId]).filter(p => p != null);
    this.learningDimensionSelectItems = this.learningDimensionSelectItems.map(p => this.metadataTagsDic[p.tagId]).filter(p => p != null);
    this.learningAreaSelectItems = this.learningAreaSelectItems.map(p => this.metadataTagsDic[p.tagId]).filter(p => p != null);
    this.learningSubAreaSelectItems = this.learningSubAreaSelectItems.map(p => this.metadataTagsDic[p.tagId]).filter(p => p != null);
  }

  public getAllTagIds(): string[] {
    return []
      .concat(this.pdActivityType ? [this.pdActivityType] : [])
      .concat(this.courseLevel ? [this.courseLevel] : [])
      .concat(this.serviceSchemeIds || [])
      .concat(this.subjectAreaIds || [])
      .concat(this.learningFrameworkIds || [])
      .concat(this.learningDimensionIds || [])
      .concat(this.learningAreaIds || [])
      .concat(this.learningSubAreaIds || [])
      .concat(this.categoryIds || [])
      .concat(this.teachingCourseStudyIds || [])
      .concat(this.teachingLevels || [])
      .concat(this.teachingSubjectIds || [])
      .concat(this.developmentalRoleIds || [])
      .concat(this.learningMode || [])
      .concat(this.natureOfCourse || []);
  }

  // tslint:disable-next-line:no-any
  public getTagsIncludedTagNames(): any {
    return {
      pdActivityType: this.pdActivityType ? [this.pdActivityType] : [],
      courseLevel: this.courseLevel ? [this.courseLevel] : [],
      serviceSchemeIds: this.serviceSchemeIds || [],
      subjectAreaIds: this.subjectAreaIds || [],
      learningFrameworkIds: this.learningFrameworkIds || [],
      learningDimensionIds: this.learningDimensionIds || [],
      learningAreaIds: this.learningAreaIds || [],
      learningSubAreaIds: this.learningSubAreaIds || [],
      categoryIds: this.categoryIds || [],
      teachingCourseStudyIds: this.teachingCourseStudyIds || [],
      teachingLevels: this.teachingLevels || [],
      teachingSubjectIds: this.teachingSubjectIds || [],
      developmentalRoleIds: this.developmentalRoleIds || [],
      learningMode: !this.learningMode ? [] : [this.learningMode],
      natureOfCourse: !this.natureOfCourse ? [] : [this.natureOfCourse]
    };
  }

  public hasOnlyOneServiceSchemesChecked(): boolean {
    return this.serviceSchemeIds && this.serviceSchemeIds.length === 1;
  }

  public serviceSchemesContains(codingScheme: MetadataCodingScheme): boolean {
    return this.serviceSchemeIds && this.serviceSchemeIds.find(p => this.metadataTagsDic[p].codingScheme === codingScheme) != null;
  }

  public get count(): number {
    return (
      // array
      (this.categoryIds.length ? 1 : 0) +
      (this.serviceSchemeIds.length ? 1 : 0) +
      (this.subjectAreaIds.length ? 1 : 0) +
      (this.learningFrameworkIds.length ? 1 : 0) +
      (this.learningDimensionIds.length ? 1 : 0) +
      (this.learningAreaIds.length ? 1 : 0) +
      (this.learningSubAreaIds.length ? 1 : 0) +
      (this.developmentalRoleIds.length ? 1 : 0) +
      (this.teachingLevels.length ? 1 : 0) +
      // string
      (this.pdActivityType && this.pdActivityType !== '' ? 1 : 0) +
      (this.learningMode && this.learningMode !== '' ? 1 : 0) +
      (this.courseLevel && this.courseLevel !== '' ? 1 : 0) +
      (this.natureOfCourse && this.natureOfCourse !== '' ? 1 : 0) +
      (this.attachmentType && this.attachmentType !== '' ? 1 : 0) +
      (this.communitiesType.length ? 1 : 0)
    );
  }

  private removeUselessFillterData(): void {
    if (this.serviceSchemeSelectItems.length > 0) {
      this.serviceSchemeSelectItems = this.serviceSchemeSelectItems.filter(element => {
        if (element.codingScheme && element.fullStatement) {
          return element.codingScheme.toLocaleLowerCase() !== 'n/a' || element.fullStatement.toLocaleLowerCase() !== 'not applicable';
        }
        return true;
      });
    }

    if (this.teachingLevelsSelectItems.length > 0) {
      this.teachingLevelsSelectItems = this.teachingLevelsSelectItems.filter(element => {
        if (element.codingScheme && element.fullStatement) {
          return element.codingScheme.toLocaleLowerCase() !== 'n/a' || element.fullStatement.toLocaleLowerCase() !== 'not applicable';
        }
        return true;
      });
    }
  }

  //#region Metadata
  private buildSubjectSelectItems(serviceSchemeIds: string[]): MetadataTagModel[] {
    return Utils.rightJoinBy(
      Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TAXONOMY], []).filter(p => p.codingScheme != null),
      serviceSchemeIds,
      p => p.parentTagId,
      p => p
    );
  }

  private buildDevelopmentalRoleSelectItems(): MetadataTagModel[] {
    return Utils.orderBy(
      Utils.rightJoinBy(
        Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.DEVROLES], []),
        this.serviceSchemeIds,
        p => p.parentTagId,
        p => p
      ),
      p => p.displayText
    );
  }

  private buildLearningFrameworksSelectItems(): MetadataTagModel[] {
    return Utils.rightJoinBy(
      Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.LEARNING_FXS], []),
      this.serviceSchemeIds,
      p => p.parentTagId,
      p => p
    );
  }

  private buildLearningDimensionSelectItems(): MetadataTagModel[] {
    return Utils.rightJoinBy(this.metadataTags, this.learningFrameworkIds, p => p.parentTagId, p => p);
  }

  private buildLearningAreaSelectItems(): MetadataTagModel[] {
    return Utils.rightJoinBy(this.metadataTags, this.learningDimensionIds, p => p.parentTagId, p => p);
  }

  private buildLearningSubAreaSelectItems(): MetadataTagModel[] {
    return Utils.rightJoinBy(this.metadataTags, this.learningAreaIds, p => p.parentTagId, p => p);
  }

  private buildAttachmentTypeSelectItems(): SelectDataModel[] {
    return Object.keys(AttachmentType).map(
      key =>
        new SelectDataModel({
          id: key,
          displayText: AttachmentType[key].toString()
        })
    );
  }

  private buildcommunitiesTypeSelectItems(): SelectDataModel[] {
    return Object.keys(CommunityType).map(
      key =>
        new SelectDataModel({
          id: key,
          displayText: CommunityType[key].toString()
        })
    );
  }

  private setMetadataTagsDicInfo(metadataTags: MetadataTagModel[]): void {
    this.metadataTagsDic = Utils.toDictionary(metadataTags, p => p.tagId);
    this.metadataTagsDicByGroupCode = Utils.toDictionaryGroupBy(metadataTags.filter(p => p.groupCode != null), p => p.groupCode);
  }
  //#endregion
}
