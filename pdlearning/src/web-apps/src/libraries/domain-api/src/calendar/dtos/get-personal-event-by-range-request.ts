import { IGetParams } from '@opal20/infrastructure';
export class GetPersonalEventByRangeRequest implements IGetParams {
  [param: string]: string | number | boolean | IGetParams | string[] | boolean[] | number[];
  public startAt: string;
  public endAt: string;
}
