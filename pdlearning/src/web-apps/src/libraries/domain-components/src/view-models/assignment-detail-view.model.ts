import {
  Assessment,
  Assignment,
  AssignmentAssessmentConfig,
  AssignmentType,
  QuizAssignmentFormQuestion,
  QuizAssignmentQuestionType,
  ScoreMode
} from '@opal20/domain-api';

import { Utils } from '@opal20/infrastructure';

export class AssignmentDetailViewModel {
  public data: Assignment = new Assignment();
  public originalData: Assignment = new Assignment();
  public selectedAssignmentQuestion: QuizAssignmentFormQuestion | undefined;
  public assessmentDic: Dictionary<Assessment> = {};

  constructor(assignment?: Assignment, public assessments: Assessment[] = []) {
    if (assignment) {
      this.updateAssignmentData(assignment);
    }

    this.assessmentDic = Utils.toDictionarySelect(assessments, x => x.id, x => x);
  }

  public get id(): string {
    return this.data.id;
  }
  public set id(id: string) {
    this.data.id = id;
  }

  public get courseId(): string {
    return this.data.courseId;
  }
  public set courseId(courseId: string) {
    this.data.courseId = courseId;
  }

  public get classRunId(): string | null {
    return this.data.classRunId;
  }
  public set classRunId(classRunId: string | null) {
    this.data.classRunId = classRunId;
  }

  public get createdBy(): string {
    return this.data.createdBy;
  }

  public set createdBy(createBy: string) {
    this.data.createdBy = createBy;
  }

  public get type(): AssignmentType {
    return this.data.type;
  }

  public set type(type: AssignmentType) {
    this.data.type = type;
  }

  public get title(): string {
    return this.data.title;
  }

  public set title(title: string) {
    this.data.title = title;
  }

  public get assessmentConfig(): AssignmentAssessmentConfig | null {
    return this.data.assessmentConfig;
  }

  public set assessmentConfig(assessmentConfig: AssignmentAssessmentConfig | null) {
    this.data.assessmentConfig = assessmentConfig;
  }

  public get assessmentId(): string | null {
    return this.data.assessmentConfig ? this.data.assessmentConfig.assessmentId : null;
  }
  public set assessmentId(v: string | null) {
    if (v == null) {
      this.data.assessmentConfig = null;
    } else {
      if (this.data.assessmentConfig == null) {
        this.data.assessmentConfig = new AssignmentAssessmentConfig();
      }
      this.data.assessmentConfig.assessmentId = v;
    }
  }
  public get numberAutoAssessor(): number | null {
    return this.data.assessmentConfig ? this.data.assessmentConfig.numberAutoAssessor : null;
  }
  public set numberAutoAssessor(v: number | null) {
    if (v != null) {
      if (this.data.assessmentConfig == null) {
        this.data.assessmentConfig = new AssignmentAssessmentConfig();
      }
      this.data.assessmentConfig.numberAutoAssessor = v;
    }
  }
  public get scoreMode(): ScoreMode | null {
    return this.data.assessmentConfig ? this.data.assessmentConfig.scoreMode : null;
  }
  public set scoreMode(v: ScoreMode | null) {
    if (v != null) {
      if (this.data.assessmentConfig == null) {
        this.data.assessmentConfig = new AssignmentAssessmentConfig();
      }
      this.data.assessmentConfig.scoreMode = v;
    }
  }

  public get randomizedQuestions(): boolean {
    return this.data.quizAssignmentForm.randomizedQuestions;
  }

  public set randomizedQuestions(randomizedQuestions: boolean) {
    this.data.quizAssignmentForm.randomizedQuestions = randomizedQuestions;
  }

  public get questions(): QuizAssignmentFormQuestion[] {
    return this.data.quizAssignmentForm.questions;
  }

  public set questions(questions: QuizAssignmentFormQuestion[]) {
    this.data.quizAssignmentForm.questions = questions;
  }

  public get totalQuestion(): number {
    return this.questions.length;
  }

  public updateAssignmentData(assignment: Assignment): void {
    this.originalData = Utils.cloneDeep(assignment);
    this.data = Utils.cloneDeep(assignment);

    if (this.data.quizAssignmentForm != null) {
      this.questions = Utils.orderBy(this.data.quizAssignmentForm.questions, x => x.priority);
    }
  }

  public hasDataChanged(): boolean {
    return Utils.isDifferent(this.originalData, this.data);
  }

  public insertQuestion(questionType: QuizAssignmentQuestionType, priority: number): void {
    const newQuestion = this.createNewQuestion(questionType, priority);
    this.updateQuestion(questionsData => {
      questionsData.splice(priority, 0, newQuestion);
      this.processQuestionsPriority(questionsData);
    });
    this.selectedAssignmentQuestion = newQuestion;
  }

  public updateQuestion(actionFn: (data: QuizAssignmentFormQuestion[]) => void): void {
    this.questions = Utils.clone(this.questions, p => {
      actionFn(p);
    });
  }

  public deleteQuestion(questionId: string): void {
    this.updateQuestion(questionsData => {
      const questionIndex = questionsData.findIndex(p => p.id === questionId);
      if (questionIndex > -1) {
        questionsData.splice(questionIndex, 1);
        this.processQuestionsPriority(questionsData);
      }
      this.selectedAssignmentQuestion = null;
    });
  }

  public moveQuestion(questionId: string, step: number): void {
    this.updateQuestion(questionsData => {
      Utils.move(questionsData, p => p.id === questionId, p => p + step);
      this.processQuestionsPriority(questionsData);
      this.selectedAssignmentQuestion = questionsData.find(p => p.id === questionId);
    });
  }

  private createNewQuestion(type: QuizAssignmentQuestionType, priority: number): QuizAssignmentFormQuestion {
    const questionTitle = '';
    switch (type) {
      case QuizAssignmentQuestionType.FillInTheBlanks:
        return QuizAssignmentFormQuestion.createNewFillInTheBlanksQuestion(this.id, priority, questionTitle, 1);
      case QuizAssignmentQuestionType.FreeText:
        return QuizAssignmentFormQuestion.createNewFreeTextQuestion(this.id, priority, questionTitle, 1);
      case QuizAssignmentQuestionType.TrueFalse:
        return QuizAssignmentFormQuestion.createNewTrueFalseQuestion(this.id, priority, questionTitle, 1);
      case QuizAssignmentQuestionType.SingleChoice:
        return QuizAssignmentFormQuestion.createNewSingleChoiceQuestion(this.id, priority, questionTitle, 1);
      case QuizAssignmentQuestionType.MultipleChoice:
        return QuizAssignmentFormQuestion.createNewMultipleChoiceQuestion(this.id, priority, questionTitle, 1);
      case QuizAssignmentQuestionType.DropDown:
        return QuizAssignmentFormQuestion.createNewDropDownQuestion(this.id, priority, questionTitle, 1);
      case QuizAssignmentQuestionType.DatePicker:
        return QuizAssignmentFormQuestion.createNewDatePickerQuestion(this.id, priority, questionTitle, 1);
      case QuizAssignmentQuestionType.DateRangePicker:
        return QuizAssignmentFormQuestion.createNewDateRangePickerQuestion(this.id, priority, questionTitle, 1);
      default:
        return undefined;
    }
  }

  private processQuestionsPriority(questions: QuizAssignmentFormQuestion[]): void {
    questions.forEach((p, i) => {
      p.priority = i;
    });
  }
}
