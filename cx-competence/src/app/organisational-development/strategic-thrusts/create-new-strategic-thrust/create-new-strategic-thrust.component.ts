import { Component, Output, EventEmitter } from '@angular/core';
import { CxSurveyjsEventModel } from '@conexus/cx-angular-common';

@Component({
  templateUrl: './create-new-strategic-thrust.component.html',
  styleUrls: ['./create-new-strategic-thrust.component.scss'],
})
export class CreateNewStrategicThrustComponent {
  formConfigJson: any;
  btnSubmitName: string;
  dialogHeaderText: string;
  @Output() createOrEditST: EventEmitter<any> = new EventEmitter<any>();
  @Output() cancel: EventEmitter<any> = new EventEmitter<any>();
  @Output() dataAnswer: any = null;
  constructor() {}

  onCreateOrEditST(event: CxSurveyjsEventModel): void {
    this.createOrEditST.emit(event.survey.data);
  }

  onCancel(): void {
    this.cancel.emit();
  }

  onDataFormChanged(data: CxSurveyjsEventModel): void {
    if (
      data.options.question.name === 'endYear' ||
      data.options.question.name === 'startYear'
    ) {
      const endYear: any = data.survey.getQuestionByName('endYear');
      const startYear: any = data.survey.getQuestionByName('startYear');
      if (startYear.value !== undefined) {
        startYear.hasErrors(true);
      }
      if (endYear.value !== undefined) {
        endYear.hasErrors(true);
      }

      return;
    }
    data.options.question.hasErrors(true);
  }
}
