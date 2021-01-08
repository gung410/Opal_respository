import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, TranslationMessage, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output, QueryList, ViewChildren } from '@angular/core';
import { CopyrightFormModel, ICopyrightFormModelSelectItem } from '../../models/copyright-form.model';
import { CopyrightLicenseTerritory, CopyrightLicenseType, CopyrightOwnership, ICopyright } from '@opal20/domain-api';
import {
  copyrightExpiryDateValidator,
  copyrightExpiryDateValidatorType,
  copyrightValidateExpiryDate
} from '../../validators/copyright-expiry-date-validator';
import {
  copyrightStartDateValidator,
  copyrightStartDateValidatorType,
  copyrightValidateStartDate
} from '../../validators/copyright-start-date-validator';
import { ifValidator, requiredIfValidator } from '@opal20/common-components';

import { CopyrightAttributionElementFormComponent } from './copyright-attribution-element-form.component';
import { DigitalContentDetailMode } from '../../models/digital-content-detail-mode.model';
import { PermissionsTermsOfUse } from '../../models/permissions-terms-of-use.model';
import { Validators } from '@angular/forms';

@Component({
  selector: 'copyright-form',
  templateUrl: './copyright-form.component.html'
})
export class CopyrightFormComponent extends BaseFormComponent {
  public DigitalContentDetailMode: typeof DigitalContentDetailMode = DigitalContentDetailMode;
  public licenseTypeSelectItems: ICopyrightFormModelSelectItem<CopyrightLicenseType>[] = [];
  public ownershipSelectItems: ICopyrightFormModelSelectItem<CopyrightOwnership>[] = [];
  public licenseTerritoryUseSelectItems: ICopyrightFormModelSelectItem<CopyrightLicenseTerritory>[] = [];
  public licensePermissions: string[] = [
    PermissionsTermsOfUse.DownloadForPersonal,
    PermissionsTermsOfUse.WithModification,
    PermissionsTermsOfUse.WithoutModification,
    PermissionsTermsOfUse.WithModificationInMOE,
    PermissionsTermsOfUse.WithoutModificationInMOE
  ];
  @Input()
  public mode: DigitalContentDetailMode = DigitalContentDetailMode.Edit;

  public get data(): Partial<ICopyright> | undefined {
    return this._data;
  }
  @Input()
  public set data(v: Partial<ICopyright> | undefined) {
    if (v === undefined) {
      v = {};
    }
    if (!Utils.isDifferent(this._data, v)) {
      return;
    }
    this._data = v;
    this.formData = new CopyrightFormModel(v);
  }
  @ViewChildren(CopyrightAttributionElementFormComponent)
  public attributionElementItemForms: QueryList<CopyrightAttributionElementFormComponent>;

  public formData: CopyrightFormModel = new CopyrightFormModel();

  public copyRightDisplayOrder: Dictionary<number> = {};

  @Output('dataChange') public dataChangeEvent: EventEmitter<Partial<ICopyright>> = new EventEmitter();

  private _data: Partial<ICopyright> | undefined;

  constructor(moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.formData = new CopyrightFormModel(this.data);
    this.licenseTypeSelectItems = this.formData.licenseTypeSelectItems;
    this.ownershipSelectItems = this.formData.ownershipSelectItems;
    this.licenseTerritoryUseSelectItems = this.formData.licenseTerritoryUseSelectItems;
  }

  public initData(): void {
    this.patchInitialFormValue({
      ownership: this.formData.ownership,
      licenseType: this.formData.licenseType,
      termsOfUse: this.formData.termsOfUse,
      startDate: this.formData.startDate,
      expiryDate: this.formData.expiredDate,
      licenseTerritory: this.formData.licenseTerritory,
      publisher: this.formData.publisher,
      isAllowReusable: this.formData.isAllowReusable,
      isAllowDownload: this.formData.isAllowDownload,
      isAllowModification: this.formData.isAllowModification,
      acknowledgementAndCredit: this.formData.acknowledgementAndCredit,
      remarks: this.formData.remarks
    });
  }

  public getCopyrightData(): ICopyright {
    return this.formData.getCopyrightData();
  }

  /*************** On data change handlers ***************/

  public onOwnershipChanged(value: CopyrightOwnership | undefined): void {
    this.resetLicenseType();
    this.resetStartExpiryDate();
    this.resetLicenseTerritory();
    this.resetPublisherAcknowledgement();
    this.resetRemart();
    this.resetTermOfUse();
  }

  public onLicenseTypeChanged(value: string): void {
    this.resetStartExpiryDate();
    this.resetLicenseTerritory();
    this.resetPublisherAcknowledgement();
    this.resetRemart();
    this.resetTermOfUse();
  }

  public onLicenseTerritoryChanged(value: string): void {
    this.resetPublisherAcknowledgement();
    this.resetRemart();
    this.resetTermOfUse();
  }

  /*************** End On data change handlers ***************/

  public disabledStartDate(value: Date): boolean {
    return !copyrightValidateStartDate(value);
  }
  public disabledExpiryDate(value: Date): boolean {
    return !copyrightValidateExpiryDate(value, this.formData.startDate instanceof Date ? this.formData.startDate : undefined);
  }

  public updateCopyrightFormData(data: Partial<ICopyright>): void {
    this._data = data;
    this.formData = new CopyrightFormModel(data);
    this.patchInitialFormValue({
      ownership: this.formData.ownership,
      licenseType: this.formData.licenseType,
      termsOfUse: this.formData.termsOfUse,
      startDate: this.formData.startDate,
      expiryDate: this.formData.expiredDate,
      licenseTerritory: this.formData.licenseTerritory,
      publisher: this.formData.publisher,
      isAllowReusable: this.formData.isAllowReusable,
      isAllowDownload: this.formData.isAllowDownload,
      isAllowModification: this.formData.isAllowModification,
      acknowledgementAndCredit: this.formData.acknowledgementAndCredit,
      remarks: this.formData.remarks
    });
  }

  public onAllowDownloadToggle(data: boolean): void {
    if (data) {
      this.formData.termsOfUse = PermissionsTermsOfUse.DownloadForPersonal;
      return;
    }

    this.resetTermOfUse();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        ownership: {
          defaultValue: this.formData.ownership,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        },
        licenseType: {
          defaultValue: this.formData.licenseType,
          validators: [
            {
              validator: requiredIfValidator(p => this.formData.canShowLicenseType),
              validatorType: 'required'
            }
          ]
        },
        termsOfUse: {
          defaultValue: this.formData.termsOfUse,
          validators: []
        },
        startDate: {
          defaultValue: this.formData.startDate,
          validators: [
            {
              validator: requiredIfValidator(p => this.formData.canShowStartAndExpiryDate)
            },
            {
              validator: ifValidator(p => this.formData.canShowStartAndExpiryDate, () => copyrightStartDateValidator()),
              validatorType: copyrightStartDateValidatorType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'The start date must not be in the past')
            }
          ]
        },
        expiryDate: {
          defaultValue: this.formData.expiredDate,
          validators: [
            {
              validator: requiredIfValidator(p => this.formData.canShowStartAndExpiryDate)
            },
            {
              validator: ifValidator(p => this.formData.canShowStartAndExpiryDate, () => copyrightExpiryDateValidator()),
              validatorType: copyrightExpiryDateValidatorType,
              message: new TranslationMessage(
                this.moduleFacadeService.translator,
                'The expiry date must be greater than today or start date'
              )
            }
          ]
        },
        startDateNotApplicableChecked: {
          defaultValue: this.formData.startDateNotApplicableChecked
        },
        expiryDateNotApplicableChecked: {
          defaultValue: this.formData.expiryDateNotApplicableChecked
        },
        licenseTerritory: {
          defaultValue: this.formData.licenseTerritory,
          validators: [
            {
              validator: requiredIfValidator(p => this.formData.canShowLicenseTerritory),
              validatorType: 'required'
            }
          ]
        },
        publisher: {
          defaultValue: this.formData.publisher,
          validators: [
            {
              validator: requiredIfValidator(p => this.formData.canShowCopyrightOwnerAndAcknowledgement),
              validatorType: 'required'
            }
          ]
        },
        isAllowReusable: {
          defaultValue: this.formData.isAllowReusable,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        },
        isAllowDownload: {
          defaultValue: this.formData.isAllowDownload,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        },
        isAllowModification: {
          defaultValue: this.formData.isAllowModification,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        },
        acknowledgementAndCredit: {
          defaultValue: this.formData.acknowledgementAndCredit,
          validators: []
        },
        remarks: {
          defaultValue: this.formData.remarks,
          validators: []
        }
      }
    };
  }

  protected additionalCanSaveCheck(controls?: string[]): Promise<boolean> {
    return Promise.all(
      this.attributionElementItemForms
        .toArray()
        .reverse()
        .map(p => p.validate())
    ).then(_ => {
      return _.indexOf(false) >= 0 ? false : true;
    });
  }

  private resetLicenseType(): void {
    this.formData.licenseType = undefined;
  }
  private resetLicenseTerritory(): void {
    this.formData.licenseTerritory = undefined;
  }

  private resetTermOfUse(): void {
    this.formData.termsOfUse = undefined;
  }
  private resetStartExpiryDate(): void {
    this.formData.startDateNotApplicableChecked = false;
    this.formData.startDate = undefined;
    this.formData.expiryDateNotApplicableChecked = false;
    this.formData.expiredDate = undefined;
  }
  private resetPublisherAcknowledgement(): void {
    this.formData.publisher = undefined;
    this.formData.acknowledgementAndCredit = undefined;
  }
  private resetRemart(): void {
    this.formData.remarks = undefined;
  }
}
