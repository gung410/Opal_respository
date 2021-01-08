import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ContentChild, EventEmitter, Input, Output } from '@angular/core';

import { EditModeDirective } from './edit-mode.directive';
import { ViewModeDirective } from './view-mode.directive';

@Component({
  selector: 'editable',
  template: `
    <ng-container *ngTemplateOutlet="currentView"></ng-container>
    <div *ngIf="modeView && editModeTpl != null" style="display: none;">
      <ng-container *ngTemplateOutlet="editModeTpl.tpl"></ng-container>
    </div>
  `
})
export class EditableComponent extends BaseComponent {
  @Input() public modeView: boolean;
  @Output() public changed = new EventEmitter();
  @ContentChild(ViewModeDirective, { static: false }) public viewModeTpl: ViewModeDirective;
  @ContentChild(EditModeDirective, { static: false }) public editModeTpl: EditModeDirective;

  constructor(moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public get currentView(): unknown {
    return this.modeView ? this.viewModeTpl.tpl : this.editModeTpl.tpl;
  }
}
