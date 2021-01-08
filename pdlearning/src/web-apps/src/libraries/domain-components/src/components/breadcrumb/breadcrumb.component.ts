import { Component, Input } from '@angular/core';

import { BreadcrumbItem } from './../../models/breadcrumb-item.model';

@Component({
  selector: 'breadcrumb',
  templateUrl: './breadcrumb.component.html'
})
export class BreadcrumbComponent {
  @Input() public items: BreadcrumbItem[] = [];

  public onNavigateItem(item: BreadcrumbItem, isLast: boolean): void {
    if (!isLast && item.navigationFn != null) {
      item.navigationFn();
    }
  }
}
