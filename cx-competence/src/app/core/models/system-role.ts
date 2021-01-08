export class SystemRole {
  constructor(
    public localizedData: LocalizedDataItem[],
    public identity: SystemRoleIdentity,
    public entityStatus: SystemRoleEntityStatus
  ) {}
}

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

export class LocalizedDataItem {
  constructor(
    public id: number,
    public languageCode: string,
    public fields: { name: string; localizedText: string }[]
  ) {}
}
