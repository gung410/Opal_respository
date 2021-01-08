import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CxConfirmationDialogComponent } from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { Action } from 'app-models/action.model';
import { AssessmentStatusInfo } from 'app-models/assessment.model';
import {
  PDOAddType,
  PDOpportunityAnswerDTO,
  PDOpportunityDTO,
  PDOpportunityModel,
  PDOSource,
} from 'app-models/mpj/pdo-action-item.model';
import {
  IdpStatusCodeEnum,
  IdpStatusEnum,
} from 'app/individual-development/idp.constant';
import { Constant } from 'app/shared/app.constant';
import { IDPMode } from './../../idp.constant';

@Component({
  selector: 'action-item',
  templateUrl: './action-item.component.html',
  styleUrls: ['./action-item.component.scss'],
})
export class ActionItemComponent implements OnInit {
  @Input('learningOpportunity')
  set learningOpportunity(pdoModel: PDOpportunityModel) {
    this.processPDOModel(pdoModel);
  }
  @Input() mode: IDPMode;
  @Input() isEven: boolean = false;
  @Input() allowViewComment: boolean = false;
  @Input() allowManagedPDO: boolean = false;
  @Input() allowRemove: boolean = false;
  @Input() allowMarkCompletion: boolean = false;
  @Output() clickedItem: EventEmitter<PDOpportunityModel> = new EventEmitter();
  @Output() remove: EventEmitter<PDOpportunityModel> = new EventEmitter();
  @Output()
  markAsCompleted: EventEmitter<PDOpportunityModel> = new EventEmitter();
  @Output()
  markAsUnCompleted: EventEmitter<PDOpportunityModel> = new EventEmitter();

  learningOpportunityModel: PDOpportunityModel;
  pdoAnswerDTO: PDOpportunityAnswerDTO;
  pdoDTO: PDOpportunityDTO;
  EXTERNAL_PDO_THUMBNAIL_PATH: string = Constant.EXTERNAL_PDO_THUMBNAIL_PATH;
  // flags
  isNominated: boolean = false;
  unPublished: boolean = false;

  actions: Action[];
  actionItemStatus: AssessmentStatusInfo;
  constructor(
    private ngbModal: NgbModal,
    private translateService: TranslateService
  ) {}

  ngOnInit(): void {
    this.initData();
  }

  onClickItem(): void {
    if (this.unPublished) {
      return;
    }

    this.clickedItem.emit(this.learningOpportunityModel);
  }

  onRemoveItem(): void {
    if (this.unPublished) {
      return;
    }

    const content = this.translateService.instant(
      'MyPdJourney.PlannedActivities.ConfirmDeletePDO'
    ) as string;
    const confirmedCallback = () => {
      this.remove.emit(this.learningOpportunityModel);
    };
    this.confirm(content, confirmedCallback);
  }

  toogleCompleteExternalPDO(isCompleted: boolean): void {
    if (this.unPublished) {
      return;
    }

    const content = isCompleted
      ? this.translateService.instant(
          'MyPdJourney.PlannedActivities.ConfirmMarkAsCompleted'
        )
      : this.translateService.instant(
          'MyPdJourney.PlannedActivities.ConfirmMarkAsUnCompleted'
        );
    const eventEmitter = isCompleted
      ? this.markAsCompleted
      : this.markAsUnCompleted;
    const confirmedCallback = () => {
      eventEmitter.emit(this.learningOpportunityModel);
    };
    this.confirm(content, confirmedCallback);
  }

  get isCoursePadPDO(): boolean {
    return this.pdoDTO.source === PDOSource.CoursePadPDO;
  }

  get isExternalPDO(): boolean {
    return this.pdoDTO.source === PDOSource.CustomPDO;
  }

  get isCompleted(): boolean {
    return (
      this.isExternalPDO &&
      this.learningOpportunityModel &&
      this.learningOpportunityModel.additionalProperties &&
      (this.learningOpportunityModel.additionalProperties.isCompleted ||
        this.learningOpportunityModel.additionalProperties.isCompleted ===
          undefined)
    );
  }

  get isInCompleted(): boolean {
    return (
      this.isExternalPDO &&
      this.learningOpportunityModel &&
      this.learningOpportunityModel.additionalProperties &&
      (!this.learningOpportunityModel.additionalProperties.isCompleted ||
        this.learningOpportunityModel.additionalProperties.isCompleted ===
          undefined)
    );
  }

  get isApproved(): boolean {
    const notEnableApproval = !this.learningOpportunityModel?.answerDTO
      ?.learningOpportunity?.extensions?.enableExternalPDOApproval;
    const isApprovedWhenEnableApproval =
      this.learningOpportunityModel?.answerDTO?.learningOpportunity?.extensions
        ?.enableExternalPDOApproval &&
      this.learningOpportunityModel?.assessmentStatusInfo
        ?.assessmentStatusCode === IdpStatusCodeEnum.Approved;
    return notEnableApproval || isApprovedWhenEnableApproval;
  }

  get showStatus(): boolean {
    if (!this.actionItemStatus && !this.isExternalPDO) {
      return false;
    }

    const statusCode = this.actionItemStatus.assessmentStatusCode;

    const acceptedStatusCode = [
      IdpStatusCodeEnum.Approved.toString(),
      IdpStatusCodeEnum.ExternalPendingForApproval.toString(),
      IdpStatusCodeEnum.ExternalRejected.toString(),
      IdpStatusCodeEnum.Completed.toString(),
      IdpStatusCodeEnum.InCompleted.toString(),
    ];

    return acceptedStatusCode.includes(statusCode);
  }

  getStatus(): AssessmentStatusInfo {
    if (
      this.learningOpportunityModel.additionalProperties.isCompleted !==
      undefined
    ) {
      if (this.learningOpportunityModel.additionalProperties.isCompleted) {
        return new AssessmentStatusInfo({
          assessmentStatusId: IdpStatusEnum.Completed,
          assessmentStatusCode: IdpStatusCodeEnum.Completed,
          assessmentStatusName: IdpStatusCodeEnum.Completed,
        });
      } else {
        return new AssessmentStatusInfo({
          assessmentStatusId: IdpStatusEnum.InCompleted,
          assessmentStatusCode: IdpStatusCodeEnum.InCompleted,
          assessmentStatusName: IdpStatusCodeEnum.InCompleted,
        });
      }
    } else {
      return this.learningOpportunityModel.assessmentStatusInfo;
    }
  }

  private initData(): void {
    this.actions = [
      {
        text: this.translateService.instant(
          'MyPdJourney.PlannedActivities.MarkAsCompleted'
        ) as string,
        event: () => this.toogleCompleteExternalPDO(true),
        condition: this.isInCompleted && this.isApproved,
      },
      {
        text: this.translateService.instant(
          'MyPdJourney.PlannedActivities.MarkAsUnCompleted'
        ) as string,
        event: () => this.toogleCompleteExternalPDO(false),
        condition: this.isCompleted && this.isApproved,
      },
    ];

    this.actionItemStatus = this.getStatus();
  }

  private processPDOModel(pdoModel: PDOpportunityModel): void {
    if (
      !pdoModel ||
      !pdoModel.answerDTO ||
      !pdoModel.answerDTO.learningOpportunity
    ) {
      return;
    }

    this.learningOpportunityModel = pdoModel;
    this.pdoAnswerDTO = pdoModel.answerDTO;
    this.pdoDTO = this.pdoAnswerDTO.learningOpportunity;

    if (pdoModel.additionalProperties) {
      this.isNominated =
        pdoModel.additionalProperties.type === PDOAddType.Nominated ||
        pdoModel.additionalProperties.type === PDOAddType.CAMNominated;
      this.unPublished = pdoModel.unPublished;
    }
  }

  private confirm(message: string, confirmedCallback: () => void): void {
    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      size: 'sm',
      centered: true,
    });

    const modalComponent = modalRef.componentInstance as CxConfirmationDialogComponent;
    modalComponent.cancelButtonText = this.translateService.instant(
      'Odp.ConfirmationDialog.No'
    ) as string;
    modalComponent.confirmButtonText = this.translateService.instant(
      'Odp.ConfirmationDialog.Confirm'
    ) as string;
    modalComponent.header = this.translateService.instant(
      'Odp.ConfirmationDialog.Header'
    ) as string;
    modalComponent.content = message;
    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });
    modalComponent.confirm.subscribe(() => {
      confirmedCallback.apply(this);
      modalRef.close();
    });
  }
}
