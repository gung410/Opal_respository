import { Component, Input, Output, EventEmitter, ViewEncapsulation, ChangeDetectionStrategy, TemplateRef, ChangeDetectorRef, ElementRef } from '@angular/core';
import { BaseComponent } from '../../abstracts/base.component';
import { MediaObserver } from '@angular/flex-layout';

@Component({
  selector: 'cx-tag-group',
  templateUrl: './cx-tag-group.component.html',
  styleUrls: ['./cx-tag-group.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CxTagGroupComponent<RowData> extends BaseComponent {
  @Input() tagGroup: any;
  @Input() tagProperty: string;
  @Input() tagGroupField: string;
  @Input() backgroundColor: string;
  @Input() clearAllText = 'Clear All';
  @Input() tagTemplate: TemplateRef<RowData>;

  @Output() closeTagGroup: EventEmitter<any> = new EventEmitter<any>();
  @Output() clearAll: EventEmitter<any> = new EventEmitter<any>();

  constructor(
    public changeDetectorRef: ChangeDetectorRef,
    public elementRef: ElementRef,
    public media: MediaObserver) {
    super(changeDetectorRef, elementRef, media);
  }

  public onCloseTagGroupClicked(tagGroup: any) {
    this.closeTagGroup.emit(tagGroup);
  }

  public onClearAllClicked() {
    this.clearAll.emit();
  }
}
