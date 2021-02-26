import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import {
  CxSurveyjsComponent,
  CxSurveyjsEventModel,
  CxSurveyjsVariable
} from '@conexus/cx-angular-common';
import { AuthService } from 'app-auth/auth.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { ObjectUtilities } from 'app-utilities/object-utils';
import { SystemRolesStoreService } from 'app/core/store-services/system-roles-store.service';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';
import { AgeGroupTextConst } from 'app/shared/constants/age-group.enum';
import { findIndexCommon } from 'app/shared/constants/common.const';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { UserGroupsDataService } from 'app/user-groups/user-groups-data.service';
import { UserGroupFilterParams } from 'app/user-groups/user-groups.model';
import { SurveyModel } from 'survey-angular';

import { GroupFilterConst } from '../../shared/constants/filter-group.constant';
import { UserEntityStatusConst } from '../user-accounts.model';
import {
  AppliedFilterModel,
  FilterModel,
  ObjectData
} from './applied-filter.model';
import { AppliedFilterFormJSON, FilterFormJSON } from './filter-form-surveyjs';

@Component({
  selector: 'user-filter',
  templateUrl: './user-filter.component.html',
  styleUrls: ['./user-filter.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserFilterComponent extends BaseSmartComponent implements OnInit {
  currentLanguage: string;
  systemRoles: any;
  filterFormJSON: any = FilterFormJSON;
  appliedFilterFormJSON: any = AppliedFilterFormJSON;
  appliedData: FilterModel = new FilterModel();
  filterVariables: CxSurveyjsVariable[] = [];
  filterParams: UserGroupFilterParams;

  @ViewChild('filterSurveyJs') filterSurveyJs: CxSurveyjsComponent;
  @ViewChild('appliedFilterSurveyJs')
  appliedFilterSurveyJs: CxSurveyjsComponent;
  @Input() adjustAppliedData: any;
  @Output() applyClick: EventEmitter<any> = new EventEmitter<any>();
  @Output() cancel: EventEmitter<any> = new EventEmitter<any>();
  private _userGroupFilterOptions: ObjectData[];

  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private translateAdapterService: TranslateAdapterService,
    private systemRolesStoreService: SystemRolesStoreService,
    private userGroupsDataService: UserGroupsDataService,
    private authService: AuthService
  ) {
    super(changeDetectorRef);
    this.currentLanguage = this.translateAdapterService.getCurrentLanguage();
  }

  ngOnInit(): void {
    this.initFilterData();
    this.subscription.add(
      this.authService.userData().subscribe((user) => {
        if (!user || !user.departmentId) {
          return;
        }

        this.initFilterParams(user.departmentId);
        this.changeDetectorRef.detectChanges();
      })
    );
    if (this.adjustAppliedData) {
      this.appliedData = { ...this.adjustAppliedData };
      this.changeDetectorRef.detectChanges();
    }
  }

  initFilterData(): void {
    this.filterVariables.push(
      new CxSurveyjsVariable({
        name: 'replaceTS',
        value: Math.random().toString()
      })
    );

    this.subscription.add(
      this.systemRolesStoreService.get().subscribe((systemRoles) => {
        if (!systemRoles) {
          return;
        }
        this.systemRoles = systemRoles;
        this.changeDetectorRef.detectChanges();
      })
    );
  }

  onAddCriteriaChanged(data: CxSurveyjsEventModel): void {
    if (data.options.question.name === 'filterOptions') {
      this.handleChangingFilterOptions(data.survey, data.options.question);

      return;
    }
    if (
      data.options.question.name === 'creationDateTo' ||
      data.options.question.name === 'creationDateFrom'
    ) {
      const creationDateTo: any = data.survey.getQuestionByName(
        'creationDateTo'
      );
      const creationDateFrom: any = data.survey.getQuestionByName(
        'creationDateFrom'
      );
      if (creationDateFrom.value !== undefined) {
        creationDateFrom.hasErrors(true);
      }
      if (creationDateTo.value !== undefined) {
        creationDateTo.hasErrors(true);
      }

      return;
    }
    if (
      data.options.question.name === 'expirationDateFrom' ||
      data.options.question.name === 'expirationDateTo'
    ) {
      const expirationDateTo: any = data.survey.getQuestionByName(
        'expirationDateTo'
      );
      const expirationDateFrom: any = data.survey.getQuestionByName(
        'expirationDateFrom'
      );
      if (expirationDateFrom.value !== undefined) {
        expirationDateFrom.hasErrors(true);
      }
      if (expirationDateTo.value !== undefined) {
        expirationDateTo.hasErrors(true);
      }

      return;
    }
    data.options.question.hasErrors(true);
  }

  onAddNewFilter(event: CxSurveyjsEventModel): void {
    if (!event.options.allowComplete) {
      return;
    }
    event.options.allowComplete = false;
    if (ObjectUtilities.isEmpty(event.survey.data)) {
      return;
    }
    this.convertDataToDisplay(event.survey.data);
    event.survey.data = {};
    this.changeDetectorRef.detectChanges();
  }

  onApplyFilter(): void {
    this.applyClick.emit({ filteredData: this.appliedData });
  }

  onChanged(data: CxSurveyjsEventModel): void {
    this.appliedData.appliedFilter = data.options.value;
  }

  onFilterRemoved(filteredId: string): void {
    this.appliedData.appliedFilter = this.appliedData.appliedFilter.filter(
      (filteredData) => filteredData.id !== filteredId
    );
  }

  convertDataToDisplay(filterData: any): void {
    let appliedDataWithOption;
    const appliedOptionIndex = this.appliedData.appliedFilter.findIndex(
      (item) => item.filterOptions.data === filterData.filterOptions.data
    );
    if (appliedOptionIndex > findIndexCommon.notFound) {
      appliedDataWithOption = this.appliedData.appliedFilter[
        appliedOptionIndex
      ];
    } else {
      appliedDataWithOption = new AppliedFilterModel({
        filterOptions: filterData.filterOptions,
        data: new ObjectData({
          value: [],
          text: ''
        })
      });
    }

    let optionData = appliedDataWithOption.data;
    let optionFiltered;
    switch (filterData.filterOptions.data) {
      case GroupFilterConst.STATUS:
        optionData = this.mapData(optionData, filterData.status, (item) => {
          return StatusTypeEnum[item].text;
        });
        break;
      case GroupFilterConst.AGE_GROUP:
        optionData = this.mapData(optionData, filterData.ageGroup, (item) => {
          return AgeGroupTextConst[item];
        });
        break;
      case GroupFilterConst.USER_GROUP:
        optionData = this.mapData(optionData, filterData.userGroup, (item) => {
          return this._userGroupFilterOptions.find(
            (usr) => usr.value.toString() === item.toString()
          ).text;
        });
        break;

      case GroupFilterConst.ROLE:
        this.mapRoleExtIdToId(filterData);
        optionData = this.mapData(optionData, filterData.systemRole, (item) => {
          const index = this.systemRoles.findIndex(
            (role) => role.identity.id.toString() === item.toString()
          );
          if (index > -1) {
            return this.systemRoles[index].localizedData.find(
              (localizedData) =>
                localizedData.languageCode === this.currentLanguage
            ).fields[0].localizedText;
          } else {
            return '';
          }
        });
        break;
      case GroupFilterConst.ACCOUNT_TYPE:
        optionData = this.mapData(
          optionData,
          filterData.accountType,
          (item) => {
            return UserEntityStatusConst.find(
              (status) => status.key.toString() === item.toString()
            ).description;
          }
        );
        break;
      case GroupFilterConst.TYPE_OF_OU:
        optionData.value = filterData.typeOU.id;
        optionData.text = filterData.typeOU.displayText;
        break;
      case GroupFilterConst.CREATION_DATE:
        optionFiltered = this.addFilterDate(
          filterData.creationDateFrom,
          filterData.creationDateTo
        );
        optionData = { ...optionData, ...optionFiltered };
        break;
      case GroupFilterConst.EXPIRATION_DATE:
        optionFiltered = this.addFilterDate(
          filterData.expirationDateFrom,
          filterData.expirationDateTo
        );
        optionData = { ...optionData, ...optionFiltered };
        break;
      case GroupFilterConst.SERVICE_SCHEME:
        optionData.value = filterData.personnelGroups.id;
        optionData.text = filterData.personnelGroups.displayText;
        break;
      case GroupFilterConst.TEACHING_SUBJECTS:
        optionData = this.mapData(
          optionData,
          filterData.teachingSubjects,
          (item) => {
            return item.displayText;
          }
        );
        break;
      case GroupFilterConst.JOB_FAMILY:
        optionData = this.mapData(optionData, filterData.jobFamily, (item) => {
          return item.displayText;
        });
        break;
      case GroupFilterConst.DEVELOPMENTAL_ROLE:
        optionData.value = filterData.developmentalRole.id;
        optionData.text = filterData.developmentalRole.displayText;
        break;
      case GroupFilterConst.DESIGNATION:
        optionData = this.mapData(
          optionData,
          filterData.designation,
          (item) => {
            return item.displayText;
          }
        );
        break;
      case GroupFilterConst.ORGANISATION_UNIT:
        optionData = this.mapData(
          optionData,
          filterData.organisationUnit,
          (item) => {
            return item.name;
          }
        );
        break;
      default:
        break;
    }
    appliedDataWithOption.data = optionData;
    if (appliedOptionIndex === -1) {
      this.appliedData.appliedFilter.push(appliedDataWithOption);
    }
    this.appliedData = { ...this.appliedData };

    return;
  }

  onCancel(): void {
    this.cancel.emit();
  }

  unionArray(oldArr: any, newArr: any): any[] {
    const unionData = new Set([...oldArr, ...newArr]);

    return Array.from(unionData);
  }

  onSave(): void {
    this.appliedFilterSurveyJs.doComplete();
  }

  private mapData(optionData: any, filterData: any, mapExpression: any): any {
    const dataTemp = this.unionArray(optionData.value, filterData);

    return {
      ...optionData,
      value: dataTemp,
      text: dataTemp.map(mapExpression).join(', ')
    };
  }

  private addFilterDate(from: any, to: any): any {
    return {
      text: from + ' - ' + to,
      value: [from, to]
    };
  }

  private mapRoleExtIdToId(filterData: any): void {
    filterData.systemRole.forEach((roleExtId, i) => {
      const index = this.systemRoles.findIndex(
        (role) => role.identity.extId.toString() === roleExtId.toString()
      );
      if (index < 0) {
        return;
      }
      filterData.systemRole[i] = this.systemRoles[index].identity.id;
    });
  }
  private handleChangingFilterOptions(
    survey: SurveyModel,
    filterOptionQuestion: any
  ): void {
    if (filterOptionQuestion.value.data === GroupFilterConst.USER_GROUP) {
      this.loadUserGroupOptions(survey);
    }
  }
  private loadUserGroupOptions(survey: SurveyModel): void {
    this.userGroupsDataService
      .getUserGroups(this.filterParams)
      .subscribe((response) => {
        const question = survey.getQuestionByName('userGroup');
        if (question) {
          const userGroupFilterOptions = response.items.map((item) => {
            return {
              value: item.identity.id,
              text: item.name
            };
          });
          question.choices = userGroupFilterOptions;
        }
        this._userGroupFilterOptions = question.choices;
      });
  }

  private initFilterParams(currentDepartmentId: number): void {
    this.filterParams = new UserGroupFilterParams();
    this.filterParams.departmentIds = [];
    this.filterParams.orderBy = 'name Asc';
    this.filterParams.departmentIds.push(currentDepartmentId);
    this.filterParams.countActiveMembers = true;
  }
}
