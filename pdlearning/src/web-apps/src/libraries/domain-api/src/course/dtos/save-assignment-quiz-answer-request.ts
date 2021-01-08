export interface ISaveAssignmentQuizAnswerRequest {
  registrationId: string;
  assignmentId: string;
  questionAnswers: ISaveAssignmentQuizAnswerRequestQuestionAnswer[];
  isSubmit: boolean;
}

export interface ISaveAssignmentQuizAnswerRequestQuestionAnswer {
  quizAssignmentFormQuestionId: string;
  answerValue: unknown;
}
