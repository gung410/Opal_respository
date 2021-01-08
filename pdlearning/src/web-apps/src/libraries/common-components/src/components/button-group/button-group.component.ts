import { BaseComponent, Utils } from '@opal20/infrastructure';
import { Component, ContentChild, Directive, Input, TemplateRef } from '@angular/core';

import { ContextMenuEvent } from '@progress/kendo-angular-menu';
import { ContextMenuItem } from './../../models/context-menu-item.model';
@Directive({ selector: '[buttonTmp]' })
export class ButtonTemplateDirective {
  constructor(public template: TemplateRef<unknown>) {}
}

@Directive({ selector: '[moreButtonTmp]' })
export class MoreButtonTemplateDirective {
  constructor(public template: TemplateRef<unknown>) {}
}

@Component({
  selector: 'button-group',
  templateUrl: './button-group.component.html'
})
export class ButtonGroupComponent extends BaseComponent {
  @ContentChild(ButtonTemplateDirective, { read: TemplateRef, static: false }) public buttonTmp: TemplateRef<unknown>;
  @ContentChild(MoreButtonTemplateDirective, { read: TemplateRef, static: false }) public moreButtonTmp: TemplateRef<unknown>;
  @Input() public set buttons(buttons: Partial<ButtonGroupButton>[]) {
    this._buttons = buttons;
    this._normalButtons = this.buttons.filter(a => !this.canShowInMoreBtn(a));
    const shownInMoreBtn = this.buttons.filter(a => this.canShowInMoreBtn(a));
    this._moreButtonDict = Utils.toDictionary(shownInMoreBtn, p => p.id);
    this._moreItems = shownInMoreBtn.map(
      p =>
        <ContextMenuItem>{
          id: p.id || p.icon || p.displayText,
          text: p.displayText,
          icon: p.icon
        }
    );
  }

  public get buttons(): Partial<ButtonGroupButton>[] {
    return this._buttons;
  }

  public get normalButtons(): Partial<ButtonGroupButton>[] {
    return this._normalButtons.filter(p => this.canShowBtn(p));
  }

  public get menuItems(): ContextMenuItem[] {
    return this._moreItems.filter(p => this.canShowBtn(this._moreButtonDict[p.id]));
  }

  public contentChildrenArray: Array<TemplateRef<unknown>> = [];
  private _moreItems: ContextMenuItem[] = [];
  private _normalButtons: Partial<ButtonGroupButton>[] = [];
  private _moreButtonDict: Dictionary<Partial<ButtonGroupButton>> = {};
  private _buttons: Partial<ButtonGroupButton>[] = [];

  public canShowBtn(button: Partial<ButtonGroupButton>): boolean {
    return !button.shownIfFn || button.shownIfFn();
  }

  public canShowInMoreBtn(button: Partial<ButtonGroupButton>): boolean {
    return button.shownInMoreFn && button.shownInMoreFn();
  }

  public onClick(button: ButtonGroupButton): void {
    if (button.onClickFn) {
      button.onClickFn();
    }
  }

  public onItemSelect(contextMenuEmit: ContextMenuEvent): void {
    const button: Partial<ButtonGroupButton> = this._moreButtonDict[contextMenuEmit.item.id];
    if (button && button.onClickFn) {
      button.onClickFn();
    }
  }
}

export interface ButtonGroupButton {
  id: string;
  displayText: string;
  icon: string;
  type: string;
  onClickFn: () => void;
  isDisabledFn: () => boolean;
  shownIfFn: () => boolean;
  shownInMoreFn: () => boolean;
  isPrimaryFn: () => boolean;
}
