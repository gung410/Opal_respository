import { Identity } from 'app-models/common.model';
import {
  ExternalPDOExtensions,
  PDOActionItemDTO,
  PDOAddType,
  PDOpportunityAnswerDTO,
  PDOpportunityDTO,
  PDOpportunityModel,
  PDOSource,
} from 'app-models/mpj/pdo-action-item.model';
import { PDPlanDto } from 'app-models/pdplan.model';
import {
  PDODetailInfo,
  PDOpportunityDetailModel,
  PDOpportunityStatusEnum,
} from 'app-services/pd-opportunity/pd-opportunity-detail.model';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { ObjectUtilities } from 'app-utilities/object-utils';
import { Utilities } from 'app-utilities/utilities';
import { IdpStatusCodeEnum } from 'app/individual-development/idp.constant';
import { PDCatalogCourseModel } from 'app/individual-development/models/opportunity.model';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import { Constant } from 'app/shared/app.constant';
import { isEmpty } from 'lodash';

export class PDPlannerHelpers {
  static toPDOpportunityDTO(
    opportunityDTO: PDOpportunityDetailModel
  ): PDOpportunityDTO {
    const learningOpportunityDTO: PDOpportunityDTO = {
      uri: PDPlannerHelpers.generateUriPDO(opportunityDTO.id),
      source: PDOSource.CoursePadPDO,
    };

    return learningOpportunityDTO;
  }

  static toPDOpportunityAnswerDTO(
    pdCatalogCourse: PDCatalogCourseModel
  ): PDOpportunityAnswerDTO {
    const pdOpportunityDTO = PDPlannerHelpers.toPDOpportunityDTO(
      pdCatalogCourse.course
    );
    const opportunityAnswer: PDOpportunityAnswerDTO = {
      learningOpportunity: pdOpportunityDTO,
    };

    return opportunityAnswer;
  }

  static toPDOpportunityAnswerDTOForBE(
    course: PDOpportunityDetailModel
  ): PDOpportunityAnswerDTO {
    const pdOpportunityDTO = PDPlannerHelpers.toPDOpportunityDTO(course);
    const opportunityAnswer: PDOpportunityAnswerDTO = {
      learningOpportunity: pdOpportunityDTO,
    };

    return opportunityAnswer;
  }

  static toPDOpportunityAnswerDTOFromCourseDetail(
    course: PDOpportunityDetailModel
  ): PDOpportunityAnswerDTO {
    const pdoAnswer = PDPlannerHelpers.toPDOpportunityAnswerDTOForBE(course);
    PDPlannerHelpers.updateCoursePadPDOInfo(course, pdoAnswer);

    return pdoAnswer;
  }

  static externalToPDOpportunityDTO(
    pdoRawData: object,
    uri?: string
  ): PDOpportunityDTO {
    const processedPDOData = ObjectUtilities.fieldWithDotToObject(pdoRawData);
    const learningOpportunityDTO = processedPDOData as PDOpportunityDTO;
    const extensions = learningOpportunityDTO.extensions as ExternalPDOExtensions;

    if (extensions) {
      if (extensions.startDate) {
        extensions.startDate = DateTimeUtil.surveyToServerFormat(
          extensions.startDate
        );
      }

      if (extensions.endDate) {
        extensions.endDate = DateTimeUtil.surveyToServerFormat(
          extensions.endDate
        );
      }
    }

    learningOpportunityDTO.source = PDOSource.CustomPDO;
    learningOpportunityDTO.uri = uri ? uri : PDPlannerHelpers.generateUriPDO();

    return learningOpportunityDTO;
  }

  static toSurveyExternalPDOData(pdoDTO: PDOpportunityDTO): object {
    if (!pdoDTO) {
      return;
    }
    const extensions = pdoDTO.extensions as ExternalPDOExtensions;
    if (extensions) {
      delete extensions.tags;
      if (extensions.startDate) {
        const result = DateTimeUtil.toSurveyFormat(extensions.startDate);
        extensions.startDate =
          result === Constant.INVALID_DATE_STRING
            ? extensions.startDate
            : result;
      }

      if (extensions.endDate) {
        const result = DateTimeUtil.toSurveyFormat(extensions.endDate);
        extensions.endDate =
          result === Constant.INVALID_DATE_STRING ? extensions.endDate : result;
      }
    }

    const externalPDOData = ObjectUtilities.objectToFieldWithDot(pdoDTO, [
      'extensions',
    ]);

    return externalPDOData;
  }

  static generateNewPlan(userIdentity: Identity): IdpDto {
    const newPlan = new IdpDto();
    newPlan.objectiveInfo = {
      identity: userIdentity,
    };
    newPlan.assessmentStatusInfo = {
      assessmentStatusCode: IdpStatusCodeEnum.NotStarted,
    };

    return newPlan;
  }

  static toPDOpportunityModel(
    actionItem: PDOActionItemDTO
  ): PDOpportunityModel {
    if (!actionItem) {
      return;
    }

    return {
      answerDTO: actionItem.answer,
      identityActionItemDTO: actionItem.resultIdentity,
      assessmentStatusInfo: actionItem.assessmentStatusInfo,
      createdBy: actionItem.createdBy,
      additionalProperties: actionItem.additionalProperties,
      canDelete: actionItem.canDelete,
    };
  }

  static toPDOpportunityAnswer(
    learningOpportunityDTO: PDOpportunityDTO
  ): PDOpportunityAnswerDTO {
    const opportunityAnswer: PDOpportunityAnswerDTO = {
      learningOpportunity: learningOpportunityDTO,
    };

    return opportunityAnswer;
  }

  static generatePDOActionItem(
    answer: PDOpportunityAnswerDTO,
    learnerIdentity: Identity
  ): PDOActionItemDTO {
    const itemDTO = new PDOActionItemDTO();
    itemDTO.objectiveInfo = {
      identity: learnerIdentity,
    };
    itemDTO.answer = answer;
    itemDTO.forceCreateResult = true;
    itemDTO.additionalProperties = {
      type: PDOAddType.SelfRegistered,
      learningOpportunityUri: answer.learningOpportunity.uri,
    };

    return itemDTO;
  }

  static sortPlanByCreatedDate(plans: IdpDto[]): IdpDto[] {
    return plans.sort((plan1, plan2) => {
      const date1 = +new Date(plan1.created);
      const date2 = +new Date(plan2.created);

      return date1 - date2;
    });
  }

  static toArrayPDOpportunityModel(
    pdoList: PDOActionItemDTO[]
  ): PDOpportunityModel[] {
    if (isEmpty(pdoList)) {
      return [];
    }

    return pdoList.map((pdo) => PDPlannerHelpers.toPDOpportunityModel(pdo));
  }

  static generateUriPDO(courseId?: string): string {
    const system = Constant.MOE_SYSTEM_URI;
    const object = courseId ? PDOSource.CoursePadPDO : PDOSource.CustomPDO;
    const id = courseId ? courseId : Utilities.generateGUID();
    const uri = `${system}:${object}:${id}`;

    return uri;
  }

  static getCourseIdFromURI(uri: string): string {
    if (!uri) {
      return;
    }

    const isExternalPDOUri = uri.includes('custom-pdo');
    const isPDCatalogPDOUri = this.isExternalPDOUri(uri);
    if (!isExternalPDOUri && !isPDCatalogPDOUri) {
      return;
    }

    const regex = isPDCatalogPDOUri
      ? /coursepad-pdo:(.*)$/
      : /custom-pdo:(.*)$/;
    const matches = uri.match(regex);
    if (matches && matches[1]) {
      return matches[1] ? matches[1].toLocaleLowerCase() : undefined;
    }

    return undefined;
  }

  static isExternalPDOUri(uri: string): boolean {
    return uri.includes('coursepad-pdo');
  }

  static getCourseIdFromPDOAnswer(pdoAnswer: PDOpportunityAnswerDTO): string {
    if (!pdoAnswer || !pdoAnswer.learningOpportunity) {
      return;
    }
    const uri = pdoAnswer.learningOpportunity.uri;

    return PDPlannerHelpers.getCourseIdFromURI(uri);
  }

  static getPDPlanSamePeriodFromPDPlans(
    pdplan: PDPlanDto,
    pdplans: PDPlanDto[]
  ): PDPlanDto {
    if (!pdplan || !pdplans || !pdplans.length) {
      return;
    }

    return pdplans.find((plan) =>
      PDPlannerHelpers.isSamePeriodPlan(pdplan, plan)
    );
  }

  static isSamePeriodPlan(pdplan1: PDPlanDto, pdplan2: PDPlanDto): boolean {
    if (!pdplan1 || !pdplan2 || !pdplan1.surveyInfo || !pdplan2.surveyInfo) {
      return false;
    }
    const pdplanDate1 = new Date(pdplan1.surveyInfo.startDate);
    const pdplanDate2 = new Date(pdplan2.surveyInfo.startDate);
    if (!pdplanDate1 || !pdplanDate2) {
      return false;
    }

    return pdplanDate1.getFullYear() === pdplanDate2.getFullYear();
  }

  static getIdFromPDOModel(pdoModel: PDOpportunityModel): string {
    if (pdoModel && pdoModel.answerDTO) {
      return PDPlannerHelpers.getIdFromPDOAnswer(pdoModel.answerDTO);
    }
  }

  static getIdFromPDOAnswer(pdoAnswer: PDOpportunityAnswerDTO): string {
    if (
      pdoAnswer &&
      pdoAnswer.learningOpportunity &&
      pdoAnswer.learningOpportunity.uri
    ) {
      return PDPlannerHelpers.getCourseIdFromURI(
        pdoAnswer.learningOpportunity.uri
      );
    }
  }

  static isExternalPDO(pdoModel: PDOpportunityModel): boolean {
    if (pdoModel) {
      return PDPlannerHelpers.isExternalPDOByAnswer(pdoModel.answerDTO);
    }

    return false;
  }

  static isExternalPDOByAnswer(answerDTO: PDOpportunityAnswerDTO): boolean {
    if (
      answerDTO &&
      answerDTO.learningOpportunity &&
      answerDTO.learningOpportunity.source === PDOSource.CustomPDO
    ) {
      return true;
    }

    return false;
  }

  static isCoursePadPDOByPDOModel(pdoModel: PDOpportunityModel): boolean {
    if (pdoModel && pdoModel.answerDTO) {
      return PDPlannerHelpers.isCoursePadPDOByPDOAnswer(pdoModel.answerDTO);
    }

    return false;
  }

  static isCoursePadPDOByPDOAnswer(pdoAnswer: PDOpportunityAnswerDTO): boolean {
    if (
      pdoAnswer &&
      pdoAnswer.learningOpportunity &&
      pdoAnswer.learningOpportunity.source === PDOSource.CoursePadPDO
    ) {
      return true;
    }

    return false;
  }

  static getPDCataloguePDOsIdFromPDOModels(
    pdoModelList: PDOpportunityModel[]
  ): string[] {
    const pdCataloguelPDOIds = pdoModelList
      .filter(PDPlannerHelpers.isCoursePadPDOByPDOModel)
      .map(PDPlannerHelpers.getIdFromPDOModel);

    return pdCataloguelPDOIds;
  }

  static getPDCataloguePDOsIdFromPDOAnswers(
    pdoAnswer: PDOpportunityAnswerDTO[]
  ): string[] {
    const pdCataloguelPDOIds = pdoAnswer
      .filter(PDPlannerHelpers.isCoursePadPDOByPDOAnswer)
      .map(PDPlannerHelpers.getIdFromPDOAnswer);

    return pdCataloguelPDOIds;
  }

  static updateCoursePadPDOInfo(
    pdoDetail: PDOpportunityDetailModel,
    pdoAnswer: PDOpportunityAnswerDTO
  ): PDOpportunityDTO {
    const pdoData = pdoAnswer.learningOpportunity;

    if (!pdoDetail) {
      pdoData.name = 'N/A';

      return pdoData;
    }

    pdoData.name = pdoDetail.name;
    pdoData.thumbnailUrl = pdoDetail.thumbnail;

    if (!pdoData.extensions) {
      pdoData.extensions = {};
    }

    pdoData.extensions.description = pdoDetail.description;
    pdoData.extensions.duration = pdoDetail.duration;
    pdoData.extensions.tags = pdoDetail.tags;
    pdoData.extensions.courseCode = pdoDetail.code;

    return pdoData;
  }

  static updateTagForExternalPDO(pdoDto: PDOpportunityDTO): void {
    if (!pdoDto || !pdoDto.extensions) {
      return;
    }
    const extensions: ExternalPDOExtensions = pdoDto.extensions;
    extensions.tags = [];

    if (
      extensions.pdOpportunityType &&
      extensions.pdOpportunityType.displayText
    ) {
      extensions.tags.push(extensions.pdOpportunityType.displayText);
    }
    if (extensions.mode && extensions.mode.displayText) {
      extensions.tags.push(extensions.mode.displayText);
    }
    if (!isEmpty(pdoDto.subject)) {
      const firstItem = pdoDto.subject[0];
      if (firstItem.displayText) {
        extensions.tags.push(firstItem.displayText);
      }
    }
  }

  static updatePDOAnswersInfo(
    pdoAnswers: PDOpportunityAnswerDTO[],
    courseDetails: PDOpportunityDetailModel[]
  ): void {
    if (isEmpty(pdoAnswers) || isEmpty(courseDetails)) {
      return;
    }

    for (const pdoAnswer of pdoAnswers) {
      if (!pdoAnswer || !pdoAnswer.learningOpportunity) {
        continue;
      }
      const learningOpportunity = pdoAnswer.learningOpportunity;
      const pdoSource = learningOpportunity.source;

      if (pdoSource === PDOSource.CustomPDO) {
        PDPlannerHelpers.updateTagForExternalPDO(learningOpportunity);
      }

      if (pdoSource === PDOSource.CoursePadPDO) {
        const plannedPDOId = PDPlannerHelpers.getIdFromPDOAnswer(pdoAnswer);
        const courseDetail = courseDetails.find(
          (course) => course.id === plannedPDOId
        );
        pdoAnswer.learningOpportunity = PDPlannerHelpers.updateCoursePadPDOInfo(
          courseDetail,
          pdoAnswer
        );
      }
    }
  }

  static updatePDOpportunityModelInfo(
    pdoModels: PDOpportunityModel[],
    courseDetails: PDOpportunityDetailModel[]
  ): void {
    if (isEmpty(pdoModels) || isEmpty(courseDetails)) {
      return;
    }

    for (const pdoModel of pdoModels) {
      const pdoAnswer = pdoModel.answerDTO;

      if (!pdoAnswer || !pdoAnswer.learningOpportunity) {
        continue;
      }

      const learningOpportunity = pdoAnswer.learningOpportunity;
      const pdoSource = learningOpportunity.source;

      if (pdoSource === PDOSource.CustomPDO) {
        PDPlannerHelpers.updateTagForExternalPDO(learningOpportunity);
      }

      if (pdoSource === PDOSource.CoursePadPDO) {
        const plannedPDOId = PDPlannerHelpers.getIdFromPDOAnswer(pdoAnswer);
        const courseDetail = courseDetails.find(
          (course) => course.id === plannedPDOId
        );
        pdoAnswer.learningOpportunity = PDPlannerHelpers.updateCoursePadPDOInfo(
          courseDetail,
          pdoAnswer
        );

        if (courseDetail) {
          pdoModel.unPublished =
            courseDetail.status !== PDOpportunityStatusEnum.Published;
        }
      }
    }
  }

  static generatePDODetailFromExternalPDODto(
    externalPDODTO: PDOpportunityDTO
  ): PDOpportunityDetailModel {
    const pdoDetailModel = new PDOpportunityDetailModel();
    pdoDetailModel.id = externalPDODTO.uri;
    pdoDetailModel.name = externalPDODTO.name;
    pdoDetailModel.thumbnail = externalPDODTO.thumbnailUrl;
    const pdoExtension: ExternalPDOExtensions = externalPDODTO.extensions;
    pdoDetailModel.duration = pdoExtension.duration;
    pdoDetailModel.description = pdoExtension.description;
    pdoDetailModel.costForMOELearner = pdoExtension.cost;
    pdoDetailModel.costForNonMOELearner = pdoExtension.cost;
    pdoDetailModel.infos = [];

    if (!pdoExtension) {
      return;
    }

    if (!isEmpty(pdoExtension.pdOpportunityType)) {
      pdoDetailModel.infos.push(
        new PDODetailInfo(
          'Type of PD Activity',
          pdoExtension.pdOpportunityType.displayText
        )
      );
    }

    if (!isEmpty(pdoExtension.mode)) {
      pdoDetailModel.infos.push(
        new PDODetailInfo('Mode of Learning', pdoExtension.mode.displayText)
      );
    }

    if (!isEmpty(pdoExtension.courseNature)) {
      pdoDetailModel.infos.push(
        new PDODetailInfo(
          'Nature of Course',
          pdoExtension.courseNature.displayText
        )
      );
    }

    if (!isEmpty(pdoExtension.capacity)) {
      pdoDetailModel.infos.push(
        new PDODetailInfo('Capacity of Attendee', pdoExtension.capacity)
      );
    }

    if (!isEmpty(pdoExtension.organiser)) {
      pdoDetailModel.infos.push(
        new PDODetailInfo('Name of Organiser', pdoExtension.organiser)
      );
    }

    if (!isEmpty(pdoExtension.venue)) {
      pdoDetailModel.infos.push(
        new PDODetailInfo('Locality of PD Opportunity', pdoExtension.venue)
      );
    }

    return pdoDetailModel;
  }
}
