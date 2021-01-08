import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import {
  CxSurveyjsEventModel,
  CxSurveyjsVariable
} from '@conexus/cx-angular-common';
import { exportConfirmationFormJSON } from './export-confirmation-form';

@Component({
  selector: 'export-confirmation',
  templateUrl: './export-confirmation.component.html',
  styleUrls: ['./export-confirmation.component.scss']
})
export class ExportConfirmationComponent implements OnInit {
  surveyFormJSON: any = exportConfirmationFormJSON;
  surveyVariables: CxSurveyjsVariable[] = [];
  /** Determines whether the export would take long time to complete or quick. */
  isLongProcess: boolean;
  /** The output value is true if the report should be sent via email; Otherwise, it should be rendered directly as soon as finished. */
  @Output() complete: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() cancel: EventEmitter<void> = new EventEmitter<void>();

  ngOnInit(): void {
    this.surveyVariables.push(
      new CxSurveyjsVariable({
        name: 'isLongProcess',
        value: this.isLongProcess
      })
    );
  }

  onConfirmClick(event: CxSurveyjsEventModel): void {
    event.options.allowComplete = false;
    const submittingData = event.survey.data;
    const sendEmail =
      this.isLongProcess || submittingData.downloadDirectly === 'no';
    this.complete.emit(sendEmail);
  }

  onCancel(): void {
    this.cancel.emit();
  }
}
