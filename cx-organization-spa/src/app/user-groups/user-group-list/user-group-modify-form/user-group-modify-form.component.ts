import { Component, Input, OnInit, ViewChild } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  Validators
} from '@angular/forms';
import { MatTabGroup } from '@angular/material/tabs';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { Utils } from 'app-utilities/utils';
import { noContentWhiteSpaceValidator } from 'app/shared/validators/no-content-white-space-validator';
import { UserManagement } from 'app/user-accounts/models/user-management.model';
import { UserGroupsDataService } from 'app/user-groups/user-groups-data.service';

@Component({
  selector: 'user-group-modify-form',
  templateUrl: './user-group-modify-form.component.html',
  styleUrls: ['./user-group-modify-form.component.scss']
})
export class UserGroupModifyFormComponent implements OnInit {
  @ViewChild(MatTabGroup) modifyGrid: MatTabGroup;

  @Input() selectedUserGroupId: number = null;
  @Input() name: string = '';
  @Input() description: string = '';

  get modifyUserGroupTitle(): AbstractControl {
    return this.modifyUserGroupForm.get('name');
  }

  get modifyUserGroupDescription(): AbstractControl {
    return this.modifyUserGroupForm.get('description');
  }

  readonly DEFAULT_MAX_LENGTH: number = 256;

  modifyUserGroupForm: FormGroup;
  isMissingNameField: boolean = false;
  isEditMode: boolean = false;

  selectedUsers: UserManagement[] = [];
  removedUsers: UserManagement[] = [];
  belongingGroupUserIds: string[] = null;

  dialogTitle: string = '';
  firstTabLabel: string = '';
  secondTabLabel: string = '';

  private modifyUserGroup: IModifyUserGroupRequest = {
    name: '',
    description: ''
  };

  constructor(
    private fb: FormBuilder,
    private activeModal: NgbActiveModal,
    private userGroupsDataService: UserGroupsDataService,
    private translateAdapterService: TranslateAdapterService
  ) {}
  ngOnInit(): void {
    this.isEditMode = this.selectedUserGroupId != null;
    this.modifyUserGroup.name = this.name;
    this.modifyUserGroup.description = this.description;
    this.dialogTitle = this.isEditMode
      ? this.getImmediatelyLanguage(
          'User_Group_Page.Modify_User_Group_Popup.Title.Edit'
        )
      : this.getImmediatelyLanguage(
          'User_Group_Page.Modify_User_Group_Popup.Title.Create'
        );

    this.createFormBuilderDefinition();
    this.initTabLabel();

    if (this.isEditMode) {
      this.getMembersForUserGroup(this.selectedUserGroupId);
    } else {
      this.belongingGroupUserIds = [];
      this.updateUserTabLabel();
    }
  }

  onCancel(): void {
    this.activeModal.dismiss();
  }

  onSelectedUserChange(users: UserManagement[]): void {
    this.selectedUsers = users;
    this.updateUserTabLabel();
  }

  onRemoveRequestChange(removedUsers: UserManagement[]): void {
    this.removedUsers = removedUsers;
  }

  onConfirmBtnClicked(): void {
    this.isMissingNameField = Utils.isNullOrEmpty(this.modifyUserGroup.name)
      ? true
      : false;

    if (this.isMissingNameField) {
      this.modifyGrid.selectedIndex = this.isEditMode ? 1 : 0;

      return;
    }

    const result = {
      name: this.modifyUserGroup.name,
      description: this.modifyUserGroup.description,
      addedUsers: this.selectedUsers.filter(
        (user: UserManagement) =>
          !this.belongingGroupUserIds.includes(user.identity.extId)
      ),
      removedUsers: this.removedUsers
    };

    this.activeModal.close(result);
  }

  private createFormBuilderDefinition(): void {
    this.modifyUserGroupForm = this.fb.group({
      name: [
        '',
        [
          Validators.required,
          noContentWhiteSpaceValidator,
          Validators.maxLength(this.DEFAULT_MAX_LENGTH)
        ]
      ],
      description: ['']
    });
  }

  private initTabLabel(): void {
    const informationTabLabel = this.getImmediatelyLanguage(
      'User_Group_Page.Modify_User_Group_Popup.Tab.Information_Tab'
    );
    const usersTabLabel =
      this.getImmediatelyLanguage(
        'User_Group_Page.Modify_User_Group_Popup.Tab.Users_Added_Tab'
      ) + ` (${this.selectedUsers.length})`;

    this.firstTabLabel = this.isEditMode ? usersTabLabel : informationTabLabel;
    this.secondTabLabel = this.isEditMode ? informationTabLabel : usersTabLabel;
  }

  private getImmediatelyLanguage(columnName: string): string {
    return this.translateAdapterService.getValueImmediately(columnName);
  }

  private getMembersForUserGroup(groupId: number): void {
    this.userGroupsDataService.getMembers(groupId).subscribe((members) => {
      if (!members) {
        this.belongingGroupUserIds = [];
        this.updateUserTabLabel();

        return;
      }
      this.belongingGroupUserIds = members.map((user) => user.identity.extId);
      this.updateUserTabLabel(this.belongingGroupUserIds.length);
    });
  }

  private updateUserTabLabel(userCount: number = null): void {
    if (this.isEditMode) {
      this.firstTabLabel =
        this.getImmediatelyLanguage(
          'User_Group_Page.Modify_User_Group_Popup.Tab.Users_Added_Tab'
        ) + ` (${userCount ? userCount : this.selectedUsers.length})`;
    } else {
      this.secondTabLabel =
        this.getImmediatelyLanguage(
          'User_Group_Page.Modify_User_Group_Popup.Tab.Users_Added_Tab'
        ) + ` (${userCount ? userCount : this.selectedUsers.length})`;
    }
  }
}
export interface IModifyUserGroupRequest {
  name: string;
  description?: string;
}
