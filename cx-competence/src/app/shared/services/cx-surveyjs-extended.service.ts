import { Injectable } from '@angular/core';
import { CxSurveyjsService } from '@conexus/cx-angular-common';
import { User } from 'app-models/auth.model';
import { PdPlanType } from 'app-models/pdplan.model';
import { DepartmentStoreService } from 'app/core/store-services/department-store.service';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { AppConstant } from '../app.constant';
import { PDCatalogueConstant } from '../constants/pdcatalogue.const';
import { SurveyVariableEnum } from '../constants/survey-variable.enum';

@Injectable({
  providedIn: 'root',
})
export class CxSurveyjsExtendedService {
  constructor(
    private cxSurveyjsService: CxSurveyjsService,
    private departmentStoreService: DepartmentStoreService
  ) {}

  setCurrentUserVariables(user: User): void {
    const variables = {};
    variables[SurveyVariableEnum.currentUser_systemRoles] = !user.systemRoles
      ? []
      : user.systemRoles.map((role) => role.identity.extId);
    variables[SurveyVariableEnum.currentUser_extId] = user.extId;
    this.cxSurveyjsService.setVariables(variables);
  }

  setCurrentDepartmentVariables(departmentId: number): void {
    this.departmentStoreService
      .getDepartmentById(departmentId)
      .subscribe((department) => {
        if (department) {
          const variables = {};
          variables[SurveyVariableEnum.currentDepartment_id] =
            department.identity.id;
          variables[SurveyVariableEnum.currentDepartment_name] =
            department.departmentName;
          variables[SurveyVariableEnum.currentDepartment_archetype] =
            department.identity.archetype;
          this.cxSurveyjsService.setVariables(variables);
        }
      });
    this.departmentStoreService
      .getDepartmentTypesByDepartmentId(departmentId)
      .subscribe((departmentTypes) => {
        const variables = {};
        variables[SurveyVariableEnum.currentDepartment_types] = departmentTypes
          ? departmentTypes.map((p) => p.identity.extId)
          : [];
        this.cxSurveyjsService.setVariables(variables);
      });
  }

  setCurrentObjectVariables(obj: any): void {
    const variables = {};
    // TODO: Remove this surveyTimeStamp since the new version of Survey JS supporting to not cache the api request.
    // This variable use to put to url of choiceByUrl, to disable cache request
    variables[
      SurveyVariableEnum.surveyTimeStamp
    ] = new Date().getTime().toString();

    if (this.isODPAssessment(obj) || this.isIDPAssessment(obj)) {
      const assessmentInfo = obj.assessmentStatusInfo;
      variables[SurveyVariableEnum.currentObject_id] = obj.resultIdentity
        ? obj.resultIdentity.id
        : null;
      variables[SurveyVariableEnum.currentObject_extId] = obj.resultIdentity
        ? obj.resultIdentity.extId
        : null;
      variables[
        SurveyVariableEnum.currentObject_entityStatus
      ] = obj.entityStatus ? obj.entityStatus.statusId : null;
      variables[SurveyVariableEnum.currentObject_statusType] = assessmentInfo
        ? assessmentInfo.assessmentStatusCode
        : null;
      variables[SurveyVariableEnum.currentObject_isExternallyMastered] = false; // We don't have the externally master for assessment now.

      this.cxSurveyjsService.setVariables(variables);
      if (this.isODPAssessment(obj)) {
        if (obj.objectiveInfo && obj.objectiveInfo.identity) {
          this.setCurrentObject_DepartmentVariables(
            obj.objectiveInfo.identity.id
          );
        }
        // Reset the currentObject_userTypes to an empty list since it doesn't relate to the ODP assessment.
        this.setCurrentObject_userTypeVariables(null);
      } else {
        this.setCurrentObject_userTypeVariables(obj.currentObjectUser);
      }
    } else {
      variables[SurveyVariableEnum.currentObject_id] = obj.identity
        ? obj.identity.id
        : null;
      variables[SurveyVariableEnum.currentObject_extId] = obj.identity
        ? obj.identity.extId
        : null;
      variables[
        SurveyVariableEnum.currentObject_entityStatus
      ] = obj.entityStatus ? obj.entityStatus.statusId : null;
      variables[
        SurveyVariableEnum.currentObject_isExternallyMastered
      ] = obj.entityStatus ? obj.entityStatus.externallyMastered : false;
      this.cxSurveyjsService.setVariables(variables);
      this.setCurrentObject_DepartmentVariables(obj.departmentId);
    }
  }

  setPDCatalogueVariables(): void {
    const cxSurveyjsVariable = {};
    Object.keys(PDCatalogueConstant).forEach((key: string) => {
      cxSurveyjsVariable[PDCatalogueConstant[key].variableCode] =
        PDCatalogueConstant[key].code;
    });
    this.cxSurveyjsService.setVariables(cxSurveyjsVariable);
  }

  setCurrentObject_DepartmentVariables(departmentId: number): void {
    if (departmentId) {
      this.departmentStoreService
        .getDepartmentById(departmentId)
        .subscribe((department) => {
          if (department) {
            const variables = {};
            variables[SurveyVariableEnum.currentObject_departmentId] =
              department.identity.id;
            variables[SurveyVariableEnum.currentObject_departmentName] =
              department.departmentName;
            variables[SurveyVariableEnum.currentObject_departmentArchetype] =
              department.identity.archetype;
            this.cxSurveyjsService.setVariables(variables);
          }
        });
      this.departmentStoreService
        .getDepartmentTypesByDepartmentId(departmentId)
        .subscribe((departmentTypes) => {
          const variables = {};
          variables[
            SurveyVariableEnum.currentObject_departmentTypes
          ] = departmentTypes
            ? departmentTypes.map((p) => p.identity.extId)
            : [];
          this.cxSurveyjsService.setVariables(variables);
        });
    } else {
      const variables = {};
      variables[SurveyVariableEnum.currentObject_departmentId] = null;
      variables[SurveyVariableEnum.currentObject_departmentName] = '';
      variables[SurveyVariableEnum.currentObject_departmentArchetype] = null;
      variables[SurveyVariableEnum.currentObject_departmentTypes] = [];
    }
  }

  setCurrentObject_userTypeVariables(user: Staff): void {
    const variables = {};
    let userTypeExtIds = [];
    let userServiceSchemeExtIds = [];
    let userDevelopmentalRoleExtIds = [];
    let userLearningFrameworkExtIds = [];
    if (user) {
      const systemRoleExtIds = user.systemRoleInfos
        ? user.systemRoleInfos.map((p) => p.identity.extId)
        : [];
      const careerPathExtIds = user.careerPaths
        ? user.careerPaths.map((p) => p.identity.extId)
        : [];
      const experienceCategoryExtIds = user.experienceCategories
        ? user.experienceCategories.map((p) => p.identity.extId)
        : [];
      userServiceSchemeExtIds = user.personnelGroups
        ? user.personnelGroups.map((p) => p.identity.extId)
        : [];
      userDevelopmentalRoleExtIds = user.developmentalRoles
        ? user.developmentalRoles.map((p) => p.identity.extId)
        : [];
      userLearningFrameworkExtIds = user.learningFrameworks
        ? user.learningFrameworks.map((p) => p.identity.extId)
        : [];
      userTypeExtIds = systemRoleExtIds
        .concat(userServiceSchemeExtIds)
        .concat(userDevelopmentalRoleExtIds)
        .concat(careerPathExtIds)
        .concat(experienceCategoryExtIds);
    }

    variables[SurveyVariableEnum.currentObject_userTypes] = userTypeExtIds;
    variables[
      SurveyVariableEnum.currentObject_userServiceSchemes
    ] = userServiceSchemeExtIds;
    variables[
      SurveyVariableEnum.currentObject_userDevelopmentalRoles
    ] = userDevelopmentalRoleExtIds;
    variables[
      SurveyVariableEnum.currentObject_userLearningFrameworks
    ] = userLearningFrameworkExtIds;
    this.cxSurveyjsService.setVariables(variables);
  }

  setAPIVariables(): void {
    const variables = {};
    variables[SurveyVariableEnum.competenceApi_BaseUrl] =
      AppConstant.api.competence;
    variables[SurveyVariableEnum.organizationApi_BaseUrl] =
      AppConstant.api.organization;
    variables[SurveyVariableEnum.assessmentApi_BaseUrl] =
      AppConstant.api.assessment;
    variables[SurveyVariableEnum.learningcatalogApi_BaseUrl] =
      AppConstant.api.learningcatalog;
    this.cxSurveyjsService.setVariables(variables);
  }

  getVariable(name: string): string {
    const matchingVariable = this.cxSurveyjsService.variables.find(
      (p) => p.name === name
    );

    return matchingVariable ? matchingVariable.value : null;
  }

  initCxSurveyVariable(user: User): void {
    this.setAPIVariables();
    this.setCurrentUserVariables(user);
    this.setCurrentDepartmentVariables(user.departmentId);
    this.setPDCatalogueVariables();
  }

  /**
   * Removes a survey form property.
   * @param surveyFormJson The survey form json.
   * @param questionName The question name which is searching for.
   * @param propertyName The property of the question for deleting.
   */
  removeSurveyFormProperty(
    surveyFormJson: any,
    questionName: string,
    propertyName: string
  ): void {
    const pages = surveyFormJson ? surveyFormJson.pages : [];
    const numberOfPages = pages.length;
    let question;
    for (
      let pageIndex = 0;
      pageIndex < numberOfPages && !question;
      pageIndex++
    ) {
      const elements = pages[pageIndex].elements;
      question = this.findQuestion(elements, questionName);
    }
    if (question) {
      delete question[propertyName];
    }
  }

  private findQuestion(elements: any[], questionName: string): any {
    if (!elements) {
      return;
    }

    // tslint:disable-next-line:prefer-for-of
    for (let elementIndex = 0; elementIndex < elements.length; elementIndex++) {
      const element = elements[elementIndex];
      if (element.name === questionName) {
        return element;
      } else if (element.type === 'panel' && element.elements) {
        const foundQuestion = this.findQuestion(element.elements, questionName);
        if (foundQuestion) {
          return foundQuestion;
        }
      }
    }
  }

  private isODPAssessment(obj: any): boolean {
    return obj.pdPlanType === PdPlanType.Odp;
  }

  private isIDPAssessment(obj: any): boolean {
    return obj.pdPlanType === PdPlanType.Idp;
  }
}
