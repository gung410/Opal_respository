import { PersonalFileModel } from '../models/personal-file.model';

export interface ICreatePersonalFilesRequest {
  personalFiles: PersonalFileModel[] | undefined;
}
