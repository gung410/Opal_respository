import { ComponentPortal } from '../portal/portal';
import { Fragment } from './fragment';
import { Injectable } from '@angular/core';

@Injectable()
export class FragmentRegistry {
  private fragments: Map<string, ComponentPortal<Fragment>> = new Map();

  public get(position: string): ComponentPortal<Fragment> | undefined {
    return this.fragments.get(position);
  }

  public register(position: string, fragment: ComponentPortal<Fragment>): void {
    this.fragments.set(position, fragment);
  }

  public remove(position: string): void {
    const fragment: ComponentPortal<Fragment> | undefined = this.fragments.get(position);

    if (!fragment) {
      return;
    }

    if (fragment.isAttached) {
      fragment.detach();
    }

    this.fragments.delete(position);
  }

  public empty(): void {
    this.fragments = new Map();
  }
}
