import { CourseContentItemType } from '../models/course-content-item.model';

export interface IChangeContentOrderRequest {
  id: string;
  direction: MovementDirection;
  type: CourseContentItemType;
  courseId: string;
  classRunId?: string;
}

export enum MovementDirection {
  Up = 'Up',
  Down = 'Down'
}
