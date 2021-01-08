import { Component, ElementRef, Input, Output, ViewChild } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { EventEmitter } from 'events';
import { GlobalSpinnerService } from '@opal20/infrastructure';
import { IOpalSelectDefaultItem } from '../../models/select.model';
import { ImageCroppedEvent } from 'ngx-image-cropper';

@Component({
  selector: 'crop-image-dialog',
  templateUrl: './crop-image-dialog.component.html',
  styleUrls: ['./crop-image-dialog.component.scss']
})
export class CropImageDialogComponent {
  @Input() public file: File;
  @Input() public aspectRatio = 16 / 9;
  @Input() public width: number;
  @Input() public height: number;
  @Input() public maintainAspectRatio = false;
  public originRatio: number;
  public originWidth: number;
  public originHeight: number;
  public imageChangedEvent: ElementRef;
  public croppedFile: Blob;
  public thumbnail: SafeUrl;
  public allowedaspectRatios: IOpalSelectDefaultItem<Number>[];
  public showCropper = false;
  @Output()
  public saveImage = new EventEmitter();
  @ViewChild('img', { static: true })
  private img: ElementRef;
  constructor(protected dialogRef: DialogRef, private globalSpinner: GlobalSpinnerService, private domSanitizer: DomSanitizer) {
    this.globalSpinner.show();
  }

  public imageCropped(event: ImageCroppedEvent): void {
    this.croppedFile = event.file;
    const objectURL = URL.createObjectURL(this.croppedFile);
    this.thumbnail = this.domSanitizer.bypassSecurityTrustUrl(objectURL);
  }
  public imageLoaded(): void {
    this.showCropper = true;
  }
  public cropperReady(): void {
    this.globalSpinner.hide();
  }
  public loadImageFailed(): void {
    this.dialogRef.close();
  }
  public cancel(): void {
    this.dialogRef.close();
  }
  public save(): void {
    this.dialogRef.close(this.croppedFile);
  }

  public get fileFormat(): string {
    return this.file.type.replace('image/', '');
  }

  public initRatios(): void {
    const objectUrl = URL.createObjectURL(this.file);
    const image = new Image();
    image.src = objectUrl;
    image.onload = e => {
      this.originWidth = image.width;
      this.originHeight = image.height;
      if (!this.width || !this.height) {
        this.width = image.width;
        this.height = Math.round(this.width / this.aspectRatio);
      }
      this.originRatio = image.width / image.height;
      this.allowedaspectRatios = [
        {
          label: '16/9',
          value: 16 / 9
        },
        {
          label: '4/3',
          value: 4 / 3
        },
        {
          label: 'Original',
          value: this.originRatio
        }
      ];
    };
  }
  public updateRenderRatio(): void {
    this.width = this.originWidth;
    this.height = Math.round(this.width / this.aspectRatio);
    if (this.height > this.originHeight) {
      if (this.height > this.originHeight) {
        this.height = this.originHeight;
      }
      this.width = Math.round(this.height * this.aspectRatio);
    }
    this.aspectRatio = this.width / this.height;
    this.imageChangedEvent = this.img;
  }

  public updateRenderSizeByRatio(width: number, height: number): void {
    if (width) {
      if (width > this.originWidth) {
        this.width = this.originWidth;
      }
      this.height = Math.round(this.width / this.aspectRatio);
    }
    if (height || this.height > this.originHeight) {
      if (this.height > this.originHeight) {
        this.height = this.originHeight;
      }
      this.width = Math.round(this.height * this.aspectRatio);
    }
    this.aspectRatio = this.width / this.height;
    this.imageChangedEvent = this.img;
  }
}
