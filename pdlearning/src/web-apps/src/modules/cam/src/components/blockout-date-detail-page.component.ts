import {
  BLOCKOUT_DATE_STATUS_COLOR_MAP,
  BlockoutDateDetailMode,
  BlockoutDateDetailViewModel,
  BreadcrumbItem,
  BreadcrumbService,
  CAMRoutePaths,
  CAMTabConfiguration,
  ContextMenuAction,
  NavigationPageService,
  RouterPageInput
} from '@opal20/domain-components';
import {
  BaseFormComponent,
  IFormBuilderDefinition,
  IntervalScheduler,
  ModuleFacadeService,
  NotificationType,
  TranslationMessage,
  Utils
} from '@opal20/infrastructure';
import {
  BlockoutDateModel,
  BlockoutDateRepository,
  BlockoutDateStatus,
  CoursePlanningCycle,
  CoursePlanningCycleRepository,
  ISaveBlockoutDateRequest,
  TaggingRepository,
  UserInfoModel
} from '@opal20/domain-api';
import {
  ButtonGroupButton,
  futureDateValidator,
  ifValidator,
  requiredAndNoWhitespaceValidator,
  requiredForListValidator,
  startEndValidator,
  validateFutureDateType
} from '@opal20/common-components';
import { Observable, Subscription, combineLatest, from, of } from 'rxjs';

import { BlockoutDateDetailPageInput } from '../models/blockout-date-detail-input.model';
import { CAM_ROUTE_BREADCUMB_MAPPING_FN } from '../helpers/cam-route-breadcumb-mapping-fn';
import { Component } from '@angular/core';
import { SelectEvent } from '@progress/kendo-angular-layout';
import { Validators } from '@angular/forms';

@Component({
  selector: 'blockout-date-detail-page',
  templateUrl: './blockout-date-detail-page.component.html'
})
export class BlockoutDateDetailPageComponent extends BaseFormComponent {
  public get detailPageInput(): RouterPageInput<BlockoutDateDetailPageInput, CAMTabConfiguration, unknown> | undefined {
    return this._detailPageInput;
  }

  public set detailPageInput(v: RouterPageInput<BlockoutDateDetailPageInput, CAMTabConfiguration, unknown> | undefined) {
    if (Utils.isDifferent(this._detailPageInput, v) && v) {
      this._detailPageInput = v;
      if (this.initiated) {
        this.loadBlockouDateInfo();
      }
    }
  }
  public get title(): string {
    return this.blockoutDateDetailVM.data.title;
  }

  public CAMTabConfiguration: typeof CAMTabConfiguration = CAMTabConfiguration;

  public get blockoutDateDetailVM(): BlockoutDateDetailViewModel {
    return this._blockoutDateDetailVM;
  }
  public set blockoutDateDetailVM(v: BlockoutDateDetailViewModel) {
    this._blockoutDateDetailVM = v;
  }

  public blockoutDateStatusColorMap = BLOCKOUT_DATE_STATUS_COLOR_MAP;
  public breadCrumbItems: BreadcrumbItem[] = [];
  public loadingBlockoutDates: boolean = false;
  public BlockoutDateStatus: typeof BlockoutDateStatus = BlockoutDateStatus;

  public buttonGroup: Partial<ButtonGroupButton>[] = [
    {
      displayText: 'Edit',
      onClickFn: () => this.onEditBlockoutDate(),
      shownIfFn: () => this.canEditBlockoutDate()
    },
    {
      displayText: 'Save',
      onClickFn: () => this.onSaveClicked(),
      shownIfFn: () => this.canSaveBlockoutDate(),
      isDisabledFn: () => !this.dataHasChanged()
    },
    {
      displayText: 'Submit',
      onClickFn: () => this.onSubmitClicked(),
      shownIfFn: () => this.canSubmitBlockoutDate()
    },
    {
      id: ContextMenuAction.Delete,
      icon: 'delete',
      displayText: 'Delete',
      shownInMoreFn: () => true,
      onClickFn: () => this.onDeleteBlockoutDate(),
      shownIfFn: () => this.canDeleteBlockoutDate()
    }
  ];

  private _detailPageInput: RouterPageInput<BlockoutDateDetailPageInput, CAMTabConfiguration, unknown> = Navigator[
    CAMRoutePaths.BlockoutDateDetailPage
  ] as RouterPageInput<BlockoutDateDetailPageInput, CAMTabConfiguration, unknown>;

  private _loadBlockoutDateInfoSub: Subscription = new Subscription();
  private _blockoutDateDetailVM: BlockoutDateDetailViewModel = new BlockoutDateDetailViewModel();
  private coursePlanningCycle: CoursePlanningCycle = new CoursePlanningCycle();
  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();

  // Auto save after 30 minutes
  private scheduler: IntervalScheduler = new IntervalScheduler(600000, () => {
    if (this.dataHasChanged()) {
      this.saveDraftBlockoutDate();
    }
  });

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private blockoutDateRepository: BlockoutDateRepository,
    private taggingRepository: TaggingRepository,
    public navigationPageService: NavigationPageService,
    private breadcrumbService: BreadcrumbService,
    private coursePlanningRepository: CoursePlanningCycleRepository
  ) {
    super(moduleFacadeService);
  }

  public onTabSelected(event: SelectEvent): void {
    this.detailPageInput.activeTab = blockoutDateDetailPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.detailPageInput);
  }

  public onEditBlockoutDate(): void {
    this.detailPageInput.data.mode = BlockoutDateDetailMode.Edit;
  }

  public canEditBlockoutDate(): boolean {
    return this.detailPageInput.data.mode === BlockoutDateDetailMode.View && BlockoutDateModel.haveCudPermission(this.currentUser);
  }

  public onSaveClicked(): void {
    this.saveDraftBlockoutDate().subscribe(() => this.navigationPageService.navigateBack());
  }

  public saveDraftBlockoutDate(): Observable<void> {
    return from(
      this.saveBlockoutDate(BlockoutDateStatus.Draft).then(_ => {
        this.showNotification();
      })
    );
  }

  public canSaveBlockoutDate(): boolean {
    return (
      (this.detailPageInput.data.mode === BlockoutDateDetailMode.Edit ||
        this.detailPageInput.data.mode === BlockoutDateDetailMode.NewBlockoutDate) &&
      BlockoutDateModel.haveCudPermission(this.currentUser)
    );
  }

  public onSubmitClicked(): void {
    this.validateAndSaveBlockoutDate(BlockoutDateStatus.Active).subscribe(() => this.navigationPageService.navigateBack());
  }

  public canSubmitBlockoutDate(): boolean {
    return (
      this.detailPageInput.data.mode === BlockoutDateDetailMode.View &&
      this.blockoutDateDetailVM.data.status === BlockoutDateStatus.Draft &&
      BlockoutDateModel.haveCudPermission(this.currentUser) &&
      this.blockoutDateDetailVM.data.isConfirmed === false
    );
  }

  public onDeleteBlockoutDate(): void {
    this.modalService.showConfirmMessage(
      new TranslationMessage(this.moduleFacadeService.globalTranslator, 'Are you sure you want to delete this block-out date?'),
      () => {
        this.blockoutDateRepository.deleteBlockoutDate(this.detailPageInput.data.id).then(() => {
          this.showNotification('Deleted Successfully', NotificationType.Success);
          this.navigationPageService.navigateBack();
        });
      }
    );
  }

  public canDeleteBlockoutDate(): boolean {
    return (
      !this.blockoutDateDetailVM.data.isConfirmed &&
      this.detailPageInput.data.id !== null &&
      BlockoutDateModel.haveCudPermission(this.currentUser)
    );
  }

  public dataHasChanged(): boolean {
    return (
      this.blockoutDateDetailVM &&
      this.blockoutDateDetailVM.dataHasChanged() &&
      (this.detailPageInput.data.mode === BlockoutDateDetailMode.Edit ||
        this.detailPageInput.data.mode === BlockoutDateDetailMode.NewBlockoutDate)
    );
  }

  public loadBlockouDateInfo(): void {
    this._loadBlockoutDateInfoSub.unsubscribe();
    const coursPlanningObs: Observable<CoursePlanningCycle> = this.coursePlanningRepository.loadCoursePlanningCycleById(
      this.detailPageInput.data.coursePlanningCycleId
    );
    const blockoutDateObs: Observable<BlockoutDateModel | null> =
      this.detailPageInput.data.id != null ? this.blockoutDateRepository.loadBlockoutDate(this.detailPageInput.data.id) : of(null);
    const taggingObs = this.taggingRepository.loadAllMetaDataTags();
    this.loadingBlockoutDates = true;
    this._loadBlockoutDateInfoSub = combineLatest(blockoutDateObs, taggingObs, coursPlanningObs)
      .pipe(this.untilDestroy())
      .subscribe(
        ([blockoutDateResult, taggingResult, coursePlanningCycle]) => {
          if (blockoutDateResult == null) {
            blockoutDateResult = new BlockoutDateModel();
            blockoutDateResult.validYear = coursePlanningCycle.yearCycle;
          }
          this.blockoutDateDetailVM = new BlockoutDateDetailViewModel(blockoutDateResult, taggingResult);
          this.coursePlanningCycle = coursePlanningCycle;
          this.loadBreadcrumb();
          this.loadingBlockoutDates = false;
        },
        error => {
          this.loadingBlockoutDates = false;
        },
        () => {
          this.loadingBlockoutDates = false;
        }
      );
  }

  public validateAndSaveBlockoutDate(status: BlockoutDateStatus): Observable<void> {
    const mode = this.detailPageInput.data.mode;
    if (!(mode === BlockoutDateDetailMode.NewBlockoutDate || mode === BlockoutDateDetailMode.Edit)) {
      return of();
    }

    return from(
      new Promise<void>((resolve, reject) => {
        this.validate().then(valid => {
          if (valid) {
            this.saveBlockoutDate(status).then(_ => {
              this.showNotification('Saved Successfully');
              resolve();
            }, reject);
          } else {
            reject();
          }
        });
      })
    );
  }

  public onClickBack(): void {
    this.navigationPageService.navigateBack(
      () => this.dataHasChanged(),
      () => this.validateAndSaveBlockoutDate(this.blockoutDateDetailVM.status)
    );
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      // Overview Info
      formName: 'form',
      validateByGroupControlNames: [['startDateTime', 'endDateTime']],
      controls: {
        startDateTime: {
          defaultValue: this.blockoutDateDetailVM.startDateTime,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            },
            {
              validator: ifValidator(p => Utils.isDifferent(this.blockoutDateDetailVM.startDateTime, p.value), () => futureDateValidator()),
              validatorType: validateFutureDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start Date cannot be in the past')
            },
            {
              validator: startEndValidator(
                'blockoutDateStartDateWithEndDate',
                p => p.value,
                p => this.blockoutDateDetailVM.endDateTime,
                true,
                'dateOnly'
              ),
              validatorType: 'blockoutDateStartDateWithEndDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start Date cannot be larger than End Date')
            }
          ]
        },
        endDateTime: {
          defaultValue: this.blockoutDateDetailVM.endDateTime,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            },
            {
              validator: ifValidator(p => Utils.isDifferent(this.blockoutDateDetailVM.endDateTime, p.value), () => futureDateValidator()),
              validatorType: validateFutureDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End Date cannot be in the past')
            },
            {
              validator: startEndValidator(
                'blockoutDateEndDate',
                p => this.blockoutDateDetailVM.startDateTime,
                p => p.value,
                true,
                'dateOnly'
              ),
              validatorType: 'blockoutDateEndDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End Date cannot be less than Start Date')
            }
          ]
        },
        title: {
          defaultValue: this.blockoutDateDetailVM.title,
          validators: [
            {
              validator: requiredAndNoWhitespaceValidator(),
              validatorType: 'required'
            },
            {
              validator: Validators.maxLength(2000)
            }
          ]
        },
        description: {
          defaultValue: this.blockoutDateDetailVM.description,
          validators: null
        },
        serviceSchemes: {
          defaultValue: this.blockoutDateDetailVM.serviceSchemes,
          validators: [
            {
              validator: requiredForListValidator(),
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Please select at least one service scheme')
            }
          ]
        }
      }
    };
  }

  protected onInit(): void {
    this.getNavigatePageData();
    this.loadBlockouDateInfo();
    this.scheduler.init();
  }

  protected onDestroy(): void {
    this.scheduler.destroy();
  }

  private getNavigatePageData(): void {
    const navigateData: RouterPageInput<BlockoutDateDetailPageInput, CAMTabConfiguration, CAMTabConfiguration> = this.getNavigateData();
    if (navigateData) {
      this.detailPageInput = navigateData;
    } else {
      this.navigationPageService.returnHome();
    }
  }

  private saveBlockoutDate(status: BlockoutDateStatus): Promise<BlockoutDateModel> {
    const request: ISaveBlockoutDateRequest = {
      data: {
        startDay: this.blockoutDateDetailVM.startDateTime.getDate(),
        endMonth: this.blockoutDateDetailVM.endDateTime.getMonth() + 1,
        endDay: this.blockoutDateDetailVM.endDateTime.getDate(),
        startMonth: this.blockoutDateDetailVM.startDateTime.getMonth() + 1,
        validYear: this.blockoutDateDetailVM.startDateTime.getFullYear(),
        serviceSchemes: this.blockoutDateDetailVM.data.serviceSchemes,
        planningCycleId: this.detailPageInput.data.coursePlanningCycleId,
        id: this.blockoutDateDetailVM.data.id,
        description: this.blockoutDateDetailVM.data.description,
        title: this.blockoutDateDetailVM.data.title,
        status: this.blockoutDateDetailVM.data.status
      }
    };

    return this.blockoutDateRepository
      .saveBlockoutDate(request)
      .pipe(this.untilDestroy())
      .toPromise();
  }

  private loadBreadcrumb(): void {
    this.breadCrumbItems = this.breadcrumbService.loadBreadcrumbTab(
      this.detailPageInput,
      CAM_ROUTE_BREADCUMB_MAPPING_FN(
        this.detailPageInput,
        p =>
          this.navigationPageService.navigateByRouter(
            p,
            () => this.dataHasChanged(),
            () => this.validateAndSaveBlockoutDate(this.blockoutDateDetailVM.status)
          ),
        {
          [CAMRoutePaths.CoursePlanningCycleDetailPage]: { textFn: () => this.coursePlanningCycle.title },
          [CAMRoutePaths.BlockoutDateDetailPage]: { textFn: () => this.blockoutDateDetailVM.title }
        }
      )
    );
  }
}

export const blockoutDateDetailPageTabIndexMap = {
  0: CAMTabConfiguration.AllBlockoutDateTab
};
