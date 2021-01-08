import { ChangeDetectorRef, Component, ElementRef, NgZone, forwardRef } from '@angular/core';
import {
  PasteCleanupSettings as CommonPasteCleanUpSettings,
  EditorState,
  EditorView,
  Transaction,
  baseKeymap,
  buildKeymap,
  buildListKeymap,
  columnResizing,
  hasSameMarkup,
  history,
  keymap,
  parseContent,
  pasteCleanup,
  removeAttribute,
  removeComments,
  sanitize,
  sanitizeClassAttr,
  sanitizeStyleAttr
} from '@progress/kendo-editor-common';
import { EditorComponent, EditorLocalizationService } from '@progress/kendo-angular-editor';
import { KendoInput, isDocumentAvailable } from '@progress/kendo-angular-common';
import { L10N_PREFIX, LocalizationService } from '@progress/kendo-angular-l10n';
import { auditTime, filter, map } from 'rxjs/operators';
import { conditionallyExecute, removeEmptyEntries } from '@progress/kendo-angular-editor/dist/es2015/util';
import { defaultStyle, rtlStyles, tablesStyles } from '@progress/kendo-angular-editor/dist/es2015/common/styles';
import { fromEvent, merge, pipe } from 'rxjs';

import { DialogService } from '@progress/kendo-angular-dialog';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { PasteCleanupSettings } from '@progress/kendo-angular-editor/dist/es2015/common/paste-cleanup-settings';
import { getToolbarState } from '@progress/kendo-angular-editor/dist/es2015/editor-toolbar-state';
import { schema } from '@progress/kendo-angular-editor/dist/es2015/config/schema';

// tslint:disable:no-any

const defaultPasteCleanupSettings: PasteCleanupSettings = {
  convertMsLists: false,
  removeAttributes: [],
  removeHtmlComments: false,
  removeInvalidHTML: false,
  removeMsClasses: false,
  removeMsStyles: false,
  stripTags: ['']
};

const getPasteCleanupAttributes = (config: PasteCleanupSettings): CommonPasteCleanUpSettings['attributes'] => {
  if (config.removeAttributes === 'all') {
    return { '*': removeAttribute };
  }

  const initial = removeEmptyEntries({
    style: config.removeMsStyles ? sanitizeStyleAttr : undefined,
    class: config.removeMsClasses ? sanitizeClassAttr : undefined
  });

  return config.removeAttributes.reduce((acc, curr) => ({ ...acc, [curr]: removeAttribute }), initial);
};

const removeCommentsIf = conditionallyExecute(removeComments);

const removeInvalidHTMLIf = conditionallyExecute(sanitize);

// Override kendo-editor to add columnResizing plugins for handle resize table column in editor
@Component({
  selector: 'opal-kendo-editor',
  providers: [
    EditorLocalizationService,
    {
      provide: LocalizationService,
      useExisting: EditorLocalizationService
    },
    {
      provide: L10N_PREFIX,
      useValue: 'kendo.editor'
    },
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EditorComponent),
      multi: true
    },
    {
      provide: KendoInput,
      useExisting: forwardRef(() => EditorComponent)
    },
    {
      provide: EditorComponent,
      useExisting: forwardRef(() => OpalKendoEditorComponent)
    }
  ],

  templateUrl: './opal-kendo-editor-component.html'
})
export class OpalKendoEditorComponent extends EditorComponent {
  constructor(protected ds: DialogService, l: LocalizationService, c: ChangeDetectorRef, n: NgZone) {
    super(ds, l, c, n);
    const thisObj = this as any;
    thisObj.initialize = this.internalInitialize.bind(this);
  }

  public internalInitialize(): void {
    if (!isDocumentAvailable()) {
      return;
    }

    const thisObj = this as any;

    const containerNativeElement: HTMLElement = this.container.element.nativeElement;
    const contentNode: any = parseContent((this.value || '').trim(), schema);

    if (this.iframe) {
      const iframeDoc = (<HTMLIFrameElement>containerNativeElement).contentDocument;

      const meta = iframeDoc.createElement('meta');
      meta.setAttribute('charset', 'utf-8');
      iframeDoc.head.appendChild(meta);

      [defaultStyle, tablesStyles, this.dir === 'rtl' ? rtlStyles : undefined].forEach(styles => {
        if (styles) {
          const style = iframeDoc.createElement('style');
          style.appendChild(iframeDoc.createTextNode(styles));
          iframeDoc.head.appendChild(style);
        }
      });

      const element = iframeDoc.createElement('div');
      iframeDoc.body.appendChild(element);
    } else {
      const element = document.createElement('div');
      containerNativeElement.appendChild(element);
    }

    const state = EditorState.create({
      schema: schema,
      doc: contentNode,
      plugins: [columnResizing({}), history(), keymap(buildListKeymap(schema)), keymap(buildKeymap(schema)), keymap(baseKeymap)]
    });

    if (this.iframe) {
      this.viewMountElement = (<HTMLIFrameElement>containerNativeElement).contentDocument.querySelector('div');
    } else {
      this.viewMountElement = containerNativeElement.querySelector('div');
    }

    thisObj.ngZone.runOutsideAngular(() => {
      thisObj.view = new EditorView(
        { mount: this.viewMountElement },
        {
          state,
          dispatchTransaction: function(tr: Transaction): void {
            // `this` is bound to the view instance.
            if (!(thisObj.disabled || thisObj.readonly)) {
              this.updateState(this.state.apply(tr));
              thisObj.stateChange.next(getToolbarState(this.state));
              // thisObj.cdr.detectChanges();
              // When the user utilizes keyboard shortcuts&mdash;for example, `Ctrl`+`b`&mdash;
              // `tr.docChanged` is `true` and the toolbar is not updated.
              // A possible future solution is to move the keymaps to the service.
              // if (!tr.docChanged) {
              //     thisObj.stateChange.emit(updateToolBar(thisObj.view));
              // }

              const value = thisObj.value;
              if (!hasSameMarkup(value, thisObj._previousValue, this.state.schema)) {
                thisObj._previousValue = value;
                thisObj.ngZone.run(() => thisObj.valueModified.next(value));
              }
            }
          },
          transformPastedHTML: (html: string): string => {
            const pasteCleanupSettings = { ...defaultPasteCleanupSettings, ...this.pasteCleanupSettings };

            const clean = pipe(
              removeCommentsIf(pasteCleanupSettings.removeHtmlComments),
              removeInvalidHTMLIf(pasteCleanupSettings.removeInvalidHTML)
            )(html);

            const attributes = getPasteCleanupAttributes(pasteCleanupSettings);

            return pasteCleanup(clean, {
              convertMsLists: pasteCleanupSettings.convertMsLists,
              stripTags: pasteCleanupSettings.stripTags.join('|'),
              attributes
            });
          }
        }
      );
    });

    thisObj.subs.add(
      merge(
        this.stateChange.pipe(
          filter(() => this.updateInterval > 0),
          auditTime(this.updateInterval)
        ),
        this.stateChange.pipe(filter(() => this.updateInterval === 0))
      ).subscribe(() => {
        if (this.userToolBarComponent) {
          (<any>this.userToolBarComponent).cdr.detectChanges();
        } else {
          thisObj.cdr.detectChanges();
        }
      })
    );

    thisObj.subs.add(
      merge(
        this.valueModified.pipe(
          filter(() => this.updateInterval > 0),
          auditTime(this.updateInterval)
        ),
        this.valueModified.pipe(filter(() => this.updateInterval === 0))
      ).subscribe((value: string) => {
        thisObj.onChangeCallback(value);
        this.valueChange.emit(value);
        thisObj.cdr.markForCheck();
      })
    );

    thisObj.subs.add(
      fromEvent(this.viewMountElement, 'keyup')
        .pipe(
          map((e: any) => e.keyCode),
          filter((code: number) => code === 121), // F10
          map(() => this.userToolBarElement || thisObj.defaultToolbar)
        )
        .subscribe((toolbar: ElementRef) => toolbar.nativeElement.focus())
    );

    thisObj.subs.add(
      fromEvent(this.viewMountElement, 'blur')
        .pipe(filter((event: any) => !this.viewMountElement.contains(event.relatedTarget)))
        .subscribe(() => thisObj.onTouchedCallback())
    );
  }
}
