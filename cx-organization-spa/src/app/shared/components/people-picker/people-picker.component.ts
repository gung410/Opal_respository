import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  OnChanges,
  Output,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import { CxObjectUtil, debounce } from '@conexus/cx-angular-common';

import { get, last } from 'lodash';
import { SurveyUtils } from '../../utilities/survey-utils';
import { BasePresentationComponent } from '../component.abstract';
import { PeoplePickerEventModel } from './people-picker.model';
import { AppConstant } from 'app/shared/app.constant';
import { UserManagement } from 'app/user-accounts/models/user-management.model';

@Component({
  selector: 'people-picker',
  templateUrl: './people-picker.component.html',
  styleUrls: ['./people-picker.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PeoplePickerComponent<T>
  extends BasePresentationComponent
  implements OnChanges {
  getPropertyValue: any = get;
  @Input() get selectedPeople(): any {
    return this._selectedPeople;
  }

  set selectedPeople(val: any) {
    this._selectedPeople = val;
    this.selectedPeopleChange.emit(this._selectedPeople);
  }
  @Input() stopLoadMore: boolean = true;
  @Input() closeOnSelect: boolean;
  @Input() multiple: boolean;
  @Input() placeholder: string;
  @Input() avatarField: string;
  @Input() displayPrimaryField: string;
  @Input() displaySecondaryField: string;
  @Input() displayThirdField: string;
  @Input() emailField: string;
  @Input() numberOfItemsBeforeFetchingMore: number = 10;
  @Input() people: T[] = [];
  @Input() disabled: boolean = false;
  @Input() compareWith: (a: any, b: any) => boolean = (a, b) => a === b;
  @Output() selectedPeopleChange: EventEmitter<any> = new EventEmitter();
  @Output() loadMore: EventEmitter<PeoplePickerEventModel> = new EventEmitter();
  @Output() open: EventEmitter<void> = new EventEmitter();
  @Output() focus: EventEmitter<void> = new EventEmitter();
  loading: boolean = false;
  @ViewChild('ngSelect') ngSelect: any;
  @ViewChild('selecHeaderInput') selecHeaderInput: ElementRef;
  @ViewChild('selectFooterInput') selectFooterInput: ElementRef;

  hoveredUser: UserManagement;

  private pageIndex: number = 1;
  private searchKey: any = '';
  private _selectedPeople: T[] = [];
  private viewResizeEventQueue: any = [];
  constructor(changeDetectorRef: ChangeDetectorRef) {
    super(changeDetectorRef);
  }

  getAvatar(user): string {
    const avatar = this.getPropertyValue(user, this.avatarField);

    return avatar ? avatar : AppConstant.defaultAvatar;
  }

  searchFn = () => true;

  ngOnChanges(): void {
    this.loading = false;
  }

  onScrollToEnd(): void {
    if (this.loading || this.stopLoadMore) {
      return;
    }
    this.fetchMore();
  }

  // tslint:disable-next-line:no-magic-numbers
  @debounce(500)
  onSearch({ term }: any): void {
    this.searchKey = term;
    this.pageIndex = 1;
    this.loading = true;
    this.loadMore.emit({ key: this.searchKey, pageIndex: this.pageIndex });
  }

  @HostListener('window:resize')
  onResize(): void {
    const isOpen = this.ngSelect.isOpen;
    if (!isOpen) {
      return;
    }

    const eventId = new Date().getTime();
    const WAIT_TIME = 100;
    this.viewResizeEventQueue.push(eventId);

    // When viewport resize, this function will wait to the end of action to trigger rerender select dropdown
    // If after WAIT_TIME, the queue don't have any new event, this func detect the action ended.
    setTimeout(() => {
      const lastId = last(this.viewResizeEventQueue);
      if (lastId === eventId) {
        this.viewResizeEventQueue = [];
        this.rerenderSelect();
      }
    }, WAIT_TIME);
  }

  rerenderSelect = (): void => {
    this.ngSelect.close();
    this.changeDetectorRef.detectChanges();
    this.ngSelect.open();
  };

  onClear(): void {
    this.onSearch({ term: '' });
    this.rerenderSelect();
  }

  onFocus(): void {
    if (this.hoveredUser) {
      this.updateSelectedPeople();
    }
    if (!this.people || !this.people.length) {
      this.ngSelect.isOpen = true;
    }
    this.focus.emit();
    setTimeout(() => {
      this.focusInput(this.selecHeaderInput);
      this.focusInput(this.selectFooterInput);
    });
  }

  onOpen(): void {
    if (this.hoveredUser) {
      this.updateSelectedPeople();
    }

    if (this.searchKey) {
      this.onSearch({ term: '' });
    }
    this.open.emit();
  }

  triggerEnterHover(user: UserManagement): void {
    this.hoveredUser = user;
  }

  triggerLeaveHover(): void {
    this.hoveredUser = null;
  }

  onScroll(event: any): void {
    if (this.loading || this.stopLoadMore) {
      return;
    }
    if (
      this.ngSelect.itemsList._filteredItems.length <
        this.numberOfItemsBeforeFetchingMore ||
      event.end + this.numberOfItemsBeforeFetchingMore >= this.people.length
    ) {
      this.fetchMore();
    }
  }

  selectPeople(selectedPeople: T): void {
    this.selectedPeopleChange.emit(selectedPeople);
  }

  private updateSelectedPeople(): void {
    /* There is an issue regrading to deleting user by (x) icon at AA/AAO fields in case we have many users added at the first time
    after open the popup. The selectPeople() function is not get executed because open event from ng-select triggered first,
    so we need to check for it.
    */

    this.selectedPeople = this.selectedPeople.filter(
      (person: UserManagement) =>
        person.emailAddress !== this.hoveredUser.emailAddress
    );
    this.selectedPeopleChange.emit(this.selectedPeople);

    this.hoveredUser = null;
  }

  private fetchMore(): void {
    this.loading = true;
    this.pageIndex++;
    this.loadMore.emit({ key: this.searchKey, pageIndex: this.pageIndex });
  }

  private focusInput = (inputElementRef: ElementRef): void => {
    if (inputElementRef && inputElementRef.nativeElement) {
      const displayValue = inputElementRef.nativeElement.style.display;
      if (displayValue !== 'none') {
        inputElementRef.nativeElement.focus();
      }
    }
  };
}
