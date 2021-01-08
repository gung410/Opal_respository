import {
  Component,
  OnInit,
  ElementRef,
  ChangeDetectorRef,
  Input,
  ViewEncapsulation,
  ChangeDetectionStrategy,
  ViewChild,
  OnDestroy
} from '@angular/core';
import { CxCheckboxComponent } from '../cx-checkbox/cx-checkbox.component';
import { MediaObserver } from '@angular/flex-layout';
import { uniqueId } from 'lodash';

declare var $: any;
@Component({
  selector: "cx-customized-checkbox",
  templateUrl: './cx-customized-checkbox.component.html',
  styleUrls: ['./cx-customized-checkbox.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CxCustomizedCheckboxComponent extends CxCheckboxComponent
  implements OnDestroy {
  @Input() parentName: string;
  @Input() groupName: string;
  @Input() childrenGroupName: string;
  @Input() get selected(): boolean {
    return this._selected;
  }
  set selected(val: boolean) {
    if (this._selected !== val) {
      this._selected = val;
    }
    this.setSelectedForRelatedCheckboxes(val);
  }
  public checkboxId: string;
  @ViewChild('checkbox') checkbox: ElementRef;
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    elementRef: ElementRef,
    media: MediaObserver
  ) {
    super(changeDetectorRef, elementRef, media);
    this.checkboxId = uniqueId();
  }

  ngOnInit() {
    super.ngOnInit();
  }

  ngAfterViewInit() {
    super.ngAfterViewInit();
    $(this.checkbox.nativeElement).on('change', event => {
      this.valueSelected.emit({
        selected: event.currentTarget.checked,
        value: this.assignedValue
      });
      this.detectChanges();
    });
    this.setSelectedForRelatedCheckboxes(this.selected);
  }

  ngOnDestroy() {
    super.ngOnDestroy();
    $(this.checkbox.nativeElement).remove();
    this.updateSelectedForParentWhenRemove();
  }

  private setSelectedForRelatedCheckboxes(currentValue: boolean) {
    // Handle for parent
    if (this.parentName !== undefined) {
      const parentCheckboxRefs = $(`input[name='${this.parentName}']`);
      const hasParent = parentCheckboxRefs.length > 0;
      if (hasParent) {
        const siblingCheckboxRefs = $(
          `input[name='${this.groupName}']:not(#${this.checkboxId})`
        );
        if (currentValue === true) {
          const allSiblingChecked =
            siblingCheckboxRefs.not(':checked').length === 0;
          if (allSiblingChecked) {
            parentCheckboxRefs.prop('indeterminate', false);
            parentCheckboxRefs.prop('checked', true);
          } else {
            parentCheckboxRefs.prop('indeterminate', true);
            parentCheckboxRefs.prop('checked', false);
          }
        } else {
          const anySiblingChecked =
            this.anyCheckboxChecked(siblingCheckboxRefs);
          if (anySiblingChecked) {
            parentCheckboxRefs.prop('indeterminate', true);
            parentCheckboxRefs.prop('checked', false);
          } else {
            parentCheckboxRefs.prop('indeterminate', false);
            parentCheckboxRefs.prop('checked', false);
          }
        }
      }
    }
    // Handle children
    const childrenCheckboxRefs = $(`input[name='${this.childrenGroupName}']`);
    const hasChildren = childrenCheckboxRefs.length > 0;
    if (hasChildren) {
      const isCurrentCheckboxIndeterminate = $(`#${this.checkboxId}`).prop(
        'indeterminate'
      );
      if (isCurrentCheckboxIndeterminate === true) {
        childrenCheckboxRefs.prop('checked', true).trigger('change');
      } else {
        childrenCheckboxRefs.prop('checked', currentValue).trigger('change');
      }
    }
  }

  private updateSelectedForParentWhenRemove() {
    const parentCheckboxRefs = $(`input[name='${this.parentName}']`);
    const hasParent = parentCheckboxRefs.length > 0;
    if (hasParent) {
      const siblingCheckboxRefs = $(
        `input[name='${this.groupName}']:not(#${this.checkboxId})`
      );
      const allSiblingChecked =
        siblingCheckboxRefs.length > 0 && siblingCheckboxRefs.not(':checked').length === 0;
      if (allSiblingChecked) {
        parentCheckboxRefs.prop('indeterminate', false);
        parentCheckboxRefs.prop('checked', true);
      } else {
        const anySiblingChecked = this.anyCheckboxChecked(siblingCheckboxRefs);
        if (anySiblingChecked) {
          parentCheckboxRefs.prop('indeterminate', true);
          parentCheckboxRefs.prop('checked', false);
        } else {
          parentCheckboxRefs.prop('indeterminate', false);
          parentCheckboxRefs.prop('checked', false);
        }
      }
    }
  }

  private anyCheckboxChecked(checkboxRefs) {
    for (let i = 0; i < checkboxRefs.length; i++) {
      if (checkboxRefs[i].checked === false) {
        continue;
      }
      return true;
    }
    return false;
  }
}
