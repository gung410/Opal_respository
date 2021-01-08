import { Observable, Subject } from 'rxjs';

import { Injectable } from '@angular/core';
import { QuizAssignmentQuestionType } from '@opal20/domain-api';

@Injectable()
export class AssignmentQuestionTypeSelectionService {
  public get getNewQuestionType$(): Observable<{ type: QuizAssignmentQuestionType; priority: number }> {
    return this.questionTypeSelectionSubject.asObservable();
  }

  private questionTypeSelectionSubject: Subject<{ type: QuizAssignmentQuestionType; priority: number }> = new Subject();

  public setNewQuestionType(newQuestionType: QuizAssignmentQuestionType, priority: number): void {
    this.questionTypeSelectionSubject.next({ type: newQuestionType, priority: priority });
  }
}
