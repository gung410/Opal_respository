import {
  ChangeDetectorRef,
  Component,
  Input,
  OnInit,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import {
  CxFormModal,
  CxGlobalLoaderService,
  cxThrottle,
} from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from 'app-auth/auth.service';
import { ResultIdentity } from 'app-models/assessment.model';
import { User } from 'app-models/auth.model';
import {
  PDOpportunityDTO,
  PDOpportunityModel,
} from 'app-models/mpj/pdo-action-item.model';
import { PagingResponseModel } from 'app-models/user-management.model';
import { CommentEventEntity } from 'app-services/comment-event.constant';
import { CommentService } from 'app-services/comment.service';
import { IdpService } from 'app-services/idp.service';
import { ExternalPDOService } from 'app-services/idp/pd-catalogue/external-pdo.service';
import { PDPlannerHelpers } from 'app-services/idp/pd-planner/pd-planner-helpers';
import { PdPlannerService } from 'app-services/idp/pd-planner/pd-planner.service';
import { ResultHelper } from 'app-services/idp/result-helpers';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { ObjectUtilities } from 'app-utilities/object-utils';
import { CourseDetailModalComponent } from 'app/approval-page/modals/course-detail-modal/course-detail-modal.component';
import {
  EvaluationTypeToIdpStatusCode,
  IDPMode,
  IdpStatusCodeEnum,
  PDEvaluationType,
} from 'app/individual-development/idp.constant';
import {
  PDPlanActionEnum,
  WebViewMessage,
} from 'app/mobile/models/webview-message.model';
import { MobileAuthService } from 'app/mobile/services/mobile-auth.service';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import { ODPFilterParams } from 'app/organisational-development/models/odp.models';
import { Constant } from 'app/shared/app.constant';
import { UserStatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { clone, isEmpty } from 'lodash';
import { ToastrService } from 'ngx-toastr';
import { CommentChangeData, CommentData } from '../cx-comment/comment.model';
import { CxCommentComponent } from '../cx-comment/cx-comment.component';
import { CoursepadNoteDto } from '../models/course-note.model';
import { PDEvaluationModel } from '../models/pd-evaluation.model';
import { PdEvaluationDialogComponent } from '../shared/pd-evalution-dialog/pd-evaluation-dialog.component';
import {
  IPDPlanPermission,
  PDPlanPermission,
} from './../../shared/models/common/permission/pdplan-permission';

enum SortEnum {
  Upcoming = 'upcoming',
  Nominated = 'nominated',
  Completed = 'completed',
}

const evaluationDialogHeader = {
  [PDEvaluationType.Approve]: 'MyPdJourney.Action.Acknowledge',
  [PDEvaluationType.Reject]: 'MyPdJourney.Action.Reject',
};

const evaluationDialogActionName = {
  [PDEvaluationType.Approve]: 'MyPdJourney.Action.Acknowledge',
  [PDEvaluationType.Reject]: 'MyPdJourney.Action.Reject',
};

@Component({
  selector: 'pd-opportunities',
  templateUrl: './pd-opportunities.component.html',
  styleUrls: ['./pd-opportunities.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class PdOpportunitiesComponent implements OnInit, IPDPlanPermission {
  @Input() user: Staff;
  @Input() mode: IDPMode = IDPMode.Learner;
  @Input() needsResults: IdpDto[];

  public IDPMode: any = IDPMode;
  public learningOpportunitiesResponse: PagingResponseModel<PDOpportunityModel> = new PagingResponseModel<PDOpportunityModel>();
  public bookmarkedCourses: Map<string, CoursepadNoteDto>;
  public sorts: { value: SortEnum; text: string }[] = [
    { value: SortEnum.Upcoming, text: 'Upcoming' },
    { value: SortEnum.Nominated, text: 'Nominated' },
    { value: SortEnum.Completed, text: 'Completed' },
  ];
  public currentSort: any = this.sorts[0].value;
  public statusCode: any = IdpStatusCodeEnum;
  public pdplans: IdpDto[];
  public currentStatus: string;
  public visibleToolbarActions: boolean;
  public period: number;
  commentEventEntity: CommentEventEntity = CommentEventEntity.IdpPlan;
  comments: CommentData[];

  // Flag variables
  validToManagedPDO: boolean = false;
  validToAddPDO: boolean = false;
  validToSubmitPDPlan: boolean = false;

  public pageIndex: number = 1;
  public pageSize: number = 10;

  private _currentPlan: IdpDto;
  pdPlanPermission: PDPlanPermission;
  public set currentPlan(plan: IdpDto) {
    const isDifferentPlan =
      !this._currentPlan ||
      this._currentPlan.resultIdentity.extId !== plan.resultIdentity.extId;
    this._currentPlan = plan;
    this.currentStatus = this._currentPlan.assessmentStatusInfo.assessmentStatusCode;
    this.updateToolbarActionsVisibility();

    if (isDifferentPlan) {
      // System should not get the list of action items again if the same PD Plan is set to the component.
      //  e.g: The PD Plan has changed the status type but the list of action items didn't changed.
      this.period = +plan?.surveyInfo?.name;
      this.getListActionItem();

      this.commentService
        .getComments(
          this.commentEventEntity,
          this.currentPlan.resultIdentity.extId
        )
        .subscribe((comments) => {
          this.comments = comments;
        });
    }
  }
  public get currentPlan(): IdpDto {
    return this._currentPlan;
  }

  @ViewChild('commentComponent') private commentComponent: CxCommentComponent;
  constructor(
    private idpService: IdpService,
    private toastrService: ToastrService,
    private cxFormModal: CxFormModal,
    private ngbModal: NgbModal,
    private translateAdapterService: TranslateAdapterService,
    private pdplannerService: PdPlannerService,
    private globalLoader: CxGlobalLoaderService,
    private commentService: CommentService,
    private externalPDOService: ExternalPDOService,
    private communicateService: MobileAuthService,
    private changeDetectorRef: ChangeDetectorRef,
    private authService: AuthService
  ) {}
  initPDPlanPermission(loginUser: User): void {
    this.pdPlanPermission = new PDPlanPermission(loginUser, this.mode);
  }

  ngOnInit(): void {
    this.initPDPlanPermission(this.authService.userData().getValue());
    this.getPlans();
  }

  public planNextPeriod(): void {
    this.globalLoader.showLoader();
    const currentYearPeriod = new Date(
      this.pdplans[this.pdplans.length - 1].surveyInfo.startDate
    ).getFullYear();
    const monthOfPeriod = 5;
    const nextPeriod = new Date(Date.UTC(currentYearPeriod + 1, monthOfPeriod));
    const newPlan = new IdpDto();
    newPlan.objectiveInfo = {
      identity: this.user.identity,
    };
    newPlan.assessmentStatusInfo = {
      assessmentStatusCode: IdpStatusCodeEnum.Started,
    };
    newPlan.timestamp = nextPeriod.toUTCString();
    newPlan.forceCreateResult = true;
    this.idpService.savePlanResult(newPlan).subscribe(
      (plan) => {
        this.globalLoader.hideLoader();
        if (!plan) {
          return;
        }
        this.pdplans.push(plan);
        const currentPlan = this.pdplans[this.pdplans.length - 1];
        this.setCurrentPlan(currentPlan);
      },
      () => {
        this.globalLoader.hideLoader();
      }
    );
  }

  public startNewPlan(): void {
    // Check valid user status before creating new PD Plan for the user.
    const validUserStatusToStartNewPlan = [
      UserStatusTypeEnum.New,
      UserStatusTypeEnum.Active,
    ];
    if (
      !validUserStatusToStartNewPlan.includes(this.user.entityStatus.statusId)
    ) {
      return;
    }
    this.globalLoader.showLoader();
    const newPlan = new IdpDto();
    newPlan.objectiveInfo = {
      identity: this.user.identity,
    };
    newPlan.assessmentStatusInfo = {
      assessmentStatusCode: IdpStatusCodeEnum.Started,
    };
    this.idpService.savePlanResult(newPlan).subscribe(
      (plan) => {
        this.globalLoader.hideLoader();
        if (!plan) {
          return;
        }
        this.getPlans();
      },
      () => {
        this.globalLoader.hideLoader();
      }
    );
  }

  // TODO: OP-6661
  // public addFromCatalog(): void {
  //   this.openCatalogDialog();
  // }

  // TODO: OP-6661
  // async openCatalogDialog(): Promise<void> {
  //   if (this.cxFormModal.hasOpenModals()) { return; }
  //   this.globalLoader.showLoader();
  //   const tagIds = this.getUserGroupTags();
  //   const selectedCourseIds = this.getSelectedCourseIdsFromOpportunities();
  //   const personnelGroupsIds = this.getPersonelGroupIds(this.user);

  //   const modalRef = this.pdplannerService.openOpportunitiesCatalogDialog(
  //     tagIds,
  //     selectedCourseIds,
  //     personnelGroupsIds,
  //     true
  //   );

  //   modalRef.addToPlan.subscribe(this.onSelectOpportunity);
  //   modalRef.addToBookmark.subscribe(this.onSelectBookmark);
  //   modalRef.addExternalPDO.subscribe(this.onAddExternalPDO);
  //   this.globalLoader.hideLoader();
  // }

  // tslint:disable-next-line:typedef
  // getSelectedCourseIdsFromOpportunities() {
  //   return this.learningOpportunitiesResponse.items.map(opp => {
  //     if (
  //       opp.answerDTO &&
  //       opp.answerDTO.learningOpportunity &&
  //       opp.answerDTO.learningOpportunity.uri
  //     ) {
  //       return PDPlannerHelpers.getCourseIdFromURI(opp.answerDTO.learningOpportunity.uri);
  //     }
  //   });
  // }

  public onSortChanged(): void {
    // TODO
  }

  public onPeriodChanged({ value }): void {
    const selectedPlan = this.pdplans.find(
      (plan) => plan.resultIdentity.extId === value
    );
    if (!selectedPlan) {
      return;
    }
    this.setCurrentPlan(selectedPlan);
  }

  public submitForApproval(): void {
    const canSubmit = this.pdplannerService.checkCanSubmitPlan(
      this.currentPlan,
      this.needsResults,
      this.user
    );
    if (!canSubmit) {
      return;
    }

    this.changePlanStatus(IdpStatusCodeEnum.PendingForApproval);
  }

  public onRemoveActionItem(actionItem: PDOpportunityModel): void {
    this.removeActionItems(actionItem.identityActionItemDTO);
  }

  public onEvaluated(event: PDEvaluationModel): void {
    this.globalLoader.showLoader();
    this.idpService
      .changeStatusPDPLan({
        resultIdentities: [this.currentPlan.resultIdentity],
        targetStatusType: {
          assessmentStatusCode: EvaluationTypeToIdpStatusCode[event.type],
        },
      })
      .subscribe(
        (planResponse) => {
          this.globalLoader.hideLoader();
          if (!planResponse) {
            return;
          }
          const changeStatusResult = planResponse.find((result) =>
            this.pdplans.some(
              (plan) => result.identity.extId === plan.resultIdentity.extId
            )
          );
          const planIndex = this.pdplans.findIndex(
            (plan) =>
              changeStatusResult.identity.extId === plan.resultIdentity.extId
          );
          if (planIndex > -1) {
            const currentPlan = {
              ...this.pdplans[planIndex],
              assessmentStatusInfo: changeStatusResult.targetStatusType,
            };
            this.setCurrentPlan(currentPlan);
            // TODO: Disable for now OP-6736
            // this.afterPDPlanStatusChanged(planResponse, this.learningOpportunitiesResponse.items);
            this.commentService
              .saveEvaluationComment(
                this.commentEventEntity,
                currentPlan.resultIdentity.extId,
                event
              )
              .subscribe(() => {});
          }
        },
        () => {
          this.globalLoader.hideLoader();
        }
      );
  }

  public onApprove(): void {
    this.evaluate(PDEvaluationType.Approve);
  }

  public onReject(): void {
    this.evaluate(PDEvaluationType.Reject);
  }

  // TODO: Disable for now
  // public onBookmarkClicked(): void {
  //   this.openCatalogDialog();
  // }

  onClickedPDO(pdoData: PDOpportunityModel): void {
    if (!pdoData) {
      return;
    }

    const ngbModal = this.ngbModal.open(CourseDetailModalComponent, {
      centered: true,
      size: 'lg',
      windowClass: 'mobile-dialog-slide-right',
    });

    const courseUri = pdoData.additionalProperties.learningOpportunityUri;
    if (!courseUri) {
      return console.error('Cannot get course uri');
    }

    const courseId = PDPlannerHelpers.getCourseIdFromURI(courseUri);
    const modalRef = ngbModal.componentInstance as CourseDetailModalComponent;
    modalRef.resultId = pdoData.identityActionItemDTO.id;
    modalRef.courseId = courseId;
    modalRef.isExternalPDO = PDPlannerHelpers.isExternalPDO(pdoData);
    modalRef.pdoModel = pdoData;
    modalRef.pdPlanMode = true;
    modalRef.commentPermission = this.pdPlanPermission.comment;
    modalRef.allowEditExternalPDO = this.checkAllowEditExternalPDO(pdoData);
    modalRef.close.subscribe(() => ngbModal.close(false));
    modalRef.updatedExternalPDO.subscribe(async (data) => {
      const updatedPDOModel = await this.submitEditedExternalPDO(data);
      if (updatedPDOModel) {
        ngbModal.close();
        this.onClickedPDO(updatedPDOModel);
      }
    });
  }

  async toogleCompleteExternalPDO(
    pdoData: PDOpportunityModel,
    isCompleted: boolean
  ): Promise<void> {
    const result = await this.pdplannerService.updateExternalPdoCompleteStatus(
      pdoData,
      isCompleted
    );
    this.onPDOUpdated(result);
  }

  onChangeComment(changeData: CommentChangeData): void {
    this.commentService
      .saveComment(
        this.commentEventEntity,
        this.currentPlan.resultIdentity.extId,
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

  onPDOUpdated = (pdoModel: PDOpportunityModel): void => {
    if (!pdoModel) {
      return;
    }

    const currentPDO = this.learningOpportunitiesResponse.items.find(
      (pdo) =>
        pdo.identityActionItemDTO.id === pdoModel.identityActionItemDTO.id
    );
    if (!currentPDO) {
      return;
    }

    if (PDPlannerHelpers.isExternalPDO(pdoModel)) {
      PDPlannerHelpers.updateTagForExternalPDO(
        pdoModel.answerDTO.learningOpportunity
      );
    }

    const index = this.learningOpportunitiesResponse.items.indexOf(currentPDO);
    this.learningOpportunitiesResponse.items[index] = ObjectUtilities.clone(
      pdoModel
    );

    this.changeDetectorRef.detectChanges();
  };

  @cxThrottle(2000)
  addFromCatalog(): void {
    const message = new WebViewMessage({
      action: PDPlanActionEnum.CLICKED_OPEN_PDCATALOGUE,
    });
    this.communicateService.sendMessage(message);
  }

  onClickAddExternalPDO(): void {
    this.showExternalPDOForm();
  }

  allowRemove(pdo: PDOpportunityModel): boolean {
    if (!pdo || this.mode !== IDPMode.Learner) {
      return false;
    }

    const isExternalPDO = PDPlannerHelpers.isExternalPDOByAnswer(pdo.answerDTO);

    return (
      pdo.canDelete &&
      ((isExternalPDO &&
        this.pdPlanPermission.externalPDOpportunity.allowDelete) ||
        (!isExternalPDO && this.pdPlanPermission.pdOpportunity.allowDelete))
    );
  }

  public async getListActionItem(
    pageSize: number = 10,
    pageIndex: number = 1
  ): Promise<void> {
    window.scroll(0, 0);
    this.pageSize = pageSize;
    this.pageIndex = pageIndex;
    this.globalLoader.showLoader();
    const learnerId: number = this.user.identity.id;
    this.learningOpportunitiesResponse = await this.pdplannerService.getPlannedPDOsAsync(
      this.period,
      learnerId,
      pageSize,
      pageIndex
    );
    this.checkRight();
    this.globalLoader.hideLoader();
  }
  // TODO: Hide boookmark for now
  // private onSelectBookmark = (opportunity: OpportunityModel): void => {
  //   if (!opportunity || !opportunity.dto) {
  //     return;
  //   }
  //   const courseId = opportunity.dto.courseId.toString();
  //   if (opportunity.isBookmarked) {
  //     this.pdCatalogueService.uncheckBookmark(courseId).subscribe(response => {
  //       this.toastrService.success('Uncheck Bookmark successfully.');
  //       opportunity.isBookmarked = !opportunity.isBookmarked;
  //       this.bookmarkedCourses.delete(courseId);

  //     }, reason => {
  //       this.toastrService.warning('Something went wrong, please try again!');
  //     });
  //   } else {

  //     this.pdCatalogueService.saveBookmarkToPlan(courseId).subscribe(response => {
  //       this.toastrService.success('Add Bookmark successfully.');
  //       opportunity.isBookmarked = !opportunity.isBookmarked;
  //       this.bookmarkedCourses.set(courseId, opportunity.dto);
  //     }, reason => {
  //       this.toastrService.warning('Something went wrong, please try again!');
  //     });
  //   }

  // }
  // TODO: OP-6661
  // private onSelectOpportunity = async (opportunity: OpportunityModel): Promise<void> => {
  //   if (!opportunity || !opportunity.dto) {
  //     return;
  //   }

  //   const pdoDTO = opportunity.dto;
  //   const courseId = opportunity.dto.courseId;
  //   const isExisted = PDPlannerHelpers.checkPDOExisted(courseId, this.learningOpportunitiesResponse.items);
  //   if (isExisted) {
  //     this.toastrService.info('This course existed on your plan.');
  //     opportunity.isSelected = true;

  //     return;
  //   }

  //   const timestamp = this.currentPlan.timestamp;
  //   const userIdentity = this.user.identity;
  //   const actionItemResp = await this.pdplannerService.addPDOCatalogToPlanAsync(pdoDTO, timestamp, userIdentity);
  //   if (actionItemResp) {
  //     this.updatePlanWhenAddPDOSuccess(actionItemResp);
  //     opportunity.isSelected = true;
  //   } else {
  //     this.toastrService.warning('Some thing went wrong.');
  //   }
  // }

  private updatePlanWhenAddPDOSuccess(newPDO: PDOpportunityModel): void {
    this.learningOpportunitiesResponse.items = clone(
      [newPDO].concat(this.learningOpportunitiesResponse.items)
    );
    this.checkRight();
    // TODO: Disable for now OP-6736
    // this.changePlanStatus(IdpStatusCodeEnum.Started);
    this.toastrService.success(
      'Added PD opportunity successfully to your PD Plan.'
    );
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
    modalRefComponentInstance.doneButtonText = this.translateAdapterService.getValueImmediately(
      evaluationDialogActionName[type]
    );
    modalRefComponentInstance.done.subscribe((reason) => {
      this.onEvaluated(new PDEvaluationModel({ type, reason }));
      modalRef.close();
    });
    modalRefComponentInstance.cancel.subscribe(() => modalRef.close());
  }

  private updateToolbarActionsVisibility(): void {
    this.visibleToolbarActions =
      this.mode === IDPMode.ReportingOfficer &&
      this.currentStatus === IdpStatusCodeEnum.PendingForApproval;
  }

  private async removeActionItems(
    resultIdentity: ResultIdentity
  ): Promise<void> {
    if (!resultIdentity) {
      return;
    }

    this.globalLoader.showLoader();
    const result = await this.pdplannerService.removePDOOnPlanAsync(
      resultIdentity
    );

    if (result) {
      const deletedPDOIndex = this.learningOpportunitiesResponse.items.findIndex(
        (x) => resultIdentity === x.identityActionItemDTO
      );
      this.learningOpportunitiesResponse.items.splice(deletedPDOIndex, 1);
      // TODO: Disable for now OP-6736
      // this.changePlanStatus(IdpStatusCodeEnum.Started);
    } else {
      this.toastrService.warning('Something went wrong, please try again!');
    }

    this.globalLoader.hideLoader();
  }

  private getPlans(): void {
    this.globalLoader.showLoader();

    const filterParams = new ODPFilterParams({
      userIds: [this.user.identity.id],
    });
    this.idpService.getPlansResult(filterParams).subscribe(
      (plans) => {
        this.startNewPlanIfNeeded(plans);
        if (isEmpty(plans)) {
          this.pdplans = [];
          this.globalLoader.hideLoader();

          return;
        }
        plans.sort(
          (plan1, plan2) => +new Date(plan1.created) - +new Date(plan2.created)
        );
        this.pdplans = plans;
        const currentPlan = this.getCurrentPlan(plans);
        this.setCurrentPlan(currentPlan);
        setTimeout(() => {
          this.globalLoader.hideLoader();
        }, Constant.DELAY_RENDER_DOM);
      },
      () => {
        this.globalLoader.hideLoader();
      }
    );
  }

  private startNewPlanIfNeeded = (plans: IdpDto[]) => {
    if (isEmpty(plans)) {
      return this.startNewPlan();
    }
    const currentYearPDPlan = plans.find(ResultHelper.checkIsCurrentYearResult);
    if (!currentYearPDPlan) {
      this.startNewPlan();
    }
  };

  private isFuturePlan = (plan: IdpDto): boolean => {
    if (!plan || !plan.surveyInfo) {
      return false;
    }

    const planStartDateISOString = plan.surveyInfo.startDate;
    const planStartDateYear = new Date(planStartDateISOString).getFullYear();
    const currentYear = new Date().getFullYear();

    return planStartDateYear > currentYear;
  };

  /**
   * Gets the current plan.
   * It will be the plan of this year; otherwise, it will be the first item in the list.
   * @param plans The list of PD Plans of the user
   */
  private getCurrentPlan(plans: IdpDto[]): IdpDto {
    let currentPlan = plans.find(ResultHelper.checkIsCurrentYearResult);
    if (!currentPlan) {
      currentPlan = plans[0];
    }

    return currentPlan;
  }

  // TODO: Disable for now OP-6736
  // private getUserGroupTags(): string[] {
  //   const serviceSchemes = [...this.user.personnelGroups.map(pg => pg.name)];
  //   const developmentalRoles = [...this.user.developmentalRoles.map(dr => dr.name)];

  //   return serviceSchemes.concat(developmentalRoles);
  // }

  private changePlanStatus(statusCode: IdpStatusCodeEnum): void {
    if (
      statusCode === this.currentPlan.assessmentStatusInfo.assessmentStatusCode
    ) {
      return;
    }
    this.globalLoader.showLoader();

    this.idpService
      .changeStatusPDPLan({
        resultIdentities: [this.currentPlan.resultIdentity],
        targetStatusType: {
          assessmentStatusCode: statusCode,
        },
      })
      .subscribe(
        (planResponse) => {
          this.globalLoader.hideLoader();
          if (!planResponse) {
            return;
          }
          const changeStatusResult = planResponse.find((result) =>
            this.pdplans.some(
              (plan) => result.identity.extId === plan.resultIdentity.extId
            )
          );
          const planIndex = this.pdplans.findIndex(
            (plan) =>
              changeStatusResult.identity.extId === plan.resultIdentity.extId
          );
          if (planIndex > -1) {
            const currentPlan = {
              ...this.pdplans[planIndex],
              assessmentStatusInfo: changeStatusResult.targetStatusType,
            };
            this.setCurrentPlan(currentPlan);
          }
        },
        () => {
          this.globalLoader.hideLoader();
        }
      );
  }

  // TODO: Disable for now OP-6736
  // private afterPDPlanStatusChanged(
  //   pdplan: IdpDto,
  //   learningOpportunitiesResponse.items: PDOpportunityModel[]
  // ): void {
  //   if (
  //     pdplan.assessmentStatusInfo.assessmentStatusCode ===
  //     IdpStatusCodeEnum.Approved
  //   ) {
  //     this.updateStatusForDraftLearningOpportunities(learningOpportunitiesResponse.items);
  //   }
  // }

  // TODO: Disable for now OP-6736
  // private updateStatusForDraftLearningOpportunities(
  //   learningOpportunitiesResponse.items: PDOpportunityModel[]
  // ): void {
  //   // Call API to update status of all draft PDOs to Pending approval.
  //   const draftLearningOpportunities = learningOpportunitiesResponse.items.filter(
  //     p =>
  //       p.assessmentStatusInfo.assessmentStatusCode ===
  //       IdpStatusCodeEnum.Started
  //   );

  //   if (!draftLearningOpportunities || draftLearningOpportunities.length === 0) {
  //     return;
  //   }

  //   const changeMassPdPlanStatusType = this.buildChangeMassPdPlanStatusType(
  //     draftLearningOpportunities
  //   );

  //   this.idpService
  //     .changeStatusActionItems(changeMassPdPlanStatusType)
  //     .subscribe(
  //       (changeMassPdPlanStatusTypeResults: ChangePdPlanStatusTypeResult[]) => {
  //         this.handleAfterMassStatusChange(
  //           draftLearningOpportunities,
  //           changeMassPdPlanStatusTypeResults
  //         );
  //       },
  //       error => {
  //         console.error(error);
  //         this.toastrService.error(`Failed to change status for PDOs.'`);
  //       }
  //     );
  // }

  // TODO: Disable for now OP-6736
  // private handleAfterMassStatusChange(
  //   draftLearningOpportunities: PDOpportunityModel[],
  //   changeMassPdPlanStatusTypeResults: ChangePdPlanStatusTypeResult[]
  // ): void {
  //   draftLearningOpportunities.forEach(draftLearningOpportunity => {
  //     const changeMassPdPlanStatusTypeResult = changeMassPdPlanStatusTypeResults.find(
  //       p =>
  //         p.identity.extId ===
  //         draftLearningOpportunity.identityActionItemDTO.extId
  //     );
  //     if (
  //       changeMassPdPlanStatusTypeResult.status ===
  //       HttpStatusCodeEnum.Status200OK ||
  //       // No content meaning that it has early been changed to the correct status.
  //       changeMassPdPlanStatusTypeResult.status ===
  //       HttpStatusCodeEnum.Status204NoContent
  //     ) {
  //       draftLearningOpportunity.assessmentStatusInfo =
  //         changeMassPdPlanStatusTypeResult.targetStatusType;
  //     } else {
  //       this.toastrService.error(
  //         `Change status for PDO '${changeMassPdPlanStatusTypeResult.identity.id}' has ` +
  //         `error '${changeMassPdPlanStatusTypeResult.message}'`
  //       );
  //     }
  //   });
  // }

  // TODO: Disable for now OP-6736
  // private buildChangeMassPdPlanStatusType(
  //   draftLearningOpportunities: PDOpportunityModel[]
  // ): ChangeMassPdPlanStatusType {
  //   const resultIdentitiesForChangeStatus: Identity[] = draftLearningOpportunities.map(
  //     draftLearningOpportunity => draftLearningOpportunity.identityActionItemDTO
  //   );

  //   const statusInfo: AssessmentStatusInfo = {
  //     assessmentStatusCode: IdpStatusCodeEnum.PendingForApproval
  //   };
  //   const changeMassPdPlanStatusType = new ChangeMassPdPlanStatusType({
  //     resultIdentities: resultIdentitiesForChangeStatus,
  //     targetStatusType: statusInfo
  //   });

  //   return changeMassPdPlanStatusType;
  // }

  private getPersonelGroupIds(user: Staff): string[] {
    return user && user.personnelGroups && user.personnelGroups.length
      ? user.personnelGroups.map(
          (personnelGroup) => personnelGroup.identity.extId
        )
      : [];
  }

  private async showExternalPDOForm(): Promise<void> {
    this.globalLoader.showLoader();
    const personnelGroupsIds = this.getPersonelGroupIds(this.user);
    const modalRef = await this.externalPDOService.showExternalPDOFormAsync(
      personnelGroupsIds,
      undefined,
      true
    );
    if (!modalRef) {
      this.globalLoader.hideLoader();

      return;
    }

    const modalRefComponent = modalRef.componentInstance as CxFormModal;
    modalRefComponent.changeValue.subscribe(() => {});

    modalRefComponent.submitting.subscribe((event) => {
      // tslint:disable-next-line: no-unsafe-any
      const pdoDTO = PDPlannerHelpers.externalToPDOpportunityDTO(
        event.survey.data
      );
      this.onAddExternalPDO(pdoDTO);
      modalRef.close();
    });

    this.globalLoader.hideLoader();
  }

  private async onAddExternalPDO(
    externalPDODTO: PDOpportunityDTO
  ): Promise<void> {
    const timestamp = this.currentPlan.timestamp;
    const userIdentity = this.user.identity;
    PDPlannerHelpers.updateTagForExternalPDO(externalPDODTO);
    const actionItemResp = await this.pdplannerService.addExternalPDOToPlanAsync(
      externalPDODTO,
      timestamp,
      userIdentity
    );

    if (actionItemResp) {
      this.updatePlanWhenAddPDOSuccess(actionItemResp);
    } else {
      this.toastrService.warning('Some thing went wrong.');
    }
  }

  private setCurrentPlan(plan: IdpDto): void {
    this.currentPlan = plan;
    this.checkRight();
  }

  private checkRight(): void {
    this.validToManagedPDO = this.checkValidManagedPDO();
    this.validToAddPDO = this.checkValidToAddPDO();
    this.validToSubmitPDPlan = this.checkValidToSubmitPDPlan();
  }

  private checkValidManagedPDO(): boolean {
    const isPendingPDPlan =
      this.currentStatus === IdpStatusCodeEnum.PendingForApproval;
    const isCurrentYearPlan = ResultHelper.checkIsCurrentYearResult(
      this.currentPlan
    );
    const isLearnerMode = this.mode === IDPMode.Learner;
    const isFuturePlan = this.isFuturePlan(this.currentPlan);

    return (
      !isPendingPDPlan && isLearnerMode && (isCurrentYearPlan || isFuturePlan)
    );
  }

  private checkValidToAddPDO(): boolean {
    const isPendingPDPlan =
      this.currentStatus === IdpStatusCodeEnum.PendingForApproval;
    const isCurrentYearPlan = ResultHelper.checkIsCurrentYearResult(
      this.currentPlan
    );
    const isFuturePlan = this.isFuturePlan(this.currentPlan);
    const learnerMode = this.mode === IDPMode.Learner;

    return (
      !isPendingPDPlan && learnerMode && (isCurrentYearPlan || isFuturePlan)
    );
  }

  private checkValidToSubmitPDPlan(): boolean {
    const hasStatusAbleToSubmit =
      this.currentStatus === IdpStatusCodeEnum.Started ||
      this.currentStatus === IdpStatusCodeEnum.NotStarted ||
      this.currentStatus === IdpStatusCodeEnum.Rejected;

    const isCurrentYearPlan = ResultHelper.checkIsCurrentYearResult(
      this.currentPlan
    );
    const planNotEmpty = !isEmpty(this.learningOpportunitiesResponse.items);

    return hasStatusAbleToSubmit && planNotEmpty && isCurrentYearPlan;
  }

  private submitEditedExternalPDO = async (
    externalPDODto: PDOpportunityDTO
  ): Promise<PDOpportunityModel> => {
    if (!externalPDODto?.uri) {
      return;
    }

    this.globalLoader.showLoader();
    const targetExternalPDOUri = externalPDODto.uri;
    const targetPDOModel = this.learningOpportunitiesResponse.items.find(
      (i) =>
        i.additionalProperties.learningOpportunityUri === targetExternalPDOUri
    );

    const resultPDOModel = await this.pdplannerService.updateExternalPDOInfoAsync(
      externalPDODto,
      targetPDOModel.answerDTO,
      targetPDOModel.identityActionItemDTO.id
    );

    this.globalLoader.hideLoader();

    if (!resultPDOModel) {
      console.error('Cannot update external PDO');

      return;
    }

    this.onPDOUpdated(resultPDOModel);

    return resultPDOModel;
  };

  private isOwnPDO(pdoModel: PDOpportunityModel): boolean {
    if (
      !this.user ||
      !this.user.identity ||
      !pdoModel ||
      !pdoModel.createdBy ||
      !pdoModel.createdBy.identity
    ) {
      return false;
    }

    return pdoModel.createdBy.identity.id === this.user.identity.id;
  }

  private checkAllowEditExternalPDO(pdoModel: PDOpportunityModel): boolean {
    const isExternalPDO = PDPlannerHelpers.isExternalPDO(pdoModel);
    const isOwnedPDO = this.isOwnPDO(pdoModel);

    return isExternalPDO && isOwnedPDO;
  }
}
