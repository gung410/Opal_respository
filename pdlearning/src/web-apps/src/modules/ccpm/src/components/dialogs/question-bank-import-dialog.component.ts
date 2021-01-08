import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { QuestionBankViewModel, QuestionGroup, QuestionType } from '@opal20/domain-api';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { QuestionBankMode } from './../question-bank-list.component';
import { QuestionGroupDisplayMode } from '../question-bank-group.component';

@Component({
  selector: 'question-bank-import-dialog',
  templateUrl: './question-bank-import-dialog.component.html'
})
export class QuestionBankImportDialogComponent extends BasePageComponent {
  @Input() public questionType: QuestionType[] = [];
  public mode: QuestionBankMode = QuestionBankMode.Import;
  public questionBankImport: QuestionBankViewModel[] = [];
  public textSearch: string = '';
  public submitSearch: string = '';
  public questionGroupIds: string[] = [];
  public questionGroupDisplayMode = QuestionGroupDisplayMode;

  constructor(public moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef) {
    super(moduleFacadeService);
  }

  public onCancel(): void {
    this.dialogRef.close();
  }

  public async onSubmit(): Promise<void> {
    if (this.questionBankImport && this.questionBankImport.length) {
      this.dialogRef.close(this.questionBankImport);
    } else {
      this.moduleFacadeService.modalService.showWarningMessage('Please select at least question.', null, null);
    }
  }

  public onQuestionGroupChange(questionGroup: QuestionGroup[]): void {
    this.questionGroupIds = questionGroup.map(data => data.id);
  }

  public onSubmitSearch(): void {
    this.submitSearch = this.textSearch.slice();
  }
}
