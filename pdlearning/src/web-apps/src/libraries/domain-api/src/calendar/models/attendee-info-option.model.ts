import { PublicUserInfo } from './../../share/models/user-info.model';
export class AttendeeInfoOption extends PublicUserInfo {
  public disabled: boolean;

  constructor(user: PublicUserInfo, disable: boolean = false) {
    super(user);
    this.disabled = disable;
  }
}
