import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewEncapsulation
} from '@angular/core';
import { AuthService } from 'app-auth/auth.service';
import {
  AgGridConfigModel,
  ColumDefModel
} from 'app-models/ag-grid-config.model';
import { User } from 'app-models/auth.model';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import {
  PagingResponseModel,
  UserManagement,
  UserManagementQueryModel
} from 'app/user-accounts/models/user-management.model';
import { UserAccountsDataService } from 'app/user-accounts/user-accounts-data.service';
import { CellRemoveButtonActionComponent } from 'app/user-accounts/user-list/cell-components/cell-remove-button-action/cell-remove-button-action.component';
import { CellUserInfoComponent } from 'app/user-accounts/user-list/cell-components/cell-user-info/cell-user-info.component';
import { USER_LIST_HEADER_CONST } from 'app/user-accounts/user-list/models/user-list-header.const';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'user-select',
  templateUrl: './user-select.component.html',
  styleUrls: ['./user-select.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserSelectComponent extends BaseScreenComponent implements OnInit {
  fetchUsersFn: (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<UserManagement[]> = null;

  @Input() selectedUserExtIds: string[] = [];
  @Output() selectedUserChange: EventEmitter<
    UserManagement[]
  > = new EventEmitter();
  @Output() removeRequestChange: EventEmitter<
    UserManagement[]
  > = new EventEmitter();

  userList: UserManagement[] = [];
  belongingUserInUserGroup: UserManagement[] = [];
  defaultAvatar: string = `../../../../../assets/images/default-avatar.png`;
  agGridConfig: AgGridConfigModel;
  selectedUserIds: string[] = [];
  removedUsers: UserManagement[] = [];

  private filterParams: UserManagementQueryModel = new UserManagementQueryModel();

  private defaultStatusFilter: string[] = [
    StatusTypeEnum.Active.code,
    StatusTypeEnum.New.code,
    StatusTypeEnum.Inactive.code,
    StatusTypeEnum.IdentityServerLocked.code,
    StatusTypeEnum.Archived.code
  ];

  constructor(
    protected changeDetectorRef: ChangeDetectorRef,
    protected authService: AuthService,
    private userAccountsDataService: UserAccountsDataService,
    private translateAdapterService: TranslateAdapterService
  ) {
    super(changeDetectorRef, authService);
    this.fetchUsersFn = this.createFetchUsersFn();
  }

  ngOnInit(): void {
    this.initGridConfig();

    this.filterParams.extIds = [];
    if (this.selectedUserExtIds.length) {
      this.getUsers();
    }
  }

  onUserSelected(userSelectedList: UserManagement[]): void {
    let isAddMoreUser = false;
    let includedRemovedUsers: UserManagement[] = [];

    const newUserSelectedListIds = userSelectedList.map(
      (user: UserManagement) => user.identity.extId
    );

    const oldUserSelectedListIds = this.agGridConfig.rowData.map(
      (user: UserManagement) => user.identity.extId
    );

    isAddMoreUser =
      newUserSelectedListIds.length > oldUserSelectedListIds.length
        ? true
        : false;

    let updatedUserId: string[] = [];

    if (isAddMoreUser) {
      updatedUserId = newUserSelectedListIds.filter(
        (extId: string) => !oldUserSelectedListIds.includes(extId)
      );
    } else {
      updatedUserId = oldUserSelectedListIds.filter(
        (extId: string) => !newUserSelectedListIds.includes(extId)
      );
    }

    includedRemovedUsers = this.belongingUserInUserGroup.filter(
      (user: UserManagement) => user.identity.extId === updatedUserId[0]
    );

    if (includedRemovedUsers.length) {
      if (isAddMoreUser) {
        const corresspondUserId = includedRemovedUsers.map(
          (user: UserManagement) => user.identity.extId
        )[0];

        this.removedUsers = this.removedUsers.filter(
          (user: UserManagement) => user.identity.extId !== corresspondUserId
        );
      } else {
        this.removedUsers = this.removedUsers.concat(includedRemovedUsers);
      }

      this.removeRequestChange.emit(this.removedUsers);
    }

    this.agGridConfig.rowData = userSelectedList;
    this.selectedUserChange.emit(this.agGridConfig.rowData);
  }

  isUserSelected(user: UserManagement): boolean {
    return this.agGridConfig.rowData.includes(user);
  }

  onGridReady(params: any): void {
    if (!params) {
      return;
    }

    this.agGridConfig.gridApi = params.api;
    this.agGridConfig.gridColumnApi = params.columnApi;
    this.agGridConfig.gridApi.setDomLayout('autoHeight');
    this.agGridConfig.gridApi.sizeColumnsToFit();

    if (
      this.agGridConfig.gridColumnApi &&
      this.agGridConfig.gridColumnApi.columnController &&
      this.agGridConfig.columnShowHide &&
      this.agGridConfig.columnShowHide.length <= 0
    ) {
      this.agGridConfig.columnShowHide = this.agGridConfig.gridColumnApi.columnController.columnDefs;
      this.agGridConfig.columnShowHide.pop();
    }
  }

  onRemoveUserClicked(): void {
    const selectedExtIdRows = this.agGridConfig.selectedRows.map(
      (user: UserManagement) => user.identity.extId
    );
    this.agGridConfig.rowData = this.agGridConfig.rowData.filter(
      (user: UserManagement) => !selectedExtIdRows.includes(user.identity.extId)
    );

    this.selectedUserIds = this.selectedUserIds.filter(
      (selectedExtId: string) => !selectedExtIdRows.includes(selectedExtId)
    );

    const belongingRemovedUsers = this.belongingUserInUserGroup.filter(
      (user: UserManagement) => selectedExtIdRows.includes(user.identity.extId)
    );
    this.removedUsers = this.removedUsers.concat(belongingRemovedUsers);

    this.selectedUserChange.emit(this.agGridConfig.rowData);
    this.removeRequestChange.emit(this.removedUsers);
  }
  onSelectionChanged(event: any): void {
    this.agGridConfig.selectedRows = event.api.getSelectedRows();
  }

  handlingPlaceholder(): string {
    const elements = document.getElementsByClassName(
      'ng-select-container ng-has-value'
    );
    const requiredElement = elements[0];

    if (requiredElement) {
      requiredElement.classList.remove('ng-has-value');
    }

    return this.translateAdapterService.getValueImmediately(
      'User_Group_Page.Modify_User_Group_Popup.Placeholder'
    );
  }

  onSearchIconClicked(): void {
    // This function is used for triggering the user dropdown.
    // Without it, dropdown would not be shown... Must have second click on search bar to make dropdown happens!
  }

  private getUsers(): void {
    this.userAccountsDataService
      .getUsers(
        new UserManagementQueryModel({
          extIds: this.selectedUserExtIds,
          pageIndex: 0,
          pageSize: 0,
          userEntityStatuses: this.defaultStatusFilter
        })
      )
      .subscribe((userData: PagingResponseModel<UserManagement>) => {
        this.belongingUserInUserGroup = userData.items;
        this.agGridConfig.rowData = userData.items;
        this.selectedUserIds = this.selectedUserIds.concat(
          userData.items.map((user: UserManagement) => user.identity.extId)
        );
        this.changeDetectorRef.detectChanges();
      });
  }

  private createFetchUsersFn(): (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<UserManagement[]> {
    return (searchText, skipCount, maxResultCount) =>
      this.userAccountsDataService
        .getUsers(
          new UserManagementQueryModel({
            searchKey: searchText,
            orderBy: 'firstName asc',
            parentDepartmentId: [this.currentUser.departmentId],
            userEntityStatuses: this.defaultStatusFilter,
            pageIndex:
              maxResultCount === 0 ? 1 : skipCount / maxResultCount + 1,
            pageSize: maxResultCount,
            filterOnSubDepartment: true
          })
        )
        .pipe(
          map((usersPaging: PagingResponseModel<UserManagement>) => {
            const userDataResponse = usersPaging.items;
            this.userList = [...userDataResponse];

            const searchIcon = document.getElementById('search-icon');
            if (searchIcon) {
              searchIcon.click();
            }

            return this.userList;
          })
        );
  }

  private initGridConfig(): void {
    this.agGridConfig = new AgGridConfigModel({
      columnDefs: this.setColumnDef(),
      frameworkComponents: {
        cellUserInfo: CellUserInfoComponent,
        cellRemoveButton: CellRemoveButtonActionComponent
      },
      rowData: [],
      selectedRows: [],
      context: { componentParent: this },
      defaultColDef: {
        resizable: true
      }
    });
  }

  private setColumnDef(): ColumDefModel[] {
    return [
      new ColumDefModel({
        headerName: this.getImmediatelyLanguage(
          USER_LIST_HEADER_CONST.Name.text
        ),
        field: USER_LIST_HEADER_CONST.Name.fieldName,
        colId: USER_LIST_HEADER_CONST.Name.colId,
        checkboxSelection: true,
        headerCheckboxSelection: true,
        sortable: true,
        cellRenderer: 'cellUserInfo',
        minWidth: 353,
        maxWidth: 353
      }),
      new ColumDefModel({
        headerName: 'Place of Work',
        field: USER_LIST_HEADER_CONST.OrganisationUnit.fieldName,
        colId: USER_LIST_HEADER_CONST.OrganisationUnit.colId,
        sortable: false,
        minWidth: 240,
        maxWidth: 240
      }),
      new ColumDefModel({
        headerName: '',
        cellRenderer: 'cellRemoveButton',
        minWidth: 100,
        maxWidth: 100,
        sortable: false,
        suppressSizeToFit: false,
        suppressMenu: true,
        cellStyle: {
          overflow: 'visible',
          'z-index': '9999',
          'padding-left': '10px',
          'max-width': '150px'
        },
        cellRendererParams: {
          onClick: this.onRemoveActionClicked.bind(this)
        }
      })
    ];
  }

  private onRemoveActionClicked(extId: string): void {
    this.agGridConfig.rowData = this.agGridConfig.rowData.filter(
      (user: UserManagement) => user.identity.extId !== extId
    );

    this.selectedUserIds = this.selectedUserIds.filter(
      (selectedExtId: string) => selectedExtId !== extId
    );

    this.selectedUserChange.emit(this.agGridConfig.rowData);

    const belongingUserIdsInUserGroup = this.belongingUserInUserGroup.map(
      (user: UserManagement) => user.identity.extId
    );

    const isIncludedRemovedUserIds = belongingUserIdsInUserGroup.some(
      (removeExtId: string) => removeExtId === extId
    );

    if (isIncludedRemovedUserIds) {
      const corresspondUser = this.belongingUserInUserGroup.find(
        (user: UserManagement) => user.identity.extId === extId
      );
      if (corresspondUser) {
        this.removedUsers.push(corresspondUser);
      }
      this.removeRequestChange.emit(this.removedUsers);
    }
  }

  private getImmediatelyLanguage(columnName: string): string {
    return this.translateAdapterService.getValueImmediately(
      USER_LIST_HEADER_CONST.GroupKey.text + '.' + columnName
    );
  }
}
