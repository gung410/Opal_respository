export interface ICourseCriteriaServiceScheme {
  serviceSchemeId: string;
  maxParticipant?: number;
}

export class CourseCriteriaServiceScheme implements ICourseCriteriaServiceScheme {
  public serviceSchemeId: string;
  public maxParticipant?: number;
  constructor(data?: ICourseCriteriaServiceScheme) {
    if (data == null) {
      return;
    }
    this.serviceSchemeId = data.serviceSchemeId;
    this.maxParticipant = data.maxParticipant;
  }
}
