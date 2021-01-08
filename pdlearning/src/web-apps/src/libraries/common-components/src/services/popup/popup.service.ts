import { ComponentType, ZINDEX_LEVEL_4 } from '@opal20/infrastructure';
import { Injectable, NgZone, Renderer2 } from '@angular/core';
import { PopupRef, PopupService } from '@progress/kendo-angular-popup';

import { OpalPopupSettings } from './popup-configs';

// @dynamic
@Injectable()
export class OpalPopupService {
  public static get defaultPopupConfigs(): OpalPopupSettings {
    return {
      anchorAlign: { horizontal: 'left', vertical: 'top' },
      animate: {
        direction: 'right',
        duration: 100
      },
      maxWidth: '450px'
    };
  }

  public currentPopupsRef: PopupRef[] = [];

  constructor(private _kendoPopupSvc: PopupService, private _ngZone: NgZone, private renderer: Renderer2) {}

  public openPopupRef<T>(component: ComponentType<T>, inputs?: Partial<T>, configs?: OpalPopupSettings): PopupRef {
    return this._ngZone.run(() => {
      const finalConfigs = {
        ...OpalPopupService.defaultPopupConfigs,
        ...configs
      };

      const popupRef = this._kendoPopupSvc.open({
        ...finalConfigs,
        content: component
      });

      this.currentPopupsRef.push(popupRef);

      if (inputs != null) {
        this._assignInputsData(popupRef, { ...inputs, popupRef: popupRef });
      }

      const popupContainerElement = this.getPopupContainerElement(popupRef);

      popupContainerElement.style.display = 'flex';
      popupContainerElement.style.flexDirection = 'column';
      popupContainerElement.style.overflow = 'visible';
      popupContainerElement.style.maxWidth = finalConfigs.maxWidth != null ? finalConfigs.maxWidth : '100vw';
      popupContainerElement.style.width = finalConfigs.maxWidth != null ? finalConfigs.maxWidth : '100vw';
      popupContainerElement.style.zIndex = finalConfigs.zIndex != null ? finalConfigs.zIndex : `${ZINDEX_LEVEL_4}`;
      popupContainerElement.style.maxHeight = '100vh';
      popupContainerElement.style.height = '100vh';

      const popupContentWrapperElement = this.getPopupContentWrapperElement(popupRef);
      popupContentWrapperElement.style.background = '#FFFFFF';
      popupContentWrapperElement.style.width = '100%';
      popupContentWrapperElement.style.height = '100%';
      popupContentWrapperElement.style.borderRadius = '0px';
      popupContentWrapperElement.style.position = 'relative';
      popupContentWrapperElement.style.zIndex = finalConfigs.zIndex != null ? finalConfigs.zIndex : `${ZINDEX_LEVEL_4}`;

      this.appendPopupOverlay(popupContainerElement, popupRef);

      return popupRef;
    });
  }

  public getPopupContainerElement(popupRef: PopupRef): HTMLElement {
    // tslint:disable-next-line:no-any
    return (<any>popupRef).popup.instance.container.nativeElement;
  }

  public getPopupContentWrapperElement(popupRef: PopupRef): HTMLElement {
    // tslint:disable-next-line:no-any
    return (<any>popupRef).popup.instance.contentContainer.nativeElement;
  }

  private _assignInputsData(popupRef: PopupRef, inputs: Object): void {
    const sourceProperties = Object.getOwnPropertyNames(inputs);

    sourceProperties.forEach(prop => {
      // tslint:disable-next-line:no-any
      const propValue = (<any>inputs)[prop];

      if (propValue !== null) {
        popupRef.content.instance[prop] = propValue;
      }
    });
  }

  private appendPopupOverlay(popupContainerElement: HTMLElement, popupRef: PopupRef): void {
    const overlay: HTMLElement = this.renderer.createElement('div');
    overlay.classList.add('popup-container-overlay');

    popupContainerElement.appendChild(overlay);
  }
}
