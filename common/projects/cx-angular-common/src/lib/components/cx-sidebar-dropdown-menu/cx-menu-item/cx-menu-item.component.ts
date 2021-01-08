import { Component, OnInit, Input } from "@angular/core";
import { CxSideBarMenuItem } from "../models/cx-side-bar-menu-item";

@Component({
  selector: "cx-menu-item",
  templateUrl: "./cx-menu-item.component.html",
  styleUrls: ["./cx-menu-item.component.scss"]
})
export class CxMenuItemComponent implements OnInit {
  // inputs
  @Input() menuItem: CxSideBarMenuItem;

  // properties
  indentation: string;
  haveChildren: boolean;

  constructor() {}

  ngOnInit() {
    this.haveChildren = this.menuItem.children.length > 0;
    this.indentation = this.menuItem.level * 15 + 'px';
  }

  // methods
  collapse() {
    this.menuItem.isCollapsed = !this.menuItem.isCollapsed;
    if (this.menuItem.level === 1 && this.menuItem.isCollapsed) {
      this.collapseDescendants(this.menuItem);
    }
  }

  collapseDescendants(menuItem: CxSideBarMenuItem) {
    menuItem.children.forEach(child => {
      child.isCollapsed = true;
      if (child.children) {
        this.collapseDescendants(child);
      }
    });
  }
}
