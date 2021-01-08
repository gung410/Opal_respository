import { IFilter } from '@opal20/infrastructure';

export interface ISearchBlockoutDateRequest {
  searchText: string;
  filter: IFilter;
  skipCount: number | null;
  maxResultCount: number | null;
  coursePlanningCycleId: string;
}
