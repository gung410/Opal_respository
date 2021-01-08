import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import {
  CSLCommunityResults,
  CollaborativeSocialLearningApiService,
  CommunityResultModel,
  OrganizationUnitLevelEnum,
  UserInfoModel,
  UserRepository
} from '@opal20/domain-api';
import { Component, Input, ViewChild } from '@angular/core';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { Observable } from 'rxjs';
import { OpalSelectComponent } from '@opal20/common-components';
import { map } from 'rxjs/operators';

@Component({
  selector: 'add-participant-dialog',
  templateUrl: './add-participant-dialog.component.html'
})
export class AddParticipantDialogComponent extends BaseComponent {
  @Input() public participantSelectedIds: string[];

  public participantSelectedUsers: UserInfoModel[] = [];
  public participantSelectedCommunities: CommunityResultModel[] = [];
  public selectedValue: string;
  public userOpt: ParticipantOption = ParticipantOption.ByUser;
  public PARTICIPANTOPTION = ParticipantOption;
  public fetchUserSelectItemFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<UserInfoModel[]>;
  public fetchCommunitySelectItemFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<CommunityResultModel[]>;
  // Filter user in opal-select by term
  public filterUserFn: (term: string, item: UserInfoModel) => boolean;
  public filterCommunityFn: (term: string, item: CommunityResultModel) => boolean;

  @ViewChild(OpalSelectComponent, { static: false }) public opalSelectComponent: OpalSelectComponent;

  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();

  public get isAdmin(): boolean {
    return this.currentUser.hasAdministratorRoles();
  }

  public get canProceed(): boolean {
    return this.participantSelectedUsers.length > 0 || this.participantSelectedCommunities.length > 0;
  }

  public get parentDepartmentId(): number {
    return this.isAdmin ? 1 : this.currentUser.departmentId;
  }

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public dialogRef: DialogRef,
    public userRepository: UserRepository,
    private cslService: CollaborativeSocialLearningApiService
  ) {
    super(moduleFacadeService);
    this.createFetchParticipantFn();
    this.createFilterParticipantFn();
    this.createFilterCommunityFn();
  }

  public onCancel(): void {
    this.dialogRef.close();
  }
  public onProceed(): void {
    if (this.userOpt === ParticipantOption.ByUser) {
      const participantSelectedIds = this.participantSelectedUsers.map(u => u.id);
      this.dialogRef.close(participantSelectedIds);
    } else {
      Promise.all(
        this.participantSelectedCommunities
          .filter(p => !Utils.isNullOrEmpty(p.guid))
          .map(m => {
            // hard code page, limit to get all of members of current community
            return this.cslService.getCommunityMembers(m.guid, 0, 100000).toPromise();
          })
      ).then(results => {
        let members = [];
        results.forEach(arr => {
          members = [
            ...members,
            ...arr.results.map(m =>
              m.user.url
                .split('/u/')[1]
                .replace('/', '')
                .toLowerCase()
            )
          ];
        });
        this.dialogRef.close(
          members
            .filter((n, i) => members.indexOf(n) === i)
            .filter(x => !this.participantSelectedIds.includes(x))
            .filter(x => x !== 'admin' && x !== this.currentUser.id)
        );
      });
    }
  }
  public removeUser(user: UserInfoModel): void {
    const index = this.participantSelectedUsers.findIndex(u => u.id === user.id);

    this.participantSelectedUsers.splice(index, 1);
  }

  public userItemChange(user: UserInfoModel): void {
    if (!this.participantSelectedUsers) {
      this.participantSelectedUsers.push(user);
    } else {
      const idx = this.participantSelectedUsers.findIndex(u => u.id === user.id);

      if (idx >= 0) {
        this.participantSelectedUsers.splice(idx, 1);
      } else {
        this.participantSelectedUsers.push(user);
      }
    }
  }

  public isUserSelected(user: UserInfoModel): boolean {
    return this.participantSelectedUsers && this.participantSelectedUsers.some(x => x.id === user.id);
  }

  public onSelectedUserItemChanged(): void {
    this.opalSelectComponent.ngSelect.handleClearClick();
  }

  public removeCommunity(community: CommunityResultModel): void {
    const index = this.participantSelectedCommunities.findIndex(c => c.guid === community.guid);

    this.participantSelectedCommunities.splice(index, 1);
  }

  public communityItemChange(community: CommunityResultModel): void {
    if (!this.participantSelectedCommunities) {
      this.participantSelectedCommunities.push(community);
    } else {
      const idx = this.participantSelectedCommunities.findIndex(c => c.guid === community.guid);

      if (idx >= 0) {
        this.participantSelectedCommunities.splice(idx, 1);
      } else {
        this.participantSelectedCommunities.push(community);
      }
    }
  }

  public isCommunitySelected(community: CommunityResultModel): boolean {
    return this.participantSelectedCommunities && this.participantSelectedCommunities.some(x => x.guid === community.guid);
  }

  public onSelectedCommunityItemChanged(): void {
    this.opalSelectComponent.ngSelect.handleClearClick();
  }

  private createFilterParticipantFn(): void {
    this.filterUserFn = (term: string, item: UserInfoModel) => {
      return item.hasValue(term, m => m.fullName) || item.hasValue(term, m => m.emailAddress);
    };
  }

  private _createFetchUserSelectFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<UserInfoModel[]> {
    return (_searchText, _skipCount, _maxResultCount) =>
      this.userRepository
        .loadUserInfoList(
          {
            parentDepartmentId: [this.parentDepartmentId],
            pageSize: _maxResultCount,
            pageIndex: _maxResultCount === 0 ? 1 : _skipCount / _maxResultCount + 1,
            filterOnSubDepartment: true,
            searchKey: _searchText,
            departmentExtIds: [
              OrganizationUnitLevelEnum.Branch,
              OrganizationUnitLevelEnum.Cluster,
              OrganizationUnitLevelEnum.Division,
              OrganizationUnitLevelEnum.Ministry,
              OrganizationUnitLevelEnum.OrganizationUnit,
              OrganizationUnitLevelEnum.School,
              OrganizationUnitLevelEnum.Wing
            ],
            userEntityStatuses: ['Active']
          },
          null,
          null,
          false
        )
        .pipe(this.untilDestroy());
  }

  private _createFetchCommunitySelectFn(): (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<CSLCommunityResults> {
    return this.isAdmin
      ? (_searchText, _skipCount, _maxResultCount) =>
          this.cslService
            .getAllCommunities(_maxResultCount === 0 ? 1 : _skipCount / _maxResultCount + 1, _maxResultCount, _searchText, false)
            .pipe(map(p => p))
            .pipe(this.untilDestroy())
      : (_searchText, _skipCount, _maxResultCount) =>
          this.cslService
            .getCommunitiesByUserId(
              this.currentUser.extId,
              _maxResultCount === 0 ? 1 : _skipCount / _maxResultCount + 1,
              _maxResultCount,
              _searchText,
              false
            )
            .pipe(map(p => p))
            .pipe(this.untilDestroy());
  }

  private createFetchParticipantFn(): void {
    const _fetchUserSelectFn = this._createFetchUserSelectFn();
    const _fetchCommunitySelectFn = this._createFetchCommunitySelectFn();

    this.fetchUserSelectItemFn = (searchText: string, skipCount: number, maxResultCount: number) =>
      _fetchUserSelectFn(searchText, skipCount, maxResultCount).pipe(
        map(p => {
          return p.filter(x => !this.participantSelectedIds.includes(x.id)).filter(x => x.id !== this.currentUser.id);
        })
      );
    this.fetchCommunitySelectItemFn = (searchText: string, skipCount: number, maxResultCount: number) =>
      _fetchCommunitySelectFn(searchText, skipCount, maxResultCount).pipe(
        map(p => {
          return p.items;
        })
      );
  }

  private createFilterCommunityFn(): void {
    this.filterCommunityFn = (term: string, item: CommunityResultModel) => {
      return item.hasValue(term, m => m.name) || item.hasValue(term, m => m.description);
    };
  }
}

enum ParticipantOption {
  ByUser = 'byUser',
  ByCommunity = 'byCommunity'
}
