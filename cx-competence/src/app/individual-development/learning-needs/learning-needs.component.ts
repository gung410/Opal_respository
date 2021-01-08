import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { CxGlobalLoaderService } from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Chart } from 'angular-highcharts';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import {
  ILearningNeedPermission,
  LearningNeedPermission,
} from 'app-models/common/permission/learning-need-permission';
import { ChartInfo, IdpConfigParams } from 'app-models/pdplan.model';
import { CommentEventEntity } from 'app-services/comment-event.constant';
import { CommentService } from 'app-services/comment.service';
import { IdpService } from 'app-services/idp.service';
import { PdPlannerService } from 'app-services/idp/pd-planner/pd-planner.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import { CxGanttChartModel } from 'app/shared/components/cx-gantt-chart/models/cx-gantt-chart.model';
import { isEmpty } from 'lodash';
import { CommentChangeData, CommentData } from '../cx-comment/comment.model';
import { CxCommentComponent } from '../cx-comment/cx-comment.component';
import {
  EvaluationTypeToIdpStatusCode,
  IDPMode,
  IdpStatusCodeEnum,
  PDEvaluationType,
} from '../idp.constant';
import { LearningNeedsAnalysisReviewDialogComponent } from '../learning-needs-analysis/learning-needs-analysis-review-dialog/learning-needs-analysis-review-dialog.component';
import { PDEvaluationModel } from '../models/pd-evaluation.model';
import LearningAreaChartHelper from '../shared/learning-area-chart/learning-area-chart.helper';
import { LearningAreaChartModel } from '../shared/learning-area-chart/learning-area-chart.model';
import { PdEvaluationDialogComponent } from '../shared/pd-evalution-dialog/pd-evaluation-dialog.component';
import {
  ILearningNeedAnalysisPermission,
  LearningNeedAnalysisPermission,
} from './../../shared/models/common/permission/learning-need-analysis-permission';

const evaluationDialogHeader: any = {
  [PDEvaluationType.Approve]: 'MyPdJourney.Action.Acknowledge',
  [PDEvaluationType.Reject]: 'MyPdJourney.Action.Reject',
};

const evaluationDialogSubHeader: any = {
  [PDEvaluationType.Approve]: undefined,
  [PDEvaluationType.Reject]: 'MyPdJourney.Action.RejectSubHeader',
};

const evaluationDialogActionName: any = {
  [PDEvaluationType.Approve]: 'MyPdJourney.Action.Acknowledge',
  [PDEvaluationType.Reject]: 'MyPdJourney.Action.Reject',
};

@Component({
  selector: 'learning-needs',
  templateUrl: './learning-needs.component.html',
  styleUrls: ['./learning-needs.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class LearningNeedsComponent
  extends BaseScreenComponent
  implements
    OnChanges,
    OnInit,
    ILearningNeedPermission,
    ILearningNeedAnalysisPermission {
  @Input() needsResults: IdpDto[];
  @Input() user: any;
  @Input() mode: IDPMode = IDPMode.Learner;
  @Output() needToReloadLNA: EventEmitter<void> = new EventEmitter<void>();

  currentStatus: string;
  IDPMode: any = IDPMode;
  visibleToolbarActions: boolean;
  commentEventEntity: CommentEventEntity =
    CommentEventEntity.IdpLearningNeedsAnalysis;
  comments: CommentData[];
  learningAreaCharts: LearningAreaChartModel[];
  carrerChartData: CxGanttChartModel;
  learningNeedPermission: LearningNeedPermission;
  learningNeedAnalysisPermission: LearningNeedAnalysisPermission;

  get currentNeedsResult(): IdpDto {
    return this._currentNeedsResult;
  }

  set currentNeedsResult(result: IdpDto) {
    if (!result) {
      return;
    }
    this._currentNeedsResult = result;
    this.currentStatus = this._currentNeedsResult.assessmentStatusInfo.assessmentStatusCode;
    this.updateToolbarActionsVisibility();
    this.getLearningNeedsReportData(this._currentNeedsResult.resultIdentity.id);

    this.commentService
      .getComments(
        this.commentEventEntity,
        this._currentNeedsResult.resultIdentity.extId
      )
      .subscribe((comments) => {
        this.comments = comments;
      });
  }

  private _currentNeedsResult: IdpDto;

  @ViewChild('commentComponent') private commentComponent: CxCommentComponent;

  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private idpService: IdpService,
    private pdPlannerService: PdPlannerService,
    private ngbModal: NgbModal,
    private translateAdapterService: TranslateAdapterService,
    private commentService: CommentService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    protected authService: AuthService
  ) {
    super(changeDetectorRef, authService);
  }

  ngOnInit(): void {
    this.initData();
  }

  ngOnChanges(): void {
    if (this.needsResults && this.needsResults.length > 0) {
      this.currentNeedsResult = this.needsResults[0];
    }
  }

  initLearningNeedPermission(loginUser: User): void {
    this.learningNeedPermission = new LearningNeedPermission(
      loginUser,
      this.mode
    );
  }

  initLearningNeedAnalysisPermission(loginUser: User): void {
    this.learningNeedAnalysisPermission = new LearningNeedAnalysisPermission(
      loginUser,
      this.mode
    );
  }

  onReviewLNA(lna: IdpDto): void {
    const modalRef = this.ngbModal.open(
      LearningNeedsAnalysisReviewDialogComponent,
      { centered: true, windowClass: 'modal-size-xl' }
    );
    const componentInstance = modalRef.componentInstance as LearningNeedsAnalysisReviewDialogComponent;
    componentInstance.needsResults = this.needsResults;
    componentInstance.user = this.user;
    componentInstance.mode = this.mode;
    componentInstance.learningNeeds = lna;
    componentInstance.cancel.subscribe(() => {
      modalRef.close();
    });
    componentInstance.needToReloadLNA.subscribe(() => {
      this.needToReloadLNA.emit();
      modalRef.close();
    });
  }

  getLearningNeedsReportData(needsResultId: number): void {
    const params = new IdpConfigParams({
      resultId: needsResultId,
    });
    this.idpService.getLearningNeedReport(params).subscribe((reports) => {
      if (!reports) {
        this.learningAreaCharts = [];

        return;
      }
      const chartModels: LearningAreaChartModel[] = [];
      reports.forEach((report) => {
        const prioritisationChart: ChartInfo = LearningAreaChartHelper.reportDataToChartInfo(
          report
        );
        const timeSpentCharts: Chart[] = LearningAreaChartHelper.seriesToPieCharts(
          prioritisationChart.series
        );
        const chartModel = new LearningAreaChartModel({
          prioritisationChart,
          timeSpentCharts,
        });
        chartModels.push(chartModel);
      });
      this.learningAreaCharts = chartModels;
    });
  }

  public onPeriodChanged(result: IdpDto): void {
    if (
      result.resultIdentity.id === this.currentNeedsResult.resultIdentity.id
    ) {
      return;
    }
    this.currentNeedsResult = result;
  }

  public onEvaluated(event: PDEvaluationModel): void {
    this.cxGlobalLoaderService.showLoader();

    this.idpService
      .changeStatusLearningNeeds({
        resultIdentities: [this.currentNeedsResult.resultIdentity],
        targetStatusType: {
          assessmentStatusCode: EvaluationTypeToIdpStatusCode[event.type],
        },
      })
      .subscribe(
        (resultResponse) => {
          this.cxGlobalLoaderService.hideLoader();
          if (!resultResponse) {
            return;
          }
          const changeStatusResult = resultResponse.find((result) =>
            this.needsResults.some(
              (plan) => result.identity.extId === plan.resultIdentity.extId
            )
          );
          const needsResultIndex = this.needsResults.findIndex(
            (plan) =>
              changeStatusResult.identity.extId === plan.resultIdentity.extId
          );

          if (needsResultIndex > -1) {
            this.needsResults[needsResultIndex] = {
              ...this.needsResults[needsResultIndex],
              assessmentStatusInfo: changeStatusResult.targetStatusType,
            };
            this.currentNeedsResult = {
              ...this.needsResults[needsResultIndex],
            };
          }
          this.commentService
            .saveEvaluationComment(
              this.commentEventEntity,
              changeStatusResult.identity.extId,
              event
            )
            .subscribe(() => {});
          this.getChartData();
        },
        () => {
          this.cxGlobalLoaderService.hideLoader();
        }
      );
  }

  public onApprove(): void {
    this.evaluate(PDEvaluationType.Approve);
  }

  public onReject(): void {
    this.evaluate(PDEvaluationType.Reject);
  }

  onChangeComment(changeData: CommentChangeData): void {
    this.commentService
      .saveComment(
        this.commentEventEntity,
        this.currentNeedsResult.resultIdentity.extId,
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
        }
      );
  }

  get isValidCarrerAspirationChartData(): boolean {
    return this.carrerChartData && !isEmpty(this.carrerChartData.tasks);
  }

  private initData(): void {
    const currentUser = this.authService.userData().getValue();
    this.initLearningNeedPermission(currentUser);
    this.initLearningNeedAnalysisPermission(currentUser);
    this.getChartData();
  }

  private evaluate(type: PDEvaluationType): void {
    if (this.ngbModal.hasOpenModals()) {
      this.ngbModal.dismissAll();
    }

    const modalRef = this.ngbModal.open(PdEvaluationDialogComponent, {
      centered: true,
      size: 'lg',
    });
    const modalRefComponentInstance = modalRef.componentInstance as PdEvaluationDialogComponent;
    modalRefComponentInstance.header = this.translateAdapterService.getValueImmediately(
      evaluationDialogHeader[type]
    );
    modalRefComponentInstance.subHeader = this.translateAdapterService.getValueImmediately(
      evaluationDialogSubHeader[type]
    );
    modalRefComponentInstance.doneButtonText = this.translateAdapterService.getValueImmediately(
      evaluationDialogActionName[type]
    );
    modalRefComponentInstance.done.subscribe((reason) => {
      this.onEvaluated(new PDEvaluationModel({ type, reason }));
      modalRef.close();
    });
    modalRefComponentInstance.cancel.subscribe(() => modalRef.close());
  }

  private updateToolbarActionsVisibility(): boolean {
    return (this.visibleToolbarActions =
      this.mode === IDPMode.ReportingOfficer &&
      this.currentStatus === IdpStatusCodeEnum.PendingForApproval);
  }

  private async getChartData(): Promise<void> {
    const userExtId =
      this.user && this.user.identity && this.user.identity.extId;
    if (userExtId) {
      this.carrerChartData = undefined;
      this.carrerChartData = await this.pdPlannerService.getCarrerAspirationData(
        userExtId
      );
    }
  }
}
