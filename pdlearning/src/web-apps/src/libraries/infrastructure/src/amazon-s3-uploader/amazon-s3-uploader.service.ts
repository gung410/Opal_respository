import { HttpErrorResponse, HttpHeaders, HttpResponse } from '@angular/common/http';
import { IScormProcessStatusResult, ScormProcessStatus } from './models/scorm-process-status';
import { Observable, Subscription } from 'rxjs';

import { AmazonS3ApiService } from './amazon-s3-api.service';
import { BaseBackendService } from '../backend-service/base-backend.service';
import { CommonFacadeService } from '../services/common-facade.service';
import { FileUploaderSetting } from './models/file-uploader-settings';
import { Guid } from '../utils/guid';
import { IDownloadHtmlContentRequest } from './dtos/download-html-content-request';
import { IMultipartEtag } from './models/multipart-etag';
import { IMultipartFileInfo } from './models/multipart-file-info';
import { IMultipartPreSignedUrlResult } from './dtos/multipart-pre-signed-url-result';
import { IMultipartUploadAbortionRequest } from './dtos/multipart-upload-abortion-request';
import { IMultipartUploadCompletionRequest } from './dtos/multipart-upload-completion-request';
import { Injectable } from '@angular/core';
import { UploadParameters } from './models/upload-parameters';

// tslint:disable:no-bitwise

const fileTypeFn: Function = require('../../vendors/file-type');

@Injectable()
export class AmazonS3UploaderService extends BaseBackendService {
  protected documentExtensions: string[] = ['pdf', 'docx', 'xlsx', 'pptx', 'doc'];
  protected graphicExtensions: string[] = ['jpeg', 'jpg', 'gif', 'png', 'svg'];
  protected audioExtensions: string[] = ['mp3', 'ogg'];
  protected videoExtensions: string[] = ['mp4', 'm4v', 'ogv'];
  protected learningPackageExtensions: string[] = ['zip'];
  protected allowedExtensions: string[] = [
    ...this.documentExtensions,
    ...this.graphicExtensions,
    ...this.audioExtensions,
    ...this.videoExtensions,
    ...this.learningPackageExtensions
  ];
  private _requestorRegistry: { [fileId: string]: Subscription[] } = {};
  private readonly _maxPartSizeInMb: number = 10;

  constructor(protected commonFacadeService: CommonFacadeService, private amazonS3ApiService: AmazonS3ApiService) {
    super(commonFacadeService);
  }

  public getFile(key: string): Promise<string> {
    return this.amazonS3ApiService.getFile(key);
  }

  public downloadHtmlContent(htmlContent: string, isIOSDevice: boolean = false): Observable<HttpResponse<Blob>> {
    const request: IDownloadHtmlContentRequest = {
      htmlContent
    };
    return this.amazonS3ApiService.downloadHtmlContent(request, isIOSDevice);
  }

  public async uploadFile(parameters: UploadParameters, showSpinner: boolean = true): Promise<void> {
    await Promise.resolve(parameters)
      .then(() => this.createUploadSession(parameters, showSpinner))
      .then(() => this.startUploadParts(parameters, showSpinner))
      .then(() => this.completeUploadSession(parameters, showSpinner))
      .then(() => this.completeFileUpload(parameters, showSpinner))
      .catch((error: string | Error) => {
        delete this._requestorRegistry[parameters.fileId];
        // Http request error is already handled by interceptor.
        let message: string;
        if (typeof error === 'string') {
          message = error;
        }
        if (error instanceof Error) {
          message = error.message;
        }
        if (error instanceof HttpErrorResponse && error.status === 0) {
          error = new Error('The connection has timed out.');
        }
        if (message) {
          this.commonFacadeService.globalModalService.showErrorMessage(message);
        }
        // Throw the error to be displayed on ErrorMessage session.
        throw error;
      });
  }

  public async uploadPersonalFile(parameters: UploadParameters, showSpinner: boolean = true): Promise<void> {
    await Promise.resolve(parameters)
      .then(() => this.createPersonalFileUploadSession(parameters, showSpinner))
      .then(() => this.startUploadParts(parameters, showSpinner))
      .then(() => this.completeUploadSession(parameters, showSpinner))
      .then(() => this.completeFileUpload(parameters, showSpinner))
      .catch((error: string | Error) => {
        delete this._requestorRegistry[parameters.fileId];
        // Http request error is already handled by interceptor.
        let message: string;
        if (typeof error === 'string') {
          message = error;
        }
        if (error instanceof Error) {
          message = error.message;
        }
        if (error instanceof HttpErrorResponse && error.status === 0) {
          error = new Error('The connection has timed out.');
        }
        if (message) {
          this.commonFacadeService.globalModalService.showErrorMessage(message);
        }
        // Throw the error to be displayed on ErrorMessage session.
        throw error;
      });
  }

  public async processScormFilePackage(parameters: UploadParameters, showSpinner: boolean = true): Promise<void> {
    if (parameters.fileExtension !== 'scorm') {
      return Promise.resolve();
    }

    const errorHandler = error => {
      let errorMessage: string;
      if (error instanceof HttpErrorResponse && error.status === 0) {
        errorMessage = 'The connection has timed out.';
      } else {
        errorMessage = 'The SCORM package cannot be processed, please try again.';
      }
      return errorMessage;
    };

    this.extractScormPackage(parameters, showSpinner).catch(e => {
      return Promise.reject(errorHandler(e));
    });
    return new Promise((resolve, reject) => {
      const checker = setInterval(async () => {
        let resp;
        try {
          resp = await this.getScormProcessStatus(parameters, showSpinner);
          parameters.onChangeStatus(resp.status);
        } catch (e) {
          parameters.onChangeStatus(ScormProcessStatus.Failure, errorHandler(e));
        }
        switch (resp.status) {
          case ScormProcessStatus.ExtractingFailure:
            clearInterval(checker);
            reject(
              'An error occurred while processing the SCORM package. Please check the content and ensure that the file is not password-protected. Please try again.'
            );
            break;
          case ScormProcessStatus.Failure:
          case ScormProcessStatus.Timeout:
            clearInterval(checker);
            reject('The SCORM package cannot be processed, please try again.');
            break;
          case ScormProcessStatus.Invalid:
            clearInterval(checker);
            reject('The SCORM file is invalid. Please check the file.');
            break;
          case ScormProcessStatus.Completed:
            clearInterval(checker);
            return resolve();
        }
      }, 4000);
    });
  }

  public abortUpload(request: IMultipartUploadAbortionRequest): Promise<void> {
    if (this._requestorRegistry[request.fileId]) {
      this._requestorRegistry[request.fileId].forEach(requestor => requestor.unsubscribe());
      return this.amazonS3ApiService.abortMultipartUploadSession(request);
    }
    return Promise.resolve();
  }

  public validateMineType(file: File, uploadSettings: FileUploaderSetting): Promise<IMultipartFileInfo> {
    return new Promise(
      (resolve: (value?: IMultipartFileInfo | PromiseLike<IMultipartFileInfo>) => void, reject: (reason?: string) => void) => {
        const reader: FileReader = new FileReader();

        // tslint:disable-next-line:no-any
        reader.onload = (ev: any) => {
          const buffer: Uint8Array = new Uint8Array(<ArrayBuffer>ev.target.result);
          let result: { ext: string; mime: string } = fileTypeFn(buffer);

          const isSvg = String.fromCharCode.apply(null, buffer.subarray(0, 4)) === '<svg';

          // Bypass for these file extensions
          if (file.name && file.name.endsWith('.svg') && ((result && result.ext === 'xml' && result.mime === 'application/xml') || isSvg)) {
            result = {
              ext: 'svg',
              mime: 'image/svg+xml'
            };
          }

          if (!result) {
            return reject(
              `The selected file is not supported. Supported formats: ${(uploadSettings.extensions || this.allowedExtensions).join(', ')}.`
            );
          }

          if (result.ext === 'ogx') {
            result.ext = 'ogv';
          }

          // General validation for files
          const isFileExtensionValid: boolean = (uploadSettings.extensions || this.allowedExtensions).some(ext => ext === result.ext);

          if (!isFileExtensionValid) {
            if (uploadSettings.isShowExtensionsOnError) {
              return reject(
                `The ${result.ext} extension is not supported. Supported formats: ${(
                  uploadSettings.extensions || this.allowedExtensions
                ).join(', ')}.`
              );
            } else {
              return reject(`The selected file is not supported.`);
            }
          }

          if (uploadSettings.maxFileSize && this.exceedLimitSize(file, uploadSettings.maxFileSize)) {
            return reject(`The file uploaded exceeds the allowable limit of ${uploadSettings.maxFileSize}MB.`);
          }

          // Validation for graphic files
          if (uploadSettings.isFileLimitSizeValidate) {
            if (this.exceedLimitSizeWithAllowedExtension(this.graphicExtensions, result.ext, file, 10)) {
              return reject('The file uploaded exceeds the allowable limit of 10MB.');
            }

            // Document file validation
            if (this.exceedLimitSizeWithAllowedExtension(this.documentExtensions, result.ext, file, 5)) {
              return reject('The file uploaded exceeds the allowable limit of 5MB.');
            }

            // Video file & Learning package validation
            if (
              this.exceedLimitSizeWithAllowedExtension([...this.videoExtensions, ...this.learningPackageExtensions], result.ext, file, 500)
            ) {
              return reject('The file uploaded exceeds the allowable limit of 500MB.');
            }

            // Audio file validation
            if (this.exceedLimitSizeWithAllowedExtension(this.audioExtensions, result.ext, file, 5)) {
              return reject('The file uploaded exceeds the allowable limit of 5MB.');
            }
          }

          return resolve(<IMultipartFileInfo>{
            mimeType: result.mime,
            extension: result.ext
          });
        };
        reader.readAsArrayBuffer(file);
      }
    );
  }

  private createUploadSession(parameters: UploadParameters, showSpinner: boolean = true): Promise<void> {
    this._requestorRegistry[parameters.fileId] = [];
    parameters.fileId = Guid.create().toString();

    return this.amazonS3ApiService
      .createMultipartUploadSession(
        {
          fileId: parameters.fileId,
          fileExtension: parameters.fileExtension,
          folder: parameters.folder
        },
        showSpinner
      )
      .then(response => {
        parameters.uploadId = response.uploadId;
      });
  }

  private createPersonalFileUploadSession(parameters: UploadParameters, showSpinner: boolean = true): Promise<void> {
    this._requestorRegistry[parameters.fileId] = [];
    parameters.fileId = Guid.create().toString();

    return this.amazonS3ApiService
      .createPersonalFileMultipartUploadSession(
        {
          fileId: parameters.fileId,
          fileExtension: parameters.fileExtension,
          folder: parameters.folder,
          fileSize: parameters.fileSize
        },
        showSpinner
      )
      .then(response => {
        parameters.uploadId = response.uploadId;
      });
  }

  private async startUploadParts(parameters: UploadParameters, showSpinner: boolean = true): Promise<void> {
    const maxSizeOfPart: number = this.getMaxSize(this._maxPartSizeInMb);
    const file: File = parameters.file;
    const totalPart: number = Math.ceil(file.size / maxSizeOfPart);
    let filePositionStart: number = 0;
    let filePositionEnd: number = maxSizeOfPart;
    let totalUploadedSizeOfFile: number = 0;

    parameters.uploadParts = [];

    const promiseOfAllUploadParts: Promise<void>[] = [];

    for (let i: number = 1; i <= totalPart; i++) {
      const currentPartSize: number = i * maxSizeOfPart;

      filePositionEnd = currentPartSize > file.size ? file.size : currentPartSize;

      const blob: Blob = parameters.file.slice(filePositionStart, filePositionEnd);

      promiseOfAllUploadParts.push(
        Promise.resolve().then(async () => {
          const preSignedUrlResult: IMultipartPreSignedUrlResult = await this.amazonS3ApiService.createMultipartPreSignedUrl(
            {
              uploadId: parameters.uploadId,
              fileId: parameters.fileId,
              fileExtension: parameters.fileExtension,
              folder: parameters.folder,
              partNumber: i
            },
            showSpinner
          );

          const eTag: string = await new Promise(
            (resolve: (value?: string | PromiseLike<string>) => void, reject: (reason?: string | Error) => void) => {
              let totalUploadedSizeOfPart: number = 0;
              const requestor = this.amazonS3ApiService.uploadPart(preSignedUrlResult.url, blob).subscribe(
                uploadPartResult => {
                  if (uploadPartResult.type === 1) {
                    const uploadedSizeOfCurrentPart: number = uploadPartResult.loaded - totalUploadedSizeOfPart;
                    totalUploadedSizeOfFile += uploadedSizeOfCurrentPart;
                    const percentage: number = Math.round((totalUploadedSizeOfFile / parameters.file.size) * 100);

                    if (parameters.onUpdateProgress) {
                      parameters.onUpdateProgress(percentage);
                    }
                    totalUploadedSizeOfPart += uploadedSizeOfCurrentPart;
                  }

                  if (uploadPartResult.type === 4 && uploadPartResult instanceof HttpResponse) {
                    resolve((uploadPartResult as { headers: HttpHeaders }).headers.get('ETag'));
                  }
                },
                error => reject(error)
              );

              if (!this._requestorRegistry[parameters.fileId]) {
                this._requestorRegistry[parameters.fileId] = [requestor];
              } else {
                this._requestorRegistry[parameters.fileId].push(requestor);
              }
            }
          );

          parameters.uploadParts.push({
            eTag: eTag,
            partNumber: i
          });
        })
      );

      filePositionStart = currentPartSize;
    }

    return Promise.all(promiseOfAllUploadParts).then();
  }

  private completeUploadSession(parameters: UploadParameters, showSpinner: boolean = true): Promise<void> {
    const request: IMultipartUploadCompletionRequest = {
      fileId: parameters.fileId,
      partETags: parameters.uploadParts.sort((a: IMultipartEtag, b: IMultipartEtag) => a.partNumber - b.partNumber),
      uploadId: parameters.uploadId,
      folder: parameters.folder,
      fileExtension: parameters.fileExtension
    };

    return this.amazonS3ApiService.completeMultipartUploadSession(request, showSpinner).then(() => {
      delete this._requestorRegistry[parameters.fileId];
    });
  }

  /**
   * TODO: move this logic to Content microservice after successfully inserting uploaded information.
   * @param parameters
   */
  private completeFileUpload(parameters: UploadParameters, showSpinner: boolean = true): Promise<void> {
    const request: IMultipartUploadCompletionRequest = {
      fileId: parameters.fileId,
      partETags: parameters.uploadParts.sort((a: IMultipartEtag, b: IMultipartEtag) => a.partNumber - b.partNumber),
      uploadId: parameters.uploadId,
      folder: parameters.folder,
      fileExtension: parameters.fileExtension,
      isTemporary: parameters.isTemporary
    };

    return this.amazonS3ApiService.completeMultipartFile(request, showSpinner).then(result => {
      parameters.fileLocation = result.location;
    });
  }

  private extractScormPackage(parameters: UploadParameters, showSpinner: boolean = true): Promise<void> {
    if (parameters.fileExtension !== 'scorm') {
      return Promise.resolve();
    }

    return this.amazonS3ApiService.extractScormPackage(
      {
        fileId: parameters.fileId,
        folder: parameters.folder,
        fileExtension: parameters.fileExtension
      },
      showSpinner
    );
  }

  private getScormProcessStatus(parameters: UploadParameters, showSpinner: boolean = true): Promise<IScormProcessStatusResult> {
    if (parameters.fileExtension !== 'scorm') {
      return Promise.resolve({ status: ScormProcessStatus.Completed });
    }

    return this.amazonS3ApiService.getScormProcessingStatus(
      {
        fileId: parameters.fileId,
        folder: parameters.folder,
        fileExtension: parameters.fileExtension
      },
      showSpinner
    );
  }
  /**
   * If your app splits a file into multiple byte ranges,
   * the size of each byte range MUST be a multiple of 320 KiB (327,680 bytes).
   * Using a fragment size that does not divide evenly by 320 KiB will result in errors committing some files.
   * @param sizeInMB : size in MB (eg: 10, 15, 20)
   */
  private getMaxSize(sizeInMB: number): number {
    const sizeInKB: number = sizeInMB * 1024 * 1024;
    const numberOfBlocks: number = (sizeInKB / 320) | 0;
    const totalBytes: number = numberOfBlocks * 320;

    return totalBytes;
  }

  private exceedLimitSizeWithAllowedExtension(allowedExtension: string[], extension: string, file: File, sizeInMb: number): boolean {
    return allowedExtension.some(ext => ext === extension) && this.exceedLimitSize(file, sizeInMb);
  }

  private exceedLimitSize(file: File, sizeInMb: number): boolean {
    return file.size > sizeInMb * 1024 * 1024;
  }
}
