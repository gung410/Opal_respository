
import { Component, OnInit, Input } from '@angular/core';
import { CxSideBarMenuItem } from './models/cx-side-bar-menu-item';

@Component({
  selector: 'cx-sidebar-dropdown-menu',
  templateUrl: './cx-sidebar-dropdown-menu.component.html',
  styleUrls: ['./cx-sidebar-dropdown-menu.component.scss']
})
export class CxSidebarDropdownMenuComponent implements OnInit {

  @Input() listMenu: Array<CxSideBarMenuItem>;

  constructor() {
  }

  ngOnInit() {

  }

}
