import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';
import { CxGlobalLoaderService } from '@conexus/cx-angular-common';
import { PDOpportunityAnswerDTO } from 'app-models/mpj/pdo-action-item.model';
import { PDPlanDto } from 'app-models/pdplan.model';
import { PDPlannerHelpers } from 'app-services/idp/pd-planner/pd-planner-helpers';
import { KeyLearningProgramService } from 'app-services/odp/learning-plan-services/key-learning-program.service';

@Component({
  selector: 'planned-list-pdo',
  templateUrl: './planned-list-pdo.component.html',
  styleUrls: ['./planned-list-pdo.component.scss'],
})
export class PlannedListPDOComponent implements OnChanges {
  @Input() klpResultDto: PDPlanDto;
  @Input() isEditing: boolean;
  @Input() allowAddingPDOPermission: boolean = true;
  @Input() allowDeletingPDOPermission: boolean = true;
  @Output() clickedAddPDO: EventEmitter<void> = new EventEmitter<void>();
  @Output()
  clickedPDO: EventEmitter<PDOpportunityAnswerDTO> = new EventEmitter<PDOpportunityAnswerDTO>();
  @Output() clickedRemovePDO: EventEmitter<string> = new EventEmitter<string>();

  klpAddedPDOs: PDOpportunityAnswerDTO[];

  constructor(
    private globalLoader: CxGlobalLoaderService,
    private klpSerivce: KeyLearningProgramService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (this.klpResultDto) {
      this.getAddedPDODataForKLP();
    }
  }

  onClickedAddPDO(): void {
    this.clickedAddPDO.emit();
  }

  onClickedPDO(pdo: PDOpportunityAnswerDTO): void {
    this.clickedPDO.emit(pdo);
  }

  onClickedRemovePDO(pdoURI: string): void {
    this.clickedRemovePDO.emit(pdoURI);
  }

  checkAllowRemove(pdoAnswer: PDOpportunityAnswerDTO): boolean {
    return (
      this.isExternalPDO(pdoAnswer) &&
      this.isEditing &&
      this.allowDeletingPDOPermission
    );
  }

  private isExternalPDO(pdoAnswer: PDOpportunityAnswerDTO): boolean {
    return PDPlannerHelpers.isExternalPDOByAnswer(pdoAnswer);
  }

  private async getAddedPDODataForKLP(): Promise<void> {
    // Temporary hide
    // this.globalLoader.showLoader();

    this.klpAddedPDOs = await this.klpSerivce.getAddedPDOListOfKLP(
      this.klpResultDto.answer
    );

    // Temporary hide
    // this.globalLoader.hideLoader();
    this.changeDetectorRef.detectChanges();
  }
}
