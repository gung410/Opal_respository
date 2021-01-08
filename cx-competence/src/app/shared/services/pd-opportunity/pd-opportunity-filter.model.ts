import { CxSelectItemModel } from 'app/shared/components/cx-select/cx-select.model';
import { Dictionary, keyBy, orderBy } from 'lodash';
import { MetadataTagModel } from './metadata-tag.model';

export class PDOpportunityFilterModel {
  public get serviceSchemeSelectItems(): MetadataTagModel[] {
    return this._serviceSchemeSelectItems;
  }
  public set serviceSchemeSelectItems(v: MetadataTagModel[]) {
    // v.map((item) => {
    //   const serviceSchemeWithSubject = this.buildSubjectSelectItems([item]);
    //   item.childs = serviceSchemeWithSubject;

    //   return item;
    // });
    this._serviceSchemeSelectItems = v;
  }
  public get subjectSelectItems(): MetadataTagModel[] {
    return this._subjectSelectItems;
  }
  public set subjectSelectItems(v: MetadataTagModel[]) {
    this._subjectSelectItems = v;
    this.subjectAreaSelectedItems = this.rightJoin(
      this.subjectAreaSelectedItems,
      this.subjectSelectItems
    );
  }
  public get learningFrameworkSelectItems(): MetadataTagModel[] {
    return this._learningFrameworksSelectItems;
  }
  public set learningFrameworkSelectItems(v: MetadataTagModel[]) {
    this._learningFrameworksSelectItems = v;
    this.learningDimensionSelectItems = this.buildLearningDimensionSelectItems();
    this.learningFrameworkSelectedItems = this.rightJoin(
      this.learningFrameworkSelectedItems,
      this.learningFrameworkSelectItems
    );
  }
  public get learningDimensionSelectItems(): MetadataTagModel[] {
    return this._learningDimensionSelectItems;
  }
  public set learningDimensionSelectItems(v: MetadataTagModel[]) {
    this._learningDimensionSelectItems = v;
    this.learningAreaSelectItems = this.buildLearningAreaSelectItems();
    this.learningDimensionSelectedItems = this.rightJoin(
      this.learningDimensionSelectedItems,
      this.learningDimensionSelectItems
    );
  }
  public get learningAreaSelectItems(): MetadataTagModel[] {
    return this._learningAreaSelectItems;
  }
  public set learningAreaSelectItems(v: MetadataTagModel[]) {
    this._learningAreaSelectItems = v;
    this.learningSubAreaSelectItems = this.buildLearningSubAreaSelectItems();
    this.learningAreaSelectedItems = this.rightJoin(
      this.learningAreaSelectedItems,
      this.learningAreaSelectItems
    );
  }
  public get learningSubAreaSelectItems(): MetadataTagModel[] {
    return this._learningSubAreaSelectItems;
  }
  public set learningSubAreaSelectItems(v: MetadataTagModel[]) {
    this._learningSubAreaSelectItems = v;
    this.learningSubAreaSelectedItems = this.rightJoin(
      this.learningSubAreaSelectedItems,
      this.learningSubAreaSelectItems
    );
  }
  public get natureCourseSelectItems(): MetadataTagModel[] {
    return this._natureCourseSelectItems;
  }
  public set natureCourseSelectItems(v: MetadataTagModel[]) {
    this._natureCourseSelectItems = v;
    this.natureOfCourse = this.rightJoinSingleBy(
      this.natureOfCourse,
      this.natureCourseSelectItems,
      (p) => p.id,
      (p) => p.id
    );
  }
  public get developmentalRoleSelectItems(): MetadataTagModel[] {
    return this._developmentalRoleSelectItems;
  }
  public set developmentalRoleSelectItems(v: MetadataTagModel[]) {
    this._developmentalRoleSelectItems = v;
    this.developmentalRoleSelectedItems = this.rightJoin(
      this.developmentalRoleSelectedItems,
      this.developmentalRoleSelectItems
    );
  }
  public get teachingLevelsSelectItems(): MetadataTagModel[] {
    return this._teachingLevelsSelectItems;
  }
  public set teachingLevelsSelectItems(v: MetadataTagModel[]) {
    this._teachingLevelsSelectItems = v;
    this.teachingLevelSelectedItems = this.rightJoin(
      this.teachingLevelSelectedItems,
      this.teachingLevelsSelectItems
    );
  }
  public get teachingSubjectSelectItems(): MetadataTagModel[] {
    return this._teachingSubjectSelectItems;
  }
  public set teachingSubjectSelectItems(v: MetadataTagModel[]) {
    this._teachingSubjectSelectItems = v;
    this.teachingSubjectSelectedItems = this.rightJoin(
      this.teachingSubjectSelectedItems,
      this.teachingSubjectSelectItems
    );
  }
  public get serviceSchemeSelectedItems(): CxSelectItemModel<
    MetadataTagModel
  >[] {
    return this._serviceSchemeSelectedItems;
  }
  public set serviceSchemeSelectedItems(
    v: CxSelectItemModel<MetadataTagModel>[]
  ) {
    this._serviceSchemeSelectedItems = v;
    this.subjectSelectItems = this.buildSubjectSelectItems(
      this._serviceSchemeSelectedItems
    );
    this.learningFrameworkSelectItems = this.buildLearningFrameworksSelectItems();
    this.developmentalRoleSelectItems = this.buildDevelopmentalRoleSelectItems();
  }
  public get subjectAreaSelectedItems(): CxSelectItemModel<MetadataTagModel>[] {
    return this._subjectAreaSelectedItems;
  }
  public set subjectAreaSelectedItems(
    v: CxSelectItemModel<MetadataTagModel>[]
  ) {
    this._subjectAreaSelectedItems = v;
  }
  public get learningFrameworkSelectedItems(): CxSelectItemModel<
    MetadataTagModel
  >[] {
    return this._learningFrameworkSelectedItems;
  }
  public set learningFrameworkSelectedItems(
    v: CxSelectItemModel<MetadataTagModel>[]
  ) {
    this._learningFrameworkSelectedItems = v;
    this.learningDimensionSelectItems = this.buildLearningDimensionSelectItems();
  }
  public get learningDimensionSelectedItems(): CxSelectItemModel<
    MetadataTagModel
  >[] {
    return this._learningDimensionSelectedItems;
  }
  public set learningDimensionSelectedItems(
    v: CxSelectItemModel<MetadataTagModel>[]
  ) {
    this._learningDimensionSelectedItems = v;
    this.learningAreaSelectItems = this.buildLearningAreaSelectItems();
  }
  public get learningAreaSelectedItems(): CxSelectItemModel<
    MetadataTagModel
  >[] {
    return this._learningAreaSelectedItems;
  }
  public set learningAreaSelectedItems(
    v: CxSelectItemModel<MetadataTagModel>[]
  ) {
    this._learningAreaSelectedItems = v;
    this.learningSubAreaSelectItems = this.buildLearningSubAreaSelectItems();
  }
  public get learningSubAreaSelectedItems(): CxSelectItemModel<
    MetadataTagModel
  >[] {
    return this._learningSubAreaSelectedItems;
  }
  public set learningSubAreaSelectedItems(
    v: CxSelectItemModel<MetadataTagModel>[]
  ) {
    this._learningSubAreaSelectedItems = v;
  }
  //#region Metadata dropdown items
  public metadataTagsDic: Dictionary<MetadataTagModel> = {};
  public metadataTagsDicByGroupCode: Dictionary<MetadataTagModel[]> = {};
  public pdTypeSelectItems: MetadataTagModel[] = [];
  public categorySelectItems: MetadataTagModel[] = [];
  public modeOfLearningSelectItems: MetadataTagModel[] = [];
  public courseLevelSelectItems: MetadataTagModel[] = [];

  protected _serviceSchemeSelectItems: MetadataTagModel[] = [];

  protected _subjectSelectItems: MetadataTagModel[] = [];

  protected _learningFrameworksSelectItems: MetadataTagModel[] = [];
  protected _learningDimensionSelectItems: MetadataTagModel[] = [];
  protected _learningAreaSelectItems: MetadataTagModel[] = [];
  protected _learningSubAreaSelectItems: MetadataTagModel[] = [];

  protected _natureCourseSelectItems: MetadataTagModel[] = [];

  protected _developmentalRoleSelectItems: MetadataTagModel[] = [];
  protected _teachingLevelsSelectItems: MetadataTagModel[] = [];
  protected _teachingSubjectSelectItems: MetadataTagModel[] = [];

  protected _serviceSchemeSelectedItems: CxSelectItemModel<
    MetadataTagModel
  >[] = [];

  protected _subjectAreaSelectedItems: CxSelectItemModel<
    MetadataTagModel
  >[] = [];

  protected _learningFrameworkSelectedItems: CxSelectItemModel<
    MetadataTagModel
  >[] = [];

  protected _learningDimensionSelectedItems: CxSelectItemModel<
    MetadataTagModel
  >[] = [];

  protected _learningAreaSelectedItems: CxSelectItemModel<
    MetadataTagModel
  >[] = [];

  protected _learningSubAreaSelectedItems: CxSelectItemModel<
    MetadataTagModel
  >[] = [];

  protected _natureOfCourse: CxSelectItemModel<MetadataTagModel>;

  protected _developmentalRoleSelectedItems: CxSelectItemModel<
    MetadataTagModel
  >[] = [];

  protected _teachingLevelSelectedItems: CxSelectItemModel<
    MetadataTagModel
  >[] = [];

  protected _teachingSubjectSelectedItems: CxSelectItemModel<
    MetadataTagModel
  >[] = [];

  protected _pdActivityType: CxSelectItemModel<MetadataTagModel>;
  public get pdActivityType(): CxSelectItemModel<MetadataTagModel> {
    return this._pdActivityType;
  }
  public set pdActivityType(
    pdActivityType: CxSelectItemModel<MetadataTagModel>
  ) {
    this._pdActivityType = pdActivityType;
  }

  protected _categorySelectedItems: CxSelectItemModel<MetadataTagModel>[] = [];
  public get categorySelectedItems(): CxSelectItemModel<MetadataTagModel>[] {
    return this._categorySelectedItems;
  }
  public set categorySelectedItems(v: CxSelectItemModel<MetadataTagModel>[]) {
    this._categorySelectedItems = v;
  }

  protected _learningMode: CxSelectItemModel<MetadataTagModel>;
  public get learningMode(): CxSelectItemModel<MetadataTagModel> {
    return this._learningMode;
  }
  public set learningMode(learningMode: CxSelectItemModel<MetadataTagModel>) {
    this._learningMode = learningMode;
  }

  protected _courseLevel: CxSelectItemModel<MetadataTagModel>;
  public get courseLevel(): CxSelectItemModel<MetadataTagModel> {
    return this._courseLevel;
  }
  public set courseLevel(courseLevel: CxSelectItemModel<MetadataTagModel>) {
    this._courseLevel = courseLevel;
  }

  public get natureOfCourse(): CxSelectItemModel<MetadataTagModel> {
    return this._natureOfCourse;
  }

  public set natureOfCourse(
    natureOfCourse: CxSelectItemModel<MetadataTagModel>
  ) {
    this._natureOfCourse = natureOfCourse;
  }

  public get developmentalRoleSelectedItems(): CxSelectItemModel<
    MetadataTagModel
  >[] {
    return this._developmentalRoleSelectedItems;
  }
  public set developmentalRoleSelectedItems(
    v: CxSelectItemModel<MetadataTagModel>[]
  ) {
    this._developmentalRoleSelectedItems = v;
  }

  public get teachingLevelSelectedItems(): CxSelectItemModel<
    MetadataTagModel
  >[] {
    return this._teachingLevelSelectedItems;
  }
  public set teachingLevelSelectedItems(
    teachingLevels: CxSelectItemModel<MetadataTagModel>[]
  ) {
    this._teachingLevelSelectedItems = teachingLevels;
  }

  public get teachingSubjectSelectedItems(): CxSelectItemModel<
    MetadataTagModel
  >[] {
    return this._teachingSubjectSelectedItems;
  }

  public set teachingSubjectSelectedItems(
    v: CxSelectItemModel<MetadataTagModel>[]
  ) {
    this._teachingSubjectSelectedItems = v;
  }

  constructor(public metadataTags: MetadataTagModel[] = []) {
    if (!metadataTags) {
      return;
    }

    if (metadataTags) {
      this.setMetadataTagsDicInfo(metadataTags);
      this.pdTypeSelectItems = this.defaultIfNull(
        this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TYPES],
        []
      );
      this.categorySelectItems = this.defaultIfNull(
        this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_CATEGORIES],
        []
      );
      this.modeOfLearningSelectItems = this.defaultIfNull(
        this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_MODES],
        []
      );
      this.courseLevelSelectItems = this.defaultIfNull(
        this.metadataTagsDicByGroupCode[MetadataTagGroupCode.COURSE_LEVELS],
        []
      );
      this.serviceSchemeSelectItems = this.defaultIfNull(
        this.metadataTagsDicByGroupCode[MetadataTagGroupCode.SERVICE_SCHEMES],
        []
      );
      this._subjectSelectItems = this.buildSubjectSelectItems(
        this.serviceSchemeSelectedItems
      );
      this._learningFrameworksSelectItems = this.buildLearningFrameworksSelectItems();
      this._learningDimensionSelectItems = this.buildLearningDimensionSelectItems();
      this._learningAreaSelectItems = this.buildLearningAreaSelectItems();
      this._learningSubAreaSelectItems = this.buildLearningSubAreaSelectItems();
      this._natureCourseSelectItems = this.defaultIfNull(
        this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_NATURES],
        []
      );
      this._developmentalRoleSelectItems = this.buildDevelopmentalRoleSelectItems();
      this._teachingLevelsSelectItems = this.orderBy(
        this.defaultIfNull(
          this.metadataTagsDicByGroupCode[MetadataTagGroupCode.TEACHING_LEVELS],
          []
        ),
        (p) => p.name
      );
      this._teachingSubjectSelectItems = this.defaultIfNull(
        this.metadataTagsDicByGroupCode[MetadataTagGroupCode.TEACHING_SUBJECTS],
        []
      );

      this.removeUselessFillterData();
    }
  }

  public updateMetadataTags(metadataTags: MetadataTagModel[]): void {
    this.setMetadataTagsDicInfo(metadataTags);

    this.pdTypeSelectItems = this.pdTypeSelectItems
      .map((p) => this.metadataTagsDic[p.tagId])
      .filter((p) => p != null);
    this.categorySelectItems = this.categorySelectItems
      .map((p) => this.metadataTagsDic[p.tagId])
      .filter((p) => p != null);
    this.modeOfLearningSelectItems = this.modeOfLearningSelectItems
      .map((p) => this.metadataTagsDic[p.tagId])
      .filter((p) => p != null);
    this.courseLevelSelectItems = this.courseLevelSelectItems
      .map((p) => this.metadataTagsDic[p.tagId])
      .filter((p) => p != null);
    this.serviceSchemeSelectItems = this.serviceSchemeSelectItems
      .map((p) => this.metadataTagsDic[p.tagId])
      .filter((p) => p != null);

    this.subjectSelectItems = this.subjectSelectItems
      .map((p) => this.metadataTagsDic[p.tagId])
      .filter((p) => p != null);
    this.learningFrameworkSelectItems = this.learningFrameworkSelectItems
      .map((p) => this.metadataTagsDic[p.tagId])
      .filter((p) => p != null);
    this.learningDimensionSelectItems = this.learningDimensionSelectItems
      .map((p) => this.metadataTagsDic[p.tagId])
      .filter((p) => p != null);
    this.learningAreaSelectItems = this.learningAreaSelectItems
      .map((p) => this.metadataTagsDic[p.tagId])
      .filter((p) => p != null);
    this.learningSubAreaSelectItems = this.learningSubAreaSelectItems
      .map((p) => this.metadataTagsDic[p.tagId])
      .filter((p) => p != null);
  }

  private buildSubjectSelectItems(
    serviceSchemeSelectItems: CxSelectItemModel<MetadataTagModel>[]
  ): MetadataTagModel[] {
    return serviceSchemeSelectItems.length > 0
      ? this.rightJoinBy(
          this.defaultIfNull(
            this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TAXONOMY],
            []
          ).filter((p) => p.codingScheme != null),
          serviceSchemeSelectItems,
          (p) => p.parentTagId,
          (p) => p.id
        )
      : [];
  }

  private buildDevelopmentalRoleSelectItems(): MetadataTagModel[] {
    return this.orderBy(
      this.rightJoinBy(
        this.defaultIfNull(
          this.metadataTagsDicByGroupCode[MetadataTagGroupCode.DEVROLES],
          []
        ),
        this.serviceSchemeSelectedItems,
        (p) => p.parentTagId,
        (p) => p.id
      ),
      (p) => p.name
    );
  }

  private buildLearningFrameworksSelectItems(): MetadataTagModel[] {
    return this.rightJoinBy(
      this.defaultIfNull(
        this.metadataTagsDicByGroupCode[MetadataTagGroupCode.LEARNING_FXS],
        []
      ),
      this.serviceSchemeSelectedItems,
      (p) => p.parentTagId,
      (p) => p.id
    );
  }

  private buildLearningDimensionSelectItems(): MetadataTagModel[] {
    return this.rightJoinBy(
      this.metadataTags,
      this.learningFrameworkSelectedItems,
      (p) => p.parentTagId,
      (p) => p.id
    );
  }

  private buildLearningAreaSelectItems(): MetadataTagModel[] {
    return this.rightJoinBy(
      this.metadataTags,
      this.learningDimensionSelectedItems,
      (p) => p.parentTagId,
      (p) => p.id
    );
  }

  private buildLearningSubAreaSelectItems(): MetadataTagModel[] {
    return this.rightJoinBy(
      this.metadataTags,
      this.learningAreaSelectedItems,
      (p) => p.parentTagId,
      (p) => p.id
    );
  }

  private removeUselessFillterData(): void {
    if (this.serviceSchemeSelectItems.length > 0) {
      this.serviceSchemeSelectItems = this.serviceSchemeSelectItems.filter(
        (element) => {
          if (element.codingScheme && element.fullStatement) {
            return (
              element.codingScheme.toLocaleLowerCase() !== 'n/a' ||
              element.fullStatement.toLocaleLowerCase() !== 'not applicable'
            );
          }

          return true;
        }
      );
    }

    if (this.teachingLevelsSelectItems.length > 0) {
      this.teachingLevelsSelectItems = this.teachingLevelsSelectItems.filter(
        (element) => {
          if (element.codingScheme && element.fullStatement) {
            return (
              element.codingScheme.toLocaleLowerCase() !== 'n/a' ||
              element.fullStatement.toLocaleLowerCase() !== 'not applicable'
            );
          }

          return true;
        }
      );
    }
  }

  private setMetadataTagsDicInfo(metadataTags: MetadataTagModel[]): void {
    this.metadataTagsDic = this.toDictionary(metadataTags, (p) => p.tagId);
    this.metadataTagsDicByGroupCode = this.toDictionaryGroupBy(
      metadataTags.filter((p) => p.groupCode != null),
      (p) => p.groupCode
    );
  }

  //#region Utils
  private rightJoin(
    left: CxSelectItemModel<MetadataTagModel>[],
    right: MetadataTagModel[]
  ): CxSelectItemModel<MetadataTagModel>[] {
    const rightDic = this.toDictionary(right, (p) => p.id);

    return left ? left.filter((p) => rightDic[p.parentTagId] != null) : [];
  }

  private toDictionary<T>(
    collection: ArrayLike<T> | undefined,
    dictionaryKeySelector?: (item: T) => string | number
  ): Dictionary<T> {
    const defaultKeySelector: (item: T) => string | number = (p) =>
      p.toString();

    return keyBy(
      collection,
      dictionaryKeySelector != null ? dictionaryKeySelector : defaultKeySelector
    );
  }

  private rightJoinBy<TLeft, TRight>(
    left: TLeft[],
    right: TRight[],
    leftBy: (item: TLeft) => string | number,
    rightBy: (item: TRight) => string | number
  ): TLeft[] {
    const rightDic = this.toDictionary(right, rightBy);

    return left ? left.filter((p) => rightDic[leftBy(p)] != null) : [];
  }

  private defaultIfNull<T>(value: T | null | undefined, defaultValue: T): T {
    return value == null ? defaultValue : value;
  }

  private orderBy<T>(
    collection: T[],
    iteratees: (
      value: T,
      index: number,
      collection: ArrayLike<T>
    ) => number | string | object,
    desc: boolean = false
  ): T[] {
    return orderBy(collection, iteratees, desc ? 'desc' : 'asc');
  }

  private rightJoinSingleBy<TLeft, TRight>(
    left: TLeft,
    right: TRight[],
    leftBy: (item: TLeft) => string | number,
    rightBy: (item: TRight) => string | number
  ): TLeft | undefined {
    const rightDic = this.toDictionary(right, rightBy);

    return rightDic[leftBy(left)] != null ? left : undefined;
  }

  private toDictionaryGroupBy<T>(
    collection: ArrayLike<T> | undefined,
    dictionaryGroupByKeySelector: (item: T) => string | number,
    mapFn?: (items: T[]) => T[]
  ): Dictionary<T[]> {
    if (collection == null) {
      return {};
    }
    const result: Dictionary<T[]> = {};
    for (let i = 0; i < collection.length; i++) {
      const item = collection[i];
      if (result[dictionaryGroupByKeySelector(item)] == null) {
        result[dictionaryGroupByKeySelector(item)] = [];
      }
      result[dictionaryGroupByKeySelector(item)].push(item);
    }

    if (mapFn != null) {
      Object.keys(result).forEach((key) => {
        result[key] = mapFn(result[key]);
      });
    }

    return result;
  }
  //#endregion Utils
}

export enum MetadataTagGroupCode {
  PDO_TYPES = 'PDO-TYPES',
  SERVICE_SCHEMES = 'SERVICESCHEMES',
  PDO_CATEGORIES = 'PDO-CATEGORIES',
  PDO_MODES = 'PDO-MODES', // MODE OF LEARNING
  PDO_NATURES = 'PDO-NATURES', // NATURE OF COURSE
  COURSE_LEVELS = 'COURSE-LEVELS',
  DEVROLES = 'DEVROLES',
  PDO_TAXONOMY = 'PDO-TAXONOMY', // Subject Areas
  LEARNING_FXS = 'LEARNING-FXS', // LEARNING FRAMEWORKS
  LEARNING_DIMENSION = 'LEARNING-DIMENSION',
  LEARNING_AREA = 'LEARNING-AREA',
  LEARNING_SUB_AREA = 'LEARNING-SUB-AREA',
  TRACKS = 'TRACKS',
  TEACHING_SUBJECTS = 'TEACHING-SUBJECTS',
  TEACHING_LEVELS = 'TEACHING-LEVELS',
  JOB_FAMILIES = 'JOB-FAMILIES',
  COURSES_OF_STUDY = 'COURSES-OF-STUDY',
  CO_CURRICULAR_ACTIVITIES = 'CCAS',
  PERIOD_PD_ACTIVITY = 'PDO-PERIODS',
}
