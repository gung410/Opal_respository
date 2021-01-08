import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnChanges,
  SimpleChanges,
} from '@angular/core';
import { get, isEmpty } from 'lodash';

@Component({
  selector: 'opportunity-tag',
  templateUrl: './opportunity-tag.component.html',
  styleUrls: ['./opportunity-tag.component.scss'],
})
export class OpportunityTagComponent<T> implements OnChanges {
  @Input() tag: T;
  @Input() bindLabel: string;
  @Output() clicked: EventEmitter<T> = new EventEmitter<T>();
  tagName: string;
  constructor() {}

  ngOnChanges(): void {
    this.tagName = isEmpty(this.bindLabel)
      ? this.tag
      : get(this.tag, this.bindLabel);
  }

  onTagClicked(): void {
    this.clicked.emit(this.tag);
  }
}
