import {
  ColumnApi,
  GridApi,
  GridOptions,
  RowNode,
} from '@ag-grid-community/core';
import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  HostListener,
  OnInit,
} from '@angular/core';
import {
  CxConfirmationDialogComponent,
  CxGlobalLoaderService,
  debounce,
} from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from 'app-auth/auth.service';
import { IDictionary } from 'app-models/dictionary';
import { PagingResponseModel } from 'app-models/user-management.model';
import { PdEvaluationDialogComponent } from 'app/individual-development/shared/pd-evalution-dialog/pd-evaluation-dialog.component';
import { AppConstant } from 'app/shared/app.constant';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import { CommonHelpers } from 'app/shared/helpers/common.helpers';
import { isEmpty } from 'lodash';
import { ToastrService } from 'ngx-toastr';
import { ClassRegistrationFilterFormComponent } from './filter-form/class-registration/class-registration-filter-form.component';
import { ClassrunChangeRequestFilterFormComponent } from './filter-form/classrun-change/classrun-change-filter-form.component';
import { WithrawalRequestFilterFormComponent } from './filter-form/withrawal-request/withrawal-request-filter-form.component';
import { ApprovalAgGridHelper } from './helpers/approval-ag-grid-helper';
import { ApprovalConstants } from './helpers/approval-page.constant';
import { ApprovalPageHelper } from './helpers/approval-page.helper';
import { ApprovalTargetEnum } from './models/approval.enum';
import {
  ClassRegistrationStatusEnum,
  RegistrationModel,
  WithrawalStatusEnum,
} from './models/class-registration.model';
import { ApprovalPageService } from './services/approval-page.service';
import { FilterSlidebarService } from './services/filter-slidebar.service';

@Component({
  selector: 'approval-page',
  templateUrl: './approval-page.component.html',
  styleUrls: ['./approval-page.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApprovalPageComponent
  extends BaseScreenComponent
  implements OnInit {
  // Main variables
  approvalItems: any[];
  approvalTarget: ApprovalTargetEnum;

  // Secondary variables
  gridApi: GridApi;
  gridColumnApi: any;
  gridOptions: GridOptions;
  rowSelection: string;
  selectedNodesBackup: RowNode[];
  approveButtonText: string = 'Common.Action.Approve';

  // Paging variable
  pagedData: PagingResponseModel<any>;
  totalItems: number = 0;
  currentPageIndex: number = 0;
  currentPageSize: number = AppConstant.ItemPerPage;
  defaultPageSize: number = AppConstant.ItemPerPage;

  // Flag variables
  rerender: boolean = false;
  hasReviewPermisson: boolean = false;
  hasSelectedPendingItems: boolean = false;

  // Filter variable
  currentParam: IDictionary<unknown>;
  showFilter: boolean = false;

  constructor(
    protected authService: AuthService,
    protected changeDetectorRef: ChangeDetectorRef,
    private approvalPageService: ApprovalPageService,
    private filterSlidebarService: FilterSlidebarService,
    private globalLoader: CxGlobalLoaderService,
    private toastrService: ToastrService,
    private ngbModal: NgbModal
  ) {
    super(changeDetectorRef, authService);
  }

  ngOnInit(): void {
    window.scroll(0, 0);
    this.subscribeFilterEvent();
    window.addEventListener('scroll', CommonHelpers.freezeAgGridHeader(), true);
  }

  subscribeFilterEvent(): void {
    this.subscriptionAdder = this.filterSlidebarService.onSubmitFilter.subscribe(
      async (params) => {
        this.currentParam = params;
        await this.getGridData(this.approvalTarget, params);
      }
    );
  }

  @HostListener('window:resize') // tslint:disable-next-line:no-magic-numbers
  @debounce(100)
  onResize(): void {
    if (!this.gridApi) {
      return;
    }
    this.gridApi.sizeColumnsToFit();
  }

  onGridReady(params: { api: GridApi; columnApi: ColumnApi }): void {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
    this.gridApi.setDomLayout('autoHeight');
    this.gridApi.sizeColumnsToFit();
    this.getGridData(this.approvalTarget);
  }

  onChangeApprovalTarget(target: ApprovalTargetEnum): void {
    this.approvalTarget = target;
    this.updateApproveButtonText();
    this.updateGridRenderer();
    this.updateFilterForm();
    this.hasReviewPermisson = ApprovalPageHelper.hasReviewPermisson(
      this.approvalTarget,
      this.currentUser
    );
  }

  onClickedApprove(): void {
    this.confirmApproveRequest();
  }

  onClickedReject(): void {
    if (this.isNominationsTarget) {
      this.confirmRejectNominationRequest();
    } else {
      this.evaluateRejection();
    }
  }

  onSelectionChanged(): void {
    const selectedNodes = this.gridApi.getSelectedNodes();
    this.checkAndHandleSingleSelectForSpecialTarget(selectedNodes);
    this.selectedNodesBackup = selectedNodes;
    const existSelectedItem = this.gridApi.getSelectedRows();
    this.detectCanApproveReject(existSelectedItem);
  }

  onCurrentPageChange(pageIndex: number): void {
    this.currentPageIndex = pageIndex - 1;
    this.getGridData(this.approvalTarget, this.currentParam);
    window.scroll(0, 0);
  }

  onPageSizeChange(pageSize: number): void {
    this.currentPageIndex = 0;
    this.currentPageSize = pageSize;
    this.getGridData(this.approvalTarget, this.currentParam);
    window.scroll(0, 0);
  }

  get isNominationsTarget(): boolean {
    return (
      ApprovalConstants.NOMINATIONS_TARGET_ENUMS.includes(
        this.approvalTarget
      ) ||
      ApprovalConstants.ADHOC_NOMINATIONS_TARGET_ENUMS.includes(
        this.approvalTarget
      )
    );
  }

  /**
   * Check if user can approve/reject or not.
   */
  private detectCanApproveReject(selectedItems: any[]): void {
    if (isEmpty(selectedItems)) {
      this.hasSelectedPendingItems = false;

      return;
    }
    const findNotPendingItem = (
      statusField: string,
      pendingEnum: ClassRegistrationStatusEnum | WithrawalStatusEnum
    ) => {
      return selectedItems.find((item: RegistrationModel) => {
        return item[statusField] !== pendingEnum;
      });
    };

    switch (this.approvalTarget) {
      case ApprovalTargetEnum.ClassRegistration:
        // Do not allow approve/reject if there is at least a selected item which not in pending status.
        const notPendingRegistrationItem = findNotPendingItem(
          'registrationStatus',
          ClassRegistrationStatusEnum.PendingConfirmation
        );

        this.hasSelectedPendingItems = notPendingRegistrationItem === undefined;
        break;
      case ApprovalTargetEnum.ClassWidthdrawal:
        // Do not allow approve/reject if there is at least a selected item which not in pending status.
        const notPendingWithrawItem = findNotPendingItem(
          'withdrawalStatus',
          WithrawalStatusEnum.PendingConfirmation
        );

        this.hasSelectedPendingItems = notPendingWithrawItem === undefined;
        break;
      case ApprovalTargetEnum.ClassChangeRequest:
        this.hasSelectedPendingItems = true;
        // Do not allow approve/reject if there is at least a selected item which not in pending status.
        const notPendingClassRunChangeItem = findNotPendingItem(
          'classRunChangeStatus',
          ClassRegistrationStatusEnum.PendingConfirmation
        );

        this.hasSelectedPendingItems =
          notPendingClassRunChangeItem === undefined;
        break;
      default:
        this.hasSelectedPendingItems = true;
        break;
    }
  }

  private resetVariable(): void {
    this.pagedData = undefined;
    this.totalItems = 0;
    this.currentPageIndex = 0;
    this.currentPageSize = this.defaultPageSize;
    this.rerender = false;
    this.hasSelectedPendingItems = false;
    this.hasReviewPermisson = false;
    this.gridOptions = undefined;
  }

  private async getGridData(
    approvalTarget: ApprovalTargetEnum,
    filterParam: IDictionary<unknown> = {}
  ): Promise<void> {
    this.globalLoader.showLoader();
    this.gridApi.showLoadingOverlay();
    this.pagedData = await this.approvalPageService.getGridDataAsync(
      approvalTarget,
      this.currentPageIndex,
      this.currentPageSize,
      filterParam
    );

    this.globalLoader.hideLoader();
    this.gridApi.hideOverlay();

    if (this.pagedData && !isEmpty(this.pagedData.items)) {
      this.approvalItems = this.pagedData.items;
      this.gridApi.setRowData(this.approvalItems);
      this.totalItems = this.pagedData.totalItems;
    } else {
      this.gridApi.setRowData([]);
      this.gridApi.showNoRowsOverlay();
      this.totalItems = 0;
    }

    this.changeDetectorRef.detectChanges();
  }

  private updateGridRenderer(): void {
    this.resetVariable();
    this.rerender = true;
    this.changeDetectorRef.detectChanges();
    setTimeout(() => {
      const options = ApprovalAgGridHelper.getGridOptions(
        this.approvalTarget,
        this
      );
      this.gridOptions = options;
      this.rerender = false;
      this.changeDetectorRef.detectChanges();
    });
  }

  private updateFilterForm(): void {
    // Set showFilter and call detectChanges to trigger destroy filter slidebar
    this.showFilter = false;
    this.currentParam = null;
    this.changeDetectorRef.detectChanges();

    // Then check filter form again.
    switch (this.approvalTarget) {
      case ApprovalTargetEnum.ClassRegistration:
        this.showFilter = true;
        this.filterSlidebarService.initSlidebar(
          ClassRegistrationFilterFormComponent
        );
        break;
      case ApprovalTargetEnum.ClassWidthdrawal:
        this.showFilter = true;
        this.filterSlidebarService.initSlidebar(
          WithrawalRequestFilterFormComponent
        );
        break;
      case ApprovalTargetEnum.ClassChangeRequest:
        this.showFilter = true;
        this.filterSlidebarService.initSlidebar(
          ClassrunChangeRequestFilterFormComponent
        );
        break;
      default:
        this.showFilter = false;
        break;
    }
    this.changeDetectorRef.detectChanges();
  }

  private updateApproveButtonText(): void {
    switch (this.approvalTarget) {
      case ApprovalTargetEnum.LNA:
      case ApprovalTargetEnum.PDPlan:
        this.approveButtonText = 'Common.Action.Acknowledge';
        break;
      default:
        this.approveButtonText = 'Common.Action.Approve';
        break;
    }
  }

  private evaluateRejection(): void {
    if (this.ngbModal.hasOpenModals()) {
      this.ngbModal.dismissAll();
    }

    const modalRef = this.ngbModal.open(PdEvaluationDialogComponent, {
      centered: true,
      size: 'lg',
    });
    const modalRefComponentInstance = modalRef.componentInstance as PdEvaluationDialogComponent;
    modalRefComponentInstance.header = ApprovalPageHelper.getRejectConfirmMessage(
      this.approvalTarget
    );

    modalRefComponentInstance.doneButtonText = 'Submit';
    this.subscriptionAdder = modalRefComponentInstance.done.subscribe(
      async (reason: string) => {
        await this.performReject(reason);
        modalRef.close();
      }
    );
    this.subscriptionAdder = modalRefComponentInstance.cancel.subscribe(() =>
      modalRef.close()
    );
  }

  private confirmApproveRequest(): void {
    this.showModalConfirm(
      'Confirmation',
      ApprovalPageHelper.getApproveConfirmMessage(this.approvalTarget),
      this.performApprove
    );
  }

  private confirmRejectNominationRequest(): void {
    this.showModalConfirm(
      'Confirmation',
      'Reject selected nomination request(s)?',
      this.performReject
    );
  }

  private showModalConfirm(
    confirmTitle: string,
    confirmContent: string,
    confirmCallback: (reason?: string) => Promise<void>,
    cancelCallback: () => unknown = null
  ): void {
    if (this.ngbModal.hasOpenModals()) {
      this.ngbModal.dismissAll();
    }

    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      size: 'sm',
      centered: true,
    });

    const modalComponent = modalRef.componentInstance as CxConfirmationDialogComponent;
    modalComponent.cancelButtonText = 'Cancel';
    modalComponent.confirmButtonText = 'Confirm';
    modalComponent.header = confirmTitle;
    modalComponent.content = confirmContent;

    modalComponent.cancel.subscribe(() => {
      if (typeof cancelCallback === 'function') {
        cancelCallback.apply(this);
      }
      modalRef.close();
    });
    modalComponent.confirm.subscribe((rejectReason?: string) => {
      confirmCallback.apply(this, rejectReason);
      modalRef.close();
    });
  }

  private async performApprove(): Promise<void> {
    const items: any[] = this.gridApi.getSelectedRows();
    if (isEmpty(items)) {
      return;
    }
    this.globalLoader.showLoader();
    const ids = this.getItemIds(items);
    const result = await this.approvalPageService.approveRequest(
      ids,
      this.approvalTarget
    );

    let message: string = '';
    if (result) {
      await this.getGridData(this.approvalTarget);
      this.onSelectionChanged();

      if (!this.isNominationsTarget) {
        message = this.approvalPageService.getApproveSuccessMessage(
          this.approvalTarget
        );

        this.toastrService.success(message);
      }
    } else if (!this.isNominationsTarget) {
      message = this.approvalPageService.getApproveFailMessage(
        this.approvalTarget
      );
      this.toastrService.error(message);
    }

    this.globalLoader.hideLoader();
  }

  private async performReject(reason?: string): Promise<void> {
    const items: any[] = this.gridApi.getSelectedRows();
    if (isEmpty(items)) {
      return;
    }
    this.globalLoader.showLoader();
    const ids = this.getItemIds(items);
    const result = await this.approvalPageService.rejectRequest(
      ids,
      this.approvalTarget,
      reason
    );

    let message: string = '';
    if (result) {
      await this.getGridData(this.approvalTarget);
      this.onSelectionChanged();

      if (!this.isNominationsTarget) {
        message = this.approvalPageService.getRejectSuccessMessage(
          this.approvalTarget
        );
        this.toastrService.success(message);
      }
    } else if (!this.isNominationsTarget) {
      message = this.approvalPageService.getRejectFailMessage(
        this.approvalTarget
      );
      this.toastrService.error(message);
    }

    this.globalLoader.hideLoader();
  }

  private getItemIds(items: any[]): any[] {
    switch (this.approvalTarget) {
      case ApprovalTargetEnum.LearningDirection:
      case ApprovalTargetEnum.LearningPlan:
      case ApprovalTargetEnum.LNA:
      case ApprovalTargetEnum.PDPlan:
      case ApprovalTargetEnum.SelfAssignPDO:
        return items.map((item) => item.identity);
      default:
        return items.map((item) => item.id);
    }
  }

  private checkAndHandleSingleSelectForSpecialTarget(
    selectedNodes: RowNode[]
  ): void {
    if (
      ApprovalConstants.RESTRICT_SINGLE_SELECT_TARGET_ENUMS.includes(
        this.approvalTarget
      ) &&
      selectedNodes &&
      selectedNodes.length > 1
    ) {
      if (selectedNodes.length > this.selectedNodesBackup.length) {
        const resultNodes = selectedNodes.filter(
          (node) =>
            this.selectedNodesBackup.findIndex(
              (item) => item.id === node.id
            ) === -1
        );
        this.gridApi.deselectAll();
        resultNodes[resultNodes.length - 1].setSelected(true);
      }
    }
  }
}
