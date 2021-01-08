import {
  AccessRightApiService,
  AccessRightComponentService,
  AccessRightType,
  BaseUserInfo,
  IAccessRight,
  SystemRoleEnum,
  UserInfoModel,
  UserRepository,
  UserUtils
} from '@opal20/domain-api';
import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'access-right-tab',
  templateUrl: './access-right-tab.component.html',
  encapsulation: ViewEncapsulation.None
})
export class AccessRightTabComponent extends BasePageComponent {
  @Input()
  public set originalObjectId(v: string) {
    this._originalObjectId = v;
    if (this._originalObjectId) {
      this.loadData(false);
    }
  }

  @Input()
  public allowAddCollaborator: boolean = true;

  @Input()
  public allowDeleteCollaborator: boolean = false;

  @Input()
  public itemTitle: string;

  @Input()
  public set accessRightType(v: AccessRightType) {
    this.accessRightApiService.initApiService(v);
  }

  @Input()
  public set addSAOnly(v: boolean) {
    this._addSAOnly = v;
  }

  @Input() public currentModule: string = 'CCPM';

  /*** Listing variables ***/
  public _addSAOnly: boolean = false;
  public gridData: GridDataResult;
  public pageNumber: number = 0;
  public pageSize: number = 25;
  public accessRightData: IAccessRight[];

  /*** Dialog variables ***/
  public dialogRef: DialogRef;
  public _hasMoreDataToFetch: boolean = true;
  public collaboratorSelectedIds: string[] = [];
  // Fetch user by from API by term
  public fetchCollaboratorsFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]>;
  // Filter user in opal-select by term
  public filterCollaboratorFn: (term: string, item: BaseUserInfo) => boolean;

  /*** Internal component variables ***/
  private _originalObjectId: string = '';
  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();
  private currentCollaborator: string[] = [];

  @ViewChild('addCollaboratorDialogTemplate', { static: false })
  private addCollaboratorDialogTemplate: TemplateRef<unknown>;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private accessRightApiService: AccessRightApiService,
    private accessRightComponentService: AccessRightComponentService,
    private userRepository: UserRepository
  ) {
    super(moduleFacadeService);
    this.createFilterCollaboratorFn();
  }

  public openModalRevertVersion(): void {
    this._hasMoreDataToFetch = true;
    this.collaboratorSelectedIds = [];
    this.dialogRef = this.moduleFacadeService.dialogService.open({
      content: this.addCollaboratorDialogTemplate
    });
  }

  public onclickCloseDialog(event: MouseEvent): void {
    if (event.which === 1) {
      this.dialogRef.close();
    }
  }

  public async onClickAddCollaborators(): Promise<void> {
    await this.accessRightApiService.addAccessRight(
      {
        originalObjectId: this._originalObjectId,
        userIds: this.collaboratorSelectedIds
      },
      true
    );
    this.dialogRef.close();
    this.pageNumber = 0;
    this.currentCollaborator = this.currentCollaborator.concat(this.collaboratorSelectedIds);
    await this.loadData(true);
  }

  public onClickDeleteAccessRight(item: IAccessRight): void {
    this.modalService.showConfirmMessage(this.translate('Are you sure you want to permanently delete this item?'), () => {
      this.accessRightApiService.deleteAccessRight(item.id, true).then(() => {
        this.currentCollaborator = this.currentCollaborator.filter(x => x !== item.userId);
        this.loadData(true);
      });
    });
  }

  public onPageChange(event: { skip: number }): void {
    this.pageNumber = event.skip;
    this.loadData(false);
  }

  public loadData(showSpinner: boolean = false): Promise<void> {
    return new Promise(resolve => {
      this.accessRightComponentService
        .loadCollaborators(this._originalObjectId, this.pageNumber, this.pageSize, showSpinner)
        .subscribe(result => {
          this.gridData = result;
          resolve();
        });
    });
  }

  public getAllCollaboratorsId(): Promise<void> {
    return new Promise(resolve => {
      this.accessRightComponentService.loadAllCollaboratorsIds(this._originalObjectId).subscribe(result => {
        this.currentCollaborator = result;
        resolve();
      });
    });
  }

  public createFetchCollaboratorsFn(): void {
    const collaboratorRoles: SystemRoleEnum[] = [];
    if (!this._addSAOnly) {
      if (this.currentModule === 'CCPM') {
        if (this.currentUser.hasAdministratorRoles() || this.currentUser.hasRole(SystemRoleEnum.ContentCreator)) {
          collaboratorRoles.push(SystemRoleEnum.ContentCreator);
        }

        if (this.currentUser.hasAdministratorRoles() || this.currentUser.hasRole(SystemRoleEnum.CourseContentCreator)) {
          collaboratorRoles.push(SystemRoleEnum.CourseContentCreator);
        }

        if (this.currentUser.hasAdministratorRoles() || this.currentUser.hasRole(SystemRoleEnum.CourseFacilitator)) {
          collaboratorRoles.push(SystemRoleEnum.CourseFacilitator);
        }
      } else {
        if (this.currentUser.hasAdministratorRoles() || this.currentUser.hasRole(SystemRoleEnum.DivisionTrainingCoordinator)) {
          collaboratorRoles.push(SystemRoleEnum.DivisionTrainingCoordinator);
        }
        if (this.currentUser.hasAdministratorRoles() || this.currentUser.hasRole(SystemRoleEnum.SchoolStaffDeveloper)) {
          collaboratorRoles.push(SystemRoleEnum.SchoolStaffDeveloper);
        }
        if (this.currentUser.hasAdministratorRoles() || this.currentUser.hasRole(SystemRoleEnum.DivisionAdministrator)) {
          collaboratorRoles.push(SystemRoleEnum.DivisionAdministrator);
        }
        if (this.currentUser.hasAdministratorRoles() || this.currentUser.hasRole(SystemRoleEnum.SchoolAdministrator)) {
          collaboratorRoles.push(SystemRoleEnum.SchoolAdministrator);
        }
        if (this.currentUser.hasAdministratorRoles() || this.currentUser.hasRole(SystemRoleEnum.BranchAdministrator)) {
          collaboratorRoles.push(SystemRoleEnum.BranchAdministrator);
        }
      }
    } else {
      collaboratorRoles.push(SystemRoleEnum.SystemAdministrator);
    }

    const _fetchCollaboratorsFn = UserUtils.createFetchUsersWithCheckMoreDataFn(collaboratorRoles, this.userRepository);

    this.fetchCollaboratorsFn = (searchText: string, skipCount: number, maxResultCount: number) =>
      _fetchCollaboratorsFn(searchText, skipCount, maxResultCount).pipe(
        map(p => {
          this._hasMoreDataToFetch = p.hasMoreData;
          return p.items
            .map(a => new BaseUserInfo(a))
            .filter(x => x.id !== this.currentUser.id)
            .filter(x => !this.currentCollaborator.includes(x.id));
        })
      );
  }

  private createFilterCollaboratorFn(): void {
    this.filterCollaboratorFn = (term: string, item: BaseUserInfo) =>
      term
        ? item.fullName.toLowerCase().includes(term.toLowerCase()) ||
          item.emailAddress.toLowerCase().includes(term.toLowerCase()) ||
          item.departmentName.toLowerCase().includes(term.toLowerCase())
        : true;
  }
}
