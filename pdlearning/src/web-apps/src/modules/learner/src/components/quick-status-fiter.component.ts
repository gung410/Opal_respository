import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

import { StatusFilterModel } from '../models/status-filter.model';

@Component({
  selector: 'quick-status-fiter',
  templateUrl: './quick-status-fiter.component.html'
})
export class QuickStatusFiterComponent extends BaseComponent implements OnInit {
  @Input('statusFilter') public statusFilter: StatusFilterModel[];
  @Input('currentStatusFilter') public currentStatusFilter: string;
  @Output() public handleFilter: EventEmitter<Event> = new EventEmitter();

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }
}
