import {
  ILectureQuizConfigModel,
  LectureDigitalContentConfigModel,
  LectureModel,
  LectureQuizConfigModel,
  LectureType
} from '@opal20/domain-api';

import { Utils } from '@opal20/infrastructure';

export class LectureContentViewModel {
  public data: LectureModel = new LectureModel();
  public originalData: LectureModel = new LectureModel();

  constructor(lecture?: LectureModel) {
    if (lecture) {
      this.updateData(lecture);
    }
  }

  public get id(): string {
    return this.data.id;
  }
  public set id(value: string) {
    this.data.id = value;
  }

  public get lectureIcon(): string {
    return this.data.icon;
  }
  public set lectureIcon(lectureIcon: string) {
    this.data.icon = lectureIcon;
  }

  public get title(): string {
    return this.data.title;
  }

  public set title(title: string) {
    this.data.title = title;
  }

  public get description(): string {
    return this.data.description;
  }

  public set description(description: string) {
    this.data.description = description;
  }

  public get type(): LectureType {
    return this.data.type;
  }

  public set type(type: LectureType) {
    this.data.type = type;
  }

  public get order(): number {
    return this.data.order;
  }

  public set order(order: number) {
    this.data.order = order;
  }

  public get sectionId(): string {
    return this.data.sectionId;
  }
  public set sectionId(sectionId: string) {
    this.data.sectionId = sectionId;
  }

  public get courseId(): string {
    return this.data.courseId;
  }
  public set courseId(courseId: string) {
    this.data.courseId = courseId;
  }

  public get value(): string {
    return this.data.value;
  }

  public set value(value: string) {
    this.data.value = value;
  }

  public get mimeType(): string {
    return this.data.mimeType;
  }

  public set mimeType(mimeType: string) {
    this.data.mimeType = mimeType;
  }

  public get resourceId(): string {
    return this.data.resourceId;
  }

  public set resourceId(resourceId: string) {
    this.data.resourceId = resourceId;
  }

  public get classRunId(): string {
    return this.data.classRunId;
  }

  public set classRunId(classRunId: string) {
    this.data.classRunId = classRunId;
  }

  public get base64Value(): string {
    return this.data.value != null ? Utils.encodeBase64_2(this.data.value) : '';
  }

  public get quizConfig(): ILectureQuizConfigModel {
    return this.data.quizConfig;
  }

  public set quizConfig(v: ILectureQuizConfigModel) {
    this.data.quizConfig = v;
  }

  public get quizConfig_isByPass(): boolean | null {
    return this.data.quizConfig ? this.data.quizConfig.byPassPassingRate : null;
  }
  public set quizConfig_isByPass(v: boolean | null) {
    if (v == null) {
      return;
    }
    if (this.data.quizConfig == null) {
      this.data.quizConfig = new LectureQuizConfigModel();
    }
    this.data.quizConfig.byPassPassingRate = v != null ? v : true;
  }

  public get quizConfig_isDisplayPollResult(): boolean | null {
    return this.data.quizConfig ? this.data.quizConfig.displayPollResultToLearners : null;
  }
  public set quizConfig_isDisplayPollResult(v: boolean | null) {
    if (v == null) {
      return;
    }
    if (this.data.quizConfig == null) {
      this.data.quizConfig = new LectureQuizConfigModel();
    }
    this.data.quizConfig.displayPollResultToLearners = v != null ? v : true;
  }

  public get digitalContentConfig(): LectureDigitalContentConfigModel {
    return this.data.digitalContentConfig;
  }
  public set digitalContentConfig(v: LectureDigitalContentConfigModel) {
    this.data.digitalContentConfig = v;
  }

  public get digitalContentConfig_canDownload(): boolean {
    return this.data.digitalContentConfig ? this.data.digitalContentConfig.canDownload : null;
  }
  public set digitalContentConfig_canDownload(v: boolean) {
    if (v == null) {
      return;
    }
    if (this.data.digitalContentConfig == null) {
      this.data.digitalContentConfig = new LectureDigitalContentConfigModel();
    }
    this.data.digitalContentConfig.canDownload = v != null ? v : true;
  }

  public updateData(lecture: LectureModel): void {
    this.originalData = Utils.cloneDeep(lecture);
    this.data = Utils.cloneDeep(lecture);
  }

  public resetData(): void {
    this.data = Utils.cloneDeep(this.originalData);
  }

  public hasDataChanged(): boolean {
    return Utils.isDifferent(this.originalData, this.data);
  }

  public isQuiz(): boolean {
    return this.originalData.type === LectureType.Quiz;
  }

  public isDigitalContent(): boolean {
    return this.originalData.type === LectureType.DigitalContent;
  }

  public isInline(): boolean {
    return this.originalData.type === LectureType.InlineContent;
  }

  public isCheckPassingRate(): boolean {
    return this.quizConfig_isByPass != null ? this.quizConfig_isByPass : true;
  }
}
