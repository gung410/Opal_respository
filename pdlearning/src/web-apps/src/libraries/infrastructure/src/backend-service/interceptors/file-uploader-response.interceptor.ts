import { HttpResponseInterceptor } from './http-response.interceptor';
import { Injectable } from '@angular/core';

@Injectable()
export class FileUploaderResponseInterceptor extends HttpResponseInterceptor {
  protected key: string = 'FILE_UPLOADER_HTTP_RESPONSE_INTERCEPTOR';

  protected handleConnectionError(errorMessage: string): void {
    return;
  }
}
