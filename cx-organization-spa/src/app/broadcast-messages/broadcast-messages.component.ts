import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import {
  CxGlobalLoaderService,
  DepartmentHierarchiesModel
} from '@conexus/cx-angular-common';
import { AuthService } from 'app-auth/auth.service';
import { ShowHideColumnModel } from 'app-models/ag-grid-column.model';
import { SortModel } from 'app-models/sort.model';
import { Utils } from 'app-utilities/utils';
import { AppConstant } from 'app/shared/app.constant';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import { BroadcastMessageStatus } from 'app/shared/constants/broadcast-message-status.enum';
import { HTTP_STATUS_CODE } from 'app/shared/constants/http-status-code';
import { StatusActionTypeEnum } from 'app/shared/constants/status-action-type.enum';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import {
  PagingResponseModel,
  UserManagement,
  UserManagementQueryModel
} from 'app/user-accounts/models/user-management.model';
import { UserAccountsDataService } from 'app/user-accounts/user-accounts-data.service';
import { ToastrService } from 'ngx-toastr';
import { SAM_PERMISSIONS } from '../shared/constants/sam-permission.constant';
import { ActionsItemModel } from './events/action-items.model';
import {
  BroadcastMessagesDto,
  BroadcastMessagesFilterParams
} from './models/broadcast-messages.model';
import { BroadcastMessageViewModel } from './models/broadcast-messages.view.model';
import { BroadcastMessagesApiService } from './services/broadcast-messages-api.service';
import { BroadcastMessagesService } from './services/broadcast-messages.service';

@Component({
  selector: 'broadcast-message',
  templateUrl: './broadcast-messages.component.html',
  styleUrls: ['./broadcast-messages.component.scss']
})
export class BroadcastMessagesComponent
  extends BaseScreenComponent
  implements OnInit {
  defaultPageSize: number = AppConstant.Item25PerPage;
  broadcastMessagesDataPaging: PagingResponseModel<BroadcastMessagesDto>;
  isClearSelected: boolean = false;
  departmentModel: DepartmentHierarchiesModel = new DepartmentHierarchiesModel();
  showHideColumns: ShowHideColumnModel;
  broadcastMessageOwners: UserManagement[] = [];

  private _filterParams: BroadcastMessagesFilterParams;
  private _defaultSortField: string = 'CreatedDate';
  private _defaultSortType: string = 'Desc';
  private _defaultStatusFilter: any[] = [
    StatusTypeEnum.Active.code,
    StatusTypeEnum.New.code,
    StatusTypeEnum.Deactive.code,
    StatusTypeEnum.Inactive.code,
    StatusTypeEnum.IdentityServerLocked.code
  ];
  constructor(
    protected changeDetectorRef: ChangeDetectorRef,
    protected authService: AuthService,
    private broadcastMessagesService: BroadcastMessagesService,
    private toastrService: ToastrService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private broadcastMessagesApiSvc: BroadcastMessagesApiService,
    private userAccountsDataSvc: UserAccountsDataService,
    private router: Router
  ) {
    super(changeDetectorRef, authService);
  }

  ngOnInit(): void {
    this.initFilterParams();
    this.getBroadcastMessages();
  }

  get isCurrentUserAllowToMakeActions(): boolean {
    return this.currentUser.hasPermission(
      SAM_PERMISSIONS.CRUDinBroadcastMessages
    );
  }

  initFilterParams(): void {
    this._filterParams = new BroadcastMessagesFilterParams({
      pageIndex: 1,
      pageSize: this.defaultPageSize,
      orderBy: this._defaultSortField,
      orderType: this._defaultSortType
    });
  }

  createNewBroadcastMessage(): void {
    this.router.navigate(['/broadcast-messages/detail/', '']);
  }

  getBroadcastMessages(): void {
    this.cxGlobalLoaderService.showLoader();
    const loadMessageSub = this.broadcastMessagesService
      .getBroadcastMessages(this._filterParams)
      .subscribe(
        (broadcastMessageDtos) => {
          // No data response
          if (!broadcastMessageDtos) {
            return;
          }

          // empty data at first page
          if (
            !broadcastMessageDtos.items.length &&
            this._filterParams.pageIndex <= 1
          ) {
            this.broadcastMessagesDataPaging = broadcastMessageDtos;

            return;
          }

          // when delete the last record on a page, it will go back to the preceding page index
          if (!broadcastMessageDtos.items.length) {
            this.onBroadcastMessagesPageChange(
              this._filterParams.pageIndex - 1
            );

            return;
          }

          this.userAccountsDataSvc
            .getUsers(
              new UserManagementQueryModel({
                extIds: broadcastMessageDtos.items.map(
                  (broadcastMessage) => broadcastMessage.ownerId
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

              this.broadcastMessageOwners = userManagements.items;
              const userDic = Utils.toDictionary(
                this.broadcastMessageOwners,
                (u) => u.identity.extId.toUpperCase()
              );

              const broadcastMessagesVmResult: PagingResponseModel<BroadcastMessageViewModel> = {
                items: broadcastMessageDtos.items.map((message) =>
                  BroadcastMessageViewModel.createFromModel(
                    message,
                    userDic[message.ownerId.toUpperCase()]
                      ? userDic[message.ownerId.toUpperCase()].firstName
                      : 'Unknown user',
                    message.status
                  )
                ),
                hasMoreData: broadcastMessageDtos.hasMoreData,
                pageIndex: broadcastMessageDtos.pageIndex,
                pageSize: broadcastMessageDtos.pageSize,
                totalItems: broadcastMessageDtos.totalItems
              };

              this.broadcastMessagesDataPaging = broadcastMessagesVmResult;
            });
        },
        () => {
          this.toastrService.error('Error during getting broadcast messages');
        },
        () => {
          this.changeDetectorRef.detectChanges();
          this.cxGlobalLoaderService.hideLoader();
        }
      );

    this.subscription.add(loadMessageSub);
  }

  onPageSizeChange(pageSize: number): void {
    if (pageSize > Number(this._filterParams.pageSize)) {
      this._filterParams.pageIndex = 1;
    }
    this._filterParams.pageSize = pageSize;
    this.getBroadcastMessages();
    window.scroll(0, 0);
  }

  onBroadcastMessagesPageChange(pageIndex: number): void {
    this._filterParams.pageIndex = pageIndex <= 0 ? 1 : pageIndex;
    this.getBroadcastMessages();
    window.scroll(0, 0);
  }

  onSortTypeChange($event: SortModel): void {
    this._filterParams.orderBy = $event.currentFieldSort;
    this._filterParams.orderType = $event.currentSortType;
    this.getBroadcastMessages();
  }

  onSingleActionClicked(
    $event: ActionsItemModel<BroadcastMessageViewModel>
  ): void {
    if ($event && $event.action) {
      switch ($event.action.actionType) {
        case StatusActionTypeEnum.Edit:
          this.onEditBroadcastMessagesClicked($event.item);
          break;
        case StatusActionTypeEnum.Deactivate:
          this.onDeactivate($event.item);
          break;
        case StatusActionTypeEnum.Delete:
          this.onDelete($event.item);
          break;
        default:
          break;
      }
    }
  }

  onEditBroadcastMessagesClicked(message: BroadcastMessageViewModel): void {
    this.router.navigate([
      '/broadcast-messages/detail/',
      message.broadcastMessageId
    ]);
  }

  private onDeactivate(broadcastMessage: BroadcastMessagesDto): void {
    broadcastMessage.status = BroadcastMessagesDto.parseStatusToText(
      BroadcastMessageStatus.Deactivate
    );

    const deactivateSub = this.broadcastMessagesApiSvc
      .editBroadcastMessage(broadcastMessage)
      .subscribe((result) => {
        if (result.status === HTTP_STATUS_CODE.STATUS_200_SUCCESS) {
          this.broadcastMessagesApiSvc
            .changeBroadcastMessageStatus(broadcastMessage)
            .subscribe((_) => {
              this.getBroadcastMessages();
            });
        } else {
          this.toastrService.error(
            'This broadcast message can not be deactivated'
          );
        }
      });

    this.subscription.add(deactivateSub);
  }

  private onDelete(broadcastMessage: BroadcastMessagesDto): void {
    const deleteMessageSub = this.broadcastMessagesApiSvc
      .deleteBroadcastMessage(broadcastMessage.broadcastMessageId)
      .subscribe((result) => {
        if (result) {
          this.getBroadcastMessages();
          this.toastrService.success('Message is deleted successfully');
        } else {
          this.toastrService.error('This message not found');
        }
      });

    this.subscription.add(deleteMessageSub);
  }
}
