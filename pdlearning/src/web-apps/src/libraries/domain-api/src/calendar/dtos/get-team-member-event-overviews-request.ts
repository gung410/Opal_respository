import { IGetParams } from '@opal20/infrastructure';

export class GetTeamMemberEventOverviewsRequest implements IGetParams {
  [param: string]: string | number | boolean | IGetParams | string[] | boolean[] | number[];
  public rangeStart: string;
  public rangeEnd: string;
}
