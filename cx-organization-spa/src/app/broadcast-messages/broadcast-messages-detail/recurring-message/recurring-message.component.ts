import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild
} from '@angular/core';
import { OnDestroy } from '@angular/core';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { Dictionary } from 'app-utilities/utils';
import { DayRepetition } from 'app/broadcast-messages/constant/day-repetition.enum';
import { MonthRepetition } from 'app/broadcast-messages/constant/month-repetition.enum';
import { MonthRecurringSelection } from 'app/broadcast-messages/models/month-recurring-selection.model';
import {
  findIndexCommon,
  NUMBER_OF_REMOVED_ITEM_DEFAULT
} from 'app/shared/constants/common.const';
import { RecurrenceType } from 'app/shared/constants/recurrence-type.enum';
import { fromEvent, Subscription } from 'rxjs';

@Component({
  selector: 'recurring-message',
  templateUrl: './recurring-message.component.html',
  styleUrls: ['./recurring-message.component.scss']
})
export class RecurringMessageComponent
  implements OnInit, AfterViewInit, OnDestroy {
  get recurringTypeOption(): RecurrenceType {
    return this._recurringTypeOption;
  }
  @Input() set recurringTypeOption(recurringTypeOption: RecurrenceType) {
    if (recurringTypeOption == null) {
      return;
    }

    this._recurringTypeOption = recurringTypeOption;
    this.recurringTypeOptionChange.emit(recurringTypeOption);
  }
  @Output()
  recurringTypeOptionChange: EventEmitter<RecurrenceType> = new EventEmitter<RecurrenceType>();

  get dayRepetitions(): DayRepetition[] {
    return this._dayRepetitions;
  }
  @Input() set dayRepetitions(dayRepetitions: DayRepetition[]) {
    if (dayRepetitions == null) {
      return;
    }

    this._dayRepetitions = dayRepetitions;
    this.dayRepetitionsChange.emit(dayRepetitions);
  }
  @Output()
  dayRepetitionsChange: EventEmitter<DayRepetition[]> = new EventEmitter<
    DayRepetition[]
  >();

  get monthRepetition(): MonthRepetition {
    return this._monthRepetition;
  }
  @Input() set monthRepetition(monthRepetition: MonthRepetition) {
    if (monthRepetition == null) {
      return;
    }

    this._monthRepetition = monthRepetition;
    this.monthRepetitionChange.emit(monthRepetition);
  }
  @Output()
  monthRepetitionChange: EventEmitter<MonthRepetition> = new EventEmitter<MonthRepetition>();

  get numberOfRepetition(): number {
    return this._numberOfRepetition;
  }
  @Input() set numberOfRepetition(numberOfRepetition: number) {
    if (numberOfRepetition == null) {
      return;
    }

    this._numberOfRepetition = numberOfRepetition;
    this.numberOfRepetitionChange.emit(numberOfRepetition);
  }

  @Output()
  numberOfRepetitionChange: EventEmitter<number> = new EventEmitter<number>();

  @Input() isDisabledControls: boolean = false;
  @Input() set targetDate(targetDate: Date) {
    if (!targetDate) {
      return;
    }

    if (typeof targetDate === 'string') {
      targetDate = new Date(targetDate);
    }

    this._targetDate = targetDate;
    this.updateMonthRepetitions();
  }

  @ViewChild('repetitionInput', { static: false })
  repetitionInputRef: ElementRef;

  monthRepetitionItems: MonthRecurringSelection[];

  recurringType: string[] = [];
  dayRepetitionItems: DayRepetition[] = [
    DayRepetition.Sunday,
    DayRepetition.Monday,
    DayRepetition.Tuesday,
    DayRepetition.Wednesday,
    DayRepetition.Thursday,
    DayRepetition.Friday,
    DayRepetition.Saturday
  ];
  dayRepetitionsDic: Dictionary<string> = {};

  private _targetDate: Date = new Date();
  private _recurringTypeOption: RecurrenceType = RecurrenceType.Week;
  private _dayRepetitions: DayRepetition[];
  private _monthRepetition: MonthRepetition;
  private _numberOfRepetition: number;
  private _subscription: Subscription = new Subscription();

  constructor() {
    this.recurringType.push(RecurrenceType.Week, RecurrenceType.Month);
    this.buildMonthRepetitionItems();
    this.buildDayRepetitionsDic();
  }

  ngOnInit(): void {}

  ngAfterViewInit(): void {
    this._subscription.add(
      fromEvent(this.repetitionInputRef.nativeElement, 'keypress').subscribe(
        (event: KeyboardEvent) => {
          if (
            (event as any).target.value.length >= 2 ||
            (event.key === '0' && (event as any).target.value.length !== 1)
          ) {
            event.preventDefault();
          }
        }
      )
    );
  }

  ngOnDestroy(): void {
    this._subscription.unsubscribe();
  }

  onDayClicked(day: DayRepetition): void {
    const dayIndex = this.dayRepetitions.findIndex(
      (selectedDay) => selectedDay === day
    );

    if (dayIndex === findIndexCommon.notFound) {
      this.dayRepetitions.push(day);
      this.dayRepetitionsChange.emit(this.dayRepetitions);

      return;
    }

    if (this.dayRepetitions.length === 1) {
      return;
    }

    this.dayRepetitions.splice(dayIndex, NUMBER_OF_REMOVED_ITEM_DEFAULT);
    this.dayRepetitionsChange.emit(this.dayRepetitions);
  }

  getDayToString(dayValue: DayRepetition): string {
    return this.dayRepetitionsDic[dayValue];
  }

  handlingInput(value: number): number {
    const minNumber = 1;
    const maxNumber = 99;
    if (value < minNumber || value > maxNumber) {
      return 1;
    } else {
      return value;
    }
  }

  onRepetitionChange(value: number): void {
    let numberValue = Number(value) === 0 ? 1 : Number(value);
    numberValue = this.handlingInput(numberValue);
    this.numberOfRepetition = numberValue;
    this.repetitionInputRef.nativeElement.value = numberValue.toString();
  }

  private getOrderOfDayInMonthLabel(targetDate: Date): string {
    return DateTimeUtil.getOrderOfDayInMonthText(targetDate);
  }

  private buildDayRepetitionsDic(): void {
    this.dayRepetitionsDic[DayRepetition.Sunday] = 'S';
    this.dayRepetitionsDic[DayRepetition.Monday] = 'M';
    this.dayRepetitionsDic[DayRepetition.Tuesday] = 'T';
    this.dayRepetitionsDic[DayRepetition.Wednesday] = 'W';
    this.dayRepetitionsDic[DayRepetition.Thursday] = 'T';
    this.dayRepetitionsDic[DayRepetition.Friday] = 'F';
    this.dayRepetitionsDic[DayRepetition.Saturday] = 'S';
  }

  private buildMonthRepetitionItems(): void {
    this.monthRepetitionItems = [
      {
        monthRepetition: MonthRepetition.OnDay,
        description: `Monthly on day ${this._targetDate.getDate()}`
      },
      {
        monthRepetition: MonthRepetition.InOrder,
        description: `Monthly on the ${this.getOrderOfDayInMonthLabel(
          this._targetDate
        )} ${DateTimeUtil.getWeekDay(this._targetDate)}`
      }
    ];
  }

  private updateMonthRepetitions(): void {
    this.monthRepetitionItems[0].description = `Monthly on day ${this._targetDate.getDate()}`;
    this.monthRepetitionItems[1].description = `Monthly on the ${this.getOrderOfDayInMonthLabel(
      this._targetDate
    )} ${DateTimeUtil.getWeekDay(this._targetDate)}`;
  }
}
