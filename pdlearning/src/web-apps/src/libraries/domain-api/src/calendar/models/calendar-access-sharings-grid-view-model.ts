import { IGridDataItem } from '@opal20/infrastructure';
import { IUserAccessSharingModel } from './user-access-sharing-model';

export interface ICalendarAccessSharingGridViewModel extends IUserAccessSharingModel {
  serviceScheme: string;
  developmentRole: string;
  placeOfWork: string;
  avatarUrl: string;
}

export class CalendarAccessSharingGridViewModel implements IGridDataItem, ICalendarAccessSharingGridViewModel {
  public id: string;
  public selected: boolean;

  public userId: string;
  public fullName: string;
  public email: string;
  public shared: boolean;
  public serviceScheme: string;
  public developmentRole: string;
  public placeOfWork: string;
  public avatarUrl: string;

  public static buildGridData(
    userAccessSharing: IUserAccessSharingModel,
    placeOfWork?: string,
    serviceScheme?: string,
    developmentRole?: string,
    avatarUrl?: string
  ): CalendarAccessSharingGridViewModel {
    return new CalendarAccessSharingGridViewModel({
      ...userAccessSharing,
      placeOfWork,
      serviceScheme,
      developmentRole,
      avatarUrl
    });
  }

  constructor(data?: ICalendarAccessSharingGridViewModel) {
    if (data != null) {
      this.id = data.userId;

      this.userId = data.userId;
      this.fullName = data.fullName;
      this.email = data.email;
      this.shared = data.shared;

      this.placeOfWork = data.placeOfWork ? data.placeOfWork : null;
      this.serviceScheme = data.serviceScheme ? data.serviceScheme : null;
      this.developmentRole = data.developmentRole ? data.developmentRole : null;
      this.avatarUrl = data.avatarUrl ? data.avatarUrl : null;
    }
  }
}
