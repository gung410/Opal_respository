import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { MetadataTagModel, TrackingSharedDetailByModel } from '@opal20/domain-api';

import { CourseDataService } from '../services/course-data.service';
import { Observable } from 'rxjs';
import { UserTrackingService } from '../user-activities-tracking/user-tracking.service';
import { map } from 'rxjs/operators';

const pageSize = 10;
@Component({
  selector: 'learning-shared-page',
  templateUrl: './learning-shared-page.component.html'
})
export class LearningSharedPageComponent extends BaseComponent implements OnInit {
  @Output()
  public sharedClick: EventEmitter<TrackingSharedDetailByModel> = new EventEmitter<TrackingSharedDetailByModel>();
  @Output()
  public sharedBackClick: EventEmitter<void> = new EventEmitter<void>();

  public sharedItems: TrackingSharedDetailByModel[] = [];
  public totalCount: number = 0;
  public sharedMetadata: Observable<Dictionary<MetadataTagModel>>;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private userTrackingService: UserTrackingService,
    private courseDataService: CourseDataService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.userTrackingService.getSharedToIncludedTag(0, pageSize).then(result => {
      this.totalCount = result.totalCount;
      this.sharedItems = result.items;

      this.sharedMetadata = this.courseDataService.getCourseMetadata().pipe(map(tags => Utils.toDictionary(tags, p => p.id)));
    });
  }

  public onBackClicked(): void {
    this.sharedBackClick.emit();
  }

  public onSharedClicked(sharedItem: TrackingSharedDetailByModel): void {
    this.sharedClick.emit(sharedItem);
  }
}
