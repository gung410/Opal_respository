import { BaseComponent, KeyCode, Utils } from '@opal20/infrastructure';
import { Component, Input, OnInit } from '@angular/core';

import { INavigationMenuItem } from '../../models/navigation-menu.model';
import { TreeItem } from '@progress/kendo-angular-treeview';

@Component({
  selector: 'mega-menu',
  templateUrl: 'mega-menu.component.html'
})
export class MegaMenuComponent extends BaseComponent implements OnInit {
  @Input() public serviceScheme: INavigationMenuItem[] = [];

  @Input() public subMenu: INavigationMenuItem[] = [];
  public serviceSchemesResult: INavigationMenuItem[] = [];
  public searchText: string = '';
  public totalSearchResult: number = 0;
  public isSearching: boolean = false;
  public expandedKeys: string[] = [];

  public ngOnInit(): void {
    this.cloneServiceSchemeData();
  }

  public onNodeClick(event: INavigationMenuItem): void {
    event.onClick(event);
  }

  public onChildNodeClick(event: TreeItem): void {
    // If event.dataItem.data has data that belongs to parent node => we skip do anything
    if (event && event.dataItem && event.dataItem.data) {
      const isExpanded = this.expandedKeys.includes(event.index);
      if (!isExpanded) {
        this.expandedKeys.push(event.index);
      } else {
        const idx = this.expandedKeys.indexOf(event.index);
        this.expandedKeys.splice(idx, 1);
      }
      return;
    }

    event.dataItem.onClick();
  }

  public onSearch(): void {
    this.cloneServiceSchemeData();
    this.totalSearchResult = 0;
    this.isSearching = true;

    if (!this.searchText) {
      return;
    }

    let searchResult: INavigationMenuItem[] = [];

    searchResult = this.serviceSchemesResult.map(sScheme => {
      if (sScheme.data) {
        const subjectList = sScheme.data as INavigationMenuItem[];
        const subjectResult = this.subjectFilterList(subjectList);

        if (subjectResult && subjectResult.length) {
          const textExtention = subjectResult.length > 2 ? ` (${subjectResult.length} founds)` : ` (${subjectResult.length} found)`;

          sScheme.name += this.translateCommon(textExtention);

          this.totalSearchResult += subjectResult.length;
        }

        sScheme.data = subjectResult;
      }

      return sScheme;
    });

    this.serviceSchemesResult = searchResult;
  }

  public onSearchInputKeydown(event: KeyboardEvent): void {
    if (event.keyCode === KeyCode.Enter) {
      this.onSearch();
    }
  }

  public removeSearchKey(): void {
    this.searchText = '';
    this.totalSearchResult = 0;
    this.isSearching = false;
    this.cloneServiceSchemeData();
  }

  private cloneServiceSchemeData(): void {
    this.serviceSchemesResult = Utils.cloneDeep(this.serviceScheme);
  }

  private subjectFilterList(subjectList: INavigationMenuItem[]): INavigationMenuItem[] {
    return subjectList.filter(subject => {
      const name = subject.name as string;
      if (name.toUpperCase().includes(this.searchText.toUpperCase())) {
        return subject;
      }
    });
  }
}
