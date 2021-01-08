import { ComponentFactoryResolver, Directive, EventEmitter, Input, OnDestroy, OnInit, Output, ViewContainerRef } from '@angular/core';

import { MetadataSelectDeselectAllNodeTemplateComponent } from '../components/metadata-select-deselect-all-node-template/metadata-select-deselect-all-node-template.component';
import { MetadataTagModel } from '@opal20/domain-api';
import { Subscription } from 'rxjs';
import { TreeViewComponent } from '@progress/kendo-angular-treeview';

@Directive({
  selector: 'kendo-treeview[metadataCustomSelectDeselectAll]'
})
export class KendoTreeviewMetadataCustomSelectDeselectAllDirective implements OnInit, OnDestroy {
  @Input() public checkedKeys: string[] = [];
  @Output() public checkedKeysChange: EventEmitter<string[]> = new EventEmitter<string[]>();
  private subs: Subscription[] = [];
  constructor(
    protected treeView: TreeViewComponent,
    public viewContainerRef: ViewContainerRef,
    private componentFactoryResolver: ComponentFactoryResolver
  ) {}

  public ngOnInit(): void {
    this.loadNodeTemplate();
  }

  public ngOnDestroy(): void {
    this.subs.forEach(sub => sub.unsubscribe());
  }

  private loadNodeTemplate(): void {
    const componentFactory = this.componentFactoryResolver.resolveComponentFactory(MetadataSelectDeselectAllNodeTemplateComponent);

    // tslint:disable-next-line:no-any
    const componentRef: any = this.viewContainerRef.createComponent(componentFactory);
    const instance = componentRef.instance as MetadataSelectDeselectAllNodeTemplateComponent;
    this.treeView.nodeTemplate = instance.nodeTemplateDirective;

    this.subs.push(
      instance.selectionSubject.subscribe(data => {
        if (data != null) {
          if (data.checkAll) {
            this.checkedKeys = MetadataTagModel.selectAllTreeItemIds(data.dataItem, this.checkedKeys);
          } else {
            this.checkedKeys = MetadataTagModel.deselectAllTreeItemIds(data.dataItem, this.checkedKeys);
          }
          this.checkedKeysChange.emit(this.checkedKeys.slice());
        }
      })
    );
  }
}
