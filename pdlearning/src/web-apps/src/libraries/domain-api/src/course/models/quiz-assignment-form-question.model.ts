import { AssignmentQuestionOption, AssignmentQuestionOptionType, IAssignmentQuestionOption } from './assignment-question-option.model';
import { Guid, Utils } from '@opal20/infrastructure';
import { IOptionMedia, MediaType } from './../../form/models/form-question-option.model';

import { AssignmentAnswerTrack } from './assignment-answer-track.model';
import { AssignmentType } from './assignment-type.model';
import { ParticipantAssignmentTrackQuizQuestionAnswer } from './participant-assignment-track-quiz-question-answer.model';
import { ValidationErrors } from '@angular/forms';

export type AssignmentQuestionAnswerSingleValue = string | number | boolean | Date;

export type AssignmentQuestionAnswerValue = AssignmentQuestionAnswerSingleValue | AssignmentQuestionAnswerSingleValue[];

export enum QuizAssignmentQuestionType {
  TrueFalse = 'TrueFalse',
  SingleChoice = 'SingleChoice',
  MultipleChoice = 'MultipleChoice',
  FreeText = 'FreeText',
  FillInTheBlanks = 'FillInTheBlanks',
  DropDown = 'DropDown',
  DatePicker = 'DatePicker',
  DateRangePicker = 'DateRangePicker'
}

export enum AssignmentQuestionModelValidationKey {
  questionOptionsAtLeast2 = 'questionOptionsAtLeast2',
  questionOptionsValueNotDuplicated = 'questionOptionsValueNotDuplicated',
  questionMustHaveCorrectAnswer = 'questionMustHaveCorrectAnswer',
  questionHasSomeTextOptions = 'questionHasSomeTextOptions',
  questionHasSomeBlankOptions = 'questionHasSomeTextOptions',
  questionMustHaveTitle = 'questionMustHaveTitle',
  questionMustFillAllTheBlank = 'questionMustFillAllTheBlank'
}

export interface IQuizAssignmentFormQuestion {
  id: string | null;
  quizAssignmentFormId: string;
  priority: number;
  maxScore: number;
  randomizedOptions: boolean;
  question_Type: QuizAssignmentQuestionType;
  question_Title: string;
  question_CorrectAnswer: AssignmentQuestionAnswerValue;
  question_Options?: IAssignmentQuestionOption[] | null;
  question_Hint: string | null;
  question_AnswerExplanatoryNote: string | null;
  question_FeedbackCorrectAnswer: string | null;
  question_FeedbackWrongAnswer: string | null;
  createdDate: Date;
  changedDate: Date | undefined;
  isDeleted: boolean;
}

export class QuizAssignmentFormQuestion implements IQuizAssignmentFormQuestion {
  public static questionTypeToShowExplationNote: QuizAssignmentQuestionType[] = [
    QuizAssignmentQuestionType.DatePicker,
    QuizAssignmentQuestionType.DateRangePicker,
    QuizAssignmentQuestionType.FillInTheBlanks,
    QuizAssignmentQuestionType.DropDown,
    QuizAssignmentQuestionType.FreeText,
    QuizAssignmentQuestionType.SingleChoice,
    QuizAssignmentQuestionType.TrueFalse,
    QuizAssignmentQuestionType.MultipleChoice
  ];

  public static questionTypeToShowRandomizeOption: QuizAssignmentQuestionType[] = [
    QuizAssignmentQuestionType.DropDown,
    QuizAssignmentQuestionType.SingleChoice,
    QuizAssignmentQuestionType.TrueFalse,
    QuizAssignmentQuestionType.MultipleChoice
  ];

  public id: string | null;
  public quizAssignmentFormId: string;
  public priority: number = 0;
  public maxScore: number = 1;
  public randomizedOptions: boolean = false;
  // tslint:disable-next-line:variable-name
  public question_Type: QuizAssignmentQuestionType = QuizAssignmentQuestionType.FreeText;
  // tslint:disable-next-line:variable-name
  public question_Title: string = '';
  // tslint:disable-next-line:variable-name
  public question_CorrectAnswer: AssignmentQuestionAnswerValue;
  // tslint:disable-next-line:variable-name
  public question_Options?: AssignmentQuestionOption[] | null;
  // tslint:disable-next-line:variable-name
  public question_Hint: string | null;
  // tslint:disable-next-line:variable-name
  public question_AnswerExplanatoryNote: string | null;
  // tslint:disable-next-line:variable-name
  public question_FeedbackCorrectAnswer: string | null;
  // tslint:disable-next-line:variable-name
  public question_FeedbackWrongAnswer: string | null;
  public createdDate: Date = new Date();
  public changedDate: Date | undefined;
  public isDeleted: boolean;

  public static validate(model: QuizAssignmentFormQuestion, assignmentType: AssignmentType): ValidationErrors | undefined {
    switch (model.question_Type) {
      case QuizAssignmentQuestionType.MultipleChoice:
      case QuizAssignmentQuestionType.SingleChoice:
      case QuizAssignmentQuestionType.DropDown:
        if (model.question_Options && model.question_Options.length < 2) {
          return { [AssignmentQuestionModelValidationKey.questionOptionsAtLeast2]: 'Must have at least 2 options' };
        }
        if (model.question_Options && Utils.hasDuplicatedItems(model.question_Options, p => p.value)) {
          return { [AssignmentQuestionModelValidationKey.questionOptionsValueNotDuplicated]: 'All options must be unique' };
        }
        return model.hasCorrectAnswer() || assignmentType !== AssignmentType.Quiz
          ? null
          : { [AssignmentQuestionModelValidationKey.questionMustHaveCorrectAnswer]: 'Question need to have a correct answer' };
      case QuizAssignmentQuestionType.FillInTheBlanks:
        if (!model.question_Options.some(option => option.type === AssignmentQuestionOptionType.Text)) {
          return {
            [AssignmentQuestionModelValidationKey.questionHasSomeTextOptions]: 'Question must has at least a text option'
          };
        }
        if (!model.question_Options.some(option => option.type === AssignmentQuestionOptionType.Blank)) {
          return {
            [AssignmentQuestionModelValidationKey.questionHasSomeBlankOptions]: 'Question must has at least a blank option'
          };
        }
        if (
          model.question_CorrectAnswer == null ||
          (Array.isArray(model.question_CorrectAnswer) &&
            model.question_Options.filter(option => option.type === AssignmentQuestionOptionType.Blank).length !==
              model.question_CorrectAnswer.length)
        ) {
          return {
            [AssignmentQuestionModelValidationKey.questionMustFillAllTheBlank]: 'Question must fill all blank option'
          };
        }
        return null;
      case QuizAssignmentQuestionType.DateRangePicker:
        return model.question_CorrectAnswer instanceof Array &&
          model.question_CorrectAnswer.length === 2 &&
          model.question_CorrectAnswer[0] != null &&
          model.question_CorrectAnswer[1] != null
          ? null
          : { [AssignmentQuestionModelValidationKey.questionMustHaveCorrectAnswer]: 'Question need to have from-to correct answer' };
      case QuizAssignmentQuestionType.FreeText:
      case QuizAssignmentQuestionType.TrueFalse:
      case QuizAssignmentQuestionType.DatePicker:
        return model.hasCorrectAnswer()
          ? null
          : { [AssignmentQuestionModelValidationKey.questionMustHaveCorrectAnswer]: 'Question need to have a correct answer' };
      default:
        break;
    }
  }

  public static createNewTrueFalseQuestion(
    quizAssignmentFormId: string,
    priority: number,
    questionTitle: string,
    maxScore: number | undefined
  ): QuizAssignmentFormQuestion {
    const question: QuizAssignmentFormQuestion = new QuizAssignmentFormQuestion();
    question.id = Guid.create().toString();
    question.quizAssignmentFormId = quizAssignmentFormId;
    question.priority = priority;
    question.question_Type = QuizAssignmentQuestionType.TrueFalse;
    question.question_Title = questionTitle;
    question.createdDate = new Date();
    question.question_Options = [
      new AssignmentQuestionOption({ code: 1, value: true, feedback: null, type: null, imageUrl: null, videoUrl: null }),
      new AssignmentQuestionOption({ code: 2, value: false, feedback: null, type: null, imageUrl: null, videoUrl: null })
    ];
    question.maxScore = maxScore;
    return question;
  }

  public static createNewSingleChoiceQuestion(
    quizAssignmentFormId: string,
    priority: number,
    questionTitle: string,
    maxScore: number | undefined
  ): QuizAssignmentFormQuestion {
    const question: QuizAssignmentFormQuestion = new QuizAssignmentFormQuestion();
    question.id = Guid.create().toString();
    question.quizAssignmentFormId = quizAssignmentFormId;
    question.priority = priority;
    question.question_Type = QuizAssignmentQuestionType.SingleChoice;
    question.question_Title = questionTitle;
    question.question_Options = [];
    question.createdDate = new Date();
    question.maxScore = maxScore;
    return question;
  }

  public static createNewMultipleChoiceQuestion(
    quizAssignmentFormId: string,
    priority: number,
    questionTitle: string,
    maxScore: number | undefined
  ): QuizAssignmentFormQuestion {
    const question: QuizAssignmentFormQuestion = new QuizAssignmentFormQuestion();
    question.id = Guid.create().toString();
    question.quizAssignmentFormId = quizAssignmentFormId;
    question.priority = priority;
    question.question_Type = QuizAssignmentQuestionType.MultipleChoice;
    question.question_Title = questionTitle;
    question.question_Options = [];
    question.createdDate = new Date();
    question.maxScore = maxScore;
    question.question_CorrectAnswer = [];
    return question;
  }

  public static createNewFreeTextQuestion(
    quizAssignmentFormId: string,
    priority: number,
    questionTitle: string,
    maxScore: number | undefined
  ): QuizAssignmentFormQuestion {
    const question: QuizAssignmentFormQuestion = new QuizAssignmentFormQuestion();
    question.id = Guid.create().toString();
    question.quizAssignmentFormId = quizAssignmentFormId;
    question.priority = priority;
    question.question_Type = QuizAssignmentQuestionType.FreeText;
    question.question_Title = questionTitle;
    question.createdDate = new Date();
    question.maxScore = maxScore;
    return question;
  }

  public static createNewFillInTheBlanksQuestion(
    quizAssignmentFormId: string,
    priority: number,
    questionTitle: string,
    maxScore: number | undefined
  ): QuizAssignmentFormQuestion {
    const question: QuizAssignmentFormQuestion = new QuizAssignmentFormQuestion();
    question.id = Guid.create().toString();
    question.quizAssignmentFormId = quizAssignmentFormId;
    question.priority = priority;
    question.question_Options = [];
    question.question_CorrectAnswer = [];
    question.question_Type = QuizAssignmentQuestionType.FillInTheBlanks;
    question.question_Title = questionTitle;
    question.createdDate = new Date();
    question.maxScore = maxScore;
    return question;
  }

  public static createNewDropDownQuestion(
    quizAssignmentFormId: string,
    priority: number,
    questionTitle: string,
    maxScore: number | undefined
  ): QuizAssignmentFormQuestion {
    const question: QuizAssignmentFormQuestion = new QuizAssignmentFormQuestion();
    question.id = Guid.create().toString();
    question.quizAssignmentFormId = quizAssignmentFormId;
    question.priority = priority;
    question.question_Type = QuizAssignmentQuestionType.DropDown;
    question.question_Title = questionTitle;
    question.question_Options = [];
    question.createdDate = new Date();
    question.maxScore = maxScore;
    return question;
  }

  public static createNewDatePickerQuestion(
    quizAssignmentFormId: string,
    priority: number,
    questionTitle: string,
    maxScore: number | undefined
  ): QuizAssignmentFormQuestion {
    const question: QuizAssignmentFormQuestion = new QuizAssignmentFormQuestion();
    question.id = Guid.create().toString();
    question.quizAssignmentFormId = quizAssignmentFormId;
    question.priority = priority;
    question.question_Type = QuizAssignmentQuestionType.DatePicker;
    question.question_Title = questionTitle;
    question.question_Options = [];
    question.createdDate = new Date();
    question.maxScore = maxScore;
    return question;
  }

  public static createNewDateRangePickerQuestion(
    quizAssignmentFormId: string,
    priority: number,
    questionTitle: string,
    maxScore: number | undefined
  ): QuizAssignmentFormQuestion {
    const question: QuizAssignmentFormQuestion = new QuizAssignmentFormQuestion();
    question.id = Guid.create().toString();
    question.quizAssignmentFormId = quizAssignmentFormId;
    question.priority = priority;
    question.question_Type = QuizAssignmentQuestionType.DateRangePicker;
    question.question_Title = questionTitle;
    question.question_Options = [];
    question.createdDate = new Date();
    question.maxScore = maxScore;
    return question;
  }

  public static calcMaxScore(questions: QuizAssignmentFormQuestion[]): number {
    let result = 0;
    questions.forEach(p => {
      result += p.maxScore != null ? p.maxScore : 0;
    });
    return result;
  }

  constructor(data?: IQuizAssignmentFormQuestion) {
    if (data != null) {
      this.id = data.id;
      this.quizAssignmentFormId = data.quizAssignmentFormId;
      this.priority = data.priority;
      this.maxScore = data.maxScore;
      this.randomizedOptions = data.randomizedOptions;
      this.question_Type = data.question_Type;
      this.question_Title = data.question_Title;

      const isDateQuestion = this.isDateQuestion();
      this.question_CorrectAnswer =
        data.question_CorrectAnswer != null
          ? isDateQuestion
            ? Array.isArray(data.question_CorrectAnswer)
              ? data.question_CorrectAnswer.map(x => new Date(x.toString()))
              : new Date(data.question_CorrectAnswer.toString())
            : data.question_CorrectAnswer
          : null;
      this.question_Options =
        data.question_Options != null ? data.question_Options.map(p => new AssignmentQuestionOption(p, isDateQuestion)) : null;

      this.question_Hint = data.question_Hint;
      this.question_AnswerExplanatoryNote = data.question_AnswerExplanatoryNote;
      this.question_FeedbackCorrectAnswer = data.question_FeedbackCorrectAnswer;
      this.question_FeedbackWrongAnswer = data.question_FeedbackWrongAnswer;
    }
  }

  public getBlankOptions(): AssignmentQuestionOption[] {
    return this.question_Options.filter(questionOption => questionOption.type === AssignmentQuestionOptionType.Blank);
  }

  public generateTitle(): void {
    if (!(this.question_Type === QuizAssignmentQuestionType.FillInTheBlanks)) {
      return;
    }
    let questionTitleBuilder: string = '';
    for (const questionOption of this.question_Options) {
      switch (questionOption.type) {
        case AssignmentQuestionOptionType.Text: {
          questionTitleBuilder += questionOption.value + ' ';
          break;
        }
        case AssignmentQuestionOptionType.Blank: {
          questionTitleBuilder += '---' + ' ';
          break;
        }
      }
    }
    this.question_Title = questionTitleBuilder.trim();
  }

  public updateQuestionOptionValue(value: AssignmentQuestionAnswerSingleValue, optionIndex: number): void {
    const currentOption = this.question_Options[optionIndex];
    if (currentOption.type !== AssignmentQuestionOptionType.Text) {
      const currentOptionValue = currentOption.value;
      const isCurrentOptionValueCorrect = this.isOptionValueCorrect(currentOptionValue);
      if (isCurrentOptionValueCorrect) {
        this.updateQuestionCorrectAnswer(currentOptionValue, value);
      }
    }
    this.question_Options = Utils.clone(this.question_Options, questionOptions => {
      questionOptions[optionIndex] = Utils.clone(questionOptions[optionIndex], questionOption => {
        questionOption.value = value;
      });
    });
  }

  public addQuestionOptionMediaUrl(media: IOptionMedia, optionIndex: number): void {
    this.question_Options = Utils.clone(this.question_Options, questionOptions => {
      questionOptions[optionIndex] = Utils.clone(questionOptions[optionIndex], questionOption => {
        switch (media.type) {
          case MediaType.Video:
            questionOption.videoUrl = media.src;
            questionOption.imageUrl = '';
            break;
          case MediaType.Image:
            questionOption.imageUrl = media.src;
            questionOption.videoUrl = '';
            break;
        }
      });
    });
  }

  public isOptionValueCorrect(optionValue: AssignmentQuestionAnswerSingleValue): boolean {
    if (this.question_CorrectAnswer == null) {
      return false;
    }
    if (Array.isArray(this.question_CorrectAnswer)) {
      return this.question_CorrectAnswer.indexOf(optionValue) >= 0;
    }

    return this.question_CorrectAnswer === optionValue;
  }

  public updateQuestionCorrectAnswer(
    currentOptionValue: AssignmentQuestionAnswerSingleValue | undefined,
    newOptionValue: AssignmentQuestionAnswerSingleValue | undefined
  ): void {
    switch (this.question_Type) {
      case QuizAssignmentQuestionType.MultipleChoice:
      case QuizAssignmentQuestionType.FillInTheBlanks: {
        if (!(this.question_CorrectAnswer instanceof Array)) {
          throw new Error('Question Correct answer must be an array');
        }
        const currentOptionValueIndex = this.question_CorrectAnswer.indexOf(currentOptionValue);
        if (currentOptionValueIndex < 0) {
          this.question_CorrectAnswer = Utils.clone(this.question_CorrectAnswer, p => {
            p.push(newOptionValue);
          });
          break;
        }
        this.question_CorrectAnswer = Utils.clone(this.question_CorrectAnswer, p => {
          if (newOptionValue) {
            p[currentOptionValueIndex] = newOptionValue;
          } else {
            p.splice(currentOptionValueIndex, 1);
          }
        });
        break;
      }
      default: {
        this.question_CorrectAnswer = newOptionValue;
        break;
      }
    }
  }

  public addNewOption(addNewOptionValue: AssignmentQuestionAnswerSingleValue, isCorrect: boolean): void {
    this.question_Options = Utils.clone(this.question_Options, questionOptions => {
      questionOptions.push(
        new AssignmentQuestionOption({
          code: questionOptions.length + 1,
          value: addNewOptionValue,
          feedback: null,
          type: null,
          imageUrl: null,
          videoUrl: null
        })
      );
    });
    if (isCorrect) {
      this.updateQuestionCorrectAnswer(undefined, addNewOptionValue);
    }
  }

  public addTextOption(addNewOptionValue: AssignmentQuestionAnswerSingleValue): void {
    this.question_Options = Utils.clone(this.question_Options, questionOptions => {
      questionOptions.push(
        new AssignmentQuestionOption({
          code: questionOptions.length + 1,
          value: addNewOptionValue,
          feedback: null,
          type: AssignmentQuestionOptionType.Text,
          imageUrl: null,
          videoUrl: null
        })
      );
    });
  }

  public addBlankOption(addNewOptionValue: AssignmentQuestionAnswerSingleValue, optionFeedback: string | null): void {
    this.question_Options = Utils.clone(this.question_Options, questionOptions => {
      questionOptions.push(
        new AssignmentQuestionOption({
          code: questionOptions.length + 1,
          value: addNewOptionValue,
          feedback: optionFeedback,
          type: AssignmentQuestionOptionType.Blank,
          imageUrl: null,
          videoUrl: null
        })
      );
    });
    this.updateQuestionCorrectAnswer(undefined, addNewOptionValue);
  }

  public removeOption(optionIndex: number): void {
    const currentOptionValue = this.question_Options[optionIndex].value;
    const isCurrentOptionValueCorrect = this.isOptionValueCorrect(currentOptionValue);
    if (isCurrentOptionValueCorrect) {
      this.updateQuestionCorrectAnswer(currentOptionValue, undefined);
    }
    this.question_Options = Utils.clone(this.question_Options, questionOptions => {
      questionOptions.splice(optionIndex, 1);
      questionOptions.forEach((p, i) => {
        questionOptions[i] = Utils.clone(questionOptions[i], questionOption => {
          questionOption.code = i;
        });
      });
    });
  }

  public hasCorrectAnswer(): boolean {
    if (this.question_Type === QuizAssignmentQuestionType.FreeText) {
      return !Utils.isNullOrEmpty(this.question_CorrectAnswer);
    }
    return (
      !Utils.isNullOrEmpty(this.question_CorrectAnswer) &&
      (!(this.question_CorrectAnswer instanceof Array) || this.question_CorrectAnswer.length > 0)
    );
  }

  public getCorrectAnswerLenght(): number {
    return (this.question_CorrectAnswer as string[]).length;
  }

  public isDateQuestion(): boolean {
    return (
      this.question_Type === QuizAssignmentQuestionType.DatePicker || this.question_Type === QuizAssignmentQuestionType.DateRangePicker
    );
  }

  public getAssignmentAnswerTrack(answer: ParticipantAssignmentTrackQuizQuestionAnswer): AssignmentAnswerTrack {
    const isDateQuestion = this.isDateQuestion();

    const answerValue =
      answer != null && answer.answerValue != null
        ? isDateQuestion
          ? Array.isArray(answer.answerValue)
            ? answer.answerValue.map(x => (x != null ? new Date(x.toString()) : null))
            : new Date(answer.answerValue.toString())
          : answer.answerValue
        : null;

    const questionOptionDic = this.hasQuestionOption() ? this.buildQuestionOptionCorrectDic(answerValue) : null;

    const isMultipleCorrectAnswer = Array.isArray(this.question_CorrectAnswer);

    let numberCorrectOptionCorrect = 0;

    if (questionOptionDic != null) {
      Object.keys(questionOptionDic).forEach(key => {
        if (questionOptionDic[key] === true) {
          numberCorrectOptionCorrect++;
        }
      });
    }

    return {
      questionId: this.id,
      questionAnswer: answerValue,
      correctAnswer: this.question_CorrectAnswer,
      manualScore: answer ? answer.manualScore : null,
      score: answer ? answer.score : null,
      giveScore: answer ? answer.givedScore : null,
      questionOptionCorrectDic: questionOptionDic,
      isCorrect:
        isMultipleCorrectAnswer === true
          ? numberCorrectOptionCorrect === (this.question_CorrectAnswer as AssignmentQuestionAnswerSingleValue[]).length
          : numberCorrectOptionCorrect === 1,
      submitedDate: answer ? answer.submittedDate : null
    };
  }

  private checkCorrectSingleAnswer(correctValue: AssignmentQuestionAnswerValue, answerValue: AssignmentQuestionAnswerValue): boolean {
    if (correctValue == null || answerValue == null) {
      return false;
    }
    if (correctValue === answerValue) {
      return true;
    }
    if (
      (typeof correctValue && typeof answerValue) === 'string' &&
      correctValue
        .toString()
        .trim()
        .toLowerCase() ===
        answerValue
          .toString()
          .trim()
          .toLowerCase()
    ) {
      return true;
    }

    return false;
  }

  private hasQuestionOption(): boolean {
    return this.question_Type !== QuizAssignmentQuestionType.FreeText;
  }

  private buildQuestionOptionCorrectDic(answerValue: AssignmentQuestionAnswerValue): Dictionary<boolean> {
    const questionOptionDic = Utils.toDictionarySelect(this.question_Options, p => p.code, p => null);

    if (Array.isArray(this.question_CorrectAnswer) && Array.isArray(answerValue)) {
      if (this.question_Type === QuizAssignmentQuestionType.FillInTheBlanks) {
        let indexOfBlankOption = -1;
        this.question_Options.forEach(option => {
          if (option.type === AssignmentQuestionOptionType.Blank) {
            indexOfBlankOption++;
          }
          if (indexOfBlankOption > -1) {
            questionOptionDic[option.code] =
              answerValue[indexOfBlankOption] != null
                ? this.checkCorrectSingleAnswer(this.question_CorrectAnswer[indexOfBlankOption], answerValue[indexOfBlankOption])
                : null;
          }
        });
      } else {
        this.question_Options.forEach(option => {
          answerValue.forEach(answer => {
            if (this.checkCorrectSingleAnswer(option.value, answer)) {
              questionOptionDic[option.code] =
                (this.question_CorrectAnswer as AssignmentQuestionAnswerSingleValue[]).find(x =>
                  this.checkCorrectSingleAnswer(x, answer)
                ) != null;
            }
          });
        });
      }
    } else {
      this.question_Options.forEach(option => {
        if (this.checkCorrectSingleAnswer(option.value, answerValue)) {
          questionOptionDic[option.code] = this.checkCorrectSingleAnswer(this.question_CorrectAnswer, answerValue);
        }
      });
    }

    return questionOptionDic;
  }
}
