import { ClassRun, IClassRun, PublicUserInfo } from '@opal20/domain-api';
import { IGridDataItem, Utils } from '@opal20/infrastructure';

export interface IClassRunViewModel extends IClassRun {
  selected: boolean;
  facilitators: PublicUserInfo[];
}

// @dynamic
export class ClassRunViewModel extends ClassRun implements IClassRunViewModel, IGridDataItem {
  public id: string;
  public selected: boolean;
  public facilitators: PublicUserInfo[] = [];
  public static createFromModel(
    classRun: ClassRun,
    facilitators: PublicUserInfo[],
    checkAll: boolean = false,
    selecteds: Dictionary<boolean> = {}
  ): ClassRunViewModel {
    return new ClassRunViewModel({
      ...Utils.toPureObject<ClassRun, IClassRun>(classRun),
      selected: checkAll || selecteds[classRun.id],
      facilitators: facilitators
    });
  }

  constructor(data?: IClassRunViewModel) {
    super(data);
    if (data != null) {
      this.selected = data.selected;
      this.facilitators = data.facilitators;
    }
  }
}
