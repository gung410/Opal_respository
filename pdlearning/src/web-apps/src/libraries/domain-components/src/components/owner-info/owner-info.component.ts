import { BaseUserInfo, CourseUser, PublicUserInfo, UserInfoModel } from '@opal20/domain-api';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'owner-info',
  templateUrl: './owner-info.component.html'
})
export class OwnerInfoComponent {
  @Input() public owner: BaseUserInfo | UserInfoModel | PublicUserInfo | null;
  @Input() public iconLocation: string;
  /** Reflect to css class */
  @Input() public iconPosition: 'top-left' | 'top-right' | 'bottom-left' | 'bottom-right' = 'top-left';

  public get ownerId(): string {
    if (!this.owner) {
      return '';
    }

    if (this.owner instanceof BaseUserInfo || this.owner instanceof UserInfoModel || this.owner instanceof CourseUser) {
      return this.owner.id;
    }

    return this.owner.userCxId;
  }

  public get isMine(): boolean {
    return UserInfoModel.getMyUserInfo().extId === this.ownerId.toLocaleLowerCase();
  }

  public get avatarUrl(): string {
    return this.owner ? (this.owner instanceof UserInfoModel ? this.owner.avatarUrl : this.owner.avatarUrl) : '';
  }

  public get fullName(): string {
    return this.owner ? (this.owner instanceof UserInfoModel ? this.owner.fullName : this.owner.fullName) : '';
  }

  public get emailAddress(): string {
    return this.owner ? (this.owner instanceof UserInfoModel ? this.owner.emailAddress : this.owner.emailAddress) : '';
  }
}
