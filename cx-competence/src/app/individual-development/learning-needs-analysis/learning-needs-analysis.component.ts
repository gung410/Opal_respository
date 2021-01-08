import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { CommentService } from 'app-services/comment.service';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { ToastrService } from 'ngx-toastr';

import { CommentChangeData, CommentData } from '../cx-comment/comment.model';
import { CxCommentComponent } from '../cx-comment/cx-comment.component';
import { LearningNeedsAnalysisContentComponent } from './learning-needs-analysis-content/learning-needs-analysis-content.component';
import { IdpStatusCodeEnum } from '../idp.constant';
import { CommentEventEntity } from 'app-services/comment-event.constant';
import { SubmittedLNAEventData } from '../models/pd-evaluation.model';

@Component({
  selector: 'learning-needs-analysis',
  templateUrl: './learning-needs-analysis.component.html',
  styleUrls: ['./learning-needs-analysis.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class LearningNeedsAnalysisComponent extends BaseSmartComponent
  implements OnInit {
  @Input() user: Staff;
  @Input() learningNeeds: IdpDto;
  @Input() needsResults: IdpDto[];
  @Output() submit: EventEmitter<SubmittedLNAEventData> = new EventEmitter<
    SubmittedLNAEventData
  >();
  @Output() navigateToPlan: EventEmitter<void> = new EventEmitter<void>();

  @ViewChild(CxCommentComponent) commentComponent: CxCommentComponent;
  @ViewChild(LearningNeedsAnalysisContentComponent)
  lnaContentComponent: LearningNeedsAnalysisContentComponent;

  commentEventEntity: CommentEventEntity =
    CommentEventEntity.IdpLearningNeedsAnalysis;
  learningNeedsAnalysisComments: CommentData[];
  isRejected: boolean;

  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private toastrService: ToastrService,
    private commentService: CommentService
  ) {
    super(changeDetectorRef);
  }

  ngOnInit(): void {
    this.isRejected =
      this.learningNeeds.assessmentStatusInfo.assessmentStatusCode ===
      IdpStatusCodeEnum.Rejected;
    if (this.isRejected) {
      this.commentService
        .getComments(
          this.commentEventEntity,
          this.learningNeeds.resultIdentity.extId
        )
        .subscribe((comments) => {
          this.learningNeedsAnalysisComments = comments;
        });
    }
  }

  onChangeComment(changeData: CommentChangeData): void {
    this.commentService
      .saveComment(
        this.commentEventEntity,
        this.learningNeeds.resultIdentity.extId,
        changeData
      )
      .subscribe(
        (newCommentItem) => {
          changeData.commentItem = newCommentItem;
          changeData.changeResult = true;
          this.commentComponent.changeCommentResult(changeData);
        },
        (error) => {
          console.error(error);
          this.toastrService.error(`Error occurred while saving comments.`);
        }
      );
  }

  onClickRetake(): void {
    if (!this.lnaContentComponent) {
      return;
    }

    // Wait until animation done
    const animationTime = 1000;
    setTimeout(() => {
      this.isRejected = false;
      this.lnaContentComponent.retakeLNAAsync();
    }, animationTime);
  }

  onSubmittedLearningNeeds(eventData: SubmittedLNAEventData): void {
    this.submit.emit(eventData);
  }

  navigateToPdplan(): void {
    this.navigateToPlan.emit();
  }
}
