import { AttendanceTracking, IAttendanceTracking, MetadataTagModel, PublicUserInfo } from '@opal20/domain-api';

import { IGridDataItem } from '@opal20/infrastructure';

export interface IAttendanceTrackingViewModel extends IAttendanceTracking {
  selected: boolean;
  register: PublicUserInfo;
  registerAllMetadataDic: Dictionary<MetadataTagModel>;
}

// @dynamic
export class AttendanceTrackingViewModel extends AttendanceTracking implements IAttendanceTrackingViewModel, IGridDataItem {
  public selected: boolean;
  public register: PublicUserInfo;
  public teachingSubjectsDisplayText: string = '';
  public teachingLevelDisplayText: string = '';
  public serviceChemeDisplayText: string = '';

  public registerAllMetadataDic: Dictionary<MetadataTagModel> = {};

  public static createFromModel(
    attendanceTracking: AttendanceTracking,
    register: PublicUserInfo,
    checkAll: boolean = false,
    selecteds: Dictionary<boolean> = {},
    registerAllMetadataDic: Dictionary<MetadataTagModel>
  ): AttendanceTrackingViewModel {
    return new AttendanceTrackingViewModel({
      ...attendanceTracking,
      selected: checkAll || selecteds[attendanceTracking.id],
      register: register,
      registerAllMetadataDic: registerAllMetadataDic
    });
  }

  constructor(data?: IAttendanceTrackingViewModel) {
    super(data);
    if (data != null) {
      this.selected = data.selected;
      this.register = data.register;
      this.registerAllMetadataDic = data.registerAllMetadataDic;
      this.serviceChemeDisplayText = this.getServiceSchemeDisplayText();
      this.teachingLevelDisplayText = this.getTeachingLevelDisplayText();
      this.teachingSubjectsDisplayText = this.getTeachingSubjectsDisplayText();
    }
  }

  public getServiceSchemeDisplayText(): string {
    if (this.register == null) {
      return '';
    }
    const serviceScheme = this.registerAllMetadataDic[this.register.serviceScheme];
    return serviceScheme ? serviceScheme.displayText : '';
  }

  public getTeachingSubjectsDisplayText(): string {
    if (this.register == null) {
      return '';
    }
    return []
      .concat(this.register.teachingSubjects)
      .map(p => this.registerAllMetadataDic[p])
      .filter(p => p != null)
      .map(p => p.displayText)
      .join(', ');
  }
  public getTeachingLevelDisplayText(): string {
    if (this.register == null) {
      return '';
    }
    return this.register.teachingLevels
      .map(p => this.registerAllMetadataDic[p])
      .filter(p => p != null)
      .map(p => p.displayText)
      .join(', ');
  }
}
