import { Component, OnInit, ViewChildren, QueryList } from "@angular/core";
import {
  CxCheckboxComponent,
  CxCustomizedCheckboxComponent
} from "projects/cx-angular-common/src";

@Component({
  selector: "cx-checkbox-doc",
  templateUrl: "./cx-checkbox-doc.component.html",
  styleUrls: ["./cx-checkbox-doc.component.scss"]
})
export class CxCheckboxDocComponent implements OnInit {
  public cbs1 = ["1", "2"];
  public cbs2 = ["1", "2", "3"];
  public cbs3 = ["1", "2", "3"];
  public checkboxes1 = [];
  public checkboxes2 = [];
  public checkboxes3 = [];
  constructor() {}

  ngOnInit() {}
}
