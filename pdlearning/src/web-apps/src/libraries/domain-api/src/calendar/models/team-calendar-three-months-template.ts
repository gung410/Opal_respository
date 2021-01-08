import '@progress/kendo-ui/js/kendo.gantt';

import { TeamCalendarSlotModel } from './team-calendar-slot-model';

// tslint:disable-next-line:no-any
declare var kendo: any;

export class ThreeMonthsGanttTemplate {
  public create(start: Date, end: Date): void {
    return kendo.ui.GanttView.extend({
      name: 'three-months-gantt-template',

      options: {
        weekHeaderTemplate: kendo.template(
          "<span class=''>#=kendo.toString(start, 'dd/MM')# - #=kendo.toString(kendo.date.addDays(end, -1), 'dd/MM')#<span>"
        )
      },

      range(range: unknown): void {
        this.start = new Date(start);
        this.end = new Date(end);
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

      /** Create a column (slot) in header have information from Monday to Sunday (7 days)
       * Every column equal 1 span.*/
      _createSlots(): Array<TeamCalendarSlotModel> {
        const slots: Array<TeamCalendarSlotModel> = new Array<TeamCalendarSlotModel>();
        slots.push(
          this._generateSlots(date => {
            date.setDate(date.getDate() + 7);
          }, 1)
        );
        return slots;
      },

      _layout(): Array<unknown> {
        const rows: Array<unknown> = new Array<unknown>();
        const options = this.options;

        rows.push(this._slotHeaders(this._slots[0], kendo.template(options.weekHeaderTemplate)));

        return rows;
      }
    });
  }
}
