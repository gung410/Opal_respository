import { AnnouncementStatus } from '../models/announcement.model';

export interface IChangeAnnouncementStatusRequest {
  status: AnnouncementStatus;
  ids: string[];
}
