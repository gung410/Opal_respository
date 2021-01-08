import { IFilter } from '@opal20/infrastructure';
export interface ISearchAttendaceTrackingRequest {
  sessionId: string;
  searchText: string;
  filter: IFilter;
  skipCount: number;
  maxResultCount: number;
}
