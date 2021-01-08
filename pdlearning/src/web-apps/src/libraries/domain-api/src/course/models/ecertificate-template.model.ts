import { SystemRoleEnum, UserInfoModel } from './../../share/models/user-info.model';

import { ECertificateSupportedField } from './ecertificate-layout.model';
import { Utils } from '@opal20/infrastructure';
export interface IECertificateTemplateModel {
  id: string;
  title: string;
  eCertificateLayoutId: string;
  params: IECertificateTemplateParam[];
  totalCoursesUsing?: number;
  totalLearnersReceived?: number;
  status?: ECertificateTemplateStatus;
  createdBy?: string;
  hasFullRight?: boolean;
}

export class ECertificateTemplateModel implements IECertificateTemplateModel {
  public static optionalProps: (keyof IECertificateTemplateModel)[] = ['totalCoursesUsing', 'totalLearnersReceived'];
  public static allowedThumbnailExtensions = ['jpeg', 'jpg', 'gif', 'png', 'svg'];

  public id: string;
  public title: string;
  public eCertificateLayoutId: string;
  public params: IECertificateTemplateParam[] = [];
  public totalCoursesUsing?: number;
  public totalLearnersReceived?: number;
  public status?: ECertificateTemplateStatus = ECertificateTemplateStatus.Draft;
  public createdBy?: string;
  public hasFullRight: boolean = false;

  public static canCreateOrModify(user: UserInfoModel): boolean {
    return (user && user.hasAdministratorRoles()) || user.hasRole(SystemRoleEnum.CourseContentCreator);
  }

  constructor(data?: IECertificateTemplateModel) {
    if (data) {
      this.id = data.id;
      this.title = data.title;
      this.eCertificateLayoutId = data.eCertificateLayoutId;
      this.params = data.params ? data.params : [];
      this.status = data.status;
      this.totalCoursesUsing = data.totalCoursesUsing;
      this.totalLearnersReceived = data.totalLearnersReceived;
      this.status = data.status;
      this.createdBy = data.createdBy;
      this.hasFullRight = data.hasFullRight != null ? data.hasFullRight : this.hasFullRight;
    }
  }

  public get paramDict(): Dictionary<IECertificateTemplateParam> {
    return Utils.toDictionary(this.params, p => p.key);
  }

  public canDeleteECertificateTemplate(currentUser: UserInfoModel): boolean {
    return this.hasOwnerPermission(currentUser);
  }

  public hasOwnerPermission(user: UserInfoModel): boolean {
    return this.createdBy === user.id || this.hasFullRight;
  }
}

export interface IECertificateTemplateParam {
  key: ECertificateSupportedField;
  value: string;
}
export class ECertificateTemplateParam implements IECertificateTemplateParam {
  public key: ECertificateSupportedField;
  public value: string;

  public static create(key: ECertificateSupportedField): ECertificateTemplateParam {
    const ecertificateTemplateParam = new ECertificateTemplateParam();
    ecertificateTemplateParam.key = key;
    return ecertificateTemplateParam;
  }

  constructor(data?: IECertificateTemplateParam) {
    if (data) {
      this.key = data.key;
      this.value = data.value;
    }
  }
}

export enum ECertificateTemplateStatus {
  Draft = 'Draft',
  Active = 'Active'
}
