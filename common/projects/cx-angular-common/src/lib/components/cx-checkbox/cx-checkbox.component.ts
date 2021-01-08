import {
  Component,
  ViewEncapsulation,
  ChangeDetectorRef,
  ElementRef,
  Input,
  ChangeDetectionStrategy,
  EventEmitter,
  Output
} from "@angular/core";
import { MediaObserver } from "@angular/flex-layout";
import { BaseComponent } from "../../abstracts/base.component";

@Component({
  selector: "cx-checkbox",
  templateUrl: "./cx-checkbox.component.html",
  styleUrls: ["./cx-checkbox.component.scss"],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CxCheckboxComponent extends BaseComponent {
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    elementRef: ElementRef,
    media: MediaObserver
  ) {
    super(changeDetectorRef, elementRef, media);
  }

  @Input() assignedValue: any = true;
  @Input() disabled = false;
  protected _selected = false;
  @Input() get selected(): boolean {
    return this._selected;
  }
  set selected(val: boolean) {
    if (this._selected !== val) {
      this._selected = val;
      this.selectedChange.emit(val);
      this.valueSelected.emit({ selected: val, value: this.assignedValue });
    }
  }

  @Output() selectedChange = new EventEmitter<boolean>();
  @Output() valueSelected = new EventEmitter<{
    selected: boolean;
    value: any;
  }>();
}
