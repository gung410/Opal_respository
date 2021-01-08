import { Injectable } from '@angular/core';
import {
  CxFormModal,
  CxSurveyjsFormModalOptions,
  CxSurveyjsVariable,
} from '@conexus/cx-angular-common';
import { NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { ArchetypeEnum } from 'app-enums/archetypeEnum';
import { PDOpportunityAnswerDTO } from 'app-models/mpj/pdo-action-item.model';
import { KLPPlannedAreaModel } from 'app-models/opj/klp-planned-areas.model';
import { PDPlannerHelpers } from 'app-services/idp/pd-planner/pd-planner-helpers';
import { PDOpportunityService } from 'app-services/pd-opportunity/pd-opportunity.service';
import { ObjectUtilities } from 'app-utilities/object-utils';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import { OdpService } from 'app/organisational-development/odp.service';
import { isEmpty } from 'lodash';
import { KeyLearningProgramHelper } from './key-learning-program.helper';
import { LEARNING_AREA_SELECTOR_FORM } from './learning-area-selector-form';

@Injectable()
export class KeyLearningProgramService {
  constructor(
    private formModal: CxFormModal,
    private odpService: OdpService,
    private pdOpportunity: PDOpportunityService,
    private keyLearningProgramHelper: KeyLearningProgramHelper
  ) {}

  async updateKeyLearningProgramContent(pdPlan: IdpDto): Promise<boolean> {
    const response = await this.odpService.saveKeyLearningProgram(pdPlan);
    if (response.error) {
      return false;
    }

    return !isEmpty(response.data);
  }

  buildGroupTagFromKLPData(
    klpData: any,
    taggingByTargetAudience: any,
    klpLearningAreas: any[]
  ): string[] {
    if (!klpData) {
      return [];
    }

    let groupTag = [];
    if (!isEmpty(klpData.devrole)) {
      groupTag = groupTag.concat(klpData.devrole);
    }

    if (!isEmpty(taggingByTargetAudience.DevelopmentalRole)) {
      groupTag = groupTag.concat(taggingByTargetAudience.DevelopmentalRole);
    }

    if (!isEmpty(taggingByTargetAudience.PersonnelGroup)) {
      groupTag = groupTag.concat(taggingByTargetAudience.PersonnelGroup);
    }

    if (!isEmpty(klpLearningAreas)) {
      const learningAreaIds = klpLearningAreas
        .filter((areaModel: KLPPlannedAreaModel) => !!areaModel.area)
        .map((areaModel: KLPPlannedAreaModel) => areaModel.area.id);
      groupTag = groupTag.concat(learningAreaIds);
    }

    if (!isEmpty(klpData.teachingSubjects)) {
      groupTag = groupTag.concat(klpData.teachingSubjects);
    }

    if (!isEmpty(klpData.jobFamilies)) {
      groupTag = groupTag.concat(klpData.jobFamilies);
    }

    return groupTag;
  }

  async getTaggingByTargetAudience(
    formData: any,
    userTypeArchetypes?: ArchetypeEnum[],
    getMostPopularInEachGroup?: boolean
  ): Promise<any> {
    if (!formData) {
      return;
    }

    if (formData.targetAudienceBy === 'userProfileParameters') {
      return {
        PersonnelGroup: formData.personnelGroups,
        DevelopmentalRole: formData.devrole,
      };
    }

    if (
      formData.targetAudienceBy !== 'userGroups' &&
      formData.targetAudienceBy !== 'individualLearners'
    ) {
      return {
        PersonnelGroup: [],
        DevelopmentalRole: [],
      };
    }

    return this.keyLearningProgramHelper.getTaggingByTargetAudienceOfUserGroupOrIndividualUsers(
      formData.userGroups,
      formData.individualLearners,
      userTypeArchetypes,
      getMostPopularInEachGroup
    );
  }

  getSelectedCourseIdsOnKLP(klpData: any): string[] {
    if (!klpData || isEmpty(klpData.listLearningOpportunity)) {
      return [];
    }
    const pdoAnswers: PDOpportunityAnswerDTO[] =
      klpData.listLearningOpportunity;
    const courseIds: string[] = pdoAnswers
      .filter((pdoAnswer) => !!pdoAnswer.learningOpportunity)
      .map((pdoAnswer) => {
        return PDPlannerHelpers.getCourseIdFromPDOAnswer(pdoAnswer);
      });

    return courseIds;
  }

  async getAddedPDOListOfKLP(
    klpFormData: any
  ): Promise<PDOpportunityAnswerDTO[]> {
    if (!klpFormData || isEmpty(klpFormData.listLearningOpportunity)) {
      return [];
    }
    const plannedPDOAnswersKLP: PDOpportunityAnswerDTO[] = ObjectUtilities.clone(
      klpFormData.listLearningOpportunity
    ).filter((pdoAnswer) => !!pdoAnswer.learningOpportunity);
    const catalogPDOIds = PDPlannerHelpers.getPDCataloguePDOsIdFromPDOAnswers(
      plannedPDOAnswersKLP
    );

    if (isEmpty(catalogPDOIds)) {
      return plannedPDOAnswersKLP;
    }

    const listPDODetailModel = await this.pdOpportunity.getPDCatalogPDODetailListAsync(
      catalogPDOIds
    );
    PDPlannerHelpers.updatePDOAnswersInfo(
      plannedPDOAnswersKLP,
      listPDODetailModel
    );

    return plannedPDOAnswersKLP;
  }

  checkPDOExisted(
    courseId: string,
    pdoAnswers: PDOpportunityAnswerDTO[]
  ): boolean {
    const result = pdoAnswers.find(
      (answer) =>
        answer &&
        answer.learningOpportunity &&
        answer.learningOpportunity.uri &&
        answer.learningOpportunity.uri.includes(courseId)
    );

    return !!result;
  }

  showLearningAreaSelectorPopup(personnelGroupIds: string[]): NgbModalRef {
    const title = 'Add Learning Areas';
    const formJSON = LEARNING_AREA_SELECTOR_FORM;
    const variables: CxSurveyjsVariable[] = [];
    variables.push(
      new CxSurveyjsVariable({
        name: 'parentObject',
        value: { personnelGroupIds },
      })
    );
    const options = new CxSurveyjsFormModalOptions({
      variables,
      fixedButtonsFooter: true,
      fixedHeight: true,
      showModalHeader: true,
      modalHeaderText: title,
      cancelName: 'Cancel',
      submitName: 'Add',
    });

    const modalRef = this.formModal.openSurveyJsForm(
      formJSON,
      null,
      [],
      options,
      {
        size: 'lg',
        centered: true,
        windowClass: 'mobile-dialog-slide-right',
      }
    );

    return modalRef;
  }
}
