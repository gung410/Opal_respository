import { IPagedResultRequestDto } from '../../share/dtos/paged-request.dto';

export interface ISearchUsersForLearningPathRequestDto extends IPagedResultRequestDto {
  searchText: string;
}
