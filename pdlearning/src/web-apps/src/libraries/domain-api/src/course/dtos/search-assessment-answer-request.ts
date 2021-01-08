import { IFilter } from '@opal20/infrastructure';

export interface ISearchAssessmentAnswerRequest {
  participantAssignmentTrackId?: string;
  userId?: string;
  searchText: string;
  filter: IFilter;
  skipCount: number;
  maxResultCount: number;
}
