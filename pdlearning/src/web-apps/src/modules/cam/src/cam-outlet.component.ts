import { BaseModuleOutlet, FragmentRegistry, ModuleFlowManager, ModuleInstance } from '@opal20/infrastructure';
import { Component, OnDestroy, OnInit } from '@angular/core';

import { RootElementScrollableService } from '@opal20/common-components';
import { Subscription } from 'rxjs';

@Component({
  selector: 'cam-outlet',
  template: `
    <div class="match-parent column">
      <ng-template [portalOutlet]="fragmentRegistry.get('f-navigation-menu')"></ng-template>
      <div class="page-content">
        <broadcast-message-notification></broadcast-message-notification>
        <div class="d-flex-column flex-grow-1 flex-shrink-0">
          <ng-container #moduleContainer></ng-container>
        </div>
        <opal-footer></opal-footer>
      </div>
    </div>
    <div *ngIf="showScrollToTop" class="scroll-to-top" (click)="onScrollToTop()">
      <div class="scroll-to-top__container">
        <div class="scroll-to-top__chevron"></div>
        <div class="scroll-to-top__chevron"></div>
        <div class="scroll-to-top__chevron"></div>
      </div>
    </div>
  `
})
export class CAMOutletComponent extends BaseModuleOutlet implements OnInit, OnDestroy {
  public showScrollToTop: boolean = false;

  private subscription: Subscription = new Subscription();
  constructor(
    protected moduleInstance: ModuleInstance,
    public fragmentRegistry: FragmentRegistry,
    protected moduleFlowManager: ModuleFlowManager,
    private rootElementScrollableService: RootElementScrollableService
  ) {
    super(moduleInstance, fragmentRegistry, moduleFlowManager);
  }

  public ngOnInit(): void {
    this.subscription.add(
      this.rootElementScrollableService.onScroll$.subscribe((htmlElement: HTMLElement) => {
        if (htmlElement != null) {
          // Height of header and navigation menu
          this.showScrollToTop = htmlElement.scrollTop > 118;
        }
      })
    );
  }

  public onScrollToTop(): void {
    this.rootElementScrollableService.scrollToTop();
  }

  public ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
