import { CommunityStatisticType, LearningPathStatisticType, MyLearningStatisticType } from '../models/search-filter-statistic-type.dto';

import { IPagedResultRequestDto } from '../../share/dtos/paged-request.dto';

export interface IMyLearningSearchRequest extends IPagedResultRequestDto {
  type: string;
  includeStatistic: boolean;
  statisticsFilter: SearchStatisticType[];
  statusFilter: SearchStatisticType;
  searchText: string;
}

export type SearchStatisticType = MyLearningStatisticType | LearningPathStatisticType | CommunityStatisticType;
