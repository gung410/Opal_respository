import { Observable, of } from 'rxjs';

import { CommentRepository } from '../repositories/comment.repository';
import { CommentViewModel } from '../models/comment-view-model';
import { EntityCommentType } from '../models/entity-comment-type';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Injectable } from '@angular/core';
import { UserRepository } from '../../user/repositories/user.repository';
import { Utils } from '@opal20/infrastructure';
import { switchMap } from 'rxjs/operators';

@Injectable()
export class CommentComponentService {
  constructor(private userRepository: UserRepository, private commentRepository: CommentRepository) {}

  public loadComments(
    objectId: string,
    entityCommentType: EntityCommentType,
    actionType: string | null,
    skipCount: number,
    maxResultCount: number,
    showSpinner: boolean = false
  ): Observable<GridDataResult> {
    return this.commentRepository.searchComments(objectId, entityCommentType, actionType, skipCount, maxResultCount, showSpinner).pipe(
      switchMap(commentSearchResult => {
        if (commentSearchResult.totalCount === 0) {
          return of(null);
        }
        return this.userRepository
          .loadPublicUserInfoList({ userIds: Utils.uniq(commentSearchResult.items.map(comment => comment.userId)) }, showSpinner)
          .pipe(
            switchMap(usersList => {
              const userDic = Utils.toDictionary(usersList, p => p.id);
              const searchVmResult = <GridDataResult>{
                data: commentSearchResult.items.map(comment => CommentViewModel.createFromModel(comment, userDic[comment.userId])),
                total: commentSearchResult.totalCount
              };
              return of(searchVmResult);
            })
          );
      })
    );
  }
}
