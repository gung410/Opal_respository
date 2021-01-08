import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { SystemRoleEnum, UserInfoModel, UserRepository } from '@opal20/domain-api';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { ISelectUserDialogResult } from '../../models/select-user-dialog-result.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'select-user-dialog',
  templateUrl: './select-user-dialog.component.html'
})
export class SelectUserDialogComponent extends BaseComponent {
  @Input() public title: string;

  @Input() public inRoles: SystemRoleEnum;
  public selectedValue: string;
  public fetchUserSelectItemFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<UserInfoModel[]>;

  public static selectTransferOwnerCoursesConfig(): Partial<SelectUserDialogComponent> {
    return {
      title: 'Transfer Ownership',
      inRoles: SystemRoleEnum.CourseContentCreator
    };
  }

  constructor(public moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef, public userRepository: UserRepository) {
    super(moduleFacadeService);
    this.fetchUserSelectItemFn = this._createFetchUserSelectFn();
  }

  public onCancel(): void {
    this.dialogRef.close();
  }
  public onProceed(): void {
    const result: ISelectUserDialogResult = { id: this.selectedValue };
    this.dialogRef.close(result);
  }

  private _createFetchUserSelectFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<UserInfoModel[]> {
    return (_searchText, _skipCount, _maxResultCount) =>
      this.userRepository
        .loadUserInfoList(
          {
            parentDepartmentId: [1],
            inRoles: [SystemRoleEnum.CourseContentCreator],
            pageSize: _maxResultCount,
            pageIndex: _maxResultCount === 0 ? 1 : _skipCount / _maxResultCount + 1,
            filterOnSubDepartment: true,
            searchKey: _searchText
          },
          false
        )
        .pipe(this.untilDestroy());
  }
}
