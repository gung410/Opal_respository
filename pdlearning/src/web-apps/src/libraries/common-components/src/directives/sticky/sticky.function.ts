import { DomUtils, ZINDEX_LEVEL_3 } from '@opal20/infrastructure';

import { Renderer2 } from '@angular/core';
import { TabStripComponent } from '@progress/kendo-angular-layout';

export const SPACING_CONTENT: number = 24;

export function setElementSticky(
  renderer: Renderer2,
  element: HTMLElement,
  dependElement: HTMLElement | TabStripComponent,
  spacing: number
): number {
  renderer.setStyle(element, 'position', 'sticky');
  renderer.setStyle(element, 'z-index', `${ZINDEX_LEVEL_3}`);

  const top = getStickyTopPosition(dependElement) + spacing;
  renderer.setStyle(element, 'top', `${top}px`);

  return top;
}

export function setGridSticky(
  renderer: Renderer2,
  element: HTMLElement,
  dependElement: HTMLElement | TabStripComponent,
  spacing: number
): number {
  renderer.setStyle(element, 'width', `${element.offsetWidth}px`);
  renderer.setStyle(element, 'position', 'fixed');
  renderer.setStyle(element, 'z-index', `${ZINDEX_LEVEL_3}`);

  const top = getStickyTopPosition(dependElement) + spacing;
  renderer.setStyle(element, 'top', `${top}px`);
  return top;
}

export function getStickyTopPosition(dependElement: HTMLElement | TabStripComponent): number {
  let dependHTMLElement: HTMLElement = null;

  if (dependElement instanceof HTMLElement) {
    dependHTMLElement = dependElement;
  } else if (dependElement instanceof TabStripComponent) {
    // tslint:disable-next-line:no-any
    dependHTMLElement = (dependElement as any).wrapper.nativeElement.children[0];
  }

  let stickyTop = 0;

  if (dependElement != null) {
    const stickyTopOfDependElement = DomUtils.getComputedPxSizeStyle(dependHTMLElement, 'top');
    const heightElement = dependHTMLElement.offsetHeight;
    stickyTop = stickyTopOfDependElement + heightElement;
  }

  return stickyTop;
}
