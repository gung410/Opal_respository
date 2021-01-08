export interface IUserTrackingEventRequest {
  eventName: string;
  time: Date;
  sessionId: string;
  userId: string;
  payload: any;
}

export interface ITrackingRequest {
  itemId: string;
  itemType: string;
  isLike?: boolean;
  sharedUsers?: string[];
}
