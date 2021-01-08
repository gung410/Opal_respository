import { Component, OnInit } from "@angular/core";
import { rolesData } from "./mock-data/item.data";

@Component({
  selector: "cx-expandable-list-doc",
  templateUrl: "./cx-expandable-list-doc.component.html",
  styleUrls: ["./cx-expandable-list-doc.component.scss"]
})
export class CxExpandableListDocComponent implements OnInit {
  public items = rolesData;
  constructor() {}

  ngOnInit() {
  }
}
