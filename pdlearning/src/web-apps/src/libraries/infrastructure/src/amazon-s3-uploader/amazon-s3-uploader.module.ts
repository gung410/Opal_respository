import { AmazonS3ApiService } from './amazon-s3-api.service';
import { AmazonS3UploaderService } from './amazon-s3-uploader.service';
import { NgModule } from '@angular/core';

@NgModule({
  providers: [AmazonS3UploaderService, AmazonS3ApiService]
})
export class AmazonS3UploaderModule {}
