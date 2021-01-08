import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import {
  CxGlobalLoaderService,
  CxSurveyjsVariable,
} from '@conexus/cx-angular-common';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import {
  IKeyLearningProgrammePermission,
  KeyLearningProgrammePermission,
} from 'app-models/common/permission/key-learning-programme-permission';
import {
  SurveyFormJSON,
  SurveySubmitEventData,
} from 'app-models/common/surveyjs.model';
import {
  ExternalPDOExtensions,
  PDOpportunityAnswerDTO,
  PDOpportunityDTO,
  PDOSource,
  TranningCost,
} from 'app-models/mpj/pdo-action-item.model';
import { ResultHelper } from 'app-services/idp/result-helpers';
import { KeyLearningProgramService } from 'app-services/odp/learning-plan-services/key-learning-program.service';
import { PDOpportunityDetailModel } from 'app-services/pd-opportunity/pd-opportunity-detail.model';
import { PDOpportunityService } from 'app-services/pd-opportunity/pd-opportunity.service';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import { SurveyjsMode } from 'app/shared/constants/surveyjs-mode.constant';
import { clone } from 'lodash';
import { ToastrService } from 'ngx-toastr';
import { CACULATE_COST_FORM } from './planned-pdo-cost.form';
import { CostFormDataModel } from './planned-pdo-cost.model';
@Component({
  selector: 'planned-pdo-cost',
  templateUrl: './planned-pdo-cost.component.html',
  styleUrls: ['./planned-pdo-cost.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlannedPDOCostComponent
  implements OnInit, IKeyLearningProgrammePermission {
  @Input() pdoAnswer: PDOpportunityAnswerDTO;
  @Input() klpDto: IdpDto;
  @Input() pdoDetail: PDOpportunityDetailModel;
  @Input() allowManagePDO: boolean = false;
  @Output()
  onUpdateExternalPDOPlannedCost: EventEmitter<PDOpportunityAnswerDTO> = new EventEmitter();

  formJson: SurveyFormJSON;
  formData: CostFormDataModel = {};
  pdoDTO: PDOpportunityDTO;
  formVariables: CxSurveyjsVariable[] = [];

  // Permission
  currentUser: User;
  keyLearningProgrammePermission: KeyLearningProgrammePermission;

  private departmentId: number;

  constructor(
    protected authService: AuthService,
    private pdOpportunityService: PDOpportunityService,
    private klpService: KeyLearningProgramService,
    private changeDetectorRef: ChangeDetectorRef,
    private globalLoader: CxGlobalLoaderService,
    private toastrService: ToastrService
  ) {}

  initKeyLearningProgrammePermission(loginUser: User): void {
    this.keyLearningProgrammePermission = new KeyLearningProgrammePermission(
      loginUser
    );
  }

  ngOnInit(): void {
    this.currentUser = this.authService.userData().getValue();
    this.initKeyLearningProgrammePermission(this.currentUser);
    this.initData();
  }

  onSubmitting(eventData: SurveySubmitEventData): void {
    if (!eventData.options.allowComplete) {
      return;
    }
    eventData.options.allowComplete = false;
    const trainingCostResult: CostFormDataModel = eventData.survey.data;
    this.formData = trainingCostResult;
    const plannedTrainingCost: TranningCost = this.generateTraningCostObject(
      trainingCostResult
    );
    this.submitTranningCost(plannedTrainingCost);
  }

  onCancel(): void {}

  onClickEdit(): void {
    this.changeMode(SurveyjsMode.edit);
  }

  onClickCancel(): void {
    this.changeMode(SurveyjsMode.display);
  }

  get isEditing(): boolean {
    return this.formJson && this.formJson.mode === SurveyjsMode.edit;
  }

  get isExternalPDO(): boolean {
    return this.pdoDTO.source === PDOSource.CustomPDO;
  }

  get isCoursePadPDO(): boolean {
    return this.pdoDTO.source === PDOSource.CoursePadPDO;
  }

  private changeMode(modeParam: string): void {
    this.formJson = {
      ...this.formJson,
      mode: modeParam,
    };
  }

  private async initData(): Promise<void> {
    if (!this.pdoAnswer || !this.pdoDetail) {
      return;
    }

    this.globalLoader.showLoader();
    this.pdoDTO = this.pdoAnswer.learningOpportunity;
    this.departmentId = ResultHelper.getObjectiveId(this.klpDto);

    // Init survey form
    this.formJson = clone(CACULATE_COST_FORM);

    const isExternalPDO = new CxSurveyjsVariable({
      name: 'isExternalPDO',
      value: this.isExternalPDO,
    });
    this.formVariables.push(isExternalPDO);
    const isCoursePadPDO = new CxSurveyjsVariable({
      name: 'isCoursePadPDO',
      value: this.isCoursePadPDO,
    });
    this.formVariables.push(isCoursePadPDO);

    this.getPlannedFormData();

    if (this.isCoursePadPDO) {
      await this.getActualFormData();
    }

    this.globalLoader.hideLoader();
    this.changeDetectorRef.detectChanges();
  }

  private getPlannedFormData(): void {
    const formData: CostFormDataModel = {};

    // Init data for survey form
    const savedTranningCost = this.pdoAnswer.tranningCost;
    formData.numOfOfficers_Planned = savedTranningCost
      ? savedTranningCost.numOfOfficersPlanned
      : 0;

    if (this.isExternalPDO) {
      const externalPDOExtensions: ExternalPDOExtensions = this.pdoDTO
        .extensions;
      formData.numOfHours_Planned = savedTranningCost
        ? savedTranningCost.numOfHoursPlanned
        : externalPDOExtensions.duration || 0;
      formData.costPerPax_Planned = savedTranningCost
        ? savedTranningCost.costPerPaxPlanned
        : externalPDOExtensions.cost || 0;
    }

    if (this.pdoDetail && this.isCoursePadPDO) {
      formData.numOfHours_Planned = this.pdoDetail.duration;
      formData.costPerPax_Planned = this.pdoDetail.costForMOELearner;
    }

    this.formData = formData;
  }

  private async getActualFormData(): Promise<void> {
    let noRegistrationCompleted: number = 0;

    if (this.klpDto && this.klpDto.surveyInfo && this.departmentId > 0) {
      const courseId = this.pdoDetail.id;
      const planStartDate = this.klpDto.surveyInfo.startDate;
      const planEndDate = this.klpDto.surveyInfo.endDate;
      noRegistrationCompleted = await this.pdOpportunityService.getNoRegistrationCompleted(
        courseId,
        this.departmentId,
        planStartDate,
        planEndDate
      );
    }

    this.formData = {
      ...this.formData,
      numOfHours_Actual: this.pdoDetail.duration || 0,
      costPerPax_Actual: this.pdoDetail.costForMOELearner || 0,
      numOfOfficers_Actual: noRegistrationCompleted || 0,
    };
  }

  private generateTraningCostObject(result: CostFormDataModel): TranningCost {
    const tranningCost: TranningCost = {};

    tranningCost.numOfOfficersPlanned =
      result.numOfOfficers_Planned || undefined;
    if (this.pdoDTO.source === PDOSource.CustomPDO) {
      tranningCost.numOfHoursPlanned = result.numOfHours_Planned;
      tranningCost.costPerPaxPlanned = result.costPerPax_Planned;
    }

    return tranningCost;
  }

  private async submitTranningCost(
    plannedTrainingCost: TranningCost
  ): Promise<void> {
    if (!plannedTrainingCost || !this.klpDto || !this.pdoAnswer) {
      return;
    }
    this.globalLoader.showLoader();
    const courseUri = this.pdoAnswer.learningOpportunity.uri;
    const newKlpDto = clone(this.klpDto);
    const newPDOAnswer = this.findCurrentPDOAnswer(courseUri, newKlpDto);
    newPDOAnswer.tranningCost = plannedTrainingCost;

    const result = await this.klpService.updateKeyLearningProgramContent(
      newKlpDto
    );
    if (result) {
      if (this.isExternalPDO) {
        newPDOAnswer.learningOpportunity.extensions['cost'] =
          plannedTrainingCost.costPerPaxPlanned;
        newPDOAnswer.learningOpportunity.extensions.duration =
          plannedTrainingCost.numOfHoursPlanned;
        this.onUpdateExternalPDOPlannedCost.emit(newPDOAnswer);
      }
      this.pdoAnswer.tranningCost = plannedTrainingCost;
      this.toastrService.success('Update training cost success');
      this.changeMode(SurveyjsMode.display);
    } else {
      this.toastrService.error('Update training cost error');
    }
    this.globalLoader.hideLoader();
    this.changeDetectorRef.detectChanges();
  }

  private findCurrentPDOAnswer(
    courseUri: string,
    klpDto: IdpDto
  ): PDOpportunityAnswerDTO {
    if (
      !courseUri ||
      !klpDto ||
      !klpDto.answer ||
      !klpDto.answer.listLearningOpportunity
    ) {
      return;
    }

    const plannedPDOs: PDOpportunityAnswerDTO[] =
      klpDto.answer.listLearningOpportunity;
    const foundPDO = plannedPDOs.find((plannedPDO) => {
      const plannedPDOUri = plannedPDO.learningOpportunity.uri;

      return plannedPDOUri === courseUri;
    });

    return foundPDO;
  }
}
