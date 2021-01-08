import { IFilter } from '@opal20/infrastructure';
export interface ISearchParticipantAssignmentTrackRequest {
  courseId: string;
  classRunId: string;
  assignmentId: string;
  searchText: string;
  filter: IFilter;
  registrationIds: string[];
  skipCount: number | null;
  maxResultCount: number | null;
  includeQuizAssignmentFormAnswer: boolean;
}
