import { AnnouncementFilterModel } from '@opal20/domain-components';
import { Utils } from '@opal20/infrastructure';

export class AnnouncementActivityLogViewModel {
  public filterData: AnnouncementFilterModel = new AnnouncementFilterModel();

  public get fromCreatedDate(): Date {
    return this.filterData.fromCreatedDate;
  }

  public set fromCreatedDate(v: Date) {
    if (Utils.isDifferent(this.filterData.fromCreatedDate, v)) {
      this.filterData.fromCreatedDate = v;
    }
  }

  public get toCreatedDate(): Date {
    return this.filterData.toCreatedDate;
  }

  public set toCreatedDate(v: Date) {
    if (Utils.isDifferent(this.filterData.toCreatedDate, v)) {
      this.filterData.toCreatedDate = v;
    }
  }
}
