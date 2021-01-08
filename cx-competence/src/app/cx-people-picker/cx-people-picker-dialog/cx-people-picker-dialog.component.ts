import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import {
  CxSurveyjsComponent,
  CxSurveyjsEventModel,
  CxSurveyjsVariable,
} from '@conexus/cx-angular-common';
import { TranslateService } from '@ngx-translate/core';
import { User } from 'app-models/auth.model';
import { AppConstant } from 'app/shared/app.constant';
import { UserStatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { FilterParamModel } from 'app/staff/staff.container/staff-list/models/filter-param.model';
import { ToastrService } from 'ngx-toastr';
import { CxPeopleListComponent } from '../cx-people-list/cx-people-list.component';
import { ListType } from '../list.type';
import { CxPeoplePickerDialogHelper } from './cx-people-picker-dialog.helper';
import { peoplePickerFilterFormJSON } from './people-picker-filter.form';
import { StartingHierarchyDepartment } from './starting-hierarchy-department.enum';

@Component({
  selector: 'cx-people-picker-dialog',
  templateUrl: './cx-people-picker-dialog.component.html',
  styleUrls: ['./cx-people-picker-dialog.component.scss'],
})
export class CxPeoplePickerDialogComponent implements OnInit {
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
  @Input()
  currentNumberOfPeople: number = 0;
  @Input()
  maxNumberOfPeople: number = 0;

  /**
   * The search key for looking up people.
   */
  @Input()
  searchKey: string;
  @Input()
  supportFilterByServiceScheme: boolean;

  @Input()
  startingHierarchyDepartment: StartingHierarchyDepartment;
  @Input()
  specifyStartingHierarchyDepartmentId?: number;
  /**
   * Determines whether it should lookup for users in descendant departments or not.
   */
  @Input()
  includeUsersInDescendentDepartments: boolean = true;
  /**
   * The list of user statuses using for filtering.
   */
  @Input()
  userEntityStatuses: UserStatusTypeEnum[] = [
    UserStatusTypeEnum.Active,
    UserStatusTypeEnum.New,
  ];

  /**
   * The output event containing selected users.
   */
  @Output() done: EventEmitter<any[]> = new EventEmitter<any[]>();
  @Output() cancel: EventEmitter<void> = new EventEmitter<void>();

  listType: ListType = ListType.PickingPeople;
  disableSearchBtn: boolean = true;
  filterSurveyFormJSON: any = peoplePickerFilterFormJSON;
  filterSurveyVariables: CxSurveyjsVariable[] = [];
  @ViewChild(CxSurveyjsComponent, { static: true }) filterSurveyJs: CxSurveyjsComponent;
  @ViewChild(CxPeopleListComponent, { static: true })
  cxPeopleListComponent: CxPeopleListComponent;
  selectedRowCount: number = 0;
  defaultPageSize: number = AppConstant.ItemPerPageOnDialog;
  filterParams: FilterParamModel;

  constructor(
    private toastrService: ToastrService,
    private translateService: TranslateService
  ) {}

  ngOnInit(): void {
    this.filterParams = CxPeoplePickerDialogHelper.initFilterParams(
      this.currentUser,
      this.defaultPageSize,
      this.startingHierarchyDepartment,
      this.specifyStartingHierarchyDepartmentId
    );
    this.filterSurveyVariables = [
      new CxSurveyjsVariable({
        name: 'supportFilterByServiceScheme',
        value: this.supportFilterByServiceScheme,
      }),
    ];
  }

  afterFilterQuestionsRendered(event: CxSurveyjsEventModel): void {
    const question = event.options.question;
    const htmlElement = event.options.htmlElement;
    if (question.name === 'searchKey') {
      const inputs = htmlElement.getElementsByTagName('input');
      if (inputs) {
        // Add event listener triggering the search command when the user hits Enter.
        const searchKeyInput = inputs[0];
        searchKeyInput.addEventListener(
          'keyup',
          (keyboardEvent: KeyboardEvent) => {
            this.disableSearchBtn =
              (keyboardEvent.target as any).value.length === 0;
            const enterKeyCode = 13; // Number is the "Enter" key on the keyboard
            if (keyboardEvent.keyCode === enterKeyCode) {
              // Cancel the default action, if needed
              keyboardEvent.preventDefault();
              if (!this.disableSearchBtn) {
                // Trigger the complete survey event.
                this.filterSurveyJs.doComplete();
              }
            }
          }
        );
        setTimeout(() => {
          searchKeyInput.focus();
        });
      }
    }
  }

  onFilterChanged(surveyEvent: CxSurveyjsEventModel): void {
    const surveyData = surveyEvent.survey.data;
    this.disableSearchBtn = CxPeoplePickerDialogHelper.checkDisableSearchButton(
      surveyData
    );
  }

  onSearchClick(surveyEvent: CxSurveyjsEventModel): void {
    if (!surveyEvent.options.allowComplete) {
      return;
    }
    surveyEvent.options.allowComplete = false;
    const surveyData = surveyEvent.survey.data;
    this.filterParams.pageIndex = 1;
    this.filterParams.idpEmployeeSearchKey = surveyData.searchKey;
    this.filterParams.multiUserTypeExtIds = CxPeoplePickerDialogHelper.hasFilterOnServiceSchemes(
      surveyData
    )
      ? [surveyData.serviceSchemes]
      : [];

    this.cxPeopleListComponent.getList(this.filterParams);
  }

  onCancel(): void {
    this.cancel.emit();
  }

  onDone(): void {
    const numberOfOverflowedPeople = this.calculateNumberOfOverflowedPeople();
    if (numberOfOverflowedPeople > 0) {
      const warningMessage = this.translateService.instant(
        'People_List.Dialog.WarningMessage.MaxRowCountIsReached',
        { numberOfOverflowedPeople }
      );
      this.toastrService.warning(warningMessage);
    } else {
      this.done.emit(this.cxPeopleListComponent.getSelectedPeople());
    }
  }

  onSelectionChanged(selectedPeople: any[]): void {
    this.selectedRowCount = selectedPeople.length;
  }

  private calculateNumberOfOverflowedPeople(): number {
    return (
      this.currentNumberOfPeople +
      this.selectedRowCount -
      this.maxNumberOfPeople
    );
  }
}
