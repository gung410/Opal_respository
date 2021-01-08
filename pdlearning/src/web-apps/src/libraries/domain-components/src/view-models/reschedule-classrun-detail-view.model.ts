import { ClassRun, Session } from '@opal20/domain-api';

import { Utils } from '@opal20/infrastructure';

export class RescheduleClassRunDetailViewModel {
  public classRunData: ClassRun = new ClassRun();
  public originClassRunData: ClassRun = new ClassRun();

  public sessionsData: Session[] = [];
  public originSessionsData: Session[] = [];
  public originSessionDataDict: Dictionary<Session> = {};

  private _comment: string;

  constructor(classRun?: ClassRun, sessions: Session[] = null) {
    if (classRun) {
      this.updateClassRunData(classRun);
    }

    if (sessions && sessions.length > 0) {
      this.updateSessionsData(sessions);
    }
  }

  public get courseId(): string {
    return this.classRunData.courseId;
  }
  public set courseId(courseId: string) {
    this.classRunData.courseId = courseId;
  }

  public get startDate(): Date {
    return this.classRunData.startDate;
  }
  public set startDate(startDate: Date) {
    this.classRunData.startDate = startDate;
  }

  public get startDateTime(): Date {
    return this.classRunData.startDateTime;
  }
  public set startDateTime(startDateTime: Date) {
    this.classRunData.startDateTime = startDateTime;
  }

  public get endDate(): Date {
    return this.classRunData.endDate;
  }
  public set endDate(endDate: Date) {
    this.classRunData.endDate = endDate;
  }

  public get endDateTime(): Date {
    return this.classRunData.endDateTime;
  }
  public set endDateTime(endDateTime: Date) {
    this.classRunData.endDateTime = endDateTime;
  }

  public get planningStartTime(): Date {
    return this.classRunData.planningStartTime;
  }
  public set planningStartTime(planningStartTime: Date) {
    this.classRunData.planningStartTime = planningStartTime;
  }

  public get planningEndTime(): Date {
    return this.classRunData.planningEndTime;
  }
  public set planningEndTime(planningEndTime: Date) {
    this.classRunData.planningEndTime = planningEndTime;
  }

  public get comment(): string {
    return this._comment;
  }
  public set comment(comment: string) {
    this._comment = comment;
  }

  public dataHasChanged(): boolean {
    return Utils.isDifferent(this.originClassRunData, this.classRunData) && Utils.isDifferent(this.sessionsData, this.originSessionsData);
  }

  public updateClassRunData(classRun: ClassRun): void {
    this.originClassRunData = Utils.cloneDeep(classRun);
    this.classRunData = Utils.cloneDeep(classRun);
  }

  public updateSessionsData(sessions: Session[]): void {
    this.originSessionsData = Utils.cloneDeep(sessions);
    this.originSessionDataDict = Utils.toDictionary(this.originSessionsData, p => p.id);
    this.sessionsData = Utils.cloneDeep(sessions);
  }
}
