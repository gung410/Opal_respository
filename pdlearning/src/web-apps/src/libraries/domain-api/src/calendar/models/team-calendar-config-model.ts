import { TeamCalendarViewType } from '../enums/team-calendar-view-type-enum';

export class TeamCalendarConfigModel {
  public rowHeight: number;
  public columns: [];
  public views: [];
  public listWidth: number;

  public getConfig(adjustHeight: number): object {
    return {
      tooltip: { visible: true },
      rowHeight: 50,
      columns: [
        {
          field: 'title',
          title: 'Name',
          sortable: true,
          editable: false
        }
      ],
      views: [
        {
          type: TeamCalendarViewType.CurrentMonth,
          title: '1 Month'
        },
        {
          type: TeamCalendarViewType.ThreeMonths,
          title: '3 Months'
        },
        {
          type: TeamCalendarViewType.Year,
          title: 'Year'
        }
      ],
      listWidth: 200,
      height: 980,
      editable: {
        update: false,
        resize: false,
        dragPercentComplete: false,
        create: false,
        confirmation: false,
        move: false,
        destroy: false,
        dependencyCreate: false,
        dependencyDestroy: false,
        reorder: false
      },
      dataBound: function(): void {
        /**Because we config a model-specific, so need to adjust the header's calendar.*/
        const height: number = this.timeline.view()._slots.length * adjustHeight;
        this.list.header.find('tr').height(height + 'em');
        this.list._adjustHeight();
      },
      dataSource: {
        schema: {
          model: {
            id: 'id',
            fields: {
              id: { from: 'id', type: 'string' },
              parentId: { from: 'parentId', type: 'string', defaultValue: null, nullable: true, validation: { required: true } },
              start: { from: 'start', type: 'date' },
              end: { from: 'end', type: 'date' },
              title: { from: 'title', defaultValue: '', type: 'string' },
              summary: { from: 'summary', type: 'boolean' },
              expanded: { from: 'expanded', type: 'boolean', defaultValue: false }
            }
          }
        }
      }
    };
  }
}
