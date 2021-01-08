import { Observable, Subject } from 'rxjs';

import { Injectable } from '@angular/core';
import { StandaloneSurveyQuestionType } from '@opal20/domain-api';

@Injectable()
export class StandaloneSurveyQuestionTypeSelectionService {
  public get getNewQuestionType$(): Observable<{ type: StandaloneSurveyQuestionType; priority: number; minorPriority: number }> {
    return this.questionTypeSelectionSubject.asObservable();
  }

  private questionTypeSelectionSubject: Subject<{
    type: StandaloneSurveyQuestionType;
    priority: number;
    minorPriority: number;
  }> = new Subject();

  public setNewQuestionType(newQuestionType: StandaloneSurveyQuestionType, priority: number, minorPriority: number): void {
    this.questionTypeSelectionSubject.next({ type: newQuestionType, priority: priority, minorPriority: minorPriority });
  }
}
