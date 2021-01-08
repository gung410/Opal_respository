import { IGetParams } from '@opal20/infrastructure';
// TODO: Rename to GetAllPersonalEventsRequest
export class GetAllEventsRequest implements IGetParams {
  [param: string]: string | number | boolean | IGetParams | string[] | boolean[] | number[];
  public offsetPoint: string;
  public numberMonthOffset: number;

  constructor() {
    this.offsetPoint = null;
    this.numberMonthOffset = 7;
  }
}
