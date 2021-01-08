import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { IVideoChapterSaveRequest, VideoChapter, VideoChapterApiService, VideoCommentSourceType } from '@opal20/domain-api';
import { VideoAnnotationChapterInfo, VideoAnnotationCommentInfo, VideoAnnotationMode } from '../../view-models/video-annotation-view.model';

@Component({
  selector: 'video-annotation-player',
  templateUrl: './video-annotation-player.component.html'
})
export class VideoAnnotationPlayerComponent extends BaseComponent {
  @Input() public videoId: string;
  @Input() public videoUrl: string;
  @Input() public fileType: string;
  @Input() public fileExtension: string;
  @Input() public ownerId: string;
  @Input() public mode: VideoAnnotationMode;
  @Input() public onSavedCallback: () => void;

  public chapterInfo: VideoAnnotationChapterInfo;
  public commentInfo: VideoAnnotationCommentInfo;

  public chapters: VideoChapter[];

  private _originalChapters: VideoChapter[];
  constructor(protected moduleFacadeService: ModuleFacadeService, private videoChapterApiService: VideoChapterApiService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.chapterInfo = {
      objectId: this.videoId
    };
    this.commentInfo = {
      objectId: this.videoId,
      sourceType: VideoCommentSourceType.CSL
    };
    this.videoChapterApiService.search({ objectId: this.videoId }).then(result => {
      this.updateChaptersData(result);
    });
  }

  public onCancel(): void {
    this.chapters = Utils.cloneDeep(this._originalChapters);
  }

  public onSave(): void {
    this.upsert()
      .then(chapters => this.updateChaptersData(chapters))
      .then(() => {
        if (this.onSavedCallback) {
          this.onSavedCallback();
        }
      });
  }

  public upsert(): Promise<VideoChapter[]> {
    const request: IVideoChapterSaveRequest = {
      objectId: this.videoId,
      chapters: this.chapters
    };
    return this.videoChapterApiService.update(request);
  }

  public get isEditMode(): boolean {
    return this.mode === VideoAnnotationMode.Management;
  }

  private updateChaptersData(chapters: VideoChapter[]): void {
    this._originalChapters = chapters;
    this.chapters = Utils.cloneDeep(this._originalChapters);
  }
}
