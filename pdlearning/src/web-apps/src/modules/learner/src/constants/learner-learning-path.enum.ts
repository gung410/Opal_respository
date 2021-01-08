export enum LearningPathSectionEnum {
  MyOwn = 'MyOwn',
  SharedToMe = 'SharedToMe',
  RecommendForYou = 'RecommendForYou'
}

export const LEARNER_LEARNING_PATH_SECTION = new Map<LearningPathSectionEnum, string>([
  [LearningPathSectionEnum.MyOwn, 'My own learning paths'],
  [LearningPathSectionEnum.SharedToMe, 'Shared learning paths for you'],
  [LearningPathSectionEnum.RecommendForYou, 'Recommendations for You']
]);
