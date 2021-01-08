import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import {
  BrokenLinkReportType,
  ContentApiService,
  DigitalContent,
  ILectureDigitalContentConfigModel,
  ISaveLectureRequest,
  LearningContentRepository,
  LectureDigitalContentConfigModel,
  LectureModel,
  LectureType,
  UserInfoModel,
  VideoCommentSourceType
} from '@opal20/domain-api';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { LectureContentViewModel, PreviewMode, VideoAnnotationCommentInfo } from '@opal20/domain-components';
import { Observable, Subscription, combineLatest, from, of } from 'rxjs';
import { ifValidator, requiredAndNoWhitespaceValidator } from '@opal20/common-components';
import { map, switchMap } from 'rxjs/operators';

import { EditorComponent } from '@progress/kendo-angular-editor';
import { ToolBarButtonComponent } from '@progress/kendo-angular-toolbar';

@Component({
  selector: 'lecture-content-editor',
  templateUrl: './lecture-content-editor.component.html'
})
export class LectureContentEditor extends BaseFormComponent {
  public serviceType: BrokenLinkReportType = BrokenLinkReportType.Course;
  public get lecture(): LectureModel | undefined {
    return this._lecture;
  }
  @Input()
  public set lecture(v: LectureModel | undefined) {
    if (Utils.isEqual(this._lecture, v)) {
      return;
    }
    this._lecture = v;
    if (this.initiated && v != null) {
      if (this.lectureVm.isInline() && v.type === LectureType.InlineContent && this.editor != null) {
        this.editor.value = v.value;
      }
      this.lectureVm.updateData(v);
    }
  }

  public get lectureId(): string | null {
    return this._lectureId;
  }
  @Input()
  public set lectureId(v: string | null) {
    if (Utils.isEqual(this._lectureId, v)) {
      return;
    }
    this._lectureId = v;
    if (this.initiated && this._lecture == null) {
      this.loadData();
    }
  }

  @Input() public courseId: string = '';
  @Input() public sectionId?: string;
  @Input() public classrunId?: string;
  @Input() public order: number = 0;
  @Input() public lectureIcon?: string;
  @Input() public onSaveFn?: (data: LectureModel) => void;
  @Input() public onCancelFn?: () => void;
  @Input() public readonly: boolean = false;
  @Input() public hideEditContent: boolean = false;
  @Input() public isAllowDownload: boolean;

  @Output() public onSave: EventEmitter<LectureModel> = new EventEmitter<LectureModel>();
  @Output() public onCancel = new EventEmitter();
  @Output() public lectureChange: EventEmitter<LectureModel> = new EventEmitter<LectureModel>();

  @ViewChild(EditorComponent, { static: false })
  public editor: EditorComponent;
  @ViewChild('subscriptButton', { static: true })
  public subscriptButton: ToolBarButtonComponent;
  @ViewChild('superscriptButton', { static: true })
  public superscriptButton: ToolBarButtonComponent;

  public lectureVm: LectureContentViewModel = new LectureContentViewModel(this.defaultLecture());
  public digitalContent: DigitalContent;
  public videoAnnotationCommentInfo: VideoAnnotationCommentInfo;
  public PreviewMode: typeof PreviewMode = PreviewMode;
  private _loadDataSub: Subscription = new Subscription();
  private _lecture: LectureModel | undefined;
  private _lectureId: string | null;

  private currentUser = UserInfoModel.getMyUserInfo();
  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private learningContentRepository: LearningContentRepository,
    private contentApiService: ContentApiService
  ) {
    super(moduleFacadeService);
  }

  public removeSubscript(): void {
    if (this.editor === undefined || this.subscriptButton === undefined) {
      return;
    }

    if (this.subscriptButton.selected === true) {
      this.editor.exec('subscript');
    }
  }

  public removeSuperscript(): void {
    if (this.editor === undefined || this.superscriptButton === undefined) {
      return;
    }

    if (this.superscriptButton.selected === true) {
      this.editor.exec('superscript');
    }
  }

  public saveLectureData(autoSave?: boolean): Promise<LectureModel> {
    const request: ISaveLectureRequest = {
      id: this.lectureVm.id,
      courseId: this.lectureVm.courseId,
      description: this.lectureVm.description,
      lectureName: this.lectureVm.title,
      lectureIcon: this.lectureVm.lectureIcon,
      order: this.lectureVm.order,
      sectionId: this.lectureVm.sectionId,
      type: this.lectureVm.type,
      resourceId: this.lectureVm.resourceId,
      mimeType: this.lectureVm.mimeType,
      classRunId: this.lectureVm.classRunId,
      base64Value: this.lectureVm.base64Value,
      quizConfig: this.lectureVm.quizConfig,
      digitalContentConfig: this.lectureVm.digitalContentConfig
    };
    return this.learningContentRepository
      .saveLecture(request, autoSave !== true)
      .toPromise()
      .then(_ => {
        this.showNotification();
        if (this.lectureVm.id == null || _.id === this.lectureVm.id) {
          this.lectureVm.updateData(_);
        }
        return _;
      });
  }

  public validateAndSaveLectureData(autoSave?: boolean): Observable<LectureModel> {
    return from(
      new Promise<LectureModel>((resolve, reject) => {
        this.validate().then(isValid => {
          if (isValid) {
            this.saveLectureData(autoSave).then(_ => {
              this.onSave.emit(_);
              if (this.onSaveFn) {
                this.onSaveFn(_);
              }
              resolve(_);
            }, reject);
          } else {
            reject('validation error');
          }
        });
      })
    );
  }

  public onSaveBtnClick(): void {
    this.validateAndSaveLectureData().subscribe();
  }

  public onCancelBtnClick(): void {
    if (this.onCancelFn != null) {
      this.onCancelFn();
      this.editor.value = this.lectureVm.data.value;
    }
    this.onCancel.emit();
    this.lectureVm.resetData();
    this.lectureChange.emit(this.lectureVm.originalData);
  }

  public onValueChanged(editorHtml: string): void {
    const listAnchor: NodeList = this.editor.viewMountElement.querySelectorAll('a');
    let hasChanged = false;
    listAnchor.forEach((a: HTMLAnchorElement) => {
      const href = Utils.getHrefFromAnchor(a);
      if (!Utils.isAbsoluteUrl(href)) {
        const newHref = `http://${href}`;
        const newHtml = a.outerHTML.replace(href, newHref);
        editorHtml = editorHtml.replace(a.outerHTML, newHtml);
        hasChanged = true;
      }
    });

    if (hasChanged) {
      this.editor.value = editorHtml;
    }
    this.lectureVm.value = this.editor.value;
    this._lecture.value = this.editor.value;
    this.lectureChange.emit(this.lectureVm.data);
  }

  public hasDataChanged(): boolean {
    return this.lectureVm.hasDataChanged();
  }

  public canAllowDownload(): boolean {
    return this.lectureVm.data.hasAllowDownloadCourseContentPermission(this.currentUser) && this.isAllowDownload;
  }

  protected onInit(): void {
    this.loadData();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        title: {
          defaultValue: null,
          validators: [
            {
              validator: ifValidator(_ => this.lectureVm.isInline(), requiredAndNoWhitespaceValidator),
              validatorType: 'required'
            }
          ]
        },
        value: {
          defaultValue: null,
          validators: []
        },
        quizConfig_isByPass: {
          defaultValue: null,
          validators: []
        },
        quizConfig_isDisplayPollResult: {
          defaultValue: null,
          validators: []
        },
        digitalContentConfig_canDownload: {
          defaultValue: null,
          validators: []
        }
      }
    };
  }

  private loadData(): void {
    this._loadDataSub.unsubscribe();
    this._loadDataSub = combineLatest(this.getLecture())
      .pipe(
        switchMap(([lecture]) => {
          return this.fetchContent(lecture).pipe(map(content => <[LectureModel, DigitalContent]>[lecture, content]));
        }),
        this.untilDestroy()
      )
      .subscribe(([lecture, content]) => {
        this.lectureVm = new LectureContentViewModel(lecture);
        if (this.editor) {
          this.editor.value = this.lectureVm.data.value;
        }
        if (content != null) {
          this.digitalContent = content;
          this.videoAnnotationCommentInfo = {
            objectId: lecture.classRunId,
            originalObjectId: lecture.id,
            sourceType: VideoCommentSourceType.LMM
          };
        }
      });
  }

  private getLecture(): Observable<LectureModel> {
    let lectureModelObs: Observable<LectureModel>;
    if (this.lectureId != null && this.lecture == null) {
      lectureModelObs = from(this.learningContentRepository.getLecture(this.lectureId)).pipe(this.untilDestroy());
    } else if (this.lecture != null) {
      lectureModelObs = of(this.lecture);
    } else {
      lectureModelObs = of(this.defaultLecture());
    }
    return lectureModelObs;
  }

  private defaultLecture(): LectureModel {
    return new LectureModel({
      id: null,
      icon: this.lectureIcon,
      title: '',
      description: '',
      type: LectureType.InlineContent,
      order: this.order,
      sectionId: this.sectionId,
      courseId: this.courseId,
      classRunId: this.classrunId,
      resourceId: null,
      quizConfig: null,
      digitalContentConfig: new LectureDigitalContentConfigModel(<ILectureDigitalContentConfigModel>{
        canDownload: false
      })
    });
  }

  private fetchContent(lecture: LectureModel): Observable<DigitalContent | null> {
    if (lecture.hasResourceId() && lecture.type === LectureType.DigitalContent) {
      return from(this.contentApiService.getDigitalContent(lecture.resourceId));
    }

    return of(null);
  }
}
