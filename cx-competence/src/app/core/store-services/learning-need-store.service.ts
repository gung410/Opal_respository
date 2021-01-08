import { Injectable } from '@angular/core';
import { PDPlanConfig } from 'app-models/pdplan.model';
import { BehaviorSubject } from 'rxjs';
import { LearningNeedService } from '../store-data-services/learning-need.services';
import { BaseStoreService } from './base-store.service';

@Injectable({
  providedIn: 'root',
})
export class LearningNeedStoreService extends BaseStoreService<PDPlanConfig> {
  constructor(private learningNeedDataService: LearningNeedService) {
    super();
  }

  get(): BehaviorSubject<PDPlanConfig> {
    const currentValue = this.checkForCurrentData();
    if (currentValue) {
      return this.storeSubject;
    }
    this.learningNeedDataService.getLearningNeedConfig().subscribe(
      (learningNeedConfig: any) => {
        this.storeSubject.next(learningNeedConfig);
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
