import { MassNominationErrorTypeEnum } from 'app-enums/mass-nomination-enum';

export interface PDCatalogSearchResult {
  id: string;
  name: string;
  description: string;
  thumbnailUrl: string;
  publisher: string;
  resourcetype: string;
  publishdate: Date;
}

export interface PDCatalogSearchDTO {
  total: number;
  resources: PDCatalogSearchResult[];
}

export interface PDCatalogSearchPayload {
  page: number;
  limit: number;
  searchText?: string;
  searchFields?: string[];
  searchCriteria?: SearchCriteria;
  resourceTypesFilter?: string[];
  statisticResourceTypes?: string[];
  useFuzzy?: boolean;
  useSynonym?: boolean;
}

export interface SearchCriteria {
  resourceType?: string[];
  status?: string[];
  'tags.id'?: string[];
  expiredDate?: string[];
  startDate?: string[];
  IsArchived?: string[];
}

export class MassNominationResultDto {
  number: string;
  courseCode: string;
  classRunCode: string;
  email: string;
  reason: string;

  constructor(data?: Partial<MassNominationResultDto>) {
    if (!data) {
      return;
    }
    this.number = data.number;
    this.courseCode = data.courseCode;
    this.classRunCode = data.classRunCode;
    this.email = data.email;
    this.reason = data.reason;
  }
}

export class MassNominationValidationResultDto {
  invalidNominations: MassNominationResultDto[];
  totalRecords: number;
  isValidToMassNominate: boolean;
  numberOfValidRecords: number;
  numberOfInValidRecords: number;
  exception: MassNominationExceptionDto;

  constructor(data?: Partial<MassNominationValidationResultDto>) {
    if (!data) {
      return;
    }
    this.invalidNominations = data.invalidNominations || [];
    this.totalRecords = data.totalRecords;
    this.isValidToMassNominate = data.isValidToMassNominate;
    this.numberOfInValidRecords = data.numberOfInValidRecords;
    this.numberOfValidRecords = data.numberOfValidRecords;
    this.exception = data.exception || null;
  }
}

export class MassNominationExceptionDto {
  errorType: MassNominationErrorTypeEnum;
  message?: string;

  constructor(data?: Partial<MassNominationExceptionDto>) {
    if (!data) {
      return;
    }
    this.errorType = data.errorType || null;
    this.message = data.message;
  }
}
