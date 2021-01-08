import { BaseComponent, MAX_INT, ModuleFacadeService } from '@opal20/infrastructure';
import { BaseUserInfo, UserRepository, UserUtils } from '@opal20/domain-api';
import { ClassRunDetailMode, showClassRunDetailViewOnly } from '../../models/classrun-detail-mode.model';
import { Component, Input } from '@angular/core';

import { ClassRunDetailViewModel } from './../../view-models/classrun-detail-view.model';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'classrun-planning-tab',
  templateUrl: './classrun-planning-tab.component.html'
})
export class ClassRunPlanningTabComponent extends BaseComponent {
  @Input() public form: FormGroup;
  @Input() public classRun: ClassRunDetailViewModel;
  @Input() public mode: ClassRunDetailMode | undefined;
  public fetchClassRunFacilitatorItemsFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]>;
  public fetchClassRunCoFacilitatorItemsFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]>;
  public fetchUserItemsByIdsFn: (ids: string[]) => Observable<BaseUserInfo[]>;
  public ignoreClassRunFacilitatorItemFn: (item: BaseUserInfo) => boolean;
  public ignoreClassRunCoFacilitatorItemFn: (item: BaseUserInfo) => boolean;

  public maxInt: number = MAX_INT;
  public ClassRunDetailMode: typeof ClassRunDetailMode = ClassRunDetailMode;

  constructor(public moduleFacadeService: ModuleFacadeService, private userRepository: UserRepository) {
    super(moduleFacadeService);

    this.fetchClassRunFacilitatorItemsFn = this._createFetchUserSelectItemFn(ClassRunDetailViewModel.classRunFacilitatorItemsPermissions);
    this.fetchClassRunCoFacilitatorItemsFn = this._createFetchUserSelectItemFn(ClassRunDetailViewModel.classRunFacilitatorItemsPermissions);

    this.fetchUserItemsByIdsFn = this._createFetchUserItemsByIdsFn();

    this.ignoreClassRunFacilitatorItemFn = x => x.id === this.classRun.facilitatorId;
    this.ignoreClassRunCoFacilitatorItemFn = x => x.id === this.classRun.coFacilitatorId;
  }

  public showClassRunDetailViewOnly(): boolean {
    return showClassRunDetailViewOnly(this.mode);
  }

  private _createFetchUserSelectItemFn(
    hasPermissions?: string[],
    mapFn?: (data: BaseUserInfo[]) => BaseUserInfo[]
  ): (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]> {
    const createFetchUsersFn = UserUtils.createFetchUsersByPermissionsFn(hasPermissions, this.userRepository);
    return (searchText: string, skipCount: number, maxResultCount: number) =>
      createFetchUsersFn(searchText, skipCount, maxResultCount).pipe(
        map(users => {
          if (mapFn) {
            return mapFn(users);
          }
          return users;
        })
      );
  }

  private _createFetchUserItemsByIdsFn(): (ids: string[]) => Observable<BaseUserInfo[]> {
    return UserUtils.createFetchUsersByIdsFn(this.userRepository, false, ['All']);
  }
}
