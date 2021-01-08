import { AddingParticipantFormViewModel, IAddingParticipantFormViewModelData } from '../models/adding-participant-form-view.model';
import {
  BaseFormComponent,
  ComponentType,
  IContainFilter,
  IFilter,
  IFormBuilderDefinition,
  IGridFilter,
  ModuleFacadeService,
  NotificationType,
  Utils
} from '@opal20/infrastructure';
import {
  CAMRoutePaths,
  CAMTabConfiguration,
  ListRegistrationGridDisplayColumns,
  NavigationPageService,
  RegistrationFilterComponent,
  RegistrationFilterModel,
  RegistrationViewModel,
  RouterPageInput
} from '@opal20/domain-components';
import {
  ClassRun,
  ClassRunRepository,
  ClassRunStatus,
  CourseRepository,
  CourseUser,
  DepartmentInfoModel,
  IAddParticipantsRequest,
  IAddParticipantsResult,
  OrganizationRepository,
  PublicUserInfo,
  RegistrationRepository,
  SearchClassRunType,
  SearchRegistrationsType,
  SelectLearnerType,
  TargetParticipantType,
  UserRepository,
  organizationUnitLevelConst
} from '@opal20/domain-api';
import { Component, Input } from '@angular/core';
import { Observable, Subscription, from } from 'rxjs';

import { CourseDetailPageInput } from '../models/course-detail-page-input.model';
import { SearchLearnerProfileType } from '../models/search-learner-profile-type.model';
import { Validators } from '@angular/forms';
import { map } from 'rxjs/operators';
import { requiredIfValidator } from '@opal20/common-components';

@Component({
  selector: 'adding-participant-page',
  templateUrl: './adding-participant-page.component.html'
})
export class AddingParticipantPageComponent extends BaseFormComponent {
  @Input() public stickyDependElement: HTMLElement;

  public filterPopupContent: ComponentType<RegistrationFilterComponent> = RegistrationFilterComponent;
  public addingParticipantVM: AddingParticipantFormViewModel = new AddingParticipantFormViewModel();

  public get courseDetailPageInput(): RouterPageInput<CourseDetailPageInput, CAMTabConfiguration, CAMTabConfiguration> | undefined {
    return this._courseDetailPageInput;
  }

  @Input()
  public set courseDetailPageInput(v: RouterPageInput<CourseDetailPageInput, CAMTabConfiguration, CAMTabConfiguration> | undefined) {
    if (Utils.isDifferent(this._courseDetailPageInput, v) && v) {
      this._courseDetailPageInput = v;
      if (this.initiated) {
        this.loadData();
      }
    }
  }

  public userIds?: string[];
  public departmentIds?: number;
  public searchText: string = '';
  public filterData: RegistrationFilterModel = null;
  public includeChildren?: boolean;
  public includeDepartmentType?: boolean;
  public departmentTypeIds?: number[];
  public TargetParticipantType: typeof TargetParticipantType = TargetParticipantType;
  public SelectLearnerType: typeof SelectLearnerType = SelectLearnerType;
  public SearchLearnerProfileType: typeof SearchLearnerProfileType = SearchLearnerProfileType;
  public CAMTabConfiguration: typeof CAMTabConfiguration = CAMTabConfiguration;
  public searchRegistrationsType: typeof SearchRegistrationsType = SearchRegistrationsType;
  public filter: IGridFilter = {
    search: '',
    filter: null
  };
  public displayColumnsAddParticipants: ListRegistrationGridDisplayColumns[] = [
    ListRegistrationGridDisplayColumns.name,
    ListRegistrationGridDisplayColumns.organisation,
    ListRegistrationGridDisplayColumns.designation,
    ListRegistrationGridDisplayColumns.addedToClassRun,
    ListRegistrationGridDisplayColumns.addedDate,
    ListRegistrationGridDisplayColumns.status
  ];
  public fetchUserSelectItemFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<CourseUser[]>;
  public fetchDepartmentSelectItemFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]>;
  public fetchClassRunSelectItemFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<ClassRun[]>;
  public loadingData: boolean = false;

  private _courseDetailPageInput: RouterPageInput<CourseDetailPageInput, CAMTabConfiguration, CAMTabConfiguration> | undefined;
  private _loadDataSub: Subscription = new Subscription();
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public classRunRepository: ClassRunRepository,
    public registrationRepository: RegistrationRepository,
    public courseRepository: CourseRepository,
    public organizationRepository: OrganizationRepository,
    private navigationPageService: NavigationPageService,
    private userRepository: UserRepository
  ) {
    super(moduleFacadeService);
    this.fetchUserSelectItemFn = this._createFetchUserSelectItemFn();
    this.fetchDepartmentSelectItemFn = this._createFetchDepartmentSelectItemFn();
    this.fetchClassRunSelectItemFn = this._createFetchClassRunSelectItemFn();
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();
    if (this.courseDetailPageInput.data.id == null) {
      return;
    }

    const courseObs = this.courseRepository.loadCourse(this.courseDetailPageInput.data.id);
    this.loadingData = true;
    this._loadDataSub.unsubscribe();
    this._loadDataSub = courseObs.pipe(this.untilDestroy()).subscribe(
      course => {
        if (this.loadingData) {
          this.addingParticipantVM = new AddingParticipantFormViewModel(
            <IAddingParticipantFormViewModelData>{
              course: course
            },
            selectedUsers => this.loadUserInfo(selectedUsers)
          );
        } else {
          this.addingParticipantVM.course = course;
        }
        this.loadingData = false;
      },
      error => {
        this.loadingData = false;
      }
    );
  }

  public onSubmitSearch(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText
    };
  }

  public loadUserInfo(selectedUsers: CourseUser[]): Observable<PublicUserInfo[]> {
    return this.userRepository.loadPublicUserInfoList({ userIds: selectedUsers.map(i => i.id) }).pipe(this.untilDestroy());
  }

  public onSubmitBtnClicked(): void {
    this.validateAndSubmitParticipants().subscribe(data => {
      this.addingParticipantVM.resetForm();
      if (data.numberOfAddedParticipants < data.totalNumberOfUsers) {
        this.showNotification(
          this.translate(
            'Some learners were not successfully added due to reaching maximum class size or conflicting registration/nomination.'
          ),
          NotificationType.Warning
        );
      } else {
        this.showNotification();
      }
    });
  }

  public submitParticipants(): Promise<IAddParticipantsResult> {
    return new Promise((resolve, reject) => {
      const request: IAddParticipantsRequest = {
        courseId: this.courseDetailPageInput.data.id,
        classRunId: this.addingParticipantVM.selectedClassRunId,
        userIds:
          this.addingParticipantVM.selectLearnerType === SelectLearnerType.SelectIndividualLearners
            ? this.addingParticipantVM.selectedLearners.map(item => item.id)
            : null,
        departmentIds:
          this.addingParticipantVM.selectLearnerType === SelectLearnerType.SelectDepartments
            ? this.addingParticipantVM.selectedLearnerDepartments.map(item => item.id)
            : null,
        followCourseTargetParticipant:
          this.addingParticipantVM.targetParticipantType === TargetParticipantType.FollowingCourseTargetParticipant
      };
      this.registrationRepository.addParticipants(request).then(data => {
        resolve(data);
      }, reject);
    });
  }

  public validateAndSubmitParticipants(): Observable<IAddParticipantsResult> {
    return from(
      new Promise<IAddParticipantsResult>((resolve, reject) => {
        this.validate().then(valid => {
          if (valid) {
            this.submitParticipants().then(resolve, reject);
          } else {
            reject();
          }
        });
      })
    );
  }

  public onViewRegistration(dataItem: RegistrationViewModel, activeTab: CAMTabConfiguration, searchType: SearchLearnerProfileType): void {
    this.navigationPageService.navigateTo(
      CAMRoutePaths.LearnerProfilePage,
      {
        activeTab: CAMTabConfiguration.PersonalInfoTab,
        data: {
          registrationId: dataItem.id,
          userId: dataItem.userId,
          courseId: dataItem.courseId,
          classRunId: dataItem.classRunId,
          searchType: searchType
        }
      },
      this.courseDetailPageInput
    );
  }

  public onApplyFilter(data: RegistrationFilterModel): void {
    this.filterData = data;

    this.filter = {
      ...this.filter,
      filter: this.filterData ? this.filterData.convert() : null
    };
  }

  public resetFilter(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText
    };
  }

  protected onInit(): void {
    this.loadData();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      validateByGroupControlNames: [],
      controls: {
        selectedClassRunId: {
          defaultValue: null,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        },
        selectedToAddLearnerId: {
          defaultValue: null,
          validators: [
            {
              validator: requiredIfValidator(() => this.addingParticipantVM.isSelectedAddLearner()),
              validatorType: 'required'
            }
          ]
        },
        selectedToAddLearnerDepartment: {
          defaultValue: null,
          validators: [
            {
              validator: requiredIfValidator(() => this.addingParticipantVM.isSelectedAddLearner()),
              validatorType: 'required'
            }
          ]
        }
      }
    };
  }

  private _createFetchClassRunSelectItemFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<ClassRun[]> {
    return (searchText: string, skipCount: number, maxResultCount: number) => {
      return this.classRunRepository
        .loadClassRunsByCourseId(
          this.courseDetailPageInput.data.id,
          SearchClassRunType.Owner,
          searchText,
          <IFilter>{
            containFilters: <IContainFilter[]>[
              {
                field: 'Status',
                values: [ClassRunStatus.Published],
                notContain: false
              }
            ]
          },
          this.addingParticipantVM.course.isELearning() ? false : true,
          this.addingParticipantVM.course.isELearning() ? true : false,
          skipCount,
          maxResultCount,
          null,
          false
        )
        .pipe(map(data => data.items));
    };
  }

  private _createFetchUserSelectItemFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<CourseUser[]> {
    return (searchText, skipCount, maxResultCount) => {
      return this.courseRepository
        .loadSearchCourseUsers(
          {
            searchText: searchText,
            skipCount: skipCount,
            maxResultCount: maxResultCount,
            forCourse: {
              courseId: this.courseDetailPageInput.data.id,
              followCourseTargetParticipant:
                this.addingParticipantVM.targetParticipantType === TargetParticipantType.FollowingCourseTargetParticipant
            }
          },
          false
        )
        .pipe(map(_ => _.items));
    };
  }

  private _createFetchDepartmentSelectItemFn(): (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<DepartmentInfoModel[]> {
    return (_searchText, _skipCount, _maxResultCount) => {
      return this.organizationRepository
        .loadDepartmentInfoList(
          {
            departmentId: 1,
            includeChildren: true,
            includeDepartmentType: true,
            departmentTypeIds: [
              organizationUnitLevelConst.Division,
              organizationUnitLevelConst.Branch,
              organizationUnitLevelConst.Cluster,
              organizationUnitLevelConst.School
            ]
          },
          false
        )
        .pipe(
          map(_ => {
            if (this.addingParticipantVM.targetParticipantType === TargetParticipantType.FollowingCourseTargetParticipant) {
              const allDepartmentsDic = Utils.toDictionary(_, p => p.id);
              return this.addingParticipantVM.course
                .getTargetParticipantDepartmentIds()
                .map(id => allDepartmentsDic[id])
                .filter(department => department != null);
            }
            return _;
          })
        );
    };
  }
}
