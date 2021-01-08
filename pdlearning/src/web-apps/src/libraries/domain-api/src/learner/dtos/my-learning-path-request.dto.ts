import { IPagedResultRequestDto } from '../../share/dtos/paged-request.dto';

export interface IGetMyLearningPathRequest extends IPagedResultRequestDto {
  searchText: string;
}
