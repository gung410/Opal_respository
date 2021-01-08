import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import {
  Course,
  ISaveMyLearningPath,
  ISaveMyLearningPathCourse,
  ISaveUserSharingDetail,
  LearnerLearningPath,
  MyLearningPathApiService,
  PublicUserInfo,
  SharingType,
  UserSharing,
  UserSharingAPIService
} from '@opal20/domain-api';
import { DialogAction, OpalDialogService, ScrollableMenu, requiredAndNoWhitespaceValidator } from '@opal20/common-components';
import { LearningPathDetailMode, LearningPathDetailViewModel, LearningPathTabInfo } from '@opal20/domain-components';

import { MyLearningPathDataService } from '../../services/my-learning-path-data.service';
import { Observable } from 'rxjs';
import { Validators } from '@angular/forms';

@Component({
  selector: 'learner-learning-path-form',
  templateUrl: 'learner-learning-path-form.component.html'
})
export class LearnerLearningPathFormComponent extends BaseFormComponent {
  @Input() public mode: LearningPathDetailMode;

  public get learningPathDetailVM(): LearningPathDetailViewModel {
    return this._learningPathDetailVM;
  }
  @Input() public set learningPathDetailVM(data: LearningPathDetailViewModel) {
    if (!Utils.isDifferent(this._learningPathDetailVM, data)) {
      return;
    }
    this._learningPathDetailVM = data;
    this.getSharedUsers();
  }
  public _learningPathDetailVM: LearningPathDetailViewModel;
  @Output() public backClick: EventEmitter<void> = new EventEmitter<void>();
  @Output() public handleActionForm: EventEmitter<void> = new EventEmitter<void>();

  @ViewChild('basicInfoTabContainer', { static: false })
  public basicInfoTabContainer: ElementRef;
  @ViewChild('pdOpportunitiesTabContainer', { static: false })
  public pdOpportunitiesTabContainer: ElementRef;
  @ViewChild('sharingTabContainer', { static: false })
  public sharingTabContainer: ElementRef;

  public tabs: ScrollableMenu[] = [
    {
      id: LearningPathTabInfo.BasicInfo,
      title: 'Basic Information',
      elementFn: () => {
        return this.basicInfoTabContainer;
      }
    },
    {
      id: LearningPathTabInfo.PdOpportunities,
      title: 'PD Opportunities',
      elementFn: () => {
        return this.pdOpportunitiesTabContainer;
      }
    },
    {
      id: LearningPathTabInfo.Sharing,
      title: 'Sharing',
      elementFn: () => {
        return this.sharingTabContainer;
      }
    }
  ];
  public activeTab: string = LearningPathTabInfo.BasicInfo;
  public loadingData: boolean = false;

  public sharedUsers: PublicUserInfo[] = [];
  public originSharedUsers: PublicUserInfo[] = [];

  private sharingId: string;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private myLearningPathApiService: MyLearningPathApiService,
    private myLearningPathDataService: MyLearningPathDataService,
    private userSharingAPIService: UserSharingAPIService,
    private opalDialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
    this.learningPathDetailVM = new LearningPathDetailViewModel();
  }

  public asViewMode(mode: LearningPathDetailMode): boolean {
    return mode === LearningPathDetailMode.View;
  }

  public get formTitle(): string {
    return this.mode === LearningPathDetailMode.NewLearningPath ? 'Create new Learning Path' : this.learningPathDetailVM.title;
  }

  public onBackClick(): void {
    if (!this.hasSharedUsersChanged() && !this.learningPathDetailVM.dataHasChanged()) {
      this.backClick.emit();
      return;
    }
    const confirmMsg =
      this.mode === LearningPathDetailMode.NewLearningPath
        ? 'Do you want to cancel creating new learning path?'
        : 'Do you want to cancel updating this learning path?';
    this.opalDialogService
      .openConfirmDialog({
        yesBtnText: 'Yes',
        noBtnText: 'No',
        confirmMsg: confirmMsg
      })
      .subscribe(result => {
        if (result === DialogAction.OK) {
          this.backClick.emit();
        }
      });
  }

  public onHandleActionForm(): void {
    this.validate().then(val => {
      if (!val) {
        return;
      }
      const requestModel = this.createSaveLearningPathModel();
      this.onSavingData(requestModel).then(response => {
        this.handleActionForm.emit();
      });
    });
  }

  public onDeleteClicked(): void {
    this.opalDialogService
      .openConfirmDialog({
        yesBtnText: 'Yes',
        noBtnText: 'No',
        confirmMsg: 'Do you want to delete this learning path?'
      })
      .subscribe(result => {
        if (result === DialogAction.OK) {
          this.onDelete();
        }
      });
  }

  public onDelete(): void {
    const learningPathId = this.learningPathDetailVM.id;
    this.myLearningPathApiService.deleteLearningPathById(learningPathId).then(() => {
      this.handleActionForm.emit();
      this.showNotification('Deleted successfully');
    });
  }

  public get isDisabledYesBtn(): boolean {
    return (
      this.form.invalid ||
      this.learningPathDetailVM.listCourses.length === 0 ||
      (!this.hasSharedUsersChanged() && !this.learningPathDetailVM.dataHasChanged())
    );
  }

  public get isEditMode(): boolean {
    return this.mode === LearningPathDetailMode.Edit;
  }

  public fetchUsersFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<PublicUserInfo[]> = (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => this.myLearningPathDataService.searchUsers({ searchText, maxResultCount, skipCount });

  public fetchPublishedCoursesFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<Course[]> = (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => this.myLearningPathDataService.searchCourses(searchText, skipCount, maxResultCount);

  public hasSharedUsersChanged(): boolean {
    return Utils.isDifferent(this.originSharedUsers, this.sharedUsers);
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'basic-info',
      controls: {
        title: {
          validators: [
            {
              validator: requiredAndNoWhitespaceValidator(),
              validatorType: 'required'
            },
            {
              validator: Validators.maxLength(2000)
            }
          ]
        },
        thumbnailUrl: {}
      }
    };
  }

  private getSharedUsers(): void {
    if (!this.learningPathDetailVM.id) {
      return;
    }
    this.userSharingAPIService
      .getUserSharingByItemId(this.learningPathDetailVM.id)
      .toPromise()
      .then(response => {
        if (Utils.isEmpty(response)) {
          // Call create new userSharing if It's not create before
          this.createUserSharing(this.learningPathDetailVM.id).then(userSharing => {
            this.sharingId = userSharing.id;
          });
          return;
        }
        this.sharingId = response.id;
        if (!response.users.length) {
          return;
        }
        const userIds = response.users.map(user => user.userId);
        const followingUserIds = response.users.filter(user => user.isFollowing).map(user => user.userId);
        this.myLearningPathDataService
          .loadPublicUsers(userIds, followingUserIds)
          .pipe(this.untilDestroy())
          .subscribe(users => {
            if (!users || !users.length) {
              return;
            }
            this.sharedUsers = users;
            this.originSharedUsers = Utils.cloneDeep(this.sharedUsers);
          });
      });
  }

  private onSavingData(request: ISaveMyLearningPath): Promise<LearnerLearningPath> {
    return this.mode === LearningPathDetailMode.NewLearningPath
      ? this.myLearningPathApiService.createMyLearningPath(request).then(value => {
          return this.createUserSharing(value.id).then(() => {
            this.showNotification('Created successfully');
            return value;
          });
        })
      : this.myLearningPathApiService.updateMyLearningPath(request).then(value => {
          if (this.hasSharedUsersChanged()) {
            return this.updateUserSharing().then(() => {
              this.showNotification('Updated successfully');
              return value;
            });
          }
          this.showNotification('Updated successfully');
          return value;
        });
  }

  private createSaveLearningPathModel(): ISaveMyLearningPath {
    const listCourses: ISaveMyLearningPathCourse[] = this.learningPathDetailVM.listCourses.map(c => {
      return {
        id: c.id,
        courseId: c.courseId,
        order: c.order,
        learningPathId: this.learningPathDetailVM.id
      };
    });
    const model: ISaveMyLearningPath = {
      id: this.learningPathDetailVM.id,
      title: this.learningPathDetailVM.title,
      thumbnailUrl: this.learningPathDetailVM.thumbnailUrl,
      courses: listCourses
    };

    return model;
  }

  private updateUserSharing(): Promise<void> {
    const userIds: ISaveUserSharingDetail[] = this.sharedUsers.map(user => {
      return {
        userId: user.id
      };
    });
    return this.userSharingAPIService
      .updateUserSharing({
        id: this.sharingId,
        itemId: this.learningPathDetailVM.id,
        itemType: SharingType.LearningPath,
        usersShared: userIds
      })
      .then();
  }

  private createUserSharing(learningPathId: string): Promise<UserSharing> {
    const userIds: ISaveUserSharingDetail[] = this.sharedUsers.map(user => {
      return {
        userId: user.id
      };
    });
    return this.userSharingAPIService.createUserSharing({
      itemId: learningPathId,
      itemType: SharingType.LearningPath,
      usersShared: userIds
    });
  }
}
