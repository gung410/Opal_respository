import { SharingType } from './user-sharing-request.dto';

export interface ISaveUserSharing {
  id?: string;
  itemId: string;
  itemType: SharingType;
  usersShared: ISaveUserSharingDetail[];
  createdBy?: string;
}

export interface ISaveUserSharingDetail {
  id?: string;
  userId: string;
  userSharingId?: string;
}
