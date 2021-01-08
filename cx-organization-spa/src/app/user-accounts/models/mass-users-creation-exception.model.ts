import { MassUsersCreationFileError } from './../../shared/constants/mass-users-creation-file-error-enum';

export class MassUsersCreationResultDto {
  salutation: string;
  name: string;
  officialEmail: string;
  gender: string;
  placeOfWork: string;
  accountActiveFrom: string;
  dateOfExpiryAccount: string;
  reasonForUserAccountRequest: string;
  systemRole: string;
  personalSpaceLimitation: string;
  number: number;

  constructor(data?: Partial<MassUsersCreationResultDto>) {
    if (!data) {
      return;
    }
    this.salutation = data.salutation;
    this.name = data.name;
    this.officialEmail = data.officialEmail;
    this.gender = data.gender;
    this.placeOfWork = data.placeOfWork;
    this.accountActiveFrom = data.accountActiveFrom;
    this.dateOfExpiryAccount = data.dateOfExpiryAccount;
    this.reasonForUserAccountRequest = data.reasonForUserAccountRequest;
    this.systemRole = data.systemRole;
    this.personalSpaceLimitation = data.personalSpaceLimitation;
    this.number = data.number;
  }
}

// tslint:disable-next-line:max-classes-per-file
export class InvalidMassUsersCreationDto extends MassUsersCreationResultDto {
  reason: string;
  constructor(data?: Partial<InvalidMassUsersCreationDto>) {
    super(data);
    this.reason = data.reason;
  }
}
// tslint:disable-next-line:max-classes-per-file
export class MassUsersCreationValidationResultDto {
  invalidMassUserCreationDto: InvalidMassUsersCreationDto[];
  totalRecords: number;
  isValidToMassUserCreation: boolean;
  numberOfValidRecords: number;
  numberOfInValidRecords: number;
  exception: MassUsersCreationException;

  constructor(data?: Partial<MassUsersCreationValidationResultDto>) {
    if (!data) {
      return;
    }
    this.invalidMassUserCreationDto = data.invalidMassUserCreationDto || [];
    this.totalRecords = data.totalRecords;
    this.isValidToMassUserCreation = data.isValidToMassUserCreation;
    this.numberOfInValidRecords = data.numberOfInValidRecords;
    this.numberOfValidRecords = data.numberOfValidRecords;
    this.exception = data.exception || null;
  }
}

// tslint:disable-next-line:max-classes-per-file
export class MassUsersCreationException {
  errorType: MassUsersCreationFileError;
  message?: string;

  constructor(data?: Partial<MassUsersCreationException>) {
    if (!data) {
      return;
    }

    this.errorType = data.errorType || null;
    this.message = data.message;
  }
}
