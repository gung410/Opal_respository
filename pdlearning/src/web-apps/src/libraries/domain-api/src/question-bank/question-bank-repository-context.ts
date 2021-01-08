import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { QuestionBank } from './models/question-bank';

@Injectable()
export class QuestionBankRepositoryContext extends BaseRepositoryContext {
  public questionBanksSubject: BehaviorSubject<Dictionary<QuestionBank>> = new BehaviorSubject({});
}
