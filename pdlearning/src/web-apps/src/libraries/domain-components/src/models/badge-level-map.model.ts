import { BadgeLevelEnum } from '@opal20/domain-api';

export const BADGE_LEVEL_MAP: Dictionary<{ text: string }> = {
  [BadgeLevelEnum.Unknown]: {
    text: 'Unknown'
  },
  [BadgeLevelEnum.Level1]: {
    text: 'Level 1'
  },
  [BadgeLevelEnum.Level2]: {
    text: 'Level 2'
  },
  [BadgeLevelEnum.Level3]: {
    text: 'Level 3'
  }
};
