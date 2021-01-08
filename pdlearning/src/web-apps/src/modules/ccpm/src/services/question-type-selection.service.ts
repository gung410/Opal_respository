import { IQuestionBankSelection, QuestionType } from '@opal20/domain-api';
import { Observable, Subject } from 'rxjs';

import { Injectable } from '@angular/core';

@Injectable()
export class QuestionTypeSelectionService {
  public get getNewQuestionType$(): Observable<{ type: QuestionType; priority: number; minorPriority: number }> {
    return this.questionTypeSelectionSubject.asObservable();
  }

  public get getQuestionListImport$(): Observable<IQuestionBankSelection> {
    return this.getQuestionListImportSubject.asObservable();
  }

  private questionTypeSelectionSubject: Subject<{ type: QuestionType; priority: number; minorPriority: number }> = new Subject();
  private getQuestionListImportSubject: Subject<IQuestionBankSelection> = new Subject();

  public setNewQuestionType(newQuestionType: QuestionType, priority: number, minorPriority: number): void {
    this.questionTypeSelectionSubject.next({ type: newQuestionType, priority: priority, minorPriority: minorPriority });
  }
  public setQuestionListImport(questionBankSelection: IQuestionBankSelection): void {
    this.getQuestionListImportSubject.next({
      listQuestion: questionBankSelection.listQuestion,
      priority: questionBankSelection.priority,
      minorPriority: questionBankSelection.minorPriority
    });
  }
}
