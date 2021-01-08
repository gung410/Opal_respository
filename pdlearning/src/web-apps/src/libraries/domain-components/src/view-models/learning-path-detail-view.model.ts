import {
  Course,
  LearningPathCourseModel,
  LearningPathModel,
  LearningPathStatus,
  MetadataTagGroupCode,
  MetadataTagModel
} from '@opal20/domain-api';
import { MAX_INT, Utils } from '@opal20/infrastructure';
import { Observable, of } from 'rxjs';

import { LearningPathCourseViewModel } from './learning-path-course-view.model';
export interface ILearningPathDetailViewModel {
  originLearningPath: LearningPathModel;
  learningPath: LearningPathModel;
  bookmarkNumber: number;
  learningPathCourseVMs: LearningPathCourseViewModel[];
  allowedThumbnailExtensions: string[];
  coursesDict: Dictionary<Course>;
}

export class LearningPathDetailViewModel implements ILearningPathDetailViewModel {
  //#region Metadata constant for filter
  public static readonly ORDERED_LEARNINGPATHS_LEVELS: Dictionary<number> = {
    ['Emergent']: 1,
    ['Proficient']: 2,
    ['Accomplished']: 3,
    ['Leading']: 4,
    ['Not Applicable']: 5
  };
  //#endregion

  public bookmarkNumber: number = 0;
  public allowedThumbnailExtensions: string[] = ['jpeg', 'jpg', 'gif', 'png', 'svg'];
  public coursesDict: Dictionary<Course> = {};
  public originLearningPath: LearningPathModel = new LearningPathModel();
  public learningPath: LearningPathModel = new LearningPathModel();
  public learningPathCourseVMs: LearningPathCourseViewModel[] = [];

  //#region Metadata dropdown items
  public metadataTagsDic: Dictionary<MetadataTagModel> = {};
  public metadataTagsDicByGroupCode: Dictionary<MetadataTagModel[]> = {};
  public pdTypeSelectItems: MetadataTagModel[] = [];
  public categorySelectItems: MetadataTagModel[] = [];
  public modeOfLearningSelectItems: MetadataTagModel[] = [];
  public courseLevelSelectItems: MetadataTagModel[] = [];
  public serviceSchemeSelectItems: MetadataTagModel[] = [];
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
  private _pdAreaThemeSelectItems: MetadataTagModel[] = [];
  public get pdAreaThemeSelectItems(): MetadataTagModel[] {
    return this._pdAreaThemeSelectItems;
  }
  public set pdAreaThemeSelectItems(v: MetadataTagModel[]) {
    this._pdAreaThemeSelectItems = v;
    this.pdAreaThemeIds = Utils.rightJoin(this.pdAreaThemeIds, this.pdAreaThemeSelectItems.map(p => p.tagId));
  }
  //#endregion

  constructor(
    data?: LearningPathModel,
    learningPathCourses: Course[] = [],
    bookmarkNumber: number = 0,
    public metadataTags: MetadataTagModel[] = []
  ) {
    this.bookmarkNumber = bookmarkNumber;
    this.coursesDict = this.createCoursesDict(learningPathCourses);
    this.createOrUpdateModel(data);
    this.setMetadataTagsDicInfo(metadataTags);

    //#region Metadata dropdown items
    this.pdTypeSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TYPES], []);
    this.categorySelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_CATEGORIES], []);
    this.modeOfLearningSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_MODES], []);
    this.courseLevelSelectItems = Utils.orderBy(
      Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.COURSE_LEVELS], []),
      p => LearningPathDetailViewModel.ORDERED_LEARNINGPATHS_LEVELS[p.displayText] || MAX_INT
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

    if (!this.metadataTags.length) {
      return;
    }

    this._subjectAreaTree = this.buildSubjectAreaTree();
    this._subjectAreaTreeValues = this.buildSubjectAreaTreeValues();
    this.selectedSubjectAreaTree = this.buildSelectedSubjectAreaTree();

    this._learningFrameworkTree = this.buildLearningFrameworkTree();
    this._learningFrameworkTreeValues = this.buildLearningFrameworkTreeValues();
    this.selectedLearningFrameworkTree = this.buildSelectedLearningFrameworkTree();

    this._learningDimensionAreaTree = this.buildLearningDimensionAreaTree();
    this._learningDimensionAreaTreeValues = this.buildLearningDimensionAreaTreeValues();
    this.selectedLearningDimensionAreaTree = this.buildSelectedLearningDimensionAreaTree();

    //#endregion
  }

  public get id(): string {
    return this.learningPath.id;
  }

  public set id(id: string) {
    this.learningPath.id = id;
  }

  public get title(): string {
    return this.learningPath.title;
  }

  public set title(title: string) {
    this.learningPath.title = title;
  }

  public get thumbnailUrl(): string {
    return this.learningPath.thumbnailUrl;
  }

  public set thumbnailUrl(thumbnailUrl: string) {
    this.learningPath.thumbnailUrl = thumbnailUrl;
  }

  public get createdDate(): Date {
    return this.learningPath.createdDate;
  }

  public set createdDate(createdDate: Date) {
    this.learningPath.createdDate = createdDate;
  }

  public get createdBy(): string {
    return this.learningPath.createdBy;
  }

  public set createdBy(createdBy: string) {
    this.learningPath.createdBy = createdBy;
  }

  public get listCourses(): LearningPathCourseModel[] {
    return this.learningPath.listCourses;
  }

  public set listCourses(listCourses: LearningPathCourseModel[]) {
    this.learningPath.listCourses = listCourses;
    this.processListCourses();
  }

  public get status(): LearningPathStatus {
    return this.learningPath.status;
  }

  public set status(status: LearningPathStatus) {
    this.learningPath.status = status;
  }

  //#region Metadata
  public get courseLevelIds(): string[] {
    return this.learningPath.courseLevelIds;
  }
  public set courseLevelIds(courseLevelIds: string[]) {
    this.learningPath.courseLevelIds = courseLevelIds;
  }

  public get serviceSchemeIds(): string[] {
    return this.learningPath.serviceSchemeIds;
  }
  public set serviceSchemeIds(serviceSchemeIds: string[]) {
    this.learningPath.serviceSchemeIds = serviceSchemeIds;
    this.subjectAreaTree = this.buildSubjectAreaTree();
    this.learningFrameworkTree = this.buildLearningFrameworkTree();
  }

  public get subjectAreaIds(): string[] {
    return this.learningPath.subjectAreaIds;
  }
  public set subjectAreaIds(subjectAreaIds: string[]) {
    this.learningPath.subjectAreaIds = subjectAreaIds;
    this.pdAreaThemeSelectItems = this.subjectAreaIds.map(p => this.metadataTagsDic[p]).filter(p => p != null && p.codingScheme != null);
  }

  public get pdAreaThemeIds(): string[] {
    return this.learningPath.pdAreaThemeIds;
  }
  public set pdAreaThemeIds(pdAreaThemeIds: string[]) {
    this.learningPath.pdAreaThemeIds = pdAreaThemeIds;
  }

  public get learningFrameworkIds(): string[] {
    return this.learningPath.learningFrameworkIds;
  }
  public set learningFrameworkIds(learningFrameworkIds: string[]) {
    this.learningPath.learningFrameworkIds = learningFrameworkIds;
    this.learningDimensionAreaTree = this.buildLearningDimensionAreaTree();
  }

  public get learningDimensionIds(): string[] {
    return this.learningPath.learningDimensionIds;
  }
  public set learningDimensionIds(learningDimensionIds: string[]) {
    this.learningPath.learningDimensionIds = learningDimensionIds;
  }

  public get learningAreaIds(): string[] {
    return this.learningPath.learningAreaIds;
  }
  public set learningAreaIds(learningAreaIds: string[]) {
    this.learningPath.learningAreaIds = learningAreaIds;
  }

  public get learningSubAreaIds(): string[] {
    return this.learningPath.learningSubAreaIds;
  }
  public set learningSubAreaIds(learningSubAreaIds: string[]) {
    this.learningPath.learningSubAreaIds = learningSubAreaIds;
  }

  public get learningDimensionAreas(): string[] {
    const learningAreas = this.learningAreaIds.concat(this.learningSubAreaIds);
    return this.learningDimensionIds.concat(learningAreas);
  }

  public get metadataKeys(): string[] {
    return this.learningPath.metadataKeys;
  }

  public set metadataKeys(metadataKeys: string[]) {
    this.learningPath.metadataKeys = metadataKeys;
  }
  //#endregion
  public getMetadataTagChilds(item: MetadataTagModel): Observable<MetadataTagModel[]> {
    if (item.childs == null) {
      return of([]);
    }
    return of(item.childs);
  }

  public isMetadataTagHasChilds(item: MetadataTagModel): boolean {
    return item.childs != null && item.childs.length > 0;
  }

  public createOrUpdateModel(learningPath?: LearningPathModel): void {
    let usedLearningPath = new LearningPathModel();
    if (learningPath) {
      usedLearningPath = learningPath;
    }
    this.originLearningPath = Utils.cloneDeep(usedLearningPath);
    this.learningPath = Utils.cloneDeep(usedLearningPath);
    this.processListCourses();
  }

  public reset(): void {
    this.learningPath = Utils.cloneDeep(this.originLearningPath);
    this.processListCourses();
  }

  public addToListCourses(learningPathCourse: LearningPathCourseViewModel): void {
    if (this.listCourses.findIndex(x => x.courseId === learningPathCourse.learningPathCourse.courseId) > -1) {
      return;
    }
    this.learningPath.listCourses.push(learningPathCourse.learningPathCourse);
    if (this.coursesDict[learningPathCourse.learningPathCourse.courseId] == null) {
      this.coursesDict[learningPathCourse.learningPathCourse.courseId] = learningPathCourse.course;
    }
    this.processListCourses();
  }

  public changeCourseItemOrder(currentOrder: number, direction: 'up' | 'down'): void {
    const currentItem = this.listCourses.find(ele => ele.order === currentOrder);
    if (currentItem == null) {
      return;
    }
    if (direction === 'up') {
      const beforeItem = this.listCourses.find(ele => ele.order === currentOrder - 1);
      if (beforeItem != null) {
        beforeItem.order++;
        currentItem.order--;
      }
    } else {
      const afterItem = this.listCourses.find(ele => ele.order === currentOrder + 1);
      if (afterItem != null) {
        afterItem.order--;
        currentItem.order++;
      }
    }
    this.processListCourses();
  }

  public removeFromListCourses(learningPathCourse: LearningPathCourseViewModel): void {
    const currentOrder = learningPathCourse.learningPathCourse.order;
    this.learningPath.listCourses = this.listCourses.filter(ele => ele.order !== currentOrder);
    this.listCourses.forEach(ele => {
      if (ele.order > currentOrder) {
        ele.order--;
      }
    });
    this.processListCourses();
  }

  public dataHasChanged(): boolean {
    return Utils.isDifferent(this.originLearningPath, this.learningPath);
  }

  public buildCourseLevelSelectItems(courseLevels: MetadataTagModel[], orderedCourseLevelDislayTexts: string[]): MetadataTagModel[] {
    if (courseLevels === undefined) {
      return [];
    }
    const courseLevelsDict = Utils.toDictionary(courseLevels, p => p.displayText);
    return orderedCourseLevelDislayTexts.map(p => courseLevelsDict[p]);
  }

  public populateMetadata(): void {
    this.serviceSchemeIds = Utils.distinct(
      Utils.flatMap(this.learningPath.listCourses.map(p => this.coursesDict[p.courseId].serviceSchemeIds).concat(this.serviceSchemeIds))
    );
    this.subjectAreaIds = Utils.distinct(
      Utils.flatMap(this.learningPath.listCourses.map(p => this.coursesDict[p.courseId].subjectAreaIds).concat(this.subjectAreaIds))
    );
    this.pdAreaThemeIds = Utils.distinct(
      Utils.flatMap(this.learningPath.listCourses.map(p => [this.coursesDict[p.courseId].pdAreaThemeId]).concat(this.pdAreaThemeIds))
    );
    this.learningFrameworkIds = Utils.distinct(
      Utils.flatMap(
        this.learningPath.listCourses.map(p => this.coursesDict[p.courseId].learningFrameworkIds).concat(this.learningFrameworkIds)
      )
    );
    this.learningDimensionIds = Utils.distinct(
      Utils.flatMap(
        this.learningPath.listCourses.map(p => this.coursesDict[p.courseId].learningDimensionIds).concat(this.learningDimensionIds)
      )
    );
    this.learningAreaIds = Utils.distinct(
      Utils.flatMap(this.learningPath.listCourses.map(p => this.coursesDict[p.courseId].learningAreaIds).concat(this.learningAreaIds))
    );
    this.learningSubAreaIds = Utils.distinct(
      Utils.flatMap(this.learningPath.listCourses.map(p => this.coursesDict[p.courseId].learningSubAreaIds).concat(this.learningSubAreaIds))
    );
    this.courseLevelIds = Utils.distinct(
      Utils.flatMap(this.learningPath.listCourses.map(p => [this.coursesDict[p.courseId].courseLevel]).concat(this.courseLevelIds))
    );
  }

  private processListCourses(): void {
    this.learningPath.listCourses = Utils.orderBy(this.listCourses, p => p.order, false);
    this.learningPathCourseVMs = this.listCourses.map(a => new LearningPathCourseViewModel(a, this.coursesDict[a.courseId]));
  }

  private createCoursesDict(learningPathCourses: Course[]): Dictionary<Course> {
    const distinctCourses = Utils.distinctBy([...learningPathCourses], p => p.id);
    return Utils.toDictionary(distinctCourses, p => p.id);
  }

  //#region Metadata
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

  private setMetadataTagsDicInfo(metadataTags: MetadataTagModel[]): void {
    this.metadataTagsDic = Utils.toDictionary(metadataTags, p => p.tagId);
    this.metadataTagsDicByGroupCode = Utils.toDictionaryGroupBy(
      metadataTags.filter(p => p.groupCode != null),
      p => p.groupCode,
      items => Utils.orderBy(items, p => p.displayText)
    );
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
    return MetadataTagModel.buildTree(this.metadataTags, p => learningFrameworkDic[p.tagId] != null).filter(
      p => p.childs != null && p.childs.length > 0
    );
  }

  private buildSelectedLearningDimensionAreaTree(): MetadataTagModel[] {
    return MetadataTagModel.filterSelectedTreeNodes(this.learningDimensionAreaTree, this.learningDimensionAreas);
  }

  private buildLearningDimensionAreaTreeValues(): string[] {
    return Utils.clone(this.learningDimensionAreas);
  }
  //#endregion
}
