import {
  Component,
  OnInit,
  Input,
  ViewChild,
  Output,
  EventEmitter,
} from '@angular/core';
import { CxPeopleListComponent } from '../cx-people-list/cx-people-list.component';
import { ListType } from '../list.type';
import { FilterParamModel } from 'app/staff/staff.container/staff-list/models/filter-param.model';
import { User } from 'app-models/auth.model';
import { AppConstant } from 'app/shared/app.constant';

@Component({
  selector: 'cx-people-list-dialog',
  templateUrl: './cx-people-list-dialog.component.html',
  styleUrls: ['./cx-people-list-dialog.component.scss'],
})
export class CxPeopleListDialogComponent implements OnInit {
  /**
   * The currently login user.
   */
  @Input()
  currentUser: User;

  /**
   * The title of the dialog.
   */
  @Input()
  dialogTitle: string;

  /**
   * The search key for looking up people.
   */
  @Input()
  filterParams: FilterParamModel;
  @Input()
  defaultPageSize: number = AppConstant.ItemPerPageOnDialog;
  @Input()
  editMode: boolean = false;
  @Input()
  allowDeletion: boolean;

  /**
   * The output event containing removing users.
   */
  @Output() done: EventEmitter<any[]> = new EventEmitter<any[]>();
  @Output() cancel: EventEmitter<void> = new EventEmitter<void>();

  listType: ListType = ListType.ViewingSelectedPeople;
  @ViewChild(CxPeopleListComponent, { static: true })
  cxPeopleListComponent: CxPeopleListComponent;
  removingUsers: any[] = [];

  constructor() {}

  ngOnInit(): void {
    if (this.filterParams) {
      this.filterParams.pageSize = this.defaultPageSize;
      // Set timeout to prevent exception when accessing to the ag-grid api.
      setTimeout(() => {
        this.cxPeopleListComponent.getList(this.filterParams);
      });
    }
  }

  onPeopleRemoved(removingPerson: any): void {
    this.removingUsers.push(removingPerson);
  }

  onCancel(): void {
    this.cancel.emit();
  }

  onDone(): void {
    this.done.emit(this.removingUsers);
  }
}
