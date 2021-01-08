import { ECertificateTemplateModel, IECertificateTemplateModel } from '@opal20/domain-api';

import { IGridDataItem } from '@opal20/infrastructure';

export interface IECertificateViewModel extends IECertificateTemplateModel {
  selected: boolean;
}

// @dynamic
export class ECertificateViewModel extends ECertificateTemplateModel implements IGridDataItem {
  public id: string;
  public selected: boolean;
  public static createFromModel(
    eCertificate: ECertificateTemplateModel,
    checkAll: boolean = false,
    selecteds: Dictionary<boolean> = {}
  ): ECertificateViewModel {
    return new ECertificateViewModel({
      ...eCertificate,
      selected: checkAll || selecteds[eCertificate.id]
    });
  }

  constructor(data?: IECertificateViewModel) {
    super(data);
    if (data != null) {
      this.selected = data.selected;
    }
  }
}
