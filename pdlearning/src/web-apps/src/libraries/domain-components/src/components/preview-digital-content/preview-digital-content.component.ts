import { AmazonS3UploaderService, BaseFormComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input, ViewChild } from '@angular/core';
import { DigitalContent, DigitalContentType, VideoCommentSourceType } from '@opal20/domain-api';
import { VideoAnnotationChapterInfo, VideoAnnotationCommentInfo, VideoAnnotationMode } from '../../view-models/video-annotation-view.model';

import { DigitalContentDetailMode } from '../../models/digital-content-detail-mode.model';
import { VideoAnnotationComponent } from '../video-annotation/video-annotation.component';

@Component({
  selector: 'preview-digital-content',
  templateUrl: './preview-digital-content.component.html'
})
export class PreviewDigitalContentComponent extends BaseFormComponent {
  public contentUrl: string;
  public description: string;
  public htmlContent: string;
  public isShowMessagePreventCookie: boolean = false;
  public resourceId: string;

  @Input() public set digitalContent(v: DigitalContent) {
    if (Utils.isDifferent(v, this._digitalContent)) {
      this._digitalContent = v;
      this.resourceId = this._digitalContent.id;
      this.onContentLoaded();
    }
  }
  public get digitalContent(): DigitalContent {
    return this._digitalContent;
  }

  public get lectureId(): string {
    return this._lectureId;
  }
  @Input()
  public set lectureId(v: string) {
    this._lectureId = v;
  }
  @Input() public showSpinner: boolean;
  @Input() public mode: DigitalContentDetailMode = DigitalContentDetailMode.View;
  @Input() public videoAnnotationCommentInfo: VideoAnnotationCommentInfo;

  @ViewChild(VideoAnnotationComponent, { static: false })
  public videoAnnotationComponent: VideoAnnotationComponent;

  public videoAnnotationChapterInfo: VideoAnnotationChapterInfo;

  public videoId: string;

  private _digitalContent: DigitalContent;
  private fileExtension: string;
  private _lectureId: string;
  constructor(protected moduleFacadeService: ModuleFacadeService, private uploaderService: AmazonS3UploaderService) {
    super(moduleFacadeService);
  }

  public get canShowDocument(): boolean {
    return this.canShow(['pdf', 'docx', 'xlsx', 'pptx']);
  }

  public get canShowAudio(): boolean {
    return this.canShow(['mp3', 'ogg']);
  }

  public get canShowVideo(): boolean {
    return this.canShow(['mp4', 'm4v', 'ogv']);
  }

  public get canShowScorm(): boolean {
    return this.canShow(['scorm']);
  }

  public get canShowImage(): boolean {
    return this.canShow(['jpeg', 'jpg', 'gif', 'png', 'svg']);
  }

  public get canShowLearningContent(): boolean {
    return this.htmlContent !== undefined;
  }

  public onContentLoaded(): void {
    this.contentUrl = null;
    if (this.digitalContent.type === DigitalContentType.LearningContent) {
      this.htmlContent = this.digitalContent.htmlContent;
      return;
    } else {
      this.htmlContent = undefined;
      this.setupVideoAnnotation();
    }

    this.fileExtension = this.digitalContent.fileExtension;
    this.description = this.digitalContent.description;
    // Use Amazon CloudFront query string parameters authentication instead of signed cookies,
    // because we can't pass cookies to Google document viewer.
    if (this.canShowDocument) {
      this.uploaderService.getFile(this.digitalContent.fileLocation).then(url => {
        this.contentUrl = url;
      });
    }

    if (this.canShowImage || this.canShowVideo || this.canShowAudio || this.canShowImage) {
      this.contentUrl = `${AppGlobal.environment.cloudfrontUrl}/${this.digitalContent.fileLocation}`;
    }

    if (this.canShowScorm) {
      // Do not thing. Just call getter to re-computed value
    }
  }

  public handleImgError(event: Event): void {
    // For now, only show message if user uses ios. this issue occur because the OS prevent cookie
    const isIOSDevice = !!navigator.platform && /iPad|iPhone|iPod/.test(navigator.platform);
    this.isShowMessagePreventCookie = event.type === 'error' && isIOSDevice ? true : false;
  }

  public get canEditVideoAnnotation(): boolean {
    return this.mode === DigitalContentDetailMode.Create || this.mode === DigitalContentDetailMode.Edit;
  }

  public get viewAnnotationMode(): VideoAnnotationMode {
    return this.canEditVideoAnnotation ? VideoAnnotationMode.Management : VideoAnnotationMode.View;
  }

  protected additionalCanSaveCheck(): Promise<boolean> {
    return this.videoAnnotationComponent ? this.videoAnnotationComponent.validate() : Promise.resolve(true);
  }

  private canShow(extensions: string[]): boolean {
    return extensions.some(item => item === this.fileExtension);
  }

  private setupVideoAnnotation(): void {
    this.videoId = this.digitalContent.getVideoId();
    this.videoAnnotationChapterInfo = {
      objectId: this.digitalContent.id,
      originalObjectId: this.digitalContent.originalObjectId
    };
    if (this.videoAnnotationCommentInfo == null) {
      this.videoAnnotationCommentInfo = {
        objectId: this.digitalContent.id,
        originalObjectId: this.digitalContent.originalObjectId,
        sourceType: VideoCommentSourceType.CCPM
      };
    }
  }
}
