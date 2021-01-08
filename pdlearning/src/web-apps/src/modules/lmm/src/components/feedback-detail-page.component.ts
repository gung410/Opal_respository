import { BasePageComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { Course, CourseLearnerRepository, IPageUserReviewRequest, UserReviewItemType } from '@opal20/domain-api';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';

import { ListFeedbackGridComponentService } from '../services/list-feedback-grid-component.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'feedback-detail-page',
  templateUrl: 'feedback-detail-page.component.html'
})
export class FeedbackDetailPageComponent extends BasePageComponent {
  public get course(): Course {
    return this._course;
  }

  @Input()
  public set course(v: Course) {
    if (!Utils.isDifferent(this._course, v)) {
      return;
    }
    this._course = v;
  }

  public get classRunId(): string | null {
    return this._classrunId;
  }

  @Input()
  public set classRunId(v: string | null) {
    if (!Utils.isDifferent(this._classrunId, v)) {
      return;
    }
    this._classrunId = v;
  }

  public state: PageChangeEvent = {
    skip: 0,
    take: 10
  };

  public get showLoadMore(): boolean {
    return this.gridView != null && this.gridView.total > (this.state.skip + 1) * this.state.take;
  }

  public gridView: GridDataResult;
  public averageRating: number = 0;
  public totalVotes: number;
  private _course: Course = new Course();
  private _classrunId: string | null;
  private _loadFeedbacksSub: Subscription = new Subscription();

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    public listFeedbackGridSvc: ListFeedbackGridComponentService,
    public courseLearnerRepository: CourseLearnerRepository
  ) {
    super(moduleFacadeService);
  }

  public loadFeedbacks(showSpinner: boolean = false): void {
    this._loadFeedbacksSub.unsubscribe();
    const request: IPageUserReviewRequest = {
      itemId: this.course.id,
      classrunId: this.classRunId,
      itemTypeFilter: [UserReviewItemType.Course],
      maxResultCount: (this.state.skip + 1) * this.state.take,
      orderBy: 'CreatedDate DESCENDING',
      skipCount: 0
    };
    this._loadFeedbacksSub = this.listFeedbackGridSvc
      .loadFeedbacks(request, showSpinner)
      .pipe(this.untilDestroy())
      .subscribe(data => {
        if (data != null) {
          if (data.feedbackGridDataResult != null && data.feedbackGridDataResult.data != null) {
            data.feedbackGridDataResult.data.reverse();
          }
          this.gridView = data.feedbackGridDataResult;
          this.totalVotes = data.feedbackGridDataResult.total;
          this.averageRating = data.rating;
        }
      });
  }

  public onShowMore(): void {
    this.state.take++;
    this.loadFeedbacks();
  }

  public canViewRating(): boolean {
    return this.course.isMicrolearning();
  }

  protected onInit(): void {
    this.loadFeedbacks();
  }
}
