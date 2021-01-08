import { IPagedResultRequestDto } from './../../share/dtos/paged-request.dto';
import { MyAssignmentStatus } from '../models/my-assignment.model';

export interface IMyAssignmentRequest extends IPagedResultRequestDto {
  registrationId?: string;
}

export interface IChangeMyAssignmentStatus {
  registrationId: string;
  assignmentId: string;
  status: MyAssignmentStatus;
}
