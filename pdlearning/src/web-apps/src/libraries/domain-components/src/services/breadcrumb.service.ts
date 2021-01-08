import { BreadcrumbItem } from '../models/breadcrumb-item.model';
import { Injectable } from '@angular/core';
import { RouterPageInput } from '../models/router-info.model';

@Injectable()
export class BreadcrumbService {
  public loadBreadcrumbTab(
    currentRoute: RouterPageInput<unknown, unknown, unknown> | null,
    routeBreadcumbMapping: Dictionary<BreadcrumbItem>
  ): BreadcrumbItem[] {
    if (currentRoute == null) {
      return [];
    }

    const breadCrumbItems: BreadcrumbItem[] = [];
    this._loadBreadcrumbTabRecursively(currentRoute, routeBreadcumbMapping, breadCrumbItems);
    return breadCrumbItems;
  }

  private _loadBreadcrumbTabRecursively(
    currentRoute: RouterPageInput<unknown, unknown, unknown>,
    routeBreadcumbMapping: Dictionary<BreadcrumbItem>,
    breadCrumbItems: BreadcrumbItem[]
  ): void {
    breadCrumbItems.unshift(
      routeBreadcumbMapping[currentRoute.path]
        ? routeBreadcumbMapping[currentRoute.path]
        : {
            text: currentRoute.path
          }
    );
    if (currentRoute.parent != null) {
      this._loadBreadcrumbTabRecursively(currentRoute.parent, routeBreadcumbMapping, breadCrumbItems);
    }
  }
}
