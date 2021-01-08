export interface IFileUploaderSetting {
  extensions?: string[];
  maxFileSize?: number;
  isFileLimitSizeValidate?: boolean;
  isCropImage?: boolean;
  isShowExtensionsOnError?: boolean;
}

export class FileUploaderSetting implements IFileUploaderSetting {
  public extensions?: string[];
  public maxFileSize?: number;
  public isFileLimitSizeValidate?: boolean = true;
  public isCropImage?: boolean = true;
  public isShowExtensionsOnError?: boolean = true;
  constructor(params?: IFileUploaderSetting) {
    if (params != null) {
      const { extensions, maxFileSize, isFileLimitSizeValidate = true, isCropImage = true, isShowExtensionsOnError = true } = params;
      this.extensions = extensions;
      this.maxFileSize = maxFileSize;
      this.isFileLimitSizeValidate = isFileLimitSizeValidate;
      this.isCropImage = isCropImage;
      this.isShowExtensionsOnError = isShowExtensionsOnError;
    }
  }
}
