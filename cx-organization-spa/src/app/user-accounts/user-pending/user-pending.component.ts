import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  Renderer2,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import {
  CxColumnSortType,
  CxItemTableHeaderModel,
  CxTableComponent
} from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from 'app-auth/auth.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { SurveyUtils } from 'app-utilities/survey-utils';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';

import { UserManagement } from '../models/user-management.model';
import { UserAccountsDataService } from '../user-accounts-data.service';

@Component({
  selector: 'user-pending',
  templateUrl: './user-pending.component.html',
  styleUrls: ['./user-pending.component.scss'],
  providers: [UserAccountsDataService],
  encapsulation: ViewEncapsulation.None
})
export class UserPendingComponent
  extends BaseScreenComponent
  implements OnInit, OnDestroy {
  @Input() get items(): UserManagement[] {
    return this._items;
  }
  set items(data: UserManagement[]) {
    this._items = data;

    // [Tom - Tam Pham][06/09/2019] in the case of undefined data, this._items.some() make an exception,
    // so we need to check undefine befor get some().
    if (data) {
      this.isPendingSpecialApprovalList = this._items.some(
        (item) =>
          item.entityStatus.statusId === StatusTypeEnum.PendingApproval3rd.code
      );
    }
  }
  @Input() isCurrentUserSuperAdmin: boolean;
  @Input() isCurrentUserAccountAdmin: boolean;
  @Input() viewOnly: boolean;
  @Output() acceptedPending: EventEmitter<any[]> = new EventEmitter<any[]>();
  @Output() rejectedPending: EventEmitter<any[]> = new EventEmitter<any[]>();
  @Output() requestSpecialApprovalClicked: EventEmitter<
    any[]
  > = new EventEmitter<any[]>();
  @Output() editPendingUserClicked: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild(CxTableComponent) cxTableComponent: CxTableComponent<any>;
  currentUserTableSortType: CxColumnSortType = CxColumnSortType.ASCENDING;
  headers: CxItemTableHeaderModel[];
  statusTypeEnum: typeof StatusTypeEnum = StatusTypeEnum;
  isPendingSpecialApprovalList: boolean = false;

  private _items: UserManagement[];

  constructor(
    public authService: AuthService,
    changeDetectorRef: ChangeDetectorRef,
    public ngbModal: NgbModal,
    private translate: TranslateAdapterService,
    private renderer2: Renderer2
  ) {
    super(changeDetectorRef, authService);
  }

  ngOnInit(): void {
    this.subscription.add(
      this.translate
        .getValueBasedOnKey('User_Account_Page.Table_Header')
        .subscribe((values) => {
          this.headers = [
            {
              text: values.Name,
              fieldSort: 'firstName',
              sortType: CxColumnSortType.UNSORTED
            },
            {
              text: values.Email,
              sortType: CxColumnSortType.UNSORTED
            },
            {
              text: values.Department,
              sortType: CxColumnSortType.UNSORTED
            },
            {
              text: values.Registered,
              sortType: CxColumnSortType.UNSORTED
            },
            {
              text: values.Status,
              sortType: CxColumnSortType.UNSORTED
            }
          ];
        })
    );

    window.addEventListener('scroll', this.windowScroll.bind(this), true);
  }

  ngOnDestroy(): void {
    super.ngOnDestroy();

    window.removeEventListener('scroll', this.windowScroll, true);
  }

  onAcceptClicked(user?: any): void {
    return user
      ? this.acceptedPending.emit(user)
      : this.acceptedPending.emit(
          Object.values(this.cxTableComponent.getSelectedItemsMap())
        );
  }

  onRejectClicked(user?: any): void {
    return user
      ? this.rejectedPending.emit(user)
      : this.rejectedPending.emit(
          Object.values(this.cxTableComponent.getSelectedItemsMap())
        );
  }

  onRequestSpecialApprovalClicked(user?: any): void {
    return user
      ? this.requestSpecialApprovalClicked.emit(user)
      : this.requestSpecialApprovalClicked.emit(
          Object.values(this.cxTableComponent.getSelectedItemsMap())
        );
  }

  onSortTypeChange($event: {
    fieldSort: string;
    sortType: CxColumnSortType;
  }): void {
    this.items = this.getPendingUsersInPage(
      this.items,
      $event.sortType,
      $event.fieldSort
    );
  }

  onEditPendingUserClicked(employee: any): void {
    this.editPendingUserClicked.emit(employee);
  }

  private getPendingUsersInPage(
    allUsers: any[],
    sortType: CxColumnSortType,
    fieldSort: string
  ): any[] {
    const sortedEmployees = JSON.parse(JSON.stringify(allUsers)) as any[];
    switch (sortType) {
      case CxColumnSortType.ASCENDING:
        sortedEmployees.sort((emp1, emp2) => {
          return emp1[fieldSort] > emp2[fieldSort] ? 1 : -1;
        });
        break;
      case CxColumnSortType.DESCENDING:
        sortedEmployees.sort((emp1, emp2) => {
          return emp1[fieldSort] > emp2[fieldSort] ? -1 : 1;
        });
        break;
      case CxColumnSortType.UNSORTED:
        break;
      default:
        break;
    }

    return sortedEmployees;
  }

  private windowScroll(): void {
    const dropdownElement: HTMLElement = document.querySelector(
      'body > .dropdown'
    );

    if (dropdownElement) {
      this.renderer2.removeChild(
        dropdownElement.parentElement,
        dropdownElement
      );
    }
  }
}
