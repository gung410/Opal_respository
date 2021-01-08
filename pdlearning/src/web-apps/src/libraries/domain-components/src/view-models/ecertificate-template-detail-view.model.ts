import {
  ECertificateLayoutModel,
  ECertificateSupportedField,
  ECertificateTemplateModel,
  ECertificateTemplateStatus,
  IECertificateLayoutParam,
  IECertificateTemplateParam
} from '@opal20/domain-api';

import { Utils } from '@opal20/infrastructure';

export class ECertificateTemplateDetailViewModel {
  public eCertificateTemplateData: ECertificateTemplateModel = new ECertificateTemplateModel();
  public originECertificateTemplateData: ECertificateTemplateModel = new ECertificateTemplateModel();
  public eCertificateLayoutDic: Dictionary<ECertificateLayoutModel> = {};
  public selectedLayout: ECertificateLayoutModel;

  public get allowedThumbnailExtensions(): string[] {
    return ECertificateTemplateModel.allowedThumbnailExtensions;
  }

  constructor(eCertificateTemplate?: ECertificateTemplateModel, public eCertificateLayouts: ECertificateLayoutModel[] = []) {
    if (eCertificateTemplate) {
      this.updateSessionData(eCertificateTemplate);
    }

    this.eCertificateLayoutDic = Utils.toDictionary(eCertificateLayouts, p => p.id);
    this.buildTemplateParams();
  }

  public get layoutParams(): IECertificateLayoutParam[] {
    return this.selectedLayout.params;
  }

  public getTemplateParam(key: ECertificateSupportedField): string {
    const param = this.eCertificateTemplateData.params.find(x => x.key === key);

    if (param == null) {
      this.eCertificateTemplateData.params.push({
        key: key,
        value: ''
      });
    }

    return param != null ? param.value : '';
  }

  public setTemplateParam(key: ECertificateSupportedField, value: string): void {
    const param = this.eCertificateTemplateData.params.find(x => x.key === key);
    param.value = value;
  }

  public get title(): string {
    return this.eCertificateTemplateData.title;
  }
  public set title(title: string) {
    this.eCertificateTemplateData.title = title;
  }

  public get params(): IECertificateTemplateParam[] {
    return this.eCertificateTemplateData.params;
  }

  public get status(): ECertificateTemplateStatus {
    return this.eCertificateTemplateData.status;
  }
  public set status(status: ECertificateTemplateStatus) {
    this.eCertificateTemplateData.status = status;
  }

  public get createBy(): string {
    return this.eCertificateTemplateData.createdBy;
  }

  public set createBy(createBy: string) {
    this.eCertificateTemplateData.createdBy = createBy;
  }

  public get eCertificateLayoutId(): string {
    return this.eCertificateTemplateData.eCertificateLayoutId;
  }
  public set eCertificateLayoutId(eCertificateLayoutId: string) {
    this.eCertificateTemplateData.eCertificateLayoutId = eCertificateLayoutId;
    this.buildTemplateParams();
  }

  public updateSessionData(eCertificateTemplate: ECertificateTemplateModel): void {
    this.originECertificateTemplateData = Utils.cloneDeep(eCertificateTemplate);
    this.eCertificateTemplateData = Utils.cloneDeep(eCertificateTemplate);
  }

  public dataHasChanged(): boolean {
    return Utils.isDifferent(this.originECertificateTemplateData, this.eCertificateTemplateData);
  }

  private buildTemplateParams(): void {
    this.selectedLayout = this.eCertificateTemplateData.eCertificateLayoutId
      ? this.eCertificateLayoutDic[this.eCertificateTemplateData.eCertificateLayoutId]
      : null;
  }
}
