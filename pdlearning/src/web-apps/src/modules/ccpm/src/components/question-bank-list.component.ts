import { BaseGridComponent, ModuleFacadeService, NotificationType, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import {
  ModelMappingHelper,
  QUESTION_TYPE_LABEL,
  QuestionBank,
  QuestionBankApiService,
  QuestionBankViewModel,
  QuestionGroup,
  QuestionType,
  SaveQuestionBankRequest
} from '@opal20/domain-api';

import { CellClickEvent } from '@progress/kendo-angular-grid';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { OpalDialogService } from '@opal20/common-components';
import { QuestionBankListService } from '@opal20/domain-components';
import { QuestionBankPreviewDialogComponent } from './dialogs/question-bank-preview-dialog.component';
import { Subscription } from 'rxjs';

export enum QuestionBankMode {
  View = 'View',
  Import = 'Import'
}
@Component({
  selector: 'question-bank-list',
  templateUrl: './question-bank-list.component.html'
})
export class QuestionBankListComponent extends BaseGridComponent<QuestionBankViewModel> {
  @Input() public mode: QuestionBankMode = QuestionBankMode.Import;
  @Input() public questionType?: QuestionType[] = [];

  // Filter
  @Input()
  public set questionGroupIds(value: string[]) {
    if (!this.initiated) {
      return;
    }

    this._questionGroupIds = value;
    this.refreshData();
  }
  public get questionGroupIds(): string[] {
    return this._questionGroupIds;
  }
  // Search
  @Input()
  public set search(value: string | undefined) {
    if (!this.initiated) {
      return;
    }

    this._searchText = value;
    this.refreshData();
  }

  public get search(): string | undefined {
    return this._searchText;
  }

  public questionBankMode = QuestionBankMode;
  public editingRow: number = -1;
  public readonly gapQuestionGroupPosition: number = 4;
  public questionTypeLabel = QUESTION_TYPE_LABEL;

  private _searchText: string | undefined;
  private _questionGroupIds: string[] = [];
  private _loadDataSub: Subscription = new Subscription();

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public opalDialogService: OpalDialogService,
    private apiSvc: QuestionBankListService,
    private apiService: QuestionBankApiService
  ) {
    super(moduleFacadeService);
  }

  public onCheckAll(checked: boolean): void {
    this.checkAll = checked;

    // Clear list selected
    if (checked === false) {
      this.selectedItems = [];
      this.selecteds = {};
    }

    this.gridData.data.forEach(item => {
      if (checked) {
        this.selectedItems.push(item);
      }
      this.selecteds[item.id] = checked;
      item.selected = checked;
    });
    this.selectedItemsChangeEvent.emit(this.selectedItems);
  }

  public onCheckItem(checked: boolean, dataItem: QuestionBankViewModel): void {
    this.selecteds[dataItem.id] = checked;
    if (checked) {
      if (!this.selectedItems.includes(dataItem)) {
        this.selectedItems.push(dataItem);
      }
    } else {
      this.selectedItems = Utils.removeFirst(this.selectedItems, dataItem) as QuestionBankViewModel[];
    }

    this.checkAll = this.selectedItems.length === this.gridData.total;
    this.selectedItemsChangeEvent.emit(this.selectedItems);
  }

  public onDeleteFile(item: QuestionBank): void {
    this.moduleFacadeService.modalService.showConfirmMessage('Are you sure you want to delete this question?', () => {
      this.apiService.deleteQuestionBank(item.id).then(() => {
        if (this.editingRow > -1) {
          this.editingRow = -1;
        }
        this.refreshData();
        this.showNotification(`Question successfully deleted`, NotificationType.Success);
      });
    });
  }

  public onQuestionGroupChange(questionBank: QuestionBank, questionGroup: QuestionGroup[]): void {
    questionBank.questionGroupName = questionGroup && questionGroup[0] ? questionGroup[0].name : null;
    const request = new SaveQuestionBankRequest(questionBank);
    this.apiService.updateQuestionBank(request).then(() => {
      this.editingRow = -1;
      this.refreshData();
    });
  }

  public onClosed(): void {
    this.editingRow = -1;
  }

  public onGridCellClick(event: CellClickEvent): void {
    if (
      event.dataItem === undefined ||
      (event.columnIndex !== 0 && event.columnIndex !== 1) ||
      (event.columnIndex === 0 && this.mode === QuestionBankMode.Import)
    ) {
      return;
    }

    const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: QuestionBankPreviewDialogComponent });
    const configurationPopup = dialogRef.content.instance as QuestionBankPreviewDialogComponent;
    const questionBank = new QuestionBank(event.dataItem);
    const question = ModelMappingHelper.questionBankToFormQuestion(questionBank);
    configurationPopup.question = question;
  }

  public editQuestionGroup(index: number): void {
    this.editingRow = index;
  }

  public getQuestionTypeLabel(question: QuestionType): string {
    return this.translateCommon(this.questionTypeLabel.get(question));
  }

  public isQuestionGroupDisplayTop(rowIndex: number, questionBanks: QuestionBank[]): boolean {
    const questionBanksLength = questionBanks.length;
    let rowIdxDisplayTop: number;
    if (rowIdxDisplayTop < this.gapQuestionGroupPosition) {
      rowIdxDisplayTop = questionBanksLength;
    } else if (questionBanksLength === this.gapQuestionGroupPosition) {
      rowIdxDisplayTop = questionBanksLength - 1;
    } else {
      const minusValue = questionBanksLength - this.gapQuestionGroupPosition;
      rowIdxDisplayTop = minusValue > this.gapQuestionGroupPosition ? minusValue : this.gapQuestionGroupPosition - 1;
    }
    return rowIndex >= rowIdxDisplayTop;
  }

  protected loadData(): void {
    this._loadDataSub.unsubscribe();
    this._loadDataSub = this.apiSvc
      .loadQuestionBanks(this.search, this.questionGroupIds, this.questionType, this.state.skip, this.state.take)
      .pipe(this.untilDestroy())
      .subscribe(data => {
        this.gridData = data;
      });
  }
}
