import { IQuestionBank, QuestionBank } from './question-bank';

import { IGridDataItem } from '@opal20/infrastructure';

export class QuestionBankViewModel extends QuestionBank implements IGridDataItem {
  public type: string;
  public id: string;
  public selected: boolean;

  constructor(data?: IQuestionBank) {
    super(data);
  }
}
