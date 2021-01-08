import { AmazonS3UploaderModule, DocumentViewerModule, FunctionModule } from '@opal20/infrastructure';
import {
  ButtonGroupComponent,
  ButtonTemplateDirective,
  MoreButtonTemplateDirective
} from './components/button-group/button-group.component';
import { DateInputsModule, DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { ModuleWithProviders, NgModule, Type } from '@angular/core';
import { OpalLabelTemplateDirective, OpalOptionTemplateDirective } from './components/select/templates.directives';
import { VideojsPlayerComponent, registerVideojsPlugins } from './components/videojs-player/videojs-player.component';

import { ActionBarComponent } from './components/action-bar/action-bar.component';
import { AudioPlayerComponent } from './components/audio-player/audio-player.component';
import { AuthenticationModule } from '@opal20/authentication';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { CommonModule } from '@angular/common';
import { ContextMenuModule } from '@progress/kendo-angular-menu';
import { CountdownTimerComponent } from './components/countdown-timer/countdown-timer.component';
import { CropImageDialogComponent } from './components/crop-image-dialog/crop-image-dialog.component';
import { DataCheckIndicatorComponent } from './components/data-check-indicator/data-check-indicator.component';
import { DataFilterComponent } from './components/data-filter/data-filter.component';
import { DetailTitleComponent } from './components/detail-title/detail-title.component';
import { DocumentPlayerComponent } from './components/document-player/document-player.component';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { DropFileUploaderComponent } from './components/drop-file-uploader/drop-file-uploader.component';
import { EditModeDirective } from './components/editable/edit-mode.directive';
import { EditableComponent } from './components/editable/editable.component';
import { EditorModule } from '@progress/kendo-angular-editor';
import { EmbedIframeDialogComponent } from './components/embed-iframe-dialog/embed-iframe-dialog.component';
import { ExternalLinkGuardDirective } from './directives/external-link-guard.directive';
import { GridModule } from '@progress/kendo-angular-grid';
import { GuidelinePopulatedFieldsDialogComponent } from './components/guideline-populated-fields-dialog/guideline-populated-fields-dialog.component';
import { ImageCropperModule } from 'ngx-image-cropper';
import { KendoEditorBrokenLinkScannerButtonDirective } from './directives/kendo-editor-broken-link-scanner-button.directive';
import { KendoEditorCustomInsertFileButtonDirective } from './directives/kendo-editor-insert-file-button.directive';
import { KendoEditorCustomInsertIframeButtonDirective } from './directives/kendo-editor-insert-iframe-button.directive';
import { KendoEditorCustomInsertImageButtonDirective } from './directives/kendo-editor-insert-image-button.directive';
import { KendoEditorCustomInsertMathLiveButtonDirective } from './directives/kendo-editor-insert-mathlive-button.directive';
import { KendoEditorPopulatedFieldsButtonDirective } from './directives/kendo-editor-populated-fields-button.directive';
import { MathLiveEditorDialogComponent } from './components/math-live-editor-dialog/math-live-editor-dialog.component';
import { MediaPreloadDirective } from './directives/media-preload.directive';
import { MobilePreviewerComponent } from './components/mobile-previewer/mobile-previewer.component';
import { MobilePreviewerContentDirective } from './components/mobile-previewer/mobile-previewer-content.directive';
import { MultilineTextDirective } from './directives/multiline-text.directive';
import { NgSelectModule } from '@ng-select/ng-select';
import { OpalConfirmDialogComponent } from './services/dialog/confirm-dialog/confirm-dialog.component';
import { OpalDialogService } from './services/dialog/dialog.service';
import { OpalFileUploaderComponent } from './components/file-uploader/file-uploader.component';
import { OpalListComponent } from './components/opal-list/opal-list.component';
import { OpalListItemTemplateDirective } from './components/opal-list/opal-list.directives';
import { OpalNumericDirective } from './directives/opal-numeric.directive';
import { OpalPopupService } from './services/popup/popup.service';
import { OpalSelectComponent } from './components/select/select.component';
import { PopupContainerComponent } from './services/popup/popup-container/popup-container.component';
import { PopupContentDirective } from './services/popup/popup-container/popup-content.directive';
import { ReadMoreComponent } from './components/read-more/read-more.component';
import { RootElementScrollableService } from './services/root-element-scrollable.service';
import { SafeHtmlPipe } from './pipes/safe-html.pipe';
import { ScrollableMenuComponent } from './components/scrollable-menu/scrollable-menu.component';
import { SearchInputComponent } from './components/search-input/search-input.component';
import { StatusIndicatorComponent } from './components/status-indicator/status-indicator.component';
import { StickyElementDirective } from './directives/sticky/sticky-element.directive';
import { StickyGridHeaderElementDirective } from './directives/sticky/stick-grid-header-element.directive';
import { StickyTabHeaderElementDirective } from './directives/sticky/sticky-tab-header-element.directive';
import { TextBoxModule } from '@progress/kendo-angular-inputs';
import { UploadFileDialogComponent } from './components/upload-file-dialog/upload-file-dialog.component';
import { UploadImageDialogComponent } from './components/upload-image-dialog/upload-image-dialog.component';
import { VideoPlayerComponent } from './components/video-player/video-player.component';
import { ViewModeDirective } from './components/editable/view-mode.directive';

const COMMON_FRAGMENTS: Type<unknown>[] = [];

const COMMON_DIRECTIVES: Type<unknown>[] = [
  ExternalLinkGuardDirective,
  MediaPreloadDirective,
  KendoEditorCustomInsertImageButtonDirective,
  KendoEditorPopulatedFieldsButtonDirective,
  KendoEditorCustomInsertIframeButtonDirective,
  KendoEditorCustomInsertMathLiveButtonDirective,
  KendoEditorCustomInsertFileButtonDirective,
  OpalOptionTemplateDirective,
  OpalLabelTemplateDirective,
  MobilePreviewerContentDirective,
  EditModeDirective,
  ViewModeDirective,
  OpalNumericDirective,
  OpalListItemTemplateDirective,
  MultilineTextDirective,
  KendoEditorBrokenLinkScannerButtonDirective,
  PopupContentDirective,
  StickyElementDirective,
  StickyTabHeaderElementDirective,
  StickyGridHeaderElementDirective,
  ButtonTemplateDirective,
  MoreButtonTemplateDirective
];

const COMMON_PIPES: Type<unknown>[] = [SafeHtmlPipe];

const COMMON_COMPONENT: Type<unknown>[] = [
  CountdownTimerComponent,
  AudioPlayerComponent,
  DocumentPlayerComponent,
  CropImageDialogComponent,
  UploadImageDialogComponent,
  EmbedIframeDialogComponent,
  UploadFileDialogComponent,
  GuidelinePopulatedFieldsDialogComponent,
  MathLiveEditorDialogComponent,
  VideoPlayerComponent,
  ActionBarComponent,
  StatusIndicatorComponent,
  ScrollableMenuComponent,
  OpalConfirmDialogComponent,
  OpalSelectComponent,
  OpalListComponent,
  OpalFileUploaderComponent,
  SearchInputComponent,
  DetailTitleComponent,
  MobilePreviewerComponent,
  EditableComponent,
  ReadMoreComponent,
  DataCheckIndicatorComponent,
  DataFilterComponent,
  PopupContainerComponent,
  DropFileUploaderComponent,
  ButtonGroupComponent,
  VideojsPlayerComponent
];

@NgModule({
  imports: [
    FunctionModule,
    CommonModule,
    AmazonS3UploaderModule,
    ContextMenuModule,
    ButtonModule,
    TextBoxModule,
    DropDownsModule,
    AuthenticationModule,
    DocumentViewerModule,
    ImageCropperModule,
    DateInputsModule,
    DatePickerModule,
    NgSelectModule,
    GridModule,
    EditorModule
  ],
  declarations: [...COMMON_FRAGMENTS, ...COMMON_DIRECTIVES, ...COMMON_PIPES, ...COMMON_COMPONENT],
  entryComponents: [
    ...COMMON_FRAGMENTS,
    OpalConfirmDialogComponent,
    CropImageDialogComponent,
    EmbedIframeDialogComponent,
    UploadImageDialogComponent,
    UploadFileDialogComponent,
    MathLiveEditorDialogComponent,
    GuidelinePopulatedFieldsDialogComponent,
    PopupContainerComponent
  ],
  exports: [...COMMON_FRAGMENTS, ...COMMON_DIRECTIVES, ...COMMON_PIPES, ...COMMON_COMPONENT],
  providers: [OpalDialogService, OpalPopupService]
})
export class CommonComponentsModule {
  public static forRoot(): ModuleWithProviders[] {
    return [
      {
        ngModule: CommonComponentsModule,
        providers: [RootElementScrollableService, OpalDialogService, OpalPopupService]
      }
    ];
  }
  constructor() {
    this.registerVideojsPlugins();
  }

  private registerVideojsPlugins(): void {
    registerVideojsPlugins();
  }
}
