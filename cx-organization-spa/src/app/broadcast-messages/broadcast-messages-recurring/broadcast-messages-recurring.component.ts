import {
  Component,
  Inject,
  OnInit,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormGroupDirective,
  Validators
} from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { FormBuilderService } from 'app-services/form-builder.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { Utils } from 'app-utilities/utils';
import { DAY_OF_WEEK } from 'app/shared/constants/broadcast-message-status.constant';
import { RecurrenceType } from 'app/shared/constants/recurrence-type.enum';
import { requiredIfValidator } from 'app/shared/validators/required-if-validator';
import { BroadcastMessagesDetailComponent } from '../broadcast-messages-detail/broadcast-messages-detail.component';
import { RecurringDialogModel } from '../models/recurring-dialog.model';
import {
  DateTimeValidation,
  RecurringDateValidation,
  RecurringModel,
  WeeklyValidation
} from '../models/recurring.model';

const getMonth = (idx: number): string => {
  const objDate = new Date();
  objDate.setDate(1);
  objDate.setMonth(idx - 1);
  const locale = 'en-us';
  const month = objDate.toLocaleString(locale, { month: 'long' });

  return month;
};

const MAX_MONTH = 12;
const MAX_WEEK_DAYS = 6;

@Component({
  selector: 'broadcast-messages-recurring',
  templateUrl: './broadcast-messages-recurring.component.html',
  styleUrls: ['./broadcast-messages-recurring.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class BroadcastMessageRecurringDialogComponent implements OnInit {
  @ViewChild(FormGroupDirective) recurringFormDirective: FormGroupDirective;
  get startDays(): number[] {
    const dayCount = this.getDaysInMonth(
      this.selectedYear,
      this.data.startMonth
    );
    if (!dayCount) {
      return [];
    }

    return Array(dayCount)
      .fill(0)
      .map((i, idx) => idx + 1);
  }

  get endDays(): number[] {
    const dayCount = this.getDaysInMonth(this.selectedYear, this.data.endMonth);
    if (!dayCount) {
      return [];
    }

    return Array(dayCount)
      .fill(0)
      .map((i, idx) => idx + 1);
  }

  get recurrenceOptionType(): RecurrenceType {
    return this.data.type;
  }
  set recurrenceOptionType(recurringType: RecurrenceType) {
    if (!recurringType) {
      return;
    }
    this.resetRecurringDate();
    this.recurringFormDirective.resetForm();
    this.setAllValidation();
    this.data.type = recurringType;
    this.formBuilderSvc.updateAllValidators(this.recurringForm);
  }
  name: string = 'Broadcast Message';
  recurringType: string[] = [];
  recurringTimes: number = 0;
  daysOfTheWeek: string[] = [];
  maxDayOfMonth: number = 31;
  daysOfMonth: number[] = [];
  months: string[] = [];

  selectedYear: number = new Date().getFullYear();
  selectedStartMonth: number = new Date().getMonth() + 1; // array begin by 0
  selectedStartDay: number = new Date().getUTCDate();

  selectedEndMonth: number = new Date().getMonth() + 1; // array begin by 0
  selectedEndDay: number = new Date().getUTCDate();

  minRecurringStartDate: Date = new Date();
  maxRecurringStartDate: Date;
  minRecurringEndDate: Date = new Date();
  maxRecurringEndDate: Date;

  recurringForm: FormGroup;

  isWarningForInvalidWeekDay: boolean = false;
  isWarningForInvalidMonthDay: boolean = false;
  isWarningForInvalidYearDay: boolean = false;
  isInvalidRecurringDate: boolean = false;
  isInPastWarning: boolean = false;
  data: RecurringDialogModel = new RecurringDialogModel();
  recurringModel: RecurringModel = new RecurringModel();

  yearlyValidation: DateTimeValidation = new DateTimeValidation();
  monthlyValidation: DateTimeValidation = new DateTimeValidation();
  weeklyValidation: WeeklyValidation = new WeeklyValidation();
  recurringDateValidation: RecurringDateValidation = new RecurringDateValidation();

  constructor(
    public dialogRef: MatDialogRef<BroadcastMessagesDetailComponent>,
    private formBuilderSvc: FormBuilderService,
    private translateAdapterSvc: TranslateAdapterService,
    private fb: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public recurringData: RecurringModel
  ) {}

  ngOnInit(): void {
    this.buildRecurringDialogModel();
    this.recurringType.push(
      RecurrenceType.Week,
      RecurrenceType.Month,
      RecurrenceType.Year
    );
    this.buildMonths();
    this.buildDayOfMonth();
    this.createFormBuilderDefinition();
    this.daysOfTheWeek.push(...DAY_OF_WEEK);
    this.buildValidationMessage();
  }

  onSubmit(): void {
    if (!this.isValidDataToSubmit()) {
      return;
    }

    this.dialogRef.close({
      recurringDataDialog: this.data,
      recurringModel: this.recurringModel
    });
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  buildMonths(): void {
    this.months = Array(MAX_MONTH)
      .fill(0)
      .map((i, idx) => getMonth(idx + 1));
  }

  buildDayOfMonth(): void {
    let day = 1;
    while (this.daysOfMonth.length < this.maxDayOfMonth) {
      this.daysOfMonth.push(day);
      day++;
    }
  }

  onDateChange(
    type: 'recurringStartDate' | 'recurringEndDate',
    event: MatDatepickerInputEvent<Date>
  ): void {
    switch (type) {
      case 'recurringStartDate':
        this.minRecurringEndDate = event.value;
        break;
      case 'recurringEndDate':
        this.maxRecurringStartDate = event.value;
        break;
      default:
        return;
    }
  }

  isWeekly(): boolean {
    return this.data.type === RecurrenceType.Week;
  }

  isMonthly(): boolean {
    return this.data.type === RecurrenceType.Month;
  }

  isYearly(): boolean {
    return this.data.type === RecurrenceType.Year;
  }

  getDaysInMonth(year: number, month: number): number {
    if (!year || !month) {
      return;
    }
    const overMaxDayOfMonth = this.maxDayOfMonth + 1;

    return (
      overMaxDayOfMonth - new Date(year, month - 1, overMaxDayOfMonth).getDate()
    );
  }

  isValidDataToSubmit(): boolean {
    const isValidRecurringDate = this.isValidRecurringDate();
    let isValidRecurringType: boolean = false;

    switch (this.data.type) {
      case RecurrenceType.Week:
        isValidRecurringType = this.isValidWeekly();
        break;
      case RecurrenceType.Month:
        isValidRecurringType = this.isValidMonthly();
        break;
      case RecurrenceType.Year:
        isValidRecurringType = this.isValidYearly();
        break;
      default:
        break;
    }

    if (!isValidRecurringType || !isValidRecurringDate) {
      return false;
    }

    this.recurringModel.type = this.data.type;

    return true;
  }

  // Recurring Logic
  isValidWeekly(): boolean {
    const startWeekDay = DAY_OF_WEEK.indexOf(this.data.startWeekDay);
    const endWeekDay = DAY_OF_WEEK.indexOf(this.data.endWeekDay);

    // Start week day must <= end week day
    const fromDay = this.getDay(startWeekDay);
    const toDay = this.getDay(endWeekDay);

    const fromDate = DateTimeUtil.buildDateTime(
      this.getDateFromDay(fromDay, this.data.recurringStartDate),
      DateTimeUtil.createDefaultDateFromTime(this.data.startAt)
    );

    const toDate = DateTimeUtil.buildDateTime(
      this.getDateFromDay(toDay, this.data.recurringStartDate),
      DateTimeUtil.createDefaultDateFromTime(this.data.endAt)
    );

    this.weeklyValidation.time.isError =
      fromDay === toDay && fromDate >= toDate;

    this.weeklyValidation.weekday.isError = startWeekDay > endWeekDay;

    if (
      this.weeklyValidation.weekday.isError ||
      this.weeklyValidation.time.isError
    ) {
      return false;
    }

    this.recurringModel.fromDate = fromDate;
    this.recurringModel.toDate = toDate;

    return true;
  }

  getDay(dayIndex: number): number {
    // If day is Sunday. Move it to week begin. Else move normal day next 1 day.
    if (dayIndex === MAX_WEEK_DAYS) {
      dayIndex = 0;
    } else {
      dayIndex += 1;
    }

    return dayIndex;
  }

  // Get date from day
  getDateFromDay(targetDay: number, recurringDate: Date): Date {
    const recurringDay = recurringDate.getDay();
    let distance = 0;

    if (recurringDay > targetDay) {
      // If from day or to day is future of recurring day. Then, pick same day in next week
      distance = MAX_WEEK_DAYS - recurringDay + targetDay + 1;
    } else {
      // Else just simple get distance day
      distance = targetDay - recurringDay;
    }
    const targetDate = Utils.cloneDeep(recurringDate);

    return new Date(targetDate.setDate(targetDate.getDate() + distance));
  }

  isValidRecurringDate(): boolean {
    const startDateCompare = DateTimeUtil.buildDateTime(
      this.data.recurringStartDate,
      DateTimeUtil.createDefaultDateFromTime(this.data.recurringStartTime)
    );

    const endDateCompare = DateTimeUtil.buildDateTime(
      this.data.recurringEndDate,
      DateTimeUtil.createDefaultDateFromTime(this.data.recurringEndTime)
    );

    this.recurringDateValidation.inPast.isError = DateTimeUtil.IsDateInPast(
      endDateCompare
    );

    this.recurringDateValidation.time.isError =
      !this.recurringDateValidation.inPast.isError &&
      DateTimeUtil.compareDate(startDateCompare, endDateCompare, false) === 0 &&
      DateTimeUtil.compareTimeOfDate(startDateCompare, endDateCompare) >= 0;

    this.recurringDateValidation.date.isError =
      !this.recurringDateValidation.inPast.isError &&
      DateTimeUtil.compareDate(startDateCompare, endDateCompare, false) > 0;

    if (
      this.recurringDateValidation.time.isError ||
      this.recurringDateValidation.date.isError ||
      this.recurringDateValidation.inPast.isError ||
      !this.data.type
    ) {
      return false;
    }

    this.recurringModel.recurringStartDate = startDateCompare;
    this.recurringModel.recurringEndDate = endDateCompare;

    return true;
  }

  isValidMonthly(): boolean {
    const startDateCompare = DateTimeUtil.buildDateTime(
      new Date(0, 0, this.data.startDay),
      DateTimeUtil.createDefaultDateFromTime(this.data.startAt)
    );

    const endDateCompare = DateTimeUtil.buildDateTime(
      new Date(0, 0, this.data.endDay),
      DateTimeUtil.createDefaultDateFromTime(this.data.endAt)
    );

    this.monthlyValidation.time.isError =
      this.data.startDay === this.data.endDay &&
      DateTimeUtil.compareTimeOfDate(startDateCompare, endDateCompare) >= 0;

    this.monthlyValidation.date.isError = this.data.startDay > this.data.endDay;

    if (
      this.monthlyValidation.time.isError ||
      this.monthlyValidation.date.isError
    ) {
      return false;
    }

    this.recurringModel.fromDate = startDateCompare;
    this.recurringModel.toDate = endDateCompare;

    return true;
  }

  isValidYearly(): boolean {
    const startDate = DateTimeUtil.buildDateTime(
      new Date(this.selectedYear, this.data.startMonth - 1, this.data.startDay),
      DateTimeUtil.createDefaultDateFromTime(this.data.startAt)
    );

    const endDate = DateTimeUtil.buildDateTime(
      new Date(this.selectedYear, this.data.endMonth - 1, this.data.endDay),
      DateTimeUtil.createDefaultDateFromTime(this.data.endAt)
    );

    this.yearlyValidation.time.isError =
      this.data.startMonth === this.data.endMonth &&
      this.data.startDay === this.data.endDay &&
      DateTimeUtil.compareTimeOfDate(startDate, endDate) >= 0;

    this.yearlyValidation.date.isError =
      this.data.startMonth === this.data.endMonth &&
      this.data.startDay > this.data.endDay;

    this.yearlyValidation.month.isError =
      this.data.startMonth > this.data.endMonth;

    if (
      this.yearlyValidation.time.isError ||
      this.yearlyValidation.date.isError ||
      this.yearlyValidation.month.isError
    ) {
      return false;
    }

    this.recurringModel.fromDate = startDate;
    this.recurringModel.toDate = endDate;

    return true;
  }
  private resetRecurringDate(): void {
    this.minRecurringStartDate = new Date();
    this.maxRecurringStartDate = null;
  }

  private setAllValidation(): void {
    this.yearlyValidation.resetError();
    this.monthlyValidation.resetError();
    this.weeklyValidation.resetError();
    this.recurringDateValidation.resetError();
  }

  private buildRecurringDialogModel(): void {
    if (this.recurringData.type === RecurrenceType.None) {
      return;
    }
    const recurringDialogModel = new RecurringDialogModel({
      ...this.recurringData
    });
    recurringDialogModel.recurringStartTime = DateTimeUtil.getTimeFromDate(
      this.recurringData.recurringStartDate
    );
    recurringDialogModel.recurringEndTime = DateTimeUtil.getTimeFromDate(
      this.recurringData.recurringEndDate
    );
    recurringDialogModel.startAt = DateTimeUtil.getTimeFromDate(
      this.recurringData.fromDate
    );
    recurringDialogModel.endAt = DateTimeUtil.getTimeFromDate(
      this.recurringData.toDate
    );

    switch (this.recurringData.type) {
      case RecurrenceType.Week:
        recurringDialogModel.startWeekDay = DateTimeUtil.getWeekDay(
          this.recurringData.fromDate
        );
        recurringDialogModel.endWeekDay = DateTimeUtil.getWeekDay(
          this.recurringData.toDate
        );
        break;
      case RecurrenceType.Month:
        recurringDialogModel.startDay = this.recurringData.fromDate.getDate();
        recurringDialogModel.endDay = this.recurringData.toDate.getDate();
        break;
      case RecurrenceType.Year:
        recurringDialogModel.startMonth =
          this.recurringData.fromDate.getMonth() + 1;
        recurringDialogModel.endMonth =
          this.recurringData.toDate.getMonth() + 1;
        recurringDialogModel.startDay = this.recurringData.fromDate.getDate();
        recurringDialogModel.endDay = this.recurringData.toDate.getDate();
        break;
      default:
        return;
    }

    this.data = recurringDialogModel;
  }

  private buildValidationMessage(): void {
    // Recurring Date Case
    this.recurringDateValidation.date.message = this.translateAdapterSvc.getValueImmediately(
      'Broadcast_Message_Page.Recurring_dialog.validation_message.Recurring.Date'
    );
    this.recurringDateValidation.inPast.message = this.translateAdapterSvc.getValueImmediately(
      'Broadcast_Message_Page.Recurring_dialog.validation_message.Recurring.InPast'
    );
    this.recurringDateValidation.time.message = this.translateAdapterSvc.getValueImmediately(
      'Broadcast_Message_Page.Recurring_dialog.validation_message.Recurring.Time'
    );

    // Yearly Case
    this.yearlyValidation.month.message = this.translateAdapterSvc.getValueImmediately(
      'Broadcast_Message_Page.Recurring_dialog.validation_message.Yearly.Month'
    );
    this.yearlyValidation.date.message = this.translateAdapterSvc.getValueImmediately(
      'Broadcast_Message_Page.Recurring_dialog.validation_message.Yearly.Date'
    );
    this.yearlyValidation.time.message = this.translateAdapterSvc.getValueImmediately(
      'Broadcast_Message_Page.Recurring_dialog.validation_message.Yearly.Time'
    );

    // Monthly Case
    this.monthlyValidation.date.message = this.translateAdapterSvc.getValueImmediately(
      'Broadcast_Message_Page.Recurring_dialog.validation_message.Monthly.Date'
    );
    this.monthlyValidation.time.message = this.translateAdapterSvc.getValueImmediately(
      'Broadcast_Message_Page.Recurring_dialog.validation_message.Monthly.Time'
    );

    // Weekly Case
    this.weeklyValidation.weekday.message = this.translateAdapterSvc.getValueImmediately(
      'Broadcast_Message_Page.Recurring_dialog.validation_message.Weekly.Weekday'
    );
    this.weeklyValidation.time.message = this.translateAdapterSvc.getValueImmediately(
      'Broadcast_Message_Page.Recurring_dialog.validation_message.Weekly.Time'
    );
  }

  private createFormBuilderDefinition(): void {
    this.recurringForm = this.fb.group({
      selectedYear: [''],
      recurringStartDate: ['', Validators.required],
      recurringStartTime: ['', Validators.required],
      recurringEndDate: ['', Validators.required],
      recurringEndTime: ['', Validators.required],
      startAt: ['', Validators.required],
      endAt: ['', Validators.required],
      startWeekDay: [
        '',
        requiredIfValidator((control) => this.data.type === RecurrenceType.Week)
      ],
      endWeekDay: [
        '',
        requiredIfValidator((control) => this.data.type === RecurrenceType.Week)
      ],
      startDay: [
        '',
        requiredIfValidator(
          (control) =>
            this.data.type === RecurrenceType.Month ||
            this.data.type === RecurrenceType.Year
        )
      ],
      endDay: [
        '',
        requiredIfValidator(
          (control) =>
            this.data.type === RecurrenceType.Month ||
            this.data.type === RecurrenceType.Year
        )
      ],
      startMonth: [
        '',
        requiredIfValidator((control) => this.data.type === RecurrenceType.Year)
      ],
      endMonth: [
        '',
        requiredIfValidator((control) => this.data.type === RecurrenceType.Year)
      ]
    });
  }
}
