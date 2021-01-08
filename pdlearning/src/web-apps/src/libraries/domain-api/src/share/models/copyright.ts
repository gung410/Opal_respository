export interface ICopyright {
  ownership: CopyrightOwnership;
  licenseType: CopyrightLicenseType;
  termsOfUse: string | CopyrightCommonTermsOfUse;
  startDate: Date | undefined;
  expiredDate: Date | undefined;
  licenseTerritory: CopyrightLicenseTerritory;
  publisher: string | undefined;
  isAllowReusable: boolean;
  isAllowDownload: boolean;
  isAllowModification: boolean;
  acknowledgementAndCredit: string | undefined;
  remarks: string | undefined;
  attributionElements: AttributionElement[];
}

export interface IAttributionElement {
  id: string;
  courseId: string;
  title: string;
  author: string;
  source: string;
  licenseType: CopyrightLicenseType;
}

export class AttributionElement implements IAttributionElement {
  public id: string;
  public courseId: string;
  public title: string;
  public author: string;
  public source: string;
  public licenseType: CopyrightLicenseType;

  constructor(data?: IAttributionElement) {
    if (data == null) {
      return;
    }

    this.id = data.id;
    this.courseId = data.courseId;
    this.title = data.title;
    this.author = data.author;
    this.source = data.source;
    this.licenseType = data.licenseType;
  }
}

export enum CopyrightOwnership {
  MoeOwned = 'MoeOwned',
  MoeOwnedWithLicensedMaterial = 'MoeOwnedWithLicensedMaterial',
  MoeCoOwned = 'MoeCoOwned',
  MoeCoOwnedWithLicensedMaterial = 'MoeCoOwnedWithLicensedMaterial',
  LicensedMaterials = 'LicensedMaterials'
}

export function copyrightOwnershipDisplay(value: CopyrightOwnership): string {
  switch (value) {
    case CopyrightOwnership.MoeOwned:
      return 'MOE-Owned';
    case CopyrightOwnership.MoeOwnedWithLicensedMaterial:
      return 'MOE-Owned with Licensed Material';
    case CopyrightOwnership.MoeCoOwned:
      return 'MOE Co-owned';
    case CopyrightOwnership.MoeCoOwnedWithLicensedMaterial:
      return 'MOE Co-owned with Licensed Material';
    case CopyrightOwnership.LicensedMaterials:
      return 'Licensed Material';
  }
}

export enum CopyrightLicenseType {
  Perpetual = 'Perpetual',
  SubscribedForLimitedPeriod = 'SubscribedForLimitedPeriod',
  FreeToUse = 'FreeToUse',
  CreativeCommons = 'CreativeCommons'
}

export function copyrightLicenseTypeDisplay(value: CopyrightLicenseType): string {
  switch (value) {
    case CopyrightLicenseType.Perpetual:
      return 'Perpetual License';
    case CopyrightLicenseType.SubscribedForLimitedPeriod:
      return 'Subscribed for a Limited Period';
    case CopyrightLicenseType.FreeToUse:
      return 'Free to Use';
    case CopyrightLicenseType.CreativeCommons:
      return 'Creative Commons License';
  }
}

export enum CopyrightCommonTermsOfUse {
  Attribution = 'Modify for any purpose with acknowledgement (Attribution CC BY)',
  AttributionShareAlike = 'Modify for any purpose with acknowledgement and license new works under identical terms (Attribution ShareAlike CC BY-SA)',
  AttributionNonCommercial = 'Modify for non-commercial purposes with acknowledgement (Attribution-NonCommercial CC BY-NC)',
  // tslint:disable-next-line:max-line-length
  AttributionNonCommercialShareAlike = 'Modify for non-commercial purposes with acknowledgement and license new works under identical terms (Attribution-NonCommercial-ShareAlike CC BY-NC-SA)',
  AttributionNoDerivs = 'Reuse, without modification, for any purpose with acknowledgement (Attribution-NoDerivs CC BY-ND)',
  AttributionNonCommercialNoDerivs = 'Reuse, without modification, for non-commercial purposes (Attribution-NonCommercial-NoDerivs CC BY-NC-ND)'
}

export enum CopyrightLicenseTerritory {
  Singapore = 'Singapore',
  SingaporeAndHongKong = 'SingaporeAndHongKong',
  Worldwide = 'Worldwide'
}

export function copyrightLicenseTerritoryDisplay(value: CopyrightLicenseTerritory): string {
  switch (value) {
    case CopyrightLicenseTerritory.Singapore:
      return 'Singapore';
    case CopyrightLicenseTerritory.SingaporeAndHongKong:
      return 'Singapore & Hong Kong';
    case CopyrightLicenseTerritory.Worldwide:
      return 'Worldwide';
  }
}
