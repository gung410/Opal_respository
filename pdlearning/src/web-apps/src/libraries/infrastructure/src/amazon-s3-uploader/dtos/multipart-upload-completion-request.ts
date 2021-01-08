import { IMultipartEtag } from '../models/multipart-etag';

export interface IMultipartUploadCompletionRequest {
  fileId: string;
  uploadId: string;
  fileExtension: string;
  folder: string;
  partETags: IMultipartEtag[];
  isTemporary?: boolean;
}
