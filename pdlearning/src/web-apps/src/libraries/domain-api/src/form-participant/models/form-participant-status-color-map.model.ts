import { FormParticipantStatus } from './form-participant-status.enum';

export const FORM_PARTICIPANT_STATUS_COLOR_MAP = {
  [FormParticipantStatus.NotStarted]: {
    text: 'Not Started',
    color: '#D8DCE6'
  },
  [FormParticipantStatus.Completed]: {
    text: FormParticipantStatus.Completed,
    color: '#3BDC87'
  },
  [FormParticipantStatus.Incomplete]: {
    text: FormParticipantStatus.Incomplete,
    color: '#FF6262'
  }
};
