import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { OdpDataService } from '../store-data-services/odp-data.service';
import { BaseStoreService } from './base-store.service';

@Injectable({
  providedIn: 'root',
})
export class DirectionStoreService extends BaseStoreService<any> {
  constructor(private odpDataService: OdpDataService) {
    super();
  }

  get(): BehaviorSubject<any> {
    const currentValue = this.checkForCurrentData();
    if (currentValue) {
      return this.storeSubject;
    }
    this.odpDataService.getDirectionConfig().subscribe(
      (directionConfig: any) => {
        this.storeSubject.next(directionConfig);
      },
      (error) => {
        this.storeSubject.error(error);
      }
    );

    return this.storeSubject;
  }

  edit(obj: any): BehaviorSubject<any> {
    throw new Error('Method not implemented.');
  }
}
