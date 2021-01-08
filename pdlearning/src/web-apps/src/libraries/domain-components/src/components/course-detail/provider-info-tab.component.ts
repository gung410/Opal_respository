import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import {
  DepartmentInfoModel,
  MetadataId,
  OrganizationRepository,
  OrganizationUnitLevelEnum,
  OtherTrainingAgencyReasonType,
  TrainingAgencyType,
  UserInfoModel,
  UserRepository,
  otherTrainingAgencyReasonDic
} from '@opal20/domain-api';

import { CourseDetailComponent } from './course-detail.component';
import { CourseDetailMode } from '../../models/course-detail-mode.model';
import { CourseDetailViewModel } from '../../view-models/course-detail-view.model';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'provider-info-tab',
  templateUrl: './provider-info-tab.component.html'
})
export class ProviderInfoTabComponent extends BaseComponent {
  @Input() public form: FormGroup;
  @Input() public course: CourseDetailViewModel;
  @Input() public mode: CourseDetailMode | undefined;
  public fetchOwnerDivisionFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> = null;
  public fetchPartnerOrganizationFn: (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<DepartmentInfoModel[]> = null;

  public fetchOwnerBranchFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> = null;
  public fetchMOEOfficersFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<UserInfoModel[]> = null;
  public otherTrainingAgencyReasonDic: Dictionary<string> = otherTrainingAgencyReasonDic;

  public MetadataId: typeof MetadataId = MetadataId;
  public CourseDetailMode: typeof CourseDetailMode = CourseDetailMode;
  public TrainingAgencyType: typeof TrainingAgencyType = TrainingAgencyType;
  public OtherTrainingAgencyReasonType: typeof OtherTrainingAgencyReasonType = OtherTrainingAgencyReasonType;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public userRepository: UserRepository,
    public organizationRepository: OrganizationRepository
  ) {
    super(moduleFacadeService);
    this.fetchOwnerBranchFn = this._createFetchOwnerBranchFn();
    this.fetchMOEOfficersFn = this._createFetchMOEOfficersFn();
    this.fetchPartnerOrganizationFn = this._createFetchPartnerOrganisationFn();
    this.fetchOwnerDivisionFn = this._createFetchDivisionFn();
  }

  public asViewMode(): boolean {
    return CourseDetailComponent.asViewMode(this.mode);
  }

  public isPlanningVerificationRequired(): boolean {
    return CourseDetailComponent.isPlanningVerificationRequired(this.course);
  }

  public asViewModeForCompletingCourseForPlanning(): boolean {
    return CourseDetailComponent.asViewModeForCompletingCourseForPlanning(this.course);
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

  private _createFetchPartnerOrganisationFn(): (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<DepartmentInfoModel[]> {
    return (searchText, skipCount, maxResultCount) =>
      this.organizationRepository.loadOrganizationalUnits(
        searchText,
        null,
        null,
        true,
        true,
        null,
        false,
        maxResultCount === 0 ? 1 : skipCount / maxResultCount + 1,
        maxResultCount,
        false
      );
  }

  private _createFetchOwnerBranchFn(): (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<DepartmentInfoModel[]> {
    return (searchText, skipCount, maxResultCount) =>
      this.organizationRepository
        .loadOrganizationalUnits(
          searchText,
          [OrganizationUnitLevelEnum.Branch],
          this.course.ownerDivisionIds,
          true,
          null,
          null,
          false,
          maxResultCount === 0 ? 1 : skipCount / maxResultCount + 1,
          maxResultCount,
          false
        )
        .pipe(
          map(result => {
            if (this.course.ownerDivisionIds.length > 0) {
              return result;
            }
            return [];
          })
        );
  }

  private _createFetchMOEOfficersFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<UserInfoModel[]> {
    return (searchText, skipCount, maxResultCount) =>
      this.userRepository.loadMOEOfficers(this.course.ownerDivisionIds, true, false).pipe(this.untilDestroy());
  }
}
