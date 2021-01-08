import {
  Component, OnInit, Input, ViewEncapsulation, Output,
  EventEmitter, TemplateRef,
  ViewChild
} from '@angular/core';
import { CxTreeButtonCondition } from '../models/cx-tree-button-condition.model';
import { CxTreeText } from '../models/cx-tree-text.model';
import { CxTreeIcon } from '../models/cx-tree-icon.model';
import { NgbDropdownConfig, NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { CxObjectRoute } from '../models/cx-object-route.model';
import { MultipleLanguages } from '../../cx-header/models/multiple-languages.model';
@Component({
  selector: 'cx-tree-dropdown',
  templateUrl: './cx-tree-dropdown.component.html',
  styleUrls: ['./cx-tree-dropdown.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [NgbDropdownConfig]
})
export class CxTreeDropdownComponent<T> implements OnInit {

  @Input() havingExtensiveArea: boolean;
  @Input() isViewMode: boolean;
  @Input() flatObjectsArray: T[];
  @Input() idFieldRoute: string;
  @Input() parentIdFieldRoute: string;
  @Input() displayFieldRoute: string;
  @Input() icon: CxTreeIcon;
  @Input() text: CxTreeText;
  @Input() displayTemplate: TemplateRef<any>;
  @Input() displaySearchResult: TemplateRef<any>;
  @Input() extendFooterDepartmentTree: TemplateRef<any>;
  @Input() isShowSearchResult = false;
  @Input() buttonCondition: CxTreeButtonCondition<T>;
  @Input() treeHeader: string;
  @Input() currentDepartmentId: number;
  @Input() isDetectExpandTree: boolean;
  @Input() currentRoutes: [] = [];
  @Input() multipleLanguages: MultipleLanguages = {
    notifications: 'Notifications',
    search: 'Search for'
  };
  @Input() title = '';
  @Input() enableSearch = false;
  private _searchValue = '';
  @Input() get searchValue() {
    return this._searchValue;
  }
  set searchValue(val) {
    this._searchValue = val;
    this.searchValueChange.emit(val);
  }
  @Output() selectItem: EventEmitter<CxObjectRoute<T>> = new EventEmitter();
  @Output() searchValueChange = new EventEmitter<string>();
  @Output() searchOnSearchBox = new EventEmitter<string>();
  @Output() expandChildOrganisation = new EventEmitter<{}>();
  @ViewChild(NgbDropdown)
  private dropdown: NgbDropdown;

  constructor() { }
  ngOnInit() {
  }

  public onSelectItem(objectRoute: CxObjectRoute<T>) {
    this.selectItem.emit(objectRoute);
  }

  public onClearSearch() {
    this.searchValue = '';
    this.searchOnSearchBox.emit(this.searchValue);
  }

  public onSearch(searchTerm: string): void {
    this.searchOnSearchBox.emit(searchTerm);
  }

  public closeDropdown() {
    this.dropdown.close();
  }

  public toggleDropdown = () => {
    this.dropdown.toggle();
  }

  public isShowDropdown = () => {
    return this.dropdown && this.dropdown.isOpen();
  }

  public loadChildrenDepartment(selectedDepartment) {
    this.expandChildOrganisation.emit(selectedDepartment);
  }

}
