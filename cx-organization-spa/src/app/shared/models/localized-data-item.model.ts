import { LocalizedDataField } from './localized-data-field.model';

export class LocalizedDataItem {
  constructor(
    public id: number,
    public languageCode: string,
    public fields: LocalizedDataField[]
  ) {}
}
