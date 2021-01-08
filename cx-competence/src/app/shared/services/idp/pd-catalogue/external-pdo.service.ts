import { Injectable } from '@angular/core';
import {
  CxFormModal,
  CxSurveyjsFormModalOptions,
  CxSurveyJsUtil,
  CxSurveyjsVariable,
} from '@conexus/cx-angular-common';
import { NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from 'app-auth/auth.service';
import { UserTypeEnum } from 'app-enums/userType.enum';
import { CxSurveyDataModel } from 'app-models/mpj/cx-survey-data.model';
import { PDOpportunityDTO } from 'app-models/mpj/pdo-action-item.model';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { UserService } from 'app-services/user.service';
import { SurveyVariableEnum } from 'app/shared/constants/survey-variable.enum';
import { isEmpty } from 'lodash';
import { IDPService } from '../idp.service';
import { PDPlannerHelpers } from '../pd-planner/pd-planner-helpers';
import { LearnerInfoDTO } from './../../../models/common/learner-info.model';

@Injectable()
export class ExternalPDOService {
  private externalPDOForm: object;

  constructor(
    private idpService: IDPService,
    private translateService: TranslateAdapterService,
    private formModal: CxFormModal,
    private userService: UserService,
    private authService: AuthService
  ) {}

  async showExternalPDOFormAsync(
    personnelGroupsIds: string[],
    pdoData?: PDOpportunityDTO,
    displayApproval?: boolean
  ): Promise<NgbModalRef> {
    const surveyDataExternalPdoModel = await this.generateExternalPDOCxSurveyDataAync(
      personnelGroupsIds,
      pdoData
    );
    if (!surveyDataExternalPdoModel) {
      return;
    }

    const title = pdoData
      ? this.translateService.getValueImmediately(
          'MyPdJourney.PlannedActivities.EditExternalPDO'
        )
      : this.translateService.getValueImmediately(
          'MyPdJourney.PlannedActivities.AddExternalPDO'
        );
    surveyDataExternalPdoModel.variables.push(
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.formDisplayMode,
        value: pdoData ? 'edit' : 'create',
      })
    );

    surveyDataExternalPdoModel.variables.push(
      new CxSurveyjsVariable({
        name: SurveyVariableEnum.displayApproval,
        value: displayApproval ? true : false,
      })
    );

    const options = new CxSurveyjsFormModalOptions({
      fixedButtonsFooter: true,
      fixedHeight: true,
      showModalHeader: true,
      modalHeaderText: title,
      variables: surveyDataExternalPdoModel.variables,
      cancelName: surveyDataExternalPdoModel.cancelName,
      submitName: surveyDataExternalPdoModel.submitName,
    });

    const admins =
      pdoData && pdoData.extensions.externalPDOApprovingOfficerExtId
        ? [
            await this.getAdmin(
              pdoData.extensions.externalPDOApprovingOfficerExtId
            ),
          ]
        : await this.getAdmins([this.authService.userDepartmentId]);

    if (admins && admins.length > 0) {
      CxSurveyJsUtil.addProperty(
        surveyDataExternalPdoModel.json,
        'extensions.externalPDOApprovingOfficerExtId',
        'choices',
        admins.map((admin) => ({
          value: admin.userCxId,
          text: admin.fullName,
        }))
      );
      surveyDataExternalPdoModel.json = { ...surveyDataExternalPdoModel.json };
    }

    const modalRef = this.formModal.openSurveyJsForm(
      surveyDataExternalPdoModel.json,
      surveyDataExternalPdoModel.data,
      [],
      options,
      {
        size: 'lg',
        centered: true,
        backdrop: 'static',
        windowClass: 'mobile-dialog-slide-right external-pdo-dialog',
      }
    );

    return modalRef;
  }

  async getAdmins(departmentIds: number[]): Promise<LearnerInfoDTO[]> {
    const result = await this.userService.getUserBasicInfoAsync(departmentIds, [
      UserTypeEnum.schooladmin,
      UserTypeEnum.divisionadmin,
      UserTypeEnum.branchadmin,
    ]);

    return result.data.items;
  }

  async getAdmin(userExtId: string): Promise<LearnerInfoDTO> {
    let admins = [];
    if (userExtId) {
      admins = await this.userService.getUserInfoAsync([userExtId]);
    }

    return admins && admins.length > 0 ? admins[0] : undefined;
  }

  private async generateExternalPDOCxSurveyDataAync(
    personnelGroupsIds: string[],
    pdoData?: PDOpportunityDTO
  ): Promise<CxSurveyDataModel> {
    const jsonForm = await this.getExternalPDOFormAsync();
    if (!jsonForm) {
      console.error("Can't get external PDO register form");

      return;
    }
    const surveyjsVariables = [];

    surveyjsVariables.push(
      new CxSurveyjsVariable({
        name: 'hasPersonnelGroup',
        value: !isEmpty(personnelGroupsIds) ? true : false,
      })
    );

    if (!isEmpty(personnelGroupsIds)) {
      surveyjsVariables.push(
        new CxSurveyjsVariable({
          name: 'personnelGroups',
          value: personnelGroupsIds,
        })
      );
    }

    const surveyData = PDPlannerHelpers.toSurveyExternalPDOData(pdoData);
    const cancelName = this.translateService.getValueImmediately(
      'Common.Action.Cancel'
    );
    const submitName = surveyData
      ? this.translateService.getValueImmediately('Common.Action.Save')
      : this.translateService.getValueImmediately('Common.Action.Add');

    return {
      json: jsonForm,
      variables: surveyjsVariables,
      cancelName,
      submitName,
      data: surveyData,
    };
  }

  private async getExternalPDOFormAsync(): Promise<object> {
    if (!this.externalPDOForm) {
      const response = await this.idpService.getActionItemsConfig();
      this.externalPDOForm =
        !response.error && response.data
          ? response.data.configuration
          : undefined;
    }

    return this.externalPDOForm;
  }
}
