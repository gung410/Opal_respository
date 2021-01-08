import { BadgeLevel, IBadgeLevel } from './badge-level.model';

export interface IUserBadgeModel {
  badgeId: string;
  issuedBy: string;
  issuedDate: Date;
  badgeLevel: IBadgeLevel;
}

export class UserBadgeModel implements IUserBadgeModel {
  public badgeId: string;
  public issuedBy: string;
  public issuedDate: Date;
  public badgeLevel: BadgeLevel;
  constructor(data?: IUserBadgeModel) {
    if (!data) {
      return;
    }

    this.badgeId = data.badgeId;
    this.issuedBy = data.issuedBy;
    this.issuedDate = data.issuedDate != null ? new Date(data.issuedDate) : null;
    this.badgeLevel = data.badgeLevel != null ? new BadgeLevel(data.badgeLevel) : null;
  }
}

export interface IYearlyUserStatistic {
  id: string;
  userId: string;
  type: YearlyUserStatisticType;
  statistic: UserStatistic;
  awarded: boolean;
  awaredBadges: IUserBadgeModel[];
}
export class YearlyUserStatistic implements IYearlyUserStatistic {
  public id: string;
  public userId: string;
  public type: YearlyUserStatisticType;
  public statistic: UserStatistic;
  public awarded: boolean;
  public awaredBadges: UserBadgeModel[] = [];

  constructor(data?: IYearlyUserStatistic) {
    if (!data) {
      return;
    }
    this.id = data.id;
    this.userId = data.userId;
    this.type = data.type;
    this.statistic = data.statistic;
    this.awarded = data.awarded;
    this.awaredBadges = data.awaredBadges != null ? data.awaredBadges.map(x => new UserBadgeModel(x)) : [];
  }
}

export enum YearlyUserStatisticType {
  LatestDaily,
  LatestMonthly,
  Yearly
}

export interface IUserStatistic {
  executedDate: Date;
  numOfPost: number;
  numOfLikePost: number;
  numOfFollowCommunity: number;
  numOfPostResponding: number;
  numOfForward: number;
  numOfReflection: number;
  numOfSharedReflection: number;
  numOfCompletedMLU: number;
  numOfCompletedDigitalResources: number;
  numOfCompletedElearning: number;
  numOfCreatedLearningPath: number;
  numOfSharedLearningPath: number;
  numOfBookmarkedLearningPath: number;
  numOfCreatedMLU: number;
  total: number;
}

export class UserStatistic implements IUserStatistic {
  public executedDate: Date;

  public numOfPost: number;

  public numOfLikePost: number;

  public numOfFollowCommunity: number;

  public numOfPostResponding: number;

  public numOfForward: number;

  public numOfReflection: number;

  public numOfSharedReflection: number;

  public numOfCompletedMLU: number;

  public numOfCompletedDigitalResources: number;

  public numOfCompletedElearning: number;

  public numOfCreatedLearningPath: number;

  public numOfSharedLearningPath: number;

  public numOfBookmarkedLearningPath: number;

  public numOfCreatedMLU: number;

  public total: number;

  constructor(data?: IUserStatistic) {
    if (data != null) {
      this.executedDate = data.executedDate ? new Date(data.executedDate) : undefined;
      this.numOfPost = data.numOfPost;
      this.numOfLikePost = data.numOfLikePost;
      this.numOfFollowCommunity = data.numOfFollowCommunity;
      this.numOfPostResponding = data.numOfPostResponding;
      this.numOfForward = data.numOfForward;
      this.numOfReflection = data.numOfReflection;
      this.numOfSharedReflection = data.numOfSharedReflection;
      this.numOfCompletedMLU = data.numOfCompletedMLU;
      this.numOfCompletedDigitalResources = data.numOfCompletedDigitalResources;
      this.numOfCompletedElearning = data.numOfCompletedElearning;
      this.numOfCreatedLearningPath = data.numOfCreatedLearningPath;
      this.numOfBookmarkedLearningPath = data.numOfBookmarkedLearningPath;
      this.numOfSharedLearningPath = data.numOfSharedLearningPath;
      this.numOfCreatedMLU = data.numOfCreatedMLU;
      this.total = data.total;
    }
  }
}
