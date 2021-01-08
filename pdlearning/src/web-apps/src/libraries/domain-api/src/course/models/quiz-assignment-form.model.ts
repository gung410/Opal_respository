import { IQuizAssignmentFormQuestion, QuizAssignmentFormQuestion } from './quiz-assignment-form-question.model';

export interface IQuizAssignmentForm {
  id: string | null;
  randomizedQuestions: boolean;
  questions: IQuizAssignmentFormQuestion[];
}

export class QuizAssignmentForm implements IQuizAssignmentForm {
  public id: string | null;
  public randomizedQuestions: boolean = false;
  public questions: QuizAssignmentFormQuestion[] = [];

  constructor(data?: IQuizAssignmentForm) {
    if (data != null) {
      this.id = data.id;
      this.randomizedQuestions = data.randomizedQuestions;
      this.questions = data.questions != null ? data.questions.map(p => new QuizAssignmentFormQuestion(p)) : [];
    }
  }
}
