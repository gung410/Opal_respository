import { BaseGridComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import {
  CalendarAccessSharingGridViewModel,
  GRID_SHARE_CALENDAR_STATUS_COLOR_MAP,
  GetCalendarAccessSharingRequest,
  SaveTeamCalendarAccessSharingsRequest,
  ShareCalendarActionsEnum,
  TaggingRepository,
  TeamCalendarSharingService,
  UserAccessSharingModel,
  UserApiService
} from '@opal20/domain-api';
import { CellClickEvent, GridDataResult } from '@progress/kendo-angular-grid';
import { map, switchMap } from 'rxjs/operators';

import { ButtonAction } from '@opal20/common-components';
import { Component } from '@angular/core';

const DEFAULT_USER_AVATAR = 'assets/images/others/default-avatar.png';
@Component({
  selector: 'share-calendar',
  templateUrl: './share-calendar.component.html'
})
export class ShareCalendarComponent extends BaseGridComponent<CalendarAccessSharingGridViewModel> {
  public actionBtnGroup: ButtonAction<CalendarAccessSharingGridViewModel>[] = [
    {
      id: 'share',
      text: this.translateCommon('Share'),
      conditionFn: dataItem => dataItem.shared === false,
      actionFn: dataItems => this.saveCalendarAccessSharings(ShareCalendarActionsEnum.Share, dataItems),
      hiddenFn: () => false
    },
    {
      id: 'unshare',
      text: this.translateCommon('Stop sharing'),
      conditionFn: dataItem => dataItem.shared === true,
      actionFn: dataItems => this.saveCalendarAccessSharings(ShareCalendarActionsEnum.Unshare, dataItems),
      hiddenFn: () => false
    }
  ];

  private calendarAccessSharingGridData: CalendarAccessSharingGridViewModel[] = [];

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public teamCalendarSharingService: TeamCalendarSharingService,
    public userService: UserApiService,
    public taggingRepository: TaggingRepository
  ) {
    super(moduleFacadeService);
  }

  public onGridCellClick(event: CellClickEvent): void {
    throw new Error('Method not implemented.');
  }

  public getStatusSharing(status: boolean): string {
    return status ? ShareCalendarActionsEnum.Share : ShareCalendarActionsEnum.Unshare;
  }

  public get getStatusSharingColorMap(): unknown {
    return GRID_SHARE_CALENDAR_STATUS_COLOR_MAP;
  }

  protected onInit(): void {
    super.onInit();
  }

  protected loadData(): void {
    const request: GetCalendarAccessSharingRequest = {
      maxResultCount: this.state.take,
      skipCount: this.state.skip
    };

    this.teamCalendarSharingService.getCalendarAccessSharings(request).subscribe(res => {
      const data = res.items.map(p => CalendarAccessSharingGridViewModel.buildGridData(p));
      this.calendarAccessSharingGridData = data;

      this.getUsersInfo(res.items);
      this.gridData = <GridDataResult>{ data: this.calendarAccessSharingGridData, total: res.totalCount };
      this.updateSelectedsAndGridData();
    });
  }

  private saveCalendarAccessSharings(action: ShareCalendarActionsEnum, dataItems: CalendarAccessSharingGridViewModel[]): Promise<boolean> {
    const request: SaveTeamCalendarAccessSharingsRequest = {
      action: action,
      userIds: dataItems.map(x => x.userId)
    };
    return this.teamCalendarSharingService.saveCalendarAccessSharings(request).then(() => {
      this.selectedItems = [];
      this.loadData();
      return true;
    });
  }

  private getUsersInfo(userAccessSharings: UserAccessSharingModel[]): void {
    const userIds = userAccessSharings.map(x => x.userId);
    this.userService
      .getPublicUserInfoList({ userIds }, true)
      .pipe(
        switchMap(userInfos => {
          return this.taggingRepository.loadAllMetaDataTags(true).pipe(
            map(metaDataTags => {
              return { metaDataTags, userInfos };
            })
          );
        })
      )
      .subscribe(res => {
        const metadataTagsDic = Utils.toDictionary(res.metaDataTags, p => p.tagId);
        res.userInfos.forEach(item => {
          const userAccessSharing = this.calendarAccessSharingGridData.find(x => x.userId === item.id);
          userAccessSharing.placeOfWork = item.departmentName;
          userAccessSharing.avatarUrl = item.avatarUrl ? item.avatarUrl : DEFAULT_USER_AVATAR;
          userAccessSharing.serviceScheme = metadataTagsDic[item.serviceScheme] ? metadataTagsDic[item.serviceScheme].displayText : '';
          userAccessSharing.developmentRole = item.developmentRoles
            .map(p => metadataTagsDic[p])
            .filter(p => p != null)
            .map(p => p.displayText)
            .join(', ');
        });
      });
  }
}
