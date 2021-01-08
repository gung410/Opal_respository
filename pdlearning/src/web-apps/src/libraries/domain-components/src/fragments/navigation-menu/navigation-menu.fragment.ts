import { ChangeDetectorRef, Component, ElementRef, HostBinding, Renderer2 } from '@angular/core';

import { Fragment } from '@opal20/infrastructure';
import { FragmentPosition } from '../fragment-position';
import { NavigationMenuService } from './navigation-menu.service';

@Component({
  selector: 'navigation-menu-fragment',
  templateUrl: 'navigation-menu.html'
})
export class NavigationMenuFragment extends Fragment {
  protected position: string = FragmentPosition.NavigationMenu;

  constructor(
    protected renderer: Renderer2,
    protected changeDetectorRef: ChangeDetectorRef,
    protected elementRef: ElementRef,
    public navigationMenuService: NavigationMenuService
  ) {
    super(renderer, changeDetectorRef, elementRef);

    this.subscribe(this.navigationMenuService.onHide, () => this.hide());
    this.subscribe(this.navigationMenuService.onShow, () => this.show());
  }

  @HostBinding('class.navigation-menu')
  public getNavigationMenuClass(): boolean {
    return true;
  }
}
