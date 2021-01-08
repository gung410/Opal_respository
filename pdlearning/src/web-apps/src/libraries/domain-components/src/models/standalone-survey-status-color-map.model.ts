import { SurveyStatus } from '@opal20/domain-api';

export const STANDALONE_SURVEY_STATUS_COLOR_MAP = {
  [SurveyStatus.Draft]: {
    text: 'Draft',
    color: '#D8DCE6'
  },
  [SurveyStatus.Published]: {
    text: 'Published',
    color: '#3BDC87'
  },
  [SurveyStatus.Unpublished]: {
    text: 'Unpublished',
    color: '#EFDC33'
  },
  [SurveyStatus.Archived]: {
    text: 'Archived',
    color: '#FF6262'
  }
};
