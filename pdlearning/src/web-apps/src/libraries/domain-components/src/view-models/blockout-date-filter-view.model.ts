import { MetadataTagGroupCode, MetadataTagModel } from '@opal20/domain-api';
import { Observable, of } from 'rxjs';

import { BlockoutDateFilterModel } from './../models/blockout-date-filter.model';
import { Utils } from '@opal20/infrastructure';

export class BlockoutDateFilterViewModel {
  public static readonly create = create;
  public metadataTagsDicByGroupCode: Dictionary<MetadataTagModel[]> = {};
  public serviceSchemeSelectItems: MetadataTagModel[] = [];

  private originData: BlockoutDateFilterModel = new BlockoutDateFilterModel();

  constructor(public metadataTags: MetadataTagModel[] = [], public data: BlockoutDateFilterModel = new BlockoutDateFilterModel()) {
    this.setMetadataTagsDicInfo(this.metadataTags);

    this.serviceSchemeSelectItems = Utils.defaultIfNull(
      Utils.rightJoinBy(
        Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.SERVICE_SCHEMES], []),
        Utils.defaultIfNull(this.metadataTagsDicByGroupCode[MetadataTagGroupCode.PDO_TAXONOMY], []),
        x => x.tagId,
        x => x.parentTagId
      ),
      []
    );

    this.originData = Utils.cloneDeep(this.data);
  }

  public dataHasChanged(): boolean {
    return Utils.isDifferent(this.originData, this.data);
  }

  private setMetadataTagsDicInfo(metadataTags: MetadataTagModel[]): void {
    this.metadataTagsDicByGroupCode = Utils.toDictionaryGroupBy(metadataTags.filter(p => p.groupCode != null), p => p.groupCode);
  }
}

function create(
  metadataTags: MetadataTagModel[] = [],
  data: BlockoutDateFilterModel = new BlockoutDateFilterModel()
): Observable<BlockoutDateFilterViewModel> {
  return of(new BlockoutDateFilterViewModel(metadataTags, data));
}
