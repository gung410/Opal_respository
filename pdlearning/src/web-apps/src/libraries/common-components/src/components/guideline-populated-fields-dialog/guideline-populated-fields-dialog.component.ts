import { BaseFormComponent, LocalTranslatorService, ModuleFacadeService } from '@opal20/infrastructure';

import { Component } from '@angular/core';
import { DialogRef } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'guideline-populated-fields-dialog',
  templateUrl: './guideline-populated-fields-dialog.component.html'
})
export class GuidelinePopulatedFieldsDialogComponent extends BaseFormComponent {
  public guideLineItem = [
    {
      shortCode: '[name]',
      description: this.translateCommon('The name of the participant completing the form.')
    },
    {
      shortCode: '[email]',
      description: this.translateCommon('(OPAL2.0 log in email): The email of the participant completing the form.')
    },
    {
      shortCode: '[placeofwork]',
      description: this.translateCommon('The place of work of the participant completing the form.')
    },
    {
      shortCode: '[designation]',
      description: this.translateCommon('The designation of the participant completing the form.')
    },
    {
      shortCode: '[teachinglevel]',
      description: this.translateCommon('The teaching level of the participant completing the form.')
    },
    {
      shortCode: '[teachingsubject_or_jobfamily]',
      description: this.translateCommon('The Teaching subject / Job family of the participant completing the form.')
    }
  ];

  constructor(protected moduleFacadeService: ModuleFacadeService, private dialogRef: DialogRef, public translator: LocalTranslatorService) {
    super(moduleFacadeService);
  }

  public closeDialog(): void {
    this.dialogRef.close();
  }
}
