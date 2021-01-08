import { SearchRegistrationsType } from '../../course/models/search-registrations-type.model';

export interface IMyECertificateRequest {
  searchType: SearchRegistrationsType;
  searchText: string;
  applySearchTextForCourse: boolean;
  myRegistrationOnly: boolean;
  skipCount: number | null;
  maxResultCount: number | null;
}
