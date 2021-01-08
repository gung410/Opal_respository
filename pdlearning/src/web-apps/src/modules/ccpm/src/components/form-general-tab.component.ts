import {
  ADMINISTRATOR_ROLES,
  BaseUserInfo,
  FormModel,
  FormSurveyType,
  FormType,
  SqRatingType,
  SystemRoleEnum,
  UserInfoModel,
  UserRepository,
  UserRepositoryContext,
  UserUtils
} from '@opal20/domain-api';
import { BaseComponent, DateUtils, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { FormDetailMode } from '@opal20/domain-components';
import { FormEditModeService } from '../services/form-edit-mode.service';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'form-general-tab',
  templateUrl: './form-general-tab.component.html'
})
export class FormGeneralTabComponent extends BaseComponent {
  @Input('form') public form: FormGroup;

  @Input() public set formData(formModel: FormModel) {
    this._formData = formModel;
    this.createFetchAlternativeApprovalOfficersFn();
    this.createFetchPrimaryApprovalOfficersFn();
  }
  public get formData(): FormModel {
    return this._formData;
  }

  public approvalOfficerRoles = [
    SystemRoleEnum.MOEHQContentApprovingOfficer,
    SystemRoleEnum.CourseApprovingOfficer,
    SystemRoleEnum.SchoolContentApprovingOfficer,
    ...ADMINISTRATOR_ROLES
  ];

  public usersDicById: Dictionary<BaseUserInfo> = {};

  // Filter user in opal-select by term
  public filterApprovalOfficersFn: (term: string, item: BaseUserInfo) => boolean;

  // Fetch user by from API by term
  public fetchAlternativeApprovalOfficersFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]>;
  public fetchPrimaryApprovalOfficersFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]>;

  // Fetch user by from API by userIds
  public fetchApprovalOfficersByIdsFn: (userIds: string[]) => Observable<BaseUserInfo[]>;

  public formType = FormType;
  public surveyTypes: IDataItem[];
  public sqRatingTypes: IDataItem[];
  public mode: FormDetailMode = this.formEditModeService.initMode;
  public FormDetailMode: typeof FormDetailMode = FormDetailMode;
  public FormSurveyType: typeof FormSurveyType = FormSurveyType;

  private readonly currentUser = UserInfoModel.getMyUserInfo();
  private _formData: FormModel = new FormModel();

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private userRepositoryContext: UserRepositoryContext,
    private userRepository: UserRepository,
    private formEditModeService: FormEditModeService
  ) {
    super(moduleFacadeService);
    this.initSurveyTypeOptions();
    this.initSqRatingTypeOptions();
    this.createFilterFn();
    this.fetchApprovalOfficersByIdsFn = UserUtils.createFetchUsersByIdsFn(this.userRepository);
  }

  public get isSystemAdmin(): boolean {
    return this.currentUser.hasRole(SystemRoleEnum.SystemAdministrator);
  }

  public createFetchAlternativeApprovalOfficersFn(): void {
    const _fetchAaoFn = UserUtils.createFetchUsersFn(this.approvalOfficerRoles, this.userRepository);

    this.fetchAlternativeApprovalOfficersFn = (searchText: string, skipCount: number, maxResultCount: number) =>
      _fetchAaoFn(searchText, skipCount, maxResultCount).pipe(
        map(p => p.filter(x => x.id !== this.currentUser.extId).filter(x => x.id !== this.formData.primaryApprovingOfficerId))
      );
  }

  public createFetchPrimaryApprovalOfficersFn(): void {
    const _fetchPaoFn = UserUtils.createFetchUsersFn(this.approvalOfficerRoles, this.userRepository);

    this.fetchPrimaryApprovalOfficersFn = (searchText: string, skipCount: number, maxResultCount: number) =>
      _fetchPaoFn(searchText, skipCount, maxResultCount).pipe(
        map(p => p.filter(x => x.id !== this.currentUser.extId).filter(x => x.id !== this.formData.alternativeApprovingOfficerId))
      );
  }

  public disableDatesBeforeToday(value: Date): boolean {
    const today = new Date();
    value.setHours(23, 59, 59, 999);
    if (this.formData.endDate) {
      this.formData.endDate.setHours(23, 59, 59, 999);
    }
    return value < today || (this.formData.endDate && value > this.formData.endDate);
  }

  public disableDatesBeforeStartDate(value: Date): boolean {
    const today = new Date();
    value.setHours(23, 59, 59, 999);
    return (this.formData.startDate === null && value < today) || (this.formData.startDate !== null && value < this.formData.startDate);
  }

  public isViewMode(): boolean {
    return this.mode !== FormDetailMode.Edit;
  }

  public onCheckSendNotifyItem(value: boolean): void {
    this.formData.isSendNotification = value;
  }

  public onEndDateSelectChange(date: Date): void {
    if (!date) {
      this.formData.isSendNotification = undefined;
    }

    this.formData.endDate = DateUtils.setTimeToEndInDay(date);
  }

  public onFormRemindBeforeDaysChange(value: number): void {
    this.formData.remindBeforeDays = value;
  }

  public get maximumDatesRemind(): number {
    if (!this.formData.endDate) {
      return null;
    }

    const maximumDates = DateUtils.calculateAmountOfDayFromPresentToDate(this.formData.endDate);
    return maximumDates;
  }

  protected onInit(): void {
    this.subscribe(this.formEditModeService.modeChanged, mode => {
      this.mode = mode;
    });

    this.subscribe(this.userRepositoryContext.baseUserInfoSubject, usersDicById => {
      this.usersDicById = usersDicById;
    });
  }

  private initSurveyTypeOptions(): void {
    this.surveyTypes = [
      { text: this.translate('Standalone'), value: FormSurveyType.Standalone },
      { text: this.translate('Pre Course Survey'), value: FormSurveyType.PreCourse },
      { text: this.translate('During Course'), value: FormSurveyType.DuringCourse },
      { text: this.translate('Post Course Survey'), value: FormSurveyType.PostCourse },
      { text: this.translate('Follow-up Post Course Survey'), value: FormSurveyType.FollowUpPostCourse }
    ];
  }

  private initSqRatingTypeOptions(): void {
    this.sqRatingTypes = [
      {
        text: this.translate('Type 1: COURSE/WORKSHOP/MASTER CLASS/SEMINAR/CONFERENCE'),
        value: SqRatingType.CourseWorkshopMasterclassSeminarConference
      },
      { text: this.translate('Type 2: E-LEARNING COURSE'), value: SqRatingType.ELearningCourse },
      { text: this.translate('Type 3: BLENDED COURSE/WORKSHOP/MASTER CLASS'), value: SqRatingType.BlendedCourseWorkshopMasterclass },
      { text: this.translate('Type 4: LEARNING EVENT'), value: SqRatingType.LearningEvent }
    ];
  }

  private createFilterFn(): void {
    this.filterApprovalOfficersFn = (term: string, item: BaseUserInfo) =>
      term
        ? item.fullName.toLowerCase().includes(term.toLowerCase()) ||
          item.emailAddress.toLowerCase().includes(term.toLowerCase()) ||
          item.departmentName.toLowerCase().includes(term.toLowerCase())
        : true;
  }
}
