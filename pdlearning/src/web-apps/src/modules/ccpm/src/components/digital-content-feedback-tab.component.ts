import { BasePageComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input, ViewEncapsulation } from '@angular/core';
import {
  IPageUserReviewRequest,
  PagedUserReviewModelResult,
  PublicUserInfo,
  UserRepository,
  UserReviewApiService,
  UserReviewItemType
} from '@opal20/domain-api';

import { GridDataResult } from '@progress/kendo-angular-grid';

@Component({
  selector: 'digital-content-feedback-tab',
  templateUrl: './digital-content-feedback-tab.component.html',
  encapsulation: ViewEncapsulation.None
})
export class DigitalContentFeedbackTabComponent extends BasePageComponent {
  @Input() public digitalContentId: string;
  public reviews: PagedUserReviewModelResult;
  public pageNumber: number = 0;
  public pageSize: number = 25;
  public gridView: GridDataResult;
  public userDic: Dictionary<unknown>;
  public overalRate: number = 0;
  public math: Math = Math;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private userReviewApiService: UserReviewApiService,
    private userRepository: UserRepository
  ) {
    super(moduleFacadeService);
  }

  public onPageChange(event: { skip: number }): void {
    this.pageNumber = 0;
    this.loadReview();
  }

  protected onInit(): void {
    this.loadReview();
  }

  private loadReview(): void {
    const request: IPageUserReviewRequest = {
      itemId: this.digitalContentId,
      itemTypeFilter: [UserReviewItemType.DigitalContent],
      maxResultCount: this.pageSize,
      orderBy: 'ChangedDate',
      skipCount: this.pageNumber
    };
    this.userReviewApiService.getReviews(request).then(reviews => {
      this.reviews = reviews;
      this.userRepository
        .loadUserInfoList({ userIds: Utils.uniq(reviews.items.map(review => review.userId)), pageIndex: 0, pageSize: 0 }, true)
        .subscribe(users => {
          this.userDic = Utils.toDictionary(users, p => p.id);
          this.reviews.items.map(review => {
            review.user = <PublicUserInfo>this.userDic[review.userId];
          });
          this.gridView = <GridDataResult>{
            data: reviews.items,
            total: reviews.totalCount
          };
        });
      this.overalRate = this.reviews.items.reduce((value, item) => value + item.rate, 0) / this.reviews.totalCount;
    });
  }
}
