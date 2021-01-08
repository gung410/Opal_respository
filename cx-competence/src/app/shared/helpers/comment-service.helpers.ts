import { Injectable } from '@angular/core';
import { UserService } from 'app-services/user.service';
import {
  CommentActionEnum,
  CommentChangeData,
  CommentData,
  CommentTagTypeEnum,
} from 'app/individual-development/cx-comment/comment.model';
import { PDEvaluationType } from 'app/individual-development/idp.constant';
import { PDEvaluationModel } from 'app/individual-development/models/pd-evaluation.model';

@Injectable()
export class CommentServiceHelpers {
  constructor(private userService: UserService) {}

  buildEvaluationCommentChangeData(
    evaluationModel: PDEvaluationModel
  ): CommentChangeData {
    let tag = CommentTagTypeEnum.Approval;
    if (evaluationModel.type === PDEvaluationType.Reject) {
      tag = CommentTagTypeEnum.Rejection;
    }

    const now = new Date().toISOString();
    const commentItem = new CommentData({
      content: evaluationModel.reason,
      tag,
      created: now,
      owner: this.userService.getCurrentUserBasicInfo(),
    });

    return new CommentChangeData(commentItem, CommentActionEnum.ADD);
  }
}
