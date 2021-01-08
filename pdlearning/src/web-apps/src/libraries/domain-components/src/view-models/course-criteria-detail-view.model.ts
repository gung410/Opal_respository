import {
  Course,
  CourseCriteria,
  CourseCriteriaAccountType,
  CourseCriteriaDepartmentLevelType,
  CourseCriteriaDepartmentUnitType,
  CourseCriteriaPlaceOfWork,
  CourseCriteriaPlaceOfWorkType,
  CourseCriteriaServiceScheme,
  CourseCriteriaSpecificDepartment,
  DepartmentInfoModel,
  DepartmentLevelModel,
  MetadataId,
  MetadataTagGroupCode,
  MetadataTagModel,
  TypeOfOrganization
} from '@opal20/domain-api';
import { Observable, combineLatest, of } from 'rxjs';

import { IOpalSelectDefaultItem } from '@opal20/common-components';
import { Utils } from '@opal20/infrastructure';
import { map } from 'rxjs/operators';

export class CourseCriteriaDetailViewModel {
  public static readonly create = create;

  public get id(): string {
    return this.courseCriteriaData.id;
  }

  public set id(id: string) {
    this.courseCriteriaData.id = id;
  }
  public courseCriteriaAccountTypeSelectedItems: IOpalSelectDefaultItem<string>[] = [
    { value: CourseCriteriaAccountType.AllLearners, label: 'All Learners' },
    { value: CourseCriteriaAccountType.MOELearners, label: 'MOE Learners' },
    { value: CourseCriteriaAccountType.ExternalLearners, label: 'External Learners' }
  ];

  public courseCriteriaAccountTypeDic: Dictionary<string> = Utils.toDictionarySelect(
    this.courseCriteriaAccountTypeSelectedItems,
    x => x.value,
    x => x.label
  );

  public get accountType(): CourseCriteriaAccountType {
    return this.courseCriteriaData.accountType;
  }
  public set accountType(accountType: CourseCriteriaAccountType) {
    this.courseCriteriaData.accountType = accountType;
  }

  public _courseCriteriaServiceSchemeIds: string[] = [];
  public get courseCriteriaServiceSchemeIds(): string[] {
    return this._courseCriteriaServiceSchemeIds;
  }
  public set courseCriteriaServiceSchemeIds(v: string[]) {
    this._courseCriteriaServiceSchemeIds = v;
    this.courseCriteriaData.courseCriteriaServiceSchemes = this.buildNewCourseCriteriaServiceSchemes(
      this.courseCriteriaData.courseCriteriaServiceSchemes,
      v
    );
    this.developmentalRolesTree = this.buildDevelopmentalRolesTree();
    this.easSubstantiveGradeBandingTree = this.buildEasSubstantiveGradeBandingTree();
  }
  public get courseCriteriaServiceSchemes(): CourseCriteriaServiceScheme[] {
    return this.courseCriteriaData.courseCriteriaServiceSchemes;
  }
  public set courseCriteriaServiceSchemes(courseCriteriaServiceSchemes: CourseCriteriaServiceScheme[]) {
    this.courseCriteriaData.courseCriteriaServiceSchemes = courseCriteriaServiceSchemes;
  }

  public get tracks(): string[] {
    return this.courseCriteriaData.tracks;
  }
  public set tracks(tracks: string[]) {
    this.courseCriteriaData.tracks = tracks;
  }

  public get devRoles(): string[] {
    return this.courseCriteriaData.devRoles;
  }
  public set devRoles(devRoles: string[]) {
    this.courseCriteriaData.devRoles = devRoles;
  }

  public get teachingLevels(): string[] {
    return this.courseCriteriaData.teachingLevels;
  }
  public set teachingLevels(teachingLevels: string[]) {
    this.courseCriteriaData.teachingLevels = teachingLevels;
  }

  public get teachingCourseOfStudy(): string[] {
    return this.courseCriteriaData.teachingCourseOfStudy;
  }
  public set teachingCourseOfStudy(teachingCourseOfStudy: string[]) {
    this.courseCriteriaData.teachingCourseOfStudy = teachingCourseOfStudy;
  }

  public get jobFamily(): string[] {
    return this.courseCriteriaData.jobFamily;
  }

  public set jobFamily(jobFamily: string[]) {
    this.courseCriteriaData.jobFamily = jobFamily;
  }

  public get coCurricularActivity(): string[] {
    return this.courseCriteriaData.coCurricularActivity;
  }
  public set coCurricularActivity(coCurricularActivity: string[]) {
    this.courseCriteriaData.coCurricularActivity = coCurricularActivity;
  }

  public get subGradeBanding(): string[] {
    return this.courseCriteriaData.subGradeBanding;
  }
  public set subGradeBanding(subGradeBanding: string[]) {
    this.courseCriteriaData.subGradeBanding = subGradeBanding;
  }

  public _placeOfWorkDepatmentUnitIds: string[] = [];
  public get placeOfWorkDepatmentUnitIds(): string[] {
    return this._placeOfWorkDepatmentUnitIds;
  }
  public set placeOfWorkDepatmentUnitIds(v: string[]) {
    this._placeOfWorkDepatmentUnitIds = v;
    this.courseCriteriaData.placeOfWork.departmentUnitTypes = this.buildNewCourseCriteriaDepartmentUnitType(
      this.courseCriteriaData.placeOfWork.departmentUnitTypes,
      v
    );
  }

  public _placeOfWorkDepatmentLevelIds: string[] = [];
  public get placeOfWorkDepatmentLevelIds(): string[] {
    return this._placeOfWorkDepatmentLevelIds;
  }
  public set placeOfWorkDepatmentLevelIds(v: string[]) {
    this._placeOfWorkDepatmentLevelIds = v;
    this.courseCriteriaData.placeOfWork.departmentLevelTypes = this.buildNewCourseCriteriaDepartmentLevelType(
      this.courseCriteriaData.placeOfWork.departmentLevelTypes,
      v
    );
  }

  public _placeOfWorkSpecificDepartmentsIds: number[] = [];
  public get placeOfWorkSpecificDepartmentsIds(): number[] {
    return this._placeOfWorkSpecificDepartmentsIds;
  }
  public set placeOfWorkSpecificDepartmentsIds(v: number[]) {
    this._placeOfWorkSpecificDepartmentsIds = v;
    this.courseCriteriaData.placeOfWork.specificDepartments = this.buildNewCourseCriteriaCourseCriteriaSpecificDepartment(
      this.courseCriteriaData.placeOfWork.specificDepartments,
      v
    );
  }

  public get placeOfWork(): CourseCriteriaPlaceOfWork {
    return this.courseCriteriaData.placeOfWork;
  }
  public set placeOfWork(placeOfWork: CourseCriteriaPlaceOfWork) {
    this.courseCriteriaData.placeOfWork = placeOfWork;
  }

  public get selectedPlaceOfWorkItems():
    | CourseCriteriaDepartmentUnitType[]
    | CourseCriteriaDepartmentLevelType[]
    | CourseCriteriaSpecificDepartment[] {
    switch (this.selectedPlaceOfWorkType) {
      case CourseCriteriaPlaceOfWorkType.DepartmentLevelTypes:
        return this.courseCriteriaData.placeOfWork.departmentLevelTypes;
      case CourseCriteriaPlaceOfWorkType.DepartmentUnitTypes:
        return this.courseCriteriaData.placeOfWork.departmentUnitTypes;
      case CourseCriteriaPlaceOfWorkType.SpecificDepartments:
        return this.courseCriteriaData.placeOfWork.specificDepartments;
      default:
        return [];
    }
  }
  public set selectedPlaceOfWorkItems(
    v: CourseCriteriaDepartmentUnitType[] | CourseCriteriaDepartmentLevelType[] | CourseCriteriaSpecificDepartment[]
  ) {
    switch (this.selectedPlaceOfWorkType) {
      case CourseCriteriaPlaceOfWorkType.DepartmentLevelTypes:
        this.courseCriteriaData.placeOfWork.departmentLevelTypes = <CourseCriteriaDepartmentLevelType[]>v;
      case CourseCriteriaPlaceOfWorkType.DepartmentUnitTypes:
        this.courseCriteriaData.placeOfWork.departmentUnitTypes = <CourseCriteriaDepartmentUnitType[]>v;
      case CourseCriteriaPlaceOfWorkType.SpecificDepartments:
        this.courseCriteriaData.placeOfWork.specificDepartments = <CourseCriteriaSpecificDepartment[]>v;
    }
  }

  public get selectedPlaceOfWorkType(): CourseCriteriaPlaceOfWorkType {
    return this.placeOfWork.type;
  }
  public set selectedPlaceOfWorkType(v: CourseCriteriaPlaceOfWorkType) {
    this.placeOfWork.type = v;
  }

  public get placeOfWorkMaxParticipantsTitle(): string {
    switch (this.selectedPlaceOfWorkType) {
      case CourseCriteriaPlaceOfWorkType.DepartmentUnitTypes:
        return 'Organisation type';
      case CourseCriteriaPlaceOfWorkType.DepartmentLevelTypes:
        return 'Organisation level';
      case CourseCriteriaPlaceOfWorkType.SpecificDepartments:
        return 'Specific organisation';
    }
  }

  public get preRequisiteCourses(): string[] {
    return this.courseCriteriaData.preRequisiteCourses;
  }
  public set preRequisiteCourses(preRequisiteCourses: string[]) {
    this.courseCriteriaData.preRequisiteCourses = preRequisiteCourses;
  }
  public courseCriteriaData: CourseCriteria = new CourseCriteria();
  public originalData: CourseCriteria = new CourseCriteria();

  //#region Metadata dropdown items
  public metadataTagsDic: Dictionary<MetadataTagModel> = {};
  public metadataTagsDicByGroupCode: Dictionary<MetadataTagModel[]> = {};
  public serviceSchemeSelectItems: MetadataTagModel[] = [];

  public _departments: DepartmentInfoModel[];
  public get departments(): DepartmentInfoModel[] {
    return this._departments;
  }
  public set departments(v: DepartmentInfoModel[]) {
    if (Utils.isDifferent(this._departments, v)) {
      this._departments = v;
      this.departmentsDic = { ...this.departmentsDic, ...Utils.toDictionary(v, p => p.id) };
    }
  }

  public _departmentsUnits: TypeOfOrganization[];
  public get departmentsUnits(): TypeOfOrganization[] {
    return this._departmentsUnits;
  }
  public set departmentsUnits(v: TypeOfOrganization[]) {
    if (Utils.isDifferent(this._departmentsUnits, v)) {
      this._departmentsUnits = v;
      this.departmentsUnitDic = { ...this.departmentsUnitDic, ...Utils.toDictionary(v, p => p.id) };
    }
  }

  public _departmentsLevels: DepartmentLevelModel[];
  public get departmentsLevels(): DepartmentLevelModel[] {
    return this._departmentsLevels;
  }
  public set departmentsLevels(v: DepartmentLevelModel[]) {
    if (Utils.isDifferent(this._departmentsLevels, v)) {
      this._departmentsLevels = v;
      this.departmentsLevelDic = { ...this.departmentsLevelDic, ...Utils.toDictionary(v, p => p.id) };
    }
  }

  public departmentsDic: Dictionary<DepartmentInfoModel> = {};
  public departmentsUnitDic: Dictionary<TypeOfOrganization> = {};
  public departmentsLevelDic: Dictionary<DepartmentLevelModel> = {};

  public _prerequisiteCoursesSelectItems: Course[] = [];
  public get prerequisiteCoursesSelectItems(): Course[] {
    return this._prerequisiteCoursesSelectItems;
  }
  public set prerequisiteCoursesSelectItems(v: Course[]) {
    if (Utils.isDifferent(this._prerequisiteCoursesSelectItems, v)) {
      this._prerequisiteCoursesSelectItems = v;
      this.prerequisiteCoursesDic = { ...this.prerequisiteCoursesDic, ...Utils.toDictionary(v, p => p.id) };
    }
  }
  public prerequisiteCoursesDic: Dictionary<Course> = {};

  public _tracksTreeValues: string[] = [];
  public get tracksTreeValues(): string[] {
    return this._tracksTreeValues;
  }
  public set tracksTreeValues(v: string[]) {
    if (this._tracksTreeValues !== v) {
      this._tracksTreeValues = v;
      this.tracks = this._tracksTreeValues.filter(p => this.metadataTagsDic[p].groupCode !== MetadataTagGroupCode.SERVICE_SCHEMES);
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
      this.teachingCourseOfStudy = this._teachingCourseStudysTreeValues.filter(
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

  public _easSubstantiveGradeBandingTreeValues: string[] = [];
  public get easSubstantiveGradeBandingTreeValues(): string[] {
    return this._easSubstantiveGradeBandingTreeValues;
  }
  public set easSubstantiveGradeBandingTreeValues(v: string[]) {
    if (this._easSubstantiveGradeBandingTreeValues !== v) {
      this._easSubstantiveGradeBandingTreeValues = v;
      this.subGradeBanding = this._easSubstantiveGradeBandingTreeValues.filter(
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

  public _developmentalRolesTreeValues: string[] = [];
  public get developmentalRolesTreeValues(): string[] {
    return this._developmentalRolesTreeValues;
  }
  public set developmentalRolesTreeValues(v: string[]) {
    if (this._developmentalRolesTreeValues !== v) {
      this._developmentalRolesTreeValues = v;
      this.devRoles = this._developmentalRolesTreeValues.filter(
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

  public _cocurricularActivitySelectItems: MetadataTagModel[] = [];
  public get cocurricularActivitySelectItems(): MetadataTagModel[] {
    return this._cocurricularActivitySelectItems;
  }
  public set cocurricularActivitySelectItems(v: MetadataTagModel[]) {
    this._cocurricularActivitySelectItems = v;
    this.coCurricularActivity = Utils.rightJoin(this.coCurricularActivity, this.cocurricularActivitySelectItems.map(p => p.tagId));
  }
  //#endregion

  public courseCriteriaServiceSchemeLabelFn: (item: CourseCriteriaServiceScheme) => string;

  constructor(
    courseCriteria?: CourseCriteria,
    public metadataTags: MetadataTagModel[] = [],
    prerequisiteCourses: Course[] = [],
    departmentsUnits: TypeOfOrganization[] = [],
    departmentsLevels: DepartmentLevelModel[] = [],
    departments: DepartmentInfoModel[] = []
  ) {
    if (courseCriteria) {
      this.updateCourseCriteriaData(courseCriteria);
    }

    this.setMetadataTagsDicInfo(metadataTags);
    this.departmentsUnits = departmentsUnits;
    this.departmentsLevels = departmentsLevels;
    this.departments = departments;
    this.prerequisiteCoursesSelectItems = prerequisiteCourses;
    //#region Metadata dropdown items
    this.serviceSchemeSelectItems = Utils.defaultIfNull(
      Utils.rightJoinBy(
        Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.SERVICE_SCHEMES], []),
        Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TAXONOMY], []),
        x => x.tagId,
        x => x.parentTagId
      ),
      []
    );
    this._tracksTree = this.buildTracksTree();
    this._tracksTreeValues = this.buildTracksTreeValues();
    this.selectedTracksTree = this.buildSelectedTracksTree();

    this._teachingLevelsTree = this.buildTeachingLevelsTree();
    this._developmentalRolesTreeValues = this.buildTeachingLevelsTreeValues();
    this.selectedTeachingLevelsTree = this.buildSelectedTeachingLevelsTree();

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

    this._developmentalRolesTree = this.buildDevelopmentalRolesTree();
    this._developmentalRolesTreeValues = this.buildDevelopmentalRolesTreeValues();
    this.selectedDevelopmentalRolesTree = this.buildSelectedDevelopmentalRolesTree();
    ////#endregion

    this.courseCriteriaServiceSchemeLabelFn = item => this.metadataTagsDic[item.serviceSchemeId].displayText;
  }

  public getSelectedPlaceOfWorkItemFn: (
    item: CourseCriteriaDepartmentUnitType | CourseCriteriaDepartmentLevelType | CourseCriteriaSpecificDepartment
  ) => string = item => {
    if (item instanceof CourseCriteriaDepartmentUnitType && this.departmentsUnitDic[item.departmentUnitTypeId] != null) {
      return this.departmentsUnitDic[item.departmentUnitTypeId].displayText;
    }
    if (item instanceof CourseCriteriaDepartmentLevelType && this.departmentsLevelDic[item.departmentLevelTypeId] != null) {
      return this.departmentsLevelDic[item.departmentLevelTypeId].departmentLevelName;
    }
    if (item instanceof CourseCriteriaSpecificDepartment && this.departmentsDic[item.departmentId] != null) {
      return this.departmentsDic[item.departmentId].departmentName;
    }
    return 'n/a';
  };

  public setPlaceOfWork(value: CourseCriteriaPlaceOfWorkType): void {
    this.selectedPlaceOfWorkType = value;
  }

  public checkDepartmentUnitTypes(): boolean {
    return this.selectedPlaceOfWorkType === CourseCriteriaPlaceOfWorkType.DepartmentUnitTypes;
  }

  public checkDepartmentLevelTypes(): boolean {
    return this.selectedPlaceOfWorkType === CourseCriteriaPlaceOfWorkType.DepartmentLevelTypes;
  }

  public checkDepartmentSpecificTypes(): boolean {
    return this.selectedPlaceOfWorkType === CourseCriteriaPlaceOfWorkType.SpecificDepartments;
  }
  public updateMetadataTags(metadataTags: MetadataTagModel[]): void {
    this.setMetadataTagsDicInfo(metadataTags);
    this.serviceSchemeSelectItems = this.serviceSchemeSelectItems.map(p => this.metadataTagsDic[p.tagId]).filter(p => p != null);
  }

  public updateCourseCriteriaData(courseCriteria: CourseCriteria): void {
    this.originalData = Utils.cloneDeep(courseCriteria);
    this.courseCriteriaData = Utils.cloneDeep(courseCriteria);
    this._courseCriteriaServiceSchemeIds = courseCriteria.courseCriteriaServiceSchemes.map(p => p.serviceSchemeId);
    this.placeOfWorkDepatmentLevelIds = this.courseCriteriaData.placeOfWork.departmentLevelTypes.map(p => p.departmentLevelTypeId);
    this.placeOfWorkDepatmentUnitIds = this.courseCriteriaData.placeOfWork.departmentUnitTypes.map(p => p.departmentUnitTypeId);
    this.placeOfWorkSpecificDepartmentsIds = this.courseCriteriaData.placeOfWork.specificDepartments.map(p => p.departmentId);
  }

  public dataCourseCriteriaHasChanged(): boolean {
    return Utils.isDifferent(this.originalData, this.courseCriteriaData);
  }

  public reuseCourseExistingData(course: Course): void {
    this.courseCriteriaData.reuseCourseExistingData(course);
    this.courseCriteriaServiceSchemeIds = Utils.addIfNotExist(this.courseCriteriaServiceSchemeIds, course.serviceSchemeIds, p => p);
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

  public serviceSchemesContains(tagId: MetadataId): boolean {
    return (
      this.courseCriteriaServiceSchemeIds &&
      this.courseCriteriaServiceSchemeIds.find(p => this.metadataTagsDic[p] && this.metadataTagsDic[p].tagId === tagId) != null
    );
  }

  private setMetadataTagsDicInfo(metadataTags: MetadataTagModel[]): void {
    this.metadataTagsDic = Utils.toDictionary(metadataTags, p => p.tagId);
    this.metadataTagsDicByGroupCode = Utils.toDictionaryGroupBy(
      metadataTags.filter(p => p.groupCode != null),
      p => p.groupCode,
      items => Utils.orderBy(items, p => p.displayText)
    );
  }
  private buildNewCourseCriteriaServiceSchemes(
    oldValues: CourseCriteriaServiceScheme[],
    selectedServiceSchemeIds: string[]
  ): CourseCriteriaServiceScheme[] {
    const byServiceSchemeIdOldValuesDic = Utils.toDictionary(oldValues, p => p.serviceSchemeId);
    return selectedServiceSchemeIds.map(p =>
      byServiceSchemeIdOldValuesDic[p] != null ? byServiceSchemeIdOldValuesDic[p] : new CourseCriteriaServiceScheme({ serviceSchemeId: p })
    );
  }
  private buildNewCourseCriteriaDepartmentUnitType(
    oldValues: CourseCriteriaDepartmentUnitType[],
    selectedDeparmentUnit: string[]
  ): CourseCriteriaDepartmentUnitType[] {
    const byServiceSchemeIdOldValuesDic = Utils.toDictionary(oldValues, p => p.departmentUnitTypeId);
    return selectedDeparmentUnit.map(p =>
      byServiceSchemeIdOldValuesDic[p] != null
        ? byServiceSchemeIdOldValuesDic[p]
        : new CourseCriteriaDepartmentUnitType({ departmentUnitTypeId: p })
    );
  }
  private buildNewCourseCriteriaDepartmentLevelType(
    oldValues: CourseCriteriaDepartmentLevelType[],
    selectedDeparmentLevel: string[]
  ): CourseCriteriaDepartmentLevelType[] {
    const byServiceSchemeIdOldValuesDic = Utils.toDictionary(oldValues, p => p.departmentLevelTypeId);
    return selectedDeparmentLevel.map(p =>
      byServiceSchemeIdOldValuesDic[p] != null
        ? byServiceSchemeIdOldValuesDic[p]
        : new CourseCriteriaDepartmentLevelType({ departmentLevelTypeId: p })
    );
  }
  private buildNewCourseCriteriaCourseCriteriaSpecificDepartment(
    oldValues: CourseCriteriaSpecificDepartment[],
    selectedDeparmentLevel: number[]
  ): CourseCriteriaSpecificDepartment[] {
    const byServiceSchemeIdOldValuesDic = Utils.toDictionary(oldValues, p => p.departmentId);
    return selectedDeparmentLevel.map(p =>
      byServiceSchemeIdOldValuesDic[p] != null
        ? byServiceSchemeIdOldValuesDic[p]
        : new CourseCriteriaSpecificDepartment({ departmentId: p })
    );
  }

  private buildEasSubstantiveGradeBandingTree(): MetadataTagModel[] {
    if (this.courseCriteriaServiceSchemeIds.length === 0) {
      return [];
    }
    const serviceSchemesDic = Utils.toDictionary(
      this.courseCriteriaServiceSchemeIds.filter(p => p === MetadataId.ExecutiveAndAdministrativeStaff),
      p => p
    );
    return MetadataTagModel.buildTree(this.metadataTags, p => serviceSchemesDic[p.tagId] != null, [
      p => p.groupCode === MetadataTagGroupCode.LEARNING_FXS
    ]).filter(p => p.childs != null && p.childs.length > 0);
  }

  private buildSelectedEasSubstantiveGradeBandingTree(): MetadataTagModel[] {
    return MetadataTagModel.filterSelectedTreeNodes(this.easSubstantiveGradeBandingTree, this.subGradeBanding);
  }

  private buildEasSubstantiveGradeBandingTreeValues(): string[] {
    return Utils.clone(this.subGradeBanding);
  }

  private buildTracksTree(): MetadataTagModel[] {
    return MetadataTagModel.buildTree(this.metadataTags, null, [p => p.groupCode === MetadataTagGroupCode.TRACKS]).filter(
      p => p.childs != null && p.childs.length > 0
    );
  }

  private buildSelectedTracksTree(): MetadataTagModel[] {
    return MetadataTagModel.filterSelectedTreeNodes(this.tracksTree, this.tracks);
  }

  private buildTracksTreeValues(): string[] {
    return Utils.clone(this.tracks);
  }

  private buildTeachingCourseStudysTree(): MetadataTagModel[] {
    return MetadataTagModel.buildTree(this.metadataTags, null, [p => p.groupCode === MetadataTagGroupCode.COURSES_OF_STUDY]).filter(
      p => p.childs != null && p.childs.length > 0
    );
  }

  private buildSelectedTeachingCourseStudysTree(): MetadataTagModel[] {
    return MetadataTagModel.filterSelectedTreeNodes(this.teachingCourseStudysTree, this.teachingCourseOfStudy);
  }

  private buildTeachingCourseStudysTreeValues(): string[] {
    return Utils.clone(this.teachingCourseOfStudy);
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
    if (this.courseCriteriaServiceSchemeIds.length === 0) {
      return [];
    }
    const serviceSchemesDic = Utils.toDictionary(this.courseCriteriaServiceSchemeIds, p => p);
    return MetadataTagModel.buildTree(this.metadataTags, p => serviceSchemesDic[p.tagId] != null, [
      p => p.groupCode === MetadataTagGroupCode.DEVROLES
    ]).filter(p => p.childs != null && p.childs.length > 0);
  }

  private buildSelectedDevelopmentalRolesTree(): MetadataTagModel[] {
    return MetadataTagModel.filterSelectedTreeNodes(this.developmentalRolesTree, this.devRoles);
  }

  private buildDevelopmentalRolesTreeValues(): string[] {
    return Utils.clone(this.devRoles);
  }
}

function create(
  courseCriteria: CourseCriteria,
  metadataTags: MetadataTagModel[] = [],
  departmentsUnits: TypeOfOrganization[] = [],
  departmentsLevels: DepartmentLevelModel[] = [],
  getDepartmentByIdsFn: (ids: number[]) => Observable<DepartmentInfoModel[]>,
  getCourseByIdsFn: (ids: string[]) => Observable<Course[]>
): Observable<CourseCriteriaDetailViewModel> {
  const preRequisiteCoursesObs =
    courseCriteria.preRequisiteCourses.length === 0 ? of([]) : getCourseByIdsFn(courseCriteria.preRequisiteCourses);
  const departmentObs =
    courseCriteria.placeOfWork.specificDepartments.length === 0
      ? of([])
      : getDepartmentByIdsFn(courseCriteria.placeOfWork.specificDepartments.map(p => p.departmentId));
  return combineLatest(preRequisiteCoursesObs, departmentObs).pipe(
    map(
      ([courses, departments]) =>
        new CourseCriteriaDetailViewModel(courseCriteria, metadataTags, courses, departmentsUnits, departmentsLevels, departments)
    )
  );
}
