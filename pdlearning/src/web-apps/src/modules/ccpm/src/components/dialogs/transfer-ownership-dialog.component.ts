import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { OrganizationRepository, OrganizationUnitLevelEnum, SystemRoleEnum, UserInfoModel, UserRepository } from '@opal20/domain-api';
import { map, take } from 'rxjs/operators';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { Observable } from 'rxjs';

@Component({
  selector: 'transfer-ownership-dialog',
  templateUrl: './transfer-ownership-dialog.component.html'
})
export class TransferOwnershipDialogComponent extends BaseComponent {
  @Input() public userAcceptableRoles?: SystemRoleEnum[];

  public selectedValue: string;
  public currentDepartmentOfUser: OrganizationUnitLevelEnum[] = [];
  public filterCollaboratorFn: (term: string, item: UserInfoModel) => boolean;
  public fetchCollaboratorsFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<UserInfoModel[]>;
  public _hasMoreDataToFetch: boolean = true;
  public schoolDepartmentId: OrganizationUnitLevelEnum[] = [OrganizationUnitLevelEnum.School];
  public acceptableDepartmentIds: OrganizationUnitLevelEnum[] = [
    OrganizationUnitLevelEnum.Branch,
    OrganizationUnitLevelEnum.Division,
    OrganizationUnitLevelEnum.Ministry
  ];

  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public dialogRef: DialogRef,
    public userRepository: UserRepository,
    private organizationRepository: OrganizationRepository
  ) {
    super(moduleFacadeService);
    this.organizationRepository.loadOrganizationalUnitsByIds([this.currentUser.departmentId]).subscribe(departmentModel => {
      const isInSchoolOrganization = departmentModel.some(d => d.departmentType === OrganizationUnitLevelEnum.School);
      this.createFetchCollaboratorsFn(isInSchoolOrganization);
      this.createFilterCollaboratorFn();
    });
  }

  public onCancel(): void {
    this.dialogRef.close();
  }
  public onProceed(): void {
    if (!this.selectedValue) {
      return;
    }

    this.modalService.showConfirmMessage(this.translate('Are you sure you want to transfer the ownership of this item?'), () => {
      if (this.userAcceptableRoles && this.userAcceptableRoles.length) {
        this.checkUserAcceptableRoles();
      } else {
        this.dialogRef.close(this.selectedValue);
      }
    });
  }

  private checkUserAcceptableRoles(): void {
    this.userRepository
      .loadUserInfoList(
        {
          userIds: [this.selectedValue],
          pageIndex: 0,
          pageSize: 0
        },
        null,
        [],
        false
      )
      .pipe(take(1))
      .subscribe(usersInfo => {
        const userInfo = usersInfo.find(p => p.extId === this.selectedValue);
        const hasAcceptableRole: boolean = userInfo.systemRoles.some(role => this.userAcceptableRoles.includes(role));
        if (!hasAcceptableRole) {
          this.modalService.showErrorMessage(
            this.translate(
              'Target user does not have the CCC/CF/CC* role. Please contact administrator to assign the target user the relevant roles before trying this again.'
            )
          );
        } else {
          this.dialogRef.close(this.selectedValue);
        }
      });
  }

  private createFilterCollaboratorFn(): void {
    this.filterCollaboratorFn = (term: string, item: UserInfoModel) =>
      term
        ? item.fullName.toLowerCase().includes(term.toLowerCase()) ||
          item.emailAddress.toLowerCase().includes(term.toLowerCase()) ||
          item.departmentName.toLowerCase().includes(term.toLowerCase())
        : true;
  }

  private createFetchCollaboratorsFn(isInSchoolOrganization: boolean): void {
    const userRoles: SystemRoleEnum[] = [];
    if (
      this.currentUser.hasAdministratorRoles() ||
      this.currentUser.hasRole(SystemRoleEnum.ContentCreator, SystemRoleEnum.CourseContentCreator, SystemRoleEnum.CourseFacilitator)
    ) {
      userRoles.push(SystemRoleEnum.ContentCreator);
      userRoles.push(SystemRoleEnum.CourseContentCreator);
      userRoles.push(SystemRoleEnum.CourseFacilitator);
    }
    const departmentIds = isInSchoolOrganization ? this.schoolDepartmentId : this.acceptableDepartmentIds;
    const _fetchCollaboratorsFn = this._createFetchUserSelectFn(userRoles, departmentIds);

    this.fetchCollaboratorsFn = (searchText: string, skipCount: number, maxResultCount: number) =>
      _fetchCollaboratorsFn(searchText, skipCount, maxResultCount).pipe(
        map(p => {
          return p.filter(x => x.id !== this.currentUser.extId);
        })
      );
  }

  private _createFetchUserSelectFn(
    inRoles: SystemRoleEnum[],
    departmentExtIds: OrganizationUnitLevelEnum[]
  ): (searchText: string, skipCount: number, maxResultCount: number) => Observable<UserInfoModel[]> {
    return (_searchText, _skipCount, _maxResultCount) =>
      this.userRepository
        .loadUserInfoList(
          {
            parentDepartmentId: [this.currentUser.departmentId],
            inRoles: inRoles,
            pageSize: _maxResultCount,
            pageIndex: _maxResultCount === 0 ? 1 : _skipCount / _maxResultCount + 1,
            filterOnSubDepartment: true,
            searchKey: _searchText,
            departmentExtIds: departmentExtIds
          },
          null,
          [],
          false
        )
        .pipe(this.untilDestroy());
  }
}
