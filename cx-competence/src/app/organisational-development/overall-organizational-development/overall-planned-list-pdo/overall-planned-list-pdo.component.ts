import {
  ChangeDetectorRef,
  Component,
  Input,
  OnChanges,
  SimpleChanges,
} from '@angular/core';
import { PDOpportunityAnswerDTO } from 'app-models/mpj/pdo-action-item.model';
import { PDPlanDto } from 'app-models/pdplan.model';
import { KeyLearningProgramService } from 'app-services/odp/learning-plan-services/key-learning-program.service';

@Component({
  selector: 'overall-planned-list-pdo',
  templateUrl: './overall-planned-list-pdo.component.html',
})
export class OverallPlannedListPDOComponent implements OnChanges {
  @Input() klpResultDto: PDPlanDto;

  klpAddedPDOs: PDOpportunityAnswerDTO[];

  constructor(
    private klpService: KeyLearningProgramService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (this.klpResultDto) {
      this.getAddedPDODataForKLP();
    }
  }

  private async getAddedPDODataForKLP(): Promise<void> {
    this.klpAddedPDOs = await this.klpService.getAddedPDOListOfKLP(
      this.klpResultDto.answer
    );

    this.changeDetectorRef.detectChanges();
  }
}
