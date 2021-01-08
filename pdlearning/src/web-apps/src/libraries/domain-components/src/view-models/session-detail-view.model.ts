import { DateUtils, Utils } from '@opal20/infrastructure';
import { DigitalContent, Session } from '@opal20/domain-api';

export interface ISessionDetailViewModel {
  sessionData: Session;
  sessionPreRecordClip?: DigitalContent;
}

export class SessionDetailViewModel implements ISessionDetailViewModel {
  public sessionData: Session = new Session();
  public originSessionData: Session = new Session();

  public get classRunId(): string {
    return this.sessionData.classRunId;
  }
  public set classRunId(classRunId: string) {
    this.sessionData.classRunId = classRunId;
  }

  public get sessionTitle(): string {
    return this.sessionData.sessionTitle;
  }
  public set sessionTitle(sessionTitle: string) {
    this.sessionData.sessionTitle = sessionTitle;
  }

  public get venue(): string {
    return this.sessionData.venue;
  }
  public set venue(venue: string) {
    this.sessionData.venue = venue;
  }

  public get learningMethod(): boolean {
    return this.sessionData.learningMethod;
  }
  public set learningMethod(learningMethod: boolean) {
    this.sessionData.learningMethod = learningMethod;
  }

  public get sessionDate(): Date {
    return this.sessionData.sessionDate;
  }
  public set sessionDate(sessionDate: Date) {
    this.sessionData.sessionDate = sessionDate;
  }

  public get startTime(): Date {
    return this.sessionData.startTime;
  }
  public set startTime(startTime: Date) {
    this.sessionData.startTime = startTime;
  }

  public get endTime(): Date {
    return this.sessionData.endTime;
  }
  public set endTime(endTime: Date) {
    this.sessionData.endTime = DateUtils.setTimeToEndInMinute(endTime);
  }

  public get usePreRecordClip(): boolean {
    return this.sessionData.usePreRecordClip;
  }
  public set usePreRecordClip(usePreRecordClip: boolean) {
    this.sessionData.usePreRecordClip = usePreRecordClip;
  }

  public get preRecordId(): string {
    return this.sessionData.preRecordId;
  }
  public set preRecordId(preRecordId: string) {
    this.sessionData.preRecordId = preRecordId;
  }

  public get preRecordPath(): string {
    return this.sessionData.preRecordPath;
  }
  public set preRecordPath(preRecordPath: string) {
    this.sessionData.preRecordPath = preRecordPath;
  }

  public _preRecordClip: DigitalContent | null;
  public get preRecordClip(): DigitalContent | null {
    return this._preRecordClip;
  }
  public set preRecordClip(v: DigitalContent | null) {
    if (Utils.isDifferent(this._preRecordClip, v)) {
      this._preRecordClip = v;
      if (v != null) {
        this.usePreRecordClip = true;
        this.preRecordId = v.id;
        this.preRecordPath = v.fileLocation;
      } else {
        this.usePreRecordClip = false;
        this.preRecordId = null;
        this.preRecordPath = null;
      }
    }
  }

  constructor(data?: Partial<ISessionDetailViewModel>) {
    if (data) {
      if (data.sessionData != null) {
        this.updateSessionData(data.sessionData);
      }
      if (data.sessionPreRecordClip != null) {
        this._preRecordClip = data.sessionPreRecordClip;
      }
    }
  }

  public updateSessionData(session: Session): void {
    this.originSessionData = Utils.cloneDeep(session);
    this.sessionData = Utils.cloneDeep(session);
  }

  public dataHasChanged(): boolean {
    return Utils.isDifferent(this.originSessionData, this.sessionData);
  }
}
