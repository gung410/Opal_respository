import { IPagedResultRequestDto } from '../../share/dtos/paged-request.dto';

export interface IGetUserSharingRequest extends IPagedResultRequestDto {
  searchText: string;
  itemType?: SharingType;
}

export enum SharingType {
  Course = 'Course',
  DigitalContent = 'DigitalContent',
  LearningPath = 'LearningPath',
  Community = 'Community'
}
