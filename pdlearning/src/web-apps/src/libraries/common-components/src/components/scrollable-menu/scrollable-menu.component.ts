import { BaseComponent, DomUtils, ModuleFacadeService } from '@opal20/infrastructure';
import { ChangeDetectorRef, Component, EventEmitter, Input, Output } from '@angular/core';

import { RootElementScrollableService } from '../../services/root-element-scrollable.service';
import { ScrollableMenu } from './../../models/scrollable-menu.model';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'scrollable-menu',
  templateUrl: './scrollable-menu.component.html'
})
export class ScrollableMenuComponent extends BaseComponent {
  @Input() public title: string;
  @Input() public tabs: ScrollableMenu[] = [];
  @Input() public activeTabId?: string;
  @Output() public activeChange: EventEmitter<ScrollableMenu> = new EventEmitter();
  @Output() public activeTabIdChange: EventEmitter<string> = new EventEmitter();

  private _prevIsHiddenValDic: Dictionary<boolean> = {};
  private contentContainer: HTMLElement;
  constructor(
    moduleFacadeService: ModuleFacadeService,
    private rootElementScrollableService: RootElementScrollableService,
    public changeDetectorRef: ChangeDetectorRef
  ) {
    super(moduleFacadeService);
  }

  public onSelectTab(t: ScrollableMenu): void {
    this.activeTab(t, true);
  }

  public activeTab(tab: ScrollableMenu, scrollToTab: boolean): void {
    this.activeTabId = tab.id;
    this.activeChange.next(tab);
    this.activeTabIdChange.next(tab.id);

    if (scrollToTab) {
      const element: HTMLElement = tab.elementFn() ? tab.elementFn().nativeElement : null;
      if (element) {
        element.scrollIntoView({ behavior: 'smooth', block: 'start', inline: 'nearest' });
      }
    }
  }

  public isHidden(t: ScrollableMenu): boolean {
    const newVal =
      (t.isHidden != null && t.isHidden()) ||
      t.elementFn() == null ||
      t.elementFn().nativeElement == null ||
      DomUtils.getComputedStyle(t.elementFn().nativeElement, 'display') === 'none';

    // To fix ExpressionChangedAfterItHasBeenCheckedError
    if (newVal !== this._prevIsHiddenValDic[t.id] && this.initiated) {
      this._prevIsHiddenValDic[t.id] = newVal;
      this.changeDetectorRef.detectChanges();
    }
    return newVal;
  }

  /**
   * Active tab on scroll rule:
   * - Active for first tab which is full visible both top and bottom. If not exist,
   * - Active for first tab which is visible from top (top of the tab is in viewport container). If not exist,
   * - Active for first tab is have a part visible in the container
   */
  public onScroll(rootElement: HTMLElement): void {
    this.contentContainer = rootElement;
    const visibleTabs = this.visibleTabs();
    const firstFullVisibleInContainerTab = visibleTabs.find(tab =>
      DomUtils.isChildVisibleInParentViewPort(this.contentContainer, tab.elementFn().nativeElement, true)
    );
    if (firstFullVisibleInContainerTab != null) {
      this.activeTab(firstFullVisibleInContainerTab, false);
      return;
    }

    const firstVisibleFromTopInContainerTab = visibleTabs.find(tab =>
      DomUtils.isChildVisibleFromTopInParentViewPort(this.contentContainer, tab.elementFn().nativeElement)
    );
    if (firstVisibleFromTopInContainerTab != null) {
      this.activeTab(firstVisibleFromTopInContainerTab, false);
      return;
    }

    const firstVisibleInContainerTab = visibleTabs.find(tab =>
      DomUtils.isChildVisibleInParentViewPort(this.contentContainer, tab.elementFn().nativeElement)
    );
    if (firstVisibleInContainerTab != null) {
      this.activeTab(firstVisibleInContainerTab, false);
      return;
    }
  }

  public visibleTabs(): ScrollableMenu[] {
    return this.tabs.filter(tab => this.isVisibleTab(tab));
  }

  public isVisibleTab(tab: ScrollableMenu): boolean {
    return !(this.isHidden(tab) || tab.elementFn() == null);
  }

  protected onInit(): void {
    this.subscribe(this.rootElementScrollableService.onScroll$.pipe(debounceTime(100)), htmlElement => {
      if (htmlElement) {
        this.onScroll(htmlElement);
      }
    });
    if (this.activeTabId == null && this.visibleTabs().length > 0) {
      this.activeTabId = this.visibleTabs()[0].id;
    }
  }

  protected onAfterViewInit(): void {
    if (this.activeTabId == null && this.visibleTabs().length > 0) {
      // SetTimeout to delay to prevent expression has changed after view checked
      setTimeout(() => {
        this.activeTab(this.visibleTabs()[0], false);
      });
    }
  }
}
