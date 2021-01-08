import { BaseFormComponent, Guid, IUploaderProgress, ModuleFacadeService, UploadProgressStatus, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { PublicUserInfo, UserRepository, VideoChapter } from '@opal20/domain-api';
import { VideoAnnotationChapterInfo, VideoAnnotationCommentInfo, VideoAnnotationMode } from '../../view-models/video-annotation-view.model';
import { VideojsChapter, VideojsPlayerCustom } from '@opal20/common-components';

import { VideoChapterListComponent } from './video-chapter-list.component';
import { VideoJsPlayerOptions } from 'video.js';
import { map } from 'rxjs/operators';

@Component({
  selector: 'video-annotation',
  templateUrl: './video-annotation.component.html'
})
export class VideoAnnotationComponent extends BaseFormComponent {
  @Input() public mode: VideoAnnotationMode = VideoAnnotationMode.Learn;

  @Input() public disableFullscreen: boolean = false;
  @Input() public fullscreenCallback: (isFullScreen: boolean) => void;

  @Input() public src: string;

  @Input() public fileExtension: string;
  @Input() public fileType: string;
  @Input() public fileDuration: number;

  @Input() public videoId: string;
  @Input() public ownerId: string;
  @Input() public description: string;

  @Input() public chapterInfo: VideoAnnotationChapterInfo;
  @Input() public commentInfo: VideoAnnotationCommentInfo;

  @Input() public set chapters(v: VideoChapter[]) {
    if (Utils.isDifferent(v, this._chapters)) {
      this._chapters = v;
      this.showChapterDetail(this.chapters[0]);
      this.getOwnerInfo();
    }
  }

  public get chapters(): VideoChapter[] {
    return this._chapters;
  }

  @Output() public videoErrorEvent: EventEmitter<Event> = new EventEmitter<Event>();

  @ViewChild(VideoChapterListComponent, { static: false })
  public videoChapterListComponent: VideoChapterListComponent;

  public currentChapter: VideoChapter;
  public contentOwner: PublicUserInfo;

  public showAnnotation: boolean = true;
  public videojsOption: VideoJsPlayerOptions;

  public currentVideoTime: number = 0;

  /**
   * Includes `currentChapter` && `uploadingInChapters` to keep the progress of the chapters that are uploading
   */
  public chapterDetailList: VideoChapter[] = [];
  /**
   * To keep the chapters that are uploading in background.
   */
  private uploadingInChapters: VideoChapter[] = [];

  private _chapters: VideoChapter[];
  private videojsPlayer: VideojsPlayerCustom;
  constructor(protected moduleFacadeService: ModuleFacadeService, private userRepository: UserRepository) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.videojsOption = {
      muted: false,
      fluid: true,
      aspectRatio: '16:9',
      sources: [
        {
          src: this.src,
          type: this.fileType
        }
      ],
      controlBar: {
        volumePanel: {
          inline: true
        },
        children: [
          'playToggle',
          'volumePanel',
          'currentTimeDisplay',
          'timeDivider',
          'durationDisplay',
          'progressControl',
          'fullscreenToggle'
        ]
      }
    };
  }

  public onChapterClicked(chapter: VideoChapter): void {
    this.moveVideojsTimestamp(chapter.timeStart);
    if (chapter !== this.currentChapter) {
      this.showChapterDetail(chapter);
    }
  }

  public onAddChapter(): void {
    const newChapterStartTime = this.chapters.length ? this.chapters[this.chapters.length - 1].timeEnd : 0;
    const newChapter = new VideoChapter({
      id: Guid.create().toString(),
      objectId: this.chapterInfo.objectId,
      originalObjectId: this.chapterInfo.originalObjectId,
      title: 'New Chapter',
      description: '',
      attachments: [],
      timeStart: newChapterStartTime,
      timeEnd: this.fileDuration ? this.fileDuration : 0
    });
    this.chapters.push(newChapter);
    this.addVideojsChapter(newChapter);
    this.showChapterDetail(newChapter);
  }

  public onUpdateChapter(): void {
    this.updateChapterInVideojs(this.currentChapter);
    if (this.mode === VideoAnnotationMode.Management) {
      this.videoChapterListComponent.validateChapterItem(this.currentChapter);
    }
  }

  public onVideoError(event: Event): void {
    this.videoErrorEvent.emit(event);
  }

  public onDurationLoaded(duration: number): void {
    if (Utils.isDifferent(this.fileDuration, duration)) {
      this.fileDuration = duration;
    }
  }

  public onTimestampClicked(timestamp: number): void {
    this.moveVideojsTimestamp(timestamp);
  }

  public onDeleteChapter(): void {
    this.removeUploadingChapterList(this.currentChapter);
    this.removeVideojsChapter();
    const index = this.chapters.indexOf(this.currentChapter);
    this.chapters.splice(index, 1);
    this.showChapterDetail(this.chapters[0]);
  }

  public onUploadingFile(event: { chapter: VideoChapter; uploaderProgress: IUploaderProgress }): void {
    if (event.uploaderProgress.status === UploadProgressStatus.Start) {
      this.uploadingInChapters.push(event.chapter);
      return;
    }
    const fileName = event.uploaderProgress.parameters.file.name;
    this.showNotification(`The file ${fileName} is uploaded successfully in chapter ${event.chapter.title}.`);
    this.removeUploadingChapterList(event.chapter);
  }

  public onVideojsReady(videojsPlayer: VideojsPlayerCustom): void {
    this.videojsPlayer = videojsPlayer;
    this.initVideojsChapter();
    this.initToggleAnnotateBtn();
  }

  public onVideoTimeUpdate(timestamp: number): void {
    this.currentVideoTime = Math.floor(timestamp);
  }

  protected additionalCanSaveCheck(): Promise<boolean> {
    return this.videoChapterListComponent.validate();
  }

  private onChapterChange(newChapter: VideojsChapter, oldChapter: VideojsChapter): void {
    if (!this.currentChapter || newChapter.id !== this.currentChapter.id) {
      const newCurrentChapter = this.chapters.find(p => p.id === newChapter.id);
      this.showChapterDetail(newCurrentChapter);
    }
  }

  private showChapterDetail(chapter: VideoChapter): void {
    this.removeChapterListIfNotUploading();
    this.currentChapter = chapter;
    if (!this.chapterDetailList.includes(this.currentChapter)) {
      this.chapterDetailList.push(this.currentChapter);
    }
  }

  private removeUploadingChapterList(chapter: VideoChapter): void {
    const index = this.uploadingInChapters.indexOf(chapter);
    if (index > -1) {
      this.uploadingInChapters.splice(index, 1);
    }
  }

  private removeChapterListIfNotUploading(): void {
    const chapterDetailIndex = this.chapterDetailList.indexOf(this.currentChapter);
    if (!this.uploadingInChapters.includes(this.currentChapter) && chapterDetailIndex > -1) {
      this.chapterDetailList.splice(chapterDetailIndex, 1);
    }
  }

  private getOwnerInfo(): void {
    this.userRepository
      .loadPublicUserInfoList({ userIds: [this.ownerId] }, false)
      .pipe(
        this.untilDestroy(),
        map(users => users && users[0])
      )
      .subscribe(p => {
        this.contentOwner = p;
      });
  }
  //#region videojs
  private initVideojsChapter(): void {
    const videojsChapterView = this.chapters.map(p => {
      return this.newVideojsChapter(p);
    });
    this.videojsPlayer.chapters({
      chapters: videojsChapterView,
      onChapterChange: this.onChapterChange.bind(this)
    });
  }

  private addVideojsChapter(chapter: VideoChapter): void {
    const newVideojsChapter = this.newVideojsChapter(chapter);
    this.videojsPlayer.chaptersFn.add(newVideojsChapter);
  }

  private updateChapterInVideojs(chapter: VideoChapter): void {
    if (this.videojsPlayer == null) {
      return;
    }
    const updatingVideojsChapter = this.newVideojsChapter(chapter);
    this.videojsPlayer.chaptersFn.update(updatingVideojsChapter);
  }

  private removeVideojsChapter(): void {
    this.videojsPlayer.chaptersFn.remove(this.currentChapter.id);
  }

  private moveVideojsTimestamp(timestamp: number): void {
    this.videojsPlayer.currentTime(timestamp);
  }

  private initToggleAnnotateBtn(): void {
    const btnPosition = 6;
    const newBtn = this.videojsPlayer.controlBar.addChild('Button', {}, btnPosition) as videojs.default.Button;

    const getBtnClass = () => `vjs-control vjs-button ${this.showAnnotation ? 'vjs-annotation-btn' : 'vjs-annotation-btn-off'} `;
    const btnEl = newBtn.el();
    btnEl.className = getBtnClass();
    const onBtnClicked = () => {
      this.showAnnotation = !this.showAnnotation;
      btnEl.className = getBtnClass();
    };
    // click event is not fired on device, so use 'touchstart'
    // https://github.com/videojs/video.js/issues/4817#issuecomment-382762373
    this.videojsPlayer.on(newBtn, 'click', onBtnClicked);
    this.videojsPlayer.on(newBtn, 'touchstart', onBtnClicked);
  }

  private newVideojsChapter(chapter: VideoChapter): VideojsChapter {
    const videojsChapter: VideojsChapter = {
      id: chapter.id,
      startTime: chapter.timeStart,
      endTime: chapter.timeEnd,
      title: chapter.title
    };
    return videojsChapter;
  }
  //#endregion
}
