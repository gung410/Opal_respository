import { Announcement, AnnouncementStatus, AnnouncementTemplate, Registration } from '@opal20/domain-api';

import { Utils } from '@opal20/infrastructure';

export class SendAnnouncementViewModel {
  public data: Announcement = new Announcement();
  public originalData: Announcement = new Announcement();

  private _selectedAnnouncementTemplate: AnnouncementTemplate;
  private _isSentNow: boolean = true;
  private _isSentToAllParticipants = true;
  private _announcementTemplateDict: Dictionary<AnnouncementTemplate> = {};
  private _announcementTemplates: AnnouncementTemplate[] = [];
  private _selectedRegistrations: Registration[] = [];

  constructor(announcement?: Announcement) {
    if (announcement) {
      this.updateAnnouncementData(announcement);
    }
  }

  public get id(): string | null {
    return this.data.id;
  }

  public get selectedRegistrations(): Registration[] {
    return this._selectedRegistrations;
  }

  public set selectedRegistrations(v: Registration[]) {
    if (Utils.isDifferent(this.selectedRegistrations, v)) {
      this._selectedRegistrations = v;
      this.participants = this.selectedRegistrations.map(p => p.id);
    }
  }

  public get isSentToAllParticipants(): boolean {
    return this._isSentToAllParticipants;
  }

  public setIsSentToAllParticipants(v: boolean): void {
    if (Utils.isDifferent(this.isSentToAllParticipants, v)) {
      this._isSentToAllParticipants = v;
      if (this.isSentToAllParticipants) {
        this.selectedRegistrations = [];
      }
    }
  }

  public get isSentNow(): boolean {
    return this._isSentNow;
  }

  public setIsSentNow(v: boolean): void {
    if (Utils.isDifferent(this.isSentNow, v)) {
      this._isSentNow = v;
      if (this.isSentNow) {
        this.data.scheduleDate = null;
      }
    }
  }

  public get announcementTemplates(): AnnouncementTemplate[] {
    return this._announcementTemplates;
  }

  public set announcementTemplates(v: AnnouncementTemplate[]) {
    if (Utils.isDifferent(this.announcementTemplates, v)) {
      this._announcementTemplates = v;
      this._announcementTemplateDict = Utils.toDictionary(this._announcementTemplates, p => p.id);
    }
  }

  public get selectedAnnouncementTemplateId(): string | null {
    return this._selectedAnnouncementTemplate && this._selectedAnnouncementTemplate.id;
  }

  public set selectedAnnouncementTemplateId(v: string | null) {
    if (Utils.isDifferent(this.selectedAnnouncementTemplateId, v)) {
      this._selectedAnnouncementTemplate = this._announcementTemplateDict[v];
      this.applyTemplateContent();
    }
  }

  public get message(): string {
    return this.data.message;
  }

  public set message(v: string) {
    if (Utils.isDifferent(this.data.message, v)) {
      this.data.message = v;
    }
  }

  public get title(): string {
    return this.data.title;
  }

  public set title(v: string) {
    if (Utils.isDifferent(this.data.title, v)) {
      this.data.title = v;
    }
  }

  public get scheduleDate(): Date {
    return this.data.scheduleDate;
  }

  public set scheduleDate(v: Date) {
    if (Utils.isDifferent(this.data.scheduleDate, v)) {
      this.data.scheduleDate = v;
    }
  }

  public get participants(): string[] {
    return this.data.participants;
  }

  public set participants(v: string[]) {
    if (Utils.isDifferent(this.data.participants, v)) {
      this.data.participants = v;
    }
  }

  public get status(): AnnouncementStatus {
    return this.data.status;
  }

  public set status(v: AnnouncementStatus) {
    if (Utils.isDifferent(this.data.status, v)) {
      this.data.status = v;
    }
  }

  public get courseId(): string {
    return this.data.courseId;
  }

  public set courseId(v: string) {
    if (Utils.isDifferent(this.data.courseId, v)) {
      this.data.courseId = v;
    }
  }

  public get classrunId(): string {
    return this.data.classrunId;
  }

  public set classrunId(v: string) {
    if (Utils.isDifferent(this.data.classrunId, v)) {
      this.data.classrunId = v;
    }
  }

  public updateAnnouncementData(data: Announcement): void {
    this.originalData = Utils.cloneDeep(data);
    this.data = Utils.cloneDeep(data);
  }

  public reset(): void {
    this.data = Utils.cloneDeep(this.originalData);
  }

  public dataHasChanged(): boolean {
    return Utils.isDifferent(this.originalData, this.data);
  }

  public clearData(): void {
    this.reset();
    this.selectedAnnouncementTemplateId = null;
    this.setIsSentNow(true);
    this.setIsSentToAllParticipants(true);
  }

  private applyTemplateContent(): void {
    this.data.title = this._selectedAnnouncementTemplate ? this._selectedAnnouncementTemplate.title : '';
    this.data.message = this._selectedAnnouncementTemplate ? this._selectedAnnouncementTemplate.message : '';
  }
}
