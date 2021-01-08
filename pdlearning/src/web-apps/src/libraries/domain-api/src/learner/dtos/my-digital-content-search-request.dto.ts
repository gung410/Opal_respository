import { DigitalContentType } from '../../content/models/digital-content-type.enum';
import { IPagedResultRequestDto } from '../../share/dtos/paged-request.dto';
import { MyDigitalContentStatus } from '../models/my-digital-content-info.model';

export interface IMyDigitalContentSearchRequest extends IPagedResultRequestDto {
  searchText?: string;
  statusFilter?: MyDigitalContentStatus;
  orderBy: string;
  contentType?: DigitalContentType[];
  skipCount?: number;
  maxResultCount?: number;
}
