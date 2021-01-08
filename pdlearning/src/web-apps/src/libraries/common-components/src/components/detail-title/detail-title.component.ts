import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, Input, Output, SimpleChanges, ViewChild } from '@angular/core';
import { DetailTitleSettings, SubTitlePosition } from '../../models/detail-title-settings.model';

@Component({
  selector: 'detail-title',
  templateUrl: './detail-title.component.html'
})
export class DetailTitleComponent extends BaseFormComponent {
  public readonly subTitlePosition: typeof SubTitlePosition = SubTitlePosition;

  @Input() public title: string;
  @Input() public subTitle: string;
  @Input() public settings: DetailTitleSettings;
  @Input() public allowEditTitle: boolean = true;
  @Input() public maxLength: number;

  // Optional for form detail
  @Input() public urlSubIcon?: string;

  @Output() public onBackButtonClick: EventEmitter<void> = new EventEmitter<void>();
  @Output() public titleChange: EventEmitter<string> = new EventEmitter<string>();

  @ViewChild('titleInput', { static: false })
  public titleInput: ElementRef;

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onBackClicked(): void {
    this.onBackButtonClick.emit();
  }

  public enableEditTitle(): void {
    this.settings.editModeEnabled = true;
    setTimeout(() => this.titleInput.nativeElement.focus());
  }

  public onTitleChanged(newTitle: string): void {
    this.title = newTitle;
    this.titleChange.emit(this.title);
  }

  public focusInput(): void {
    this.settings.editModeEnabled = true;
    setTimeout(() => this.titleInput.nativeElement.focus());
  }

  public async disableEditMode(): Promise<void> {
    const isFormValid = await this.validate();
    if (isFormValid) {
      setTimeout(() => {
        if (this.settings.editModeEnabled) {
          this.settings.editModeEnabled = false;
        }
      }, this.titleInput.nativeElement.blur());
    } else {
      this.focusInput();
    }
  }

  protected onInit(): void {
    if (this.settings) {
      return;
    }
    this.settings = new DetailTitleSettings();
  }

  protected onChanges(changes: SimpleChanges): void {
    if (!changes.title) {
      return;
    }
    this.initFormData();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        title: {
          defaultValue: this.title,
          validators: this.settings && this.settings.titleValidators ? this.settings.titleValidators : []
        }
      }
    };
  }
}
