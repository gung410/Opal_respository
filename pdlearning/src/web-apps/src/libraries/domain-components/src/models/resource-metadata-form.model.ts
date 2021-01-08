import { MetadataTagGroupCode, MetadataTagModel, ResourceModel } from '@opal20/domain-api';
import { Observable, of } from 'rxjs';

import { Utils } from '@opal20/infrastructure';

export class ResourceMetadataFormModel {
  public pdTypeSelectItems: MetadataTagModel[] = [];
  public get subjectAreaSelectItems(): MetadataTagModel[] {
    return this._subjectAreaSelectItems;
  }
  public set subjectAreaSelectItems(v: MetadataTagModel[]) {
    this._subjectAreaSelectItems = v;
    if (v.map(p => p.tagId).indexOf(this.mainSubjectArea) < 0) {
      this.mainSubjectArea = undefined;
    }
  }
  public get subjectAreasAndKeywordsTree(): MetadataTagModel[] {
    return this._subjectAreasAndKeywordsTree;
  }
  public set subjectAreasAndKeywordsTree(v: MetadataTagModel[]) {
    this._subjectAreasAndKeywordsTree = v;
    this.setSubjectAreasAndKeywords(this.getCurrentValidSubjectAreasAndKeywords());
  }
  public get developmentalRolesTree(): MetadataTagModel[] {
    return this._developmentalRolesTree;
  }
  public set developmentalRolesTree(v: MetadataTagModel[]) {
    this._developmentalRolesTree = v;
    this.setDevelopmentalRoles(this.getCurrentValidDevelopmentalRoles());
  }
  public get learningFrameworksSelectItems(): MetadataTagModel[] {
    return this._learningFrameworksSelectItems;
  }
  public set learningFrameworksSelectItems(v: MetadataTagModel[]) {
    this._learningFrameworksSelectItems = v;
    this.learningFrameworks = this.getCurrentValidLearningFrameworks();
  }
  public get dimensionsAndAreasTree(): MetadataTagModel[] {
    return this._dimensionsAndAreasTree;
  }
  public set dimensionsAndAreasTree(v: MetadataTagModel[]) {
    this._dimensionsAndAreasTree = v;
    this.setDimensionsAndAreas(this.getCurrentValidDimensionsAndAreas());
  }

  public get pdOpportunityType(): string | undefined {
    const pdOpTypes = this.resourceMetadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TYPES];
    if (pdOpTypes == null || pdOpTypes.length === 0) {
      return undefined;
    }
    return pdOpTypes[0].tagId;
  }
  public set pdOpportunityType(value: string | undefined) {
    if (!Utils.isDifferent(this.pdOpportunityType, value)) {
      return;
    }
    this.setSingleResourceTag(this.pdOpportunityType, value);
  }
  public get serviceSchemes(): string[] {
    return this._serviceSchemes;
  }
  public set serviceSchemes(value: string[]) {
    if (Utils.isDifferent(this._serviceSchemes, value)) {
      this.setMultipleResourceTag(this.getCurrentResourceServiceSchemes(), value);
      this._serviceSchemes = value;
      this.subjectAreaSelectItems = this.buildDropdownSubjectAreaTagItems();
      this.developmentalRolesTree = this.buildDevelopmentalRolesTree();
      this.subjectAreasAndKeywordsTree = this.buildSubjectAreasAndKeywordsTree();
      this.learningFrameworksSelectItems = this.buildLearningFrameworksSelectItems();
    }
  }
  public get mainSubjectArea(): string | undefined {
    return this.resource.mainSubjectAreaTagId;
  }
  public set mainSubjectArea(value: string | undefined) {
    if (value != null) {
      this.setSingleResourceTag(this.resource.mainSubjectAreaTagId, value);
      this._subjectAreasAndKeywords = this.getCurrentSubjectAreasAndKeywords();
    }
    this.resource.mainSubjectAreaTagId = value;
  }
  public get courseLevels(): string[] {
    return this._courseLevels;
  }
  public set courseLevels(value: string[]) {
    this.setMultipleResourceTag(this.getCurrentResourceCourseLevels(), value);
    this._courseLevels = value;
  }
  public get developmentalRoles(): string[] {
    return this._developmentalRoles;
  }
  public set developmentalRoles(value: string[]) {
    this.setDevelopmentalRoles(value);
  }
  public get subjectAreasAndKeywords(): string[] {
    return this._subjectAreasAndKeywords;
  }
  public set subjectAreasAndKeywords(value: string[]) {
    this.setSubjectAreasAndKeywords(value);
  }
  public get learningFrameworks(): string[] {
    return this._learningFrameworks;
  }
  public set learningFrameworks(value: string[]) {
    if (Utils.isDifferent(this._learningFrameworks, value)) {
      const currentDimensionsAndAreas = this.getCurrentDimensionsAndAreas();
      this.setMultipleResourceTag(this.getCurrentResourceLearningFrameworks(), value);
      this._learningFrameworks = value;
      this.dimensionsAndAreasTree = this.buildDimensionsAndAreasTree();
      this.setMultipleResourceTag(currentDimensionsAndAreas, this.getCurrentValidDimensionsAndAreas());
    }
  }
  public get dimensionsAndAreas(): string[] {
    return this._dimensionsAndAreas;
  }
  public set dimensionsAndAreas(value: string[]) {
    this.setDimensionsAndAreas(value);
  }
  public orderedCourseLevelDislayTexts: string[] = ['Emergent', 'Proficient', 'Accomplished', 'Leading', 'Not Applicable'];
  public resource: ResourceModel;
  public metadataTags: MetadataTagModel[];

  public metadataTagsDic: Dictionary<MetadataTagModel> = {};
  public metadataTagsDicByParentTagId: Dictionary<MetadataTagModel[]> = {};
  public metadataTagsDicByGroupCode: Dictionary<MetadataTagModel[]> = {};
  public resourceMetadataTagsDicByGroupCode: Dictionary<MetadataTagModel[]> = {};

  private _subjectAreaSelectItems: MetadataTagModel[] = [];
  private _developmentalRolesTree: MetadataTagModel[] = [];
  private _subjectAreasAndKeywordsTree: MetadataTagModel[] = [];
  private _learningFrameworksSelectItems: MetadataTagModel[] = [];
  private _dimensionsAndAreasTree: MetadataTagModel[] = [];

  private _subjectAreasAndKeywords: string[] = [];
  private _dimensionsAndAreas: string[] = [];
  private _serviceSchemes: string[] = [];
  private _courseLevels: string[] = [];
  private _developmentalRoles: string[] = [];
  private _learningFrameworks: string[] = [];

  constructor(resource: ResourceModel | undefined, metadataTags: MetadataTagModel[]) {
    this.resource = resource == null ? new ResourceModel() : resource;
    this.metadataTags = metadataTags;

    this.metadataTagsDic = Utils.toDictionary(this.metadataTags, p => p.tagId);
    this.resourceMetadataTagsDicByGroupCode = this.buildResourceMetadataTagsDicByGroupCode();
    this.metadataTagsDicByGroupCode = this.buildMetadataTagsDicByGroupCode(this.metadataTags);
    this.metadataTagsDicByParentTagId = this.buildMetadataTagsDicByParentTagId(this.metadataTags);

    this._serviceSchemes = this.getCurrentResourceServiceSchemes();

    this._courseLevels = this.getCurrentResourceCourseLevels();
    this._learningFrameworks = this.getCurrentResourceLearningFrameworks();

    this.pdTypeSelectItems = this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TYPES]
      ? this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TYPES]
      : [];
    this._subjectAreaSelectItems = this.buildDropdownSubjectAreaTagItems();
    this._developmentalRolesTree = this.buildDevelopmentalRolesTree();
    this._subjectAreasAndKeywordsTree = this.buildSubjectAreasAndKeywordsTree();
    this._learningFrameworksSelectItems = this.buildLearningFrameworksSelectItems();
    this._dimensionsAndAreasTree = this.buildDimensionsAndAreasTree();

    this._developmentalRoles = this.getCurrentResourceDevelopmentalRoles(true);
    this._subjectAreasAndKeywords = this.getCurrentSubjectAreasAndKeywords(true);
    this._dimensionsAndAreas = this.getCurrentDimensionsAndAreas(true);
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

  public setDimensionsAndAreas(value: string[]): void {
    this.setMultipleResourceTag(
      this.getCurrentDimensionsAndAreas(),
      value.filter(p => this.metadataTagsDic[p].groupCode !== MetadataTagGroupCode.LEARNING_FXS)
    );
    this._dimensionsAndAreas = value;
  }
  public setSubjectAreasAndKeywords(value: string[]): void {
    this.setMultipleResourceTag(
      this.getCurrentSubjectAreasAndKeywords(),
      value.filter(p => this.metadataTagsDic[p].groupCode !== MetadataTagGroupCode.SERVICE_SCHEMES)
    );
    this._subjectAreasAndKeywords = value;
  }
  public setDevelopmentalRoles(value: string[]): void {
    this.setMultipleResourceTag(
      this.getCurrentResourceDevelopmentalRoles(),
      value.filter(p => this.metadataTagsDic[p].groupCode !== MetadataTagGroupCode.SERVICE_SCHEMES)
    );
    this._developmentalRoles = value;
  }

  public getCurrentMetadataCourseLevels(): MetadataTagModel[] {
    const courseLevels = this.metadataTagsDicByGroupCode[MetadataTagGroupCode.COURSE_LEVELS];
    if (courseLevels === undefined) {
      return [];
    }
    const courseLevelsDict = Utils.toDictionary(courseLevels, p => p.displayText);
    return this.orderedCourseLevelDislayTexts.map(p => courseLevelsDict[p]);
  }

  private buildDevelopmentalRolesTree(): MetadataTagModel[] {
    if (this.serviceSchemes.length === 0) {
      return [];
    }
    const serviceSchemesDic = Utils.toDictionary(this.serviceSchemes, p => p);
    return MetadataTagModel.buildTree(this.metadataTags, p => serviceSchemesDic[p.tagId] != null, [
      p => p.groupCode === MetadataTagGroupCode.DEVROLES
    ]).filter(p => p.childs != null && p.childs.length > 0);
  }

  private buildSubjectAreasAndKeywordsTree(): MetadataTagModel[] {
    if (this.serviceSchemes.length === 0) {
      return [];
    }
    const serviceSchemesDic = Utils.toDictionary(this.serviceSchemes, p => p);
    return MetadataTagModel.buildTree(this.metadataTags, p => serviceSchemesDic[p.tagId] != null, [
      p => p.groupCode === MetadataTagGroupCode.PDO_TAXONOMY
    ]).filter(p => p.childs != null && p.childs.length > 0);
  }

  private buildDimensionsAndAreasTree(): MetadataTagModel[] {
    if (this.learningFrameworks.length === 0) {
      return [];
    }
    const learningFrameworksDic = Utils.toDictionary(this.learningFrameworks, p => p);
    return MetadataTagModel.buildTree(this.metadataTags, p => learningFrameworksDic[p.tagId] != null).filter(
      p => p.childs != null && p.childs.length > 0
    );
  }

  private buildLearningFrameworksSelectItems(): MetadataTagModel[] {
    if (this.metadataTagsDicByGroupCode[MetadataTagGroupCode.LEARNING_FXS] == null) {
      return [];
    }
    const serviceSchemesDic = Utils.toDictionary(this.serviceSchemes, p => p);
    return this.metadataTagsDicByGroupCode[MetadataTagGroupCode.LEARNING_FXS].filter(p => serviceSchemesDic[p.parentTagId] != null);
  }

  private buildMetadataTagsDicByGroupCode(metadataTags: MetadataTagModel[]): Dictionary<MetadataTagModel[]> {
    return Utils.toDictionaryGroupBy(metadataTags.filter(p => p.groupCode != null), p => p.groupCode);
  }

  private buildMetadataTagsDicByParentTagId(metadataTags: MetadataTagModel[]): Dictionary<MetadataTagModel[]> {
    return Utils.toDictionaryGroupBy(metadataTags.filter(p => p.parentTagId != null), p => p.parentTagId);
  }

  private buildResourceMetadataTagsDicByGroupCode(): Dictionary<MetadataTagModel[]> {
    return this.buildMetadataTagsDicByGroupCode(this.resource.tags.map(tagId => this.metadataTagsDic[tagId]));
  }

  private setSingleResourceTag(currentValue: string | undefined, newValue: string | undefined): void {
    if (currentValue == null) {
      if (newValue != null && !this.resource.tags.includes(newValue)) {
        this.resource.tags.push(newValue);
        this.resourceMetadataTagsDicByGroupCode = this.buildResourceMetadataTagsDicByGroupCode();
      }
    } else {
      if (newValue != null) {
        if (!this.resource.tags.includes(newValue)) {
          this.resource.tags = Utils.replaceOne(this.resource.tags, newValue, p => p === currentValue);
        }
      } else {
        Utils.remove(this.resource.tags, p => p === currentValue);
      }
      this.resourceMetadataTagsDicByGroupCode = this.buildResourceMetadataTagsDicByGroupCode();
    }
  }

  private setMultipleResourceTag(currentValue: string[], newValue: string[]): void {
    const currentValueDic = Utils.toDictionary(currentValue, p => p);
    const newValueDic = Utils.toDictionary(newValue, p => p);
    const tagsToRemove = currentValue.filter(p => newValueDic[p] == null);
    const tagsToInsert = newValue.filter(p => currentValueDic[p] == null);

    if (tagsToInsert.length > 0) {
      this.resource.tags = this.resource.tags.concat(tagsToInsert);
      this.resourceMetadataTagsDicByGroupCode = this.buildResourceMetadataTagsDicByGroupCode();
    }
    if (tagsToRemove.length > 0) {
      const tagsToRemoveDic = Utils.toDictionary(tagsToRemove, p => p);
      Utils.remove(this.resource.tags, p => tagsToRemoveDic[p] != null);
      this.resourceMetadataTagsDicByGroupCode = this.buildResourceMetadataTagsDicByGroupCode();
    }
  }

  private buildDropdownSubjectAreaTagItems(): MetadataTagModel[] {
    if (this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TAXONOMY] == null) {
      return [];
    }
    return this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TAXONOMY].filter(p => this.serviceSchemes.includes(p.parentTagId));
  }

  private getCurrentValidSubjectAreasAndKeywords(): string[] {
    return MetadataTagModel.filterIdsInTree(
      this.resource.tags.filter(p => this.metadataTagsDic[p].groupCode !== MetadataTagGroupCode.SERVICE_SCHEMES),
      this.subjectAreasAndKeywordsTree
    );
  }

  private getCurrentValidLearningFrameworks(): string[] {
    const learningFrameworksSelectItemsDic = Utils.toDictionary(this.learningFrameworksSelectItems, p => p.tagId);
    return this.resource.tags.filter(p => learningFrameworksSelectItemsDic[p] != null);
  }

  private getCurrentValidDimensionsAndAreas(): string[] {
    return MetadataTagModel.filterIdsInTree(
      this.resource.tags.filter(p => this.metadataTagsDic[p].groupCode !== MetadataTagGroupCode.LEARNING_FXS),
      this.dimensionsAndAreasTree
    );
  }

  private getCurrentValidDevelopmentalRoles(): string[] {
    return MetadataTagModel.filterIdsInTree(
      this.resource.tags.filter(p => this.metadataTagsDic[p].groupCode !== MetadataTagGroupCode.SERVICE_SCHEMES),
      this.developmentalRolesTree
    );
  }

  private getCurrentResourceServiceSchemes(): string[] {
    const serviceSchemes = this.resourceMetadataTagsDicByGroupCode[MetadataTagGroupCode.SERVICE_SCHEMES];
    if (serviceSchemes == null) {
      return [];
    }
    return serviceSchemes.map(p => p.tagId);
  }

  private getCurrentResourceCourseLevels(): string[] {
    const courseLevels = this.resourceMetadataTagsDicByGroupCode[MetadataTagGroupCode.COURSE_LEVELS];
    if (courseLevels == null) {
      return [];
    }
    return courseLevels.map(p => p.tagId);
  }

  private getCurrentResourceDevelopmentalRoles(includeParent: boolean = false): string[] {
    const devRoles = this.resourceMetadataTagsDicByGroupCode[MetadataTagGroupCode.DEVROLES];
    if (devRoles == null) {
      return [];
    }
    if (includeParent) {
      const devRoleIds = devRoles.map(p => p.tagId);
      const parentIds: string[] = [];
      const tagsTree = this.developmentalRolesTree.length === 0 ? this.buildDevelopmentalRolesTree() : this.developmentalRolesTree;
      tagsTree.forEach(element => {
        if (
          element.childs.every(child => {
            return devRoleIds.indexOf(child.id) !== -1;
          })
        ) {
          parentIds.push(element.id);
        }
      });
      return devRoleIds.concat(parentIds);
    } else {
      return devRoles.map(p => p.tagId);
    }
  }

  private getCurrentSubjectAreasAndKeywords(includeParent: boolean = false): string[] {
    const firstLevelTags = this.metadataTagsDicByGroupCode[MetadataTagGroupCode.SERVICE_SCHEMES]
      ? this.metadataTagsDicByGroupCode[MetadataTagGroupCode.SERVICE_SCHEMES]
      : [];
    const tags = this.resource.tags.map(_ => this.metadataTagsDic[_]);
    let subjectAreasAndKeywords = [];
    const remainingTags = tags.filter(p => p.groupCode !== MetadataTagGroupCode.SERVICE_SCHEMES);
    firstLevelTags.forEach(tag => {
      let childTags = this.metadataTagsDicByParentTagId[tag.id] ? this.metadataTagsDicByParentTagId[tag.id] : [];
      childTags = childTags.filter(childTag => childTag.groupCode === MetadataTagGroupCode.PDO_TAXONOMY);
      childTags.forEach(childTag => {
        if (remainingTags.map(remainTag => remainTag.id).includes(childTag.id)) {
          subjectAreasAndKeywords.push(childTag);
        }
        subjectAreasAndKeywords = subjectAreasAndKeywords.concat(
          MetadataTagModel.getChildsTagRecursively(childTag, remainingTags, this.metadataTagsDicByParentTagId)
        );
      });
    });
    if (includeParent) {
      const subjectAreasAndKeywordIds = subjectAreasAndKeywords.map(p => p.tagId);
      const parentIds: string[] = [];
      const tagsTree =
        this.subjectAreasAndKeywordsTree.length === 0 ? this.buildSubjectAreasAndKeywordsTree() : this.subjectAreasAndKeywordsTree;
      tagsTree.forEach(element => {
        if (
          element.childs.every(child => {
            return subjectAreasAndKeywordIds.indexOf(child.id) !== -1;
          })
        ) {
          parentIds.push(element.id);
        }
      });
      return subjectAreasAndKeywordIds.concat(parentIds);
    } else {
      return subjectAreasAndKeywords.map(p => p.tagId);
    }
  }

  private getCurrentResourceLearningFrameworks(): string[] {
    const learningFrameworks = this.resourceMetadataTagsDicByGroupCode[MetadataTagGroupCode.LEARNING_FXS];
    if (learningFrameworks == null) {
      return [];
    }
    return learningFrameworks.map(p => p.tagId);
  }

  private getCurrentDimensionsAndAreas(includeParent: boolean = false): string[] {
    const dimensionsAndAreas = MetadataTagModel.filterTagWithChildsByTopLevelGroupCode(
      this.resource.tags.map(_ => this.metadataTagsDic[_]),
      MetadataTagGroupCode.LEARNING_FXS,
      this.metadataTagsDicByParentTagId
    ).filter(p => p.groupCode !== MetadataTagGroupCode.LEARNING_FXS);
    if (includeParent) {
      const dimensionsAndAreaIds = dimensionsAndAreas.map(p => p.tagId);
      const parentIds: string[] = [];
      const tagsTree = this.dimensionsAndAreasTree.length === 0 ? this.buildDimensionsAndAreasTree() : this.dimensionsAndAreasTree;
      tagsTree.forEach(element => {
        if (
          element.childs.every(child => {
            return dimensionsAndAreaIds.indexOf(child.id) !== -1;
          })
        ) {
          parentIds.push(element.id);
        }
      });
      return dimensionsAndAreaIds.concat(parentIds);
    } else {
      return dimensionsAndAreas.map(p => p.tagId);
    }
  }
}
