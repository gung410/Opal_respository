import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {
  CxGlobalLoaderService,
  CxSurveyjsEventModel,
} from '@conexus/cx-angular-common';
import {
  PDOpportunityAnswerDTO,
  PDOpportunityDTO,
  PDOSource,
} from 'app-models/mpj/pdo-action-item.model';
import { ExternalPDOService } from 'app-services/idp/pd-catalogue/external-pdo.service';
import { PDPlannerHelpers } from 'app-services/idp/pd-planner/pd-planner-helpers';
import { PDOpportunityDetailModel } from 'app-services/pd-opportunity/pd-opportunity-detail.model';
import { ObjectUtilities } from 'app-utilities/object-utils';

@Component({
  selector: 'course-info',
  templateUrl: './course-info.component.html',
  styleUrls: ['./course-info.component.scss'],
})
export class CourseInfoComponent implements OnInit {
  @Input() pdoDetail: PDOpportunityDetailModel;
  @Input() pdoAnswer: PDOpportunityAnswerDTO;
  @Input() allowEditExternalPDO: boolean = false;
  @Input() personelGroups: string[];
  @Output()
  updatedExternalPDO: EventEmitter<PDOpportunityDTO> = new EventEmitter<PDOpportunityDTO>();

  pdoDTO: PDOpportunityDTO;
  isExternalPDOFormDirty: boolean;

  constructor(
    private externalPDOService: ExternalPDOService,
    private globalLoader: CxGlobalLoaderService
  ) {}

  ngOnInit(): void {
    this.processPDOModel();
  }

  async onClickEditExternalPDO(): Promise<void> {
    if (!this.pdoDTO) {
      return;
    }

    const clonePDOData = ObjectUtilities.clone(this.pdoDTO);
    this.openExternalPDOModal(this.personelGroups, clonePDOData);
  }

  private async openExternalPDOModal(
    personnelGroupIds: string[] = [],
    pdoData: PDOpportunityDTO
  ): Promise<void> {
    this.globalLoader.showLoader();
    const modalRef = await this.externalPDOService.showExternalPDOFormAsync(
      personnelGroupIds,
      pdoData,
      true
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
      event.options.allowComplete = false;
      if (!this.isExternalPDOFormDirty) {
        modalRef.close();

        return;
      }

      const pdoDTO = PDPlannerHelpers.externalToPDOpportunityDTO(
        event.survey?.data,
        this.pdoDTO.uri
      );
      this.isExternalPDOFormDirty = false;
      this.updatedExternalPDO.emit(pdoDTO);
      modalRef.close();
    });
  }

  private processPDOModel(): void {
    if (!this.pdoAnswer) {
      return;
    }

    this.pdoDTO = this.pdoAnswer.learningOpportunity;
  }

  get isExternalPDO(): boolean {
    return (
      this.pdoAnswer &&
      this.pdoAnswer.learningOpportunity.source === PDOSource.CustomPDO
    );
  }
}
