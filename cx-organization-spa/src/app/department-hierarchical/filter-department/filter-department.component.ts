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

  private DELAY_TRANSITION_TIME: number = 250;
  constructor(changeDetectorRef: ChangeDetectorRef) {
    super(changeDetectorRef);
  }

  ngOnInit(): void {
    this.addSlideInAnimation();
  }

  addSlideInAnimation(): void {
    const modalElement = document.getElementsByClassName('modal-content')[0];
    if (modalElement) {
      modalElement.classList.add('slide-in');
    }
  }

  onClear(): void {
    this.departmentFilterOptions[0].options.filter((option) => {
      option.isSelected = false;
    });
  }
  onCancel(): void {
    const modalElement = document.getElementsByClassName('modal-content')[0];
    if (modalElement) {
      modalElement.classList.remove('slide-in');
      modalElement.classList.add('slide-out');
    }

    setTimeout(() => {
      this.cancel.emit();
    }, this.DELAY_TRANSITION_TIME);
  }
  onApply(): void {
    this.done.emit(this.departmentFilterOptions);
  }

  onSelected(selectedDepartmentEvent: any): void {
    this.departmentFilterOptions[0].options.filter((option) => {
      if (option.objectId === selectedDepartmentEvent.value) {
        option.isSelected = selectedDepartmentEvent.selected;
      }
    });
  }
}
