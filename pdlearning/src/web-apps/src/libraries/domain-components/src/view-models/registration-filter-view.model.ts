import {
  DepartmentInfoModel,
  Designation,
  MetadataTagGroupCode,
  MetadataTagModel,
  NOT_APPLICABLE_ITEM_DISPLAY_TEXT
} from '@opal20/domain-api';
import { Observable, combineLatest, of } from 'rxjs';

import { RegistrationFilterModel } from '../models/registration-filter.model';
import { Utils } from '@opal20/infrastructure';
import { map } from 'rxjs/operators';

export class RegistrationFilterViewModel {
  public static readonly create = create;
  public static readonly NOT_APPLICABLE_ITEM: MetadataTagModel = new MetadataTagModel({
    tagId: null,
    displayText: NOT_APPLICABLE_ITEM_DISPLAY_TEXT,
    fullStatement: null
  });

  public static readonly NOT_APPLICABLE_ITEM_DESIGNATION: Designation = new Designation({
    id: null,
    displayText: NOT_APPLICABLE_ITEM_DISPLAY_TEXT,
    fullStatement: null
  });

  public metadataTagsDicByGroupCode: Dictionary<MetadataTagModel[]> = {};

  // Metadata selected items
  public serviceSchemeSelectItems: MetadataTagModel[] = [];
  public developmentalRoleSelectItems: MetadataTagModel[] = [];
  public teachingLevelSelectItems: MetadataTagModel[] = [];
  public teachingCourseOfStudySelectItems: MetadataTagModel[] = [];
  public teachingSubjectSelectItems: MetadataTagModel[] = [];
  public learningFrameworksSelectItems: MetadataTagModel[] = [];

  private originData: RegistrationFilterModel = new RegistrationFilterModel();

  constructor(
    public metadataTags: MetadataTagModel[] = [],
    public designationSelectItems: Designation[] = [],
    public departmentSelectItems: DepartmentInfoModel[],
    public data: RegistrationFilterModel = new RegistrationFilterModel()
  ) {
    this.metadataTags = metadataTags.filter(p => p.displayText !== NOT_APPLICABLE_ITEM_DISPLAY_TEXT);
    this.setMetadataTagsDicInfo(this.metadataTags);

    this.serviceSchemeSelectItems = Utils.defaultIfNull(
      Utils.rightJoinBy(
        Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.SERVICE_SCHEMES], []),
        Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TAXONOMY], []),
        x => x.tagId,
        x => x.parentTagId
      ),
      []
    );
    this.teachingLevelSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.TEACHING_LEVELS], []);
    this.teachingCourseOfStudySelectItems = Utils.distinctBy(
      Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.COURSES_OF_STUDY], []),
      p => p.displayText
    );
    this.teachingSubjectSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.TEACHING_SUBJECTS], []);
    this.learningFrameworksSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.LEARNING_FXS], []);
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
    this.serviceSchemeSelectItems.push(RegistrationFilterViewModel.NOT_APPLICABLE_ITEM);
    this.developmentalRoleSelectItems.push(RegistrationFilterViewModel.NOT_APPLICABLE_ITEM);
    this.teachingLevelSelectItems.push(RegistrationFilterViewModel.NOT_APPLICABLE_ITEM);
    this.teachingCourseOfStudySelectItems.push(RegistrationFilterViewModel.NOT_APPLICABLE_ITEM);
    this.teachingSubjectSelectItems.push(RegistrationFilterViewModel.NOT_APPLICABLE_ITEM);
    this.learningFrameworksSelectItems.push(RegistrationFilterViewModel.NOT_APPLICABLE_ITEM);
    this.designationSelectItems.push(RegistrationFilterViewModel.NOT_APPLICABLE_ITEM_DESIGNATION);
  }
}

function create(
  getOrganisationUnitIdsFn: (ids: number[]) => Observable<DepartmentInfoModel[]>,
  metadataTags: MetadataTagModel[] = [],
  designations: Designation[] = [],
  data: RegistrationFilterModel = new RegistrationFilterModel()
): Observable<RegistrationFilterViewModel> {
  const organisationUnitObs = data.department && data.department.length > 0 ? getOrganisationUnitIdsFn(data.department) : of([]);
  return combineLatest(organisationUnitObs).pipe(
    map(([organisationUnits]) => {
      return new RegistrationFilterViewModel(metadataTags, designations, organisationUnits, data);
    })
  );
}
