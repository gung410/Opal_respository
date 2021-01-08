import { BaseBadgeCriteria, IBaseBadgeCriteria } from './badge.model';

import { BadgeLevelEnum } from './badge-level.model';

export interface IActiveContributorsBadgeCriteria extends IBaseBadgeCriteria {
  levelOfCollaborativeLearnersBadge?: BadgeLevelEnum;
  levelOfDigitalLearnersBadge?: BadgeLevelEnum;
  levelOfReflectiveLearnersBadge?: BadgeLevelEnum;
  communityBadgesIds: string[];
  executeMonth: number;
}

export class ActiveContributorsBadgeCriteria extends BaseBadgeCriteria implements IActiveContributorsBadgeCriteria {
  public levelOfCollaborativeLearnersBadge?: BadgeLevelEnum;
  public levelOfDigitalLearnersBadge?: BadgeLevelEnum;
  public levelOfReflectiveLearnersBadge?: BadgeLevelEnum;
  public communityBadgesIds: string[] = [];
  public executeMonth: number = new Date().getMonth() + 2; // Javascript month number begin from 0 to 11. ExecuteMoth must be next month.

  constructor(data?: IActiveContributorsBadgeCriteria) {
    super(data);
    if (!data) {
      return;
    }

    this.levelOfCollaborativeLearnersBadge = data.levelOfCollaborativeLearnersBadge;
    this.levelOfDigitalLearnersBadge = data.levelOfDigitalLearnersBadge;
    this.levelOfReflectiveLearnersBadge = data.levelOfReflectiveLearnersBadge;
    this.communityBadgesIds = data.communityBadgesIds ? data.communityBadgesIds : [];
    this.executeMonth = this.executeMonth;
  }
}
