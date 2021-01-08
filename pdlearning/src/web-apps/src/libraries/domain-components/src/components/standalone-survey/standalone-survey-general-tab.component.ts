import {
  ADMINISTRATOR_ROLES,
  BaseUserInfo,
  FormSurveyType,
  FormType,
  SqRatingType,
  StandaloneSurveyModel,
  SystemRoleEnum,
  UserInfoModel,
  UserRepository,
  UserRepositoryContext,
  UserUtils
} from '@opal20/domain-api';
import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { StandaloneSurveyDetailMode } from '../../models/standalone-survey-detail-mode.model';
import { StandaloneSurveyEditModeService } from '../../services/standalone-survey-edit-mode.service';

@Component({
  selector: 'standalone-survey-general-tab',
  templateUrl: './standalone-survey-general-tab.component.html'
})
export class StandaloneSurveyGeneralTabComponent extends BaseComponent {
  @Input('form') public form: FormGroup;

  @Input() public set formData(formModel: StandaloneSurveyModel) {
    this._formData = formModel;
  }
  public get formData(): StandaloneSurveyModel {
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
  public mode: StandaloneSurveyDetailMode = this.editModeService.initMode;
  public StandaloneSurveyDetailMode: typeof StandaloneSurveyDetailMode = StandaloneSurveyDetailMode;
  public FormSurveyType: typeof FormSurveyType = FormSurveyType;

  private readonly currentUser = UserInfoModel.getMyUserInfo();
  private _formData: StandaloneSurveyModel = new StandaloneSurveyModel();

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private userRepositoryContext: UserRepositoryContext,
    private userRepository: UserRepository,
    private editModeService: StandaloneSurveyEditModeService
  ) {
    super(moduleFacadeService);
    this.initSqRatingTypeOptions();
    this.createFilterFn();
    this.fetchApprovalOfficersByIdsFn = UserUtils.createFetchUsersByIdsFn(this.userRepository);
  }

  public get isSystemAdmin(): boolean {
    return this.currentUser.hasRole(SystemRoleEnum.SystemAdministrator);
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

  protected onInit(): void {
    this.subscribe(this.editModeService.modeChanged, mode => {
      this.mode = mode;
    });

    this.subscribe(this.userRepositoryContext.baseUserInfoSubject, usersDicById => {
      this.usersDicById = usersDicById;
    });
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
