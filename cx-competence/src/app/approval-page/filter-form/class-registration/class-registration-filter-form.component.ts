import { ChangeDetectorRef, Component, ViewEncapsulation } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'app-auth/auth.service';
import { ClassRunModel } from 'app-models/classrun.model';
import { IDictionary } from 'app-models/dictionary';
import { PDCatalogSearchResult } from 'app-models/pdcatalog/pdcatalog.dto';
import { ClassRegistrationStatusEnum } from 'app/approval-page/models/class-registration.model';
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
  selector: 'class-registration-filter-form',
  templateUrl: './class-registration-filter-form.component.html',
  styleUrls: ['./class-registration-filter-form.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class ClassRegistrationFilterFormComponent
  extends BaseFilterFormComponent
  implements IFilterForm {
  //#region Filter model
  public registrationStatuses: CxSelectItemModel<ClassRegistrationStatusEnum>[] = [];
  public courseSelected: CxSelectItemModel<PDCatalogSearchResult>;
  public learnersSelected: CxSelectItemModel<Staff>[] = [];
  public classRunsSelected: CxSelectItemModel<any>[] = [];
  public registrationStartDate: Date;
  public registrationEndDate: Date;
  public currentFilter: any = {
    registrationStatuses: [],
    learnersSelected: [],
    classRunsSelected: [],
    registrationStartDate: null,
    registrationEndDate: null,
    courseSelected: null,
  };

  //#endregion End filter model

  //#region Dropdown Configuration
  public learnerSelectorConfig: CxSelectConfigModel = new CxSelectConfigModel({
    placeholder: this.translateComponentText('SelectLearnerPlaceholher'),
    searchText: this.translateComponentText('LearnerSearchText'),
    hideSelected: false,
    clearable: true,
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

  public registrationStatusSelectorConfig: CxSelectConfigModel = new CxSelectConfigModel(
    {
      placeholder: this.translateComponentText('SelectStatusPlaceholher'),
      searchable: false,
      multiple: true,
      hideSelected: false,
      clearable: true,
    }
  );

  private statusMapping: IDictionary<ClassRegistrationStatusEnum[]> = {
    [ClassRegistrationStatusEnum.PendingConfirmation]: [
      ClassRegistrationStatusEnum.PendingConfirmation,
    ],
    [ClassRegistrationStatusEnum.Approved]: [
      ClassRegistrationStatusEnum.Approved,
      ClassRegistrationStatusEnum.ConfirmedByCA,
    ],
    [ClassRegistrationStatusEnum.Rejected]: [
      ClassRegistrationStatusEnum.Rejected,
      ClassRegistrationStatusEnum.RejectedByCA,
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

  public registrationStatusObs: () => Observable<
    CxSelectItemModel<ClassRegistrationStatusEnum>[]
  > = () => {
    const dropdownItems = [
      ClassRegistrationStatusEnum.PendingConfirmation,
      ClassRegistrationStatusEnum.Approved,
      ClassRegistrationStatusEnum.Rejected,
      // ClassRegistrationStatusEnum.ConfirmedByCA,
      // ClassRegistrationStatusEnum.RejectedByCA,
      // ClassRegistrationStatusEnum.WaitlistPendingApprovalByLearner,
      // ClassRegistrationStatusEnum.WaitlistConfirmed,
      // ClassRegistrationStatusEnum.WaitlistRejected,
      // ClassRegistrationStatusEnum.OfferPendingApprovalByLearner,
      // ClassRegistrationStatusEnum.OfferRejected,
      // ClassRegistrationStatusEnum.OfferConfirmed,
      // ClassRegistrationStatusEnum.AddedByCAConflict,
      // ClassRegistrationStatusEnum.AddedByCAClassfull,
      // ClassRegistrationStatusEnum.OfferExpired,
    ].map((item) => {
      return new CxSelectItemModel<ClassRegistrationStatusEnum>({
        id: item,
        primaryField: this.translateRegistrationStatus(item),
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
    const selectedStatuses = this.registrationStatuses.map((item) => item.id);
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
      registrationStatuses: filterStatuses.length ? filterStatuses : undefined,
      registrationStartDate: getDayByHour(this.registrationStartDate, 0, 0, 1),
      registrationEndDate: getDayByHour(this.registrationEndDate, 23, 59, 59),
      learnerIds: learnerIds.length ? learnerIds : undefined,
      courseId: this.courseSelected ? this.courseSelected.id : undefined,
      classRunIds: classRunsIds.length ? classRunsIds : undefined,
    };
  }

  public onregistrationStartDateChanged(): void {
    if (
      this.registrationStartDate &&
      this.registrationEndDate &&
      this.registrationEndDate.getTime() < this.registrationStartDate.getTime()
    ) {
      this.registrationEndDate = null;
    }
  }

  public resetFilterForm(): void {
    this.learnersSelected = [];
    this.registrationStatuses = [];
    this.classRunsSelected = [];
    this.courseSelected = null;
    this.registrationStartDate = null;
    this.registrationEndDate = null;
    this.applyFilterForm();
  }

  public applyFilterForm(): void {
    this.currentFilter = {
      registrationStatuses: this.registrationStatuses,
      registrationStartDate: this.registrationStartDate,
      registrationEndDate: this.registrationEndDate,
      learnersSelected: this.learnersSelected,
      courseSelected: this.courseSelected,
      classRunsSelected: this.classRunsSelected,
    };
  }

  public getCurrentFilterValues(): void {
    super.getCurrentFilterValues();

    this.registrationStatuses = this.currentFilter.registrationStatuses;
    this.registrationStartDate = this.currentFilter.registrationStartDate;
    this.registrationEndDate = this.currentFilter.registrationEndDate;
    this.learnersSelected = this.currentFilter.learnersSelected;
    this.courseSelected = this.currentFilter.courseSelected;
    this.classRunsSelected = this.currentFilter.classRunsSelected;
  }

  private translateRegistrationStatus(
    status: ClassRegistrationStatusEnum
  ): string {
    return this.translateService.instant(
      'Common.StatusMapping.ClassRegistration.' + status
    ) as string;
  }

  private translateComponentText(text: string): string {
    return this.translateService.instant(
      'ApprovalPage.FilterSlidebar.ClassRegistration.' + text
    ) as string;
  }
}
