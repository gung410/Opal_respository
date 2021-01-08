import '@progress/kendo-ui/js/kendo.gantt';

import { TeamCalendarSlotModel } from './team-calendar-slot-model';

// tslint:disable-next-line:no-any
declare var kendo: any;

export class QuarterlyGanttTemplate {
  public create(year: number): void {
    return kendo.ui.GanttView.extend({
      name: 'quarterly-gantt-template',

      options: {
        yearHeaderTemplate: kendo.template("#=kendo.toString(start, 'yyyy')#"),
        quarterHeaderTemplate: kendo.template("# return ['Q1', 'Q2', 'Q3', 'Q4'][start.getMonth() / 3] #"),
        monthHeaderTemplate: kendo.template("<span class=''>#=kendo.toString(start, 'MMM')#<span>")
      },

      range(range: unknown): void {
        this.start = new Date(`01/01/${year}`);
        this.end = new Date(`01/01/${year + 1}`);
      },

      _generateSlots(incrementCallback: (date: Date) => void, span: number): Array<TeamCalendarSlotModel> {
        const slots: Array<TeamCalendarSlotModel> = new Array<TeamCalendarSlotModel>();
        let slotStart = new Date(this.start);
        let slotEnd;

        while (slotStart < this.end) {
          slotEnd = new Date(slotStart);
          incrementCallback(slotEnd);

          slots.push({ start: slotStart, end: slotEnd, span: span });

          slotStart = slotEnd;
        }

        return slots;
      },

      /** Create header has 3 row:
       * Row 1: Year: only column (slot) equal 12 span
       * Row 2: Quarterly: every column equal 3 span
       * Row 3: Monthly: every column equal 1 span*/
      _createSlots(): Array<TeamCalendarSlotModel> {
        const slots: Array<TeamCalendarSlotModel> = new Array<TeamCalendarSlotModel>();

        slots.push(
          this._generateSlots(date => {
            date.setFullYear(date.getFullYear() + 1);
          }, 12)
        );
        slots.push(
          this._generateSlots(date => {
            date.setMonth(date.getMonth() + 3);
          }, 3)
        );
        slots.push(
          this._generateSlots(date => {
            date.setMonth(date.getMonth() + 1);
          }, 1)
        );

        return slots;
      },

      _layout(): Array<unknown> {
        const rows: Array<unknown> = new Array<unknown>();
        const options = this.options;

        rows.push(this._slotHeaders(this._slots[0], kendo.template(options.yearHeaderTemplate)));
        rows.push(this._slotHeaders(this._slots[1], kendo.template(options.quarterHeaderTemplate)));
        rows.push(this._slotHeaders(this._slots[2], kendo.template(options.monthHeaderTemplate)));

        return rows;
      }
    });
  }
}
