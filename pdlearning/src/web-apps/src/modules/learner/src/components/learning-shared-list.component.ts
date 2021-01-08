import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MetadataTagModel, TrackingSharedDetailByModel } from '@opal20/domain-api';

import { LearningType } from '../models/learning-item.model';
import { Observable } from 'rxjs';
import { UserTrackingService } from '../user-activities-tracking/user-tracking.service';

const pageSize = 10;
@Component({
  selector: 'learning-shared-list',
  templateUrl: './learning-shared-list.component.html'
})
export class LearningSharedListComponent extends BaseComponent implements OnInit {
  public get isEmpty(): boolean {
    return this.totalCount != null && this.totalCount === 0;
  }

  public get canLoadMore(): boolean {
    return this.pageNumber * pageSize <= this.totalCount;
  }
  @Input()
  public sharedItems: TrackingSharedDetailByModel[] = [];
  @Input()
  public sharedMetadata: Observable<Dictionary<MetadataTagModel>>;
  @Input()
  public totalCount: number = 0;
  @Input()
  public isShowMore: boolean = false;
  @Input()
  public numberOfHeightTimes: number;
  @Output()
  public sharedClick: EventEmitter<TrackingSharedDetailByModel> = new EventEmitter<TrackingSharedDetailByModel>();
  @Output()
  public showMoreSharedClick: EventEmitter<void> = new EventEmitter<void>();

  public pageNumber: number = 1;

  constructor(protected moduleFacadeService: ModuleFacadeService, private userTrackingService: UserTrackingService) {
    super(moduleFacadeService);
  }

  public onShowMoreClicked(): void {
    this.showMoreSharedClick.emit();
  }

  public onSharedClicked(event: TrackingSharedDetailByModel): void {
    this.sharedClick.emit(event);
  }

  public onLoadMoreClicked(): void {
    this.increasePageNumber();
    this.loadSharedItems(pageSize, pageSize * (this.pageNumber - 1));
  }

  public getTagName(metadata: Dictionary<MetadataTagModel>, tagId: string, itemType: string): string {
    if (itemType === LearningType.DigitalContent) {
      return tagId;
    } else {
      return metadata[tagId] && metadata[tagId].displayText;
    }
  }

  private loadSharedItems(maxResultCount: number, skipCount: number): void {
    this.userTrackingService.getSharedToIncludedTag(skipCount, maxResultCount).then(result => {
      this.sharedItems = this.sharedItems.concat(result.items);
      this.totalCount = result.totalCount;
    });
  }

  private increasePageNumber(): void {
    this.pageNumber = this.pageNumber + 1;
  }
}
