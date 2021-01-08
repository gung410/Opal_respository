import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

import { BaseStoreService } from './base-store.service';

@Injectable({
  providedIn: 'root'
})
export class GlobalKeySearchStoreService extends BaseStoreService<{
  isSearch: boolean;
  searchKey: string;
}> {
  constructor() {
    super();
  }

  get(): BehaviorSubject<{ isSearch: boolean; searchKey: string }> {
    return this.storeSubject;
  }

  edit(value: { isSearch: boolean; searchKey: string }): void {
    return this.storeSubject.next(value);
  }
}
