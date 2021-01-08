import { AnnouncementType } from '../models/announcement-type.model';

export interface IPreviewAnnouncementRequest {
  announcementType: AnnouncementType;
  classRunId: string;
  base64Message: string;
  userNameTag?: string;
  courseTitleTag?: string;
  courseCodeTag?: string;
  courseAdminNameTag?: string;
  courseAdminEmailTag?: string;
  listSessionTag?: string;
  detailUrlTag?: string;
}
