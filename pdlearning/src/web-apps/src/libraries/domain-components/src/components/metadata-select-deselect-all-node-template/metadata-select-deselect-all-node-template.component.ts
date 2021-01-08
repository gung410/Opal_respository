import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ViewChild } from '@angular/core';

import { BehaviorSubject } from 'rxjs';
import { MetadataTagModel } from '@opal20/domain-api';
import { NodeTemplateDirective } from '@progress/kendo-angular-treeview';
import { SelectAllNodeTemplateModel } from './../../models/select-all-node-template.model';

@Component({
  selector: 'metadata-select-deselect-all-node-template',
  templateUrl: './metadata-select-deselect-all-node-template.component.html'
})
export class MetadataSelectDeselectAllNodeTemplateComponent extends BaseComponent {
  @ViewChild(NodeTemplateDirective, { static: true }) public nodeTemplateDirective: NodeTemplateDirective;
  public selectionSubject: BehaviorSubject<SelectAllNodeTemplateModel> = new BehaviorSubject(null);
  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public showOption(dataItem: MetadataTagModel): boolean {
    return dataItem != null && dataItem.childs != null && dataItem.childs.length > 0;
  }

  public onClickOption(dataItem: MetadataTagModel, checkAll: boolean): void {
    this.selectionSubject.next({
      checkAll: checkAll,
      dataItem: dataItem
    });
  }
}
