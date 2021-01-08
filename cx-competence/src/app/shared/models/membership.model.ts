import { Identity } from './common.model';

export class MemberShip {
  Role: string;
  Identity: Identity;
  EntityStatus: EntityStatus;
  constructor() {
    this.Identity = new Identity();
    this.EntityStatus = new EntityStatus();
  }
}

export class EntityStatus {
  ExternallyMastered: boolean;
  StatusId: string;
  StatusReasonId: string;
  Deleted: boolean;
}
