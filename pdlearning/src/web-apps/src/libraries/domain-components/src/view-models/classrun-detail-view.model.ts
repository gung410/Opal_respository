import {
  BaseUserInfo,
  ClassRun,
  ClassRunRescheduleStatus,
  ClassRunStatus,
  Course,
  ROLE_TO_PERMISSIONS,
  SystemRoleEnum,
  UserInfoModel
} from '@opal20/domain-api';
import { DateUtils, Utils } from '@opal20/infrastructure';
import { Observable, combineLatest, of } from 'rxjs';

import { map } from 'rxjs/operators';

export class ClassRunDetailViewModel {
  public static classRunFacilitatorItemsRoles = [SystemRoleEnum.CourseFacilitator];
  public static get classRunFacilitatorItemsPermissions(): string[] {
    return ROLE_TO_PERMISSIONS[SystemRoleEnum.CourseFacilitator];
  }
  public static get allUserItemsRoles(): SystemRoleEnum[] {
    return [].concat(this.classRunFacilitatorItemsRoles);
  }
  public static readonly create = create;

  public data: ClassRun = new ClassRun();
  public originalData: ClassRun = new ClassRun();

  public currentUser = Utils.cloneDeep(UserInfoModel.getMyUserInfo());

  //#region User dropdown items
  public facilitatorItems: BaseUserInfo[] = [];
  public coFacilitatorItems: BaseUserInfo[] = [];
  //#endregion

  private _comment: string;

  constructor(classRun?: ClassRun, public usersDic: Dictionary<UserInfoModel> = {}, public course: Course = new Course()) {
    this.updateClassRunData(classRun, course);
  }

  public get id(): string {
    return this.data.id;
  }

  public set id(id: string) {
    this.data.id = id;
  }

  public get courseId(): string {
    return this.data.courseId;
  }
  public set courseId(courseId: string) {
    this.data.courseId = courseId;
  }

  public get classTitle(): string {
    return this.data.classTitle;
  }
  public set classTitle(classTitle: string) {
    this.data.classTitle = classTitle;
  }

  public get classRunCode(): string {
    return this.data.classRunCode;
  }
  public set classRunCode(classRunCode: string) {
    this.data.classRunCode = classRunCode;
  }

  public get createdBy(): string {
    return this.data.createdBy;
  }
  public set createdBy(createdBy: string) {
    this.data.createdBy = createdBy;
  }

  public get startDate(): Date {
    return this.data.startDate;
  }
  public set startDate(startDate: Date) {
    this.data.startDate = startDate;
  }

  public get endDate(): Date {
    return this.data.endDate;
  }
  public set endDate(endDate: Date) {
    this.data.endDate = endDate;
  }

  public get planningStartTime(): Date {
    return this.data.planningStartTime;
  }

  public set planningStartTime(startTime: Date) {
    if (DateUtils.compareTimeOfDate(this.data.planningStartTime, startTime) !== 0) {
      this.data.planningStartTime = startTime;
    }
  }

  public get planningEndTime(): Date {
    return this.data.planningEndTime;
  }
  public set planningEndTime(endTime: Date) {
    if (DateUtils.compareTimeOfDate(this.data.planningEndTime, endTime) !== 0) {
      this.data.planningEndTime = DateUtils.setTimeToEndInMinute(endTime);
    }
  }

  public get facilitatorId(): string | null {
    return this.data.facilitatorIds.length === 0 ? null : this.data.facilitatorIds[0];
  }
  public set facilitatorId(facilitatorId: string | null) {
    this.data.facilitatorIds = facilitatorId ? [facilitatorId] : [];
  }

  public get coFacilitatorId(): string | null {
    return this.data.coFacilitatorIds.length === 0 ? null : this.data.coFacilitatorIds[0];
  }
  public set coFacilitatorId(coFacilitatorId: string) {
    this.data.coFacilitatorIds = coFacilitatorId ? [coFacilitatorId] : [];
  }

  public get minClassSize(): number {
    return this.data.minClassSize;
  }
  public set minClassSize(minClassSize: number) {
    this.data.minClassSize = minClassSize;
  }

  public get maxClassSize(): number {
    return this.data.maxClassSize;
  }
  public set maxClassSize(maxClassSize: number) {
    this.data.maxClassSize = maxClassSize;
  }

  public get applicationStartDate(): Date {
    return this.data.applicationStartDate;
  }
  public set applicationStartDate(applicationStartDate: Date) {
    this.data.applicationStartDate = applicationStartDate;
  }

  public get applicationEndDate(): Date {
    return this.data.applicationEndDate;
  }
  public set applicationEndDate(applicationEndDate: Date) {
    this.data.applicationEndDate = applicationEndDate;
  }

  public get status(): ClassRunStatus {
    return this.data.status;
  }
  public set status(status: ClassRunStatus) {
    this.data.status = status;
  }

  public get rescheduleStatus(): ClassRunRescheduleStatus {
    return this.data.rescheduleStatus;
  }
  public set rescheduleStatus(rescheduleStatus: ClassRunRescheduleStatus) {
    this.data.rescheduleStatus = rescheduleStatus;
  }

  public get courseCriteriaActivated(): boolean {
    return this.data.courseCriteriaActivated;
  }
  public set courseCriteriaActivated(courseCriteriaActivated: boolean) {
    this.data.courseCriteriaActivated = courseCriteriaActivated;
  }

  public get comment(): string {
    return this._comment;
  }
  public set comment(comment: string) {
    this._comment = comment;
  }

  public updateClassRunData(classRun: ClassRun | null, course: Course): void {
    if (course && !classRun) {
      classRun = new ClassRun();
      classRun.facilitatorIds = course.courseFacilitatorIds;
      classRun.minClassSize = course.minParticipantPerClass;
      classRun.maxClassSize = course.maxParticipantPerClass;
    }
    if (classRun) {
      this.originalData = Utils.cloneDeep(classRun);
      this.data = Utils.cloneDeep(classRun);
    }
  }

  public dataHasChanged(): boolean {
    return Utils.isDifferent(this.originalData, this.data);
  }

  public displayAssignmentsTab(): boolean {
    return this.data.publishedAndStarted();
  }
}

function create(
  getUsersByIdsFn: (ids: string[]) => Observable<UserInfoModel[]>,
  course: Course,
  classRun: ClassRun
): Observable<ClassRunDetailViewModel> {
  if (classRun == null) {
    return of(new ClassRunDetailViewModel(new ClassRun(), {}, course));
  }
  return combineLatest(classRun.getAllUserIds().length === 0 ? of([]) : getUsersByIdsFn(classRun.getAllUserIds())).pipe(
    map(([users]) => {
      return new ClassRunDetailViewModel(Utils.clone(classRun), Utils.toDictionary(users, p => p.id), course);
    })
  );
}
