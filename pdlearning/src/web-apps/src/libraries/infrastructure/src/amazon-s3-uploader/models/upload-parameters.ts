import { IMultipartEtag } from './multipart-etag';

export class UploadParameters {
  constructor(
    public file?: File,
    public fileName?: string,
    public fileSize?: number,
    public fileId?: string,
    public fileExtension?: string,
    public fileLocation?: string,
    public folder?: string,
    public mineType?: string,
    public uploadId?: string,
    public uploadParts?: IMultipartEtag[],
    public isProcessing: boolean = false,
    public isPersonalFile: boolean = false,
    public isTemporary: boolean = false,
    public onUpdateProgress?: (percentage: number) => void,
    public onChangeStatus?: (status: string, errorMessage?: string) => void
  ) {}
}
