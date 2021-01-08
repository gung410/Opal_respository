import {
  Component,
  ViewEncapsulation,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  ElementRef,
  Input,
  EventEmitter,
  Output,
  OnInit,
  AfterViewInit,
  OnChanges,
  SimpleChanges
} from '@angular/core';
import { BaseComponent } from '../../abstracts/base.component';
import { MediaObserver } from '@angular/flex-layout';
import { uniqueId } from 'lodash';
import { CxStringUtil } from '../../utils/utils.public';
declare var $: any;
@Component({
  selector: 'cx-date-picker',
  templateUrl: './cx-date-picker.component.html',
  styleUrls: ['./cx-date-picker.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CxDatePickerComponent extends BaseComponent
  implements OnInit, AfterViewInit, OnChanges {
  @Input() date: Date;
  @Input() minDate: Date;
  @Input() maxDate: Date;
  @Input() changeMonth = true;
  @Input() changeYear = true;
  @Input() format = 'dd/mm/yy';
  @Input() formatLocate = 'en-GB';
  @Input() inputMaskFormat = 'dd/mm/yyyy';
  @Input() placeholder = 'dd/mm/yyyy';
  @Input() isDisabled = false;
  @Input() invalidMinDateText = 'Provided date is not in valid range. Min date is: ';
  @Input() invalidMaxDateText = 'Provided date is not in valid range. Max date is: ';
  @Output() dateChange = new EventEmitter<Date>();

  uniqueId = 'cx-date-picker-' + uniqueId();
  invalidText: string;
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    elementRef: ElementRef,
    media: MediaObserver
  ) {
    super(changeDetectorRef, elementRef, media);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.date === null) {
      this.clearCurrentDate();
    }
    this.updateDatePickerOption();
  }

  ngOnInit() {
    super.ngOnInit();
  }

  ngAfterViewInit() {
    const datePickerElement = $('#' + this.uniqueId);
    if (!datePickerElement) {
      return;
    }
    const options: any = {};
    options.onSelect = this.onChange;
    options.changeMonth = this.changeMonth;
    options.changeYear = this.changeYear;

    if (this.format) {
      options.dateFormat = this.format;
    }

    if (this.minDate) {
      options.minDate = this.minDate;
    }

    if (this.maxDate) {
      options.maxDate = this.maxDate;
    }

    datePickerElement.change(this.onChange);

    datePickerElement.inputmask({
      alias: 'datetime',
      inputFormat: this.inputMaskFormat,
    });

    datePickerElement.datepicker(options);

    if (this.date) {
      this.setDate(this.date);
    }
  }

  onChange = () => {
    const datePickerElement: any = $('#' + this.uniqueId);
    const inputValue = datePickerElement.val();

    if (inputValue === '' || !this.isValidDateTimeString(inputValue)) {
      this.onInvalidDate();
      return;
    }

    setTimeout(() => {
        const currentDate = this.getCurrentDate();

        if (!!this.minDate && currentDate.setHours(0, 0, 0, 0) < this.minDate.setHours(0, 0, 0, 0)) {
          this.invalidText = this.invalidMinDateText + this.minDate.toLocaleDateString(this.formatLocate);
          this.onInvalidDate();
          this.changeDetectorRef.detectChanges();
          return;
        }

        if (!!this.maxDate && currentDate.setHours(0, 0, 0, 0) > this.maxDate.setHours(0, 0, 0, 0)) {
          this.invalidText = this.invalidMaxDateText + this.maxDate.toLocaleDateString(this.formatLocate);
          this.onInvalidDate();
          this.changeDetectorRef.detectChanges();
          return;
        }

        this.invalidText = undefined;
        this.date = currentDate;
        this.dateChange.emit(this.date);
        this.changeDetectorRef.detectChanges();
    });
  }

  getCurrentDate = (): Date => {
    const datePickerElement: any = $('#' + this.uniqueId);
    if (datePickerElement) {
      return datePickerElement.datepicker('getDate');
    }
    return undefined;
  }

  setDate(date: Date) {
    const setDateField = 'setDate';
    const datePickerElement: any = $('#' + this.uniqueId);
    if (datePickerElement) {
      return datePickerElement.datepicker( setDateField, date);
    }
  }

  clearCurrentDate() {
    this.date = null;
    this.dateChange.emit(this.date);
    const datePickerElement: any = $('#' + this.uniqueId);
    if (datePickerElement) {
      return datePickerElement.datepicker('setDate', null);
    }
  }

  updateDatePickerOption() {
    const datePickerElement = $('#' + this.uniqueId);
    if (!datePickerElement) {
      return;
    }

    datePickerElement.datepicker( 'option', 'minDate', this.minDate );
    datePickerElement.datepicker( 'option', 'maxDate', this.maxDate );
  }

  private isValidDateTimeString(inputValue: any): boolean {
    inputValue = inputValue.replace(/\//g, '');
    return CxStringUtil.isNumber(inputValue);
  }

  private onInvalidDate() {
    this.date = undefined;
    this.setDate(this.date);
    this.dateChange.emit(this.date);
  }

}
