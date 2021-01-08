import { BaseUserInfo, MetadataTagGroupCode, MetadataTagModel, SendAnnouncemmentEmailTemplateModel } from '@opal20/domain-api';

import { SendCoursePublicityOption } from '../models/send-course-publicity-option.model';
import { Utils } from '@opal20/infrastructure';

export class SendCoursePublicityViewModel {
  public userItems: BaseUserInfo[] = [];
  public metadataTagsDic: Dictionary<MetadataTagModel> = {};
  public metadataTagsDicByGroupCode: Dictionary<MetadataTagModel[]> = {};
  private _option: SendCoursePublicityOption = SendCoursePublicityOption.SpecificTargetAudience;
  private _teachingLevels: string[] = [];
  private _teachingSubjectIds: string[] = [];
  private _users: string[] = [];
  private _mailContent: string = '';
  private _teachingLevelsSelectItems: MetadataTagModel[] = [];
  private _teachingSubjectSelectItems: MetadataTagModel[] = [];

  constructor(
    public defaultTemplate: SendAnnouncemmentEmailTemplateModel = new SendAnnouncemmentEmailTemplateModel(),
    public metadataTags: MetadataTagModel[] = []
  ) {
    this._mailContent = defaultTemplate.template;
    this.setMetadataTagsDicInfo(metadataTags);
    this._teachingLevelsSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.TEACHING_LEVELS], []);
    this._teachingSubjectSelectItems = Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.TEACHING_SUBJECTS], []);
  }
  public get option(): SendCoursePublicityOption {
    return this._option;
  }
  public set option(option: SendCoursePublicityOption) {
    this._option = option;
  }

  public get users(): string[] {
    return this._users;
  }
  public set users(users: string[]) {
    this._users = users;
  }

  public get mailContent(): string {
    return this._mailContent;
  }
  public set mailContent(mailContent: string) {
    this._mailContent = mailContent;
  }

  public get teachingLevels(): string[] {
    return this._teachingLevels;
  }
  public set teachingLevels(teachingLevels: string[]) {
    this._teachingLevels = teachingLevels;
  }

  public get teachingSubjectIds(): string[] {
    return this._teachingSubjectIds;
  }
  public set teachingSubjectIds(teachingSubjectIds: string[]) {
    this._teachingSubjectIds = teachingSubjectIds;
  }

  public setOption(value: SendCoursePublicityOption): void {
    this.option = value;

    if (this.option === SendCoursePublicityOption.AllUsers) {
      this.users = [];
      this.teachingLevels = [];
      this.teachingSubjectIds = [];
    }
  }

  public get teachingLevelsSelectItems(): MetadataTagModel[] {
    return this._teachingLevelsSelectItems;
  }
  public set teachingLevelsSelectItems(v: MetadataTagModel[]) {
    this._teachingLevelsSelectItems = v;
    this.teachingLevels = Utils.rightJoin(this.teachingLevels, this.teachingLevelsSelectItems.map(p => p.tagId));
  }

  public get teachingSubjectSelectItems(): MetadataTagModel[] {
    return this._teachingSubjectSelectItems;
  }
  public set teachingSubjectSelectItems(v: MetadataTagModel[]) {
    this._teachingSubjectSelectItems = v;
    this.teachingSubjectIds = Utils.rightJoin(this.teachingSubjectIds, this.teachingSubjectSelectItems.map(p => p.tagId));
  }

  public checkReceiveValidation(): boolean {
    if (this.option === SendCoursePublicityOption.SpecificTargetAudience) {
      if (
        (this.users == null || this.users.length === 0) &&
        (this.teachingLevels == null || this.teachingLevels.length === 0) &&
        (this.teachingSubjectIds == null || this.teachingSubjectIds.length === 0)
      ) {
        return false;
      }
    }

    return true;
  }

  public reset(): SendCoursePublicityViewModel {
    return Utils.clone(this, p => {
      p.mailContent = p.defaultTemplate.template;
    });
  }

  private setMetadataTagsDicInfo(metadataTags: MetadataTagModel[]): void {
    this.metadataTagsDic = Utils.toDictionary(metadataTags, p => p.tagId);
    this.metadataTagsDicByGroupCode = Utils.toDictionaryGroupBy(
      metadataTags.filter(p => p.groupCode != null),
      p => p.groupCode,
      items => Utils.orderBy(items, p => p.displayText)
    );
  }
}
