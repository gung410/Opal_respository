import {
  AssignmentAnswerTrack,
  ParticipantAssignmentTrack,
  ParticipantAssignmentTrackQuizQuestionAnswer,
  ParticipantAssignmentTrackStatus,
  QuizAssignmentFormQuestion
} from '@opal20/domain-api';
import { DateUtils, Utils } from '@opal20/infrastructure';

import { AssignmentDetailViewModel } from './assignment-detail-view.model';
import { shuffle } from 'lodash-es';

export class ParticipantAssignmentTrackDetailViewModel {
  public static ASSIGNMENT_EXTENDED_DAYS: number = 30;
  public assignmentVm: AssignmentDetailViewModel = new AssignmentDetailViewModel();
  public participantAssignmentTrack: ParticipantAssignmentTrack = new ParticipantAssignmentTrack();
  public assignmentAnswerTrackDic: Dictionary<AssignmentAnswerTrack> = {};
  public originalAssignmentAnswerTrackDic: Dictionary<AssignmentAnswerTrack> = {};
  public numberCorrectAnswer: number = 0;
  public totalScore: number = 0;
  public scorePercentage: number = 0;
  public questionAnswerDic: Dictionary<ParticipantAssignmentTrackQuizQuestionAnswer>;
  // forLearnerAnswer = true => Correct_Answer will be answer of learner and it will be saved in table participant assignment track with field name is AnswerValue
  constructor(
    participantAssignmentTrack: ParticipantAssignmentTrack = null,
    assignmentVm: AssignmentDetailViewModel = null,
    forLearnerAnswer: boolean = false
  ) {
    if (participantAssignmentTrack != null) {
      this.participantAssignmentTrack = participantAssignmentTrack;
    }
    this.questionAnswerDic = Utils.toDictionary(this.questionAnswers, p => p.quizAssignmentFormQuestionId);
    if (assignmentVm) {
      this.assignmentVm = assignmentVm;
    }

    if (forLearnerAnswer === true) {
      this.buildQuestionForLearner();
    }

    this.buildAssignmentAnswerTrackDic();
  }

  public get canSaveOrSubmit(): boolean {
    return (
      DateUtils.addDays(this.participantAssignmentTrack.endDate, ParticipantAssignmentTrackDetailViewModel.ASSIGNMENT_EXTENDED_DAYS) >
        new Date() && this.participantAssignmentTrack.submittedDate == null
    );
  }

  public dataHasChanged(): boolean {
    return Utils.isDifferent(this.originalAssignmentAnswerTrackDic, this.assignmentAnswerTrackDic);
  }

  public getScoreOfQuestion(questionId: string): number {
    return this.assignmentAnswerTrackDic[questionId].score;
  }
  public setScoreOfQuestion(questionId: string, score: number): void {
    this.assignmentAnswerTrackDic[questionId].score = score;
  }

  public get questionAnswers(): ParticipantAssignmentTrackQuizQuestionAnswer[] {
    if (this.participantAssignmentTrack && this.participantAssignmentTrack.quizAnswer != null) {
      return this.participantAssignmentTrack.quizAnswer.questionAnswers;
    }

    return [];
  }

  public get status(): ParticipantAssignmentTrackStatus {
    return this.participantAssignmentTrack.status;
  }

  public get submittedDate(): Date {
    return this.participantAssignmentTrack.submittedDate;
  }

  public get completionRate(): number {
    return this.participantAssignmentTrack.getParticipantAssignmentTrackCompletionRate(this.assignmentVm.data);
  }

  public calcTotalScoreAndScorePercentage(): void {
    this.numberCorrectAnswer = 0;
    this.totalScore = 0;
    this.scorePercentage = 0;
    let totalMaxScore = 0;

    this.assignmentVm.questions.forEach(question => {
      const assignmentAnswerTrack = this.assignmentAnswerTrackDic[question.id];

      this.numberCorrectAnswer += assignmentAnswerTrack.giveScore === question.maxScore ? 1 : 0;
      this.totalScore += assignmentAnswerTrack.giveScore;
      totalMaxScore += question.maxScore;
    });

    this.scorePercentage = totalMaxScore !== 0 ? Utils.round((this.totalScore / totalMaxScore) * 100, 2) : 0;
  }

  public updateAssignmentAnswerTrack(question: QuizAssignmentFormQuestion): void {
    const assignmentAnswerTrack = question.getAssignmentAnswerTrack(this.questionAnswerDic[question.id]);
    this.assignmentAnswerTrackDic[assignmentAnswerTrack.questionId] = assignmentAnswerTrack;
  }

  private buildAssignmentAnswerTrackDic(): void {
    this.numberCorrectAnswer = 0;
    this.totalScore = 0;
    this.scorePercentage = 0;
    let totalMaxScore = 0;
    this.assignmentVm.questions.forEach(question => {
      const assignmentAnswerTrack = question.getAssignmentAnswerTrack(this.questionAnswerDic[question.id]);

      this.numberCorrectAnswer += assignmentAnswerTrack.giveScore === question.maxScore ? 1 : 0;

      if (assignmentAnswerTrack.giveScore != null) {
        this.totalScore += assignmentAnswerTrack.giveScore;
      }
      totalMaxScore += question.maxScore;
      this.assignmentAnswerTrackDic[assignmentAnswerTrack.questionId] = assignmentAnswerTrack;
    });

    this.scorePercentage = totalMaxScore !== 0 ? Utils.round((this.totalScore / totalMaxScore) * 100, 2) : 0;

    this.originalAssignmentAnswerTrackDic = Utils.cloneDeep(this.assignmentAnswerTrackDic);
  }

  private buildQuestionForLearner(): void {
    if (this.assignmentVm.randomizedQuestions === true && this.submittedDate == null) {
      this.assignmentVm.questions = shuffle(this.assignmentVm.questions);
    }
    this.assignmentVm.questions.forEach(question => {
      if (question.randomizedOptions === true && this.submittedDate == null) {
        question.question_Options = shuffle(question.question_Options);
      }
      const isDateQuestion = question.isDateQuestion();
      const answer = this.questionAnswerDic[question.id];

      question.question_CorrectAnswer =
        answer != null && answer.answerValue != null
          ? isDateQuestion
            ? Array.isArray(answer.answerValue)
              ? answer.answerValue.map(x => (x != null ? new Date(x.toString()) : null))
              : new Date(answer.answerValue.toString())
            : answer.answerValue
          : null;
    });
  }
}
