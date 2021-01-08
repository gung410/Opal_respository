import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input, ViewEncapsulation } from '@angular/core';

import { QuestionBankMode } from './question-bank-list.component';
import { QuestionGroup } from '@opal20/domain-api';
import { QuestionGroupDisplayMode } from './question-bank-group.component';

@Component({
  selector: 'question-bank-repository-page',
  templateUrl: './question-bank-repository-page.component.html',
  encapsulation: ViewEncapsulation.None
})
export class QuestionBankRepositoryPageComponent extends BasePageComponent {
  @Input() public mode: QuestionBankMode = QuestionBankMode.View;
  public questionBankMode: typeof QuestionBankMode = QuestionBankMode;
  public textSearch: string = '';
  public submitSearch: string = '';
  public questionGroupIds: string[] = [];
  public questionGroupDisplayMode = QuestionGroupDisplayMode;

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onSubmitSearch(): void {
    this.submitSearch = this.textSearch.slice();
  }

  public onQuestionGroupChange(questionGroup: QuestionGroup[]): void {
    this.questionGroupIds = questionGroup.map(group => group.id);
  }
}
