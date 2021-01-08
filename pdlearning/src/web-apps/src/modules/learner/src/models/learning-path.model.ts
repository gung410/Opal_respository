import {
  BookmarkInfoModel,
  ISearchFilterResultModel,
  LearningPathModel as LMMLearningPathModel,
  LearnerLearningPath,
  LearnerLearningPathCourse,
  LearningPathSearchFilterResultModel,
  Statistic,
  UserInfoModel
} from '@opal20/domain-api';
import { ILearningItemModel, LearningType } from './learning-item.model';

export interface ILearnerLearningPathModel extends ILearningItemModel {
  id: string;
  title: string;
  imageUrl?: string | undefined;
  isBookmark: boolean;
  numberOfItems: number;
  courses: LearnerLearningPathCourse[];
  type: LearningType;
  bookmarkInfo?: BookmarkInfoModel | undefined;
  isOwner: boolean;
  isPublic: boolean;
  fromLMM?: boolean;
}

export class LearnerLearningPathModel implements ILearnerLearningPathModel {
  public id: string;
  public title: string;
  public imageUrl?: string | undefined;
  public isBookmark: boolean;
  public numberOfItems: number;
  public courses: LearnerLearningPathCourse[];
  public type: LearningType = LearningType.LearningPath;
  public bookmarkInfo?: BookmarkInfoModel | undefined;
  public isOwner: boolean;
  public isPublic: boolean = false;
  public fromLMM?: boolean = false;
  public static createByMyLearningPath(data: LearnerLearningPath): LearnerLearningPathModel {
    if (!data) {
      return;
    }
    return new LearnerLearningPathModel({
      id: data.id,
      title: data.title,
      imageUrl: data.thumbnailUrl,
      numberOfItems: data.courses ? data.courses.length : 0,
      isBookmark: data.bookmarkInfo !== undefined,
      bookmarkInfo: data.bookmarkInfo,
      courses: data.courses,
      type: LearningType.LearningPath,
      isOwner: data.createdBy === UserInfoModel.getMyUserInfo().id,
      isPublic: data.isPublic
    });
  }

  public static convertLearningPathFromLMM(data: LMMLearningPathModel, bookmark?: BookmarkInfoModel): LearnerLearningPathModel {
    if (!data) {
      return;
    }
    let courses = [];
    if (data.listCourses) {
      courses = data.listCourses.map(p => {
        const item = new LearnerLearningPathCourse();
        item.id = p.id;
        item.courseId = p.courseId;
        item.learningPathId = p.id;
        item.order = p.order;
        return item;
      });
    }
    return new LearnerLearningPathModel({
      id: data.id,
      title: data.title,
      imageUrl: data.thumbnailUrl,
      numberOfItems: courses.length,
      isBookmark: bookmark !== undefined,
      bookmarkInfo: bookmark,
      courses: courses,
      type: LearningType.LearningPathLMM,
      isOwner: data.createdBy === UserInfoModel.getMyUserInfo().id,
      isPublic: false,
      fromLMM: true
    });
  }

  constructor(data?: ILearnerLearningPathModel) {
    if (!data) {
      return;
    }
    this.id = data.id;
    this.title = data.title;
    this.imageUrl = data.imageUrl;
    this.isBookmark = data.isBookmark;
    this.numberOfItems = data.numberOfItems;
    this.courses = data.courses;
    this.type = data.type;
    this.bookmarkInfo = data.bookmarkInfo;
    this.isOwner = data.isOwner;
    this.isPublic = data.isPublic;
    this.fromLMM = data.fromLMM ? data.fromLMM : false;
  }
}

export class LearnerLearningPathSearchFilterResultModel implements ISearchFilterResultModel<ILearnerLearningPathModel> {
  public statistics: Statistic[] = [];
  public totalCount: number;
  public items: LearnerLearningPathModel[] = [];

  public static createLearningPathSearchFilterResultModel(
    data: LearningPathSearchFilterResultModel
  ): LearnerLearningPathSearchFilterResultModel {
    return new LearnerLearningPathSearchFilterResultModel({
      totalCount: data.totalCount,
      statistics: data.statistics,
      items: data.items ? data.items.map(_ => LearnerLearningPathModel.createByMyLearningPath(_)) : []
    });
  }

  constructor(data?: ISearchFilterResultModel<LearnerLearningPathModel>) {
    if (data == null) {
      return;
    }
    this.statistics = data.statistics ? data.statistics : [];
    this.totalCount = data.totalCount ? data.totalCount : 0;
    this.items = data.items ? data.items : [];
  }
}
