export interface SurveySubmitEventData {
  options: {
    allowComplete: boolean;
  };
  survey: {
    data: object;
  };
}

export interface SurveyFormJSON {
  pages: object;
  showQuestionNumbers?: string;
  mode?: string;
}
