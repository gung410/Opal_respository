import { Observable, from } from 'rxjs';

import { BaseRepository } from '@opal20/infrastructure';
import { Comment } from '../models/comment';
import { CommentApiService } from '../services/comment-api.services';
import { CommentRepositoryContext } from '../comment-repository-context';
import { EntityCommentType } from '../models/entity-comment-type';
import { Injectable } from '@angular/core';
import { SearchCommentResult } from '../dtos/search-comment-result';

@Injectable()
export class CommentRepository extends BaseRepository<CommentRepositoryContext> {
  constructor(context: CommentRepositoryContext, private commentApiService: CommentApiService) {
    super(context);
  }

  public searchComments(
    objectId: string,
    entityCommentType: EntityCommentType,
    actionType?: string | null,
    skipCount: number = 0,
    maxResultCount: number = 25,
    showSpinner: boolean = false
  ): Observable<SearchCommentResult> {
    return this.processUpsertData<Comment, SearchCommentResult>(
      this.context.contentsSubject,
      () =>
        from(
          this.commentApiService.searchComments(
            {
              objectId: objectId,
              entityCommentType: entityCommentType,
              actionType: actionType,
              pagedInfo: {
                skipCount: skipCount,
                maxResultCount: maxResultCount
              }
            },
            showSpinner
          )
        ),
      'searchComment',
      [objectId, entityCommentType, actionType, skipCount, maxResultCount],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _);
        return apiResult;
      },
      apiResult => apiResult.items.map(comment => new Comment(comment)),
      x => x.id
    );
  }
}
