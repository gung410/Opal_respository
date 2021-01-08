import { BaseFormComponent, IFormBuilderDefinition, MAX_INT, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, ElementRef, Input, ViewChild, ViewEncapsulation } from '@angular/core';
import { LearningContentRepository, SectionModel } from '@opal20/domain-api';
import { Observable, Subscription, from, of } from 'rxjs';

import { SectionDetailViewModel } from '@opal20/domain-components';
import { requiredAndNoWhitespaceValidator } from '@opal20/common-components';

@Component({
  selector: 'section-form-editor-dialog',
  templateUrl: './section-form-editor-dialog.component.html',
  encapsulation: ViewEncapsulation.None
})
export class SectionFormEditorDialogComponent extends BaseFormComponent {
  @Input() public courseId: string = '';
  @Input() public classrunId?: string;
  @Input() public order: number = 0;

  public get sectionId(): string | null {
    return this._sectionId;
  }
  @Input()
  public set sectionId(v: string | null) {
    if (Utils.isEqual(this._sectionId, v)) {
      return;
    }
    this._sectionId = v;
    if (this.initiated && this._sectionId == null) {
      this.loadData();
    }
  }
  public get section(): SectionModel | null {
    return this._section;
  }
  @Input()
  public set section(v: SectionModel | null) {
    if (Utils.isEqual(this._section, v)) {
      return;
    }
    this._section = v;
    if (this.initiated && v != null) {
      this.sectionDetailVM.updateSectionData(v);
    }
  }
  @Input() public onSaveFn?: (data: SectionModel) => void;
  @Input() public onCancelFn?: () => void;

  public sectionDetailVM: SectionDetailViewModel;
  public maxInt: number = MAX_INT;

  private _loadDataSub: Subscription = new Subscription();
  private _section: SectionModel | null;
  private _sectionId: string | null;

  @ViewChild('titleInput', { static: true })
  private titleInput: ElementRef;
  @ViewChild('descriptionInput', { static: true })
  private descriptionInput: ElementRef;
  @ViewChild('creditsAwardInput', { static: true })
  private creditsAwardInput: ElementRef;

  constructor(protected moduleFacadeService: ModuleFacadeService, private learningContentRepository: LearningContentRepository) {
    super(moduleFacadeService);
    this.sectionDetailVM = new SectionDetailViewModel(this.defaultSection());
  }

  public onClickSave(): void {
    this.validate().then(val => {
      if (val) {
        this.learningContentRepository
          .saveSection({ data: this.sectionDetailVM.data })
          .toPromise()
          .then(_ => {
            this.sectionDetailVM.updateSectionData(_);
            if (this.onSaveFn) {
              this.onSaveFn(_);
            }
          });
      }
    });
  }

  public onClickCancel(): void {
    if (this.onCancelFn != null) {
      this.onCancelFn();
    }
    this.sectionDetailVM.reset();
  }

  protected onInit(): void {
    this.loadData();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        title: {
          defaultValue: '',
          validators: [
            {
              validator: requiredAndNoWhitespaceValidator(),
              validatorType: 'required'
            }
          ]
        },
        desription: {
          defaultValue: ''
        },
        creditsAward: {
          defaultValue: 0
        }
      }
    };
  }

  private loadData(): void {
    this._loadDataSub.unsubscribe();
    this._loadDataSub = this.getSection()
      .pipe(this.untilDestroy())
      .subscribe(section => {
        this.sectionDetailVM = new SectionDetailViewModel(section);
      });
  }

  private getSection(): Observable<SectionModel> {
    let sectionObs: Observable<SectionModel>;
    if (this.sectionId != null && this.section == null) {
      sectionObs = from(this.learningContentRepository.getSection(this.sectionId)).pipe(this.untilDestroy());
    } else if (this.section != null) {
      sectionObs = of(this.section);
    } else {
      sectionObs = of(this.defaultSection());
    }
    return sectionObs;
  }

  private defaultSection(): SectionModel {
    return new SectionModel({
      id: null,
      courseId: this.courseId,
      classRunId: this.classrunId,
      title: '',
      description: '',
      order: this.order
    });
  }
}
