import { Component, OnInit } from '@angular/core';
import { CxSideBarMenuItem } from 'projects/cx-angular-common/src/lib/components/cx-sidebar-dropdown-menu/models/cx-side-bar-menu-item';

@Component({
  selector: 'cx-sidebar-dropdown-menu-doc',
  templateUrl: './cx-sidebar-dropdown-menu.component.html',
  styleUrls: ['./cx-sidebar-dropdown-menu.component.scss']
})
export class CxSidebarDropdownMenuDocComponent implements OnInit {

  listMenu: Array<CxSideBarMenuItem>;

  constructor() { }

  ngOnInit() {
    this.listMenu =
    [
      new CxSideBarMenuItem({content : 'menu 1', level: 1, isCollapsed: true, isDisplay : true, children : [
        new CxSideBarMenuItem({content : 'child menu 1', level: 2, isDisplay : true}),
        new CxSideBarMenuItem({content : 'child menu 2', level: 2, isDisplay : true, children : [
          new CxSideBarMenuItem({content : 'child menu 3', level: 3, isDisplay : true}),
          new CxSideBarMenuItem({content : 'child menu 4', isCollapsed: true, level: 3, isDisplay : true})
        ]})
      ]}),
      new CxSideBarMenuItem({content : 'menu 2', level: 1, isDisplay : true}),
      new CxSideBarMenuItem({content : 'menu 3', isCollapsed: true, level: 1, isDisplay : true, children : [
        new CxSideBarMenuItem({content : 'child menu 5', level: 2, isDisplay : true}),
        new CxSideBarMenuItem({content : 'child menu 6', level: 2, isDisplay : true})
      ]})
    ];
  }

}
