import { ChangeDetectorRef, Component, ViewEncapsulation } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'app-auth/auth.service';
import { ClassRunModel } from 'app-models/classrun.model';
import { IDictionary } from 'app-models/dictionary';
import { PDCatalogSearchResult } from 'app-models/pdcatalog/pdcatalog.dto';
import { WithrawalStatusEnum } from 'app/approval-page/models/class-registration.model';
import { CourseFilterService } from 'app/approval-page/services/course-filter.service';
import { FilterSlidebarService } from 'app/approval-page/services/filter-slidebar.service';
import { UserFilterService } from 'app/approval-page/services/user-filter.services';
import {
  CxSelectConfigModel,
  CxSelectItemModel,
} from 'app/shared/components/cx-select/cx-select.model';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { Observable, of } from 'rxjs';
import { BaseFilterFormComponent, IFilterForm } from '../filter-form';

@Component({
  selector: 'withrawal-request-filter-form',
  templateUrl: './withrawal-request-filter-form.component.html',
  styleUrls: ['./withrawal-request-filter-form.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class WithrawalRequestFilterFormComponent
  extends BaseFilterFormComponent
  implements IFilterForm {
  //#region Filter model
  public withrawalRequestStatuses: CxSelectItemModel<WithrawalStatusEnum>[] = [];
  public courseSelected: CxSelectItemModel<PDCatalogSearchResult>;
  public learnersSelected: CxSelectItemModel<Staff>[] = [];
  public classRunsSelected: CxSelectItemModel<any>[] = [];
  public withdrawalStartDate: Date;
  public withdrawalEndDate: Date;
  public currentFilter: any = {
    learnersSelected: [],
    withrawalRequestStatuses: [],
    classRunsSelected: [],
    courseSelected: null,
    withdrawalStartDate: null,
    withdrawalEndDate: null,
  };

  //#endregion End filter model

  //#region Dropdown Configuration
  public learnerSelectorConfig: CxSelectConfigModel = new CxSelectConfigModel({
    placeholder: this.translateComponentText('SelectLearnerPlaceholher'),
    searchText: this.translateComponentText('LearnerSearchText'),
    hideSelected: false,
    clearable: false,
    multiple: true,
    autoLoadMore: true,
    pageNumberStartAt: 0,
  });

  public courseSelectorConfig: CxSelectConfigModel = new CxSelectConfigModel({
    placeholder: this.translateComponentText('SelectCoursePlaceholher'),
    searchText: this.translateComponentText('CourseSearchText'),
    searchable: true,
    multiple: false,
    hideSelected: false,
    clearable: true,
    closeOnSelect: true,
    autoLoadMore: true,
  });

  public classRunsItems: CxSelectItemModel<ClassRunModel>[] = [];
  public classRunSelectorConfig: CxSelectConfigModel = new CxSelectConfigModel({
    placeholder: this.translateComponentText('SelectClassRunPlaceholher'),
    searchable: false,
    multiple: true,
    hideSelected: false,
    clearable: true,
  });

  public withrawalStatusSelectorConfig: CxSelectConfigModel = new CxSelectConfigModel(
    {
      placeholder: this.translateComponentText('SelectStatusPlaceholher'),
      searchable: false,
      multiple: true,
      hideSelected: false,
      clearable: true,
    }
  );

  private statusMapping: IDictionary<WithrawalStatusEnum[]> = {
    [WithrawalStatusEnum.PendingConfirmation]: [
      WithrawalStatusEnum.PendingConfirmation,
    ],
    [WithrawalStatusEnum.Approved]: [WithrawalStatusEnum.Approved],
    [WithrawalStatusEnum.Rejected]: [
      WithrawalStatusEnum.Rejected,
      WithrawalStatusEnum.RejectedByCA,
    ],
  };

  //#endregion Dropdown Configuration

  constructor(
    protected filterSlidebarService: FilterSlidebarService,
    protected changeDetectorRef: ChangeDetectorRef,
    private translateService: TranslateService,
    private userFilterService: UserFilterService,
    private authService: AuthService,
    private courseFilterService: CourseFilterService
  ) {
    super(filterSlidebarService, changeDetectorRef);
  }

  public withrawalStatusObs: () => Observable<
    CxSelectItemModel<WithrawalStatusEnum>[]
  > = () => {
    const dropdownItems = [
      WithrawalStatusEnum.PendingConfirmation,
      WithrawalStatusEnum.Approved,
      WithrawalStatusEnum.Rejected,
      // WithrawalStatusEnum.RejectedByCA,
    ].map((item) => {
      return new CxSelectItemModel<WithrawalStatusEnum>({
        id: item,
        primaryField: this.translateWithrawalStatus(item),
      });
    });

    return of(dropdownItems);
  };

  public filterCourses: (
    searchText?: string,
    pageIndex?: number
  ) => Observable<CxSelectItemModel<PDCatalogSearchResult>[]> = (
    searchText?: string,
    pageIndex?: number
  ) => {
    return this.courseFilterService.filterCourses(searchText, pageIndex);
  };

  public updateClassRunItems(): void {
    this.classRunsItems = [];

    if (this.courseSelected) {
      this.courseFilterService
        .getClassRunsByCourse(this.courseSelected.id)
        .subscribe((classRuns) => {
          this.classRunsItems = classRuns;
          this.changeDetectorRef.detectChanges();
        });
    }
  }

  public classRunsObs: () => Observable<
    CxSelectItemModel<ClassRunModel>[]
  > = () => {
    return of(this.classRunsItems);
  };

  public filterLearners: (
    searchText?: string,
    pageIndex?: number
  ) => Observable<CxSelectItemModel<Staff>[]> = (
    searchText?: string,
    pageIndex?: number
  ) =>
    this.userFilterService.filterLearners(
      searchText,
      this.authService.userDepartmentId,
      pageIndex
    );

  public getFilterParam(): IDictionary<unknown> {
    const learnerIds = this.learnersSelected
      .filter((item) => item.dataObject && item.dataObject.identity)
      .map((item) => item.dataObject.identity.extId);

    const classRunsIds = this.classRunsSelected.map((item) => item.id);

    const selectedStatuses = this.withrawalRequestStatuses.map(
      (item) => item.id
    );
    let filterStatuses = [];
    for (const status of selectedStatuses) {
      filterStatuses = filterStatuses.concat(this.statusMapping[status]);
    }

    const getDayByHour = (
      date: Date,
      hour: number,
      min: number,
      sec: number
    ) => {
      return date
        ? new Date(new Date(date).setHours(hour, min, sec))
        : undefined;
    };

    return {
      withdrawalStatuses: filterStatuses.length ? filterStatuses : undefined,
      withdrawalStartDate: getDayByHour(this.withdrawalStartDate, 0, 0, 1),
      withdrawalEndDate: getDayByHour(this.withdrawalEndDate, 23, 59, 59),
      learnerIds: learnerIds.length ? learnerIds : undefined,
      courseId: this.courseSelected ? this.courseSelected.id : undefined,
      classRunIds: classRunsIds.length ? classRunsIds : undefined,
    };
  }

  public onApprovingStartDateChanged(): void {
    if (
      this.withdrawalStartDate &&
      this.withdrawalEndDate &&
      this.withdrawalEndDate.getTime() < this.withdrawalStartDate.getTime()
    ) {
      this.withdrawalEndDate = null;
    }
  }

  public resetFilterForm(): void {
    this.learnersSelected = [];
    this.withrawalRequestStatuses = [];
    this.classRunsSelected = [];
    this.courseSelected = null;
    this.withdrawalStartDate = null;
    this.withdrawalEndDate = null;
    this.applyFilterForm();
  }

  public applyFilterForm(): void {
    this.currentFilter = {
      learnersSelected: this.learnersSelected,
      withrawalRequestStatuses: this.withrawalRequestStatuses,
      classRunsSelected: this.classRunsSelected,
      courseSelected: this.courseSelected,
      withdrawalStartDate: this.withdrawalStartDate,
      withdrawalEndDate: this.withdrawalEndDate,
    };
  }

  public getCurrentFilterValues(): void {
    super.getCurrentFilterValues();

    this.learnersSelected = this.currentFilter.learnersSelected;
    this.withrawalRequestStatuses = this.currentFilter.withrawalRequestStatuses;
    this.classRunsSelected = this.currentFilter.classRunsSelected;
    this.courseSelected = this.currentFilter.courseSelected;
    this.withdrawalStartDate = this.currentFilter.withdrawalStartDate;
    this.withdrawalEndDate = this.currentFilter.withdrawalEndDate;
  }

  private translateWithrawalStatus(status: WithrawalStatusEnum): string {
    return this.translateService.instant(
      'Common.StatusMapping.WithrawalRequest.' + status
    ) as string;
  }

  private translateComponentText(text: string): string {
    return this.translateService.instant(
      'ApprovalPage.FilterSlidebar.WithrawalRequest.' + text
    ) as string;
  }
}
