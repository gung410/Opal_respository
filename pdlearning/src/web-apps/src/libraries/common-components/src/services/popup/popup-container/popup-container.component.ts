import { BaseComponent, ComponentType, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ComponentFactoryResolver, ViewChild } from '@angular/core';

import { PopupContentDirective } from './popup-content.directive';
import { PopupRef } from '@progress/kendo-angular-popup';

@Component({
  selector: 'popup-container',
  templateUrl: './popup-container.component.html'
})
export class PopupContainerComponent extends BaseComponent {
  @ViewChild(PopupContentDirective, { static: true }) public popupContent: PopupContentDirective;
  public get popupRef(): PopupRef {
    return this._popupRef;
  }

  public set popupRef(v: PopupRef) {
    if (v != null) {
      this._popupRef = v;
      this.dynamicLoadingComponent();
    }
  }
  public component: ComponentType<unknown>;
  public actionFn: unknown;
  public inputs: unknown;
  private _popupRef: PopupRef;
  constructor(moduleFacadeService: ModuleFacadeService, private componentFactoryResolver: ComponentFactoryResolver) {
    super(moduleFacadeService);
  }

  public onClose(): void {
    this.popupRef.close();
  }

  private dynamicLoadingComponent(): void {
    const componentFactory = this.componentFactoryResolver.resolveComponentFactory(this.component);

    const viewContainerRef = this.popupContent.viewContainerRef;
    viewContainerRef.clear();

    // tslint:disable-next-line:no-any
    const componentRef: any = viewContainerRef.createComponent(componentFactory);
    componentRef.instance.popupRef = this.popupRef;
    componentRef.instance.actionFn = this.actionFn;
    componentRef.instance.inputs = this.inputs;
  }
}
