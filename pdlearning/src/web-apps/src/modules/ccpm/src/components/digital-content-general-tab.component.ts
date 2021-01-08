import {
  ADMINISTRATOR_ROLES,
  BaseUserInfo,
  SystemRoleEnum,
  UserInfoModel,
  UserRepository,
  UserRepositoryContext,
  UserUtils
} from '@opal20/domain-api';
import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { DigitalContentDetailMode, DigitalContentDetailViewModel } from '@opal20/domain-components';

import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'digital-content-general-tab',
  templateUrl: './digital-content-general-tab.component.html'
})
export class DigitalContentGeneralTabComponent extends BaseComponent {
  public DigitalContentDetailMode: typeof DigitalContentDetailMode = DigitalContentDetailMode;

  public approvalOfficerRoles: SystemRoleEnum[] = [
    SystemRoleEnum.MOEHQContentApprovingOfficer,
    SystemRoleEnum.SchoolContentApprovingOfficer,
    ...ADMINISTRATOR_ROLES
  ];

  @Input('form') public form: FormGroup;
  @Input() public set contentViewModel(v: DigitalContentDetailViewModel) {
    if (!v) {
      return;
    }
    this._contentViewModel = v;
    this.createFetchAlternativeApprovalOfficersFn();
    this.createFetchPrimaryApprovalOfficersFn();
  }

  public get contentViewModel(): DigitalContentDetailViewModel {
    return this._contentViewModel;
  }

  @Input() public mode: DigitalContentDetailMode;

  public usersDicById: Dictionary<BaseUserInfo> = {};
  public _contentViewModel: DigitalContentDetailViewModel;

  // Filter user in opal-select by term
  public filterApprovalOfficersFn: (term: string, item: BaseUserInfo) => boolean;

  // Fetch user by from API by term
  public fetchAlternativeApprovalOfficersFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]>;
  public fetchPrimaryApprovalOfficersFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]>;

  // Fetch user by from API by userIds
  public fetchApprovalOfficersByIdsFn: (userIds: string[]) => Observable<BaseUserInfo[]>;

  // Internal component variables //
  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private userRepositoryContext: UserRepositoryContext,
    private userRepository: UserRepository
  ) {
    super(moduleFacadeService);

    this.fetchApprovalOfficersByIdsFn = UserUtils.createFetchUsersByIdsFn(this.userRepository);
    this.createFilterFn();
  }

  public createFetchAlternativeApprovalOfficersFn(): void {
    const _fetchAaoFn = UserUtils.createFetchUsersFn(this.approvalOfficerRoles, this.userRepository);

    this.fetchAlternativeApprovalOfficersFn = (searchText: string, skipCount: number, maxResultCount: number) =>
      _fetchAaoFn(searchText, skipCount, maxResultCount).pipe(
        map(p => p.filter(x => x.id !== this.currentUser.extId).filter(x => x.id !== this.contentViewModel.primaryApprovingOfficerId))
      );
  }

  public createFetchPrimaryApprovalOfficersFn(): void {
    const _fetchPaoFn = UserUtils.createFetchUsersFn(this.approvalOfficerRoles, this.userRepository);

    this.fetchPrimaryApprovalOfficersFn = (searchText: string, skipCount: number, maxResultCount: number) =>
      _fetchPaoFn(searchText, skipCount, maxResultCount).pipe(
        map(p => p.filter(x => x.id !== this.currentUser.extId).filter(x => x.id !== this.contentViewModel.alternativeApprovingOfficerId))
      );
  }

  public onPublishDateInputChange(date: Date): void {
    this.contentViewModel.autoPublishDate = date;
  }

  protected onInit(): void {
    this.subscribeUserInfoList();
  }

  private subscribeUserInfoList(): void {
    this.subscribe(this.userRepositoryContext.baseUserInfoSubject, usersDicById => {
      this.usersDicById = usersDicById;
    });
  }

  private createFilterFn(): void {
    this.filterApprovalOfficersFn = (term: string, item: BaseUserInfo) =>
      term
        ? item.fullName.toLowerCase().includes(term.toLowerCase()) ||
          item.emailAddress.toLowerCase().includes(term.toLowerCase()) ||
          item.departmentName.toLowerCase().includes(term.toLowerCase())
        : true;
  }
}
