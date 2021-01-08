import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CopyrightLicenseType, IAttributionElement } from '@opal20/domain-api';

import { DigitalContentDetailMode } from '../../models/digital-content-detail-mode.model';
import { ICopyrightFormModelSelectItem } from '../../models/copyright-form.model';
import { Validators } from '@angular/forms';

@Component({
  selector: 'copyright-attribution-element-form',
  templateUrl: './copyright-attribution-element-form.component.html'
})
export class CopyrightAttributionElementFormComponent extends BaseFormComponent {
  public DigitalContentDetailMode: typeof DigitalContentDetailMode = DigitalContentDetailMode;

  @Input()
  public mode: DigitalContentDetailMode = DigitalContentDetailMode.Edit;
  @Input() public data: Partial<IAttributionElement> = {};

  @Input() public set licenseTypeSelectItems(values: ICopyrightFormModelSelectItem<CopyrightLicenseType>[]) {
    this._licenseTypeSelectItems = values;
    this.licenseTypeItemsDic = Utils.toDictionary(this._licenseTypeSelectItems, p => p.value);
  }
  public get licenseTypeSelectItems(): ICopyrightFormModelSelectItem<CopyrightLicenseType>[] {
    return this._licenseTypeSelectItems;
  }

  public licenseTypeItemsDic: Dictionary<ICopyrightFormModelSelectItem<CopyrightLicenseType>> = {};

  @Output('dataChange') public dataChangeEvent: EventEmitter<Partial<IAttributionElement>> = new EventEmitter();

  public emitDataChangeDebounce: () => void = Utils.debounce(() => {
    this.dataChangeEvent.emit(this.data);
  }, 300);

  private _licenseTypeSelectItems: ICopyrightFormModelSelectItem<CopyrightLicenseType>[] = [];

  constructor(moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        title: {
          defaultValue: this.data.title,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        },
        author: {
          defaultValue: this.data.author,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        },
        source: {
          defaultValue: this.data.source,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        },
        licenseType: {
          defaultValue: this.data.licenseType,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        }
      }
    };
  }
}
