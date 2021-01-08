import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FileType, PersonalFileModel } from '@opal20/domain-api';
import { PlatformHelper } from '@opal20/common-components';
import { FileUploaderHelpers } from '../../helpers/file-uploader.helper';

@Component({
  selector: 'form-answer-attachments',
  templateUrl: './form-answer-attachments.component.html'
})
export class FormAnswerAttachmentsComponent extends BaseComponent {
  @Input() public formAnswerAttachmentsVm: PersonalFileModel[] | undefined;
  @Input() public showRemovedBtn: boolean = true;
  @Output('removeAttachment') public removeAttachmentEvent: EventEmitter<string> = new EventEmitter<string>();

  public isShowMessagePreventCookie: boolean = false;

  constructor(moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public removeAttachment(fileLocation: string): void {
    this.removeAttachmentEvent.emit(fileLocation);
  }

  public handleImgError(event: Event): void {
    this.isShowMessagePreventCookie = event.type === 'error' && PlatformHelper.isIOSDevice() ? true : false;
  }

  public canShowVideo(fileExtension: string): boolean {
    return FileUploaderHelpers.getFileType(fileExtension) === FileType.Video;
  }

  public canShowAudio(fileExtension: string): boolean {
    return FileUploaderHelpers.getFileType(fileExtension) === FileType.Audio;
  }

  public getMediaUrl(meidaUrl: string): string {
    return meidaUrl ? `${AppGlobal.environment.cloudfrontUrl}/${meidaUrl}` : '';
  }
}
