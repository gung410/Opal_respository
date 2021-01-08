import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';
import {
  CxGlobalLoaderService,
  CxSurveyjsEventModel,
} from '@conexus/cx-angular-common';
import { NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import {
  IKeyLearningProgrammePermission,
  KeyLearningProgrammePermission,
} from 'app-models/common/permission/key-learning-programme-permission';
import {
  PDOpportunityAnswerDTO,
  PDOpportunityDTO,
  PDOSource,
} from 'app-models/mpj/pdo-action-item.model';
import { PDPlanDto } from 'app-models/pdplan.model';
import { ExternalPDOService } from 'app-services/idp/pd-catalogue/external-pdo.service';
import { PDPlannerHelpers } from 'app-services/idp/pd-planner/pd-planner-helpers';
import { PDOpportunityDetailModel } from 'app-services/pd-opportunity/pd-opportunity-detail.model';
import { ObjectUtilities } from 'app-utilities/object-utils';
import { OdpActivity } from 'app/organisational-development/learning-plan-detail/odp.constant';
import { OdpService } from 'app/organisational-development/odp.service';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'planned-pdo-content',
  templateUrl: './planned-pdo-content.component.html',
  styleUrls: ['./planned-pdo-content.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlannedPDOContentComponent
  extends BaseScreenComponent
  implements OnChanges, IKeyLearningProgrammePermission {
  @Input() pdoDetail: PDOpportunityDetailModel;
  @Input() pdoAnswer: PDOpportunityAnswerDTO;
  @Input() klpDto: PDPlanDto;
  @Output()
  updateExternalPDO: EventEmitter<PDOpportunityAnswerDTO> = new EventEmitter<PDOpportunityAnswerDTO>();
  pdoDTO: PDOpportunityDTO;

  //Permission
  keyLearningProgrammePermission: KeyLearningProgrammePermission;

  private isExternalPDOFormDirty: boolean = false;

  constructor(
    protected authService: AuthService,
    protected changeDetectorRef: ChangeDetectorRef,
    private externalPDOService: ExternalPDOService,
    private globalLoader: CxGlobalLoaderService,
    private odpService: OdpService,
    private toastrService: ToastrService
  ) {
    super(changeDetectorRef, authService);
  }

  initKeyLearningProgrammePermission(loginUser: User): void {
    this.keyLearningProgrammePermission = new KeyLearningProgrammePermission(
      loginUser
    );
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.onInitData();
    this.initKeyLearningProgrammePermission(this.currentUser);
  }

  async onClickEdit(): Promise<void> {
    if (!this.pdoDTO) {
      return;
    }
    const clonePDOData = ObjectUtilities.clone(this.pdoDTO);
    const personelGroups =
      this.klpDto.answer.targetAudienceBy === 'userProfileParameters'
        ? this.klpDto.answer.personnelGroups
        : [];
    this.openExternalPDOModal(personelGroups, clonePDOData);
  }

  private onInitData(): void {
    if (!this.pdoAnswer) {
      return;
    }

    this.pdoDTO = this.pdoAnswer.learningOpportunity;
  }

  private async openExternalPDOModal(
    personnelGroupIds: string[] = [],
    pdoData: PDOpportunityDTO
  ): Promise<void> {
    this.globalLoader.showLoader();
    const modalRef = await this.externalPDOService.showExternalPDOFormAsync(
      personnelGroupIds,
      pdoData
    );

    this.globalLoader.hideLoader();

    if (!modalRef) {
      return;
    }

    const modalRefComponent = modalRef.componentInstance;
    modalRefComponent.changeValue.subscribe(() => {
      this.isExternalPDOFormDirty = true;
    });
    modalRefComponent.submitting.subscribe((event: CxSurveyjsEventModel) => {
      this.onSubmitFormEditExternalPDO(event, modalRef);
    });
  }

  private async onSubmitFormEditExternalPDO(
    event: CxSurveyjsEventModel,
    modalRef: NgbModalRef
  ): Promise<void> {
    this.globalLoader.showLoader();

    // Prevent form clear data
    event.options.allowComplete = false;

    const result = await this.handleSubmitFormEditExternalPDO(event);
    result
      ? this.toastrService.success('Update PD Opportunity successfully!')
      : this.toastrService.error('Fail to update PD Opportunity!');
    modalRef.close();
    this.globalLoader.hideLoader();
  }

  private async handleSubmitFormEditExternalPDO(
    event: CxSurveyjsEventModel
  ): Promise<boolean> {
    if (!this.isExternalPDOFormDirty || !event || !event.survey) {
      return false;
    }

    const oldPdoDto = this.pdoAnswer.learningOpportunity;
    const newAnswer = ObjectUtilities.clone(this.pdoAnswer);
    const pdoDTO = PDPlannerHelpers.externalToPDOpportunityDTO(
      event.survey.data,
      oldPdoDto.uri
    );

    if (!pdoDTO) {
      return false;
    }

    newAnswer.learningOpportunity = pdoDTO;

    // Call endpoint update external PDO in list planned PDO on KLP
    const result = await this.updateExternalPDOOnKLP(newAnswer);

    // Check result
    if (!result) {
      return false;
    }

    this.updateExternalPDO.emit(newAnswer);

    return true;
  }

  private async updateExternalPDOOnKLP(
    externalPDOAnswer: PDOpportunityAnswerDTO
  ): Promise<boolean> {
    if (!this.klpDto || !this.klpDto.answer) {
      return;
    }

    const updatedPDPlan = ObjectUtilities.clone(this.klpDto);
    const listAddedPDOOnKLP = updatedPDPlan.answer
      .listLearningOpportunity as PDOpportunityAnswerDTO[];
    const findIdex = listAddedPDOOnKLP.findIndex(
      (pdo) =>
        pdo.learningOpportunity.uri ===
        externalPDOAnswer.learningOpportunity.uri
    );

    if (findIdex > 0) {
      listAddedPDOOnKLP[findIdex] = externalPDOAnswer;
    }

    const updatedKLP = await this.odpService
      .savePlan(updatedPDPlan, OdpActivity.Programme)
      .toPromise()
      .catch((e) => console.error(e));

    return !!updatedKLP;
  }

  get isExternalPDO(): boolean {
    return this.pdoAnswer.learningOpportunity.source === PDOSource.CustomPDO;
  }
}
