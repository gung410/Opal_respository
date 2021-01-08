import { Component, OnInit, Input, TemplateRef, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'cx-expandable-list',
  templateUrl: './cx-expandable-list.component.html',
  styleUrls: ['./cx-expandable-list.component.scss']
})
export class CxExpandableListComponent<T> implements OnInit {
  @Input() items: T[];
  @Input() symbol = ',';
  @Input() numberOfDisplay: 2;
  @Input() expandTemplate: TemplateRef<any>;
  @Output() expandEvent: EventEmitter<any> = new EventEmitter<any>();
  public showExpand: boolean;
  constructor() {}

  ngOnInit() {
    this.showExpand = false;
  }

  public expandClick(element: any) {
    element.classList.add('cx-hide');
    this.showExpand = true;
    this.expandEvent.emit();
  }
}
