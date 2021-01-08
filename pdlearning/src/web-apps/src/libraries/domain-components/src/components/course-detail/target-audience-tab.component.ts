import { BaseComponent, MAX_INT, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import {
  Course,
  CourseRepository,
  DepartmentInfoModel,
  MetadataId,
  MetadataTagModel,
  OrganizationRepository,
  OrganizationUnitLevelEnum,
  PlaceOfWorkType,
  SearchCourseType,
  SystemRoleEnum
} from '@opal20/domain-api';

import { CheckedState } from '@progress/kendo-angular-treeview';
import { CourseDetailComponent } from './course-detail.component';
import { CourseDetailMode } from '../../models/course-detail-mode.model';
import { CourseDetailViewModel } from '../../view-models/course-detail-view.model';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'target-audience-tab',
  templateUrl: './target-audience-tab.component.html'
})
export class TargetAudienceTabComponent extends BaseComponent {
  @Input() public form: FormGroup;
  @Input() public course: CourseDetailViewModel;
  @Input() public mode: CourseDetailMode | undefined;
  public fetchCourseByIdsFn: (ids: string[]) => Observable<Course[]>;
  public fetchPrerequisiteCourseFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<Course[]>;
  public fetchDivisionFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> = null;
  public developmentalRolesItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public easSubstantiveGradeBandingItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public tracksItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public teachingCourseStudysItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public teachingLevelsItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public jobFamilysItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;

  public fetchBranchFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> = null;
  public fetchZoneFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> = null;
  public fetchClusterFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> = null;
  public fetchSchoolFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> = null;

  public placeOfWorkTitleDic = {
    [PlaceOfWorkType.ApplicableForEveryone]: 'Applicable for everyone',
    [PlaceOfWorkType.ApplicableForUsersInSpecificOrganisation]: 'Applicable for users in specific organisation(s)'
  };

  public CourseDetailMode: typeof CourseDetailMode = CourseDetailMode;
  public PlaceOfWorkType: typeof PlaceOfWorkType = PlaceOfWorkType;
  public MetadataId: typeof MetadataId = MetadataId;
  public maxInt: number = MAX_INT;
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public organizationRepository: OrganizationRepository,
    private courseRepository: CourseRepository
  ) {
    super(moduleFacadeService);
    this.fetchCourseByIdsFn = this._createFetchCourseByIdsFn();
    this.fetchPrerequisiteCourseFn = this._createFetchPrerequisiteCourseFn();
    this.fetchDivisionFn = this._createFetchDivisionFn();
    this.fetchBranchFn = this._createFetchBranchFn();
    this.fetchZoneFn = this._createFetchZoneFn();
    this.fetchClusterFn = this._createClusterFn();
    this.fetchSchoolFn = this._createSchoolFn();
    this.developmentalRolesItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.course.developmentalRoleIds;
    });

    this.easSubstantiveGradeBandingItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.course.easSubstantiveGradeBandingIds;
    });

    this.tracksItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.course.trackIds;
    });

    this.teachingCourseStudysItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.course.teachingCourseStudyIds;
    });

    this.teachingLevelsItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.course.teachingLevels;
    });

    this.jobFamilysItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.course.jobFamily;
    });
  }

  public asViewMode(): boolean {
    return CourseDetailComponent.asViewMode(this.mode);
  }

  public isPlanningVerificationRequired(): boolean {
    return CourseDetailComponent.isPlanningVerificationRequired(this.course);
  }

  public canViewFieldOfCourseInPlanningCycle(): boolean {
    return CourseDetailComponent.canViewFieldOfCourseInPlanningCycle(this.course);
  }

  public asViewModeForCompletingCourseForPlanning(): boolean {
    return CourseDetailComponent.asViewModeForCompletingCourseForPlanning(this.course);
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

  private _createFetchPrerequisiteCourseFn(
    inRoles?: SystemRoleEnum[]
  ): (searchText: string, skipCount: number, maxResultCount: number) => Observable<Course[]> {
    return (_searchText, _skipCount, _maxResultCount) => {
      return this.courseRepository
        .loadSearchCourses(_searchText, null, SearchCourseType.Prerequisite, null, _skipCount, _maxResultCount, null, null, false)
        .pipe(map(_ => _.items));
    };
  }

  private _createFetchCourseByIdsFn(): (ids: string[]) => Observable<Course[]> {
    return ids => {
      return this.courseRepository.loadCourses(ids, false);
    };
  }

  private _createFetchDivisionFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> {
    return (searchText, skipCount, maxResultCount) =>
      this.organizationRepository.loadOrganizationalUnits(
        searchText,
        [OrganizationUnitLevelEnum.Division],
        null,
        true,
        null,
        null,
        false,
        maxResultCount === 0 ? 1 : skipCount / maxResultCount + 1,
        maxResultCount,
        false
      );
  }

  private _createFetchBranchFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> {
    return (searchText, skipCount, maxResultCount) =>
      this.organizationRepository.loadOrganizationalUnits(
        searchText,
        [OrganizationUnitLevelEnum.Branch],
        null,
        true,
        null,
        null,
        false,
        maxResultCount === 0 ? 1 : skipCount / maxResultCount + 1,
        maxResultCount,
        false
      );
  }

  private _createFetchZoneFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> {
    return (searchText, skipCount, maxResultCount) =>
      this.organizationRepository.loadOrganizationalUnits(
        searchText,
        [OrganizationUnitLevelEnum.Branch],
        null,
        true,
        null,
        '10000666',
        false,
        maxResultCount === 0 ? 1 : skipCount / maxResultCount + 1,
        maxResultCount,
        false
      );
  }

  private _createClusterFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> {
    return (searchText, skipCount, maxResultCount) =>
      this.organizationRepository.loadOrganizationalUnits(
        searchText,
        [OrganizationUnitLevelEnum.Cluster],
        null,
        true,
        null,
        null,
        false,
        maxResultCount === 0 ? 1 : skipCount / maxResultCount + 1,
        maxResultCount,
        false
      );
  }

  private _createSchoolFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> {
    return (searchText, skipCount, maxResultCount) =>
      this.organizationRepository.loadOrganizationalUnits(
        searchText,
        [OrganizationUnitLevelEnum.School],
        null,
        true,
        null,
        null,
        false,
        maxResultCount === 0 ? 1 : skipCount / maxResultCount + 1,
        maxResultCount,
        false
      );
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
}
