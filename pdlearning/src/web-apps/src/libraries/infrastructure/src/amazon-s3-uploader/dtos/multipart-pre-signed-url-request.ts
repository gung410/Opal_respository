export interface IMultipartPreSignedUrlRequest {
  fileId: string;
  uploadId: string;
  fileExtension: string;
  folder: string;
  partNumber: number;
}
