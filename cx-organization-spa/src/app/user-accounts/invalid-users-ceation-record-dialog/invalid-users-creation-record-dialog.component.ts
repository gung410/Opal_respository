import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CxDialogTemplateComponent } from '@conexus/cx-angular-common';
import { TranslateService } from '@ngx-translate/core';
import { InvalidMassUsersCreationDto } from '../models/mass-users-creation-exception.model';

@Component({
  selector: 'invalid-users-creation-record-dialog',
  templateUrl: './invalid-users-creation-record-dialog.component.html',
  styleUrls: ['./invalid-users-creation-record-dialog.component.scss']
})
export class InvalidUsersCreationRecordDialogComponent extends CxDialogTemplateComponent {
  @Input() invalidUsersCreationResults: InvalidMassUsersCreationDto[];

  @Output() cancel: EventEmitter<any> = new EventEmitter();
  cancelButtonText: string = this.translateService.instant(
    'Common.Button.Close'
  ) as string;

  constructor(private translateService: TranslateService) {
    super();
  }

  ngOnInit(): void {}

  onCancel(): void {
    this.cancel.emit();
  }
}
