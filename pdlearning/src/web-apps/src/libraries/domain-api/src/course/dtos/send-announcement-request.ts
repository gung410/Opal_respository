export interface ISendAnnouncementRequest {
  data: ISaveAnnouncementDto;
}

export interface ISaveAnnouncementDto {
  id?: string;
  title: string;
  base64Message: string;
  scheduleDate?: Date;
  registrationIds: string[];
  courseId: string;
  classrunId: string;
  saveTemplate: boolean;
  isSentToAllParticipants: boolean;
}
