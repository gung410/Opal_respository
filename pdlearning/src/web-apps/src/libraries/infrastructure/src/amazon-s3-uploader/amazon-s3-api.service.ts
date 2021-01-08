import { HttpProgressEvent, HttpRequest, HttpResponse } from '@angular/common/http';
import { InterceptorRegistry, InterceptorType } from '../backend-service/interceptor-registry';

import { BaseBackendService } from '../backend-service/base-backend.service';
import { CommonFacadeService } from '../services/common-facade.service';
import { IBaseFileUploadRequest } from './dtos/base-file-upload-request';
import { IDownloadHtmlContentRequest } from './dtos/download-html-content-request';
import { IGetFileResult } from './dtos/get-file-result';
import { IHttpOptions } from '../backend-service/models/http-options';
import { IMultipartFileCompletionResult } from './dtos/multipart-file-completion-result';
import { IMultipartPreSignedUrlRequest } from './dtos/multipart-pre-signed-url-request';
import { IMultipartPreSignedUrlResult } from './dtos/multipart-pre-signed-url-result';
import { IMultipartUploadAbortionRequest } from './dtos/multipart-upload-abortion-request';
import { IMultipartUploadCompletionRequest } from './dtos/multipart-upload-completion-request';
import { IMultipartUploadSessionRequest } from './dtos/multipart-upload-session-request';
import { IMultipartUploadSessionResult } from './dtos/multipart-upload-session-result';
import { IScormProcessStatusResult } from './models/scorm-process-status';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()
export class AmazonS3ApiService extends BaseBackendService {
  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  protected get apiUrl(): string {
    return AppGlobal.environment.uploaderApiUrl;
  }

  public getFile(key: string): Promise<string> {
    const httpOptions: IHttpOptions = this.getHttpOptions(true, { key });

    return this.commonFacadeService.http
      .get<string>(`${this.apiUrl}/uploader/getFile`, Object.assign({}, httpOptions, { responseType: 'text' }))
      .toPromise();
  }

  public downloadHtmlContent(request: IDownloadHtmlContentRequest, isIOSDevice: boolean = false): Observable<HttpResponse<Blob>> {
    return this.postDownloadFile<IDownloadHtmlContentRequest>(`/uploader/downloadLearningContent`, request, true, isIOSDevice);
  }

  public getFiles(keys: string[]): Promise<IGetFileResult[]> {
    return this.post<string[], IGetFileResult[]>('/uploader/getFiles', keys).toPromise();
  }

  public createMultipartUploadSession(
    request: IMultipartUploadSessionRequest,
    showSpinner: boolean = true
  ): Promise<IMultipartUploadSessionResult> {
    return this.post<IMultipartUploadSessionRequest, IMultipartUploadSessionResult>(
      '/uploader/createMultipartUploadSession',
      request,
      showSpinner
    ).toPromise();
  }

  public createPersonalFileMultipartUploadSession(
    request: IMultipartUploadSessionRequest,
    showSpinner: boolean = true
  ): Promise<IMultipartUploadSessionResult> {
    return this.post<IMultipartUploadSessionRequest, IMultipartUploadSessionResult>(
      '/uploader/createPersonalFileMultipartUploadSession',
      request,
      showSpinner
    ).toPromise();
  }

  public createMultipartPreSignedUrl(
    request: IMultipartPreSignedUrlRequest,
    showSpinner: boolean = true
  ): Promise<IMultipartPreSignedUrlResult> {
    return this.post<IMultipartPreSignedUrlRequest, IMultipartPreSignedUrlResult>(
      '/uploader/createMultipartPreSignedUrl',
      request,
      showSpinner
    ).toPromise();
  }

  public completeMultipartUploadSession(request: IMultipartUploadCompletionRequest, showSpinner: boolean = true): Promise<void> {
    return this.post<IMultipartUploadCompletionRequest, void>('/uploader/completeMultipartUploadSession', request, showSpinner).toPromise();
  }

  public completeMultipartFile(
    request: IMultipartUploadCompletionRequest,
    showSpinner: boolean = true
  ): Promise<IMultipartFileCompletionResult> {
    return this.post<IMultipartUploadCompletionRequest, IMultipartFileCompletionResult>(
      '/uploader/completeMultipartFile',
      request,
      showSpinner
    ).toPromise();
  }

  public extractScormPackage(request: IBaseFileUploadRequest, showSpinner: boolean = true): Promise<void> {
    return this.post<IBaseFileUploadRequest, void>('/uploader/extractScormPackage', request, showSpinner).toPromise();
  }

  public getScormProcessingStatus(request: IBaseFileUploadRequest, showSpinner: boolean = false): Promise<IScormProcessStatusResult> {
    return this.post<IBaseFileUploadRequest, IScormProcessStatusResult>(
      '/uploader/getScormProcessingStatus',
      request,
      showSpinner
    ).toPromise();
  }

  public abortMultipartUploadSession(request: IMultipartUploadAbortionRequest): Promise<void> {
    return this.post<IMultipartUploadAbortionRequest, void>('/uploader/abortMultipartUploadSession', request).toPromise();
  }

  public uploadPart(url: string, blob: Blob): Observable<HttpProgressEvent> {
    const req = new HttpRequest('PUT', url, blob, {
      reportProgress: true
    });

    return this.commonFacadeService.http.request(req) as Observable<HttpProgressEvent>;
  }

  protected onFilterInterceptors(registry: InterceptorRegistry): InterceptorRegistry {
    return registry.replace(InterceptorType.HttpResponse, {
      key: 'FILE_UPLOADER_HTTP_RESPONSE_INTERCEPTOR',
      type: InterceptorType.HttpResponse
    });
  }
}
