import { IGetParams } from '@opal20/infrastructure';

export class GetCalendarAccessSharingRequest implements IGetParams {
  [param: string]: string | number | boolean | IGetParams | string[] | boolean[] | number[];
  public skipCount?: number;
  public maxResultCount?: number;
}
