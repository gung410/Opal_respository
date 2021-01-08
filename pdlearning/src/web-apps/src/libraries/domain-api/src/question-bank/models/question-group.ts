export interface IQuestionGroup {
  id: string;
  name: string;
}

export class QuestionGroup implements IQuestionGroup {
  public id: string;
  public name: string;

  constructor(data?: IQuestionGroup) {
    if (data == null) {
      return;
    }

    this.id = data.id;
    this.name = data.name;
  }
}
