import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output
} from '@angular/core';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';

import { DepartmentFilterGroupModel } from '../models/filter-params.model';

@Component({
  selector: 'filter-department',
  templateUrl: './filter-department.component.html',
  styleUrls: ['./filter-department.component.scss']
})
export class FilterDepartmentComponent
  extends BaseSmartComponent
  implements OnInit {
  @Input() departmentFilterOptions: DepartmentFilterGroupModel[];
  @Output() cancel: EventEmitter<any> = new EventEmitter<any>();
  @Output() done: EventEmitter<any> = new EventEmitter<any>();
  constructor(changeDetectorRef: ChangeDetectorRef) {
    super(changeDetectorRef);
  }

  ngOnInit(): void {
    //TODO
  }

  onCancel(): void {
    this.cancel.emit();
  }

  onDone(): void {
    this.done.emit(this.departmentFilterOptions);
  }
}
