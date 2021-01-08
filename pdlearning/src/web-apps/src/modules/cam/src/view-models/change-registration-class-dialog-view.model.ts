import { ClassRun, IClassRun } from '@opal20/domain-api';

export interface IChangeRegistrationClassDialogViewModel {
  courseId: string;
  currentClass: IClassRun;
  registrationIds: string[];
  changeToClassId?: string;
}

export class ChangeRegistrationClassDialogViewModel {
  public courseId: string = '';
  public currentClass: ClassRun = new ClassRun();
  public registrationIds: string[] = [];
  public changeToClassId?: string;

  constructor(data?: Partial<IChangeRegistrationClassDialogViewModel>) {
    if (data) {
      this.courseId = data.courseId ? data.courseId : '';
      this.currentClass = new ClassRun(data.currentClass);
      this.registrationIds = data.registrationIds ? data.registrationIds : [];
      this.changeToClassId = data.changeToClassId;
    }
  }
}
