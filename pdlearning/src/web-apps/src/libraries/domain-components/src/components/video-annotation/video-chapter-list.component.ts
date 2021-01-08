import { BaseFormComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, Input, Output, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { PublicUserInfo, VideoChapter } from '@opal20/domain-api';

import { VideoAnnotationMode } from '../../view-models/video-annotation-view.model';
import { VideoChapterListItemComponent } from './video-chapter-list-item.component';

@Component({
  selector: 'video-chapter-list',
  templateUrl: './video-chapter-list.component.html'
})
export class VideoChapterListComponent extends BaseFormComponent {
  @ViewChild('chapterList', { static: false })
  public chapterListComponent: ElementRef;
  @Input() public mode: VideoAnnotationMode = VideoAnnotationMode.Learn;

  @Input() public chapters: VideoChapter[];
  @Input() public contentOwner: PublicUserInfo;
  @Input() public currentChapter: VideoChapter;

  @Output() public chapterClicked: EventEmitter<VideoChapter> = new EventEmitter<VideoChapter>();
  @Output() public addChapter: EventEmitter<void> = new EventEmitter<void>();

  @ViewChildren(VideoChapterListItemComponent)
  public videoChapterListItemComponents: QueryList<VideoChapterListItemComponent>;

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public validateChapterItem(chapter: VideoChapter): void {
    const validatingChapter = this.videoChapterListItemComponents.toArray().find(p => p.chapter === chapter);
    if (validatingChapter) {
      validatingChapter.validate();
    }
  }

  public onChapterClicked(chapter: VideoChapter): void {
    this.chapterClicked.emit(chapter);
  }

  public onAddChapterClicked(): void {
    this.addChapter.emit();
    this.scrollToEnd();
  }

  public get isManagementMode(): boolean {
    return this.mode === VideoAnnotationMode.Management;
  }

  protected additionalCanSaveCheck(): Promise<boolean> {
    return Promise.all(this.videoChapterListItemComponents.toArray().map(p => p.validate())).then(
      finalResult => !finalResult.includes(false)
    );
  }

  private scrollToEnd(): void {
    setTimeout(() => {
      this.chapterListComponent.nativeElement.scrollTop = this.chapterListComponent.nativeElement.scrollHeight;
    });
  }
}
