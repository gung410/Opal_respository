import { UploadParameters } from './upload-parameters';

export enum UploadProgressStatus {
  Start = 'Start',
  Completed = 'Completed',
  Failure = 'Failure'
}

export interface IUploaderProgress {
  status: UploadProgressStatus;
  parameters?: UploadParameters;
}
