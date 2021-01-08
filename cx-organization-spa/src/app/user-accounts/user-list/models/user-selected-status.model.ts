export class UserStatusDisplayActionModel {
  allPending1st: boolean;
  allPending2nd: boolean;
  allPending3rd: boolean;
  allNew: boolean;
  allActive: boolean;
  allSupspended: boolean;
  allLocked: boolean;
  allDeactive: boolean;
  anyNew: boolean;
  anyActive: boolean;
  anySupspended: boolean;
  anyLocked: boolean;
  anyDeactive: boolean;
  constructor(data?: Partial<UserStatusDisplayActionModel>) {
    if (!data) {
      return;
    }
    this.allPending1st = data.allPending1st ? data.allPending1st : false;
    this.allPending2nd = data.allPending2nd ? data.allPending2nd : false;
    this.allPending3rd = data.allPending3rd ? data.allPending3rd : false;
    this.allNew = data.allNew ? data.allNew : false;
    this.allActive = data.allActive ? data.allActive : false;
    this.allSupspended = data.allSupspended ? data.allSupspended : false;
    this.allLocked = data.allLocked ? data.allLocked : false;
    this.allDeactive = data.allDeactive ? data.allDeactive : false;
    this.anyNew = data.anyNew ? data.anyNew : false;
    this.anyActive = data.anyActive ? data.anyActive : false;
    this.anySupspended = data.anySupspended ? data.anySupspended : false;
    this.anyLocked = data.anyLocked ? data.anyLocked : false;
    this.anyDeactive = data.anyDeactive ? data.anyDeactive : false;
  }
}
