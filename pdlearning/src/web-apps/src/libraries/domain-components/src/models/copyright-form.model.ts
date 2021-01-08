import {
  CopyrightCommonTermsOfUse,
  CopyrightLicenseTerritory,
  CopyrightLicenseType,
  CopyrightOwnership,
  IAttributionElement,
  ICopyright,
  copyrightLicenseTerritoryDisplay,
  copyrightLicenseTypeDisplay,
  copyrightOwnershipDisplay
} from '@opal20/domain-api';

import { Utils } from '@opal20/infrastructure';

export class CopyrightFormModel {
  public static DEPENDENT_DATA = {
    ownershipToShowLicenseType: [
      CopyrightOwnership.MoeOwnedWithLicensedMaterial,
      CopyrightOwnership.MoeCoOwnedWithLicensedMaterial,
      CopyrightOwnership.LicensedMaterials
    ],
    ownershipToShowLicenseTerritory: [CopyrightOwnership.MoeCoOwned, CopyrightOwnership.MoeOwned],
    licenseTypeToShowLicenseTerritory: [CopyrightLicenseType.Perpetual, CopyrightLicenseType.SubscribedForLimitedPeriod],
    licenseTypeToShowStartAndExpireDate: [CopyrightLicenseType.SubscribedForLimitedPeriod],
    licenseTypeToShowCopyrightOwnerAndAcknowledgement: [CopyrightLicenseType.FreeToUse, CopyrightLicenseType.CreativeCommons],
    licenseTypeToShowTermOfUse: [CopyrightLicenseType.CreativeCommons],
    licenseTerritoryToShowLicensePermissions: [
      CopyrightLicenseTerritory.Singapore,
      CopyrightLicenseTerritory.SingaporeAndHongKong,
      CopyrightLicenseTerritory.Worldwide
    ]
  };

  public get ownership(): CopyrightOwnership | undefined {
    return this._ownership;
  }
  public set ownership(v: CopyrightOwnership | undefined) {
    this._ownership = v;
    if (this.forcedExpiredDateNotApplicable) {
      this.expiryDateNotApplicableChecked = true;
    }
  }

  public get licenseType(): CopyrightLicenseType | undefined {
    return this._licenseType;
  }
  public set licenseType(v: CopyrightLicenseType | undefined) {
    if (
      (this._licenseType !== CopyrightLicenseType.CreativeCommons && v === CopyrightLicenseType.CreativeCommons) ||
      (this._licenseType === CopyrightLicenseType.CreativeCommons && v !== CopyrightLicenseType.CreativeCommons)
    ) {
      this.termsOfUse = undefined;
    }
    this._licenseType = v;
    if (this.forcedStartDateNotApplicable) {
      this.startDateNotApplicableChecked = true;
    }
    if (this.forcedExpiredDateNotApplicable) {
      this.expiryDateNotApplicableChecked = true;
    }
  }

  public get forcedStartDateNotApplicable(): boolean {
    return this.licenseType === CopyrightLicenseType.FreeToUse;
  }
  public get forcedExpiredDateNotApplicable(): boolean {
    return (
      this.licenseType === CopyrightLicenseType.Perpetual ||
      this.licenseType === CopyrightLicenseType.FreeToUse ||
      this.ownership === CopyrightOwnership.MoeOwned
    );
  }
  public get expiryDateNotApplicableChecked(): boolean {
    return this._expiryDateNotApplicableChecked;
  }
  public set expiryDateNotApplicableChecked(v: boolean) {
    if (this._expiryDateNotApplicableChecked === v) {
      return;
    }
    this._expiryDateNotApplicableChecked = v;
    if (v) {
      this.expiredDate = false;
    } else {
      this.expiredDate = undefined;
    }
  }
  public get startDateNotApplicableChecked(): boolean {
    return this._startDateNotApplicableChecked;
  }
  public set startDateNotApplicableChecked(v: boolean) {
    if (this._startDateNotApplicableChecked === v) {
      return;
    }
    this._startDateNotApplicableChecked = v;
    if (v) {
      this.startDate = false;
    } else {
      this.startDate = undefined;
    }
  }
  public get startDate(): Date | undefined | false {
    return this._startDate;
  }
  public set startDate(v: Date | undefined | false) {
    this._startDate = v;
    if (v instanceof Date && this.expiredDate instanceof Date && this.expiredDate <= v) {
      this.expiredDate = null;
    }
  }
  public termsOfUse: string | CopyrightCommonTermsOfUse | undefined;
  public expiredDate: Date | undefined | false;
  public licenseTerritory: CopyrightLicenseTerritory | undefined;
  public publisher: string | undefined;
  public isAllowReusable: boolean = false;
  public isAllowDownload: boolean = false;
  public isAllowModification: boolean = false;
  public acknowledgementAndCredit: string | undefined;
  public attributionElements: Partial<IAttributionElement>[] = [];
  public remarks: string | undefined;
  public get licenseDuration(): number | undefined {
    if (this.startDate instanceof Date && this.expiredDate instanceof Date) {
      return Math.round((this.expiredDate.getTime() - this.startDate.getTime()) / (3600 * 1000 * 24));
    }
    return undefined;
  }

  public ownershipSelectItems: ICopyrightFormModelSelectItem<CopyrightOwnership>[] = [];
  public ownershipItemsDic: Dictionary<ICopyrightFormModelSelectItem<CopyrightOwnership>> = {};
  public licenseTypeSelectItems: ICopyrightFormModelSelectItem<CopyrightLicenseType>[] = [];
  public licenseTypeItemsDic: Dictionary<ICopyrightFormModelSelectItem<CopyrightLicenseType>> = {};
  public commonTermsOfUseSelectItems: ICopyrightFormModelSelectItem<string>[] = [];
  public commonTermsOfUseItemsDic: Dictionary<ICopyrightFormModelSelectItem<string>> = {};
  public licenseTerritoryUseSelectItems: ICopyrightFormModelSelectItem<CopyrightLicenseTerritory>[] = [];
  public licenseTerritoryUseItemsDic: Dictionary<ICopyrightFormModelSelectItem<CopyrightLicenseTerritory>> = {};
  private _startDate: Date | undefined | false;
  private _ownership: CopyrightOwnership | undefined;

  private _licenseType: CopyrightLicenseType | undefined;
  private _expiryDateNotApplicableChecked: boolean = false;
  private _startDateNotApplicableChecked: boolean = false;

  /*************** Dynamic show/hide properties ***************/

  public get canShowLicenseType(): boolean {
    return CopyrightFormModel.DEPENDENT_DATA.ownershipToShowLicenseType.includes(this.ownership);
  }

  public get canShowStartAndExpiryDate(): boolean {
    return CopyrightFormModel.DEPENDENT_DATA.licenseTypeToShowStartAndExpireDate.includes(this.licenseType);
  }

  public get canShowLicenseTerritory(): boolean {
    return (
      CopyrightFormModel.DEPENDENT_DATA.ownershipToShowLicenseTerritory.includes(this.ownership) ||
      CopyrightFormModel.DEPENDENT_DATA.licenseTypeToShowLicenseTerritory.includes(this.licenseType)
    );
  }

  public get canShowCommonTermsOfUse(): boolean {
    return CopyrightFormModel.DEPENDENT_DATA.licenseTypeToShowTermOfUse.includes(this.licenseType);
  }

  public get canShowLicensePermission(): boolean {
    return CopyrightFormModel.DEPENDENT_DATA.licenseTerritoryToShowLicensePermissions.includes(this.licenseTerritory);
  }

  public get canSelectLicensePermission(): boolean {
    return this.canShowLicensePermission && this.isAllowDownload;
  }

  public get canShowCopyrightOwnerAndAcknowledgement(): boolean {
    return (
      CopyrightFormModel.DEPENDENT_DATA.licenseTypeToShowCopyrightOwnerAndAcknowledgement.includes(this.licenseType) ||
      !!this.licenseTerritory
    );
  }

  public get canShowRemarks(): boolean {
    return !!this.publisher;
  }

  /*************** End Dynamic show/hide properties ***************/

  public static getOwnershipSelectItems(): ICopyrightFormModelSelectItem<CopyrightOwnership>[] {
    const result = Object.keys(CopyrightOwnership).map(_ => {
      return <ICopyrightFormModelSelectItem<CopyrightOwnership>>{
        value: _,
        display: copyrightOwnershipDisplay(<CopyrightOwnership>_)
      };
    });
    return result;
  }
  public static getLicenseTypeItems(): ICopyrightFormModelSelectItem<CopyrightLicenseType>[] {
    const result = Object.keys(CopyrightLicenseType).map(_ => {
      return <ICopyrightFormModelSelectItem<CopyrightLicenseType>>{
        value: _,
        display: copyrightLicenseTypeDisplay(<CopyrightLicenseType>_)
      };
    });
    return result;
  }
  public static getCommonTermsOfUseItems(): ICopyrightFormModelSelectItem<string>[] {
    const result = Object.keys(CopyrightCommonTermsOfUse).map(_ => {
      return <ICopyrightFormModelSelectItem<string>>{
        value: CopyrightCommonTermsOfUse[_],
        display: CopyrightCommonTermsOfUse[_]
      };
    });
    return result;
  }
  public static getLicenseTerritoryUseItems(): ICopyrightFormModelSelectItem<CopyrightLicenseTerritory>[] {
    const result = Object.keys(CopyrightLicenseTerritory).map(_ => {
      return <ICopyrightFormModelSelectItem<CopyrightLicenseTerritory>>{
        value: _,
        display: copyrightLicenseTerritoryDisplay(<CopyrightLicenseTerritory>_)
      };
    });
    return result;
  }

  public static isAttributionElementItemValid(data: Partial<IAttributionElement>): boolean {
    if (data.title != null && data.author != null && data.source != null && data.licenseType != null) {
      return true;
    }
    return false;
  }
  constructor(data?: Partial<ICopyright>) {
    if (data != null) {
      this.setCopyrightData(data);
    }

    this.ownershipSelectItems = CopyrightFormModel.getOwnershipSelectItems();
    this.ownershipItemsDic = Utils.toDictionary(this.ownershipSelectItems, p => p.value);
    this.licenseTypeSelectItems = CopyrightFormModel.getLicenseTypeItems();
    this.licenseTypeItemsDic = Utils.toDictionary(this.licenseTypeSelectItems, p => p.value);
    this.commonTermsOfUseSelectItems = CopyrightFormModel.getCommonTermsOfUseItems();
    this.commonTermsOfUseItemsDic = Utils.toDictionary(this.commonTermsOfUseSelectItems, p => p.value);
    this.licenseTerritoryUseSelectItems = CopyrightFormModel.getLicenseTerritoryUseItems();
    this.licenseTerritoryUseItemsDic = Utils.toDictionary(this.licenseTerritoryUseSelectItems, p => p.value);
  }

  public setCopyrightData(data: Partial<ICopyright>): void {
    this.ownership = data.ownership;
    this._licenseType = data.licenseType;
    this.termsOfUse = data.termsOfUse;
    this.licenseTerritory = data.licenseTerritory;
    this.publisher = data.publisher;
    this.isAllowReusable = data.isAllowReusable != null ? data.isAllowReusable : false;
    this.isAllowDownload = data.isAllowDownload != null ? data.isAllowDownload : false;
    this.isAllowModification = data.isAllowModification != null ? data.isAllowModification : false;
    this.acknowledgementAndCredit = data.acknowledgementAndCredit;
    this.remarks = data.remarks;
    this.startDate = data.startDate ? new Date(data.startDate) : false;
    this.expiredDate = data.expiredDate ? new Date(data.expiredDate) : false;
    this._expiryDateNotApplicableChecked = this.expiredDate === false;
    this._startDateNotApplicableChecked = this.startDate === false;
    this.attributionElements = data.attributionElements != null ? Utils.cloneDeep(data.attributionElements) : [];
  }

  public getCopyrightData(): ICopyright {
    return {
      ownership: this.ownership,
      licenseType: this.licenseType,
      termsOfUse: this.termsOfUse,
      startDate: this.startDate === false ? undefined : this.startDate,
      expiredDate: this.expiredDate === false ? undefined : this.expiredDate,
      licenseTerritory: this.licenseTerritory,
      publisher: this.publisher,
      isAllowReusable: this.isAllowReusable,
      isAllowDownload: this.isAllowDownload,
      isAllowModification: this.isAllowModification,
      acknowledgementAndCredit: this.acknowledgementAndCredit,
      remarks: this.remarks,
      attributionElements: Utils.cloneDeep(this.attributionElements.map(p => <IAttributionElement>p))
    };
  }

  public isInvalid(): boolean {
    return (
      this.ownership == null ||
      this.licenseType == null ||
      this.termsOfUse == null ||
      this.startDate == null ||
      this.expiredDate == null ||
      this.attributionElements.findIndex(p => !CopyrightFormModel.isAttributionElementItemValid(p)) >= 0
    );
  }

  public addNewAttributionElements(): void {
    this.attributionElements = Utils.clone(this.attributionElements, _ => {
      _.push({});
    });
  }

  public removeAttributionElement(index: number): void {
    this.attributionElements = Utils.clone(this.attributionElements, _ => {
      _.splice(index, 1);
    });
  }
}

export interface ICopyrightFormModelSelectItem<T> {
  value: T;
  display: string;
}
