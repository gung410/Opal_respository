import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CxSideBarMenuItem } from '../models/cx-side-bar-menu-item';

@Component({
  selector: 'cx-dropdown-menu',
  templateUrl: './cx-dropdown-menu.component.html',
  styleUrls: ['./cx-dropdown-menu.component.scss']
})
export class CxDropdownMenuComponent implements OnInit {


  @Input() listMenu: Array<CxSideBarMenuItem>;
  @Input() isCollapsed: boolean;

  constructor() { }

  ngOnInit() {
  }

}
