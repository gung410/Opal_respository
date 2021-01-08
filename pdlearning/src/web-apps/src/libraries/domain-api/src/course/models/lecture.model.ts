import { Guid } from '@opal20/infrastructure';
import { LMM_PERMISSIONS } from '@opal20/domain-api/share/permission-keys/lmm-permission-key';
import { UserInfoModel } from '@opal20/domain-api';

export interface ILectureModel {
  id?: string;
  icon: string;
  title: string;
  description: string;
  type: LectureType;
  order: number;
  sectionId?: string;
  courseId: string;
  value?: string;
  mimeType?: string;
  resourceId?: string;
  classRunId: string | null;
  quizConfig?: ILectureQuizConfigModel;
  digitalContentConfig?: ILectureDigitalContentConfigModel;
}

export class LectureModel implements ILectureModel {
  public id?: string;
  public icon: string = '';
  public title: string = '';
  public description: string = '';
  public type: LectureType = LectureType.InlineContent;
  public order: number = 0;
  public sectionId?: string;
  public courseId: string = '';
  public value?: string;
  public mimeType?: string;
  public resourceId?: string;
  public classRunId: string | null;
  public quizConfig?: LectureQuizConfigModel;
  public digitalContentConfig?: LectureDigitalContentConfigModel;

  public static hasViewQuizStatisticsPermission(user: UserInfoModel): boolean {
    return user.hasPermissionPrefix(LMM_PERMISSIONS.ViewQuizStatistics);
  }

  constructor(data?: ILectureModel) {
    if (data) {
      this.id = data.id;
      this.icon = data.icon;
      this.title = data.title;
      this.description = data.description;
      this.type = data.type;
      this.order = data.order;
      this.sectionId = data.sectionId;
      this.courseId = data.courseId;
      this.value = data.value;
      this.mimeType = data.mimeType;
      this.resourceId = data.resourceId;
      this.classRunId = data.classRunId;
      this.quizConfig = data.quizConfig ? new LectureQuizConfigModel(data.quizConfig) : null;
      this.digitalContentConfig = data.digitalContentConfig ? new LectureDigitalContentConfigModel(data.digitalContentConfig) : null;
    }
  }

  public hasResourceId(): boolean {
    return this.resourceId != null && this.resourceId !== Guid.EMPTY;
  }

  public hasAllowDownloadCourseContentPermission(user: UserInfoModel): boolean {
    return user.hasPermissionPrefix(LMM_PERMISSIONS.AllowDownloadCourseContent);
  }
}

export enum LectureType {
  InlineContent = 'InlineContent',
  DigitalContent = 'DigitalContent',
  Quiz = 'Quiz'
}

export interface ILectureQuizConfigModel {
  byPassPassingRate: boolean;
  displayPollResultToLearners: boolean;
}
export class LectureQuizConfigModel implements ILectureQuizConfigModel {
  public byPassPassingRate: boolean = true;
  public displayPollResultToLearners: boolean = true;

  constructor(data?: Partial<ILectureQuizConfigModel>) {
    if (data) {
      this.byPassPassingRate = data.byPassPassingRate != null ? data.byPassPassingRate : this.byPassPassingRate;
      this.displayPollResultToLearners =
        data.displayPollResultToLearners != null ? data.displayPollResultToLearners : this.displayPollResultToLearners;
    }
  }
}

export interface ILectureDigitalContentConfigModel {
  canDownload: boolean;
}
export class LectureDigitalContentConfigModel implements ILectureDigitalContentConfigModel {
  public canDownload: boolean = false;

  constructor(data?: ILectureDigitalContentConfigModel) {
    if (data) {
      this.canDownload = data.canDownload;
    }
  }
}
