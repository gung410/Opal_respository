import { AbstractControl, FormGroup } from '@angular/forms';
import { Assignment, Course, CourseContentItemModel, CourseContentItemType, LectureType, UserInfoModel } from '@opal20/domain-api';
import { BaseComponent, KeyCode, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ContentItemAction, IActionMenuItem } from '../models/action-menu-item';
import { CreationPosition, ICreationMenuItem } from '../models/creation-menu-item';

import { IActionMenuMouseEvent } from '../models/action-menu-mouse-event';
import { MenuEvent } from '@progress/kendo-angular-menu';

@Component({
  selector: 'table-of-content',
  templateUrl: 'table-of-content.component.html'
})
export class TableOfContentComponent extends BaseComponent {
  public contentItemAction = ContentItemAction;
  public actionMenu: { text: string; items: IDataItem[] }[] = [
    {
      text: '',
      items: [
        {
          text: this.translateCommon('Move up'),
          value: ContentItemAction.MoveUp
        },
        {
          text: this.translateCommon('Move down'),
          value: ContentItemAction.MoveDown
        },
        {
          text: this.translateCommon('Edit'),
          value: ContentItemAction.Edit
        },
        {
          text: this.translateCommon('Delete'),
          value: ContentItemAction.Delete
        }
      ]
    }
  ];
  public creationMenu: { text: string; items: IDataItem[] }[] = [
    {
      text: '',
      items: []
    }
  ];
  public childCreationMenu: { text: string; items: IDataItem[] }[] = [
    {
      text: '',
      items: [
        { text: this.translate('Create Content'), value: LectureType.InlineContent },
        { text: this.translate('Import Content from Repository'), value: LectureType.DigitalContent },
        { text: this.translate('Add Form'), value: LectureType.Quiz }
      ]
    }
  ];

  public form: FormGroup | undefined;
  public CreationPosition = CreationPosition;

  @Input() public selectedItemId: string = '';
  @Input() public readonly: boolean = false;
  @Input() public contentItems: CourseContentItemModel[] = [];
  @Input() public set course(course: Course) {
    this._course = course;

    if (this.initiated) {
      this.creationMenu = this.buildCreationMenu();
    }
  }
  @Output() public onContentItemSelect: EventEmitter<CourseContentItemModel> = new EventEmitter();
  @Output() public onSectionExpandChange: EventEmitter<CourseContentItemModel> = new EventEmitter();
  @Output() public onCreationMenuSelect: EventEmitter<ICreationMenuItem> = new EventEmitter();
  @Output() public onActionMenuSelect: EventEmitter<IActionMenuItem> = new EventEmitter();
  @Output() public onSearch: EventEmitter<string> = new EventEmitter();

  public searchText: string = '';
  private _course: Course;
  private currentUser = UserInfoModel.getMyUserInfo();

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public getLectureIcon(iconName: string): string {
    switch (iconName) {
      case 'jpeg':
      case 'jpg':
      case 'png':
      case 'svg':
      case 'gif':
        return 'photo';
      case 'inline-content':
      case 'learning-content':
      case 'docx':
      case 'xlsx':
      case 'pptx':
      case 'html':
        return 'reading';
      case 'mp4':
      case 'm4v':
      case 'ogg':
      case 'ogv':
        return 'video';
      case 'quiz':
      case 'pdf':
        return iconName;
      case 'mp3':
      case 'zip':
        return 'file';
      default:
        return 'file';
    }
  }

  public buildCreationMenu(): { text: string; items: IDataItem[] }[] {
    const result = [
      {
        text: '',
        items: [
          { text: this.translate('Add Content'), value: LectureType.InlineContent },
          { text: this.translate('Import Content from Repository'), value: LectureType.DigitalContent },
          { text: this.translate('Add Form'), value: LectureType.Quiz },
          { text: this.translate('Add Unit'), value: CourseContentItemType.Section }
        ]
      }
    ];
    if (this._course.isMicrolearning()) {
      result[0].items = result[0].items.filter(p => p.value !== CourseContentItemType.Assignment);
    }
    return result;
  }

  public select(item: CourseContentItemModel): void {
    this.selectedItemId = item.id;
    this.onContentItemSelect.emit(item);
  }

  public onSearchInputKeydown(): void {
    this.onSearch.emit(this.searchText);
  }

  public onTocCreationMenuSelect(
    event: MenuEvent,
    position: CreationPosition,
    item?: CourseContentItemModel,
    parent?: CourseContentItemModel
  ): void {
    if (event.item.items) {
      return;
    }

    this.onCreationMenuSelect.emit({
      itemType: event.item.value,
      order: this.getOrder(item, position, parent),
      item,
      parent
    });
  }

  public onCreateNewAssignmentClick(): void {
    this.onCreationMenuSelect.emit({
      itemType: CourseContentItemType.Assignment,
      order: 1
    });
  }

  public onTocActionMenuClick(event: IActionMenuMouseEvent): void {
    event.isActionItemClicked = true;
  }

  public onTocActionMenuSelect(
    event: MenuEvent & IActionMenuMouseEvent,
    item: CourseContentItemModel,
    parent?: CourseContentItemModel
  ): void {
    if (event.item.items.length > 0) {
      return;
    }

    this.onActionMenuSelect.emit({
      actionType: event.item.data,
      item,
      parent
    });
  }

  public onTocSectionExpandChange(item: CourseContentItemModel): void {
    item.expanded = !item.expanded;
    this.onSectionExpandChange.emit(item);
  }

  public onTocContentItemSelect(item: CourseContentItemModel): void {
    if (this.selectedItemId !== item.id && !(event as IActionMenuMouseEvent).isActionItemClicked) {
      this.selectedItemId = item.id;
      this.onContentItemSelect.emit(item);
    }
  }

  public onInlineInputKeydown(event: KeyboardEvent, item: CourseContentItemModel, control: AbstractControl): void {
    if (event.keyCode !== KeyCode.Enter) {
      return;
    }
  }

  public canCreateAssignment(): boolean {
    return !this.readonly && Assignment.hasCreateAssignmentPermission(this.currentUser);
  }

  public canCreateCourseContent(): boolean {
    return !this.readonly && CourseContentItemModel.hasCreateCourseContentPermission(this.currentUser);
  }

  public canEditDeleteMineCourseContent(): boolean {
    return !this.readonly && CourseContentItemModel.hasEditDeleteMineCourseContentPermission(this.currentUser);
  }

  public canEditDeleteOthersCourseContent(): boolean {
    return !this.readonly && CourseContentItemModel.hasEditDeleteOthersCourseContentPermission(this.currentUser);
  }

  protected onInit(): void {
    this.creationMenu = this.buildCreationMenu();
  }

  private getOrder(item: CourseContentItemModel | null, creationPosition: CreationPosition, parent?: CourseContentItemModel): number {
    const basedOnOrder = item ? item.order : 0;
    switch (creationPosition) {
      case CreationPosition.After:
        return basedOnOrder + 1;
      case CreationPosition.Before:
        return basedOnOrder;
      case CreationPosition.Include:
        return 0;
      case CreationPosition.Last: {
        const orderedLastItem = this.orderedLastItem();
        return orderedLastItem ? orderedLastItem.order + 1 : 0;
      }
    }
  }

  private orderedLastItem(): CourseContentItemModel | null {
    const orderedItems = Utils.orderBy(this.contentItems.filter(p => p.order != null), p => p.order);
    return orderedItems[orderedItems.length - 1];
  }
}
