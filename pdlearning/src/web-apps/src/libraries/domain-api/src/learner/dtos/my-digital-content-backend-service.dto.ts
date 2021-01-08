import { DigitalContentType } from '../../content/models/digital-content-type.enum';
import { MyDigitalContentStatus } from '../models/my-digital-content-info.model';

export interface IMyDigitalContentRequest {
  id?: string;
  digitalContentId?: string;
  status?: MyDigitalContentStatus;
  digitalContentType?: DigitalContentType;
  progressMeasure?: number;
  readDate?: Date;
}

export interface IEnrollDigitalContentRequest {
  digitalContentId: string;
}
