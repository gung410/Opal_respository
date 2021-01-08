import * as moment from 'moment';

import { BaseComponent, LocalTranslatorService, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { LearningStatus, MetadataTagModel, MyClassRunModel, MyCourseStatus, MyRegistrationStatus } from '@opal20/domain-api';

import { CourseDataService } from '../services/course-data.service';
import { LearningActionService } from '../services/learning-action.service';
import { LearningItemModel } from '../models/learning-item.model';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { toDisplayStatusFromRegistrationStatus } from '../learner-utils';

@Component({
  selector: 'learning-long-card',
  templateUrl: './learning-long-card.component.html'
})
export class LearningLongCardComponent extends BaseComponent {
  @Input()
  public learningCardItem: LearningItemModel;
  @Input()
  public showProgressBar: boolean = true;
  @Input()
  public showUpcomingSession: boolean = true;
  @Input()
  public showBookmark: boolean = true;
  @Input()
  public displayOnlyMode: boolean = false;

  @Output()
  public learningCardClick: EventEmitter<LearningItemModel> = new EventEmitter<LearningItemModel>();

  public myCourseStatus: typeof MyCourseStatus = MyCourseStatus;

  public courseMetadata$: Observable<Dictionary<MetadataTagModel>>;
  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    public translator: LocalTranslatorService,
    protected learningActionService: LearningActionService,
    protected courseDataService: CourseDataService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    if (!this.learningCardItem) {
      return;
    }
    this.courseMetadata$ = this.courseDataService.getCourseMetadata().pipe(map(tags => Utils.toDictionary(tags, p => p.id)));

    this.learningActionService.bookmarkChanged.pipe(this.untilDestroy()).subscribe(bookmarkChanged => {
      if (this.learningCardItem.id === bookmarkChanged.itemId) {
        this.learningCardItem.course.bookmarkInfo = bookmarkChanged.data;
      }
    });
  }

  public onBookmarkChange(): void {
    this.learningCardItem.isBookmark
      ? this.learningActionService.unBookmark(this.learningCardItem.id, this.learningCardItem.getCourseBookmarkType)
      : this.learningActionService.bookmark(this.learningCardItem.id, this.learningCardItem.getCourseBookmarkType);
  }

  public onLearningCardClick(): void {
    this.learningCardClick.emit(this.learningCardItem);
  }

  public getTagName(metadata: Dictionary<MetadataTagModel>, tagId: string): string {
    return metadata[tagId] && metadata[tagId].displayText;
  }

  public get canShowStatus(): boolean {
    return this.learningCardItem.learningStatusText !== '';
  }

  public get canShowProgressBar(): boolean {
    return this.showProgressBar && (this.learningCardItem.isInProgress || this.learningCardItem.isCompleted);
  }

  public getCompletedDate(): string {
    return moment(this.learningCardItem.completedDate).format('DD/MM/YYYY');
  }

  public onClassRunChanged(classRun: MyClassRunModel): void {
    const isRejected = classRun.status === MyRegistrationStatus.OfferRejected || classRun.status === MyRegistrationStatus.WaitlistRejected;

    if (isRejected) {
      this.rejectClassRun(classRun);
    }
    const previousClassIndex = this.learningCardItem.myClassRuns.findIndex(p => p.registrationId === classRun.registrationId);

    const nextClassRun = this.learningCardItem.myClassRuns.find(p => p.learningStatus === LearningStatus.NotStarted);
    if (nextClassRun) {
      const displayStatus = toDisplayStatusFromRegistrationStatus(nextClassRun.status, nextClassRun.registrationType);
      this.learningCardItem.course.myCourseInfo.displayStatus = displayStatus;
      this.learningCardItem.myClassRuns.splice(previousClassIndex, 1);
      const nextClassRunDetail = this.learningCardItem.classRunsDetail.find(p => p.id === nextClassRun.classRunId);
      if (nextClassRunDetail) {
        this.learningCardItem.course.classRunDetail = nextClassRunDetail;
      }

      this.learningCardItem = Utils.cloneDeep(this.learningCardItem);
    }
  }

  private rejectClassRun(classRun: MyClassRunModel): void {
    const myClassRunIndex = this.learningCardItem.myClassRuns.findIndex(p => p.registrationId === classRun.registrationId);
    if (myClassRunIndex === -1) {
      return;
    }

    const displayStatus = toDisplayStatusFromRegistrationStatus(classRun.status, classRun.registrationType);

    this.learningCardItem.course.myCourseInfo.displayStatus = displayStatus;
    this.learningCardItem.course.myClassRuns.splice(myClassRunIndex, 1);
    this.learningCardItem.course.updateClassRunDetail();
    this.learningCardItem = Utils.cloneDeep(this.learningCardItem);
  }
}
