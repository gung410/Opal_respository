import { Utils } from '@opal20/infrastructure';

export interface IECertificateLayoutModel {
  id: string;
  name: string;
  description: string;
  base64PreviewImage: string;
  params: IECertificateLayoutParam[];
}

export enum ECertificateSupportedField {
  FullName = 'FullName',
  CourseName = 'CourseName',
  CompletedDate = 'CompletedDate',
  CompletedDateText = 'CompletedDateText',
  Principal = 'Principal',
  Description = 'Description',
  PrincipalSignature = 'PrincipalSignature',
  PrincipalText = 'PrincipalText',
  Logo = 'Logo',
  Logo2 = 'Logo2',
  Logo3 = 'Logo3',
  Background = 'Background'
}

export class ECertificateLayoutModel implements IECertificateLayoutModel {
  public id: string;
  public name: string;
  public description: string;
  public base64PreviewImage: string;
  public params: IECertificateLayoutParam[];
  public paramDict: Dictionary<IECertificateLayoutParam> = {};

  constructor(data?: IECertificateLayoutModel) {
    if (data) {
      this.id = data.id;
      this.name = data.name;
      this.description = data.description;
      this.base64PreviewImage = data.base64PreviewImage;
      this.params = data.params;
      this.paramDict = Utils.toDictionary(this.params, p => p.key);
    }
  }
}

export interface IECertificateLayoutParam {
  key: ECertificateSupportedField;
  title: string;
  description: string;
  type: ECertificateParamType;
  isAutoPopulated: boolean;
}

export enum ECertificateParamType {
  Text = 'Text',
  Image = 'Image'
}
