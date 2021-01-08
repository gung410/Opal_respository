import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import {
  CxGlobalLoaderService,
  CxSurveyjsComponent,
  CxSurveyjsEventModel,
  CxSurveyJsUtil,
} from '@conexus/cx-angular-common';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { ObjectUtilities } from 'app-utilities/object-utils';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';
import { AgeGroupTextConst } from 'app/shared/constants/age-group.enum';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { SurveyModel } from 'survey-angular';
import { GroupFilterConst } from '../../../../shared/constants/filter-group.constant';
import { FilterParamModel } from '../models/filter-param.model';
import {
  AppliedFilterModel,
  FilterModel,
  ObjectData,
} from '../models/filter.model';
import { UserEntityStatusConst } from '../models/user-accounts.model';
import { AppliedFilterFormJSON, FilterFormJSON } from './filter-form-surveyjs';
import { StaffFilterService } from './staff-filter.service';

@Component({
  selector: 'staff-filter',
  templateUrl: './staff-filter.component.html',
  styleUrls: ['./staff-filter.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StaffFilterComponent extends BaseSmartComponent implements OnInit {
  currentLanguage: string;
  filterFormJSON: any = FilterFormJSON;
  appliedFilterFormJSON: any = AppliedFilterFormJSON;
  appliedData: FilterModel = new FilterModel();
  statusTypeInfos: any;
  filterData: {
    value: number;
    text: string;
  }[] = [
    {
      value: 1,
      text: 'Manually created user account',
    },
    {
      value: 2,
      text: 'Synchronized HR user account',
    },
  ];
  @ViewChild('filterSurveyJs', { static: true })
  filterSurveyJs: CxSurveyjsComponent;
  @ViewChild('appliedFilterSurveyJs', { static: true })
  appliedFilterSurveyJs: CxSurveyjsComponent;
  @Input() set choicesData(data: any) {
    this._choicesData = data;
    if (data.statusTypeInfos) {
      CxSurveyJsUtil.addProperty(
        this.filterFormJSON,
        'lnaStatus',
        'choices',
        data.statusTypeInfos.LearningNeed
          ? data.statusTypeInfos.LearningNeed
          : []
      );
      CxSurveyJsUtil.addProperty(
        this.filterFormJSON,
        'pdpStatus',
        'choices',
        data.statusTypeInfos.LearningPlan
          ? data.statusTypeInfos.LearningPlan
          : []
      );
    }
    this.filterFormJSON = { ...this.filterFormJSON };
  }
  get choicesData(): any {
    return this._choicesData;
  }
  @Input() adjustAppliedData: any;
  @Input() currentDepartmentId: number[];
  @Output() applyClick: EventEmitter<any> = new EventEmitter<any>();
  @Output() cancel: EventEmitter<any> = new EventEmitter<any>();
  private filteredParams: FilterParamModel;
  private _choicesData: any;
  private _approvalGroupFilterOptions: ObjectData[];
  private _userGroupFilterOptions: ObjectData[];

  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private translateAdapterService: TranslateAdapterService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private staffFilterService: StaffFilterService
  ) {
    super(changeDetectorRef);
    this.currentLanguage = this.translateAdapterService.getCurrentLanguage();
  }

  ngOnInit(): void {
    this.initFilterData();
    if (this.adjustAppliedData) {
      this.appliedData = { ...this.adjustAppliedData };
      this.changeDetectorRef.detectChanges();
    }
  }

  initFilterData(): void {}

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
      /** In case, the user don't choose From field and To field,Then messages default appear
       * The block below remove message default after the user chooses one of both field
       */
      if (
        creationDateFrom.value !== undefined &&
        creationDateFrom.value !== ''
      ) {
        creationDateFrom.hasErrors(true);
      }
      if (creationDateTo.value !== undefined && creationDateTo.value !== '') {
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
      /** In case, the user don't choose From field and To field,Then messages default appear
       * The block below remove message default after the user chooses one of both field
       */
      if (
        expirationDateFrom.value !== undefined &&
        expirationDateFrom.value !== ''
      ) {
        expirationDateFrom.hasErrors(true);
      }
      if (
        expirationDateTo.value !== undefined &&
        expirationDateTo.value !== ''
      ) {
        expirationDateTo.hasErrors(true);
      }

      return;
    }
    if (
      data.options.question.name === 'acknowledgedLNADateTo' ||
      data.options.question.name === 'acknowledgedLNADateFrom'
    ) {
      const acknowledgedLNADateTo: any = data.survey.getQuestionByName(
        'acknowledgedLNADateTo'
      );
      const acknowledgedLNADateFrom: any = data.survey.getQuestionByName(
        'acknowledgedLNADateFrom'
      );
      if (
        acknowledgedLNADateFrom.value !== undefined &&
        acknowledgedLNADateFrom.value !== ''
      ) {
        acknowledgedLNADateFrom.hasErrors(true);
      }
      if (
        acknowledgedLNADateTo.value !== undefined &&
        acknowledgedLNADateTo.value !== ''
      ) {
        acknowledgedLNADateTo.hasErrors(true);
      }
    }
    if (
      data.options.question.name === 'acknowledgedPDPlanDateTo' ||
      data.options.question.name === 'acknowledgedPDPLanDateFrom'
    ) {
      const acknowledgedPDPlanDateTo: any = data.survey.getQuestionByName(
        'acknowledgedPDPlanDateTo'
      );
      const acknowledgedPDPLanDateFrom: any = data.survey.getQuestionByName(
        'acknowledgedPDPLanDateFrom'
      );
      if (
        acknowledgedPDPLanDateFrom.value !== undefined &&
        acknowledgedPDPLanDateFrom.value !== ''
      ) {
        acknowledgedPDPLanDateFrom.hasErrors(true);
      }
      if (
        acknowledgedPDPlanDateTo.value !== undefined &&
        acknowledgedPDPlanDateTo.value !== ''
      ) {
        acknowledgedPDPlanDateTo.hasErrors(true);
      }
    }
    data.options.question.hasErrors(true);
  }

  onAddNewFilter(event: CxSurveyjsEventModel): void {
    if (!event.options.allowComplete) {
      return;
    }
    event.options.allowComplete = false;
    this.filterFormJSON = { ...this.filterFormJSON };
    if (ObjectUtilities.isEmpty(event.survey.data)) {
      return;
    }
    this.convertDataToDisplay(event.survey.data);
    this.changeDetectorRef.detectChanges();
  }

  onApplyFilter(data: FilterModel): void {
    this.applyClick.emit({ filteredData: data });
  }

  onChanged(data: CxSurveyjsEventModel): void {
    this.appliedData.appliedFilter = data.options.value;
  }

  convertDataToDisplay(filterData: any): void {
    let appliedDataWithOption;
    const appliedOptionIndex = this.appliedData.appliedFilter.findIndex(
      (item) => item.filterOptions.data === filterData.filterOptions.data
    );
    if (appliedOptionIndex > -1) {
      appliedDataWithOption = this.appliedData.appliedFilter[
        appliedOptionIndex
      ];
    } else {
      appliedDataWithOption = new AppliedFilterModel({
        filterOptions: filterData.filterOptions,
        data: new ObjectData({
          value: [],
          text: '',
        }),
      });
    }

    let optionFiltered: object;

    let optionData = appliedDataWithOption.data;
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
      case GroupFilterConst.ACCOUNT_TYPE:
        optionData = this.mapData(
          optionData,
          filterData.accountType,
          (item) => {
            return UserEntityStatusConst.find((status) => status.key === item)
              .description;
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
        optionData.value = filterData.designation;
        optionData.text = filterData.designation;
        break;
      case GroupFilterConst.LNA_STATUS:
        optionData = this.mapData(optionData, filterData.lnaStatus, (item) => {
          return this.choicesData.statusTypeInfos.LearningNeed.find(
            (status) => status.value.toString() === item.toString()
          ).text;
        });
        break;
      case GroupFilterConst.PDP_STATUS:
        optionData = this.mapData(optionData, filterData.pdpStatus, (item) => {
          return this.choicesData.statusTypeInfos.LearningPlan.find(
            (status) => status.value.toString() === item.toString()
          ).text;
        });
        break;
      case GroupFilterConst.USER_GROUP:
        optionData = this.mapData(optionData, filterData.userGroup, (item) => {
          return this._userGroupFilterOptions.find(
            (usr) => usr.value.toString() === item.toString()
          ).text;
        });
        break;
      case GroupFilterConst.APPROVAL_GROUP:
        optionData = this.mapData(
          optionData,
          filterData.approvalGroup,
          (item) => {
            return this._approvalGroupFilterOptions.find(
              (usr) => usr.value.toString() === item.toString()
            ).text;
          }
        );
        break;
      case GroupFilterConst.LNA_ACKNOWLEDGED_PERIOD:
        optionFiltered = this.addFilterDate(
          filterData.acknowledgedLNADateFrom,
          filterData.acknowledgedLNADateTo
        );
        optionData = { ...optionData, ...optionFiltered };
        break;
      case GroupFilterConst.PD_PLAN_ACKNOWLEDGED_PERIOD:
        optionFiltered = this.addFilterDate(
          filterData.acknowledgedPDPlanDateFrom,
          filterData.acknowledgedPDPlanDateTo
        );
        optionData = { ...optionData, ...optionFiltered };
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

  private mapData(optionData: any, filterData: any, mapExpression: any): any {
    const dataTemp = this.unionArray(optionData.value, filterData);

    return {
      ...optionData,
      value: dataTemp,
      text: dataTemp.map(mapExpression).join(', '),
    };
  }

  private addFilterDate(from: any, to: any): object {
    return {
      text: from + ' - ' + to,
      value: [from, to],
    };
  }

  private handleChangingFilterOptions(
    survey: SurveyModel,
    filterOptionQuestion: any
  ): void {
    if (
      !filterOptionQuestion ||
      !filterOptionQuestion.value ||
      !filterOptionQuestion.value.data
    ) {
      return;
    }
    if (filterOptionQuestion.value.data === GroupFilterConst.APPROVAL_GROUP) {
      this.loadApprovingOfficerOptions(survey);
    } else if (
      filterOptionQuestion.value.data === GroupFilterConst.USER_GROUP
    ) {
      this.loadUserGroupOptions(survey);
    }
  }

  private loadUserGroupOptions(survey: SurveyModel): void {
    this.cxGlobalLoaderService.showLoader();
    this.staffFilterService
      .getUserGroupFilterOptions()
      .subscribe((userGroupFilterOptions) => {
        const question = survey.getQuestionByName('userGroup');
        if (question) {
          question.choices = userGroupFilterOptions;
        }
        this._userGroupFilterOptions = userGroupFilterOptions;
        this.cxGlobalLoaderService.hideLoader();
      });
  }

  private loadApprovingOfficerOptions(survey: SurveyModel): void {
    this.cxGlobalLoaderService.showLoader();
    this.staffFilterService
      .getApprovingOfficerFilterOptions()
      .subscribe((approvalGroupFilterOptions) => {
        const question = survey.getQuestionByName('approvalGroup');
        if (question) {
          question.choices = approvalGroupFilterOptions;
        }
        this._approvalGroupFilterOptions = approvalGroupFilterOptions;
        this.cxGlobalLoaderService.hideLoader();
      });
  }
}
