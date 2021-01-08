import { ChangeDetectorRef, Component, ViewEncapsulation } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { IDictionary } from 'app-models/dictionary';
import { MetadataTagModel } from 'app-services/pd-opportunity/metadata-tag.model';
import {
  MetadataTagGroupCode,
  PDOpportunityFilterModel,
} from 'app-services/pd-opportunity/pd-opportunity-filter.model';
import { PDOpportunityService } from 'app-services/pd-opportunity/pd-opportunity.service';
import {
  BaseFilterFormComponent,
  IFilterForm,
} from 'app/shared/components/catalog-filter-form/filter-form/filter-form';
import { FilterCatalogSlidebarService } from 'app/shared/components/catalog-filter-form/services/filter-catalog-slidebar.service';
// tslint:disable-next-line:max-line-length
import {
  CxSelectConfigModel,
  CxSelectItemModel,
} from 'app/shared/components/cx-select/cx-select.model';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'pdo-filter-form',
  templateUrl: './pdo-filter-form.component.html',
  styleUrls: ['./pdo-filter-form.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class PdoFilterFormComponent
  extends BaseFilterFormComponent
  implements IFilterForm {
  public metadataTagModel: PDOpportunityFilterModel;
  public showMore: boolean = false;
  public currentFilter: any = {
    pdActivityType: null,
    learningMode: null,
    categorySelectedItems: [],
    serviceSchemeSelectedItems: [],
    developmentalRoleSelectedItems: [],
    teachingLevelSelectedItems: [],
    courseLevel: null,
    subjectAreaSelectedItems: [],
    learningFrameworkSelectedItems: [],
    learningDimensionSelectedItems: [],
    learningAreaSelectedItems: [],
    learningSubAreaSelectedItems: [],
    natureOfCourse: null,
  };

  //#region filter config
  public multipleSelectorConfig: CxSelectConfigModel = new CxSelectConfigModel({
    searchable: false,
    multiple: true,
    hideSelected: false,
    clearable: true,
  });

  public singleSelectorConfig: CxSelectConfigModel = new CxSelectConfigModel({
    searchable: false,
    multiple: false,
    hideSelected: false,
    clearable: false,
  });

  public enumc: MetadataTagGroupCode;
  public metaDataMapping: any;
  public dataLoaded: any;
  public groupCodeEnum: any = MetadataTagGroupCode;

  //#endregion
  constructor(
    protected filterSlidebarService: FilterCatalogSlidebarService,
    protected changeDetectorRef: ChangeDetectorRef,
    private translateService: TranslateService,
    private pdOpportunityService: PDOpportunityService
  ) {
    super(filterSlidebarService, changeDetectorRef);
  }

  async ngOnInit(): Promise<void> {
    await this.getMetadataTags();
  }

  public async getMetadataTags(): Promise<void> {
    this.metadataTagModel = await this.pdOpportunityService.getAllMetaDataTagsAsync();
  }

  public getMetaDataTagObs(
    groupCode: MetadataTagGroupCode
  ): () => Observable<CxSelectItemModel<MetadataTagModel>[]> {
    this.metaDataMapping = this.getMetaDataMapping();
    const metaDatas: MetadataTagModel[] = this.metaDataMapping[groupCode];
    const dropdownItems = metaDatas.map((item) => {
      return new CxSelectItemModel<MetadataTagModel>({
        id: item.tagId,
        primaryField: item.name,
      });
    });

    return () => of(dropdownItems);
  }

  public getFilterParam(): IDictionary<unknown> {
    return {
      pdActivityType: this.metadataTagModel.pdActivityType
        ? this.metadataTagModel.pdActivityType.id
        : null,
      learningMode: this.metadataTagModel.learningMode
        ? this.metadataTagModel.learningMode.id
        : null,
      categorySelectedItems:
        this.metadataTagModel.categorySelectedItems.length > 0
          ? this.metadataTagModel.categorySelectedItems.map((p) => p.id)
          : null,
      serviceSchemeSelectedItems:
        this.metadataTagModel.serviceSchemeSelectedItems.length > 0
          ? this.metadataTagModel.serviceSchemeSelectedItems.map((p) => p.id)
          : null,
      developmentalRoleSelectedItems:
        this.metadataTagModel.developmentalRoleSelectedItems.length > 0
          ? this.metadataTagModel.developmentalRoleSelectedItems.map(
              (p) => p.id
            )
          : null,
      teachingLevelSelectedItems:
        this.metadataTagModel.teachingLevelSelectedItems.length > 0
          ? this.metadataTagModel.teachingLevelSelectedItems.map((p) => p.id)
          : null,
      courseLevel: this.metadataTagModel.courseLevel
        ? this.metadataTagModel.courseLevel.id
        : null,
      subjectAreaSelectedItems:
        this.metadataTagModel.subjectAreaSelectedItems.length > 0
          ? this.metadataTagModel.subjectAreaSelectedItems.map((p) => p.id)
          : null,
      learningFrameworkSelectedItems:
        this.metadataTagModel.learningFrameworkSelectedItems.length > 0
          ? this.metadataTagModel.learningFrameworkSelectedItems.map(
              (p) => p.id
            )
          : null,
      learningDimensionSelectedItems:
        this.metadataTagModel.learningDimensionSelectedItems.length > 0
          ? this.metadataTagModel.learningDimensionSelectedItems.map(
              (p) => p.id
            )
          : null,
      learningAreaSelectedItems:
        this.metadataTagModel.learningAreaSelectedItems.length > 0
          ? this.metadataTagModel.learningAreaSelectedItems.map((p) => p.id)
          : null,
      learningSubAreaSelectedItems:
        this.metadataTagModel.learningSubAreaSelectedItems.length > 0
          ? this.metadataTagModel.learningSubAreaSelectedItems.map((p) => p.id)
          : null,
      natureOfCourse: this.metadataTagModel.natureOfCourse
        ? this.metadataTagModel.natureOfCourse.id
        : null,
    };
  }

  public resetFilterForm(): void {
    this.metadataTagModel.pdActivityType = null;
    this.metadataTagModel.learningMode = null;
    this.metadataTagModel.categorySelectedItems = [];
    this.metadataTagModel.serviceSchemeSelectedItems = [];
    this.metadataTagModel.developmentalRoleSelectedItems = [];
    this.metadataTagModel.teachingLevelSelectedItems = [];
    this.metadataTagModel.courseLevel = null;
    this.metadataTagModel.subjectAreaSelectedItems = [];
    this.metadataTagModel.learningFrameworkSelectedItems = [];
    this.metadataTagModel.learningDimensionSelectedItems = [];
    this.metadataTagModel.learningAreaSelectedItems = [];
    this.metadataTagModel.learningSubAreaSelectedItems = [];
    this.metadataTagModel.natureOfCourse = null;
    // this.metadataTagModel.applyFilterForm();
  }

  public applyFilterForm(): void {
    this.currentFilter = {
      pdActivityType: this.metadataTagModel.pdActivityType
        ? this.metadataTagModel.pdActivityType.id
        : null,
      learningMode: this.metadataTagModel.learningMode
        ? this.metadataTagModel.learningMode.id
        : null,
      categorySelectedItems: this.metadataTagModel.categorySelectedItems.map(
        (p) => p.id
      ),
      serviceSchemeSelectedItems: this.metadataTagModel.serviceSchemeSelectedItems.map(
        (p) => p.id
      ),
      developmentalRoleSelectedItems: this.metadataTagModel.developmentalRoleSelectedItems.map(
        (p) => p.id
      ),
      teachingLevelSelectedItems: this.metadataTagModel.teachingLevelSelectedItems.map(
        (p) => p.id
      ),
      courseLevel: this.metadataTagModel.courseLevel
        ? this.metadataTagModel.courseLevel.id
        : null,
      subjectAreaSelectedItems: this.metadataTagModel.subjectAreaSelectedItems.map(
        (p) => p.id
      ),
      learningFrameworkSelectedItems: this.metadataTagModel.learningFrameworkSelectedItems.map(
        (p) => p.id
      ),
      learningDimensionSelectedItems: this.metadataTagModel.learningDimensionSelectedItems.map(
        (p) => p.id
      ),
      learningAreaSelectedItems: this.metadataTagModel.learningAreaSelectedItems.map(
        (p) => p.id
      ),
      learningSubAreaSelectedItems: this.metadataTagModel.learningSubAreaSelectedItems.map(
        (p) => p.id
      ),
      natureOfCourse: this.metadataTagModel.natureOfCourse
        ? this.metadataTagModel.natureOfCourse.id
        : null,
    };
  }

  public toggleShowMore(): void {
    this.showMore = !this.showMore;
  }

  public get metadataTagGroupCode(): typeof MetadataTagGroupCode {
    return MetadataTagGroupCode;
  }

  private translateComponentText(text: string): string {
    return this.translateService.instant(
      'ApprovalPage.FilterSlidebar.ClassRegistration.' + text
    ) as string;
  }

  private getMetaDataMapping(): any {
    return {
      [MetadataTagGroupCode.PDO_TYPES]: this.metadataTagModel.pdTypeSelectItems,
      [MetadataTagGroupCode.PDO_MODES]: this.metadataTagModel
        .modeOfLearningSelectItems,
      [MetadataTagGroupCode.PDO_CATEGORIES]: this.metadataTagModel
        .categorySelectItems,
      [MetadataTagGroupCode.SERVICE_SCHEMES]: this.metadataTagModel
        .serviceSchemeSelectItems,
      [MetadataTagGroupCode.DEVROLES]: this.metadataTagModel
        .developmentalRoleSelectItems,
      [MetadataTagGroupCode.TEACHING_LEVELS]: this.metadataTagModel
        .teachingLevelsSelectItems,
      [MetadataTagGroupCode.COURSE_LEVELS]: this.metadataTagModel
        .courseLevelSelectItems,
      [MetadataTagGroupCode.PDO_TAXONOMY]: this.metadataTagModel
        .subjectSelectItems,
      [MetadataTagGroupCode.LEARNING_FXS]: this.metadataTagModel
        .learningFrameworkSelectItems,
      [MetadataTagGroupCode.LEARNING_DIMENSION]: this.metadataTagModel
        .learningDimensionSelectItems,
      [MetadataTagGroupCode.LEARNING_AREA]: this.metadataTagModel
        .learningAreaSelectItems,
      [MetadataTagGroupCode.LEARNING_SUB_AREA]: this.metadataTagModel
        .learningSubAreaSelectItems,
      [MetadataTagGroupCode.PDO_NATURES]: this.metadataTagModel
        .natureCourseSelectItems,
    };
  }
}
