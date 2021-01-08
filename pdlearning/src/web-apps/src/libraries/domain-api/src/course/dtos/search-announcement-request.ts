import { IFilter } from '@opal20/infrastructure';

export interface ISearchAnnoucementRequest {
  courseId: string;
  classRunId: string;
  skipCount: number;
  maxResultCount: number;
  filter: IFilter;
}
