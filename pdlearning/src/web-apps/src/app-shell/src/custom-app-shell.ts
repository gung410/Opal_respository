import { AppShell, FragmentRegistry, GlobalScheduleService, ModuleCompiler, ScheduledTask, ShellManager } from '@opal20/infrastructure';
import { Component, ElementRef, HostListener, Injector, Renderer2 } from '@angular/core';

import { AuthService } from '@opal20/authentication';
import { RootElementScrollableService } from '@opal20/common-components';

@Component({
  selector: 'app-shell',
  template: `
    <ng-template [portalOutlet]="fragmentRegistry.get('f-header')"></ng-template>
    <div spinner class="page-wrapper row flex">
      <ng-template #mainFragment></ng-template>
    </div>
  `,
  providers: [FragmentRegistry]
})
export class CustomAppShell extends AppShell {
  constructor(
    public renderer: Renderer2,
    public injector: Injector,
    public fragmentRegistry: FragmentRegistry,
    public shellManager: ShellManager,
    public compiler: ModuleCompiler,
    private globalScheduleService: GlobalScheduleService,
    private authService: AuthService,
    private elementRef: ElementRef,
    private rootElementScrollableService: RootElementScrollableService
  ) {
    super(renderer, injector, fragmentRegistry, shellManager, compiler);
    this.registerScheduledTasks();
  }

  @HostListener('scroll')
  public onScroll(): void {
    const htmlElement = this.elementRef.nativeElement as HTMLElement;
    if (htmlElement != null) {
      this.rootElementScrollableService.emitScroll(htmlElement);
    }
  }

  private registerScheduledTasks(): void {
    this.authService.refreshToken();
    // 25 minutes = 1500000 millisecond.

    const refreshTokenTask = new ScheduledTask('refresh-token-task', 1500000, () => this.authService.triggerRefreshTokenLogic(), false);
    this.globalScheduleService.register(refreshTokenTask);
  }
}
