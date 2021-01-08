import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import {
  CourseCriteriaPlaceOfWorkType,
  DepartmentInfoModel,
  DepartmentLevelModel,
  LearningCatalogRepository,
  MetadataCodingScheme,
  MetadataId,
  MetadataTagModel,
  OrganizationRepository,
  TypeOfOrganization,
  organizationUnitLevelConst
} from '@opal20/domain-api';

import { CheckedState } from '@progress/kendo-angular-treeview';
import { CourseCriteriaDetailComponent } from './course-criteria-detail.component';
import { CourseCriteriaDetailMode } from '../../models/course-criteria-detail-mode.model';
import { CourseCriteriaDetailViewModel } from './../../view-models/course-criteria-detail-view.model';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/internal/operators/map';

@Component({
  selector: 'learner-related-criteria-tab',
  templateUrl: './learner-related-criteria-tab.component.html'
})
export class LearnerRelatedCriteriaTabComponent extends BaseComponent {
  @Input() public form: FormGroup;

  public get courseCriteria(): CourseCriteriaDetailViewModel {
    return this._courseCriteria;
  }
  @Input()
  public set courseCriteria(v: CourseCriteriaDetailViewModel) {
    this._courseCriteria = v;
  }
  @Input() public mode: CourseCriteriaDetailMode | undefined;

  public placeOfWorkTitleDic = {
    [CourseCriteriaPlaceOfWorkType.DepartmentUnitTypes]: 'Organisation type',
    [CourseCriteriaPlaceOfWorkType.DepartmentLevelTypes]: 'Organisation level',
    [CourseCriteriaPlaceOfWorkType.SpecificDepartments]: 'Specific organisation'
  };
  public CourseCriteriaPlaceOfWorkType: typeof CourseCriteriaPlaceOfWorkType = CourseCriteriaPlaceOfWorkType;
  public MetadataCodingScheme: typeof MetadataCodingScheme = MetadataCodingScheme;
  public CourseCriteriaDetailMode: typeof CourseCriteriaDetailMode = CourseCriteriaDetailMode;
  public MetadataId: typeof MetadataId = MetadataId;
  public fetchDepartmentSelectItemFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]>;
  public fetchOrganizationUnitSelectItemFn: (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<TypeOfOrganization[]>;
  public fetchOrganizationLevelSelectItemFn: (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<DepartmentLevelModel[]>;
  public developmentalRolesItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public tracksItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public teachingCourseStudysItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public teachingLevelsItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public jobFamilysItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public easSubstantiveGradeBandingItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;

  private _courseCriteria: CourseCriteriaDetailViewModel = new CourseCriteriaDetailViewModel();

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private organizationRepository: OrganizationRepository,
    private learningRepository: LearningCatalogRepository
  ) {
    super(moduleFacadeService);
    this.fetchDepartmentSelectItemFn = this._createFetchDepartmentSelectItemFn();
    this.fetchOrganizationUnitSelectItemFn = this._createFetchOrganizationUnitSelectItemFn();
    this.fetchOrganizationLevelSelectItemFn = this._createFetchOrganizationLevelSelectItemFn();
    this.developmentalRolesItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.courseCriteria.devRoles;
    });

    this.easSubstantiveGradeBandingItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.courseCriteria.subGradeBanding;
    });

    this.tracksItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.courseCriteria.tracks;
    });

    this.teachingCourseStudysItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.courseCriteria.teachingCourseOfStudy;
    });

    this.teachingLevelsItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.courseCriteria.teachingLevels;
    });

    this.jobFamilysItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.courseCriteria.jobFamily;
    });
  }

  public asViewMode(): boolean {
    return CourseCriteriaDetailComponent.asViewMode(this.mode);
  }

  public tagTvItemIsCheckedFnFactory(checkedKeysFn: () => string[]): (dataItem: MetadataTagModel, index: string) => CheckedState {
    return (dataItem: MetadataTagModel, index: string) => {
      if (
        checkedKeysFn().indexOf(dataItem.tagId) > -1 ||
        (dataItem.childs !== undefined &&
          dataItem.childs.length > 0 &&
          Utils.includesAll(checkedKeysFn(), dataItem.childs.map(p => p.tagId)))
      ) {
        return 'checked';
      }

      if (this.tagTvItemIsIndeterminate(dataItem.childs, checkedKeysFn)) {
        return 'indeterminate';
      }

      return 'none';
    };
  }

  private tagTvItemIsIndeterminate(itemChilds: MetadataTagModel[] | undefined, checkedKeysFn: () => string[]): boolean {
    if (itemChilds === undefined) {
      return false;
    }
    let idx = 0;
    let item: MetadataTagModel;
    const checkKeysDic = Utils.toDictionary(checkedKeysFn());

    while ((item = itemChilds[idx])) {
      if (checkKeysDic[item.tagId] != null || this.tagTvItemIsIndeterminate(item.childs, checkedKeysFn)) {
        return true;
      }

      idx += 1;
    }

    return false;
  }

  private _createFetchOrganizationUnitSelectItemFn(): (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<TypeOfOrganization[]> {
    return (_searchText, _skipCount, _maxResultCount) => {
      return this.learningRepository.loadUserTypeOfOrganizationList(false).pipe(map(_ => _));
    };
  }

  private _createFetchOrganizationLevelSelectItemFn(): (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<DepartmentLevelModel[]> {
    return (_searchText, _skipCount, _maxResultCount) => {
      return this.organizationRepository.loadOrganizationalLevels(false).pipe(map(_ => _));
    };
  }

  private _createFetchDepartmentSelectItemFn(): (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<DepartmentInfoModel[]> {
    return (_searchText, _skipCount, _maxResultCount) => {
      return this.organizationRepository
        .loadDepartmentInfoList(
          {
            departmentId: 1,
            includeChildren: true,
            includeDepartmentType: true,
            departmentTypeIds: [
              organizationUnitLevelConst.Division,
              organizationUnitLevelConst.Branch,
              organizationUnitLevelConst.Cluster,
              organizationUnitLevelConst.School
            ]
          },
          false
        )
        .pipe(
          map(_ => {
            return _;
          })
        );
    };
  }
}
