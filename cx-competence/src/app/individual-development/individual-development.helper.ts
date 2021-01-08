import { IDPTabsMenuEnum } from './idp.constant';

export class IndividualDevelopmentHelper {
  static getTabIndexByEnum(tabEnum: string, tabEnumMapping: any): number {
    if (!tabEnum) {
      return 0;
    }

    let tabIndex = 0;

    for (const key in tabEnumMapping) {
      if (Object.prototype.hasOwnProperty.call(tabEnumMapping, key)) {
        if (tabEnum === key) {
          break;
        }

        if (tabEnumMapping[key]) {
          tabIndex++;
        }
      }
    }

    return tabIndex;
  }

  static getTabIdByTabIndex(targetIndex: number, tabEnumMapping: any): string {
    if (targetIndex === undefined || targetIndex < 0) {
      return IDPTabsMenuEnum.LearningNeedAnalysis;
    }

    let currentIndex = 0;
    for (const key in tabEnumMapping) {
      if (Object.prototype.hasOwnProperty.call(tabEnumMapping, key)) {
        if (!tabEnumMapping[key]) {
          continue;
        }

        if (currentIndex === targetIndex) {
          return key;
        }

        currentIndex++;
      }
    }

    return IDPTabsMenuEnum.LearningNeedAnalysis;
  }
}
