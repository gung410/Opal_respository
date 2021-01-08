import { IPagedInfo } from '../../content/models/paged-info';
import { QuestionType } from '../../form/models/form-question.model';

export interface IQuestionBankSearchRequest {
  title: string;
  questionGroupIds: string[];
  questionTypes: QuestionType[];
  pagedInfo: IPagedInfo;
}
