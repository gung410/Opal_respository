import { BaseUserInfo, DepartmentInfoModel, SendAnnouncemmentEmailTemplateModel } from '@opal20/domain-api';

import { SendCourseNominationAnnoucementOption } from '../models/send-course-nomination-option.model';
import { Utils } from '@opal20/infrastructure';

export class SendCourseNominationAnnoucementViewModel {
  public userItems: BaseUserInfo[] = [];
  private _option: SendCourseNominationAnnoucementOption = SendCourseNominationAnnoucementOption.AllUsers;
  private _organisationIds: number[] = [];
  private _mailContent: string = '';
  private _organisationsSelectItems: DepartmentInfoModel[] = [];

  constructor(
    public defaultTemplate: SendAnnouncemmentEmailTemplateModel = new SendAnnouncemmentEmailTemplateModel(),
    public organisations: DepartmentInfoModel[] = []
  ) {
    this._mailContent = defaultTemplate.template;
    this._organisationsSelectItems = Utils.defaultIfNull(this.organisations, []);
  }

  public get option(): SendCourseNominationAnnoucementOption {
    return this._option;
  }
  public set option(option: SendCourseNominationAnnoucementOption) {
    this._option = option;
  }

  public get mailContent(): string {
    return this._mailContent;
  }
  public set mailContent(mailContent: string) {
    this._mailContent = mailContent;
  }

  public get organisationIds(): number[] {
    return this._organisationIds;
  }
  public set organisationIds(organisationIds: number[]) {
    this._organisationIds = organisationIds;
  }

  public setOption(value: SendCourseNominationAnnoucementOption): void {
    this.option = value;

    if (this.option === SendCourseNominationAnnoucementOption.AllUsers) {
      this.organisations = [];
    }
  }

  public get organisationsSelectItems(): DepartmentInfoModel[] {
    return this._organisationsSelectItems;
  }
  public set organisationsSelectItems(v: DepartmentInfoModel[]) {
    this._organisationsSelectItems = v;
  }

  public checkReceiveValidation(): boolean {
    if (this.option === SendCourseNominationAnnoucementOption.SpecificOrganisation) {
      if (this.organisationIds == null || this.organisationIds.length === 0) {
        return false;
      }
    }

    return true;
  }

  public reset(): SendCourseNominationAnnoucementViewModel {
    return Utils.clone(this, p => {
      p.mailContent = p.defaultTemplate.template;
    });
  }
}
