import { BaseComponent, ComponentType, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { OpalPopupService } from './../../services/popup/popup.service';
import { PopupContainerComponent } from './../../services/popup/popup-container/popup-container.component';

interface IDataFilterInput {
  data: unknown;
  settings: unknown;
}

@Component({
  selector: 'data-filter',
  templateUrl: './data-filter.component.html'
})
export class DataFilterComponent extends BaseComponent {
  @Input() public component: ComponentType<unknown>;
  @Input() public applyFn: (data: unknown) => void;
  @Input() public settings: unknown;

  public inputs: IDataFilterInput = {
    data: null,
    settings: null
  };

  public get hasDataFilter(): boolean {
    return Utils.checkAllPropertiesHasValue(this.inputs.data);
  }
  constructor(moduleFacadeService: ModuleFacadeService, private opalPopupService: OpalPopupService) {
    super(moduleFacadeService);
  }

  public openFilterPopup(): void {
    this.opalPopupService.openPopupRef(PopupContainerComponent, {
      component: this.component,
      actionFn: {
        applyFn: data => this.onApplyFilter(data)
      },
      inputs: {
        data: Utils.cloneDeep(this.inputs.data),
        settings: this.settings
      }
    });
  }

  public onClear(): void {
    Utils.setValueAllProperties(this.inputs.data, undefined);
    this.applyFn(this.inputs.data);
  }

  private onApplyFilter(data: unknown): void {
    this.inputs.data = Utils.cloneDeep(data);
    this.applyFn(data);
  }
}
