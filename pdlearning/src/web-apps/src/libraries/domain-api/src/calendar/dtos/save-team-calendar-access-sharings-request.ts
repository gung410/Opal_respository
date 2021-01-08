import { ShareCalendarActionsEnum } from '../enums/share-calendar-actions-enum';

export class SaveTeamCalendarAccessSharingsRequest {
  public action: ShareCalendarActionsEnum;
  public userIds: string[];
}
