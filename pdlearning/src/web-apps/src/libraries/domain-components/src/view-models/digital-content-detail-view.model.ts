import { DigitalContent, DigitalContentStatus, VideoChapter } from '@opal20/domain-api';

import { Utils } from '@opal20/infrastructure';

export class DigitalContentDetailViewModel {
  public data: DigitalContent = new DigitalContent();
  public originalData: DigitalContent = new DigitalContent();

  public get id(): string {
    return this.data.id;
  }
  public set id(value: string) {
    this.data.title = value;
  }

  public get originalObjectId(): string {
    return this.data.originalObjectId;
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

  public set description(value: string) {
    this.data.description = value;
  }

  public get type(): string {
    return this.data.type;
  }

  public set type(type: string) {
    this.data.type = type;
  }

  public get status(): DigitalContentStatus {
    return this.data.status;
  }

  public get archiveDate(): Date {
    return this.data.archiveDate ? new Date(this.data.archiveDate) : null;
  }

  public set archiveDate(value: Date) {
    this.data.archiveDate = value;
  }

  public set status(value: DigitalContentStatus) {
    this.data.status = value;
  }

  public get htmlContent(): string {
    return this.data.htmlContent || '';
  }

  public set htmlContent(value: string) {
    this.data.htmlContent = value;
  }

  public get primaryApprovingOfficerId(): string {
    return this.data.primaryApprovingOfficerId;
  }
  public set primaryApprovingOfficerId(primaryApprovingOfficerId: string) {
    this.data.primaryApprovingOfficerId = primaryApprovingOfficerId;
  }
  public get alternativeApprovingOfficerId(): string {
    return this.data.alternativeApprovingOfficerId;
  }
  public set alternativeApprovingOfficerId(alternativeApprovingOfficerId: string) {
    this.data.alternativeApprovingOfficerId = alternativeApprovingOfficerId;
  }

  public get fileExtension(): string {
    return this.data.fileExtension;
  }

  public set fileExtension(v: string) {
    this.data.fileExtension = v;
  }

  public get fileType(): string {
    return this.data.fileType;
  }

  public set fileType(v: string) {
    this.data.fileType = v;
  }

  public get fileLocation(): string {
    return this.data.fileLocation;
  }

  public set fileLocation(v: string) {
    this.data.fileLocation = v;
  }

  public get chapters(): VideoChapter[] {
    return this.data.chapters;
  }

  public set chapters(v: VideoChapter[]) {
    this.data.chapters = v;
  }

  public get ownerId(): string {
    return this.data.ownerId;
  }

  public set ownerId(v: string) {
    this.data.ownerId = v;
  }

  public get autoPublishDate(): Date {
    return this.data.autoPublishDate ? new Date(this.data.autoPublishDate) : null;
  }

  public set autoPublishDate(value: Date) {
    this.data.autoPublishDate = value;
  }

  public get isAutoPublish(): boolean {
    return this.data.isAutoPublish;
  }

  public set isAutoPublish(value: boolean) {
    this.data.isAutoPublish = value;
  }

  /**
   * This will be undefined when the video HTML5 loaded
   */
  public get fileDuration(): number {
    return this.data.fileDuration;
  }

  public set fileDuration(v: number) {
    this.data.fileDuration = v;
  }

  constructor(digitalContent: DigitalContent) {
    if (digitalContent) {
      this.updatedata(digitalContent);
    }
  }

  public updatedata(digitalContent: DigitalContent): void {
    this.originalData = Utils.cloneDeep(digitalContent);
    this.data = Utils.cloneDeep(digitalContent);
  }
}
