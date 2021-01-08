import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import {
  CommentActionEnum,
  CommentChangeData,
  CommentData,
  CommentResultDto,
} from 'app/individual-development/cx-comment/comment.model';
import { PDEvaluationModel } from 'app/individual-development/models/pd-evaluation.model';
import { Observable } from 'rxjs';
import { v4 as uuid } from 'uuid';
import { APIConstant } from '../app.constant';
import { CommentServiceHelpers } from '../helpers/comment-service.helpers';
import { CommentEventEntity } from './comment-event.constant';

@Injectable({
  providedIn: 'root',
})
export class CommentService {
  private baseUrl: string = `${APIConstant.BASE_URL_COMPETENCE}`;

  constructor(
    private httpHelpers: HttpHelpers,
    private commentServiceHelpers: CommentServiceHelpers
  ) {}

  saveComment(
    commentEventEntity: CommentEventEntity,
    resultExtId: string,
    commentData: CommentChangeData
  ): Observable<CommentData> {
    const correlationId = uuid();
    const headers = new HttpHeaders({ 'Correlation-Id': correlationId });
    const commentRoute = this.buildCommentRoute(
      commentEventEntity,
      resultExtId
    );
    const apiRequestUrl = `${this.baseUrl}/${commentRoute}`;
    if (commentData.action === CommentActionEnum.ADD) {
      return this.httpHelpers.post<CommentData>(
        apiRequestUrl,
        commentData.commentItem,
        undefined,
        { headers }
      );
    }

    if (commentData.action === CommentActionEnum.UPDATE) {
      return this.httpHelpers.put<CommentData>(
        apiRequestUrl,
        commentData.commentItem,
        undefined,
        { headers }
      );
    }

    return this.httpHelpers.delete<CommentData>(
      apiRequestUrl,
      commentData.commentItem,
      undefined,
      { headers }
    );
  }

  saveComments(
    commentEventEntity: CommentEventEntity,
    newCommentDtos: CommentResultDto[]
  ): Observable<void> {
    const headers = new HttpHeaders({ 'Correlation-Id': uuid() });
    const savecommentsRoute = this.buildCommentsRoute(commentEventEntity);
    const apiRequestUrl = `${this.baseUrl}/${savecommentsRoute}`;

    return this.httpHelpers.post(apiRequestUrl, newCommentDtos, undefined, {
      headers,
    });
  }

  saveEvaluationComment(
    eventEntity: CommentEventEntity,
    resultExtId: string,
    evaluationModel: PDEvaluationModel
  ): Observable<CommentData> {
    const commentData = this.commentServiceHelpers.buildEvaluationCommentChangeData(
      evaluationModel
    );

    return this.saveComment(eventEntity, resultExtId, commentData);
  }

  /**
   * Gets all comments.
   * @param commentEventEntity The comment event entity type.
   * @param resultExtId The result external identifier.
   */
  getComments(
    commentEventEntity: CommentEventEntity,
    resultExtId: string
  ): Observable<CommentData[]> {
    const commentRoute = this.buildCommentRoute(
      commentEventEntity,
      resultExtId
    );

    return this.httpHelpers.get<CommentData[]>(
      `${this.baseUrl}/${commentRoute}`
    );
  }

  /**
   * Gets all comments asynchronously.
   * @param commentEventEntity The comment event entity type.
   * @param resultExtId The result external identifier.
   */
  async getCommentsAsync(
    commentEventEntity: CommentEventEntity,
    resultExtId: string
  ): Promise<CommentData[]> {
    return await this.getComments(commentEventEntity, resultExtId).toPromise();
  }

  async saveCommentAsync(
    eventEntity: CommentEventEntity,
    entityId: string,
    evaluationModel: PDEvaluationModel
  ): Promise<boolean> {
    try {
      await this.saveEvaluationComment(
        eventEntity,
        entityId,
        evaluationModel
      ).toPromise();

      return true;
    } catch (e) {
      return false;
    }
  }

  /**
   * Builds the route for the comments of a PD Plan object.
   * @param commentEventEntity The comment event entity type.
   * @param resultExtId The result external identifier.
   */
  buildCommentRoute(
    commentEventEntity: CommentEventEntity,
    resultExtId: string
  ): string {
    const commentEventEntitySegments = commentEventEntity.toString().split('.');
    const pdPlanType = commentEventEntitySegments[0];
    const pdPlanActivity = commentEventEntitySegments[1];

    return `${pdPlanType}/${pdPlanActivity}/results/extid/${resultExtId}/comments`;
  }

  /**
   * Builds the route for the comments of a PD Plan object.
   * @param commentEventEntity The comment event entity type.
   * @param resultExtId The result external identifier.
   */
  buildCommentsRoute(commentEventEntity: CommentEventEntity): string {
    const commentEventEntitySegments = commentEventEntity.toString().split('.');
    const pdPlanType = commentEventEntitySegments[0];
    const pdPlanActivity = commentEventEntitySegments[1];

    return `${pdPlanType}/${pdPlanActivity}/results/comments`;
  }
}
