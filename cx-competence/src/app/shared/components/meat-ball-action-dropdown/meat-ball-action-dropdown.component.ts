import { Component, Input, OnInit } from '@angular/core';
import { Action } from './../../models/action.model';

@Component({
  selector: 'meat-ball-action-dropdown',
  templateUrl: './meat-ball-action-dropdown.component.html',
  styleUrls: ['./meat-ball-action-dropdown.component.scss'],
})
export class MeatBallActionDropdownComponent implements OnInit {
  @Input() actions: Action[];
  constructor() {}

  ngOnInit(): void {}
}
