import { IGetParams } from '@opal20/infrastructure';

export class GetTeamMemberEventsRequest implements IGetParams {
  [param: string]: string | number | boolean | IGetParams | string[] | boolean[] | number[];
  public rangeStart: string;
  public rangeEnd: string;
  public memberId: string;
}
