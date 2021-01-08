import { Badge, BadgeId, IYearlyUserStatistic, PublicUserInfo, YearlyUserStatistic } from '@opal20/domain-api';

import { BADGE_LEVEL_MAP } from './badge-level-map.model';

export interface IYearlyUserStatisticViewModel extends IYearlyUserStatistic {
  id: string;
  selected: boolean;
  user: PublicUserInfo;
  allBadgeDic: Dictionary<Badge>;
}

export class YearlyUserStatisticViewModel extends YearlyUserStatistic implements IYearlyUserStatisticViewModel {
  public id: string;
  public selected: boolean;
  public user: PublicUserInfo;
  public allBadgeDic: Dictionary<Badge> = {};
  public static createFromModel(
    yearlyUserStatistic: IYearlyUserStatistic,
    checkAll: boolean,
    selecteds: Dictionary<boolean>,
    user: PublicUserInfo,
    awarded: boolean,
    allBadgeDic: Dictionary<Badge> = {}
  ): YearlyUserStatisticViewModel {
    return new YearlyUserStatisticViewModel({
      ...yearlyUserStatistic,
      selected: checkAll || selecteds[yearlyUserStatistic.id],
      user,
      awarded,
      allBadgeDic
    });
  }

  constructor(data?: IYearlyUserStatisticViewModel) {
    super(data);
    if (!data) {
      return;
    }
    super(data);
    this.id = data.id;
    this.selected = data.selected;
    this.user = data.user ? new PublicUserInfo(data.user) : undefined;
    this.awarded = data.awarded;
    this.allBadgeDic = data.allBadgeDic;
  }

  public get awardedCollaborativeLearnersBadge(): string {
    if (this.awaredBadges == null || this.awaredBadges.length === 0) {
      return 'N/A';
    }

    const awaredBadge = this.awaredBadges.find(x => x.badgeId === BadgeId.CollaborativeLearner);

    return awaredBadge != null ? BADGE_LEVEL_MAP[awaredBadge.badgeLevel.level].text : 'N/A';
  }

  public get awardedDigitalLearnersBadge(): string {
    if (this.awaredBadges == null || this.awaredBadges.length === 0) {
      return 'N/A';
    }

    const awaredBadge = this.awaredBadges.find(x => x.badgeId === BadgeId.DigitalLearner);

    return awaredBadge != null ? BADGE_LEVEL_MAP[awaredBadge.badgeLevel.level].text : 'N/A';
  }

  public get awardedReflectiveLearnersBadge(): string {
    if (this.awaredBadges == null || this.awaredBadges.length === 0) {
      return 'N/A';
    }

    const awaredBadge = this.awaredBadges.find(x => x.badgeId === BadgeId.ReflectiveLearner);

    return awaredBadge != null ? BADGE_LEVEL_MAP[awaredBadge.badgeLevel.level].text : 'N/A';
  }

  public get awardedActiveContributorsBadge(): string {
    if (this.awaredBadges == null || this.awaredBadges.length === 0) {
      return 'N/A';
    }

    const awaredBadge = this.awaredBadges.find(x => x.badgeId === BadgeId.ActiveContributor);

    return awaredBadge != null ? BADGE_LEVEL_MAP[awaredBadge.badgeLevel.level].text : 'N/A';
  }

  public get awardedCommunityBuilderBadgeBadge(): string {
    if (this.awaredBadges == null || this.awaredBadges.length === 0) {
      return 'N/A';
    }

    const awaredBadges = this.awaredBadges.filter(x => x.badgeLevel == null);

    if (awaredBadges == null) {
      return 'N/A';
    }

    const awaredBadgeGroupBy = awaredBadges.reduce((total, awaredBadge) => {
      total[awaredBadge.badgeId] = (total[awaredBadge.badgeId] || 0) + 1;
      return total;
    }, {});

    const result = [];
    Object.keys(awaredBadgeGroupBy).forEach(key => {
      const count = awaredBadgeGroupBy[key] || 0;
      result.push(`${this.allBadgeDic[key].name} (${count} ${count < 2 ? 'badge' : 'badges'})`);
    });

    return []
      .concat(result)
      .filter(p => p != null)
      .join(', ');
  }
}
