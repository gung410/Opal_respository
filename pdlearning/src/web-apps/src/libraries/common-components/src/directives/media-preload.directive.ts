import { Directive, Input } from '@angular/core';

import { AmazonS3UploaderService } from '@opal20/infrastructure';
import { PlatformHelper } from '../utils/platform.utils';
/**
 * Chrome browsers (on IPad/IPhone) and safari browsers cannot attach cookie to load media from cloudfront media
 * Try to reload cloudfront media with parameterized URL on IPad devices and safari browsers.
 */
@Directive({
  selector: 'img',
  host: {
    '(error)': 'updateUrl($event)',
    '(load)': 'load($event)',
    '[src]': 'src'
  }
})
export class MediaPreloadDirective {
  @Input() public src: string;
  constructor(private uploaderService: AmazonS3UploaderService) {}

  public updateUrl($event: Event): void {
    this.retryAWSS3Resource($event);
  }

  public load($event: Event): void {
    if (PlatformHelper.isIOSDevice()) {
      this.retryAWSS3Resource($event);
    }
  }

  private retryAWSS3Resource($event: Event): void {
    const isCloufrontStorage =
      this.src &&
      this.src.indexOf != null &&
      this.src.indexOf(AppGlobal.environment.cloudfrontUrl) > -1 &&
      this.src.indexOf(`${AppGlobal.environment.cloudfrontUrl}/avatar`) === -1;
    if (isCloufrontStorage) {
      const originSrcUrl = this.src.replace(`${AppGlobal.environment.cloudfrontUrl}/`, '');
      this.uploaderService.getFile(originSrcUrl).then(resp => {
        this.src = resp;
      });
    }
  }
}
