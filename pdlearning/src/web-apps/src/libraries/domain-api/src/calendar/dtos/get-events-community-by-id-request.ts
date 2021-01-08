import { IGetParams } from '@opal20/infrastructure';
export class GetEventsByCommunityIdRequest implements IGetParams {
  [param: string]: string | number | boolean | IGetParams | string[] | boolean[] | number[];
  public communityId: string;
  public offsetPoint: string;
  public numberMonthOffset: number;

  constructor(communityId: string) {
    this.communityId = communityId;
    this.offsetPoint = null;
    this.numberMonthOffset = 7;
  }
}
