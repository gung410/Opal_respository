import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { KLPPlannedAreaModel } from 'app-models/opj/klp-planned-areas.model';

@Component({
  selector: 'planned-area',
  templateUrl: './planned-area.component.html',
  styleUrls: ['./planned-area.component.scss'],
})
export class PlannedAreaComponent implements OnInit {
  @Input() areaModel: KLPPlannedAreaModel;
  @Input() allowRemove: boolean = false;
  @Output() clickedRemove: EventEmitter<string> = new EventEmitter();

  constructor() {}

  ngOnInit(): void {}

  onClickRemove(): void {
    this.clickedRemove.emit(this.areaModel.area.id);
  }
}
