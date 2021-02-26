import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  OnInit,
  ViewEncapsulation
} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTabChangeEvent } from '@angular/material/tabs';
import {
  ActionsModel,
  CxGlobalLoaderService
} from '@conexus/cx-angular-common';
import { GridApi } from 'ag-grid-community';
import { AuthService } from 'app-auth/auth.service';
import { PagedResultDto } from 'app-models/paged-result.dto';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { Utils } from 'app-utilities/utils';
import { GlobalKeySearchStoreService } from 'app/core/store-services/search-key-store.service';
import { AppConstant } from 'app/shared/app.constant';
import { stringEmpty } from 'app/shared/common.constant';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import { SAM_PERMISSIONS } from 'app/shared/constants/sam-permission.constant';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { initTaxonomyUserActions } from 'app/user-accounts/models/user-action-mapping';
import {
  UserManagement,
  UserManagementQueryModel
} from 'app/user-accounts/models/user-management.model';
import { UserAccountsDataService } from 'app/user-accounts/user-accounts-data.service';
import { ToastrService } from 'ngx-toastr';
import { forkJoin, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { TaxonomyActionButtonEnum } from './constant/taxonomy-action-button.enum';
import { TaxonomyRequestStatusLabel } from './constant/taxonomy-request-status-label.enum';
import { TaxonomyRequestStatus } from './constant/taxonomy-request-status.enum';
import { ApproveTaxonomyRequest } from './dtos/approve-taxonomy-request.dto';
import { GetMetadataSuggestionsRequest } from './dtos/get-metadata-suggestion-request.dto';
import { RejectTaxonomyRequest } from './dtos/reject-taxonomy-request.dto';
import { ITaxonomyCreationRequest } from './dtos/taxonomy-creation-request.dto';
import { UpdateTaxonomyRequestItemRequest } from './dtos/update-taxonomy-request-Item-request.dto';
import { UpdateTaxonomyRequestRequest } from './dtos/update-taxonomy-request-request.dto';
import {
  TaxonomyActionsModel,
  TaxonomyActionToolbarModel
} from './models/actions.model';
import { TaxonomyRequestItem } from './models/taxonomy-request-item.model';
import { TaxonomyRequestStatusLabelType } from './models/taxonomy-request-status-label-type';
import { TaxonomyRequest } from './models/taxonomy-request.model';
import { TaxonomyRequestViewModel } from './models/taxonomy-request.viewmodel';
import { MetadataRequestApiService } from './services/metadata-request-api.services';
import { MetadataRequestDataListSerivce } from './services/metadata-request-data-list.service';
import { MetadataConfirmDialogComponent } from './taxonomy-dialog/metadata-confirm-dialog/metadata-confirm-dialog.component';
import {
  MetadataDialogMode,
  TaxonomyRequestDialogComponent
} from './taxonomy-dialog/metadata-request/metadata-request/taxonomy-request-dialog.component';

@Component({
  selector: 'taxonomy-management',
  templateUrl: './taxonomy-management.component.html',
  styleUrls: ['./taxonomy-management.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaxonomyManagementComponent
  extends BaseScreenComponent
  implements OnInit {
  get taxonomyActions(): TaxonomyActionToolbarModel {
    return this._taxonomyActions;
  }
  set taxonomyActions(v: TaxonomyActionToolbarModel) {
    this._taxonomyActions = v;
  }

  taxonomyItemsData: PagedResultDto<TaxonomyRequestViewModel>;

  // tslint:disable-next-line:typedef
  samPermissions = SAM_PERMISSIONS;

  pending1stLevelApprovalList: TaxonomyRequestViewModel[];
  pending2ndLevelApprovalList: TaxonomyRequestViewModel[];
  approvedList: TaxonomyRequestViewModel[];
  rejectedList: TaxonomyRequestViewModel[];
  completedList: TaxonomyRequestViewModel[];

  pending1stLevelApprovalUserInfoList: UserManagement[] = [];
  pending2ndLevelApprovalUserInfoList: UserManagement[] = [];
  approvedUserInfoList: UserManagement[] = [];
  rejectedUserInfoList: UserManagement[] = [];
  completedUserInfoList: UserManagement[] = [];

  selectedMetadatas: TaxonomyRequestViewModel[] = [];

  taxonomyRequestStatusLabel: TaxonomyRequestStatusLabelType = {
    pendingLevel1: TaxonomyRequestStatusLabel.PendingLevel1,
    pendingLevel2: TaxonomyRequestStatusLabel.PendingLevel2,
    rejected: TaxonomyRequestStatusLabel.Rejected,
    approved: TaxonomyRequestStatusLabel.Approved,
    completed: TaxonomyRequestStatusLabel.Completed
  };

  searchParam: string = stringEmpty;
  currentPageIndex: number = 1;
  defaultPageSize: number = AppConstant.ItemPerPage;

  isVerticalToShowMenuAction: boolean;
  isHideSuggestMetadataChangeButton: boolean = false;
  readonly TAXONOMY_DEFAULT_PAGE_SIZE: number = 50;

  tabThatDoNotShowAction: TaxonomyRequestStatusLabel[] = [
    TaxonomyRequestStatusLabel.Rejected,
    TaxonomyRequestStatusLabel.Completed
  ];
  currentTabAriaLabel: TaxonomyRequestStatusLabel =
    TaxonomyRequestStatusLabel.PendingLevel1;

  private filterParams: UserManagementQueryModel = new UserManagementQueryModel();

  private gridApi: GridApi;

  private _taxonomyActions: TaxonomyActionToolbarModel;
  private taxonomyListSearchKeyHistory: Map<string, string> = new Map([
    [TaxonomyRequestStatusLabel.PendingLevel1, ''],
    [TaxonomyRequestStatusLabel.PendingLevel2, ''],
    [TaxonomyRequestStatusLabel.Approved, ''],
    [TaxonomyRequestStatusLabel.Rejected, ''],
    [TaxonomyRequestStatusLabel.Completed, '']
  ]);

  private _defaultStatusFilter: any[] = [
    StatusTypeEnum.Active.code,
    StatusTypeEnum.New.code,
    StatusTypeEnum.Deactive.code,
    StatusTypeEnum.Inactive.code,
    StatusTypeEnum.IdentityServerLocked.code
  ];

  constructor(
    public authService: AuthService,
    changeDetectorRef: ChangeDetectorRef,
    private globalKeySearchStoreService: GlobalKeySearchStoreService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private metadataRequestApiService: MetadataRequestApiService,
    private metadataRequestDataListSerivce: MetadataRequestDataListSerivce,
    private toastrService: ToastrService,
    private translateAdapterService: TranslateAdapterService,
    private userAccountsDataService: UserAccountsDataService,
    private dialog: MatDialog
  ) {
    super(changeDetectorRef, authService);
  }

  async ngOnInit(): Promise<void> {
    this.filterParams.pageSize = this.defaultPageSize;
    this.getTaxonomyListBasedOnCurrentTab();
    this.registerGlobalSearchSubscription();
  }

  onActionChanged($event: TaxonomyActionToolbarModel): void {
    this.taxonomyActions = $event;
    const totalItems =
      $event.listNonEssentialActions.length + $event.listSpecifyActions.length;
    const maximumItems = 7;
    this.isVerticalToShowMenuAction = totalItems > maximumItems ? false : true;
    this.changeDetectorRef.detectChanges();
  }

  onSelectedTabChange(tabChangeEvent: MatTabChangeEvent): void {
    this.clearSelectedItems();

    this.currentPageIndex = 1;
    this.currentTabAriaLabel = tabChangeEvent.tab
      .ariaLabel as TaxonomyRequestStatusLabel;
    this.getTaxonomyListBasedOnCurrentTab();

    this.initTaxonomyActionsListBasedOnRoles();

    // this.gridApi.sizeColumnsToFit();
  }

  onMenuActionClick($event: TaxonomyActionsModel): void {
    if ($event) {
      switch ($event.actionType) {
        case TaxonomyActionButtonEnum.Approve:
          this.onApproveMultipleMetadatas();
          break;
        case TaxonomyActionButtonEnum.Reject:
          this.onRejectMultipleMetadatas();
          break;
        case TaxonomyActionButtonEnum.Complete:
          this.onCompleteMultipleMetadatas();
          break;
        default:
          break;
      }
    }
  }

  onMetadatasSelected(selectedMetadatas: TaxonomyRequestViewModel[]): void {
    this.selectedMetadatas = selectedMetadatas;

    if (
      this.selectedMetadatas.length > 0 &&
      this.taxonomyActions.listEssentialActions.length > 0
    ) {
      this.updateTaxonomyActionButtons();
    }
  }

  onGridApiReady(gridApi: GridApi): void {
    this.gridApi = gridApi;
  }

  onSuggestMetadataChangeClicked(): void {
    this.onMetadataDialogOpened('suggestion');
  }

  onEditMetadataRequest(
    taxonomyRequestViewModel: TaxonomyRequestViewModel
  ): void {
    if (taxonomyRequestViewModel == null) {
      return;
    }

    if (
      this.currentTabAriaLabel === TaxonomyRequestStatusLabel.Rejected ||
      this.currentTabAriaLabel === TaxonomyRequestStatusLabel.Approved ||
      this.currentTabAriaLabel === TaxonomyRequestStatusLabel.Completed ||
      (this.currentTabAriaLabel === TaxonomyRequestStatusLabel.PendingLevel1 &&
        !this.currentUser.hasPermission(
          SAM_PERMISSIONS.MetadataEditPending1st
        )) ||
      (this.currentTabAriaLabel === TaxonomyRequestStatusLabel.PendingLevel2 &&
        !this.currentUser.hasPermission(SAM_PERMISSIONS.MetadataEditPending2nd))
    ) {
      this.onMetadataDialogOpened(
        'readonly',
        taxonomyRequestViewModel.taxonomyRequestId
      );

      return;
    }
    this.onMetadataDialogOpened(
      'detail',
      taxonomyRequestViewModel.taxonomyRequestId
    );
  }

  onMetadataDialogOpened(
    mode: MetadataDialogMode,
    metadataRequestId?: string
  ): void {
    const metadataDialog = this.dialog.open(TaxonomyRequestDialogComponent, {
      width: '888px',
      height: '80vh',
      hasBackdrop: true,
      data: metadataRequestId
    });

    const metadataDialogInstance = metadataDialog.componentInstance;
    metadataDialogInstance.mode = mode;
    metadataDialogInstance.currentUser = this.currentUser;

    metadataDialog.afterClosed().subscribe((result: any) => {
      if (result == null) {
        return;
      }

      if (result.metadataRequest) {
        const metadataRequest = Utils.cloneDeep(
          result.metadataRequest
        ) as TaxonomyRequestItem;

        const taxonomyCreationRequest: ITaxonomyCreationRequest = {
          taxonomyRequestItems: [metadataRequest]
        };

        this.metadataRequestApiService
          .createMetadataRequest(taxonomyCreationRequest)
          .subscribe((taxonomyRequestResponse) => {
            if (taxonomyRequestResponse) {
              this.toastrService.success(
                'The request was successfully created'
              );
              this.getTaxonomyListBasedOnCurrentTab();
            }
          });
      }

      if (result.updateTaxonomyRequestResult) {
        const updateTaxonomyRequestResult = Utils.cloneDeep(
          result.updateTaxonomyRequestResult
        ) as UpdateTaxonomyRequestRequest;
        this.metadataRequestApiService
          .updateMetadata(updateTaxonomyRequestResult)
          .subscribe((taxonomyRequestItemRes) => {
            if (taxonomyRequestItemRes) {
              this.toastrService.success(
                'The request was successfully updated'
              );
            }
          });
      }

      if (result.approveTaxonomyRequestResult) {
        const approveTaxonomyRequest = result.approveTaxonomyRequestResult as ApproveTaxonomyRequest;
        this.approveRequest(approveTaxonomyRequest);
      }

      if (result.rejectTaxonomyRequestResult) {
        const rejectTaxonomyRequest = result.rejectTaxonomyRequestResult as RejectTaxonomyRequest;
        this.rejectRequest(rejectTaxonomyRequest);
      }
    });
  }

  onDropdownActionClick($event: any): void {
    switch ($event.action) {
      case 'edit':
        this.onEditMetadataRequest($event.data);
        break;
      case 'approve':
        this.onApproveActionClicked($event.action, $event.data);
        break;
      case 'reject':
        this.onRejectActionClicked($event.action, $event.data);
        break;
      case 'completed':
        this.onCloseRequest($event.data);
        break;
      default:
        break;
    }
  }

  onTaxonomyManagementPageChange(pageIndex: number): void {
    this.currentPageIndex = pageIndex;
    this.getTaxonomyListBasedOnCurrentTab();
    window.scroll(0, 0);
  }

  checkMetadataHasDataFnCreator(currentTab: string): () => Observable<number> {
    const taxonomyPageSize = 10000;
    const taxonomyFilterParam = new GetMetadataSuggestionsRequest();
    taxonomyFilterParam.statuses = this.getFilterStatusByTab(currentTab);
    taxonomyFilterParam.searchByRequestTitle = '';
    taxonomyFilterParam.maxResultCount = taxonomyPageSize;

    return () =>
      this.metadataRequestDataListSerivce
        .getMetadataRequestList(taxonomyFilterParam)
        .pipe(map((metadataResponse) => metadataResponse.totalCount));
  }

  private registerGlobalSearchSubscription(): void {
    this.subscription.add(
      this.globalKeySearchStoreService.get().subscribe((result: any) => {
        if (result) {
          this.searchParam = result.searchKey;
          if (result.isSearch) {
            this.filterParams.pageIndex = 1;
            this.getTaxonomyListBasedOnCurrentTab();
          }
        }
      })
    );
  }

  private onApproveMultipleMetadatas(): void {
    this.openConfirmDialog('approve').subscribe((comment: string) => {
      if (!comment) {
        return;
      }
      forkJoin(
        this.selectedMetadatas.map((metadata: TaxonomyRequestViewModel) => {
          const approveTaxonomyRequest = this.prepareApproveRequestModelForCallingApi(
            metadata,
            comment
          );

          return this.metadataRequestApiService.approveTaxonomyRequest(
            approveTaxonomyRequest
          );
        })
      ).subscribe((approvedRequests: TaxonomyRequest[]) => {
        if (approvedRequests.length > 0) {
          this.toastrService.success(
            'These requests were successfully approved'
          );
          this.getTaxonomyListBasedOnCurrentTab();
          this.updateTaxonomyActionButtons();
        }
      });
    });
  }

  private onRejectMultipleMetadatas(): void {
    this.openConfirmDialog('reject').subscribe((comment: string) => {
      if (!comment) {
        return;
      }
      forkJoin(
        this.selectedMetadatas.map((metadata: TaxonomyRequestViewModel) => {
          const rejectTaxonomyRequest = this.prepareRejectRequestModelForCallingApi(
            metadata,
            comment
          );

          return this.metadataRequestApiService.rejectTaxonomyRequest(
            rejectTaxonomyRequest
          );
        })
      ).subscribe((rejectedRequests: TaxonomyRequest[]) => {
        if (rejectedRequests.length > 0) {
          this.toastrService.success(
            'These requests were successfully rejected'
          );
          this.getTaxonomyListBasedOnCurrentTab();
          this.updateTaxonomyActionButtons();
        }
      });
    });
  }

  private onCompleteMultipleMetadatas(): void {
    forkJoin(
      this.selectedMetadatas.map((metadata: TaxonomyRequestViewModel) => {
        return this.metadataRequestApiService.closeTaxonomyRequestById(
          metadata.taxonomyRequestId
        );
      })
    ).subscribe((completedRequests: TaxonomyRequest[]) => {
      if (completedRequests.length > 0) {
        this.toastrService.success(
          'These requests were successfully completed'
        );
        this.getTaxonomyListBasedOnCurrentTab();
        this.updateTaxonomyActionButtons();
      }
    });
  }

  private onApproveActionClicked(
    action: string,
    metadata: TaxonomyRequestViewModel
  ): void {
    this.openConfirmDialog(action).subscribe((comment: string) => {
      if (!comment) {
        return;
      }

      const approveTaxonomyRequest = this.prepareApproveRequestModelForCallingApi(
        metadata,
        comment
      );

      this.approveRequest(approveTaxonomyRequest);
    });
  }

  private prepareApproveRequestModelForCallingApi(
    metadata: TaxonomyRequestViewModel,
    comment: string
  ): ApproveTaxonomyRequest {
    const approveTaxonomyRequest = new ApproveTaxonomyRequest();
    approveTaxonomyRequest.comment = comment;
    approveTaxonomyRequest.taxonomyRequestId = metadata.taxonomyRequestId;
    approveTaxonomyRequest.item = this.getUpdateTaxonomyRequestItemRequestModel(
      metadata
    );

    return approveTaxonomyRequest;
  }

  private approveRequest(approveTaxonomyRequest: ApproveTaxonomyRequest): void {
    this.metadataRequestApiService
      .approveTaxonomyRequest(approveTaxonomyRequest)
      .subscribe((taxonomyRequest: TaxonomyRequest) => {
        if (taxonomyRequest) {
          this.toastrService.success('The request was successfully approved');
          this.getTaxonomyListBasedOnCurrentTab();
          this.updateTaxonomyActionButtons();
        }
      });
  }

  private onRejectActionClicked(
    action: string,
    metadata: TaxonomyRequestViewModel
  ): void {
    this.openConfirmDialog(action).subscribe((comment: string) => {
      if (!comment) {
        return;
      }

      const rejectTaxonomyRequest = this.prepareRejectRequestModelForCallingApi(
        metadata,
        comment
      );

      this.rejectRequest(rejectTaxonomyRequest);
    });
  }

  private prepareRejectRequestModelForCallingApi(
    metadata: TaxonomyRequestViewModel,
    comment: string
  ): RejectTaxonomyRequest {
    const rejectTaxonomyRequest = new RejectTaxonomyRequest();
    rejectTaxonomyRequest.comment = comment;
    rejectTaxonomyRequest.taxonomyRequestId = metadata.taxonomyRequestId;
    rejectTaxonomyRequest.item = this.getUpdateTaxonomyRequestItemRequestModel(
      metadata
    );

    return rejectTaxonomyRequest;
  }

  private rejectRequest(rejectTaxonomyRequest: RejectTaxonomyRequest): void {
    this.metadataRequestApiService
      .rejectTaxonomyRequest(rejectTaxonomyRequest)
      .subscribe((taxonomyRequest: TaxonomyRequest) => {
        if (taxonomyRequest) {
          this.toastrService.success('The request was successfully rejected');
          this.getTaxonomyListBasedOnCurrentTab();
          this.updateTaxonomyActionButtons();
        }
      });
  }

  private onCloseRequest(metadata: TaxonomyRequestViewModel): void {
    this.metadataRequestApiService
      .closeTaxonomyRequestById(metadata.taxonomyRequestId)
      .subscribe((taxonomyRequest: TaxonomyRequest) => {
        if (taxonomyRequest) {
          this.toastrService.success('The request was successfully completed');
          this.getTaxonomyListBasedOnCurrentTab();
          this.updateTaxonomyActionButtons();
        }
      });
  }

  private openConfirmDialog(action: string): Observable<string> {
    return new Observable((observer) => {
      const metadataConfirmDialog = this.dialog.open(
        MetadataConfirmDialogComponent,
        {
          width: '888px',
          hasBackdrop: true
        }
      );

      const metadataConfirmDialogInstance =
        metadataConfirmDialog.componentInstance;
      metadataConfirmDialogInstance.action = action;

      metadataConfirmDialog.afterClosed().subscribe((result: any) => {
        observer.next(result.comment ? result.comment : '');
        observer.complete();
      });
    });
  }

  private getUpdateTaxonomyRequestItemRequestModel(
    metadata: TaxonomyRequestViewModel
  ): UpdateTaxonomyRequestItemRequest {
    const modelResult = new UpdateTaxonomyRequestItemRequest();

    modelResult.abbreviation = metadata.abbreviation;
    modelResult.metadataName = metadata.metadataName;
    modelResult.nodeId = metadata.nodeId;
    modelResult.reason = metadata.reason;
    modelResult.taxonomyRequestItemId = metadata.taxonomyRequestItemId;

    return modelResult;
  }

  private getTaxonomyListBasedOnCurrentTab(): void {
    const taxonomyFilterParam = new GetMetadataSuggestionsRequest();
    taxonomyFilterParam.statuses = this.getFilterStatusBasedOnCurrentTab();
    taxonomyFilterParam.searchByRequestTitle = this.searchParam;
    taxonomyFilterParam.maxResultCount = this.TAXONOMY_DEFAULT_PAGE_SIZE;
    taxonomyFilterParam.skipCount =
      (this.currentPageIndex - 1) * this.TAXONOMY_DEFAULT_PAGE_SIZE;

    this.cxGlobalLoaderService.showLoader();

    this.subscription.add(
      this.metadataRequestDataListSerivce
        .getMetadataRequestList(taxonomyFilterParam)
        .subscribe(
          async (result: PagedResultDto<TaxonomyRequestViewModel>) => {
            this.taxonomyListSearchKeyHistory.set(
              this.currentTabAriaLabel,
              this.searchParam
            );

            if (!result.items.length) {
              this.updateTaxonomyListItems(this.currentTabAriaLabel, result);
            }

            this.userAccountsDataService
              .getUsers(
                new UserManagementQueryModel({
                  extIds: result.items.map(
                    (metadataRequest: TaxonomyRequestViewModel) =>
                      metadataRequest.createdBy
                  ),
                  orderBy: 'firstName asc',
                  userEntityStatuses: this._defaultStatusFilter,
                  pageIndex: 0,
                  pageSize: 0
                })
              )
              .subscribe((userManagements) => {
                if (!userManagements || !userManagements.items.length) {
                  return;
                }

                this.updateTaxonomyUserInfoList(
                  this.currentTabAriaLabel,
                  userManagements.items
                );
                this.updateTaxonomyListItems(this.currentTabAriaLabel, result);
              });

            this.cxGlobalLoaderService.hideLoader();
            this.changeDetectorRef.detectChanges();
          },
          (error) => {
            this.cxGlobalLoaderService.hideLoader();
          }
        )
    );
  }

  private getFilterStatusBasedOnCurrentTab(): TaxonomyRequestStatus[] {
    return this.getFilterStatusByTab(this.currentTabAriaLabel);
  }

  private getFilterStatusByTab(tabAriaLabel: string): TaxonomyRequestStatus[] {
    switch (tabAriaLabel) {
      case TaxonomyRequestStatusLabel.PendingLevel1:
        return [TaxonomyRequestStatus.PendingLevel1];
      case TaxonomyRequestStatusLabel.PendingLevel2:
        return [TaxonomyRequestStatus.PendingLevel2];
      case TaxonomyRequestStatusLabel.Rejected:
        return [
          TaxonomyRequestStatus.RejectLevel1,
          TaxonomyRequestStatus.RejectLevel2
        ];
      case TaxonomyRequestStatusLabel.Approved:
        return [TaxonomyRequestStatus.Approved];
      case TaxonomyRequestStatusLabel.Completed:
        return [TaxonomyRequestStatus.Completed];
      default:
        return [];
    }
  }

  private updateTaxonomyListItems(
    currentTab: string,
    dataResponse: PagedResultDto<TaxonomyRequestViewModel>
  ): void {
    this.taxonomyItemsData = dataResponse;

    switch (currentTab) {
      case TaxonomyRequestStatusLabel.PendingLevel1:
        this.pending1stLevelApprovalList = dataResponse.items;
        break;
      case TaxonomyRequestStatusLabel.PendingLevel2:
        this.pending2ndLevelApprovalList = dataResponse.items;
        break;
      case TaxonomyRequestStatusLabel.Approved:
        this.approvedList = dataResponse.items;
        break;
      case TaxonomyRequestStatusLabel.Rejected:
        this.rejectedList = dataResponse.items;
        break;
      case TaxonomyRequestStatusLabel.Completed:
        this.completedList = dataResponse.items;
        break;
      default:
        break;
    }
    this.changeDetectorRef.detectChanges();
  }

  private updateTaxonomyUserInfoList(
    currentTab: string,
    userManagementList: UserManagement[]
  ): void {
    switch (currentTab) {
      case TaxonomyRequestStatusLabel.PendingLevel1:
        this.pending1stLevelApprovalUserInfoList = userManagementList;
        break;
      case TaxonomyRequestStatusLabel.PendingLevel2:
        this.pending2ndLevelApprovalUserInfoList = userManagementList;
        break;
      case TaxonomyRequestStatusLabel.Approved:
        this.approvedUserInfoList = userManagementList;
        break;
      case TaxonomyRequestStatusLabel.Rejected:
        this.rejectedUserInfoList = userManagementList;
        break;
      case TaxonomyRequestStatusLabel.Completed:
        this.completedUserInfoList = userManagementList;
        break;
      default:
        break;
    }
    this.changeDetectorRef.detectChanges();
  }

  private clearSelectedItems(): void {
    this.selectedMetadatas = [];
    this.changeDetectorRef.detectChanges();
  }

  private clearTaxonomyActions(): void {
    if (!this.taxonomyActions) {
      return;
    }

    this.taxonomyActions.listEssentialActions = [];
    this.taxonomyActions.listNonEssentialActions = [];
    this.taxonomyActions.listSpecifyActions = [];
  }

  private initTaxonomyActionsListBasedOnRoles(): void {
    this.isVerticalToShowMenuAction = true;

    this.clearTaxonomyActions();
    this.taxonomyActions = initTaxonomyUserActions(
      this.translateAdapterService,
      this.currentTabAriaLabel,
      this.checkIfHasPermissionToApprove(),
      this.checkIfHasPermissionToReject(),
      this.checkIfHasPermissionToComplete()
    );
  }

  private updateTaxonomyActionButtons(): void {
    this.taxonomyActions.listEssentialActions.forEach(
      (action: ActionsModel) => {
        action.disable = !action.disable;
      }
    );
  }

  private checkIfHasPermissionToApprove(): boolean {
    switch (this.currentTabAriaLabel) {
      case TaxonomyRequestStatusLabel.PendingLevel1:
        return this.currentUser.hasPermission(
          SAM_PERMISSIONS.MetadataApprovePending1st
        );
      case TaxonomyRequestStatusLabel.PendingLevel2:
        return this.currentUser.hasPermission(
          SAM_PERMISSIONS.MetadataApprovePending2nd
        );
      default:
        return false;
    }
  }

  private checkIfHasPermissionToReject(): boolean {
    switch (this.currentTabAriaLabel) {
      case TaxonomyRequestStatusLabel.PendingLevel1:
        return this.currentUser.hasPermission(
          SAM_PERMISSIONS.MetadataRejectPending1st
        );
      case TaxonomyRequestStatusLabel.PendingLevel2:
        return this.currentUser.hasPermission(
          SAM_PERMISSIONS.MetadataRejectPending2nd
        );
      default:
        return false;
    }
  }

  private checkIfHasPermissionToComplete(): boolean {
    return (
      this.currentTabAriaLabel === TaxonomyRequestStatusLabel.Approved &&
      this.currentUser.hasPermission(
        SAM_PERMISSIONS.MetadataMarkAsCompleteInApprovedList
      )
    );
  }
}
