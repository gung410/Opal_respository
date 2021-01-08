import { ILectureDigitalContentConfigModel, ILectureQuizConfigModel, LectureDigitalContentConfigModel, LectureQuizConfigModel } from '../../course/models/lecture.model';

export interface ILectureDetailModel {
  id: string;
  sectionId?: string | undefined;
  title: string;
  icon?: string | undefined;
  description?: string | undefined;
  type?: LectureContentType | undefined;
  status?: LectureStatus | undefined;
  value?: string | undefined;
  resourceId?: string | undefined;
  mimeType?: string | undefined;
  quizConfig?: ILectureQuizConfigModel;
  digitalContentConfig?: ILectureDigitalContentConfigModel;
}

export enum LectureStatus {
  Draft = 'Draft',
  Published = 'Published'
}

export class LectureDetailModel implements ILectureDetailModel {
  public static videoMimeTypes: string[] = ['video/mp4', 'video/youtube'];

  public id: string;
  public sectionId?: string;
  public title: string;
  public icon?: string;
  public description?: string;
  public type?: LectureContentType;
  public status?: LectureStatus;
  public value?: string;
  public resourceId?: string;
  public mimeType?: string;
  public quizConfig?: LectureQuizConfigModel;
  public digitalContentConfig?: LectureDigitalContentConfigModel;

  constructor(data?: ILectureDetailModel) {
    if (data == null) {
      return;
    }
    (this.id = data.id), (this.title = data.title);
    this.icon = data.icon;
    this.description = data.description;
    this.type = data.type;
    this.value = data.value;
    this.resourceId = data.resourceId;
    this.mimeType = data.mimeType;
    this.quizConfig = data.quizConfig ? new LectureQuizConfigModel(data.quizConfig) : undefined;
    this.digitalContentConfig = data.digitalContentConfig ? new LectureDigitalContentConfigModel(data.digitalContentConfig) : undefined;
  }

  /* default is enabled */
  public get enablePassingRate(): boolean {
    return this.quizConfig == null || this.quizConfig.byPassPassingRate === false;
  }
}

export enum LectureContentType {
  InlineContent = 'InlineContent',
  DigitalContent = 'DigitalContent',
  Quiz = 'Quiz',
  Url = 'Url'
}
