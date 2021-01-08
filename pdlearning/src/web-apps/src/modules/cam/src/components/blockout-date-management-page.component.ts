import {
  BasePageComponent,
  ComponentType,
  IGridFilter,
  ModuleFacadeService,
  NotificationType,
  TranslationMessage,
  Utils
} from '@opal20/infrastructure';
import {
  BlockoutDateDetailMode,
  BlockoutDateFilterComponent,
  BlockoutDateFilterModel,
  BlockoutDateViewModel,
  CAMRoutePaths,
  CAMTabConfiguration,
  ContextMenuAction,
  ContextMenuEmit,
  CoursePlanningCycleDetailMode,
  NavigationPageService,
  RouterPageInput
} from '@opal20/domain-components';
import {
  BlockoutDateModel,
  BlockoutDateRepository,
  CoursePlanningCycle,
  IConfirmBlockoutDateRequest,
  UserInfoModel
} from '@opal20/domain-api';
import { Component, Input } from '@angular/core';
import { ContextMenuItem, DialogAction, OpalDialogService } from '@opal20/common-components';

import { CoursePlanningCycleDetailPageInput } from '../models/course-planning-cycle-detail-input.model';
@Component({
  selector: 'blockout-date-management-page',
  templateUrl: './blockout-date-management-page.component.html'
})
export class BlockoutDateManagementPageComponent extends BasePageComponent {
  @Input() public set coursePlanningCycleDetailPageInput(
    coursePlanningCycleDetailPageInput:
      | RouterPageInput<CoursePlanningCycleDetailPageInput, CAMTabConfiguration, CAMTabConfiguration>
      | undefined
  ) {
    this._coursePlanningCycleDetailPageInput = coursePlanningCycleDetailPageInput;
  }
  @Input()
  public set coursePlanningCycle(v: CoursePlanningCycle | undefined) {
    if (Utils.isDifferent(this._coursePlanningCycle, v)) {
      this._coursePlanningCycle = v;
    }
  }
  @Input() public mode: BlockoutDateDetailMode;
  @Input() public stickyDependElement: HTMLElement;

  public get blockoutDate(): BlockoutDateModel | undefined {
    return this._blockoutDate;
  }
  public get coursePlanningCycleDetailPageInput():
    | RouterPageInput<CoursePlanningCycleDetailPageInput, CAMTabConfiguration, CAMTabConfiguration>
    | undefined {
    return this._coursePlanningCycleDetailPageInput;
  }
  public get coursePlanningCycle(): CoursePlanningCycle | undefined {
    return this._coursePlanningCycle;
  }

  public filterPopupContent: ComponentType<BlockoutDateFilterComponent> = BlockoutDateFilterComponent;
  public filterData: BlockoutDateFilterModel = null;
  public CAMTabConfiguration: typeof CAMTabConfiguration = CAMTabConfiguration;
  public searchText: string = '';
  public filter: IGridFilter = {
    search: '',
    filter: null
  };
  public contextMenuItemsForBlockoutDates: ContextMenuItem[] = [
    {
      id: ContextMenuAction.Delete,
      text: this.translateCommon('Delete'),
      icon: 'delete'
    }
  ];

  private _coursePlanningCycleDetailPageInput:
    | RouterPageInput<CoursePlanningCycleDetailPageInput, CAMTabConfiguration, CAMTabConfiguration>
    | undefined;
  private _blockoutDate: BlockoutDateModel;
  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();
  private _coursePlanningCycle: CoursePlanningCycle;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private opalDialogService: OpalDialogService,
    public navigationPageService: NavigationPageService,
    private blockoutDateRepo: BlockoutDateRepository
  ) {
    super(moduleFacadeService);
  }

  public onApplyFilter(data: BlockoutDateFilterModel): void {
    this.filterData = data;

    this.filter = {
      ...this.filter,
      filter: this.filterData ? this.filterData.convert() : null
    };
  }

  public canCreateBlockoutDate(): boolean {
    return (
      this.currentUser &&
      BlockoutDateModel.haveCudPermission(this.currentUser) &&
      this.coursePlanningCycle.isConfirmedBlockoutDate === false
    );
  }

  public canConfirmBlockoutDate(): boolean {
    return (
      this.currentUser &&
      BlockoutDateModel.haveCudPermission(this.currentUser) &&
      this.coursePlanningCycle.isConfirmedBlockoutDate === false
    );
  }

  public onCreateBlockoutDate(): void {
    this.navigationPageService.navigateTo(
      CAMRoutePaths.BlockoutDateDetailPage,
      {
        activeTab: CAMTabConfiguration.BlockoutDateTab,
        data: {
          id: null,
          mode: BlockoutDateDetailMode.NewBlockoutDate,
          coursePlanningCycleId: this.coursePlanningCycleDetailPageInput.data.id
        }
      },
      this.coursePlanningCycleDetailPageInput
    );
  }

  public onConfirmBlockoutDate(): void {
    const request: IConfirmBlockoutDateRequest = {
      CoursePlanningCycleId: this.coursePlanningCycleDetailPageInput.data.id
    };

    this.opalDialogService
      .openConfirmDialog({
        confirmTitle: 'Confirm',
        confirmMsgHtml:
          // tslint:disable-next-line:max-line-length
          'After confirm the list of block-out dates, you cannot add any of it into this planning cycle anymore and you also have to update the planning cycle with the information of planning period. Do you want to proceed ?',
        noBtnText: 'Cancel',
        yesBtnText: 'Yes, Proceed'
      })
      .subscribe(action => {
        if (action === DialogAction.Cancel) {
          this.navigationPageService.navigateBack();
        } else if (action === DialogAction.OK) {
          this.blockoutDateRepo
            .confirmBlockoutDate(request)
            .pipe(this.untilDestroy())
            .toPromise()
            .then(() => {
              this.navigationPageService.navigateTo(CAMRoutePaths.CoursePlanningCycleDetailPage, {
                activeTab: CAMTabConfiguration.CoursePlanningCycleInfoTab,
                data: {
                  id: this.coursePlanningCycle.id,
                  mode: CoursePlanningCycleDetailMode.Edit
                }
              });
              this.showNotification();
            });
        }
      });
  }

  public onViewBlockoutDate(dataItem: BlockoutDateViewModel): void {
    this.navigationPageService.navigateTo(
      CAMRoutePaths.BlockoutDateDetailPage,
      {
        activeTab: CAMTabConfiguration.BlockoutDateTab,
        data: {
          id: dataItem.id,
          mode: BlockoutDateDetailMode.View,
          coursePlanningCycleId: this.coursePlanningCycleDetailPageInput.data.id
        }
      },
      this.coursePlanningCycleDetailPageInput
    );
  }

  public onSubmitSearch(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText
    };
  }

  public onBlockoutDateGridContextMenuSelected(contextMenuEmit: ContextMenuEmit<BlockoutDateViewModel>): void {
    switch (contextMenuEmit.event.item.id) {
      case ContextMenuAction.Delete:
        this.deleteBlockoutDate(contextMenuEmit.dataItem);
        break;
    }
  }

  private deleteBlockoutDate(blockoutDate: BlockoutDateModel): void {
    this.modalService.showConfirmMessage(
      new TranslationMessage(this.moduleFacadeService.globalTranslator, 'Are you sure you want to delete this block-out date?'),
      () => {
        this.blockoutDateRepo.deleteBlockoutDate(blockoutDate.id).then(() => {
          this.showNotification('Deleted Successfully', NotificationType.Success);
        });
      }
    );
  }
}
