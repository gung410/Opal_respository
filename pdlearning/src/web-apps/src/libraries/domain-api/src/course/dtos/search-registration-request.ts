import { IFilter } from '@opal20/infrastructure';
import { SearchRegistrationsType } from '../models/search-registrations-type.model';

export interface ISearchRegistrationRequest {
  courseId: string;
  classRunId: string;
  excludeAssignedAssignmentId?: string;
  searchType: SearchRegistrationsType;
  searchText: string;
  userFilter: IFilter;
  filter: IFilter;
  skipCount: number | null;
  maxResultCount: number | null;
}
