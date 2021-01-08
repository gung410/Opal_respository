import * as moment from 'moment';

import {
  AttendanceTracking,
  AttendanceTrackingService,
  CommentApiService,
  CommentServiceType,
  ContentStatus,
  CourseContentItemModel,
  CourseContentItemType,
  EntityCommentType,
  FormAnswerModel,
  IReEnrollCourseRequest,
  IdpRepository,
  IndividualDevelopmentPlanApiService,
  LearningContentApiService,
  LearningCourseType,
  LearningStatus,
  MetadataId,
  MetadataTagModel,
  MyAssignmentStatus,
  MyCourseApiService,
  MyCourseStatus,
  MyLectureStatus,
  MyRegistrationStatus,
  PublicUserInfo,
  REASON_ABSENCE,
  REASON_ABSENCE_MAPPING_TEXT_CONST,
  RegistrationApiService,
  Session,
  UserInfoModel,
  UserReviewItemType
} from '@opal20/domain-api';
import { BaseComponent, ClipboardUtil, DomUtils, FileUploaderSetting, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, Input, OnInit, Output, TemplateRef, ViewChild } from '@angular/core';
import { DialogAction, OpalDialogService, OpalFileUploaderComponent } from '@opal20/common-components';
import {
  EventTrackParam,
  LearningTrackingEventPayload,
  PDODetailEventPayload,
  QuizPlayingEventPlayload,
  RevisitMLUEventPayload
} from '../user-activities-tracking/user-tracking.models';
import {
  LEARNER_PERMISSIONS,
  LearningPathSharingDialogComponent,
  QuizPlayerIntegrationsService,
  ScormPlayerMode
} from '@opal20/domain-components';
import { LearningItemModel, LearningType } from '../models/learning-item.model';
import { Observable, fromEvent, interval } from 'rxjs';
import { debounce, take, tap } from 'rxjs/operators';

import { Align } from '@progress/kendo-angular-popup';
import { AssignmentClickedEvent } from './learning-assignments.component';
import { ClassRunDataService } from '../services/class-run-data.service';
import { CourseClassRun } from '../models/class-run-item.model';
import { CourseDataService } from '../services/course-data.service';
import { CourseModel } from '../models/course.model';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { ILearningFeedbacksConfiguration } from '../models/learning-feedbacks.model';
import { LearnerDetailMenu } from '../constants/learner-detail-menu';
import { LearnerLectureModel } from '../models/lecture.model';
import { LearningActionService } from '../services/learning-action.service';
import { LearningFeedbacksConfig } from '../constants/learning-feedbacks-configs';
import { MaskedTextBoxComponent } from '@progress/kendo-angular-inputs';
import { MyAssignmentDataService } from '../services/my-assignment-data.service';
import { MyAssignmentDetail } from '../models/my-assignment-detail-model';
import { MyLearningPathDataService } from '../services/my-learning-path-data.service';
import { NgForm } from '@angular/forms';
import { TrackingSourceService } from '../user-activities-tracking/tracking-souce.service';
import { UserTrackingService } from '../user-activities-tracking/user-tracking.service';
import { generateMaskedSessionCodeFormat } from '../learner-utils';
import { isEmpty } from 'lodash-es';
import { v4 as uuidv4 } from 'uuid';

export class SubMenuType {
  public section: ElementRef;
  public text: string;
}

@Component({
  selector: 'learner-course-detail',
  templateUrl: './learner-course-detail.component.html',
  providers: [ClassRunDataService]
})
export class LearnerCourseDetailComponent extends BaseComponent implements OnInit {
  @ViewChild('upcomingSessionSection', { static: false })
  public set upcomingSessionSectionElement(v: ElementRef) {
    this._upcomingSessionSectionElement = v;
    this.initSection();
  }

  public get upcomingSessionSectionElement(): ElementRef {
    return this._upcomingSessionSectionElement;
  }

  @ViewChild('aboutSection', { static: false })
  public set aboutSectionElement(v: ElementRef) {
    this._aboutSectionElement = v;
    this.initSection();
  }

  public get aboutSectionElement(): ElementRef {
    return this._aboutSectionElement;
  }

  @ViewChild('contentSection', { static: false })
  public set contentSectionElement(v: ElementRef) {
    this._contentSectionElement = v;
    this.initSection();
  }

  public get contentSectionElement(): ElementRef {
    return this._contentSectionElement;
  }

  @ViewChild('assignmentSection', { static: false })
  public set assignmentSectionElement(v: ElementRef) {
    this._assignmentSectionElement = v;
    this.initSection();
  }

  public get assignmentSectionElement(): ElementRef {
    return this._assignmentSectionElement;
  }

  @ViewChild('classRunSection', { static: false })
  public set classRunSectionElement(v: ElementRef) {
    this._classRunSectionElement = v;
    this.initSection();
  }

  public get classRunSectionElement(): ElementRef {
    return this._classRunSectionElement;
  }

  @ViewChild('reviewSection', { static: false })
  public set reviewSectionElement(v: ElementRef) {
    this._reviewSectionElement = v;
    this.initSection();
  }

  public get reviewSectionElement(): ElementRef {
    return this._reviewSectionElement;
  }

  @ViewChild('checkinTemplate', { static: false })
  public checkinTemplate: TemplateRef<unknown>;

  @ViewChild('absenceTemplate', { static: false })
  public absenceTemplate: TemplateRef<unknown>;

  @ViewChild('checkinForm', { static: false })
  public checkinForm: NgForm;

  @ViewChild('maskedTextbox', { static: false })
  public set maskedTextbox(mask: MaskedTextBoxComponent) {
    if (mask) {
      // To make sure MaskedTextBoxComponent is rendered completely
      setTimeout(() => {
        mask.input.nativeElement.setSelectionRange(0, 0);
        mask.focus();
      });
    }
  }

  @ViewChild('fileUploader', { static: false })
  public fileUploaderComponent: OpalFileUploaderComponent;

  @Input()
  public courseId: string | undefined;
  @Input()
  public canContinueTask: boolean = false;
  @Input()
  public canStartTask: boolean = false;

  @Output()
  public backClick: EventEmitter<void> = new EventEmitter<void>();

  public myCourseStatus: typeof MyCourseStatus = MyCourseStatus;

  public get course(): CourseModel {
    return this._course;
  }
  public set course(v: CourseModel) {
    this._course = v;
    this.learningItem = new LearningItemModel(this.course);
    if (!this.course.isMicrolearning) {
      this.classRunDataService.updateCourseModel(this.course);
    }
  }
  public learningItem: LearningItemModel;
  public tableOfContents: CourseContentItemModel[];
  public lecturesMap: Map<string, LearnerLectureModel> | undefined;
  public completedLectureIds: string[] = [];
  public courseMetadata: Dictionary<MetadataTagModel> = {};

  public showMoreInfo: boolean = false;
  public showMoreContents: boolean = false;
  public showFullContents: boolean = false;

  public nextLectureId: string | undefined;
  public selectedLectureId: string | undefined;

  public scrollableParent: HTMLElement;
  public currentActiveSectionNumber: number = 1;
  public selectSection: boolean = false;

  public messageConfirmText: string = 'Accept';
  public messageCancelText: string = 'Decline';

  public myAssignments: MyAssignmentDetail[];
  public currentAssignment: MyAssignmentDetail;
  public enableScrollCommentSection: boolean = false;

  public areAllAssignmentsCompleted: boolean = false;
  public completedAttendanceTracking: boolean;
  public areAllLecturesCompleted: boolean = false;
  public isCompletedEvaluation: boolean;
  public hasPassedClassRunCourseCriteria: boolean = false;

  public checkinCode: string = '';
  public checkinCodeFormat: string = '';

  public absenceReason: string;
  public upcomingSession: Session;
  public isCheckInCompleted: boolean = false;
  public attachmentUrl: string;
  public hasContentChanged: boolean = false;
  public reviewOnly: boolean = false;
  public fileUploaderSetting: FileUploaderSetting;

  public visibleSections: SubMenuType[];
  public displayMode: ScormPlayerMode = 'learn';
  public firstSection: string;
  public isShowExplanationNote: boolean = false;
  public sharedUsers: PublicUserInfo[] = [];

  // Download / Like / Share / View
  public totalDownload: number = 0;
  public totalLike: number = 0;
  public totalShare: number = 0;
  public totalView: number = 0;
  public isLike: boolean = false;
  public popupAlign: Align = { horizontal: 'right', vertical: 'top' };
  public anchorAlign: Align = { horizontal: 'right', vertical: 'bottom' };

  public set showLecturePlayer(v: boolean) {
    this._showLecturePlayer = v;
    this.updateDetailPageVisibility();
  }
  public get showLecturePlayer(): boolean {
    return this._showLecturePlayer;
  }

  public set showPostCourse(v: boolean) {
    this._showPostCourse = v;
    this.updateDetailPageVisibility();
  }
  public get showPostCourse(): boolean {
    return this._showPostCourse;
  }

  public set showAssignmentDetail(v: boolean) {
    this._showAssignmentDetail = v;
    this.updateDetailPageVisibility();
  }
  public get showAssignmentDetail(): boolean {
    return this._showAssignmentDetail;
  }

  public get showCourseDetail(): boolean {
    return this._showCourseDetail;
  }

  public reasonAbsenceFilterStatus: REASON_ABSENCE = REASON_ABSENCE.Adoption;
  public showOtherReason: boolean = false;
  public reasonAbsenceFilter: IDataItem[] = [
    { text: this.translateCommon(REASON_ABSENCE_MAPPING_TEXT_CONST.get(REASON_ABSENCE.Adoption)), value: REASON_ABSENCE.Adoption },
    { text: this.translateCommon(REASON_ABSENCE_MAPPING_TEXT_CONST.get(REASON_ABSENCE.Sick)), value: REASON_ABSENCE.Sick },
    {
      text: this.translateCommon(REASON_ABSENCE_MAPPING_TEXT_CONST.get(REASON_ABSENCE.Compassionate)),
      value: REASON_ABSENCE.Compassionate
    },
    { text: this.translateCommon(REASON_ABSENCE_MAPPING_TEXT_CONST.get(REASON_ABSENCE.Marriage)), value: REASON_ABSENCE.Marriage },
    {
      text: this.translateCommon(REASON_ABSENCE_MAPPING_TEXT_CONST.get(REASON_ABSENCE.Operationally)),
      value: REASON_ABSENCE.Operationally
    },
    { text: this.translateCommon(REASON_ABSENCE_MAPPING_TEXT_CONST.get(REASON_ABSENCE.ParentCare)), value: REASON_ABSENCE.ParentCare },
    { text: this.translateCommon(REASON_ABSENCE_MAPPING_TEXT_CONST.get(REASON_ABSENCE.Preparation)), value: REASON_ABSENCE.Preparation },
    { text: this.translateCommon(REASON_ABSENCE_MAPPING_TEXT_CONST.get(REASON_ABSENCE.Official)), value: REASON_ABSENCE.Official },
    { text: this.translateCommon(REASON_ABSENCE_MAPPING_TEXT_CONST.get(REASON_ABSENCE.Other)), value: REASON_ABSENCE.Other }
  ];

  public reviewType: UserReviewItemType = UserReviewItemType.Course;
  public get reviewConfig(): ILearningFeedbacksConfiguration {
    return this.course.isMicrolearning ? LearningFeedbacksConfig.microlearning : LearningFeedbacksConfig.faceToFace;
  }

  public get courseCost(): IDataItem {
    if (this.course.courseDetail.courseFee === 0 && this.course.courseDetail.notionalCost === 0) {
      return { text: '', value: 0 };
    } else if (this.course.courseDetail.courseFee === 0 && this.course.courseDetail.notionalCost > 0) {
      return { text: 'Notional', value: this.course.courseDetail.notionalCost };
    } else if (this.course.courseDetail.notionalCost === 0 && this.course.courseDetail.courseFee > 0) {
      return { text: '', value: this.course.courseDetail.courseFee };
    } else {
      return { text: 'Notional', value: this.course.courseDetail.notionalCost };
    }
  }

  private enableUserTracking: boolean = true;
  private sections: SubMenuType[];
  private isAddedToPlan: boolean;

  private _showCourseDetail: boolean = false;
  private _showAssignmentDetail: boolean = false;
  private _showLecturePlayer: boolean = false;
  private _showPostCourse: boolean = false;

  private _upcomingSessionSectionElement: ElementRef;
  private _aboutSectionElement: ElementRef;
  private _contentSectionElement: ElementRef;
  private _assignmentSectionElement: ElementRef;
  private _classRunSectionElement: ElementRef;
  private _reviewSectionElement: ElementRef;
  /**
   * Result extId of action item after adding the course to the plan.
   */
  private actionItemResultExtId: string;
  private _course: CourseModel;
  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private idpCompetenceApiService: IndividualDevelopmentPlanApiService,
    private idpRepository: IdpRepository,
    private learningContentApiService: LearningContentApiService,
    private myCourseApiService: MyCourseApiService,
    private courseDataService: CourseDataService,
    private classRunDataService: ClassRunDataService,
    private registrationApiService: RegistrationApiService,
    private learningActionService: LearningActionService,
    private myAssignmentDataService: MyAssignmentDataService,
    private quizPlayerService: QuizPlayerIntegrationsService,
    private elementRef: ElementRef,
    private opalDialogService: OpalDialogService,
    private attendanceTrackingService: AttendanceTrackingService,
    private trackingSourceSrv: TrackingSourceService,
    private commentApiService: CommentApiService,
    private myLearningPathDataService: MyLearningPathDataService,
    private userTrackingService: UserTrackingService
  ) {
    super(moduleFacadeService);
    this.commentApiService.initApiService(CommentServiceType.Course);
    this.fileUploaderSetting = new FileUploaderSetting({
      extensions: ['docx', 'pdf', 'jpg', 'png'],
      maxFileSize: 1
    });
  }

  public onInit(): void {
    if (this.courseId !== undefined) {
      this.loadCourseDetails();
      this.loadCourseMetadata();
      this.listenBookmarkChanged();
      this.listenSessionActions();
    }
  }

  public get showLinkCommunity(): boolean {
    if (this.course.isMicrolearning) {
      return false;
    }

    const myRegistrationStatus = this.currentClassRun && this.currentClassRun.myRegistrationStatus;
    const myCourseInfoStatus = this.course && this.course.myCourseInfo && this.course.myCourseInfo.status;

    return (
      myRegistrationStatus === MyRegistrationStatus.OfferConfirmed ||
      myRegistrationStatus === MyRegistrationStatus.ConfirmedByCA ||
      myCourseInfoStatus === MyCourseStatus.Completed ||
      myCourseInfoStatus === MyCourseStatus.Failed
    );
  }

  public get communityUrl(): string {
    let url = '';
    if (this.course.isUnPublished) {
      url = '/csl/s/course-' + this.course.courseId;
    } else {
      if (this.course.courseDetail) {
        url = '/csl/s/' + this.course.courseDetail.courseCode;
      }
    }
    return url;
  }

  public onDestroy(): void {
    // If the course detail is open, track out pdo activity
    if (this.showCourseDetail) {
      this.trackReachOutPDOPage(false);
    }
  }

  public onAfterViewInit(): void {
    this.scrollableParent = DomUtils.findClosestVerticalScrollableParent(this.elementRef.nativeElement);
    if (this.scrollableParent == null) {
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
      { section: this.upcomingSessionSectionElement, text: LearnerDetailMenu.UpcomingSession },
      { section: this.aboutSectionElement, text: LearnerDetailMenu.About },
      { section: this.contentSectionElement, text: LearnerDetailMenu.Content },
      { section: this.assignmentSectionElement, text: LearnerDetailMenu.Assignments },
      { section: this.classRunSectionElement, text: LearnerDetailMenu.ClassRun },
      { section: this.reviewSectionElement, text: LearnerDetailMenu.Review }
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

  public onScroll(): void {
    if (this.selectSection) {
      this.selectSection = false;
      return;
    }
    const currentParentScrollPosition = this.scrollableParent.scrollTop;

    if (this.scrollableParent.scrollTop === 0) {
      this.currentActiveSectionNumber = 1;
      return;
    }

    if (this.scrollableParent.scrollTop + this.scrollableParent.clientHeight > this.scrollableParent.scrollHeight - 50) {
      this.currentActiveSectionNumber = this.visibleSections.length;
      return;
    }
    let currentActiveSection: number = 0;
    this.visibleSections.forEach((p, i) => {
      if (p.section !== undefined && p.section.nativeElement.offsetTop - 350 <= currentParentScrollPosition) {
        currentActiveSection = i + 1;
      }
    });
    this.currentActiveSectionNumber = currentActiveSection;
    this.firstSection = this.visibleSections[currentActiveSection - 1].text;
  }

  public scrollTo(el: HTMLElement, sectionNumber: number): void {
    if (el == null || this.scrollableParent == null) {
      return;
    }
    this.selectSection = true;
    this.scrollableParent.scrollTop = el.offsetTop - 300;
    setTimeout(() => (this.currentActiveSectionNumber = sectionNumber), 55);
  }

  public changeBookmark(): void {
    if (this.course.bookmarkInfo == null) {
      this.learningActionService.bookmark(this.courseId, this.learningItem.getCourseBookmarkType).then(bookmarkInfo => {
        this.course.bookmarkInfo = bookmarkInfo;
      });
    } else {
      this.learningActionService.unBookmark(this.courseId, this.learningItem.getCourseBookmarkType).then(() => {
        this.course.bookmarkInfo = undefined;
      });
    }
  }

  public changeCopyURL(): void {
    ClipboardUtil.copyTextToClipboard(this.detailUrl);
    const bookmarkTypeMessage = this.moduleFacadeService.translator.translate('Copied successfully');
    this.showNotification(bookmarkTypeMessage);
  }

  public get lectures(): LearnerLectureModel[] | undefined {
    if (this.lecturesMap == null) {
      return undefined;
    }
    return Utils.mapToArray(this.lecturesMap);
  }

  public setCompletedLectureIds(): void {
    if (this.course.myLecturesInfo == null || this.lecturesMap == null) {
      this.completedLectureIds = [];
      this.areAllLecturesCompleted = true; // Set it = true if no content (Face To Face)
      return;
    }

    const lectures = Utils.mapToArray(this.lecturesMap);
    if (lectures.length === 0 || lectures.some(p => p.myLectureInfo == null)) {
      this.completedLectureIds = [];
      return;
    }

    this.completedLectureIds = lectures.filter(p => p.myLectureInfo.status === MyLectureStatus.Completed).map(p => p.lectureId);
  }

  public updateShowFullContent(event: boolean): void {
    this.showFullContents = event;
  }

  public toggleShowMoreContents(): void {
    this.showMoreContents = !this.showMoreContents;
    this.onScroll();
  }

  public onLectureClick(event: string): void {
    this.selectedLectureId = event;
    if (this.course.myCourseInfo == null) {
      return;
    }

    const lecture = this.lecturesMap.get(event);
    if (lecture == null || (lecture.lectureId !== this.nextLectureId && lecture.myLectureInfo.status !== MyLectureStatus.Completed)) {
      return;
    }

    this.reviewOnly = this.showLearnAgain;

    this.openLecturePlayer();
  }

  public onBackClick(): void {
    this.backClick.emit();
  }

  public onLecturePlayerBackClick(): void {
    this.setTrackingActivity(false);
    this.currentActiveSectionNumber = 1;
    this.selectSection = false;
    if (this.scrollableParent !== undefined) {
      this.scrollableParent.scrollTop = 0;
    }
    this.hideLecturePlayer();
    this.selectedLectureId = undefined;
    this.hasContentChanged = false;
    this.loadMyCourseDetails()
      .pipe(this.untilDestroy())
      .subscribe();
    this.loadTableOfContents();
    this.reviewOnly = false;
  }

  public onLectureCompleted(lecture: LearnerLectureModel): void {
    this.lecturesMap.set(lecture.lectureId, lecture);
  }

  public toggleShowMoreInfo(): void {
    this.showMoreInfo = !this.showMoreInfo;
    this.onScroll();
  }

  public learnAgainCourse(): void {
    if (!this.hasLecture) {
      this.showNoContentViewDialog();
      return;
    }
    const request: IReEnrollCourseRequest = {
      courseId: this.courseId,
      courseType: this.getLearningCourseType(),
      lectureIds: Array.from(this.lecturesMap.values()).map(p => p.lectureId)
    };

    this.myCourseApiService.reEnrollCourse(request).then(() => {
      this.trackingSourceSrv.eventTrack.next({
        eventName: 'RevisitMLU',
        payload: <RevisitMLUEventPayload>{
          visitMode: 'learnAgain',
          courseId: this.course.courseId
        }
      });

      this.loadMyCourseDetails()
        .pipe(this.untilDestroy())
        .subscribe(() => {
          this.selectedLectureId = undefined;
          this.showLecturePlayer = true;
        });
    });
  }

  public get disableLearning(): boolean {
    return (
      !this.hasPermission(this.startLearningPermissionKey) ||
      this.course.isCourseUnavailable ||
      (this.currentClassRun && this.currentClassRun.isClassRunUnavailable) ||
      (this.course.isMicrolearning && !this.hasLecture)
    );
  }

  public onReviewShowMore(): void {
    this.onScroll();
  }

  public onCardMessageConfirm(): void {
    this.classRunDataService.acceptOffer(this.currentClassRun);
  }

  public onCardMessageCancel(): void {
    this.classRunDataService.rejectOffer(this.currentClassRun);
  }

  /**
   * only have with class run course
   */
  public get showAssignments(): boolean {
    return (
      this.currentClassRun &&
      (this.currentClassRun.learningStatus === LearningStatus.InProgress ||
        this.currentClassRun.learningStatus === LearningStatus.Passed ||
        this.currentClassRun.learningStatus === LearningStatus.Failed ||
        this.currentClassRun.learningStatus === LearningStatus.Completed)
    );
  }

  public onAssignmentClicked(event: AssignmentClickedEvent): void {
    this.currentAssignment = event.assignment;
    this.enableScrollCommentSection = event.scrollToComment;
    this.showAssignmentDetail = true;
  }

  public onAssignmentBack(participantAssignmentTrackId: string): void {
    // After coming back from assignment detail, hidden notification dot in comment button
    this.resetAssignmentCommentNotSeen(participantAssignmentTrackId);
    this.currentAssignment = undefined;
    this.enableScrollCommentSection = false;
    this.showAssignmentDetail = false;
  }

  public onPostCourseClicked(): void {
    this.showPostCourse = true;
    const playingSessionId = uuidv4();

    this.quizPlayerService.setup({
      onQuizFinished: (formAnswerModel: FormAnswerModel) => {
        this.onFinishedPostCourse();
        this.trackingSourceSrv.eventTrack.next(<EventTrackParam>{
          eventName: 'StopQuiz',
          payload: <QuizPlayingEventPlayload>{
            playingSessionId: playingSessionId,
            formId: formAnswerModel.formId,
            formAnswerId: formAnswerModel.id
          }
        });
      },
      onQuizInitiated: () => {
        this.trackingSourceSrv.eventTrack.next(<EventTrackParam>{
          eventName: 'PlayQuiz',
          payload: <QuizPlayingEventPlayload>{
            playingSessionId: playingSessionId,
            formId: this.course.courseDetail.postCourseEvaluationFormId
          }
        });
      },
      onQuizExited: (formAnswerModel: FormAnswerModel) => {
        this.trackingSourceSrv.eventTrack.next(<EventTrackParam>{
          eventName: 'StopQuiz',
          payload: <QuizPlayingEventPlayload>{
            playingSessionId: playingSessionId,
            formId: formAnswerModel.formId,
            formAnswerId: formAnswerModel.id
          }
        });
      },
      onQuizSubmitted: (formAnswerModel: FormAnswerModel) => {
        this.trackingSourceSrv.eventTrack.next(<EventTrackParam>{
          eventName: 'SubmittedQuizAnswer',
          payload: <QuizPlayingEventPlayload>{
            playingSessionId: playingSessionId,
            formId: formAnswerModel.formId,
            formAnswerId: formAnswerModel.id
          }
        });
      }
    });
    this.quizPlayerService.setFormId(this.course.courseDetail.postCourseEvaluationFormId);
    this.quizPlayerService.setResourceId(this.courseId);
    this.quizPlayerService.setClassRunId(this.course.myClassRun.classRunId);
    this.quizPlayerService.setMyCourseId(this.course.myCourseInfo.id);
    // TODO: set this to null because there is a problem with BehaviorSubject, don't have time to invest why they used BehaviorSubject
    this.quizPlayerService.setAssignmentId(null);
    this.quizPlayerService.setReviewOnly(false);
  }

  public onCheckInClicked(session: Session): void {
    this.classRunDataService.openCheckInDialog.next(session);
  }

  public showCheckIn(session: Session): boolean {
    return this.upcomingSession && moment().isSame(session.startDateTime, 'day');
  }

  public get disableCourseContent(): boolean {
    return this.disableLearning || this.hasContentChanged || !this.isClassRunStarted;
  }

  public get canShowPostCourseLink(): boolean {
    if (this.currentClassRun == null) {
      return false;
    }

    const learningStatus = this.currentClassRun.learningStatus;
    return (
      learningStatus === LearningStatus.Failed || learningStatus === LearningStatus.Passed || learningStatus === LearningStatus.Completed
    );
  }

  public get isDisablePostCourse(): boolean {
    if (this.currentClassRun.postCourseEvaluationFormCompleted) {
      return true;
    }

    return false;
  }

  //#region microlearning actions
  public startMicrolearning(): void {
    if (!this.hasLecture) {
      this.showNoContentViewDialog();
      return;
    }
    const lectureIds = this.lecturesMap ? Array.from(this.lecturesMap.values()).map(p => p.lectureId) : undefined;
    this.myCourseApiService.enrollCourse(this.courseId, lectureIds).then(() => {
      this.loadMyCourseDetails()
        .pipe(this.untilDestroy())
        .subscribe(() => {
          this.selectedLectureId = undefined;
          this.showLecturePlayer = this.hasLecture;
        });
    });
  }

  public learnAgain(): void {
    if (this.course.myCourseInfo.hasContentChanged) {
      this.markContentChanged();
      this.learnAgainCourse();
    } else {
      this.viewAgainMicrolearning();
    }
  }

  public viewAgainMicrolearning(): void {
    if (!this.hasLecture) {
      this.showNoContentViewDialog();
      return;
    }
    this.selectedLectureId = this.lectures[0].lectureId;
    this.showLecturePlayer = true;
    this.reviewOnly = true;
    this.displayMode = 'view';

    this.trackingSourceSrv.eventTrack.next({
      eventName: 'RevisitMLU',
      payload: <RevisitMLUEventPayload>{
        visitMode: 'viewOnly',
        courseId: this.course.courseId
      }
    });
  }
  public continueMicrolearning(): void {
    if (this.course.myCourseInfo.hasContentChanged) {
      this.opalDialogService
        .openConfirmDialog({
          confirmTitle: 'Information',
          confirmMsg:
            'There is a new update of the content for this course. Your learning progress will be reset. Sorry for this inconvenience',
          noBtnText: 'Close',
          yesBtnText: 'Continue'
        })
        .subscribe(confirm => {
          if (confirm === DialogAction.OK) {
            this.markContentChanged(false);
            this.learnAgainCourse();
          }
        });
      return;
    } else {
      this.showLecturePlayer = this.hasLecture;
    }
  }
  //#endregion

  //#region not microlearning actions
  public onAddCourseToPlan(): Promise<void> {
    if (this.isAddedToPlan) {
      return Promise.resolve();
    }
    return this.addCourseToPlan();
  }

  public removeFromPlan(): void {
    this.idpCompetenceApiService.deactivateActionItem(this.actionItemResultExtId).then(() => {
      this.isAddedToPlan = false;
      this.actionItemResultExtId = undefined;
      this.onShowMessageRemoveSuccessfully();
    });
  }

  public startClassRun(): void {
    const lectureIds = this.lecturesMap ? Array.from(this.lecturesMap.values()).map(p => p.lectureId) : undefined;
    this.myCourseApiService.enrollCourse(this.courseId, lectureIds).then(() => {
      this.loadMyCourseDetails()
        .pipe(this.untilDestroy())
        .subscribe(() => {
          this.currentClassRun.learningStatus = LearningStatus.InProgress;
          this.handleShowMessageNoSessionOrContent();
          this.checkPassingClassRunCourseCriteria();
          this.selectedLectureId = undefined;
          this.showLecturePlayer = this.hasLecture;
          this.initAssignments();
        });
    });
  }

  public continueClassRun(): void {
    this.showLecturePlayer = this.hasLecture;
  }

  public viewAgainClassRun(): void {
    this.selectedLectureId = this.lectures[0].lectureId;
    this.showLecturePlayer = true;
    this.reviewOnly = true;
    this.displayMode = 'view';
  }
  //#endregion

  //#region microlearning buttons
  public get showStartMicrolearning(): boolean {
    return this.course.myCourseInfo == null;
  }

  public get showContinueMicrolearning(): boolean {
    if (this.course.myCourseInfo == null || !this.hasLecture) {
      return false;
    }
    return this.course.myCourseInfo.status === MyCourseStatus.InProgress;
  }

  public get showViewAgainMicrolearning(): boolean {
    return (
      this.course.myCourseInfo &&
      !this.course.myCourseInfo.hasContentChanged &&
      this.course.myCourseInfo.status === MyCourseStatus.Completed
    );
  }

  public get showLearnAgain(): boolean {
    return (
      this.course.isMicrolearning &&
      this.course.myCourseInfo &&
      this.course.myCourseInfo.hasContentChanged &&
      this.course.myCourseInfo.status === MyCourseStatus.Completed
    );
  }
  //#endregion

  //#region not microlearning buttons
  public get showPlanButtons(): boolean {
    return (
      this.hasPermission(LEARNER_PERMISSIONS.Action_Plan) &&
      this.isAddedToPlan != null &&
      !this.showStartClassRun &&
      !this.showContinueClassRun &&
      !this.course.isPrivateCourse
    );
  }

  public get showAddToPlan(): boolean {
    return this.showPlanButtons && this.isAddedToPlan === false && !this.course.hasReachedMaxRelearningTimes();
  }

  public get showRemoveFromPlan(): boolean {
    return (
      this.showPlanButtons &&
      this.isAddedToPlan === true &&
      !this.course.hasReachedMaxRelearningTimes() &&
      (this.course.myClassRun == null || this.course.myClassRun.isClassRunFinished) &&
      this.hasNoAppliedClassRunInCurrentYear()
    );
  }

  public get showContinueClassRun(): boolean {
    if (this.currentClassRun == null || !this.hasLecture) {
      return false;
    }
    const learningStatus = this.currentClassRun.learningStatus;
    return learningStatus === LearningStatus.InProgress || learningStatus === LearningStatus.Passed;
  }

  public get showStartClassRun(): boolean {
    return (
      this.currentClassRun != null &&
      this.currentClassRun.isStarted &&
      this.currentClassRun.isParticipant &&
      this.currentClassRun.learningStatus === LearningStatus.NotStarted
    );
  }

  public get showViewAgainClassRun(): boolean {
    return (
      // need to check myCourseInfo Completed
      // because there is a case that the learner continously apply another class run when the current class is not ended
      // so in that case we don't show view again.
      this.hasLecture &&
      this.course.myCourseInfo &&
      this.course.myCourseInfo.status === MyCourseStatus.Completed &&
      this.currentClassRun &&
      this.currentClassRun.learningStatus === LearningStatus.Completed &&
      !this.currentClassRun.isEnded
    );
  }
  //#endregion

  public get isClassRunStarted(): boolean {
    if (this.course.isMicrolearning) {
      return true;
    }

    return this.currentClassRun && this.currentClassRun.isStarted;
  }

  public get startLearningPermissionKey(): string {
    return LEARNER_PERMISSIONS.Action_StartLearning;
  }

  public onSelectReason(reason: REASON_ABSENCE): void {
    if (reason === REASON_ABSENCE.Other) {
      this.absenceReason = '';
      this.showOtherReason = true; // Need to type other reason
    } else {
      this.absenceReason = this.translateCommon(REASON_ABSENCE_MAPPING_TEXT_CONST.get(reason));
      this.showOtherReason = false;
    }
  }

  public toggleShowExplaination(): void {
    this.isShowExplanationNote = !this.isShowExplanationNote;
  }

  public getVenue(session: Session): string {
    return session.isLearningOnline() ? 'Online Session' : session.venue;
  }
  // End Action Button Section

  public onContextMenuItemSelect(eventData: { id: string }): void {
    switch (eventData.id) {
      case 'share':
        this.openSharedDialog();
        break;
      case 'copy':
        this.changeCopyURL();
        break;
      case 'bookmarks':
        this.changeBookmark();
        break;
    }
  }

  public openSharedDialog(): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(LearningPathSharingDialogComponent, {
      fetchUsersFn: this.fetchUsersFn,
      selectedUsers: this.sharedUsers,
      onShareFn: () => {
        const learningType = !this.course.isMicrolearning ? LearningType.Course : LearningType.Microlearning;
        this.userTrackingService.share(this.courseId, learningType, this.sharedUsers.map(user => user.id)).then(result => {
          this.totalShare = result.totalShare;

          const msg = this.moduleFacadeService.translator.translateCommon('Shared successfully');
          this.showNotification(msg);
        });
      }
    });

    dialogRef.result.subscribe(() => {
      this.sharedUsers = [];
      dialogRef.close();
    });
  }

  public fetchUsersFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<PublicUserInfo[]> = (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => this.myLearningPathDataService.searchUsers({ searchText, maxResultCount, skipCount });

  public toggleLike(): void {
    const learningType = !this.course.isMicrolearning ? LearningType.Course : LearningType.Microlearning;
    this.userTrackingService.like(this.courseId, learningType, !this.isLike).then(result => {
      this.totalLike = result.totalLike;
      this.isLike = result.isLike;

      const msgCommon = (this.isLike ? 'Liked' : 'Like undone') + ' successfully';
      const msg = this.moduleFacadeService.translator.translateCommon(msgCommon);
      this.showNotification(msg);
    });
  }

  public get showCardConfirmation(): boolean {
    return this.currentClassRun != null && this.currentClassRun.hasOffer;
  }

  public get cardMessageTitle(): string {
    return this.currentClassRun.myRegistrationStatus === MyRegistrationStatus.WaitlistPendingApprovalByLearner
      ? 'You have been placed on the waitlist for'
      : 'You are now off the waitlist and have been offered to accept your registration for the class run shown below:';
  }

  public get currentClassRun(): CourseClassRun {
    return this.classRunDataService.currentClassRun;
  }

  public get courseClassRunItems(): CourseClassRun[] {
    return this.classRunDataService.courseClassRunItems;
  }

  public get checkinPermissionKey(): string {
    return LEARNER_PERMISSIONS.Action_Checkin_DoAssignment_DownloadContent_DoPostCourse;
  }

  public get hasBookmarkPermission(): boolean {
    return this.hasPermission(LEARNER_PERMISSIONS.Action_Bookmark);
  }

  public get hasLikeShareCopyPermission(): boolean {
    return this.hasPermission(
      this.course.isMicrolearning
        ? LEARNER_PERMISSIONS.Action_Microlearning_Like_Share_Copy
        : LEARNER_PERMISSIONS.Action_Course_Like_Share_Copy
    );
  }

  public get hasCheckinPermission(): boolean {
    return this.hasPermission(LEARNER_PERMISSIONS.Action_Checkin_DoAssignment_DownloadContent_DoPostCourse);
  }

  protected currentUserPermissionDic(): IPermissionDictionary {
    return UserInfoModel.getMyUserInfo().permissionDic;
  }

  private checkPassingClassRunCourseCriteria(): void {
    switch (this.course.courseDetail.learningMode) {
      case MetadataId.FaceToFace:
        this.hasPassedClassRunCourseCriteria = this.completedAttendanceTracking != null;
        break;
      case MetadataId.ELearning:
        this.hasPassedClassRunCourseCriteria = this.areAllLecturesCompleted;
        break;
      case MetadataId.BlendedLearning:
        this.hasPassedClassRunCourseCriteria = this.areAllLecturesCompleted && this.completedAttendanceTracking;
        break;
      default:
        break;
    }

    // Description for the code below: When class run in cases (auto set passed):
    // 1: E-Learning: No content
    // 2: FTF: No session
    // 3: Blended: No session & no content
    if (this.hasPassedClassRunCourseCriteria && this.currentClassRun && this.currentClassRun.learningStatus === LearningStatus.InProgress) {
      this.course.myCourseInfo.status = MyCourseStatus.Passed;
      this.currentClassRun.learningStatus = LearningStatus.Passed;
    }
  }

  private addCourseToPlan(): Promise<void> {
    return this.idpCompetenceApiService.addActionItemResult(this.courseId).then(actionItem => {
      if (!actionItem) {
        return;
      }
      this.actionItemResultExtId = actionItem.resultIdentity.extId;
      this.isAddedToPlan = true;
      this.onShowMessageAddSuccessfully();
    });
  }

  private hasNoAppliedClassRunInCurrentYear(): boolean {
    return (
      this.courseClassRunItems == null ||
      this.courseClassRunItems.length === 0 ||
      !this.courseClassRunItems.some(p => p.startDateTime.getFullYear() === new Date().getFullYear() && p.isApplied)
    );
  }

  private resetAssignmentCommentNotSeen(participantAssignmentTrackId: string): void {
    this.myAssignments.find(item => {
      if (item.myAssignment.participantAssignmentTrackId === participantAssignmentTrackId) {
        item.myAssignment.commentNotSeenIds = [];
      }
    });
  }

  private updateDetailPageVisibility(): void {
    const previousValue = this._showCourseDetail;
    const newValue = !this.showPostCourse && !this.showLecturePlayer && !this.showAssignmentDetail;
    if (previousValue === newValue) {
      return;
    }
    this._showCourseDetail = newValue;

    // newValue = true means reaching pdo
    const isReach = newValue;
    this.trackReachOutPDOPage(isReach);
  }

  /**
   * To track reach or out PDO activity
   * @param reach set true when reaching, false when leaving
   */
  private trackReachOutPDOPage(reach: boolean): void {
    this.trackingSourceSrv.eventTrack.next({
      eventName: reach ? 'ReachPDOPage' : 'OutPDOPage',
      payload: <PDODetailEventPayload>{
        itemId: this.course.courseId,
        trackingType: this.course.isMicrolearning ? 'microlearning' : 'course'
      }
    });
  }

  private listenSessionActions(): void {
    this.classRunDataService.openCheckInDialog.pipe(this.untilDestroy()).subscribe(session => this.openCheckInDialog(session));
    this.classRunDataService.openCantParticipateDialog
      .pipe(this.untilDestroy())
      .subscribe(session => this.openCantParticipateDialog(session));
  }

  private openCantParticipateDialog(attendanceTracking: AttendanceTracking): void {
    this.reasonAbsenceFilterStatus = REASON_ABSENCE.Adoption;
    this.showOtherReason = false;
    this.absenceReason = this.translateCommon(REASON_ABSENCE_MAPPING_TEXT_CONST.get(REASON_ABSENCE.Adoption));
    this.attachmentUrl = '';
    this.opalDialogService
      .openConfirmDialog({
        bodyTemplate: this.absenceTemplate,
        confirmTitle: 'Unable to participate?',
        yesBtnText: 'Submit',
        noBtnText: 'Close',
        confirmMsg: 'Please indicate your reason for absence here.',
        disableYesBtnFn: () => isEmpty(this.absenceReason.trim()),
        confirmRequest: () => this.confirmRequestUploadFile(),
        confirmRequestNotShowFailedMsg: true,
        confirmRequestNotShowSuccessMsg: true
      })
      .subscribe(confirm => {
        if (confirm === DialogAction.OK) {
          this.submitReasonForAbsence(attendanceTracking, this.attachmentUrl);
          return;
        }
        if (confirm === DialogAction.Cancel) {
          if (this.fileUploaderComponent.uploadingFile === true) {
            this.fileUploaderComponent.abortUpload();
          }
        }
      });
  }

  private confirmRequestUploadFile(): Promise<[string[], undefined] | [undefined, unknown]> {
    const confirmRequestFail: [string[], undefined] = [['error'], undefined];
    const confirmRequestSuccess: [undefined, unknown] = [undefined, undefined];
    if (this.fileUploaderComponent.file == null) {
      return Promise.resolve(confirmRequestSuccess);
    }
    return this.fileUploaderComponent.triggerUploadingFile().then(uploadedUrl => {
      if (uploadedUrl) {
        this.attachmentUrl = uploadedUrl;
        return confirmRequestSuccess;
      }
      return confirmRequestFail;
    });
  }

  private openCheckInDialog(session: Session): void {
    if (this.isCheckInCompleted) {
      return;
    }
    this.checkinCode = '';
    this.checkinCodeFormat = generateMaskedSessionCodeFormat(this.course.courseDetail.courseCode);
    this.opalDialogService
      .openConfirmDialog({
        bodyTemplate: this.checkinTemplate,
        confirmTitle: 'Check-in',
        yesBtnText: 'Confirm',
        noBtnText: 'Close',
        confirmMsg: 'Please type Class Identification code',
        disableYesBtnFn: () => isEmpty(this.checkinCode) || !this.checkinForm || !this.checkinForm.valid
      })
      .subscribe(confirm => {
        if (confirm === DialogAction.OK) {
          if (!session.isStarted()) {
            this.showMessageForNotStartedSession();
            return;
          }
          this.checkinCode = this.checkinCode.toUpperCase();
          this.submitCheckinCode(session).then(response => {
            this.showNotification('You have successfully tracked attendance');
            this.classRunDataService.updateAttendanceTracking.next(response);
            if (this.upcomingSession.id === response.sessionId) {
              this.isCheckInCompleted = true;
            }
          });
        }
      });
  }

  private submitReasonForAbsence(attendanceTracking: AttendanceTracking, attachmentUrl?: string): void {
    const attachments = attachmentUrl ? [attachmentUrl] : [];
    this.attendanceTrackingService
      .changeReasonForAbsence(
        {
          sessionId: attendanceTracking.sessionId,
          userId: attendanceTracking.userId,
          reason: this.absenceReason,
          attachment: attachments
        },
        true
      )
      .then(() => {
        const response = new AttendanceTracking({ ...attendanceTracking, reasonForAbsence: this.absenceReason, attachment: attachments });
        this.showNotification('Your reason for absence is successfully submitted');
        this.classRunDataService.updateAttendanceTracking.next(response);
      });
  }

  private submitCheckinCode(session: Session): Promise<AttendanceTracking> {
    return this.attendanceTrackingService.takeAttendanceTracking(session.id, this.checkinCode);
  }

  private onFinishedPostCourse(): void {
    this.showPostCourse = false;
    this.registrationApiService.completePostEvaluation(this.currentClassRun.registrationId).then(() => {
      this.currentClassRun.postCourseEvaluationFormCompleted = true;
      if (this.currentClassRun.learningStatus === LearningStatus.Passed) {
        this.course.myCourseInfo.status = MyCourseStatus.Completed;
        this.currentClassRun.learningStatus = LearningStatus.Completed;
      }
    });
  }

  private showMessageForNotStartedSession(): void {
    // set timeout because have to wait the primary dialog close first.
    setTimeout(() => {
      this.opalDialogService
        .openConfirmDialog({
          confirmTitle: 'Warning',
          confirmMsg: 'Unable to check-in now, the code is only available when the session starts',
          yesBtnText: 'OK',
          hideNoBtn: true
        })
        .subscribe();
    });
  }

  private onShowMessageRemoveSuccessfully(): void {
    this.showNotification('Course has been successfully removed from PD Plan');
  }

  private onShowMessageAddSuccessfully(): void {
    this.showNotification('Course has been successfully added to PD Plan');
  }

  private openLecturePlayer(): void {
    this.showLecturePlayer = true;
    const scrollableParent = DomUtils.findClosestVerticalScrollableParent(this.elementRef.nativeElement);
    if (scrollableParent !== undefined) {
      scrollableParent.scrollTop = 0;
    }
  }

  private loadCourseDetails(): void {
    this.idpRepository
      .loadPlanPdos({ courseId: this.courseId })
      .pipe(this.untilDestroy())
      .subscribe(planPdosResult => {
        if (!planPdosResult || !planPdosResult.items || !planPdosResult.items.length) {
          this.isAddedToPlan = false;
          return;
        }
        this.actionItemResultExtId = planPdosResult.items[0].resultIdentity.extId;
        this.isAddedToPlan = true;
      });
    this.courseDataService
      .getCourseDetails(this.courseId)
      .pipe(this.untilDestroy())
      .subscribe(course => {
        this.course = course;
        this.markContentChanged();
        this.loadTableOfContents();
        if (!this.course.isMicrolearning) {
          this.initClassRun();
          this.initAssignments();
          this.viewTracking(LearningType.Course);
          this.getTrackingInfoByItemId(LearningType.Course);
        } else {
          this.viewTracking(LearningType.Microlearning);
          this.getTrackingInfoByItemId(LearningType.Microlearning);
        }
        this.updateDetailPageVisibility();
      });
  }

  private initClassRun(): void {
    this.classRunDataService
      .getClassRunsWithSessionData()
      .pipe(this.untilDestroy())
      .subscribe(courseClassItems => {
        if (!courseClassItems || !courseClassItems.length) {
          return;
        }
        this.classRunDataService.courseClassRunItems = courseClassItems;
        this.classRunDataService.updateCurrentClassRun();
      });
    this.classRunDataService.currentClassRun$.pipe(this.untilDestroy()).subscribe(classRun => {
      if (!this.currentClassRun) {
        return;
      }
      if (this.currentClassRun.isApplied || this.currentClassRun.learningStatus === LearningStatus.Completed) {
        this.initAssignments();
        this.loadTableOfContents();
        this.checkPassingClassRunCourseCriteria();
      }
    });

    this.classRunDataService.upcomingSession
      .pipe(
        this.untilDestroy(),
        take(1)
      )
      .subscribe(upcomingSession => {
        if (upcomingSession == null) {
          return;
        }
        this.upcomingSession = upcomingSession;
      });

    this.classRunDataService.completedAttendanceTrackingSubject.pipe(this.untilDestroy()).subscribe(isCompleted => {
      if (this.currentClassRun && this.currentClassRun.learningStatus === LearningStatus.InProgress && isCompleted === false) {
        this.course.myCourseInfo.status = MyCourseStatus.Failed;
        this.currentClassRun.learningStatus = LearningStatus.Failed;
      }
      this.completedAttendanceTracking = isCompleted;
      this.checkPassingClassRunCourseCriteria();
    });
  }

  private initAssignments(): void {
    if (this.showAssignments && !this.myAssignments) {
      const registrationId = this.course.myClassRun.registrationId;
      this.myAssignmentDataService.getMyAssignments(registrationId).then(async myAssignments => {
        this.myAssignments = await this.mapAssignmentWithCommentNotSeen(myAssignments);
        this.markCompletedAssignments();
      });
    }
  }

  private mapAssignmentWithCommentNotSeen(myAssignments: MyAssignmentDetail[]): Promise<MyAssignmentDetail[]> {
    let myAssignmentWithComment: MyAssignmentDetail[] = [];
    if (myAssignments) {
      const participantAssignmentTrackIds = [];
      myAssignments.forEach(assignment => {
        participantAssignmentTrackIds.push(assignment.myAssignment.participantAssignmentTrackId);
      });
      // Get comment not seen Ids by participantAssignmentTrackId array
      return this.commentApiService
        .getCommentNotSeen({
          objectIds: participantAssignmentTrackIds,
          entityCommentType: EntityCommentType.ParticipantAssignmentTrackQuizAnswer
        })
        .then(result => {
          if (!result) {
            return;
          }
          // Mapping each assignment with it's comment not seen Ids
          const commentDic = Utils.toDictionarySelect(result, x => x.objectId, x => x.commentNotSeenIds);
          myAssignmentWithComment = myAssignments.map(assignment => {
            assignment.myAssignment.commentNotSeenIds = commentDic[assignment.myAssignment.participantAssignmentTrackId];
            return assignment;
          });

          return myAssignmentWithComment;
        });
    }
  }

  private markCompletedAssignments(): void {
    this.areAllAssignmentsCompleted =
      this.myAssignments.findIndex(
        assignment => !assignment.myAssignment || assignment.myAssignment.status !== MyAssignmentStatus.Completed
      ) === -1;
  }

  private markCompletedLectures(): void {
    this.areAllLecturesCompleted = this.lecturesMap == null || this.completedLectureIds.length === this.lecturesMap.size;
  }

  private loadCourseMetadata(): void {
    this.courseDataService
      .getCourseMetadata()
      .pipe(this.untilDestroy())
      .subscribe(response => {
        this.courseMetadata = Utils.toDictionary(response, p => p.tagId);
      });
  }

  private loadTableOfContents(): void {
    if (!this.tableOfContentReadyForUse) {
      return;
    }
    this.showMoreContents = false;
    const appliedClassRunId = this.currentClassRun ? this.currentClassRun.id : undefined;
    this.learningContentApiService.getTableOfContents(this.courseId, appliedClassRunId).then(toc => {
      if (toc && toc.length > 0) {
        const contents = toc.filter(p => p.type === CourseContentItemType.Section || p.type === CourseContentItemType.Lecture);
        this.tableOfContents = contents;
        this.lecturesMap = this.getLecturesFromTableOfContents(contents);
      }
      this.updateLectures();
      this.onScroll();

      this.triggerToStartOrContinueCourseTask();
    });
  }

  private loadMyCourseDetails(): Observable<CourseModel> {
    return this.courseDataService.getCourseDetails(this.courseId).pipe(
      tap(course => {
        this.course = course;
        this.updateLectures();
        this.onScroll();
      })
    );
  }

  private getLecturesFromTableOfContents(toc: CourseContentItemModel[]): Map<string, LearnerLectureModel> {
    const lecturesMap = new Map<string, LearnerLectureModel>();
    toc.forEach(content => {
      if (content.type === CourseContentItemType.Lecture) {
        lecturesMap.set(content.id, LearnerLectureModel.fromCourseContentItem(content));
      }

      if (content.type === CourseContentItemType.Section && content.items !== undefined && content.items.length > 0) {
        content.items.forEach(p => lecturesMap.set(p.id, LearnerLectureModel.fromCourseContentItem(p)));
      }
    });
    return lecturesMap;
  }

  private updateLectures(): void {
    this.updateMyLectures();
    this.setCompletedLectureIds();
    this.markCompletedLectures();
    this.checkPassingClassRunCourseCriteria();
    this.calculateNextLecture();
  }

  private updateMyLectures(): void {
    if (this.course.myLecturesInfo == null || this.lecturesMap == null) {
      return;
    }
    this.course.myLecturesInfo.forEach(p => {
      const lectureMap = this.lecturesMap.get(p.lectureId);
      if (lectureMap) {
        this.lecturesMap.get(p.lectureId).myLectureInfo = p;
      }
    });
  }

  private calculateNextLecture(): void {
    if (this.lecturesMap == null) {
      this.nextLectureId = undefined;
      return;
    }

    const lectures = this.lectures;
    if (
      lectures.length === 0 ||
      lectures.some(p => p.myLectureInfo == null) ||
      lectures.every(p => p.myLectureInfo.status === MyLectureStatus.Completed)
    ) {
      this.nextLectureId = undefined;
      return;
    }

    const firstNotStartedLecture = this.lectures.find(p => p.myLectureInfo.status === MyLectureStatus.NotStarted);
    this.nextLectureId = firstNotStartedLecture !== undefined ? firstNotStartedLecture.lectureId : undefined;
  }

  private listenBookmarkChanged(): void {
    this.learningActionService.bookmarkChanged.pipe(this.untilDestroy()).subscribe(bookmarkChanged => {
      if (bookmarkChanged.itemId === this.courseId) {
        this.course.bookmarkInfo = bookmarkChanged.isBookmarked ? bookmarkChanged.data : undefined;
      }
    });
  }

  private getLearningCourseType(): LearningCourseType {
    if (this.learningItem.pdActivityType === MetadataId.Microlearning) {
      return LearningCourseType.Microlearning;
    } else {
      return LearningCourseType.FaceToFace;
    }
  }

  private get tableOfContentReadyForUse(): boolean {
    /**
     * With the course is MicroLearning the content will load according to the course.
     * With the course is non-MicroLearning the content will load according to the course
     * when the user doesn't apply any class.
     */
    const courseContentReadyForUse = this.course.courseDetail.contentStatus === ContentStatus.Published;

    /**
     * With the course is non-MicroLearning
     * after applying the content will load according to the class run.
     */
    const classRunContentReadyForUse = this.currentClassRun && this.currentClassRun.contentStatus === ContentStatus.Published;

    return courseContentReadyForUse || classRunContentReadyForUse;
  }

  private showNoContentViewDialog(): void {
    this.moduleFacadeService.modalService.showConfirmMessage('There is no content to view.');
  }

  private get hasLecture(): boolean {
    return this.lecturesMap != null && !!this.lecturesMap.size;
  }

  private markContentChanged(contentChanged?: boolean): void {
    this.hasContentChanged =
      contentChanged == null ? this.course.myCourseInfo && this.course.myCourseInfo.hasContentChanged : contentChanged;
  }

  private handleShowMessageNoSessionOrContent(): void {
    // Case: No content & no session
    if (!this.hasLecture && this.currentClassRun && this.currentClassRun.sessions && this.currentClassRun.sessions.length === 0) {
      this.showMessage('There is no session & online content in this class. You can still proceed with learning the course.');
    }

    // Case: No content & have session
    if (!this.hasLecture && this.currentClassRun && this.currentClassRun.sessions && this.currentClassRun.sessions.length > 0) {
      this.showMessage('There is no online content in this class. You can still proceed with learning the course.');
    }

    if (this.hasLecture && this.currentClassRun && this.currentClassRun.sessions && this.currentClassRun.sessions.length === 0) {
      this.showMessage('There is no session in this class. You can still proceed with learning the course.');
    }
  }

  private showMessage(message: string): void {
    this.opalDialogService
      .openConfirmDialog({
        confirmTitle: 'Warning',
        confirmMsg: message,
        hideNoBtn: true,
        yesBtnText: 'OK'
      })
      .subscribe();
  }

  public get detailUrl(): string {
    return `${AppGlobal.environment.appUrl}/learner/detail/${this.learningItem.type.toLocaleLowerCase()}/${this.learningItem.id}`;
  }

  private triggerToStartOrContinueCourseTask(): void {
    if (this.disableLearning) {
      return;
    }

    if (this.canStartTask) {
      this.course.isMicrolearning ? this.startMicrolearning() : this.startClassRun();
    }

    if (this.canContinueTask) {
      this.course.isMicrolearning ? this.continueMicrolearning() : this.continueClassRun();
    }
  }

  private hideLecturePlayer(): void {
    this.canStartTask = false;
    this.canContinueTask = false;
    this.showLecturePlayer = false;
  }

  private setTrackingActivity(enable: boolean): void {
    this.enableUserTracking = enable;
  }

  private viewTracking(trackingType: LearningType): void {
    if (!this.enableUserTracking) {
      return;
    }
    this.trackingSourceSrv.eventTrack.next({
      eventName: 'LearningTracking',
      payload: <LearningTrackingEventPayload>{
        itemId: this.courseId,
        trackingType: trackingType.toString(),
        trackingAction: 'view'
      }
    });
  }

  private getTrackingInfoByItemId(trackingType: LearningType): void {
    this.userTrackingService.getTrackingInfoByItemId(this.courseId, trackingType).then(result => {
      this.totalDownload = result.totalDownload;
      this.totalLike = result.totalLike;
      this.totalShare = result.totalShare;
      this.totalView = result.totalView;
      this.isLike = result.isLike;
    });
  }
}
