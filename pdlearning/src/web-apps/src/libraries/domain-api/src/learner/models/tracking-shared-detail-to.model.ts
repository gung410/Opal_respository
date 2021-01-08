import { Course } from '../../course/models/course.model';

export interface ITrackingSharedDetailByModel {
  itemId: string;
  itemType: string;
  title: string;
  sharedByUsers: string[];
  thumbnailUrl: string;
  tagIds: string[];
  skipCount: number;
  maxResultCount: number;
}

export class TrackingSharedDetailByModel implements ITrackingSharedDetailByModel {
  public itemId: string;
  public itemType: string;
  public title: string;
  public sharedByUsers: string[];
  public tagIds: string[];
  public thumbnailUrl: string;
  public skipCount: number;
  public maxResultCount: number;

  // This function following buildTags at learning-item.model
  public static buildTags(courseDetail: Course): string[] {
    const pdActivityType = courseDetail.pdActivityType;
    const learningMode = courseDetail.learningMode;
    const subjectArea = courseDetail.subjectAreaIds && courseDetail.subjectAreaIds[0];
    const tagIds = [];

    if (pdActivityType) {
      tagIds.push(pdActivityType);
    }
    if (learningMode) {
      tagIds.push(learningMode);
    }
    if (subjectArea) {
      tagIds.push(subjectArea);
    }
    return tagIds;
  }

  constructor(data?: ITrackingSharedDetailByModel) {
    if (data) {
      this.itemId = data.itemId;
      this.itemType = data.itemType;
      this.title = data.title;
      this.sharedByUsers = data.sharedByUsers;
      this.thumbnailUrl = data.thumbnailUrl;
      this.tagIds = data.tagIds;
      this.skipCount = data.skipCount;
      this.maxResultCount = data.maxResultCount;
    }
  }
}
