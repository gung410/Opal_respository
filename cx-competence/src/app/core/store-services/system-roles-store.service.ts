import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { SystemRole } from '../models/system-role';
import { SystemRolesDataService } from '../store-data-services/system-roles-data.service';
import { BaseArrayStoreService } from './base-array-store.service';

@Injectable({
  providedIn: 'root',
})
export class SystemRolesStoreService extends BaseArrayStoreService<
  Array<SystemRole>
> {
  constructor(private systemRolesDataService: SystemRolesDataService) {
    super();
  }
  get(): BehaviorSubject<Array<SystemRole>> {
    const currentValue = this.checkForCurrentData();
    if (currentValue) {
      return this.storeSubject;
    }
    this.systemRolesDataService.getSystemRoles().subscribe(
      (roles: any) => {
        this.storeSubject.next(roles);
      },
      (error) => {
        this.storeSubject.error(error);
      }
    );

    return this.storeSubject;
  }

  add(element: any): BehaviorSubject<SystemRole[]> {
    throw new Error('Method not implemented.');
  }
  edit(element: any): BehaviorSubject<SystemRole[]> {
    throw new Error('Method not implemented.');
  }
  delete(element: any): BehaviorSubject<SystemRole[]> {
    throw new Error('Method not implemented.');
  }
}
