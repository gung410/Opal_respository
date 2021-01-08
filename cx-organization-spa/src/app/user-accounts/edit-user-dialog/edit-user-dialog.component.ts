import {
  AfterViewInit,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import { MatTabGroup } from '@angular/material/tabs';
import {
  CxConfirmationDialogComponent,
  CxGlobalLoaderService,
  CxSurveyjsComponent,
  CxSurveyjsEventModel,
  CxSurveyjsFormModalOptions,
  CxSurveyJsModeEnum,
  CxSurveyjsVariable
} from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DepartmentType } from 'app-models/department-type.model';
import { CxSurveyjsExtendedService } from 'app-services/cx-surveyjs-extended.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { DepartmentStoreService } from 'app/core/store-services/department-store.service';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';
import { findIndexCommon } from 'app/shared/constants/common.const';
import { INVALID_EMAIL_DOMAIN_CONST } from 'app/shared/constants/invalid-email-domain.const';
import { ServiceSchemeCodeEnum } from 'app/shared/constants/service-scheme.enum';
import { SurveyVariableEnum } from 'app/shared/constants/survey-variable.enum';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import * as _ from 'lodash';

import { User } from 'app-models/auth.model';
import { UserType } from 'app-models/user-type.model';
import { SystemRolesDataService } from 'app/core/store-data-services/system-roles-data.service';
import { UserRoleEnum } from 'app/shared/constants/user-roles.enum';
import * as Survey from 'survey-angular';
import { GenderEnum } from '../constants/user-field-mapping.constant';
import {
  UserManagement,
  UserManagementQueryModel
} from '../models/user-management.model';
import { UserAccountsDataService } from '../user-accounts-data.service';
import { UserAccountsHelper } from '../user-accounts.helper';
import { userFormJSON } from '../user-form';
import { EditUserDialogHelper } from './edit-user-dialog.helper';
import {
  EditUserDialogModeEnum,
  EditUserDialogSubmitModel
} from './edit-user-dialog.model';
import { auditHistoryTabIndexOnPendingUser, TabIndex } from './tab-index-enum';

Survey.JsonObject.metaData.addProperty('questionbase', 'keepIncorrectValues');

@Component({
  selector: 'edit-user-dialog',
  templateUrl: './edit-user-dialog.component.html',
  styleUrls: ['./edit-user-dialog.component.scss'],
  providers: [EditUserDialogHelper],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EditUserDialogComponent
  extends BaseSmartComponent
  implements OnInit, AfterViewInit {
  get getUsersCurrentSystemRoles(): UserType[] {
    return this.basicInfoSurveyJs
      ? this.basicInfoSurveyJs.surveyModel.data.systemRoles
      : [];
  }
  @Input() currentUser: User;
  @Input() user: UserManagement;
  @Input() departmentTypes: DepartmentType[];
  @Input() isPendingUser: boolean = false;
  @Input() fullUserInfoJsonData: any;
  @Input() surveyjsOptions: CxSurveyjsFormModalOptions;
  @Output()
  submit: EventEmitter<EditUserDialogSubmitModel> = new EventEmitter<EditUserDialogSubmitModel>();
  @Output() cancel: EventEmitter<any> = new EventEmitter();
  @ViewChild('basicInfoSurveyJs') basicInfoSurveyJs: CxSurveyjsComponent;
  @ViewChild('userInfoTabsGroup') userInfoTabsGroup: MatTabGroup;

  mode: EditUserDialogModeEnum = EditUserDialogModeEnum.Edit;
  validationFunctions: any[];
  userFormJSON: any = userFormJSON;
  userData: any;
  tabIndex: TabIndex = TabIndex.basicInfo;
  TabIndex: any = TabIndex;
  selectedTabIndexes: TabIndex[] = [TabIndex.basicInfo];
  auditHistoryTabIndexOnPendingUser: number = auditHistoryTabIndexOnPendingUser;

  disabledSystemRoles: any[];

  basicInfoSurveyVariables: CxSurveyjsVariable[] = [];

  isFirstPage: boolean = true;
  isLastPage: boolean = false;

  editUserSubmitModel: EditUserDialogSubmitModel = new EditUserDialogSubmitModel();
  EditUserDialogModeEnum: any = EditUserDialogModeEnum;
  private existedEmailErrorMessage: string;
  private invalidEmailDomainErrorMessage: string;
  private professionalDevelopmentTab: any;

  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private el: ElementRef,
    private systemRolesDataService: SystemRolesDataService,
    private translateAdapterService: TranslateAdapterService,
    private userAccountsDataService: UserAccountsDataService,
    private editUserDialogHelper: EditUserDialogHelper,
    private departmentStoreService: DepartmentStoreService,
    private cxSurveyjsExtendedService: CxSurveyjsExtendedService,
    private globalLoader: CxGlobalLoaderService,
    public ngbModal: NgbModal
  ) {
    super(changeDetectorRef);
    this.existedEmailErrorMessage = this.translateAdapterService.getValueImmediately(
      `User_Account_Page.User_Edit_Dialog.Error.Email_Existed`
    );
    this.invalidEmailDomainErrorMessage = this.translateAdapterService.getValueImmediately(
      `User_Account_Page.User_Edit_Dialog.Error.Invalid_Email_Domain`
    );
  }

  ngOnInit(): void {
    this.initHelper();
    // this.changeTimeStampToClearCache();
    this.editUserDialogHelper.addCustomProperties(
      this.userFormJSON,
      this.user,
      this.getUserImage(this.user)
    );

    this.userData = this.editUserDialogHelper.processSurveyJsonData(
      this.fullUserInfoJsonData
    );
    this.basicInfoSurveyVariables = _.cloneDeep(this.surveyjsOptions.variables);
    this.validationFunctions = [this.validateEmail.bind(this)];

    this.initializeAvailableRoles();
    // Due to changes in system role and permissions. Comment line below to change later
    // this.getNoPermissionRolesOfCurrentUser();
  }

  ngAfterViewInit(): void {
    this.professionalDevelopmentTab = this.el.nativeElement.querySelector(
      '[aria-label="professionalDevelopmentTab"]'
    );
    this.checkShowHideProfessionalDevelopmentTab(this.userData.personnelGroups);
  }

  changeTimeStampToClearCache(): void {
    this.userFormJSON = JSON.parse(
      JSON.stringify(this.userFormJSON).replace(
        'replaceTS',
        Math.random().toString()
      )
    );
  }

  onUserFormChanged(surveyEvent: CxSurveyjsEventModel): void {
    const surveyOptions = surveyEvent.options;
    const question = surveyOptions.question;

    if (question.name === 'systemRoles') {
      this.userData = this.editUserDialogHelper.processSurveyResultData(
        surveyEvent.survey.data
      );

      // this.getDisabledSystemRoles(this.userData.systemRoles);

      if (
        !this.userData.systemRoles &&
        this.disabledSystemRoles &&
        this.mode === EditUserDialogModeEnum.Edit
      ) {
        this.userData.systemRoles = this.disabledSystemRoles;
      }

      const currentObjectSystemRoles = this.basicInfoSurveyVariables.find(
        (variable) =>
          variable.name === SurveyVariableEnum.currentObject_systemRoles
      );

      if (currentObjectSystemRoles) {
        this.basicInfoSurveyVariables = this.basicInfoSurveyVariables.map(
          (variable) => {
            variable =
              variable.name !== 'currentObject_systemRoles'
                ? variable
                : !this.userData.systemRoles
                ? new CxSurveyjsVariable({
                    name: 'currentObject_systemRoles',
                    value: []
                  })
                : new CxSurveyjsVariable({
                    name: 'currentObject_systemRoles',
                    value: this.userData.systemRoles.map(
                      (role) => role.identity.extId
                    )
                  });

            return variable;
          }
        );
      } else {
        this.basicInfoSurveyVariables.push(
          new CxSurveyjsVariable({
            name: 'currentObject_systemRoles',
            value: this.userData.systemRoles.map((role) => role.identity.extId)
          })
        );
      }

      this.changeDetectorRef.detectChanges();
    }

    if (question.name === 'emailAddress') {
      const hasErrorEmail =
        question
          .getAllErrors()
          .findIndex(
            (err) =>
              err.text === this.existedEmailErrorMessage ||
              err.text === this.invalidEmailDomainErrorMessage
          ) > findIndexCommon.notFound;
      if (hasErrorEmail) {
        surveyOptions.ignoreRevalidation = true;
      }
    }

    if (question.name === 'personnelGroups') {
      this.checkShowHideProfessionalDevelopmentTab(surveyOptions.value);
    }

    if (question.name === 'departmentId') {
      this.handleOnDepartmentChanged(surveyEvent);
    }

    if (question.name === 'titleSalutation') {
      const genderQues = surveyEvent.survey.getQuestionByName('gender');
      switch (question.value) {
        case 'Mr':
          genderQues.value = GenderEnum.Male;
          break;

        case 'Mdm':
        case 'Miss':
        case 'Mrs':
        case 'Ms':
          genderQues.value = GenderEnum.Female;
          break;

        default:
          break;
      }
    }
  }

  onSurveyPageChanged(event: CxSurveyjsEventModel): void {
    if (
      !event.options ||
      !event.options.newCurrentPage ||
      this.tabIndex === event.options.newCurrentPage.visibleIndex
    ) {
      return;
    }
    this.tabIndex = event.options.newCurrentPage.visibleIndex;
  }

  onSave(): void {
    if (this.mode === EditUserDialogModeEnum.View) {
      this.cancel.emit();

      return;
    }

    if (!this.isWarningMissingAOorAAO()) {
      this.basicInfoSurveyJs.doComplete();

      return;
    }
    this.showNonAssignAOConfirmationDialog(
      () => {
        this.basicInfoSurveyJs.doComplete();
      },
      'Confirm',
      'You have not assigned Approving Officer/Alternate Approving Officer for this user. Proceed?'
    );
  }

  isExistRoles(roles: UserRoleEnum[]): boolean {
    const currentRoles = this.getUsersCurrentSystemRoles;
    if (!currentRoles || currentRoles.length === 0) {
      return false;
    }

    return (
      currentRoles.filter((x) =>
        roles.includes(x.identity.extId as UserRoleEnum)
      ).length > 0
    );
  }

  isWarningMissingAOorAAO(): boolean {
    return (
      this.mode !== EditUserDialogModeEnum.Create &&
      !this.isPendingUser &&
      !this.IsAOAvailable(this.editUserSubmitModel) &&
      !this.IsAAOAvailable(this.editUserSubmitModel)
    );
  }

  onSubmitting(event: CxSurveyjsEventModel): void {
    this.proceedToSaveUserInformation(event);
  }

  IsAOAvailable(editUserSubmitModel: EditUserDialogSubmitModel): boolean {
    return (
      !!editUserSubmitModel.approvalData.primaryApprovalGroup &&
      this.mode === EditUserDialogModeEnum.Edit
    );
  }

  IsAAOAvailable(editUserSubmitModel: EditUserDialogSubmitModel): boolean {
    return (
      !!editUserSubmitModel.approvalData.alternateApprovalGroup &&
      this.mode === EditUserDialogModeEnum.Edit
    );
  }

  onCancel(): void {
    this.cancel.emit();
  }

  onClickNextPage(): boolean {
    return this.basicInfoSurveyJs.nextPage();
  }

  onClickPrevPage(): boolean {
    return this.basicInfoSurveyJs.prevPage();
  }

  onSwitchingTab(newTabIndex: TabIndex): void {
    const checkBoxSelectAll: HTMLElement | null = null;
    if (!this.selectedTabIndexes.includes(newTabIndex)) {
      this.selectedTabIndexes.push(newTabIndex);
    }
    this.basicInfoSurveyJs.surveyModel.currentPageNo = newTabIndex;
    if (this.mode === EditUserDialogModeEnum.Create) {
      this.isLastPage = this.basicInfoSurveyJs.surveyModel.isLastPage;
      this.isFirstPage = this.basicInfoSurveyJs.surveyModel.isFirstPage;
    }
  }

  onAfterSurveyRendered(surveyEvent: CxSurveyjsEventModel): void {
    surveyEvent.survey.clearInvisibleValues = 'onHidden';

    // TODO: surveyEvent.survey.clearIncorrectValues();
    setTimeout(() => {
      this.removeInvisibleQuestionValues(surveyEvent);
    });
    surveyEvent.survey.onLoadChoicesFromServer.clear();
    surveyEvent.survey.onValidateQuestion.add(this.surveyValidateQuestion);
  }

  private proceedToSaveUserInformation(event: CxSurveyjsEventModel): void {
    event.options.allowComplete = false;
    this.userData = this.editUserDialogHelper.processSurveyResultData(
      event.survey.data
    );

    this.editUserSubmitModel.userData = {
      ...this.userData
    };
    this.submit.emit(this.editUserSubmitModel);
  }

  private removeInvisibleQuestionValues(
    surveyEvent: CxSurveyjsEventModel
  ): void {
    const questions = surveyEvent.survey.getQuestionsByNames([
      'careerPathsDropdown',
      'careerPathsTagbox',
      'learningFrameworksDropdown',
      'learningFrameworksTagbox'
    ]);
    questions.forEach((ques) => {
      if (!ques.visible) {
        ques.clearValueIfInvisible();
      }
    });
  }

  // private getNoPermissionRolesOfCurrentUser(): void {
  //   const roles = this.cxSurveyjsExtendedService.currentUser_noPermissionRoles;
  //   if (!roles || (roles && !roles.length)) {
  //     return;
  //   }

  //   this.systemRolesDataService
  //     .getRoles(
  //       new RolesRequest({
  //         archetypeIds: [this.systemRolesDataService.SYSTEM_ROLE_CODE],
  //         includeLocalizedData: true,
  //         extIds: roles
  //       })
  //     )
  //     .subscribe((rolesResponse) => {
  //       this.disabledSystemRoles = rolesResponse;
  //     });
  // }

  private checkShowHideProfessionalDevelopmentTab(personnelGroups: {
    codingScheme: string;
  }): void {
    if (!this.professionalDevelopmentTab) {
      return;
    }
    const showProfessionalTab =
      personnelGroups &&
      personnelGroups.codingScheme !== ServiceSchemeCodeEnum.NA;
    if (showProfessionalTab) {
      this.professionalDevelopmentTab.classList.remove('hidden-element');
    } else {
      this.professionalDevelopmentTab.classList.add('hidden-element');
    }
  }

  private surveyValidateQuestion(survey: any, options: any): void {
    if (options.name === 'expirationDate') {
      if (!survey.data.expirationDate || !survey.data.activeDate) {
        return;
      }

      const expirationDate = DateTimeUtil.surveyToDateLocalTime(
        survey.data.expirationDate
      );
      const activeDate = DateTimeUtil.surveyToDateLocalTime(
        survey.data.activeDate
      );
      if (expirationDate <= activeDate) {
        options.error =
          'Expiry Date must be greater than the Account Active From';
      }
    }
  }

  private validateEmail(survey: any, options: any): void {
    const hasError = this.validateEmailBelongToBlackList(survey, options);

    if (!hasError) {
      this.validateExistedEmail(survey, options);
    }
  }

  private validateExistedEmail(survey: any, options: any): void {
    const emailQuestion = survey.getQuestionByName('emailAddress');
    const currentEmail: string = survey.getVariable(
      'currentObject_emailAddress'
    );
    const newEmail: string = survey.data.emailAddress;
    if (
      !emailQuestion ||
      emailQuestion.isReadOnly ||
      (currentEmail &&
        newEmail &&
        currentEmail.toLowerCase() === newEmail.toLowerCase())
    ) {
      if (this.mode === EditUserDialogModeEnum.Edit) {
        survey.currentPage = survey.visiblePageCount - 1;
        // surveyjs cannot complete a page which is not the last one, so we move to the last page.
      }
      options.complete();

      return;
    }
    this.globalLoader.showLoader();

    this.userAccountsDataService
      .getUserInfoWithPost(
        new UserManagementQueryModel({
          emails: [newEmail],
          userEntityStatuses: [
            StatusTypeEnum.All.code,
            StatusTypeEnum.Deactive.code
          ]
        })
      )
      .subscribe(
        (data: any) => {
          const existedEmail = data && data.totalItems > 0;
          this.showErrorMessage(
            survey,
            options,
            'emailAddress',
            existedEmail,
            this.existedEmailErrorMessage
          );
          this.globalLoader.hideLoader();
          if (!existedEmail && this.mode === EditUserDialogModeEnum.Edit) {
            survey.currentPage = survey.visiblePageCount - 1;
          }
          options.complete();
        },
        null,
        () => this.globalLoader.hideLoader()
      );
  }

  private validateEmailBelongToBlackList(survey: any, options: any): boolean {
    const email = survey.data.emailAddress;
    if (email) {
      const invalidEmailDomain =
        INVALID_EMAIL_DOMAIN_CONST.findIndex((item: string) => {
          return email.endsWith(item);
        }) > findIndexCommon.notFound;

      this.showErrorMessage(
        survey,
        options,
        'emailAddress',
        invalidEmailDomain,
        this.invalidEmailDomainErrorMessage
      );
      if (invalidEmailDomain) {
        options.complete();
      }

      return invalidEmailDomain;
    }
  }

  private showErrorMessage(
    survey: any,
    options: any,
    questionName: string,
    hasError: boolean,
    errorMessage: string
  ): void {
    if (hasError) {
      options.errors[questionName] = errorMessage;
      const errorQuestionPageIndex = survey.getQuestionByName(questionName).page
        .visibleIndex;
      survey.currentPage = errorQuestionPageIndex;
    }
  }

  private initHelper(): void {
    this.editUserDialogHelper.init(this.user, this.currentUser);
    this.mode = this.editUserDialogHelper.getDialogMode();
    if (this.mode === EditUserDialogModeEnum.View) {
      this.userFormJSON = {
        ...this.userFormJSON,
        mode: CxSurveyJsModeEnum.Display
      };
    }
  }

  private handleOnDepartmentChanged(surveyEvent: CxSurveyjsEventModel): void {
    const departmentId = surveyEvent.options.question.value;
    if (departmentId) {
      this.departmentStoreService
        .getDepartmentTypesByDepartmentId(departmentId)
        .subscribe((departmentTypes) => {
          const systemRolesQuestion = surveyEvent.survey.getQuestionByName(
            'systemRoles'
          );
          if (systemRolesQuestion) {
            let oldSystemRoles = [];
            let newSystemRoles = [];
            let removedSystemRoles = [];
            oldSystemRoles = _.clone(systemRolesQuestion.value);
            this.setNewDepartmentAvailableRoles(departmentTypes);
            newSystemRoles = _.clone(systemRolesQuestion.value);
            removedSystemRoles = _.difference(oldSystemRoles, newSystemRoles);
            const newMappingSystemRoles = UserAccountsHelper.findMappingRemovedSystemRoles(
              removedSystemRoles,
              systemRolesQuestion.filteredChoices.map((p) => p.value)
            );
            // Set new value if any mapping of the removed one found.
            if (newMappingSystemRoles && newMappingSystemRoles.length > 0) {
              systemRolesQuestion.value = _.union(
                newSystemRoles,
                newMappingSystemRoles
              );
            }
          }
        });
    }
  }

  /**
   * Init the available roles
   */
  private initializeAvailableRoles(): void {
    const departmentId = this.userData ? this.userData.departmentId : 0;
    if (departmentId) {
      this.departmentStoreService
        .getDepartmentTypesByDepartmentId(departmentId)
        .subscribe((departmentTypes) => {
          this.setNewDepartmentAvailableRoles(departmentTypes);
        });
    }
  }

  private setNewDepartmentAvailableRoles(
    departmentTypes: DepartmentType[]
  ): void {
    const availableRoles = this.cxSurveyjsExtendedService.buildAvailableRolesByDepartmentTypes(
      departmentTypes
    );

    const currentObjectAvailableRolesVariable = this.basicInfoSurveyVariables.find(
      (variable) =>
        variable.name === SurveyVariableEnum.currentObject_availableRoles
    );
    if (currentObjectAvailableRolesVariable) {
      currentObjectAvailableRolesVariable.value = availableRoles;
    } else {
      this.basicInfoSurveyVariables.push(
        new CxSurveyjsVariable({
          name: SurveyVariableEnum.currentObject_availableRoles,
          value: availableRoles
        })
      );
    }
    this.basicInfoSurveyVariables = [...this.basicInfoSurveyVariables];
    this.changeDetectorRef.detectChanges();
  }

  private showNonAssignAOConfirmationDialog(
    onConfirmed: () => void,
    headerName: string,
    content: string
  ): void {
    const cxConfirmationDialogModalRef = this.ngbModal.open(
      CxConfirmationDialogComponent,
      {
        size: 'sm',
        centered: true
      }
    );

    const cxConfirmationDialogModal = cxConfirmationDialogModalRef.componentInstance as CxConfirmationDialogComponent;
    cxConfirmationDialogModal.showConfirmButton = true;
    cxConfirmationDialogModal.showCloseButton = true;
    cxConfirmationDialogModal.confirmButtonText = 'Confirm';
    cxConfirmationDialogModal.cancelButtonText = 'Cancel';
    cxConfirmationDialogModal.header = headerName;
    cxConfirmationDialogModal.content = content;

    cxConfirmationDialogModal.confirm.subscribe(() => {
      onConfirmed();
      cxConfirmationDialogModalRef.close();
    });
    cxConfirmationDialogModal.cancel.subscribe(() => {
      cxConfirmationDialogModalRef.close();
    });
  }
}
