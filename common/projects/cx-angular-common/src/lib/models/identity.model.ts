export class Identity {
  extId?: string;
  ownerId: number;
  customerId: number;
  archetype?: any;
  id?: number;
  constructor(data?: Partial<Identity>) {
    if (!data) {
      return;
    }

    this.extId = data.extId ? data.extId : '';
    this.ownerId = data.ownerId ? data.ownerId : 0;
    this.customerId = data.customerId ? data.customerId : 0;
    this.archetype = data.archetype ? data.archetype : '';
    this.id = data.id ? data.id : undefined;
  }
}
