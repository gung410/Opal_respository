import {
  AssessmentRepository,
  Assignment,
  AssignmentRepository,
  ClassRun,
  ContentApiService,
  Course,
  CourseContentItemModel,
  CourseContentItemType,
  CourseStatus,
  DigitalContentType,
  FormModel,
  IDigitalContent,
  ISaveLectureRequest,
  LearningContentRepository,
  LectureModel,
  LectureQuizConfigModel,
  LectureType,
  UserInfoModel
} from '@opal20/domain-api';
import {
  AssignmentDetailViewModel,
  AssignmentMode,
  DigitalContentReferenceDialog,
  NavigationPageService,
  PreviewMode
} from '@opal20/domain-components';
import { BasePageComponent, ModuleFacadeService, TranslationMessage, Utils } from '@opal20/infrastructure';
import { Component, HostBinding, Input, ViewChild } from '@angular/core';
import { ContentItemAction, IActionMenuItem } from '../models/action-menu-item';
import { Observable, Subscription, combineLatest, from, of } from 'rxjs';

import { AssignmentFormEditor } from './assignment-form-editor.component';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { FormReferenceDialogComponent } from './dialogs/form-reference-dialog.component';
import { ICreationMenuItem } from '../models/creation-menu-item';
import { LectureContentEditor } from './lecture-content-editor.component';
import { MovementDirection } from '../models/movement-direction';
import { OpalDialogService } from '@opal20/common-components';
import { PreviewContentDialogComponent } from './dialogs/preview-content-dialog.component';
import { SectionFormEditorDialogComponent } from './dialogs/section-form-editor-dialog.component';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'content-detail-page',
  templateUrl: 'content-detail-page.component.html'
})
export class ContentDetailPageComponent extends BasePageComponent {
  public AssignmentMode: typeof AssignmentMode = AssignmentMode;

  public get courseId(): string | undefined {
    return this._courseId;
  }

  @Input()
  public set courseId(v: string | undefined) {
    if (Utils.isDifferent(this._courseId, v)) {
      this._courseId = v;
    }
  }

  public get classRunId(): string | undefined {
    return this._classRunId;
  }

  @Input()
  public set classRunId(v: string | undefined) {
    if (Utils.isDifferent(this._classRunId, v)) {
      this._classRunId = v;
    }
  }

  public get course(): Course {
    return this._course;
  }
  @Input()
  public set course(v: Course) {
    if (!Utils.isDifferent(this._course, v)) {
      return;
    }
    this._course = v;
    if (this.initiated) {
      this.loadCourseOnlyTableOfContents();
    }
  }
  public get classRun(): ClassRun | null {
    return this._classRun;
  }
  @Input()
  public set classRun(v: ClassRun | null) {
    if (!Utils.isDifferent(this._classRun, v)) {
      return;
    }
    this._classRun = v;
    if (this.initiated) {
      this.loadTableOfContents();
    }
  }
  @Input()
  public isPreviewMode: boolean = false;

  public statusItems: IDataItem[] = [
    {
      text: this.translateCommon('Publish'),
      value: CourseStatus.Published
    },
    {
      text: this.translateCommon('Unpublish'),
      value: CourseStatus.Unpublished
    }
  ];
  public draftStatusItems: IDataItem[] = [
    {
      text: this.translateCommon('Draft'),
      value: CourseStatus.Draft
    },
    {
      text: this.translateCommon('Publish'),
      value: CourseStatus.Published
    }
  ];
  public contentItems: CourseContentItemModel[] = [];
  public courseContentItems: CourseContentItemModel[] = [];
  public classRunContentItems: CourseContentItemModel[] = [];
  public selectedLecture: LectureModel;
  public selectedAssignmentVm: AssignmentDetailViewModel;
  public loadingSelectedLecture: boolean = false;
  public loadingTableOfContents: boolean = false;
  public loadingCourseOnlyTableOfContents: boolean = false;
  public selectedContentItemId?: string;
  public isAllowDownloadContent: boolean;
  public PreviewMode: typeof PreviewMode = PreviewMode;

  public dropdownItems: { text: string; value: PreviewMode }[] = [
    { text: 'Web', value: PreviewMode.Web },
    { text: 'Mobile', value: PreviewMode.Mobile }
  ];

  private _course: Course = new Course();
  private _classRun: ClassRun | null;
  private loadTableOfContentsSub: Subscription = new Subscription();
  private loadSelectedLectureSub: Subscription = new Subscription();
  private loadSelectedAssignmentSub: Subscription = new Subscription();
  private loadCourseOnlyTableOfContentsSub: Subscription = new Subscription();
  private currentUser = UserInfoModel.getMyUserInfo();
  private expandedSectionItems: { [id: string]: boolean } = {};
  private _courseId: string;
  private _classRunId: string;

  @ViewChild('lectureContentEditor', { static: false })
  private lectureContentEditor: LectureContentEditor;
  @ViewChild('assignmentFormEditor', { static: false })
  private assignmentFormEditor: AssignmentFormEditor;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private assignmentRepository: AssignmentRepository,
    private learningContentRepository: LearningContentRepository,
    private dialogService: OpalDialogService,
    public navigationPageService: NavigationPageService,
    private contentApiService: ContentApiService,
    private opalDialogService: OpalDialogService,
    private assessmentRepository: AssessmentRepository
  ) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public deleteContent(item: CourseContentItemModel): void {
    const deleteFn = (id: string, courseId: string) => {
      if (item.type === CourseContentItemType.Section) {
        return this.learningContentRepository.deleteSection(item.id, this.course.id);
      } else if (item.type === CourseContentItemType.Lecture) {
        return this.learningContentRepository.deleteLecture(item.id, this.course.id);
      } else if (item.type === CourseContentItemType.Assignment) {
        return this.assignmentRepository.deleteAssignment(item.id, this.course.id);
      }
    };

    deleteFn(item.id, this.course.id).then(() => {
      this.showNotification();
      this.contentItems = CourseContentItemModel.removeItem(this.contentItems, item.id);
      this.selectFirstItem();
    });
  }

  public moveContent(item: CourseContentItemModel, direction: MovementDirection): void {
    this.learningContentRepository
      .changeContentOrder({
        id: item.id,
        direction,
        type: item.type,
        courseId: this.course.id,
        classRunId: this.classRunId
      })
      .then(data => this.updateContentItems(data));
  }

  public customizeData(): void {
    this.learningContentRepository
      .cloneContentForClassRun({
        courseId: this.course.id,
        classRunId: this.classRunId
      })
      .then(data => {
        this.setContentItemsData(this.classRunId, data);
      });
  }

  public onSectionExpandChange(item: CourseContentItemModel): void {
    this.expandedSectionItems[item.id] = item.expanded;
  }

  public onCreationMenuSelect(menu: ICreationMenuItem): void {
    switch (menu.itemType) {
      case LectureType.DigitalContent:
        this.addDigitalContent(menu);
        break;
      case CourseContentItemType.Section:
        this.addSection(menu);
        break;
      case LectureType.InlineContent:
        this.addInlineContent(menu);
        break;
      case LectureType.Quiz:
        this.addQuiz(menu);
        break;
      case CourseContentItemType.Assignment:
        this.addAssignment(menu);
        break;
      default:
        break;
    }
  }

  public onActionMenuSelect(menu: IActionMenuItem): void {
    switch (menu.actionType) {
      case ContentItemAction.Delete:
        this.modalService.showConfirmMessage(
          new TranslationMessage(this.moduleFacadeService.globalTranslator, 'Are you sure you want to permanently delete this item?'),
          () => this.deleteContent(menu.item)
        );
        break;
      case ContentItemAction.MoveUp:
        this.moveContent(menu.item, MovementDirection.Up);
        break;
      case ContentItemAction.MoveDown:
        this.moveContent(menu.item, MovementDirection.Down);
        break;
      case ContentItemAction.Edit:
        this.editSection(menu.item);
        break;
      default:
        break;
    }
  }

  public onContentItemSelect(item: CourseContentItemModel, dontSaveWhenChangeSelectedItem: boolean = false): void {
    this.selectItem(item);
  }

  public saveContentData(): Observable<unknown> {
    if (this.lectureContentEditor && this.lectureContentEditor.hasDataChanged()) {
      return this.lectureContentEditor.validateAndSaveLectureData();
    } else if (this.assignmentFormEditor && this.assignmentFormEditor.hasDataChanged()) {
      return this.assignmentFormEditor.validateAndSaveAssignmentData();
    }
  }

  public hasDataChanged(): boolean {
    if (this.lectureContentEditor && this.lectureContentEditor.hasDataChanged()) {
      return true;
    } else if (this.assignmentFormEditor && this.assignmentFormEditor.hasDataChanged()) {
      return true;
    }
  }

  public selectItem(item: CourseContentItemModel): void {
    this.clearSelectedItemInfo();
    this.selectedContentItemId = item.id;
    if (item.type === CourseContentItemType.Lecture) {
      this.loadSelectedLecture(item.id);
    }
    if (item.type === CourseContentItemType.Assignment) {
      this.loadAssignment(item.id);
    }
  }

  public canShowPreviewButton(): boolean {
    return this.selectedContentItemId != null;
  }

  public onSearch(searchText: string): void {
    this.loadTableOfContents(searchText);
  }

  public onSaveAssignment(assignment: Assignment): void {
    this.contentItems = CourseContentItemModel.updateAssignmentContentItem(this.contentItems, assignment);
  }

  public onInlineLectureContentSaved(data: LectureModel): void {
    this.contentItems = CourseContentItemModel.updateLectureContentItem(this.contentItems, data);
  }

  public readonly(): boolean {
    if (this.isPreviewMode) {
      return true;
    }
    if (this.contentForCourseMode()) {
      return !this.course.canUserEditContent(this.currentUser);
    } else {
      return this.canCustomizeContentForClassRun() || !this.classRun.canUserEditContent(this.course, this.currentUser);
    }
  }

  public contentForCourseMode(): boolean {
    return this.classRun == null;
  }

  public contentForClassRunMode(): boolean {
    return this.classRun != null;
  }

  public canCustomizeContentForClassRun(): boolean {
    return (
      this.contentForClassRunMode() &&
      this.isInheritContentFromCourse() &&
      this.course.canCustomizeContentForClassRun(this.currentUser, this.classRun)
    );
  }

  public showQuizStatistics(): boolean {
    return LectureModel.hasViewQuizStatisticsPermission(this.currentUser);
  }

  public showInheritedAlert(): boolean {
    return this.contentForClassRunMode() && this.isInheritContentFromCourse();
  }

  public showCannotEditContentForCourseAlert(): boolean {
    return this.contentForCourseMode() && this.course.status === CourseStatus.Completed;
  }

  public showCannotEditContentForClassRunAlert(): boolean {
    return this.contentForClassRunMode() && this.classRun != null && this.classRun.hasLearnerStarted;
  }

  public isInheritContentFromCourse(): boolean {
    return this.courseContentItems.length > 0 && this.classRunContentItems.length === 0;
  }

  public onPreviewClick(previewMode: PreviewMode): void {
    this.opalDialogService.openDialogRef(
      PreviewContentDialogComponent,
      {
        previewMode: previewMode,
        assignmentData: this.selectedAssignmentVm ? this.selectedAssignmentVm.data : null,
        lectureData: this.selectedLecture ? this.selectedLecture : null,
        playerType: this.selectedLecture ? 'Lecture' : 'Assignment'
      },
      {
        maxWidth: '100vw',
        maxHeight: '100vh',
        width: '100vw',
        height: '100vh',
        borderRadius: '0'
      }
    );
  }

  protected onInit(): void {
    Promise.all([this.loadCourseOnlyTableOfContents(), this.loadTableOfContents()]).then(() => this.selectFirstItem());
  }

  private loadSelectedLecture(id: string): void {
    this.loadSelectedLectureSub.unsubscribe();
    this.loadingSelectedLecture = true;

    this.loadSelectedLectureSub = this.learningContentRepository
      .getLecture(id)
      .pipe(
        this.untilDestroy(),
        switchMap(response => {
          this.selectedLecture = response;
          return this.selectedLecture == null ||
            !this.selectedLecture.hasResourceId() ||
            this.selectedLecture.type !== LectureType.DigitalContent
            ? of(null)
            : from(this.contentApiService.getDigitalContent(this.selectedLecture.resourceId));
        })
      )
      .subscribe(
        (response: IDigitalContent) => {
          this.isAllowDownloadContent = response == null ? false : response.isAllowDownload;
          this.loadingSelectedLecture = false;
        },
        error => {
          this.loadingSelectedLecture = false;
        }
      );
  }

  private loadCourseOnlyTableOfContents(): Promise<void> {
    this.loadCourseOnlyTableOfContentsSub.unsubscribe();
    this.loadingCourseOnlyTableOfContents = true;
    return new Promise((resolve, reject) => {
      this.loadCourseOnlyTableOfContentsSub = from(this.learningContentRepository.getTableOfContents(this.courseId, null))
        .pipe(this.untilDestroy())
        .subscribe(
          data => {
            this.courseContentItems = data;
            if (this.classRun == null || this.classRunContentItems.length === 0) {
              this.updateContentItems(data);
            }

            if (this.loadingCourseOnlyTableOfContents) {
              this.loadingCourseOnlyTableOfContents = false;
              resolve();
            }
          },
          error => {
            if (this.loadingCourseOnlyTableOfContents) {
              this.loadingCourseOnlyTableOfContents = false;
              reject(error);
            }
          }
        );
    });
  }

  private loadTableOfContents(searchText?: string): Promise<void> {
    this.loadTableOfContentsSub.unsubscribe();
    this.loadingTableOfContents = true;
    const classRunId = Utils.isNullOrEmpty(searchText) || !this.isInheritContentFromCourse() ? this.classRunId : null;
    return new Promise((resolve, reject) => {
      this.loadTableOfContentsSub = from(this.learningContentRepository.getTableOfContents(this.courseId, classRunId, searchText))
        .pipe(this.untilDestroy())
        .subscribe(
          data => {
            this.setContentItemsData(classRunId, data, searchText);

            if (this.loadingTableOfContents) {
              this.loadingTableOfContents = false;
              resolve();
            }
          },
          error => {
            if (this.loadingTableOfContents) {
              this.loadingTableOfContents = false;
              reject(error);
            }
          }
        );
    });
  }

  private setContentItemsData(classRunId: string | null, data: CourseContentItemModel[], searchText?: string): void {
    if (Utils.isNullOrEmpty(searchText)) {
      if (classRunId != null) {
        this.classRunContentItems = data;
      } else {
        this.courseContentItems = data;
      }
      if (this.courseContentItems.length === 0 || data.length > 0) {
        this.updateContentItems(data);
      } else {
        this.updateContentItems(this.courseContentItems);
      }
    } else {
      this.updateContentItems(data);
    }
  }

  private updateContentItems(data: CourseContentItemModel[]): void {
    this.contentItems = data
      .map(p => Utils.cloneDeep(p))
      .map(item => {
        if (item.type === CourseContentItemType.Section) {
          const existingItem: boolean = this.expandedSectionItems[item.id];

          if (existingItem === true || existingItem === undefined) {
            item.expanded = true;
          }
        }
        return item;
      });
  }

  private addDigitalContent(menu: ICreationMenuItem): void {
    const dialogRef: DialogRef = this.dialogService.openDialogRef(DigitalContentReferenceDialog, null, {
      width: 860,
      height: 600
    });

    this.subscribe(dialogRef.result, result => {
      const digitalContent: IDigitalContent | null = result as IDigitalContent;
      if (digitalContent == null || digitalContent.id == null) {
        return;
      }

      const request: ISaveLectureRequest = {
        courseId: this.course.id,
        description: digitalContent.description,
        lectureName: digitalContent.title,
        lectureIcon: digitalContent.type === DigitalContentType.UploadedContent ? digitalContent.fileExtension : 'learning-content',
        order: menu.order,
        sectionId: this.getCreationSectionId(menu),
        type: menu.itemType as LectureType,
        resourceId: digitalContent.id,
        mimeType: digitalContent.fileType,
        classRunId: this.classRunId
      };

      this.createLectureAndSelectToc(request);
    });
  }

  private loadAssignment(id: string): void {
    this.loadSelectedAssignmentSub.unsubscribe();

    const assessmentObs = this.assessmentRepository.loadAssessments();
    const asignmentObs = this.assignmentRepository.getAssignmentById(id);
    this.loadSelectedAssignmentSub = combineLatest(asignmentObs, assessmentObs)
      .pipe(this.untilDestroy())
      .subscribe(([assignment, assessmentResult]) => {
        this.selectedAssignmentVm = new AssignmentDetailViewModel(assignment, assessmentResult.items);
      });
  }
  private addAssignment(menu: ICreationMenuItem): void {
    this.assessmentRepository
      .loadAssessments()
      .pipe(this.untilDestroy())
      .subscribe(assessmentResult => {
        const dialogRef = this.dialogService.openDialogRef(
          AssignmentFormEditor,
          {
            assignmentVm: Utils.clone(new AssignmentDetailViewModel(new Assignment(), assessmentResult.items), cloneData => {
              (cloneData.courseId = this.course.id), (cloneData.classRunId = this.classRunId);
            }),
            onSaveFn: data => {
              this.addContentItem(CourseContentItemModel.fromAssignment(data));
              this.selectItemById(data.id);
              dialogRef.close();
            },
            onCancelFn: () => {
              dialogRef.close();
            },
            enablePreview: true
          },
          {
            width: '90vw',
            height: '90vh'
          }
        );
      });
  }

  private addQuiz(menu: ICreationMenuItem): void {
    const dialogRef: DialogRef = this.dialogService.openDialogRef(FormReferenceDialogComponent, null, {
      width: 860,
      height: 600
    });

    this.subscribe(dialogRef.result, result => {
      const form: FormModel = result as FormModel;

      if (!form || !form.id) {
        return;
      }

      const request: ISaveLectureRequest = {
        courseId: this.course.id,
        description: form.title,
        lectureName: form.title,
        lectureIcon: 'quiz',
        order: menu.order,
        sectionId: this.getCreationSectionId(menu),
        type: menu.itemType as LectureType,
        resourceId: form.id,
        classRunId: this.classRunId,
        quizConfig: new LectureQuizConfigModel({ displayPollResultToLearners: true })
      };
      this.createLectureAndSelectToc(request);
    });
  }

  private getCreationSectionId(menu: ICreationMenuItem): string {
    return menu.parent && menu.parent.type === CourseContentItemType.Section ? menu.parent.id : null;
  }

  private addSection(menu: ICreationMenuItem): void {
    const dialogRef = this.dialogService.openDialogRef(
      SectionFormEditorDialogComponent,
      {
        courseId: this.course.id,
        classrunId: this.classRunId,
        order: menu.order,
        onSaveFn: data => {
          this.addContentItem(CourseContentItemModel.fromSection(data));
          dialogRef.close();
        },
        onCancelFn: () => {
          dialogRef.close();
        }
      },
      {
        width: 500
      }
    );
  }

  private editSection(item: CourseContentItemModel): void {
    const dialogRef = this.dialogService.openDialogRef(
      SectionFormEditorDialogComponent,
      {
        courseId: this.course.id,
        classrunId: this.classRunId,
        sectionId: item.id,
        onSaveFn: data => {
          this.contentItems = CourseContentItemModel.updateSectionContentItem(this.contentItems, data);
          dialogRef.close();
        },
        onCancelFn: () => {
          dialogRef.close();
        }
      },
      {
        width: 500
      }
    );
  }

  private createLectureAndSelectToc(request: ISaveLectureRequest): Promise<void> {
    return this.learningContentRepository
      .saveLecture(request)
      .toPromise()
      .then(lecture => {
        this.addContentItem(CourseContentItemModel.fromLecture(lecture));
        this.selectItemById(lecture.id);
      });
  }

  private addInlineContent(menu: ICreationMenuItem): void {
    const dialogRef = this.dialogService.openDialogRef(
      LectureContentEditor,
      {
        courseId: this.course.id,
        classrunId: this.classRunId,
        order: menu.order,
        lectureIcon: 'inline-content',
        sectionId: this.getCreationSectionId(menu),
        hideEditContent: true,
        onSaveFn: lecture => {
          this.addContentItem(CourseContentItemModel.fromLecture(lecture));
          this.selectItemById(lecture.id);
          dialogRef.close();
        },
        onCancelFn: () => {
          dialogRef.close();
        }
      },
      { width: 800 }
    );
  }

  private selectFirstItem(): void {
    const item = this.contentItems.find(p => p.getFirstNotSectionId() != null);

    if (item != null) {
      this.selectItem(item);
    } else {
      this.clearSelectedItemInfo();
    }
  }

  private clearSelectedItemInfo(): void {
    this.selectedContentItemId = null;
    this.selectedAssignmentVm = null;
    this.selectedLecture = null;
  }

  private selectItemById(id: string | null): void {
    const item = this.contentItems.find(p => p.id === id);

    if (item != null) {
      this.selectItem(item);
    }
  }

  private addContentItem(item: CourseContentItemModel): void {
    const newItems = CourseContentItemModel.addItem(this.contentItems, item);
    this.updateContentItems(newItems);
    if (this.classRun == null) {
      this.courseContentItems = Utils.clone(newItems);
    } else {
      this.classRunContentItems = Utils.clone(newItems);
    }
  }
}
