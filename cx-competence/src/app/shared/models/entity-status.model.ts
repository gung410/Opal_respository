export class EntityStatus {
  public expirationDate?: string;
  public externallyMastered?: boolean;
  public lastExternallySynchronized?: any;
  public entityVersion?: any;
  public lastUpdated?: any;
  public lastUpdatedBy?: number;
  public statusId?: any;
  public statusReasonId?: any;
  public deleted?: boolean;
  constructor(data?: Partial<EntityStatus>) {
    if (!data) {
      return;
    }
    this.expirationDate = data.expirationDate ? data.expirationDate : undefined;
    this.externallyMastered = data.externallyMastered
      ? data.externallyMastered
      : undefined;
    this.lastExternallySynchronized = data.lastExternallySynchronized
      ? data.lastExternallySynchronized
      : undefined;
    this.entityVersion = data.entityVersion ? data.entityVersion : undefined;
    this.lastUpdated = data.lastUpdated ? data.lastUpdated : undefined;
    this.lastUpdatedBy = data.lastUpdatedBy ? data.lastUpdatedBy : undefined;
    this.statusId = data.statusId ? data.statusId : undefined;
    this.statusReasonId = data.statusReasonId ? data.statusReasonId : undefined;
    this.deleted = data.deleted ? data.deleted : undefined;
  }
}
