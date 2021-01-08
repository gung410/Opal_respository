import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

import { SystemRole } from '../models/system-role';
import { LearningFrameWorksDataService } from '../store-data-services/learning-frame-works-data';
import { BaseArrayStoreService } from './base-array-store.service';

@Injectable({
  providedIn: 'root'
})
export class LearningFrameWorksStoreService extends BaseArrayStoreService<
  SystemRole[]
> {
  constructor(
    private learningFrameWorksDataService: LearningFrameWorksDataService
  ) {
    super();
  }
  get(): BehaviorSubject<SystemRole[]> {
    const currentValue = this.checkForCurrentData();
    if (currentValue) {
      return this.storeSubject;
    }
    this.learningFrameWorksDataService.getLearningFrameWorks().subscribe(
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
