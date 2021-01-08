import { CheckboxesFilterModel } from './checkboxes-filter-model';
import { EventSource } from '../enums/event-source';
import { PanelBarItemModel } from '@progress/kendo-angular-layout';

export class PersonalEventFilterModel {
  public myEventFilter: Array<string> = [];
  public communityEventFilter: Array<string> = [];
  public learningEventFilter: Array<string> = [];

  public panelBarItems: Array<PanelBarItemModel> = [];

  constructor() {
    this.panelBarItems = [
      <PanelBarItemModel>{
        title: 'My events',
        content: new CheckboxesFilterModel(this.myEventFilter, [{ text: 'All my events', value: EventSource.SelfCreated }]),
        expanded: true,
        iconClass: 'my-event-icon'
      },
      <PanelBarItemModel>{
        title: "Community's events",
        content: new CheckboxesFilterModel(this.communityEventFilter, [
          { text: 'Regular', value: EventSource.Community },
          { text: 'Webinar', value: EventSource.Webinar }
        ]),
        expanded: false,
        iconClass: 'community-event-icon'
      },
      <PanelBarItemModel>{
        title: 'Learning events',
        content: new CheckboxesFilterModel(this.learningEventFilter, [
          { text: 'Session', value: EventSource.CourseSession },
          { text: 'External PDO', value: EventSource.ExternalPDO },
          // { text: 'Block-out Dates', value: 'BlockOutDate' },
          { text: 'Assignment', value: EventSource.CourseAssignment },
          { text: 'Stand-alone Form', value: EventSource.StandaloneForm },
          { text: 'LNA', value: EventSource.LNA }
        ]),
        expanded: false,
        iconClass: 'learning-event-icon'
      }
    ];
  }

  public isFilterSourcesEmpty(): boolean {
    return this.myEventFilter.length === 0 && this.communityEventFilter.length === 0 && this.learningEventFilter.length === 0;
  }

  public getFilterSources(): Array<string> {
    return this.communityEventFilter.concat(this.learningEventFilter);
  }
}
