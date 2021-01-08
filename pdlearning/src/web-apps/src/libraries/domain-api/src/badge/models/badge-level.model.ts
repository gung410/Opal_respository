export enum BadgeLevelEnum {
  Unknown = 'Unknown',
  Level1 = 'Level1',
  Level2 = 'Level2',
  Level3 = 'Level3'
}

export interface IBadgeLevel {
  level: BadgeLevelEnum;
  increaseLevelDate?: Date;
}

export class BadgeLevel implements IBadgeLevel {
  public level: BadgeLevelEnum;
  public increaseLevelDate?: Date;

  constructor(data?: IBadgeLevel) {
    if (!data) {
      return;
    }

    this.level = data.level;
    this.increaseLevelDate = data.increaseLevelDate != null ? new Date(data.increaseLevelDate) : null;
  }
}
