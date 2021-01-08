export interface IBroadcastNotification {
  active?: boolean;
  defaultBody?: string;
  defaultSubject?: string;
  startDateUtc?: Date;
  endDateUtc?: Date;
  isGlobal?: boolean;
}

export class BroadcastNotification implements IBroadcastNotification {
  active?: boolean;
  defaultBody?: string;
  defaultSubject?: string;
  startDateUtc?: Date;
  endDateUtc?: Date;
  isGlobal?: boolean;

  constructor(data?: Partial<IBroadcastNotification>) {
    if (data == null) {
      return;
    }
    this.active = data.active;
    this.defaultBody = data.defaultBody;
    this.defaultSubject = data.defaultSubject;
    this.startDateUtc = data.startDateUtc;
    this.endDateUtc = data.endDateUtc;
    this.isGlobal = data.isGlobal;
  }
}
