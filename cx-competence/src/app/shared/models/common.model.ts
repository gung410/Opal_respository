import { environment } from 'app-environments/environment';

export class Identity {
  extId?: string;
  ownerId?: number;
  customerId?: number;
  archetype?: string;
  id?: number;

  constructor(data?: Partial<Identity>) {
    if (!data) {
      this.ownerId = environment.OwnerId;
      this.customerId = environment.CustomerId;

      return;
    }
    this.extId = data.extId ? data.extId : undefined;
    this.ownerId = data.ownerId ? data.ownerId : environment.OwnerId;
    this.customerId = data.customerId
      ? data.customerId
      : environment.CustomerId;
    this.archetype = data.archetype ? data.archetype : undefined;
    this.id = data.id ? data.id : 0;
  }
}
