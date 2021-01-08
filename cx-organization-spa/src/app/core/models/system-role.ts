import { LocalizedDataItem } from 'app-models/localized-data-item.model';

export class SystemRole {
  constructor(
    public localizedData: LocalizedDataItem[],
    public identity: SystemRoleIdentity,
    public entityStatus: SystemRoleEntityStatus
  ) {}
}

// tslint:disable:max-classes-per-file
export class SystemRoleIdentity {
  constructor(
    public extId: string,
    public ownerId: number,
    public customerId: number,
    public archetype: string,
    public id: number
  ) {}
}

export class SystemRoleEntityStatus {
  constructor(
    public externallyMastered?: boolean,
    public lastUpdated?: string,
    public lastUpdatedBy?: number,
    public statusId?: string,
    public statusReasonId?: string,
    public deleted?: boolean
  ) {}
}
