import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { Comment, IComment } from '../models/comment';
import { ISearchCommentResult, SearchCommentResult } from '../dtos/search-comment-result';
import { ISeenCommentModel, SeenCommentModel } from './../models/seen-comment.model';

import { CommentServiceType } from '../models/comment-service-type';
import { ICreateCommentRequest } from '../dtos/create-comment-request';
import { IGetCommentNotSeenRequest } from './../dtos/get-comment-not-seen-request';
import { ISearchCommentRequest } from '../dtos/search-comment-request';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/internal/operators/map';

@Injectable()
export class CommentApiService extends BaseBackendService {
  protected serviceType: CommentServiceType = CommentServiceType.DigitalContent;

  protected get apiUrl(): string {
    switch (this.serviceType) {
      case CommentServiceType.DigitalContent:
        return AppGlobal.environment.contentApiUrl + '/content';
      case CommentServiceType.Form:
        return AppGlobal.environment.formApiUrl + '/form';
      case CommentServiceType.LnaForm:
        return AppGlobal.environment.lnaFormApiUrl + '/form';
      case CommentServiceType.Course:
        return AppGlobal.environment.courseApiUrl + '/course';
      default:
        return AppGlobal.environment.apiUrl;
    }
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public initApiService(serviceType: CommentServiceType): void {
    this.serviceType = serviceType;
  }

  public searchComments(request: ISearchCommentRequest, showSpinner: boolean = false): Promise<ISearchCommentResult> {
    return this.post<ISearchCommentRequest, ISearchCommentResult>('/comment/search', request, showSpinner)
      .pipe(map(data => new SearchCommentResult(data)))
      .toPromise();
  }

  public saveComment(request: ICreateCommentRequest, showSpinner: boolean = true): Promise<IComment> {
    return this.post<ICreateCommentRequest, IComment>('/comment/create', request, showSpinner)
      .pipe(map(comment => new Comment(comment)))
      .toPromise();
  }

  public getCommentNotSeen(request: IGetCommentNotSeenRequest, showSpinner: boolean = false): Promise<ISeenCommentModel[]> {
    return this.post<IGetCommentNotSeenRequest, ISeenCommentModel[]>('/comment/getCommentNotSeen', request, showSpinner)
      .pipe(map(data => data.map(x => new SeenCommentModel(x))))
      .toPromise();
  }
}
