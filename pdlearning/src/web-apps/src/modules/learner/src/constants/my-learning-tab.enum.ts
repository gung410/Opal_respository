import {
  CommunitySearchLearningType,
  CourseSearchLearningType,
  LearningPathSearchLearningType,
  SearchFilterModel
} from '../models/my-learning-search-filter-model';
import { LearnerRoutePaths, MyLearningTab } from '@opal20/domain-components';

import { LearningCourseType } from '@opal20/domain-api';

export const MY_LEARNING_TAB_NAMES = new Map<MyLearningTab, string>([
  [MyLearningTab.Courses, 'Courses'],
  [MyLearningTab.Microlearning, 'Microlearning Units'],
  [MyLearningTab.DigitalContent, 'Digital Content'],
  [MyLearningTab.LearningPaths, 'Learning Paths'],
  [MyLearningTab.Community, 'Communities'],
  [MyLearningTab.Bookmarks, 'Bookmarks']
]);

export const SEARCH_PLACEHOLDER_TAB = new Map<MyLearningTab, string>([
  [MyLearningTab.Courses, 'Courses'],
  [MyLearningTab.Microlearning, 'Microlearning'],
  [MyLearningTab.DigitalContent, 'Digital Content'],
  [MyLearningTab.LearningPaths, 'Learning Paths'],
  [MyLearningTab.Community, 'Communities'],
  [MyLearningTab.Bookmarks, 'Bookmarks']
]);

export const SEARCH_FILTER_TYPE_INITIALIZE = new Map<MyLearningTab, SearchFilterModel>([
  [
    MyLearningTab.Courses,
    new SearchFilterModel({
      type: LearningCourseType.FaceToFace,
      learningTypes: [
        CourseSearchLearningType.Registered,
        CourseSearchLearningType.Upcoming,
        CourseSearchLearningType.InProgress,
        CourseSearchLearningType.Completed
      ],
      learningTypeFilter: CourseSearchLearningType.Registered,
      navigateBack: `${LearnerRoutePaths.MyLearning}/${MyLearningTab.Courses}`
    })
  ],
  [
    MyLearningTab.Microlearning,
    new SearchFilterModel({
      type: LearningCourseType.Microlearning,
      learningTypes: [CourseSearchLearningType.InProgress, CourseSearchLearningType.Completed],
      learningTypeFilter: CourseSearchLearningType.InProgress,
      navigateBack: `${LearnerRoutePaths.MyLearning}/${MyLearningTab.Microlearning}`
    })
  ],
  [
    MyLearningTab.DigitalContent,
    new SearchFilterModel({
      type: 'DigitalContent',
      learningTypes: [CourseSearchLearningType.InProgress, CourseSearchLearningType.Completed],
      learningTypeFilter: CourseSearchLearningType.InProgress,
      navigateBack: `${LearnerRoutePaths.MyLearning}/${MyLearningTab.DigitalContent}`
    })
  ],
  [
    MyLearningTab.Community,
    new SearchFilterModel({
      type: 'Community',
      learningTypes: [CommunitySearchLearningType.Joined, CommunitySearchLearningType.Owned],
      learningTypeFilter: CommunitySearchLearningType.Joined,
      navigateBack: `${LearnerRoutePaths.MyLearning}/${MyLearningTab.Community}`
    })
  ],
  [
    MyLearningTab.LearningPaths,
    new SearchFilterModel({
      type: 'LearningPath',
      learningTypes: [
        LearningPathSearchLearningType.Owned,
        LearningPathSearchLearningType.Shared,
        LearningPathSearchLearningType.Recommended
      ],
      learningTypeFilter: LearningPathSearchLearningType.Owned,
      navigateBack: `${LearnerRoutePaths.MyLearning}/${MyLearningTab.LearningPaths}`
    })
  ]
]);
