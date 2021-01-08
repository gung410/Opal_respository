import { EventSource } from '../enums/event-source';
export const eventResources = [
  {
    name: 'background-color',
    field: 'source',
    valueField: 'value',
    colorField: 'color',
    data: [
      { value: EventSource.SelfCreated, color: '#D8DCE6' },
      { value: EventSource.Community, color: '#F2F3B5' },
      { value: EventSource.CourseSession, color: ' #C9F3B5' },
      { value: EventSource.ExternalPDO, color: ' #C9F3B5' },
      // { value: EventSource.BlockOutDate', color: ' #C9F3B5' },
      { value: EventSource.Webinar, color: ' #C9F3B5' },
      { value: EventSource.CourseAssignment, color: ' #C9F3B5' },
      { value: EventSource.StandaloneForm, color: ' #C9F3B5' },
      { value: EventSource.LNA, color: ' #C9F3B5' }
    ]
  }
];
