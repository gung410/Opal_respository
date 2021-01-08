export class LocalizedData {
  constructor(
    public id: number,
    public languageCode: string,
    public fields: { name: string; localizedText: string }[]
  ) {}
}
