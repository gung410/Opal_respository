import { Course, CoursePlanningCycle, ICourse, MetadataTagModel, PublicUserInfo } from '@opal20/domain-api';

import { IGridDataItem } from '@opal20/infrastructure';

export interface ICourseViewModel extends ICourse {
  selected: boolean;
  pdActivityTypeDisplayText: string;
  learningModeDisplayText: string;
  createdBy: string;
  owner: PublicUserInfo;
  archivedByUser: PublicUserInfo;
  coursePlanningCycle?: CoursePlanningCycle;
}

// @dynamic
export class CourseViewModel extends Course implements IGridDataItem {
  public id: string;
  public selected: boolean;
  public pdActivityTypeDisplayText: string;
  public learningModeDisplayText: string;
  public owner: PublicUserInfo;
  public archivedByUser: PublicUserInfo;
  public coursePlanningCycle?: CoursePlanningCycle;
  public static createFromModel(
    course: Course,
    metadataDic: Dictionary<MetadataTagModel>,
    courseOwner: PublicUserInfo,
    archivedByUser: PublicUserInfo,
    checkAll: boolean = false,
    selecteds: Dictionary<boolean> = {},
    coursePlanningCycle?: CoursePlanningCycle
  ): CourseViewModel {
    return new CourseViewModel({
      ...course,
      pdActivityTypeDisplayText: metadataDic[course.pdActivityType] ? metadataDic[course.pdActivityType].displayText : '',
      learningModeDisplayText: metadataDic[course.learningMode] ? metadataDic[course.learningMode].displayText : '',
      selected: checkAll || selecteds[course.id],
      owner: courseOwner,
      archivedByUser: archivedByUser,
      coursePlanningCycle: coursePlanningCycle
    });
  }

  constructor(data?: ICourseViewModel) {
    super(data);
    if (data != null) {
      this.selected = data.selected;
      this.learningModeDisplayText = data.learningModeDisplayText;
      this.pdActivityTypeDisplayText = data.pdActivityTypeDisplayText;
      this.owner = data.owner ? new PublicUserInfo(data.owner) : undefined;
      this.archivedByUser = data.archivedByUser ? new PublicUserInfo(data.archivedByUser) : undefined;
      this.coursePlanningCycle = data.coursePlanningCycle ? new CoursePlanningCycle(data.coursePlanningCycle) : null;
    }
  }

  public get yearCycle(): number | null {
    return this.coursePlanningCycle ? this.coursePlanningCycle.yearCycle : null;
  }
}
