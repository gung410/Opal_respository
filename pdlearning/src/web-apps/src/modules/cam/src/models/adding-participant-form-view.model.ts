import {
  ClassRun,
  ClassRunStatus,
  Course,
  CourseUser,
  DepartmentInfoModel,
  PublicUserInfo,
  SelectLearnerType,
  TargetParticipantType
} from '@opal20/domain-api';

import { Observable } from 'rxjs';
import { Utils } from '@opal20/infrastructure';

export interface IAddingParticipantFormViewModelData {
  classRuns: ClassRun[];
  course: Course;
}

export class AddingParticipantFormViewModel implements IAddingParticipantFormViewModelData {
  public selectedClassRun: ClassRun = new ClassRun();
  public get selectedLearners(): CourseUser[] {
    return this._selectedLearners;
  }

  public set selectedLearners(v: CourseUser[]) {
    this._selectedLearners = v;
    this.loadUserInfo(this._selectedLearners).subscribe(data => {
      this.displaySelectedUsersDic = Utils.toDictionary<PublicUserInfo>(data, p => p.id);
    });
  }
  public selectedLearnerDepartments: DepartmentInfoModel[] = [];
  public selectedToAddLearner: CourseUser | null;
  public selectedToAddLearnerDepartment: DepartmentInfoModel | null;
  public course: Course = new Course();
  public get classRuns(): ClassRun[] {
    return this._classRuns;
  }
  public set classRuns(v: ClassRun[]) {
    if (Utils.isDifferent(this._classRuns, v)) {
      this._classRuns = this.course.isELearning() ? v.filter(p => p.publishedAndNotEnded()) : v.filter(p => p.publishedAndNotStarted());
      this._classRunDic = Utils.toDictionary(this.classRuns, p => p.id);
    }
  }
  public displaySelectedUsersDic: Dictionary<PublicUserInfo> = {};

  private _classRuns: ClassRun[] = [];
  private _selectedClassRunId?: string;
  private _classRunDic: Dictionary<ClassRun> = {};
  private _targetParticipantType: TargetParticipantType = TargetParticipantType.ApplicableForEveryone;
  private _selectLeanerType: SelectLearnerType = SelectLearnerType.SelectIndividualLearners;
  private _selectedLearners: CourseUser[] = [];
  constructor(data?: IAddingParticipantFormViewModelData, public loadUserInfo?: (users: CourseUser[]) => Observable<PublicUserInfo[]>) {
    if (data != null) {
      this.classRuns = data.classRuns ? data.classRuns : [];
      this.course = data.course;
    }
  }
  public get classTitle(): string {
    return this.selectedClassRun.classTitle;
  }
  public set classTitle(v: string) {
    this.selectedClassRun.classTitle = v;
  }

  public get applicationStartDate(): Date {
    return this.selectedClassRun.applicationStartDate;
  }
  public set applicationStartDate(v: Date) {
    this.selectedClassRun.applicationStartDate = v;
  }
  public get applicationEndDate(): Date {
    return this.selectedClassRun.applicationEndDate;
  }
  public set applicationEndDate(v: Date) {
    this.selectedClassRun.applicationEndDate = v;
  }
  public get startDate(): Date {
    return this.selectedClassRun.startDate;
  }
  public set startDate(v: Date) {
    this.selectedClassRun.startDate = v;
  }
  public get endDate(): Date {
    return this.selectedClassRun.endDate;
  }
  public set endDate(v: Date) {
    this.selectedClassRun.endDate = v;
  }
  public get classSize(): number {
    return this.selectedClassRun.maxClassSize;
  }
  public set classSize(v: number) {
    this.selectedClassRun.maxClassSize = v;
  }
  public get status(): ClassRunStatus {
    return this.selectedClassRun.status;
  }
  public set status(v: ClassRunStatus) {
    this.selectedClassRun.status = v;
  }

  public get selectedClassRunId(): string | null {
    return this._selectedClassRunId;
  }
  public set selectedClassRunId(selectedClassRunId: string) {
    if (Utils.isDifferent(this._selectedClassRunId, selectedClassRunId)) {
      this._selectedClassRunId = selectedClassRunId;
      this.selectedClassRun = this._classRunDic[this.selectedClassRunId] ? this._classRunDic[this.selectedClassRunId] : new ClassRun();
    }
  }

  public get targetParticipantType(): TargetParticipantType {
    return this._targetParticipantType;
  }
  public set targetParticipantType(targetParticipantType: TargetParticipantType) {
    if (this._targetParticipantType === targetParticipantType) {
      return;
    }
    this._targetParticipantType = targetParticipantType;
  }

  public get selectLearnerType(): SelectLearnerType {
    return this._selectLeanerType;
  }
  public set selectLearnerType(selectLeanerType: SelectLearnerType) {
    if (this._selectLeanerType === selectLeanerType) {
      return;
    }
    this._selectLeanerType = selectLeanerType;
  }

  public setTargetParticipant(value: TargetParticipantType): void {
    this.selectedToAddLearner = null;
    this.selectedToAddLearnerDepartment = null;
    this.targetParticipantType = value;
  }

  public setSelectLearnerAndDepartments(value: SelectLearnerType): void {
    this.selectedToAddLearner = null;
    this.selectedToAddLearnerDepartment = null;
    this.selectLearnerType = value;
  }

  public addSelectedUserByCourseItem(): void {
    if (this.selectedToAddLearner != null && !this.selectedLearners.map(p => p.id).includes(this.selectedToAddLearner.id)) {
      this.selectedLearners = this.selectedLearners.concat(this.selectedToAddLearner);
    }
    if (
      this.selectedToAddLearnerDepartment != null &&
      !this.selectedLearnerDepartments.map(p => p.id).includes(this.selectedToAddLearnerDepartment.id)
    ) {
      this.selectedLearnerDepartments = this.selectedLearnerDepartments.concat(this.selectedToAddLearnerDepartment);
    }
  }

  public resetForm(): void {
    this.selectedToAddLearner = null;
    this.selectedToAddLearnerDepartment = null;
    this.targetParticipantType = TargetParticipantType.ApplicableForEveryone;
    this.selectLearnerType = SelectLearnerType.SelectIndividualLearners;
    this.selectedLearnerDepartments = [];
    this.selectedLearners = [];
  }

  public formValid(): boolean {
    return this.selectedClassRunId != null && (this.selectedLearners.length > 0 || this.selectedLearnerDepartments.length > 0);
  }

  public checkIndividualLeanerType(): boolean {
    return this.selectLearnerType === SelectLearnerType.SelectIndividualLearners;
  }

  public checkDepartmentType(): boolean {
    return this.selectLearnerType === SelectLearnerType.SelectDepartments;
  }

  public checkApplicableForEveryoneType(): boolean {
    return this.targetParticipantType === TargetParticipantType.ApplicableForEveryone;
  }

  public checkFollowingCourseTargetParticipantType(): boolean {
    return this.targetParticipantType === TargetParticipantType.FollowingCourseTargetParticipant;
  }
  public isSelectedAddLearner(): boolean {
    return this.selectedToAddLearnerDepartment != null || this.selectedToAddLearner != null;
  }

  public get selectedToAddLearnerId(): string | null {
    return this.selectedToAddLearner ? this.selectedToAddLearner.id : null;
  }

  public setSelectedToAddLearner(courseUser: CourseUser | null): void {
    this.selectedToAddLearner = courseUser;
  }

  public get selectedToAddLearnerDepartmentId(): number | null {
    return this.selectedToAddLearnerDepartment ? this.selectedToAddLearnerDepartment.id : null;
  }

  public setSelectedToAddLearnerDepartment(department: DepartmentInfoModel | null): void {
    this.selectedToAddLearnerDepartment = department;
  }
}
