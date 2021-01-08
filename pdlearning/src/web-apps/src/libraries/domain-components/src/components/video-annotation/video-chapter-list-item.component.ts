import {
  BaseFormComponent,
  CustomFormControl,
  IFormBuilderDefinition,
  ModuleFacadeService,
  TranslationMessage,
  Utils
} from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, Input, Output, QueryList, ViewChildren } from '@angular/core';

import { ValidatorFn } from '@angular/forms';
import { VideoChapter } from '@opal20/domain-api';

const validateOverlappingChapter = 'validateOverlappingChapter';
const validateRequiredChapterName = 'validateRequiredChapterName';
const validateStartEndTime = 'validateStartEndTime';

@Component({
  selector: 'video-chapter-list-item',
  templateUrl: './video-chapter-list-item.component.html'
})
export class VideoChapterListItemComponent extends BaseFormComponent {
  @Input() public chapters: VideoChapter[];
  @Input() public chapter: VideoChapter;
  @Input() public index: number;
  @Input() public isSelected: boolean;

  @Output() public chapterClicked: EventEmitter<VideoChapter> = new EventEmitter<VideoChapter>();
  @ViewChildren('input') private inputElements: QueryList<ElementRef>;
  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onChapterClicked(): void {
    this.chapterClicked.emit(this.chapter);
  }

  public overlappingChapter(): VideoChapter {
    return this.chapters.find(p => this.chapter !== p && this.isOverlapping(this.chapter, p));
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'formChapterListItem' + new Date(),
      validateByGroupControlNames: [['title', 'timeStart', 'timeEnd']],
      controls: {
        title: {
          defaultValue: this.chapter.title,
          validators: [
            {
              validator: this.requiredNameValidator(),
              validatorType: validateRequiredChapterName,
              message: new TranslationMessage(this.moduleFacadeService.globalTranslator, 'Chapter name is required')
            },
            {
              validator: this.overlappingValidator(),
              validatorType: validateOverlappingChapter,
              message: (control: CustomFormControl) =>
                new TranslationMessage(this.moduleFacadeService.globalTranslator, `This time line is overlap with chapter`, {
                  chapterName: this.overlappingChapter() && this.overlappingChapter().title
                })
            },
            {
              validator: this.startEndTimeValidator(),
              validatorType: validateStartEndTime,
              message: new TranslationMessage(this.moduleFacadeService.globalTranslator, 'Start time cannot be greater than end time')
            }
          ]
        },
        timeStart: {
          defaultValue: this.chapter.timeStart
        },
        timeEnd: {
          defaultValue: this.chapter.timeEnd
        }
      }
    };
  }

  // Workaround to force validating because the current inputs don't have event
  protected additionalCanSaveCheck(): Promise<boolean> {
    this.inputElements.toArray().forEach(input => {
      input.nativeElement.dispatchEvent(new Event('focusout'));
    });
    return Promise.resolve(true);
  }

  private requiredNameValidator(): ValidatorFn {
    return (control: CustomFormControl) => {
      if (Utils.isNullOrEmpty(this.chapter.title)) {
        return {
          [validateRequiredChapterName]: true
        };
      }
      return null;
    };
  }

  private overlappingValidator(): ValidatorFn {
    return (control: CustomFormControl) => {
      if (this.overlappingChapter() != null) {
        return {
          [validateOverlappingChapter]: true
        };
      }
      return null;
    };
  }

  private startEndTimeValidator(): ValidatorFn {
    return (control: CustomFormControl) => {
      if (this.chapter.timeStart > this.chapter.timeEnd) {
        return {
          [validateStartEndTime]: true
        };
      }
      return null;
    };
  }

  private isOverlapping(chapter1: VideoChapter, chapter2: VideoChapter): boolean {
    return (
      (chapter1.timeStart < chapter2.timeStart && chapter1.timeEnd > chapter2.timeStart) ||
      (chapter1.timeStart < chapter2.timeEnd && chapter1.timeEnd > chapter2.timeEnd)
    );
  }
}
