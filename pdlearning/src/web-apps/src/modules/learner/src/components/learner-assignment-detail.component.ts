import * as moment from 'moment';

import {
  Assignment,
  AssignmentApiService,
  CommentNotification,
  CommentServiceType,
  EntityCommentType,
  IChangeMyAssignmentStatus,
  MyAssignment,
  MyAssignmentApiService,
  MyAssignmentStatus,
  ParticipantAssignmentTrack,
  QuizAssignmentFormQuestion
} from '@opal20/domain-api';
import {
  AssignmentPlayerIntegrationsService,
  CommentTabInput,
  LEARNER_PERMISSIONS,
  MY_ASSIGNMENT_STATUS_COLOR_MAP
} from '@opal20/domain-components';
import { BaseComponent, DomUtils, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { fromEvent, interval } from 'rxjs';

import { AssignmentPlayingEventPayload } from '../user-activities-tracking/user-tracking.models';
import { LearnerDetailMenu } from '../constants/learner-detail-menu';
import { MyAssignmentDataService } from '../services/my-assignment-data.service';
import { MyAssignmentDetail } from '../models/my-assignment-detail-model';
import { OpalDialogService } from '@opal20/common-components';
import { TrackingSourceService } from '../user-activities-tracking/tracking-souce.service';
import { debounce } from 'rxjs/operators';
import { v4 as uuidv4 } from 'uuid';

export class SubMenuType {
  public text: string;
  public section: ElementRef;
}
@Component({
  selector: 'learner-assignment-detail',
  templateUrl: './learner-assignment-detail.component.html'
})
export class LearnerAssignmentDetailComponent extends BaseComponent {
  @Input()
  public itemDetail: MyAssignmentDetail;
  @Input()
  public canContinueTask: boolean = false;
  @Input()
  public canStartTask: boolean = false;
  @Input()
  public assignmentId: string;

  @Input()
  public set enableScrollCommentSection(enable: boolean) {
    if (enable != null && enable !== this._enableScrollCommentSection) {
      this._enableScrollCommentSection = enable;
      this.currentActiveSectionNumber = this._enableScrollCommentSection ? 2 : 1;
    }
  }
  @ViewChild('informationSection', { static: false })
  public set informationSectionElement(v: ElementRef) {
    this._informationSectionElement = v;
    this.initSection();
  }

  public get informationSectionElement(): ElementRef {
    return this._informationSectionElement;
  }

  @ViewChild('commentSection', { static: false })
  public set commentSectionElement(v: ElementRef) {
    this._commentSectionElement = v;
    this.initSection();
  }

  public get commentSectionElement(): ElementRef {
    return this._commentSectionElement;
  }

  @Output()
  public back: EventEmitter<string> = new EventEmitter<string>();

  public scrollableParent: HTMLElement;
  public currentActiveSectionNumber: number = 1;
  public MyAssignmentStatus: typeof MyAssignmentStatus = MyAssignmentStatus;
  public isShowPlayer: boolean;
  public showFinishScreen: boolean = false;
  public selectSection: boolean = false;
  public visibleSections: SubMenuType[];
  public firstSection: string;

  public get statusColorMap(): unknown {
    return MY_ASSIGNMENT_STATUS_COLOR_MAP;
  }

  private _enableScrollCommentSection: boolean = false;
  private sections: SubMenuType[];
  private _informationSectionElement: ElementRef;
  private _commentSectionElement: ElementRef;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private elementRef: ElementRef,
    private assignmentPlayerIntegrationsService: AssignmentPlayerIntegrationsService,
    private myAssignmentApiService: MyAssignmentApiService,
    private opalDialogService: OpalDialogService,
    private trackingSourceSrv: TrackingSourceService,
    private assignmentApiService: AssignmentApiService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    if (this.canStartTask || this.canContinueTask) {
      this.triggerToStartOrContinueAssignment();
    }
  }

  public onAfterViewInit(): void {
    this.scrollableParent = DomUtils.findClosestVerticalScrollableParent(this.elementRef.nativeElement);
    if (this.scrollableParent === undefined) {
      return;
    }
    const parentScrollEvent: EventTarget = this.scrollableParent;

    fromEvent(parentScrollEvent, 'scroll')
      .pipe(
        this.untilDestroy(),
        debounce(() => interval(50))
      )
      .subscribe(() => this.onScroll());
  }

  public onSelectSection(mode: string): void {
    this.visibleSections.forEach((value, index) => {
      if (mode === value.text) {
        this.firstSection = mode;
        this.scrollTo(value.section.nativeElement, index + 1);
        return;
      }
    });
  }

  public initSection(): void {
    const sections = [
      { text: LearnerDetailMenu.Information, section: this.informationSectionElement },
      { text: LearnerDetailMenu.Comments, section: this.commentSectionElement }
    ];
    if (Utils.isDifferent(sections, this.sections)) {
      this.sections = sections;
      // Recreate sections to fix this issue: https://cxtech.atlassian.net/browse/OPX-3406
      this.visibleSections = this.sections.filter(p => p.section && p.section.nativeElement && !p.section.nativeElement.hidden);
      if (this.visibleSections && this.visibleSections.length > 0) {
        this.firstSection = this.visibleSections[0].text;
      }
    }
  }

  public start(): void {
    if (!this.canStart) {
      this.canNotStartAssignmentWarningMessage();
      return;
    }

    this.showPlayer();
    const request: IChangeMyAssignmentStatus = {
      assignmentId: this.itemDetail.myAssignment.assignmentId,
      registrationId: this.itemDetail.myAssignment.registrationId,
      status: MyAssignmentStatus.InProgress
    };
    this.myAssignmentApiService.changeStatus(request).then(() => {
      this.itemDetail.myAssignment.status = MyAssignmentStatus.InProgress;
    });
  }

  public continue(): void {
    this.showPlayer();
  }

  public learnAgain(): void {
    this.showPlayer();
  }

  public onBackClick(): void {
    this.back.emit(this.itemDetail.myAssignment.participantAssignmentTrackId);
  }

  public onScroll(): void {
    if (this.selectSection) {
      this.selectSection = false;
      return;
    }

    if (this.scrollableParent.scrollTop === 0) {
      this.currentActiveSectionNumber = 1;
      return;
    }

    if (this.scrollableParent.scrollTop + this.scrollableParent.clientHeight > this.scrollableParent.scrollHeight - 50) {
      this.currentActiveSectionNumber = this.visibleSections.length;
      return;
    }

    const currentParentScrollPosition = this.scrollableParent.scrollTop;
    let currentActiveSection: number = 0;
    this.visibleSections.forEach((p, i) => {
      if (p.section !== undefined && p.section.nativeElement.offsetTop - 350 <= currentParentScrollPosition) {
        currentActiveSection = i + 1;
      }
    });
    this.firstSection = this.visibleSections[currentActiveSection - 1].text;
    this.currentActiveSectionNumber = currentActiveSection;
  }

  public scrollTo(el: HTMLElement, sectionNumber: number): void {
    if (el === undefined || this.scrollableParent === undefined) {
      return;
    }
    this.selectSection = true;
    this.scrollableParent.scrollTop = el.offsetTop - 300;
    setTimeout(() => (this.currentActiveSectionNumber = sectionNumber), 55);
  }

  public onBackPlayer(playingSessionId: string): void {
    this.hidePlayerAndSendUserTrackEvent(playingSessionId);
  }

  public onSubmitFeedbacks(): void {
    this.back.emit();
  }

  public get showStart(): boolean {
    return this.itemDetail.myAssignment.status === MyAssignmentStatus.NotStarted;
  }

  public get showContinue(): boolean {
    return this.itemDetail.myAssignment.status === MyAssignmentStatus.InProgress;
  }

  public get showViewAgain(): boolean {
    return (
      this.itemDetail.myAssignment.status === MyAssignmentStatus.Completed ||
      this.itemDetail.myAssignment.status === MyAssignmentStatus.LateSubmission ||
      this.itemDetail.myAssignment.status === MyAssignmentStatus.Incomplete
    );
  }

  public get doAssignmentPermissionKey(): string {
    return LEARNER_PERMISSIONS.Action_Checkin_DoAssignment_DownloadContent_DoPostCourse;
  }

  private onFinished(answer: ParticipantAssignmentTrack, playingSessionId: string): void {
    this.itemDetail.myAssignment.status = MyAssignmentStatus[answer.status];
    this.itemDetail.myAssignment.submittedDate = answer.submittedDate;

    // update assignmnet score
    const { score, totalScore } = MyAssignmentDataService.calculateScore(answer);
    this.itemDetail.score = score;
    this.itemDetail.totalScore = totalScore;

    this.showFinishScreen = true;
    this.hidePlayerAndSendUserTrackEvent(playingSessionId);
  }

  private hidePlayerAndSendUserTrackEvent(playingSessionId: string): void {
    this.canStartTask = false;
    this.canContinueTask = false;
    this.isShowPlayer = false;
    const playingId = uuidv4();

    // send user tracking event right after hide player
    this.trackingSourceSrv.eventTrack.next({
      eventName: 'StopAssignment',
      payload: <AssignmentPlayingEventPayload>{
        playingSessionId: playingId,
        assignmentId: this.itemDetail.myAssignment.assignmentId,
        participantAssignmentTrackId: this.itemDetail.myAssignment.participantAssignmentTrackId
      }
    });
  }

  private showPlayer(): void {
    this.isShowPlayer = true;
    const playingSessionId = uuidv4();

    this.assignmentPlayerIntegrationsService.setup({
      onAssignmentInitiated: () => {
        this.trackingSourceSrv.eventTrack.next({
          eventName: 'PlayAssignment',
          payload: <AssignmentPlayingEventPayload>{
            playingSessionId: playingSessionId,
            assignmentId: this.itemDetail.myAssignment.assignmentId,
            participantAssignmentTrackId: this.itemDetail.myAssignment.participantAssignmentTrackId
          }
        });
      },
      onAssignmentSubmitted: (answer: ParticipantAssignmentTrack) => {
        // using submittedDate to DETECT assignment was submitted and so finish it
        if (answer && answer.submittedDate) {
          this.onFinished(answer, playingSessionId);
        }
      },
      onAssignmentBack: () => {
        this.onBackPlayer(playingSessionId);
      },
      onAssignmentQuestionChanged: (question: QuizAssignmentFormQuestion) => {
        this.trackingSourceSrv.eventTrack.next({
          eventName: 'AnswerAssignment',
          payload: <AssignmentPlayingEventPayload>{
            playingSessionId: playingSessionId,
            assignmentId: this.itemDetail.myAssignment.assignmentId,
            participantAssignmentTrackId: this.itemDetail.myAssignment.participantAssignmentTrackId,
            quizAssignmentFormQuestionId: question.id
          }
        });
      }
    });
    this.assignmentPlayerIntegrationsService.setParticipantAssignmentTrackId(this.itemDetail.myAssignment.participantAssignmentTrackId);
    this.assignmentPlayerIntegrationsService.setAssignmentId(this.itemDetail.myAssignment.assignmentId);
  }

  private get canStart(): boolean {
    const startDate = this.itemDetail.myAssignment.startDate;
    if (!startDate) {
      return false;
    }
    return moment().isSameOrAfter(startDate, 'day');
  }

  public get commentTabInput(): CommentTabInput {
    return {
      originalObjectId: this.itemDetail.myAssignment.participantAssignmentTrackId,
      commentServiceType: CommentServiceType.Course,
      entityCommentType: EntityCommentType.ParticipantAssignmentTrackQuizAnswer,
      commentNotification: CommentNotification.AssignmentFeedbackToCF
    };
  }

  private canNotStartAssignmentWarningMessage(): void {
    this.opalDialogService
      .openConfirmDialog({
        confirmTitle: 'Warning',
        confirmMsg: 'You cannot start the assignment prior to its start date.',
        hideNoBtn: true,
        yesBtnText: 'OK'
      })
      .subscribe();
  }

  private triggerToStartOrContinueAssignment(): void {
    const assignmentTask: Promise<Assignment> = this.assignmentApiService.getAssignmentById(this.assignmentId);
    const myAssignmentTask: Promise<MyAssignment> = this.myAssignmentApiService.getMyAssignmentsByAssignmentId(this.assignmentId);

    Promise.all([assignmentTask, myAssignmentTask]).then(result => {
      this.itemDetail = {
        myAssignment: result[1],
        assignment: result[0],
        participantAssignmentTrack: undefined,
        score: 0,
        totalScore: 0
      };

      if (this.canStartTask) {
        this.start();
      }
      if (this.canContinueTask) {
        this.continue();
      }
    });
  }
}
