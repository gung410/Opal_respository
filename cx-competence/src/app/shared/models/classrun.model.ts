export interface ClassRunDTO {
  id: string;
  courseId: string;
  classTitle: string;
  classRunCode: string;
  planningStartTime?: Date;
  planningEndTime?: Date;
  startDateTime: Date;
  endDateTime: Date;
  startTime: Date;
  endTime: Date;
  facilitatorIds: string[];
  coFacilitatorIds: string[];
  minClassSize: number;
  maxClassSize: number;
  applicationStartDate: Date;
  applicationEndDate: Date;
  registrationStartDate: Date;
  registrationEndDate: Date;
  status: string;
}

export class ClassRunModel {
  id: string;
  code: string;
  classTitle: string;
  planningStartTime?: Date;
  planningEndTime?: Date;
  startDate?: Date;
  endDate?: Date;
  startDateTime: Date;
  endDateTime: Date;
  facilitatorIds: string[];
  maxClassSize: number;
  applicationStartDate: Date;
  applicationEndDate: Date;
  constructor(classRunDTO?: ClassRunDTO) {
    if (!classRunDTO) {
      return;
    }
    this.id = classRunDTO.id;
    this.code = classRunDTO.classRunCode;
    this.classTitle = classRunDTO.classTitle;
    this.planningStartTime = classRunDTO.planningStartTime
      ? new Date(classRunDTO.planningStartTime)
      : null;
    this.planningEndTime = classRunDTO.planningEndTime
      ? new Date(classRunDTO.planningEndTime)
      : null;
    this.startDate = classRunDTO.startDateTime
      ? this.removeTime(classRunDTO.startDateTime)
      : null;
    this.endDate = classRunDTO.endDateTime
      ? new Date(classRunDTO.endDateTime)
      : null;
    this.startDateTime = classRunDTO.startDateTime;
    this.endDateTime = classRunDTO.endDateTime;
    this.facilitatorIds = classRunDTO.facilitatorIds;
    this.maxClassSize = classRunDTO.maxClassSize;
    this.applicationStartDate = classRunDTO.applicationStartDate;
    this.applicationEndDate = classRunDTO.applicationEndDate;
  }

  private removeTime(date: Date | null): Date {
    if (date == null) {
      date = new Date();
    }

    return new Date(new Date(date).toDateString());
  }
}
