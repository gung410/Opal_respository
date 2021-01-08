import { BaseComponent, TranslationMessage, Utils } from '@opal20/infrastructure';
import { Component, ContentChild, EventEmitter, Input, Output, TemplateRef } from '@angular/core';

import { OpalListItemTemplateDirective } from './opal-list.directives';

@Component({
  selector: 'opal-list',
  templateUrl: './opal-list.component.html'
})
export class OpalListComponent extends BaseComponent {
  @Input() public items: unknown[] = [];

  @Output('itemsChange') public itemsChangeEvent: EventEmitter<unknown[]> = new EventEmitter<unknown[]>();
  @Output('remove') public removeEvent: EventEmitter<unknown> = new EventEmitter<unknown>();
  @ContentChild(OpalListItemTemplateDirective, { read: TemplateRef, static: false }) public itemTmp: TemplateRef<unknown>;
  @Input() public canRemoveFn?: (item: unknown) => boolean;

  public onRemove(item: unknown): void {
    this.removeItem(item);
  }

  public removeItem(item: unknown): void {
    this.modalService.showConfirmMessage(
      new TranslationMessage(this.moduleFacadeService.globalTranslator, 'Are you sure you want to permanently delete this item?'),
      () => {
        this.items = Utils.clone(this.items, items => {
          Utils.remove(items, p => p === item);
          return items;
        });
        this.removeEvent.emit(item);
        this.itemsChangeEvent.emit(this.items);
      }
    );
  }
}
