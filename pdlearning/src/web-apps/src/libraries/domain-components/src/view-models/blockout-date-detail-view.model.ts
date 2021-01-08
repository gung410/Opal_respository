import { BlockoutDateModel, BlockoutDateStatus, MetadataTagGroupCode, MetadataTagModel } from '@opal20/domain-api';
import { DateUtils, Utils } from '@opal20/infrastructure';

export class BlockoutDateDetailViewModel {
  public originalData: BlockoutDateModel = new BlockoutDateModel();
  public data: BlockoutDateModel = new BlockoutDateModel();

  public serviceSchemeSelectItems: MetadataTagModel[] = [];
  public metadataTagsDicByGroupCode: Dictionary<MetadataTagModel[]> = {};
  public metadataTagsDic: Dictionary<MetadataTagModel> = {};

  private _startDateTime: Date = new Date();
  private _endDateTime: Date = new Date();

  constructor(blockoutDate?: BlockoutDateModel, public metadataTags: MetadataTagModel[] = []) {
    if (blockoutDate != null) {
      this.updateBlockoutDateData(blockoutDate);

      if (blockoutDate.id != null) {
        this.startDateTime = new Date(blockoutDate.validYear, blockoutDate.startMonth - 1, blockoutDate.startDay);
        this.endDateTime = new Date(blockoutDate.validYear, blockoutDate.endMonth - 1, blockoutDate.endDay);
      } else {
        if (DateUtils.getNow().getFullYear() === blockoutDate.validYear) {
          this.startDateTime = DateUtils.endOfDay();
          this.endDateTime = DateUtils.endOfYear();
        } else {
          this.startDateTime = DateUtils.startOfYear(blockoutDate.validYear);
          this.endDateTime = DateUtils.endOfYear(blockoutDate.validYear);
        }
      }
    }

    this.setMetadataTagsDicInfo(metadataTags);
    this.serviceSchemeSelectItems = Utils.defaultIfNull(
      Utils.rightJoinBy(
        Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.SERVICE_SCHEMES], []),
        Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TAXONOMY], []),
        x => x.tagId,
        x => x.parentTagId
      ),
      []
    );
  }

  public get title(): string {
    return this.data.title;
  }
  public set title(title: string) {
    this.data.title = title;
  }

  public get description(): string {
    return this.data.description;
  }
  public set description(description: string) {
    this.data.description = description;
  }

  public get status(): BlockoutDateStatus {
    return this.data.status;
  }
  public set status(status: BlockoutDateStatus) {
    this.data.status = status;
  }

  public get startDateTime(): Date {
    return this._startDateTime;
  }
  public set startDateTime(startDateTime: Date) {
    this._startDateTime = startDateTime;
  }

  public get endDateTime(): Date {
    return this._endDateTime;
  }
  public set endDateTime(endDateTime: Date) {
    this._endDateTime = endDateTime;
  }

  public get serviceSchemes(): string[] {
    return this.data.serviceSchemes;
  }
  public set serviceSchemes(serviceSchemes: string[]) {
    this.data.serviceSchemes = serviceSchemes;
  }

  public get validYear(): number {
    return this.data.validYear;
  }
  public set validYear(validYear: number) {
    this.data.validYear = validYear;
  }

  public updateBlockoutDateData(blockoutDate: BlockoutDateModel): void {
    this.originalData = Utils.cloneDeep(blockoutDate);
    this.data = Utils.cloneDeep(blockoutDate);
  }

  public dataHasChanged(): boolean {
    return Utils.isDifferent(this.originalData, this.data);
  }

  public getMinStartDate(): Date {
    if (DateUtils.getNow().getFullYear() === this.validYear) {
      return DateUtils.endOfDay();
    }
    return DateUtils.startOfYearDateOnly(this.validYear);
  }

  public getMaxEndDate(): Date {
    return DateUtils.endOfYear(this.validYear);
  }

  private setMetadataTagsDicInfo(metadataTags: MetadataTagModel[]): void {
    this.metadataTagsDic = Utils.toDictionary(metadataTags, p => p.tagId);
    this.metadataTagsDicByGroupCode = Utils.toDictionaryGroupBy(
      metadataTags.filter(p => p.groupCode != null),
      p => p.groupCode,
      items => Utils.orderBy(items, p => p.displayText)
    );
  }
}
