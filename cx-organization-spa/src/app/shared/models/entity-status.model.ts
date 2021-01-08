export class EntityStatus {
  expirationDate?: string;
  activeDate?: string;
  externallyMastered?: boolean;
  lastExternallySynchronized?: any;
  entityVersion?: any;
  lastUpdated?: any;
  lastUpdatedBy?: number;
  statusId?: any;
  statusReasonId?: any;
  deleted?: boolean;
  constructor(data?: Partial<EntityStatus>) {
    if (!data) {
      return;
    }
    this.expirationDate = data.expirationDate ? data.expirationDate : undefined;
    this.activeDate = data.activeDate ? data.activeDate : undefined;
    this.externallyMastered =
      data.externallyMastered !== undefined
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
