import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { ChangeDetectorRef, Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, TemplateRef } from '@angular/core';

@Component({
  selector: 'content-dialog',
  templateUrl: './content-dialog.component.html'
})
export class ContentDialogComponent extends BaseComponent implements OnChanges {
  @Input() public title: string = '';
  @Input() public generalTabTemplate: TemplateRef<unknown> | undefined;
  @Input() public metaDataTabTemplate: TemplateRef<unknown> | undefined;
  @Input() public digitalRightTabTemplate: TemplateRef<unknown> | undefined;
  @Input() public activeTab: ContentDialogComponentTabType | undefined;
  @Input() public displaySaveBtn: boolean = true;
  @Input() public displayStopBtn: boolean = false;
  @Input() public displayCancelBtn: boolean = true;
  @Input() public displayCreateBtn: boolean = false;
  @Input() public displayGeneralTab: boolean = true;
  @Input() public displayMetaDataTab: boolean = true;
  @Input() public displayDigitalRightTab: boolean = true;
  @Input() public hasErrorTabs: ContentDialogComponentTabType[] | undefined;
  @Input() public highlightErrorTab: boolean = false;

  @Output('save') public saveEvent: EventEmitter<Event> = new EventEmitter();
  @Output('stop') public stopEvent: EventEmitter<Event> = new EventEmitter();
  @Output('cancel') public cancelEvent: EventEmitter<Event> = new EventEmitter();
  @Output('activeTabChange') public activeTabChangeEvent: EventEmitter<ContentDialogComponentTabType | undefined> = new EventEmitter();

  public tabType: typeof ContentDialogComponentTabType = ContentDialogComponentTabType;
  public tabs: ContentDialogComponentTabType[] = [];

  constructor(moduleFacadeService: ModuleFacadeService, private changeDetectorRef: ChangeDetectorRef) {
    super(moduleFacadeService);
  }

  public ngOnChanges(changes: SimpleChanges): void {
    if (!this.initiated) {
      return;
    }
    this.tabs = this.getTabs();
  }

  public ngOnInit(): void {
    super.ngOnInit();
    this.tabs = this.getTabs();
  }

  public selectTab(tab: ContentDialogComponentTabType | undefined): void {
    this.activeTab = tab;
    this.changeDetectorRef.detectChanges();
    this.activeTabChangeEvent.emit(tab);
  }

  public isTabHasError(tab: ContentDialogComponentTabType): boolean {
    return this.hasErrorTabs && this.hasErrorTabs.indexOf(tab) >= 0;
  }

  public showFirstErrorTab(hasErrorTabs?: ContentDialogComponentTabType[]): void {
    if (hasErrorTabs != null) {
      this.hasErrorTabs = hasErrorTabs;
    }
    if (this.hasErrorTabs.length > 0) {
      this.selectTab(this.hasErrorTabs[0]);
    }
  }

  private getTabs(): ContentDialogComponentTabType[] {
    const result: ContentDialogComponentTabType[] = [];
    if (this.generalTabTemplate !== undefined && this.displayGeneralTab) {
      result.push(ContentDialogComponentTabType.General);
    }
    if (this.metaDataTabTemplate !== undefined && this.displayMetaDataTab) {
      result.push(ContentDialogComponentTabType.MetaData);
    }
    if (this.digitalRightTabTemplate !== undefined && this.displayDigitalRightTab) {
      result.push(ContentDialogComponentTabType.DigitalRight);
    }
    return result;
  }
}

export enum ContentDialogComponentTabType {
  General = 'General',
  MetaData = 'MetaData',
  DigitalRight = 'DigitalRight'
}
