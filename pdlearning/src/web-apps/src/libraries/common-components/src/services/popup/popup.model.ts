import { ComponentType } from '@opal20/infrastructure';
import { PopupRef } from '@progress/kendo-angular-popup';

export class PopupModel {
  public component: ComponentType<unknown>;
  public popupRef?: PopupRef;
}
