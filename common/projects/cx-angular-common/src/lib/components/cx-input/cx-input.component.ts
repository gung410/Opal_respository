import {
  Component,
  Input,
  ElementRef,
  ChangeDetectorRef,
  ViewEncapsulation,
  ChangeDetectionStrategy,
  Output,
  EventEmitter,
  ViewChild
} from '@angular/core';
import { MediaObserver } from '@angular/flex-layout';
import { BaseComponent } from '../../abstracts/base.component';

@Component({
  selector: "cx-input",
  templateUrl: './cx-input.component.html',
  styleUrls: ['./cx-input.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CxInputComponent extends BaseComponent {
  private _value: string;
  @Input() get value() {
    return this._value;
  }
  set value(val) {
    this._value = val;
    this.valueChange.emit(this.value);
  }
  @Output() valueChange: EventEmitter<string> = new EventEmitter();
  @Input() iconClass = 'input';
  @Input() clearButtonIconClass: string;
  @Input() placeholder: string;
  @Output() clear: EventEmitter<undefined> = new EventEmitter();
  @Output() submit: EventEmitter<string> = new EventEmitter();
  @ViewChild('input') public inputRef: ElementRef;
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    elementRef: ElementRef,
    media: MediaObserver
  ) {
    super(changeDetectorRef, elementRef, media);
  }

  ngOnInit() {}

  public onClearClicked() {
    this.value = '';
    this.focusInput();
    this.clear.emit();
  }

  public onInputIconClicked() {
    this.submit.emit(this.value);
  }

  public focusInput() {
    setTimeout(() => {
      this.inputRef.nativeElement.focus();
    });
  }

  public onEnterKeyUp() {
    this.submit.emit(this.value);
  }
}
