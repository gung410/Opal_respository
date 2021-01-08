import { CourseLearnerRepository, IPageUserReviewRequest, UserRepository } from '@opal20/domain-api';
import { Observable, of } from 'rxjs';

import { CourseReviewViewModel } from '@opal20/domain-components';
import { IFeedbackModel } from '../models/feedback-model';
import { Injectable } from '@angular/core';
import { Utils } from '@opal20/infrastructure';
import { switchMap } from 'rxjs/operators';

@Injectable()
export class ListFeedbackGridComponentService {
  constructor(private userRepository: UserRepository, private courseLearnerRepository: CourseLearnerRepository) {}

  public loadFeedbacks(request: IPageUserReviewRequest, showSpinner: boolean = false): Observable<IFeedbackModel> {
    return this.courseLearnerRepository.loadReviews(request).pipe(
      switchMap(pagedUserReviewModelResult => {
        if (pagedUserReviewModelResult.totalCount === 0) {
          return of(null);
        }
        return this.userRepository
          .loadPublicUserInfoList({ userIds: Utils.uniq(pagedUserReviewModelResult.items.map(review => review.userId)) }, showSpinner)
          .pipe(
            switchMap(usersList => {
              const userDic = Utils.toDictionary(usersList, p => p.id);
              const feedbackGridDataResult = <OpalGridDataResult<CourseReviewViewModel>>{
                data: pagedUserReviewModelResult.items.map(review => CourseReviewViewModel.createFromModel(review, userDic[review.userId])),
                total: pagedUserReviewModelResult.totalCount
              };
              const feedbackViewModelResult = <IFeedbackModel>{
                feedbackGridDataResult: feedbackGridDataResult,
                rating: pagedUserReviewModelResult.rating
              };
              return of(feedbackViewModelResult);
            })
          );
      })
    );
  }
}
