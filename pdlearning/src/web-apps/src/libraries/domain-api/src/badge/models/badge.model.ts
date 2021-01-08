import { BadgeType } from './badge-type.model';
import { MAX_INT } from '@opal20/infrastructure';
import { RewardBadgeLimitType } from './reward-badge-limit-type.model';

export enum BadgeId {
  CollaborativeLearner = 'b747e01e-2c53-49c0-8787-1f1018c6462c',
  DigitalLearner = '449ead06-86c6-4310-8faa-32f34bb536cf',
  ReflectiveLearner = '02ae2b78-4434-11eb-b378-0242ac130002',
  ActiveContributor = 'da247ab1-5a7a-4486-81d1-913bc33c37c7',
  LifeLong = '3d58e626-517d-4e7d-9e57-f79689d87ac7'
}

export interface IBadge {
  id: string;
  type: BadgeType;
  name: string;
  images: Dictionary<string>;
  seedVersion: number;
}

export class Badge implements IBadge {
  public id: string;
  public type: BadgeType;
  public name: string;
  public images: Dictionary<string> = {};
  public seedVersion: number;

  constructor(data?: IBadge) {
    if (!data) {
      return;
    }

    this.id = data.id;
    this.type = data.type;
    this.name = data.name;
    this.images = data.images;
    this.seedVersion = data.seedVersion;
  }
}

export interface IRewardBadgeLimitation {
  limitType: RewardBadgeLimitType;
  maximumIsusedPeople: number;
  limitValues: Dictionary<number>;
}

export class RewardBadgeLimitation implements IRewardBadgeLimitation {
  public limitType: RewardBadgeLimitType;
  public maximumIsusedPeople: number = MAX_INT;
  public limitValues: Dictionary<number> = {};

  constructor(data?: IRewardBadgeLimitation) {
    if (!data) {
      return;
    }

    this.limitType = data.limitType;
    this.maximumIsusedPeople = data.maximumIsusedPeople;
    this.limitValues = data.limitValues;
  }
}

export interface IBaseBadgeCriteria {
  limitation: RewardBadgeLimitation;
}

export class BaseBadgeCriteria implements IBaseBadgeCriteria {
  public limitation: RewardBadgeLimitation = new RewardBadgeLimitation();

  constructor(data?: IBaseBadgeCriteria) {
    if (!data) {
      return;
    }

    this.limitation = data.limitation ? data.limitation : new RewardBadgeLimitation();
  }
}

// tslint:disable-next-line:no-any
export interface IBadgeWithCriteria<T extends BaseBadgeCriteria | unknown> extends IBadge {
  criteria: T;
}

// tslint:disable-next-line:no-any
export class BadgeWithCriteria<T extends BaseBadgeCriteria | unknown> extends Badge implements IBadgeWithCriteria<T> {
  public criteria: T = {} as T;

  constructor(data?: IBadgeWithCriteria<T>) {
    super(data);
    if (!data) {
      return;
    }

    this.criteria = data.criteria ? data.criteria : ({} as T);
  }
}
