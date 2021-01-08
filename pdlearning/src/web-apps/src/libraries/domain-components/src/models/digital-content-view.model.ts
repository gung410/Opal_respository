import { DigitalContent, IDigitalContent, IPublicUserInfo, MetadataTagGroupCode, ResourceMetadataModel } from '@opal20/domain-api';

export interface IDigitalContentViewModel extends IDigitalContent {
  type: string;
  owner: IPublicUserInfo;
  archivedByUser?: IPublicUserInfo | undefined;
}

// @dynamic
export class DigitalContentViewModel extends DigitalContent {
  public type: string;
  public owner: IPublicUserInfo;
  public archivedByUser?: IPublicUserInfo;

  public static createFromModel(
    digitalContent: DigitalContent,
    digitalContentMetadata: ResourceMetadataModel | null,
    digitalContentOwner: IPublicUserInfo,
    digitalContentArchivedBtUser?: IPublicUserInfo
  ): DigitalContentViewModel {
    return new DigitalContentViewModel({
      ...digitalContent,
      type: digitalContentMetadata != null ? digitalContentMetadata.getMetaData(MetadataTagGroupCode.PDO_TYPES, x => x.displayText) : null,
      owner: digitalContentOwner,
      archivedByUser: digitalContentArchivedBtUser
    });
  }

  constructor(data?: IDigitalContentViewModel) {
    super(data);
    if (data != null) {
      this.type = data.type;
      this.owner = data.owner;
      this.archivedByUser = data.archivedByUser;
    }
  }
}
