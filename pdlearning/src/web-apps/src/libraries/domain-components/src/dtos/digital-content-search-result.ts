import { IDigitalContent } from '@opal20/domain-api';

export interface IDigitalContentSearchResult {
  totalCount: number;
  items: IDigitalContent[];
}
