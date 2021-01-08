import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { CxGlobalLoaderService } from '@conexus/cx-angular-common';
import { TranslateService } from '@ngx-translate/core';
import { ClassRunModel } from 'app-models/classrun.model';
import { TabMenuItem } from 'app-models/common/cx-tab-menu-item';
import { ICRUDPermission } from 'app-models/common/permission/permission-setting';
import { CourseContentItemModel } from 'app-models/course-content-item.model';
import { CourseReviewManagementModel } from 'app-models/course-review.model';
import {
  PDOpportunityAnswerDTO,
  PDOpportunityDTO,
  PDOpportunityModel,
  PDOSource,
} from 'app-models/mpj/pdo-action-item.model';
import { CourseDetailTabEnum, SessionModel } from 'app-models/session.model';
import { CommentEventEntity } from 'app-services/comment-event.constant';
import { CommentService } from 'app-services/comment.service';
import { PDOpportunityDetailModel } from 'app-services/pd-opportunity/pd-opportunity-detail.model';
import {
  CommentChangeData,
  CommentData,
} from 'app/individual-development/cx-comment/comment.model';
import { CxCommentComponent } from 'app/individual-development/cx-comment/cx-comment.component';
import { isEmpty } from 'lodash';
import { PDPlannerHelpers } from './../../services/idp/pd-planner/pd-planner-helpers';

@Component({
  selector: 'course-detail',
  templateUrl: './course-detail.component.html',
  styleUrls: ['./course-detail.component.scss'],
})
export class CourseDetailComponent implements OnInit, AfterViewInit {
  @Input() pdoDetail: PDOpportunityDetailModel;
  @Input() pdoModel: PDOpportunityModel;
  @Input() classrunDetail: ClassRunModel[];
  @Input() sessions: SessionModel[];
  @Input() reviews: CourseReviewManagementModel;
  @Input() contents: CourseContentItemModel[];
  @Input() selectedTab: CourseDetailTabEnum;
  @Input() pdPlanMode: boolean = false;
  @Input() allowEditExternalPDO: boolean = false;
  @Input() selectCourseMode: boolean = false;
  @Input() selectCourseModeDisplayText: string = 'Add to plan';
  @Input() selectCourseModeSelected: boolean = false;
  @Input() commentPermission: ICRUDPermission = {
    allowCreate: true,
    allowDelete: true,
    allowEdit: true,
    allowView: true,
  };
  @Output()
  updatedExternalPDO: EventEmitter<PDOpportunityDTO> = new EventEmitter<PDOpportunityDTO>();
  @Output() loadMoreReview: EventEmitter<number> = new EventEmitter();
  @Output() addToPlan: EventEmitter<string> = new EventEmitter();

  @ViewChild('courseAboutElement', { static: true })
  courseAboutElement: ElementRef;
  @ViewChild('courseContentElement', { static: true })
  courseContentElement: ElementRef;
  @ViewChild('courseClassrunElement', { static: true })
  courseClassrunElement: ElementRef;
  @ViewChild('courseReviewElement', { static: true })
  courseReviewElement: ElementRef;
  @ViewChild('courseCommentElement', { static: true })
  courseCommentElement: ElementRef;
  @ViewChild('commentComponent') commentComponent: CxCommentComponent;

  tabs: TabMenuItem[];
  currentTab: TabMenuItem;
  selectedTabInput: boolean = false;
  pdoAnswer: PDOpportunityAnswerDTO;
  commentEventEntity: CommentEventEntity = CommentEventEntity.IdpPdo;
  comments: CommentData[];

  constructor(
    private translateService: TranslateService,
    private commentService: CommentService,
    private globalLoader: CxGlobalLoaderService
  ) {}

  ngOnInit(): void {
    this.initDataForPDOAnswer();
    this.tabs = this.menuItemBuilder();
    this.currentTab = this.selectedTab
      ? this.tabs.find((p) => p.id === this.selectedTab)
      : this.tabs[0];
    this.selectedTabInput = this.selectedTab != null;
    if (this.pdPlanMode) {
      this.getComment();
    }
  }

  ngAfterViewInit(): void {
    if (this.selectedTabInput) {
      this.scrollToView(this.currentTab);
    }
  }

  scrollToView(tab: TabMenuItem): void {
    this.currentTab = tab;
    tab.elementRef.nativeElement?.scrollIntoView();
  }

  onLoadMoreReview(itemCount: number): void {
    this.loadMoreReview.emit(itemCount);
  }

  onChangeComment(changeData: CommentChangeData): void {
    this.globalLoader.showLoader();
    this.commentService
      .saveComment(
        this.commentEventEntity,
        this.pdoModel.identityActionItemDTO.extId,
        changeData
      )
      .subscribe(
        (newCommentItem) => {
          changeData.commentItem = newCommentItem;
          changeData.changeResult = true;
          this.commentComponent.changeCommentResult(changeData);
          this.globalLoader.hideLoader();
        },
        (error) => {
          console.error(error);
          this.globalLoader.hideLoader();
        }
      );
  }

  onAddToPlanClicked(): void {
    this.addToPlan.emit(this.pdoDetail.id);
  }

  get isExternalPDO(): boolean {
    return (
      this.pdoAnswer &&
      this.pdoAnswer.learningOpportunity.source === PDOSource.CustomPDO
    );
  }

  public showClassrunDetailDefault(): boolean {
    return this.classrunDetail.length === 1;
  }

  private menuItemBuilder(): TabMenuItem[] {
    const menus = [
      {
        id: CourseDetailTabEnum.About,
        name: this.translateService.instant('Common.Label.About'),
        elementRef: this.courseAboutElement,
      },
    ];

    const contentMenuItem = {
      id: CourseDetailTabEnum.Content,
      name: this.translateService.instant('Common.Label.Content'),
      elementRef: this.courseContentElement,
    };

    menus.push(contentMenuItem);

    if (!this.isExternalPDO) {
      const classRunMenuItem = {
        id: CourseDetailTabEnum.ClassRun,
        name: this.translateService.instant('Common.Label.ClassRun'),
        elementRef: this.courseClassrunElement,
      };

      menus.push(classRunMenuItem);
    }

    if (!isEmpty(this.reviews)) {
      const reviewMenuItem = {
        id: CourseDetailTabEnum.Review,
        name: this.translateService.instant('Common.Label.Review'),
        elementRef: this.courseReviewElement,
      };

      menus.push(reviewMenuItem);
    }

    if (this.pdPlanMode) {
      const commentMenuItem = {
        id: CourseDetailTabEnum.Comment,
        name: this.translateService.instant('Common.Label.Comment'),
        elementRef: this.courseCommentElement,
      };

      menus.push(commentMenuItem);
    }

    return menus;
  }

  private getComment(): void {
    if (!this.pdoModel) {
      return;
    }
    this.commentService
      .getComments(
        this.commentEventEntity,
        this.pdoModel.identityActionItemDTO.extId
      )
      .subscribe((comments) => {
        this.comments = comments;
      });
  }

  private initDataForPDOAnswer(): void {
    if (this.pdoModel) {
      this.pdoAnswer = this.pdoModel?.answerDTO;
    } else if (this.pdoDetail) {
      this.pdoAnswer = PDPlannerHelpers.toPDOpportunityAnswerDTOFromCourseDetail(
        this.pdoDetail
      );
    }
  }
}
