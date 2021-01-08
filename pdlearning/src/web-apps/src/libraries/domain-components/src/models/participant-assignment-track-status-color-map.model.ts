import { ParticipantAssignmentTrackStatus } from '@opal20/domain-api';

export const PARTICIPANT_ASSIGNMENT_TRACK_STATUS_COLOR_MAP = {
  [ParticipantAssignmentTrackStatus.NotStarted]: {
    text: 'Not Started',
    color: '#D8DCE6'
  },
  [ParticipantAssignmentTrackStatus.InProgress]: {
    text: 'In-Progress',
    color: '#EFDC33'
  },
  [ParticipantAssignmentTrackStatus.Completed]: {
    text: ParticipantAssignmentTrackStatus.Completed,
    color: '#3BDC87'
  },
  [ParticipantAssignmentTrackStatus.Incomplete]: {
    text: ParticipantAssignmentTrackStatus.Incomplete,
    color: '#FF6262'
  },
  [ParticipantAssignmentTrackStatus.IncompletePendingSubmission]: {
    text: 'Incomplete (Pending Submission)',
    color: '#FF6262'
  },
  [ParticipantAssignmentTrackStatus.LateSubmission]: {
    text: 'Late in Submission',
    color: '#58A8F7'
  }
};
