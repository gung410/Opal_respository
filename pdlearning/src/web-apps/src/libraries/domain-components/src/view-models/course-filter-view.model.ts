import { CourseFilterModel, CourseFilterOwnBy } from './../models/course-filter.model';
import {
  CourseStatus,
  DepartmentInfoModel,
  MetadataTagGroupCode,
  MetadataTagModel,
  NOT_APPLICABLE_ITEM_DISPLAY_TEXT
} from '@opal20/domain-api';
import { MAX_INT, Utils } from '@opal20/infrastructure';
import { Observable, combineLatest, of } from 'rxjs';

import { CourseDetailViewModel } from './course-detail-view.model';
import { ICourseFilterSetting } from '../models/course-filter-setting.model';
import { IOpalSelectDefaultItem } from '@opal20/common-components';
import { map } from 'rxjs/operators';

export class CourseFilterViewModel {
  public static readonly create = create;
  public static readonly NOT_APPLICABLE_ITEM: MetadataTagModel = new MetadataTagModel({
    tagId: null,
    displayText: NOT_APPLICABLE_ITEM_DISPLAY_TEXT,
    fullStatement: null
  });
  public metadataTagsDicByGroupCode: Dictionary<MetadataTagModel[]> = {};

  public ownSelectItems: IOpalSelectDefaultItem<string>[] = [
    { value: CourseFilterOwnBy.All, label: 'All' },
    { value: CourseFilterOwnBy.MeOnly, label: 'Me only' },
    { value: CourseFilterOwnBy.OtherUsers, label: 'Other users' }
  ];

  public statusSelectItems: IOpalSelectDefaultItem<string>[] = [
    { value: CourseStatus.Draft, label: 'Draft' },
    { value: CourseStatus.PendingApproval, label: 'Pending Approval' },
    { value: CourseStatus.Approved, label: 'Approved' },
    { value: CourseStatus.Rejected, label: 'Rejected' },
    { value: CourseStatus.PlanningCycleVerified, label: 'Confirmed' },
    { value: CourseStatus.VerificationRejected, label: 'Verification Rejected' },
    { value: CourseStatus.PlanningCycleCompleted, label: 'Completed Planning' },
    { value: CourseStatus.Published, label: 'Published' },
    { value: CourseStatus.Unpublished, label: 'Unpublished' },
    { value: CourseStatus.Completed, label: 'Completed' }
  ];

  public contentStatusSelectItems: IOpalSelectDefaultItem<string>[] = [];

  // Metadata selected items
  public pdTypeSelectItems: MetadataTagModel[] = [];
  public categorySelectItems: MetadataTagModel[] = [];
  public serviceSchemeSelectItems: MetadataTagModel[] = [];
  public teachingLevelsSelectItems: MetadataTagModel[] = [];
  public courseLevelSelectItems: MetadataTagModel[] = [];
  public learningFrameworksSelectItems: MetadataTagModel[] = [];
  public learningDimensionSelectItems: MetadataTagModel[] = [];
  public developmentalRoleSelectItems: MetadataTagModel[] = [];
  public learningAreaSelectItems: MetadataTagModel[] = [];
  public learningSubAreaSelectItems: MetadataTagModel[] = [];
  public subjectSelectItems: MetadataTagModel[] = [];

  private originData: CourseFilterModel = new CourseFilterModel();

  constructor(
    public metadataTags: MetadataTagModel[] = [],
    public departmentSelectItems: DepartmentInfoModel[],
    public data: CourseFilterModel = new CourseFilterModel(),
    public settings: ICourseFilterSetting = null
  ) {
    this.metadataTags = metadataTags.filter(p => p.displayText !== NOT_APPLICABLE_ITEM_DISPLAY_TEXT);
    this.setMetadataTagsDicInfo(this.metadataTags);

    if (this.settings != null) {
      this.statusSelectItems =
        this.settings.statusSelectItems != null && this.settings.statusSelectItems.length > 0
          ? this.settings.statusSelectItems
          : this.statusSelectItems;
      this.contentStatusSelectItems =
        this.settings.contentStatusSelectItems != null && this.settings.contentStatusSelectItems.length > 0
          ? this.settings.contentStatusSelectItems
          : this.contentStatusSelectItems;
    }

    this.pdTypeSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TYPES], []);
    this.categorySelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_CATEGORIES], []);
    this.serviceSchemeSelectItems = Utils.defaultIfNull(
      Utils.rightJoinBy(
        Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.SERVICE_SCHEMES], []),
        Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TAXONOMY], []),
        x => x.tagId,
        x => x.parentTagId
      ),
      []
    );
    this.teachingLevelsSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.TEACHING_LEVELS], []);
    this.courseLevelSelectItems = Utils.orderBy(
      Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.COURSE_LEVELS], []),
      p => CourseDetailViewModel.ORDERED_COURSE_LEVELS[p.displayText] || MAX_INT
    );
    this.subjectSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TAXONOMY], []).filter(
      p => p.codingScheme != null
    );
    this.learningFrameworksSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.LEARNING_FXS], []);
    const learningFrameworksSelectItemsDic = Utils.toDictionary(this.learningFrameworksSelectItems, p => p.tagId);
    this.learningDimensionSelectItems = this.metadataTags.filter(p => learningFrameworksSelectItemsDic[p.parentTagId] != null);
    const learningDimensionSelectItemsDic = Utils.toDictionary(this.learningDimensionSelectItems, p => p.tagId);
    this.learningAreaSelectItems = this.metadataTags.filter(p => learningDimensionSelectItemsDic[p.parentTagId] != null);
    const learningAreaSelectItemsDic = Utils.toDictionary(this.learningAreaSelectItems, p => p.tagId);
    this.learningSubAreaSelectItems = this.metadataTags.filter(p => learningAreaSelectItemsDic[p.parentTagId] != null);
    this.developmentalRoleSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.DEVROLES], []);
    this.addNotApplicableItem();
    this.originData = Utils.cloneDeep(this.data);
  }

  public dataHasChanged(): boolean {
    return Utils.isDifferent(this.originData, this.data);
  }

  private setMetadataTagsDicInfo(metadataTags: MetadataTagModel[]): void {
    this.metadataTagsDicByGroupCode = Utils.toDictionaryGroupBy(metadataTags.filter(p => p.groupCode != null), p => p.groupCode);
  }

  private addNotApplicableItem(): void {
    this.pdTypeSelectItems.push(CourseFilterViewModel.NOT_APPLICABLE_ITEM);
    this.categorySelectItems.push(CourseFilterViewModel.NOT_APPLICABLE_ITEM);
    this.serviceSchemeSelectItems.push(CourseFilterViewModel.NOT_APPLICABLE_ITEM);
    this.teachingLevelsSelectItems.push(CourseFilterViewModel.NOT_APPLICABLE_ITEM);
    this.courseLevelSelectItems.push(CourseFilterViewModel.NOT_APPLICABLE_ITEM);
    this.learningFrameworksSelectItems.push(CourseFilterViewModel.NOT_APPLICABLE_ITEM);
    this.learningDimensionSelectItems.push(CourseFilterViewModel.NOT_APPLICABLE_ITEM);
    this.developmentalRoleSelectItems.push(CourseFilterViewModel.NOT_APPLICABLE_ITEM);
    this.learningAreaSelectItems.push(CourseFilterViewModel.NOT_APPLICABLE_ITEM);
    this.learningSubAreaSelectItems.push(CourseFilterViewModel.NOT_APPLICABLE_ITEM);
    this.subjectSelectItems.push(CourseFilterViewModel.NOT_APPLICABLE_ITEM);
  }
}

function create(
  getOrganisationUnitIdsFn: (ids: number[]) => Observable<DepartmentInfoModel[]>,
  metadataTags: MetadataTagModel[] = [],
  data: CourseFilterModel = new CourseFilterModel(),
  settings: ICourseFilterSetting = null
): Observable<CourseFilterViewModel> {
  const organisationUnitObs =
    data.fromOrganisation && data.fromOrganisation.length > 0 ? getOrganisationUnitIdsFn(data.fromOrganisation) : of([]);
  return combineLatest(organisationUnitObs).pipe(
    map(([organisationUnits]) => {
      return new CourseFilterViewModel(metadataTags, organisationUnits, data, settings);
    })
  );
}
