export interface IChangeAttendanceTrackingReasonForAbsenceRequest {
  sessionId: string;
  userId: string;
  reason: string;
  attachment: string[];
}
