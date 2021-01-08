import { HttpResponse } from '@angular/common/http';

export interface IDownloadTemplateFormat<TFileFormat> {
  fileFormatName: string;
  fileFormat: TFileFormat;
}
export interface IDownloadTemplateOption<TFileFormat> {
  templateFormats: IDownloadTemplateFormat<TFileFormat>[];
  downloadTemplateFn: (fileFormat: TFileFormat) => Promise<HttpResponse<Blob>>;
}
