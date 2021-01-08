import {
  BaseFormComponent,
  DateUtils,
  IFormBuilderDefinition,
  IntervalScheduler,
  ModuleFacadeService,
  TranslationMessage,
  Utils
} from '@opal20/infrastructure';
import {
  BreadcrumbItem,
  BreadcrumbService,
  CAMRoutePaths,
  CAMTabConfiguration,
  COURSE_PLANNING_CYCLE_STATUS_COLOR_MAP,
  CoursePlanningCycleDetailMode,
  CoursePlanningCycleDetailViewModel,
  ListCoursePlanningCycleGridComponentService,
  NavigationData,
  NavigationPageService,
  RouterPageInput
} from '@opal20/domain-components';
import {
  ButtonAction,
  ButtonGroupButton,
  SPACING_CONTENT,
  futureDateValidator,
  ifValidator,
  requiredAndNoWhitespaceValidator,
  requiredIfValidator,
  startEndValidator,
  validateFutureDateType
} from '@opal20/common-components';
import { Component, HostBinding, ViewChild } from '@angular/core';
import { CoursePlanningCycle, CoursePlanningCycleRepository, ISaveCoursePlanningCycleRequest, UserInfoModel } from '@opal20/domain-api';
import { Observable, Subscription, combineLatest, from, of } from 'rxjs';
import { SelectEvent, TabStripComponent } from '@progress/kendo-angular-layout';

import { CAM_ROUTE_BREADCUMB_MAPPING_FN } from '../helpers/cam-route-breadcumb-mapping-fn';
import { CoursePlanningCycleDetailPageInput } from '../models/course-planning-cycle-detail-input.model';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { NAVIGATORS } from '../cam.config';

@Component({
  selector: 'course-planning-cycle-detail-page',
  templateUrl: './course-planning-cycle-detail-page.component.html'
})
export class CoursePlanningCycleDetailPageComponent extends BaseFormComponent {
  @ViewChild(TabStripComponent, { static: false }) public tabStrip: TabStripComponent;
  public breadCrumbItems: BreadcrumbItem[] = [];
  public actionBtnGroup: ButtonAction<unknown>[] = [];
  public stickySpacing: number = SPACING_CONTENT;
  public get title(): string {
    return this.coursePlanningCycle.title;
  }

  public get detailPageInput(): RouterPageInput<CoursePlanningCycleDetailPageInput, CAMTabConfiguration, CAMTabConfiguration> | undefined {
    return this._detailPageInput;
  }

  public set detailPageInput(v: RouterPageInput<CoursePlanningCycleDetailPageInput, CAMTabConfiguration, CAMTabConfiguration> | undefined) {
    if (Utils.isDifferent(this._detailPageInput, v) && v) {
      this._detailPageInput = v;
      if (this.initiated) {
        this.loadCoursePlanningCycleInfo();
      }
    }
  }

  public get coursePlanningCycle(): CoursePlanningCycleDetailViewModel {
    return this._coursePlanningCycle;
  }
  public set coursePlanningCycle(v: CoursePlanningCycleDetailViewModel) {
    this._coursePlanningCycle = v;
  }

  public get statusColorMap(): unknown {
    return COURSE_PLANNING_CYCLE_STATUS_COLOR_MAP;
  }

  public navigationData: NavigationData;
  public loadingData: boolean = false;
  public CAMTabConfiguration: typeof CAMTabConfiguration = CAMTabConfiguration;
  public buttonGroup: Partial<ButtonGroupButton>[] = [
    {
      displayText: 'Edit',
      onClickFn: () => this.onEditCoursePlanningCycle(),
      shownIfFn: () => this.showEditCoursePlanningCycle()
    },
    {
      displayText: 'Save',
      onClickFn: () => this.onSaveCoursePlanningCycle(),
      shownIfFn: () => this.showSubmitCoursePlanningCycle(),
      isDisabledFn: () => !this.dataHasChanged()
    }
  ];
  private _detailPageInput: RouterPageInput<CoursePlanningCycleDetailPageInput, CAMTabConfiguration, CAMTabConfiguration> = NAVIGATORS[
    CAMRoutePaths.CoursePlanningCycleDetailPage
  ] as RouterPageInput<CoursePlanningCycleDetailPageInput, CAMTabConfiguration, CAMTabConfiguration>;
  private _loadSessionInfoSub: Subscription = new Subscription();
  private _coursePlanningCycle: CoursePlanningCycleDetailViewModel = new CoursePlanningCycleDetailViewModel();
  private currentUser = UserInfoModel.getMyUserInfo();

  // Auto save after 30 minutes
  private scheduler: IntervalScheduler = new IntervalScheduler(600000, () => {
    if (this.dataHasChanged()) {
      this.onSaveCoursePlanningCycle();
    }
  });

  public get selectedTab(): CAMTabConfiguration {
    return this.detailPageInput.activeTab != null ? this.detailPageInput.activeTab : CAMTabConfiguration.CoursePlanningCycleInfoTab;
  }

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private navigationPageService: NavigationPageService,
    private coursePlanningCycleRepository: CoursePlanningCycleRepository,
    private breadcrumbService: BreadcrumbService,
    private listCoursePlanningCycleGridSvc: ListCoursePlanningCycleGridComponentService
  ) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public onTabSelected(event: SelectEvent): void {
    this.detailPageInput.activeTab = coursePlanningCycleDetailPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.detailPageInput);
  }

  public loadCoursePlanningCycleInfo(): void {
    this._loadSessionInfoSub.unsubscribe();
    const coursePlanningCycleObs: Observable<CoursePlanningCycle | null> =
      this.detailPageInput.data && this.detailPageInput.data.id != null
        ? this.coursePlanningCycleRepository.loadCoursePlanningCycleById(this.detailPageInput.data.id)
        : of(null);
    const listCoursePlanningCycleObs: Observable<GridDataResult | null> = this.listCoursePlanningCycleGridSvc.loadCoursePlanningCycles(
      '',
      0,
      null
    );
    this.loadingData = true;
    this._loadSessionInfoSub = combineLatest(coursePlanningCycleObs, listCoursePlanningCycleObs)
      .pipe(this.untilDestroy())
      .subscribe(
        ([coursePlanningCycle, gridData]) => {
          this.coursePlanningCycle = new CoursePlanningCycleDetailViewModel(coursePlanningCycle, gridData.data);
          this.loadBreadcrumb();
          this.loadingData = false;
        },
        () => {
          this.loadingData = false;
        }
      );
  }

  public canViewCourses(): boolean {
    return this.detailPageInput.data && this.coursePlanningCycle.isConfirmedBlockoutDate;
  }

  public canViewBlockoutDates(): boolean {
    return this.detailPageInput.data.id != null;
  }

  public onClickBack(): void {
    this.navigationPageService.navigateBack(() => this.dataHasChanged(), () => this.validateAndSaveCoursePlanningCycle());
  }

  public showSubmitCoursePlanningCycle(): boolean {
    return (
      this.detailPageInput.data &&
      (this.detailPageInput.data.mode === CoursePlanningCycleDetailMode.Edit ||
        this.detailPageInput.data.mode === CoursePlanningCycleDetailMode.NewPlanningCycle) &&
      this.selectedTab === CAMTabConfiguration.CoursePlanningCycleInfoTab
    );
  }

  public showEditCoursePlanningCycle(): boolean {
    return (
      this.detailPageInput.data &&
      this.detailPageInput.data.mode === CoursePlanningCycleDetailMode.View &&
      CoursePlanningCycle.canVerifyCourse(this.currentUser) &&
      this.selectedTab === CAMTabConfiguration.CoursePlanningCycleInfoTab
    );
  }

  public onEditCoursePlanningCycle(): void {
    this.detailPageInput.data.mode = CoursePlanningCycleDetailMode.Edit;
    this.tabStrip.selectTab(0);
  }

  public onSaveCoursePlanningCycle(): void {
    this.validateAndSaveCoursePlanningCycle().subscribe(() => this.navigationPageService.navigateBack());
  }

  public validateAndSaveCoursePlanningCycle(): Observable<void> {
    return from(
      new Promise<void>((resolve, reject) => {
        this.validate().then(valid => {
          if (valid) {
            this.saveCoursePlanningCycle().then(_ => {
              this.showNotification();
              resolve();
            }, reject);
          } else {
            reject();
          }
        });
      })
    );
  }

  public dataHasChanged(): boolean {
    return this.coursePlanningCycle && this.coursePlanningCycle.dataHasChanged();
  }

  protected onInit(): void {
    this.getNavigatePageData();
    this.loadCoursePlanningCycleInfo();
    this.scheduler.init();
  }

  protected onDestroy(): void {
    this.scheduler.destroy();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'course-planning-cycle-detail',
      validateByGroupControlNames: [['startDate', 'endDate']],
      controls: {
        title: {
          defaultValue: null,
          validators: [
            {
              validator: requiredAndNoWhitespaceValidator(),
              validatorType: 'required'
            }
          ]
        },
        startDate: {
          defaultValue: null,
          validators: [
            {
              validator: requiredIfValidator(p => this.coursePlanningCycle.canEditStartDate()),
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start Date of Planning is mandatory field')
            },
            {
              validator: ifValidator(p => this.coursePlanningCycle.canEditStartDate(), () => futureDateValidator()),
              validatorType: validateFutureDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start Date of Planning cannot be in the past')
            },
            {
              validator: ifValidator(
                p => this.coursePlanningCycle.canEditStartDate(),
                () =>
                  startEndValidator('coursePlanningCycleStartDate', p => p.value, p => this.coursePlanningCycle.endDate, true, 'dateOnly')
              ),
              validatorType: 'coursePlanningCycleStartDate',
              message: new TranslationMessage(
                this.moduleFacadeService.translator,
                'Start Date of Planning cannot be greater than End Date of Planning'
              )
            }
          ]
        },
        endDate: {
          defaultValue: null,
          validators: [
            {
              validator: requiredIfValidator(p => this.coursePlanningCycle.isConfirmedBlockoutDate),
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End Date of Planning is mandatory field')
            },
            {
              validator: futureDateValidator(),
              validatorType: validateFutureDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End Date of Planning cannot be in the past')
            },
            {
              validator: startEndValidator(
                'coursePlanningCycleEndDate',
                p => this.coursePlanningCycle.startDate,
                p => p.value,
                true,
                'dateOnly'
              ),
              validatorType: 'coursePlanningCycleEndDate',
              message: new TranslationMessage(
                this.moduleFacadeService.translator,
                'End Date of Planning cannot be less than Start Date of Planning'
              )
            }
          ]
        },
        yearCycle: {
          defaultValue: null,
          validators: [
            {
              validator: requiredIfValidator(
                p => this.detailPageInput.data && this.detailPageInput.data.mode === CoursePlanningCycleDetailMode.NewPlanningCycle
              )
            }
          ]
        },
        description: {
          defaultValue: null,
          validators: null
        }
      }
    };
  }

  private getNavigatePageData(): void {
    const navigateData: RouterPageInput<
      CoursePlanningCycleDetailPageInput,
      CAMTabConfiguration,
      CAMTabConfiguration
    > = this.getNavigateData();
    if (navigateData) {
      this.detailPageInput = navigateData;
    } else {
      this.navigationPageService.returnHome();
    }
  }

  private saveCoursePlanningCycle(): Promise<CoursePlanningCycle> {
    return new Promise((resolve, reject) => {
      const request: ISaveCoursePlanningCycleRequest = {
        data: Utils.clone(this.coursePlanningCycle.coursePlanningCycleData, cloneData => {
          cloneData.startDate = cloneData.startDate ? DateUtils.removeTime(cloneData.startDate) : null;
          cloneData.endDate = cloneData.endDate ? DateUtils.setTimeToEndInDay(cloneData.endDate) : null;
        })
      };
      this.coursePlanningCycleRepository
        .saveCoursePlanningCycle(request)
        .pipe(this.untilDestroy())
        .subscribe(coursePlanningCycle => {
          resolve(coursePlanningCycle);
        }, reject);
    });
  }

  private loadBreadcrumb(): void {
    this.breadCrumbItems = this.breadcrumbService.loadBreadcrumbTab(
      this.detailPageInput,
      CAM_ROUTE_BREADCUMB_MAPPING_FN(
        this.detailPageInput,
        p => this.navigationPageService.navigateByRouter(p, () => this.dataHasChanged(), () => this.validateAndSaveCoursePlanningCycle()),
        {
          [CAMRoutePaths.CoursePlanningCycleDetailPage]: { textFn: () => this.coursePlanningCycle.title }
        }
      )
    );
  }
}

export const coursePlanningCycleDetailPageTabIndexMap = {
  0: CAMTabConfiguration.CoursePlanningCycleInfoTab,
  1: CAMTabConfiguration.CoursesOfPlanningCycleTab,
  2: CAMTabConfiguration.BlockoutDateTab
};
