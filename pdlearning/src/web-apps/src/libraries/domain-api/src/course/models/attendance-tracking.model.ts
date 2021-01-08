import { LMM_PERMISSIONS } from '@opal20/domain-api/share/permission-keys/lmm-permission-key';
import { UserInfoModel } from '@opal20/domain-api';

export interface IAttendanceTracking {
  id: string;
  sessionId: string;
  registrationId: string;
  userId: string;
  reasonForAbsence?: string;
  attachment?: string[];
  isCodeScanned: boolean;
  codeScannedDate?: Date;
  status?: AttendanceStatus;
}
export class AttendanceTracking implements IAttendanceTracking {
  public id: string;
  public sessionId: string;
  public registrationId: string;
  public userId: string;
  public reasonForAbsence?: string;
  public attachment?: string[];
  public isCodeScanned: boolean;
  public status?: AttendanceStatus;
  public codeScannedDate?: Date;

  public static hasSetPresentAbsentPermission(user: UserInfoModel): boolean {
    return user.hasPermissionPrefix(LMM_PERMISSIONS.SetPresentAbsent);
  }

  constructor(data?: IAttendanceTracking) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.sessionId = data.sessionId;
    this.registrationId = data.registrationId;
    this.userId = data.userId;
    this.reasonForAbsence = data.reasonForAbsence;
    this.attachment = data.attachment;
    this.isCodeScanned = data.isCodeScanned;
    this.codeScannedDate = data.codeScannedDate;
    this.status = data.status;
  }

  public hasViewReasonForAbsentPermission(user: UserInfoModel): boolean {
    return user.hasPermissionPrefix(LMM_PERMISSIONS.ViewReasonForAbsent);
  }
}
export enum AttendanceStatus {
  Present = 'Present',
  Absent = 'Absent'
}
export enum REASON_ABSENCE {
  Adoption = 'AdoptionChildcareMaternityPaternity',
  Sick = 'SickHospitalisation',
  Compassionate = 'Compassionate',
  Marriage = 'Marriage',
  Operationally = 'OperationallyReadyNationalService',
  ParentCare = 'ParentCare',
  Preparation = 'PreparationForExam',
  Official = 'OfficialDuties',
  Other = 'Other'
}
export const REASON_ABSENCE_MAPPING_TEXT_CONST: Map<REASON_ABSENCE, string> = new Map<REASON_ABSENCE, string>([
  [REASON_ABSENCE.Adoption, 'Adoption/childcare/maternity/paternity leave'],
  [REASON_ABSENCE.Sick, 'Sick/hospitalisation leave'],
  [REASON_ABSENCE.Compassionate, 'Compassionate leave'],
  [REASON_ABSENCE.Marriage, 'Marriage leave'],
  [REASON_ABSENCE.Operationally, 'Operationally Ready National Service'],
  [REASON_ABSENCE.ParentCare, 'Parent-care leave'],
  [REASON_ABSENCE.Preparation, 'Preparation for exam'],
  [REASON_ABSENCE.Official, 'Official duties'],
  [REASON_ABSENCE.Other, 'Other']
]);
