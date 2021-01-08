import { AttendanceStatus, AttendanceTracking } from '@opal20/domain-api';

import { Utils } from '@opal20/infrastructure';

export class AttendanceTrackingDetailViewModel {
  public attendanceTrackingData: AttendanceTracking = new AttendanceTracking();
  public originAttendaceTrackingData: AttendanceTracking = new AttendanceTracking();

  //#endregion

  constructor(attendaceTracking?: AttendanceTracking) {
    if (attendaceTracking) {
      this.updateAttendanceTrackingData(attendaceTracking);
    }
  }

  public get sessionId(): string {
    return this.attendanceTrackingData.sessionId;
  }
  public set sessionId(sessionId: string) {
    this.attendanceTrackingData.sessionId = sessionId;
  }

  public get registrationId(): string {
    return this.attendanceTrackingData.registrationId;
  }
  public set sessionTitle(registrationId: string) {
    this.attendanceTrackingData.registrationId = registrationId;
  }

  public get userId(): string {
    return this.attendanceTrackingData.userId;
  }
  public set userId(userId: string) {
    this.attendanceTrackingData.userId = userId;
  }

  public get reasonForAbsence(): string {
    return this.attendanceTrackingData.reasonForAbsence;
  }
  public set reasonForAbsence(reasonForAbsence: string) {
    this.attendanceTrackingData.reasonForAbsence = reasonForAbsence;
  }

  public get status(): AttendanceStatus {
    return this.attendanceTrackingData.status;
  }
  public set status(status: AttendanceStatus) {
    this.attendanceTrackingData.status = status;
  }
  public updateAttendanceTrackingData(attendaceTracking: AttendanceTracking): void {
    this.originAttendaceTrackingData = Utils.cloneDeep(attendaceTracking);
    this.attendanceTrackingData = Utils.cloneDeep(attendaceTracking);
  }

  public dataHasChanged(): boolean {
    return Utils.isDifferent(this.originAttendaceTrackingData, this.attendanceTrackingData);
  }
}
