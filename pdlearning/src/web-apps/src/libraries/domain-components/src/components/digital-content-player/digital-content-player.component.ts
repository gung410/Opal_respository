import { AmazonS3UploaderService, BaseComponent, ModuleFacadeService, NotificationType, Utils } from '@opal20/infrastructure';
import {
  BrokenLinkContentType,
  BrokenLinkModuleIdentifier,
  ContentApiService,
  DigitalContent,
  UserRepository,
  VideoCommentSourceType
} from '@opal20/domain-api';
import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { VideoAnnotationChapterInfo, VideoAnnotationCommentInfo, VideoAnnotationMode } from '../../view-models/video-annotation-view.model';

import { BrokenLinkReportDialogComponent } from '../broken-link-report-dialog/broken-link-report-dialog.component';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { PlatformHelper } from '@opal20/common-components';
import { PlayerHelpers } from '../../helpers/player.helper';
import { WebAppLinkBuilder } from './../../helpers/webapp-link-builder.helper';
import { saveAs } from 'file-saver';

@Component({
  selector: 'digital-content-player',
  templateUrl: './digital-content-player.component.html',
  host: {
    '(contextmenu)': 'preventRightClick($event)'
  }
})
export class DigitalContentPlayerComponent extends BaseComponent {
  @Input() public disableFullscreen: boolean = false;
  @Input() public fullscreenCallback: (isFullScreen: boolean) => void;

  @Input()
  public lectureId: string;
  @Input()
  public classRunId: string;
  @Input()
  public myLectureId: string;
  @Input()
  public resourceId: string;
  @Input()
  public myDigitalContentId: string;
  @Input()
  public maxMainContentHeight: string = '100%';
  @Input()
  public displayMode: 'preview' | 'learn' = 'preview';
  @Input()
  public onScormFinishCallback: () => void;
  @Input()
  public reviewOnly: boolean;
  @Input()
  public isLectureNextButtonClicked: boolean;

  @Input()
  public disableDownload?: boolean;

  @Output()
  public onDownloadFile: EventEmitter<void> = new EventEmitter<void>();

  public contentType = {
    LearningContent: 'LearningContent',
    UploadedContent: 'UploadedContent'
  };

  public type: string;
  public contentUrl: string;
  public htmlContent: string;
  public title: string;
  public description: string;

  public urlListData: string[];
  public fileExtension: string;
  public digitalContent: DigitalContent;

  //#region video-annotation
  public videoId: string;
  public videoAnnotationChapterInfo: VideoAnnotationChapterInfo;
  public videoAnnotationCommentInfo: VideoAnnotationCommentInfo;
  //#endregion

  private fileLocation: string;
  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private contentApiService: ContentApiService,
    private uploaderService: AmazonS3UploaderService,
    private userRepository: UserRepository
  ) {
    super(moduleFacadeService);
  }

  public ngOnChanges(changes: SimpleChanges): void {
    if (changes.resourceId) {
      this.contentApiService.getDigitalContent(this.resourceId).then(data => {
        this.digitalContent = data;
        this.fileExtension = data.fileExtension;
        this.type = data.type;
        this.title = data.title;
        this.description = data.description;
        this.fileLocation = data.fileLocation;
        if (this.type !== this.contentType.LearningContent && !this.canShowScorm) {
          // Use Amazon CloudFront query string parameters authentication instead of signed cookies,
          // because we can't pass cookies to Google document viewer.
          if (this.canShowDocument) {
            this.uploaderService.getFile(data.fileLocation).then(url => {
              this.contentUrl = url;
            });
          } else {
            this.contentUrl = `${AppGlobal.environment.cloudfrontUrl}/${data.fileLocation}`;
          }
          if (this.canShowVideo) {
            this.initVideoAnnotation();
          }
        }
        this.extractUrlList(data.htmlContent);
        this.pipeHtmlContentData(data.htmlContent).then(rs => {
          this.htmlContent = rs;
        });
      });
    }
  }

  public async pipeHtmlContentData(data: string): Promise<string> {
    if (PlatformHelper.isIOSDevice() && !Utils.isNullOrUndefined(data)) {
      const uniqueUrls = [...new Set(Utils.extracUrlfromHtml(data))];

      await Promise.all(
        uniqueUrls.map(url =>
          this.uploaderService.getFile(url.replace(`${AppGlobal.environment.cloudfrontUrl}/`, '')).then(resp => {
            data = data.split(url).join(resp);
          })
        )
      );
    }

    return Promise.resolve<string>(data);
  }

  public onDownload(): void {
    if (this.canDownloadContent) {
      this.uploaderService.getFile(this.fileLocation).then(url => {
        const isIOSDevice = !!navigator.platform && /iPad|iPhone|iPod/.test(navigator.platform);
        if (isIOSDevice) {
          fetch(url).then(res => {
            res.blob().then(blob => {
              this.showNotification('The file is being downloaded.', NotificationType.Info);
              Utils.downloadFileByFileReader(blob, `${this.digitalContent.fileName}`);
              this.onDownloadFile.emit();
            });
          });

          return;
        }
        if (this.isImageFile(this.digitalContent.fileExtension)) {
          // some image files like svg can't download by saveAs so using fetch to get file
          // but this method get all data before create file download therefore with
          // large file user can't see the progress of downloading before it has completed.
          // Using this method only for small file such as images.
          fetch(url).then(res => {
            res.blob().then(blob => {
              this.showNotification('The file is being downloaded.', NotificationType.Info);
              Utils.downloadFile(blob, `${this.digitalContent.fileName}`);
              this.onDownloadFile.emit();
            });
          });

          return;
        }

        saveAs(url, this.digitalContent.fileName);
        this.showNotification('The file is being downloaded.', NotificationType.Info);
        this.onDownloadFile.emit();
      });
    }
  }

  public onDownloadHtmlContent(): void {
    if (this.canDownloadContent) {
      const isIOSDevice = !!navigator.platform && /iPad|iPhone|iPod/.test(navigator.platform);
      this.uploaderService.downloadHtmlContent(this.htmlContent, isIOSDevice).subscribe(url => {
        this.showNotification('The file is being downloaded.', NotificationType.Info);
        this.onDownloadFile.emit();
      });
    }
  }

  public get canDownloadContent(): boolean {
    return (
      (this.type === this.contentType.UploadedContent || this.type === this.contentType.LearningContent) &&
      this.digitalContent.isAllowDownload &&
      !this.disableDownload
    );
  }

  public get canShowDocument(): boolean {
    return this.type === this.contentType.UploadedContent && this.canShow(['pdf', 'docx', 'xlsx', 'pptx']);
  }

  public get canShowAudio(): boolean {
    return this.type === this.contentType.UploadedContent && this.canShow(['mp3', 'ogg']);
  }

  public get canShowVideo(): boolean {
    return this.type === this.contentType.UploadedContent && this.canShow(['mp4', 'm4v', 'ogv']);
  }
  public get canShowImage(): boolean {
    return this.type === this.contentType.UploadedContent && this.canShow(['jpeg', 'jpg', 'gif', 'png', 'svg']);
  }
  public get canShowScorm(): boolean {
    return this.type === this.contentType.UploadedContent && this.canShow(['scorm']);
  }

  public openReportBrokenLinkDialog(): void {
    this.userRepository
      .loadPublicUserInfoList(
        {
          userIds: [this.digitalContent.ownerId]
        },
        true
      )
      .subscribe(pagedOwners => {
        const owner = pagedOwners.find(u => u.id === this.digitalContent.ownerId);

        const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: BrokenLinkReportDialogComponent });
        const configurationPopup = dialogRef.content.instance as BrokenLinkReportDialogComponent;

        configurationPopup.urlData = this.urlListData;
        configurationPopup.objectId = this.resourceId;
        configurationPopup.originalObjectId = this.digitalContent.originalObjectId;
        configurationPopup.module = BrokenLinkModuleIdentifier.Content;
        configurationPopup.contentType = BrokenLinkContentType.LearningContent;
        configurationPopup.objectDetailUrl = WebAppLinkBuilder.buildDigitalContentUrl(this.digitalContent.id);
        configurationPopup.objectOwnerId = this.digitalContent.ownerId;
        configurationPopup.objectOwnerName = owner ? owner.fullName : '';
        configurationPopup.objectTitle = this.digitalContent.title;

        configurationPopup.isPreviewMode = this.displayMode === 'preview' ? true : false;
      });
  }

  public extractUrlList(html: string): void {
    if (Utils.isNullOrUndefined(html)) {
      return;
    }
    this.urlListData = Utils.extracUrlfromHtml(html);
  }

  public canShowReportBrokenLinkBtn(): boolean {
    return !Utils.isNullOrEmpty(this.urlListData);
  }

  public preventRightClick(e: MouseEvent): void {
    PlayerHelpers.preventRightClick(e);
  }

  public get videoAnnotationMode(): VideoAnnotationMode {
    return this.displayMode === 'learn' ? VideoAnnotationMode.Learn : VideoAnnotationMode.View;
  }

  private initVideoAnnotation(): void {
    this.loadVideoId();
    this.videoAnnotationChapterInfo = {
      objectId: this.digitalContent.id,
      originalObjectId: this.digitalContent.originalObjectId
    };
    this.videoAnnotationCommentInfo = this.lectureId
      ? {
          objectId: this.classRunId,
          originalObjectId: this.lectureId,
          sourceType: VideoCommentSourceType.LMM
        }
      : {
          objectId: this.digitalContent.id,
          originalObjectId: this.digitalContent.originalObjectId,
          sourceType: VideoCommentSourceType.CCPM
        };
  }

  private loadVideoId(): void {
    this.videoId = this.digitalContent.getVideoId();
  }

  private canShow(extensions: string[]): boolean {
    return extensions.some(item => item === this.fileExtension);
  }

  private isImageFile(fileExtension: string): boolean {
    return ['jpeg', 'jpg', 'gif', 'png', 'svg'].includes(fileExtension.toLowerCase());
  }
}
