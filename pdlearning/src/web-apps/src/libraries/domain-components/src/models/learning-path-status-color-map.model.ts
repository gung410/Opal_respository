import { LearningPathStatus } from '@opal20/domain-api';

export const LEARNING_PATH_STATUS_COLOR_MAP = {
  [LearningPathStatus.Published]: {
    text: LearningPathStatus.Published,
    color: '#3BDC87'
  },
  [LearningPathStatus.Unpublished]: {
    text: LearningPathStatus.Unpublished,
    color: '#EFDC33'
  }
};
