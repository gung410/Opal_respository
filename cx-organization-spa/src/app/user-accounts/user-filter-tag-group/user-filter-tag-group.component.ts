import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  Output,
  ViewEncapsulation
} from '@angular/core';
import { BasePresentationComponent } from 'app/shared/components/component.abstract';

import { AppliedFilterModel } from '../user-filter/applied-filter.model';

@Component({
  selector: 'user-filter-tag-group',
  templateUrl: './user-filter-tag-group.component.html',
  styleUrls: ['./user-filter-tag-group.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserFilterTagGroupComponent extends BasePresentationComponent {
  @Input() filterDataApplied: { appliedFilter: AppliedFilterModel[] };

  @Output() clearAll: EventEmitter<any> = new EventEmitter<any>();
  @Output() closeTagGroup: EventEmitter<any> = new EventEmitter<any>();
  @Output() closeTag: EventEmitter<any> = new EventEmitter<any>();

  constructor(changeDetectorRef: ChangeDetectorRef) {
    super(changeDetectorRef);
  }

  onCloseTagGroup(tagGroup: AppliedFilterModel): void {
    const groupIndex = this.filterDataApplied.appliedFilter.findIndex(
      (item) => item.id === tagGroup.id
    );
    this.filterDataApplied.appliedFilter.splice(groupIndex, 1);
    this.filterDataApplied = { ...this.filterDataApplied };
    this.closeTagGroup.emit(this.filterDataApplied);
  }

  onCloseTag(tag: AppliedFilterModel): void {
    const groupIndex = this.filterDataApplied.appliedFilter.findIndex(
      (item) => item.data.id === tag.id
    );
    this.filterDataApplied.appliedFilter.splice(groupIndex, 1);
    this.filterDataApplied = { ...this.filterDataApplied };
    this.closeTag.emit(this.filterDataApplied);
  }

  onClearAllFilter(): void {
    this.filterDataApplied.appliedFilter = [];
    this.changeDetectorRef.detectChanges();
    this.clearAll.emit(this.filterDataApplied);
  }
}
