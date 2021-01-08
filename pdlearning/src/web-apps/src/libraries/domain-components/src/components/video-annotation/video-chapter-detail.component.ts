import {
  AmazonS3ApiService,
  BaseFormComponent,
  FileUploaderSetting,
  Guid,
  IFormBuilderDefinition,
  IUploaderProgress,
  ModuleFacadeService,
  UploadProgressStatus,
  Utils
} from '@opal20/infrastructure';
import { ChapterAttachment, VideoChapter } from '@opal20/domain-api';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';

import { OpalFileUploaderComponent } from '@opal20/common-components';
import { Subject } from 'rxjs';
import { VideoAnnotationMode } from '../../view-models/video-annotation-view.model';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'video-chapter-detail',
  templateUrl: './video-chapter-detail.component.html'
})
export class VideoChapterDetailComponent extends BaseFormComponent {
  @ViewChild(OpalFileUploaderComponent, { static: false }) public opalFileUploaderComponent: OpalFileUploaderComponent;
  @Input() public mode: VideoAnnotationMode = VideoAnnotationMode.Learn;

  @Input() public set videoDuration(v: number) {
    if (Utils.isDifferent(v, this._videoDuration)) {
      this._videoDuration = v;
      this.updateLimitTimeRangeOfView();
    }
  }

  public get videoDuration(): number {
    return this._videoDuration;
  }

  @Input() public set currentChapter(v: VideoChapter) {
    if (Utils.isDifferent(v, this._currentChapter)) {
      this._currentChapter = v;
      this.updateTimeRangeOfView();
    }
  }
  public get currentChapter(): VideoChapter {
    return this._currentChapter;
  }

  @Output() public deleteChapter: EventEmitter<void> = new EventEmitter<void>();
  /**
   * Just only emits when user updates data related to Time range, Chapter name (what show on video)
   */
  @Output() public updateChapter: EventEmitter<void> = new EventEmitter<void>();
  @Output() public uploadingFile: EventEmitter<{ chapter: VideoChapter; uploaderProgress: IUploaderProgress }> = new EventEmitter();

  public videoMinLengthAsDate: Date;
  public videoMaxLengthAsDate: Date;

  public uploaderUrl: string;
  public fileUploaderSetting: FileUploaderSetting = new FileUploaderSetting({ isCropImage: false });
  public uploadFolder: string = 'video-chapter-attachments';
  public showUploader: boolean = false;
  public isUploadingFile: boolean = false;

  private _timeStart: Date;
  private _timeEnd: Date;
  private _videoDuration: number;
  private _currentChapter: VideoChapter;
  private updateChapter$: Subject<void> = new Subject<void>();
  constructor(protected moduleFacadeService: ModuleFacadeService, public amazonS3ApiService: AmazonS3ApiService) {
    super(moduleFacadeService);
    this.setupUpdatingChapter();
  }

  public set timeStart(value: Date) {
    if (value === this._timeStart) {
      return;
    }
    const validTime = this.getValidTime(value);
    if (value !== validTime) {
      this.timeStart = validTime;
      return;
    }
    this._timeStart = validTime;
    this.currentChapter.timeStart = this._timeStart.getTime() / 1000;
    this.updateChapter$.next();
  }

  public get timeStart(): Date {
    return this._timeStart;
  }

  public set timeEnd(value: Date) {
    if (value === this._timeEnd) {
      return;
    }
    const validTime = this.getValidTime(value);
    if (value !== validTime) {
      this.timeEnd = validTime;
      return;
    }
    this._timeEnd = validTime;
    this.currentChapter.timeEnd = this._timeEnd.getTime() / 1000;
    this.updateChapter$.next();
  }

  public get timeEnd(): Date {
    return this._timeEnd;
  }

  public set chapterName(v: string) {
    if (v === this.currentChapter.title) {
      return;
    }
    this.currentChapter.title = v;
    this.updateChapter$.next();
  }

  public get chapterName(): string {
    return this.currentChapter.title;
  }

  public onAttachmentClicked(fileLocation: string): void {
    this.amazonS3ApiService.getFile(fileLocation).then(p => {
      window.open(p, '_blank');
    });
  }

  public onDeleteChapter(): void {
    if (this.isUploadingFile) {
      this.opalFileUploaderComponent.abortUpload();
    }
    this.deleteChapter.emit();
  }

  public removeAttachment(attachmentIndex: number): void {
    this.currentChapter.attachments.splice(attachmentIndex, 1);
  }

  public onUploaderChanged(uploaderProgress: IUploaderProgress): void {
    if (uploaderProgress.status === UploadProgressStatus.Completed) {
      const newAttachment = new ChapterAttachment({
        id: Guid.create().toString(),
        objectId: this.currentChapter.id,
        fileName: uploaderProgress.parameters.file.name,
        fileLocation: uploaderProgress.parameters.fileLocation
      });
      this.currentChapter.attachments.push(newAttachment);
      this.opalFileUploaderComponent.resetUpload();
    }
    this.isUploadingFile = uploaderProgress.status === UploadProgressStatus.Start;
    this.uploadingFile.emit({ chapter: this.currentChapter, uploaderProgress: uploaderProgress });
  }

  public get isViewMode(): boolean {
    return this.mode === VideoAnnotationMode.Learn || this.mode === VideoAnnotationMode.View;
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        title: {
          defaultValue: null
        },
        description: {
          defaultValue: null
        },
        timeStart: {
          defaultValue: null
        },
        timeEnd: {
          defaultValue: null
        },
        attachments: {
          defaultValue: null
        },
        uploader: {
          defaultValue: null
        }
      }
    };
  }

  /**
   * @returns the value of time range if the time is invalid. Otherwise return itself.
   */
  private getValidTime(time: Date): Date {
    const dateTemp = new Date();
    if (time > this.videoMaxLengthAsDate) {
      dateTemp.setTime(this.videoMaxLengthAsDate.getTime());
      return dateTemp;
    }
    if (time < this.videoMinLengthAsDate) {
      dateTemp.setTime(this.videoMinLengthAsDate.getTime());
      return dateTemp;
    }
    return time;
  }

  private updateTimeRangeOfView(): void {
    if (this.currentChapter == null) {
      return;
    }
    const timeStartTemp = new Date();
    timeStartTemp.setTime(this.currentChapter.timeStart * 1000);
    this._timeStart = timeStartTemp;

    const timeEndTemp = new Date();
    timeEndTemp.setTime(this.currentChapter.timeEnd * 1000);
    this._timeEnd = timeEndTemp;
  }

  private updateLimitTimeRangeOfView(): void {
    const minDate = new Date();
    minDate.setTime(0);
    this.videoMinLengthAsDate = minDate;

    const maxDate = new Date();
    maxDate.setTime(this.videoDuration * 1000);
    this.videoMaxLengthAsDate = maxDate;
  }

  private setupUpdatingChapter(): void {
    this.updateChapter$
      .pipe(
        debounceTime(300),
        this.untilDestroy()
      )
      .subscribe(() => this.updateChapter.emit());
  }
}
