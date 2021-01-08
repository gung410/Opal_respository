import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';
import { KLPPlannedAreaModel } from 'app-models/opj/klp-planned-areas.model';
import { PDPlanDto } from 'app-models/pdplan.model';
import { isEmpty } from 'lodash';

@Component({
  selector: 'planned-list-area',
  templateUrl: './planned-list-area.component.html',
  styleUrls: ['./planned-list-area.component.scss'],
})
export class PlannedListAreaComponent implements OnChanges {
  @Input() klpResultDto: PDPlanDto;
  @Input() isEditing: KLPPlannedAreaModel[];
  @Input() allowAddingAreaPermission: boolean = true;
  @Input() allowDeletingAreaPermission: boolean = true;

  @Output() clickedRemove: EventEmitter<string> = new EventEmitter<string>();
  @Output() clickedAddAreas: EventEmitter<void> = new EventEmitter<void>();

  klpAddedAreas: KLPPlannedAreaModel[];

  constructor() {}

  ngOnChanges(changes: SimpleChanges): void {
    if (this.klpResultDto) {
      this.getAddedAreasForKLP();
    }
  }

  onClickAddAreas(): void {
    this.clickedAddAreas.emit();
  }

  onClickedRemoveArea(areaId: string): void {
    this.clickedRemove.emit(areaId);
  }

  private getAddedAreasForKLP(): void {
    if (
      !this.klpResultDto ||
      !this.klpResultDto.answer ||
      isEmpty(this.klpResultDto.answer.listLearningArea)
    ) {
      this.klpAddedAreas = [];
    } else {
      this.klpAddedAreas = this.klpResultDto.answer.listLearningArea.filter(
        (areaModel: KLPPlannedAreaModel) => !!areaModel.area
      );
    }
  }
}
