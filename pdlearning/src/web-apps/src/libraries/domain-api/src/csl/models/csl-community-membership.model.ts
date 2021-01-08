export interface ICommunityMemberShip {
  can_cancel_membership?: number;
  last_visit?: Date;
  member_since?: Date;
  originator_user?: string;
  request_message?: string[];
  role: string;
  send_notifications: number;
  show_at_dashboard: number;
  status: number;
  updated_at?: Date;
  user: CommunityMemberShipUser;
}

export class CommunityMemberShip implements ICommunityMemberShip {
  // tslint:disable-next-line:variable-name
  public can_cancel_membership?: number;
  // tslint:disable-next-line:variable-name
  public last_visit?: Date;
  // tslint:disable-next-line:variable-name
  public member_since?: Date;
  // tslint:disable-next-line:variable-name
  public originator_user?: string;
  // tslint:disable-next-line:variable-name
  public request_message?: string[];
  public role: string;
  // tslint:disable-next-line:variable-name
  public send_notifications: number;
  // tslint:disable-next-line:variable-name
  public show_at_dashboard: number;
  // tslint:disable-next-line:variable-name
  public status: number;
  // tslint:disable-next-line:variable-name
  public updated_at?: Date;
  public user: CommunityMemberShipUser;

  constructor(data?: ICommunityMemberShip) {
    if (data != null) {
      this.can_cancel_membership = data.can_cancel_membership;
      this.last_visit = data.last_visit ? new Date(data.last_visit) : undefined;
      this.member_since = data.member_since ? new Date(data.member_since) : undefined;
      this.originator_user = data.originator_user;
      this.request_message = data.request_message;
      this.role = data.role;
      this.send_notifications = data.send_notifications;
      this.show_at_dashboard = data.show_at_dashboard;
      this.status = data.status;
      this.updated_at = data.updated_at ? new Date(data.updated_at) : undefined;
      this.user = data.user;
    }
  }
}
export class CommunityMemberShipUser {
  public id: number;
  public guid?: string;
  // tslint:disable-next-line:variable-name
  public display_name?: string;
  public url: string;
  constructor(data?: CommunityMemberShipUser) {
    if (data != null) {
      this.id = data.id;
      this.guid = data.guid;
      this.display_name = data.display_name;
      this.url = data.url;
    }
  }
}
