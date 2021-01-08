import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input, ViewChild } from '@angular/core';

import { Observable } from 'rxjs';
import { OpalSelectComponent } from '@opal20/common-components';
import { PublicUserInfo } from '@opal20/domain-api';

@Component({
  selector: 'learning-path-sharing',
  templateUrl: './learning-path-sharing.component.html'
})
export class LearningPathSharingComponent extends BaseComponent {
  @ViewChild(OpalSelectComponent, { static: false }) public opalSelectComponent: OpalSelectComponent;
  @Input() public isViewMode: boolean;

  public get selectedUsers(): PublicUserInfo[] {
    return this._selectedUsers;
  }

  @Input()
  public set selectedUsers(users: PublicUserInfo[]) {
    if (Utils.isDifferent(users, this._selectedUsers)) {
      this._selectedUsers = users;
      this.selectedUserDict = Utils.toDictionary(this._selectedUsers, user => user.id);
    }
  }

  @Input()
  public fetchUsersFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<PublicUserInfo[]> = null;

  private _selectedUsers: PublicUserInfo[];
  private selectedUserDict: Dictionary<PublicUserInfo>;

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public selectSearchFn: (term: string, item: object | string | number) => boolean = () => true;

  public onSelectedItemChanged(): void {
    this.opalSelectComponent.ngSelect.handleClearClick();
  }

  public isUserIdSelected(userId: string): boolean {
    return this.selectedUserDict[userId] != null;
  }

  public onAddUser(user: PublicUserInfo): void {
    this.selectedUserDict[user.id] = user;
    this._selectedUsers.push(user);
  }

  public onRemoveUser(user: PublicUserInfo): void {
    this.selectedUserDict[user.id] = undefined;
    const userIndex = this._selectedUsers.findIndex(i => i.id === user.id);
    if (userIndex > -1) {
      this._selectedUsers.splice(userIndex, 1);
    }
  }
}
