import { isEmpty } from 'lodash';
import { MetadataTagModel } from './metadata-tag.model';
import { PDOpportunityDetailGetDTO } from './pd-opportunity-detail.dto';

export class PDOpportunityDetailModel {
  id?: string;
  code?: string;
  name?: string;
  thumbnail?: string;
  duration?: number;
  description?: string;
  costForMOELearner?: number;
  costForNonMOELearner?: number;
  objective?: string;
  infos?: PDODetailInfo[];
  tags?: string[];
  status?: PDOpportunityStatusEnum;
  pdActivityType?: string;

  registrationMethod?: string;
  learningMode?: string;
  constructor(
    pdoOportunityDetailGetDTO?: PDOpportunityDetailGetDTO,
    metadataTags?: MetadataTagModel[]
  ) {
    if (!pdoOportunityDetailGetDTO) {
      return;
    }
    this.id = pdoOportunityDetailGetDTO.id
      ? pdoOportunityDetailGetDTO.id.toLocaleLowerCase()
      : undefined;
    this.name = pdoOportunityDetailGetDTO.courseName;
    this.thumbnail = pdoOportunityDetailGetDTO.thumbnailUrl;
    this.duration = this.processDuration(pdoOportunityDetailGetDTO);
    this.description = pdoOportunityDetailGetDTO.description;
    this.costForMOELearner = pdoOportunityDetailGetDTO.notionalCost;
    this.costForNonMOELearner = pdoOportunityDetailGetDTO.courseFee;
    this.code = pdoOportunityDetailGetDTO.courseCode;
    this.objective = pdoOportunityDetailGetDTO.courseObjective;
    this.registrationMethod = pdoOportunityDetailGetDTO.registrationMethod;
    this.learningMode = pdoOportunityDetailGetDTO.learningMode;
    this.status = this.processStatus(pdoOportunityDetailGetDTO);
    this.pdActivityType = pdoOportunityDetailGetDTO.pdActivityType;
    if (metadataTags) {
      this.setTag(metadataTags, pdoOportunityDetailGetDTO);
      this.setInfo(metadataTags, pdoOportunityDetailGetDTO);
    }
  }

  public get isMicrolearning(): boolean {
    return (
      this.pdActivityType && this.pdActivityType === MetadataId.Microlearning
    );
  }

  private processStatus(
    pdoOportunityDetailGetDTO: PDOpportunityDetailGetDTO
  ): PDOpportunityStatusEnum {
    if (pdoOportunityDetailGetDTO.status === 'Published') {
      return PDOpportunityStatusEnum.Published;
    }

    return;
  }

  private processDuration(
    pdoOportunityDetailGetDTO: PDOpportunityDetailGetDTO
  ): number {
    const numberMinutePerHour = 60;
    let duration = 0;

    if (pdoOportunityDetailGetDTO.durationHours > 0) {
      duration += pdoOportunityDetailGetDTO.durationHours;
    }

    if (pdoOportunityDetailGetDTO.durationMinutes > 0) {
      duration +=
        pdoOportunityDetailGetDTO.durationMinutes / numberMinutePerHour;
    }

    return this.roundTwoDecimalNumber(duration);
  }

  private roundTwoDecimalNumber(num: number): number {
    const oneHundredPercent = 100;

    return (
      Math.round((num + Number.EPSILON) * oneHundredPercent) / oneHundredPercent
    );
  }

  private setTag(
    metadataTags: MetadataTagModel[],
    pdoOportunityDetailGetDTO: PDOpportunityDetailGetDTO
  ): void {
    if (isEmpty(metadataTags)) {
      return;
    }

    this.tags = [];
    this.infos = [];
    const tagIds = [];
    const pdActivityTypeId = pdoOportunityDetailGetDTO.pdActivityType;
    const learningModeId = pdoOportunityDetailGetDTO.learningMode;
    const subjectAreaIds = pdoOportunityDetailGetDTO.subjectAreaIds;

    if (pdActivityTypeId) {
      tagIds.push(pdActivityTypeId);
    }
    if (learningModeId) {
      tagIds.push(learningModeId);
    }
    if (subjectAreaIds && subjectAreaIds.length > 0) {
      const firstSubjectAreaId = subjectAreaIds[0];
      tagIds.push(firstSubjectAreaId);
    }
    this.tags = tagIds.map((tag) => {
      return this.getTagName(metadataTags, tag);
    });
  }

  private setInfo(
    metadataTags: MetadataTagModel[],
    pdoOportunityDetailGetDTO: PDOpportunityDetailGetDTO
  ): void {
    if (isEmpty(metadataTags)) {
      return;
    }

    this.infos = [];
    // Id
    const pdActivityTypeId = pdoOportunityDetailGetDTO.pdActivityType;
    const categoryIds = pdoOportunityDetailGetDTO.categoryIds;
    const learningModeId = pdoOportunityDetailGetDTO.learningMode;
    // Value
    const courseCode = pdoOportunityDetailGetDTO.courseCode || 'N/A';
    const traisiCourseCode = pdoOportunityDetailGetDTO.externalCode || 'N/A';
    const courseOutlineStructure =
      pdoOportunityDetailGetDTO.courseOutlineStructure || 'N/A';
    const courseObjective = pdoOportunityDetailGetDTO.courseObjective || 'N/A';
    const pdActivityTypeValue = this.getTagName(metadataTags, pdActivityTypeId);
    const learningModeValue = this.getTagName(metadataTags, learningModeId);
    const categoryValues = this.getTagNames(metadataTags, categoryIds);

    this.infos.push(
      new PDODetailInfo('Type of PD Activity', pdActivityTypeValue)
    );
    this.infos.push(new PDODetailInfo('Mode of Learning', learningModeValue));
    this.infos.push(new PDODetailInfo('Category', categoryValues));
    this.infos.push(new PDODetailInfo('Course Code', courseCode));
    this.infos.push(new PDODetailInfo('Traisi Course Code', traisiCourseCode));
    this.infos.push(
      new PDODetailInfo('Course Outline & Structure', courseOutlineStructure)
    );
    this.infos.push(new PDODetailInfo('Objectives/Outcomes', courseObjective));
  }

  private getTagName(metadataTags: MetadataTagModel[], tagId: string): string {
    if (isEmpty(metadataTags) || isEmpty(tagId)) {
      return 'N/A';
    }
    const tag = metadataTags.find((item) => item.id === tagId);

    return tag && tag.name ? tag.name : 'N/A';
  }

  private getTagNames(
    metadataTags: MetadataTagModel[],
    tagIds: string[]
  ): string {
    if (isEmpty(metadataTags) || isEmpty(tagIds)) {
      return 'N/A';
    }

    const tags = metadataTags.filter((item) => tagIds.includes(item.id));
    if (isEmpty(tags)) {
      return 'N/A';
    }

    const tagNames = tags.map((tag) => tag.name).join(', ');

    return tagNames || 'N/A';
  }
}

export class PDODetailInfo {
  title: string;
  value: string;
  constructor(title: string, value: string) {
    this.title = title;
    this.value = value;
  }
}

export enum PDOpportunityStatusEnum {
  Published = 'Published',
}

export enum MetadataId {
  Microlearning = 'db13d0f8-d595-11e9-baec-0242ac120004',
}

export enum CourseRegistrationMethod {
  Public = 'Public',
  Private = 'Private',
}
