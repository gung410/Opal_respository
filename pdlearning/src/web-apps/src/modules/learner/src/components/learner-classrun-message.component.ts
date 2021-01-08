import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import {
  ClassRun,
  CommentApiService,
  CommentServiceType,
  EntityCommentType,
  IChangeRegistrationStatusRequest,
  IPublicUserInfo,
  ISearchCommentRequest,
  MyClassRunModel,
  MyCommentActionType,
  MyRegistrationStatus,
  RegistrationApiService,
  RegistrationStatus,
  UserApiService,
  WithdrawalStatus
} from '@opal20/domain-api';
import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges, TemplateRef, ViewChild } from '@angular/core';

import { LearningItemModel } from '../models/learning-item.model';
import { OpalDialogService } from '@opal20/common-components';
@Component({
  selector: 'learner-classrun-message',
  templateUrl: './learner-classrun-message.component.html'
})
export class LearnerClassRunMessageComponent extends BaseComponent implements OnInit {
  @Input() public learningCardItem: LearningItemModel;
  @Output() public classRunChanged: EventEmitter<MyClassRunModel> = new EventEmitter<MyClassRunModel>();
  @ViewChild('commentTemplate', { static: false })
  public commentTemplate: TemplateRef<unknown>;

  public currentClassRun: MyClassRunModel;
  public classRunDetail: ClassRun;
  public commentUserInfo: IPublicUserInfo;

  public messageTitle: string = '';
  public messageDescription: string = '';
  public showMessagePopup: boolean = false;

  public registrationMessageTitle: string = '';
  public withdrawlMessageTitle: string = '';
  public showCardConfirmation: boolean;

  public canConfirm: boolean;
  public canReadRegistrationComment: boolean;
  public canReadWithdrawlComment: boolean;
  public searchCommentRequest: ISearchCommentRequest = {
    objectId: null,
    entityCommentType: EntityCommentType.Registration,
    pagedInfo: {
      skipCount: 0,
      maxResultCount: 10
    }
  };
  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private registrationApiService: RegistrationApiService,
    private opalDialogService: OpalDialogService,
    private userApiService: UserApiService,
    protected commentApiService: CommentApiService
  ) {
    super(moduleFacadeService);
    this.commentApiService.initApiService(CommentServiceType.Course);
  }

  public ngOnInit(): void {
    super.ngOnInit();
    this.initClassRunMessage();
  }

  public ngOnChanges(changes: SimpleChanges): void {
    if (changes.learningCardItem.currentValue !== changes.learningCardItem.previousValue && changes.learningCardItem.previousValue) {
      this.initClassRunMessage();
    }
  }

  public onCardMessageConfirm(): void {
    let targetStatus;
    switch (this.currentClassRun.status) {
      case MyRegistrationStatus.WaitlistPendingApprovalByLearner:
        targetStatus = MyRegistrationStatus.WaitlistConfirmed;
        break;
      case MyRegistrationStatus.OfferPendingApprovalByLearner:
        targetStatus = MyRegistrationStatus.OfferConfirmed;
        break;
    }
    this.changeRegistrationStatus(targetStatus);
  }

  public onCardMessageCancel(): void {
    let targetStatus;
    switch (this.currentClassRun.status) {
      case MyRegistrationStatus.WaitlistPendingApprovalByLearner:
        targetStatus = MyRegistrationStatus.WaitlistRejected;
        break;
      case MyRegistrationStatus.OfferPendingApprovalByLearner:
        targetStatus = MyRegistrationStatus.OfferRejected;
        break;
    }
    this.changeRegistrationStatus(targetStatus);
  }

  public onCardMessageReadComment(): void {
    this.searchCommentRequest.objectId = this.currentClassRun.registrationId;
    let userRejected;
    if (
      this.currentClassRun.withdrawalStatus === WithdrawalStatus.Rejected ||
      this.currentClassRun.status === MyRegistrationStatus.Rejected
    ) {
      userRejected = [this.currentClassRun.changedBy];
      this.searchCommentRequest.actionType =
        this.currentClassRun.withdrawalStatus === WithdrawalStatus.Rejected
          ? MyCommentActionType.registrationWithdrawnRejected
          : MyCommentActionType.registrationRejected;
    }

    if (this.currentClassRun.withdrawalStatus === WithdrawalStatus.RejectedByCA) {
      userRejected = [this.currentClassRun.administratedBy];
      this.searchCommentRequest.actionType = MyCommentActionType.registrationWithdrawnRejectedByCA;
    }
    this.userApiService
      .getPublicUserInfoList({ userIds: userRejected })
      .pipe(this.untilDestroy())
      .subscribe(publicUserInfoList => {
        if (publicUserInfoList && publicUserInfoList.length > 0) {
          this.commentUserInfo = publicUserInfoList[0];
        }
      });
    this.commentApiService.searchComments(this.searchCommentRequest).then(resp => {
      const comment = resp.items.shift();
      if (comment) {
        this.currentClassRun.comment = comment.content;
      }
    });
    this.opalDialogService.openConfirmDialog({
      confirmTitle: 'Comment',
      confirmMsg: ' ',
      noBtnText: 'Close',
      hideYesBtn: true,
      bodyTemplate: this.commentTemplate
    });
  }

  public getCommentRole(): string {
    if (
      this.currentClassRun.status === MyRegistrationStatus.Rejected ||
      this.currentClassRun.withdrawalStatus === WithdrawalStatus.Rejected
    ) {
      return 'Approving Officer';
    } else {
      return 'Course Adminitrator';
    }
  }

  private changeRegistrationStatus(targetStatus: MyRegistrationStatus): Promise<void> {
    const registrationChangeStatus: IChangeRegistrationStatusRequest = {
      ids: [this.currentClassRun.registrationId],
      status: RegistrationStatus[targetStatus],
      comment: ''
    };
    return this.registrationApiService.changeRegistrationStatus(registrationChangeStatus).then(() => {
      this.currentClassRun.status = targetStatus;
      this.updateRegistrationStatus(targetStatus);
    });
  }

  private updateRegistrationStatus(targetStatus: MyRegistrationStatus): void {
    this.currentClassRun.status = targetStatus;
    this.canReadRegistrationComment = false;
    this.canConfirm = false;
    this.classRunChanged.emit(this.currentClassRun);
  }

  private initClassRunMessage(): void {
    if (this.learningCardItem !== null) {
      this.classRunDetail = this.learningCardItem.classRunDetail;
      this.currentClassRun = this.learningCardItem.myClassRun;
      if (this.currentClassRun) {
        switch (this.currentClassRun.status) {
          case MyRegistrationStatus.WaitlistPendingApprovalByLearner:
            this.registrationMessageTitle = MESSAGE_DIALOG_CONSTANT.waitlistTitle;
            this.canConfirm = true;
            break;
          case MyRegistrationStatus.OfferPendingApprovalByLearner:
            this.registrationMessageTitle = MESSAGE_DIALOG_CONSTANT.offerTitle;
            this.canConfirm = true;
            break;
          case MyRegistrationStatus.Rejected:
            this.registrationMessageTitle = MESSAGE_DIALOG_CONSTANT.rejectedTitle;
            this.canReadRegistrationComment = true;
            break;
        }

        if (
          this.currentClassRun.withdrawalStatus === WithdrawalStatus.Rejected ||
          this.currentClassRun.withdrawalStatus === WithdrawalStatus.RejectedByCA
        ) {
          this.withdrawlMessageTitle = MESSAGE_DIALOG_CONSTANT.withdrawalRejectTitle;
          this.canReadWithdrawlComment = true;
        }
      }
    }
  }
}

const MESSAGE_DIALOG_CONSTANT = {
  waitlistTitle: 'You have been placed on the waitlist for:',
  offerTitle: 'You are now off the waitlist and have been offered to accept your registration for the class run shown below:',
  rejectedTitle: 'Your request to join was declined for Class Run:',
  withdrawalRejectTitle: 'Your withdrawal request was declined for Class Run:'
};
