import { Utils } from './utils';

// @dynamic
export class DomUtils {
  /**
   * Prevent default for common event.
   * @param event: event
   */
  public static preventDefaultEvent(event: Event): void {
    event.cancelBubble = true;
    event.preventDefault();
    event.stopPropagation();
  }

  public static closest(element: HTMLElement | Element, predicate: (p: HTMLElement) => boolean): HTMLElement | undefined {
    const parentEl = element.parentElement;
    if (parentEl == null) {
      return undefined;
    }
    return predicate(parentEl) ? parentEl : DomUtils.closest(parentEl, predicate);
  }

  public static findClosestVerticalScrollableParent(
    element: HTMLElement | Element,
    checkContentOverflow: boolean = false
  ): HTMLElement | undefined {
    return DomUtils.closest(element, p => {
      const overflowY = window.getComputedStyle(p).overflowY;
      return (overflowY === 'auto' || overflowY === 'scroll') && (!checkContentOverflow || DomUtils.isContentVerticalOverflow(p));
    });
  }

  public static isContentVerticalOverflow(element: HTMLElement | Element): boolean {
    const elementRect = element.getBoundingClientRect();
    for (let i = 0; i < element.children.length; i++) {
      const child = element.children[i];
      const childRect = child.getBoundingClientRect();
      if (childRect.top < elementRect.top || childRect.bottom > elementRect.bottom) {
        return true;
      }
    }
    return false;
  }

  public static getElementChildsTotalHeight(element: HTMLElement | Element): number {
    let result = 0;
    for (let i = 0; i < element.children.length; i++) {
      const child = element.children[i];
      result += child.clientHeight;
    }
    return result;
  }

  public static scrollToView(element: HTMLElement | Element, force: boolean = false): void {
    const nearestScrollContainerEl = DomUtils.findClosestVerticalScrollableParent(element, !force);
    DomUtils.scrollToChild(nearestScrollContainerEl != null ? nearestScrollContainerEl : window, element, !force);
  }

  public static scrollToChild(
    parent: HTMLElement | Element | Window,
    child: HTMLElement | Element | undefined,
    onlyIfNeeded: boolean = false,
    margin: number = 5
  ): void {
    if (child == null) {
      return;
    }
    const childTopPosition = child.getBoundingClientRect().top;
    const parentTopPosition = parent instanceof Window ? 0 : parent.getBoundingClientRect().top;
    const childBottomPosition = child.getBoundingClientRect().bottom;
    const parentBottomPosition = parent instanceof Window ? window.innerHeight : parent.getBoundingClientRect().bottom;
    if (onlyIfNeeded) {
      if (childTopPosition < parentTopPosition) {
        scrollVerticalByDistance(parent, childTopPosition - parentTopPosition - margin);
      } else if (childBottomPosition > parentBottomPosition) {
        scrollVerticalByDistance(parent, childBottomPosition - parentBottomPosition + margin);
      }
    } else {
      scrollVerticalByDistance(parent, childTopPosition - parentTopPosition - margin);
    }

    function scrollVerticalByDistance(scrollContainer: HTMLElement | Element | Window, scrollDistance: number): void {
      if (scrollContainer instanceof Window) {
        scrollContainer.scrollTo(scrollContainer.scrollX, scrollContainer.scrollY + scrollDistance);
      } else {
        scrollContainer.scrollTop += scrollDistance;
      }
    }
  }

  public static findFirstElement(
    rootElement: HTMLElement,
    elFilter: (node: HTMLElement | Element) => boolean,
    isFindRecursively?: (node: HTMLElement | Element) => boolean
  ): HTMLElement | Element | undefined {
    if (rootElement == null || rootElement.children == null) {
      return undefined;
    }
    for (let i = 0; i < rootElement.children.length; i++) {
      const element = rootElement.children[i];
      if (elFilter(element)) {
        return element;
      } else if (element.childNodes.length && (isFindRecursively == null || isFindRecursively(element))) {
        const recursiveResult = this.findFirstElement(<HTMLElement>element, elFilter, isFindRecursively);
        if (recursiveResult != null) {
          return recursiveResult;
        }
      }
    }
    return undefined;
  }

  public static addClass(element: HTMLElement, className: string): void {
    if (className) {
      element.classList.remove(className);
      element.classList.add(className);
    }
  }

  public static removeClass(element: HTMLElement, predicate: (className: string) => boolean): void {
    const classList: string[] = [];
    element.classList.forEach(item => classList.push(item));
    classList.forEach(item => {
      if (predicate(item)) {
        element.classList.remove(item);
      }
    });
  }

  public static getElementContentRect(element: HTMLElement): ClientRect {
    const style = window.getComputedStyle(element);
    const paddingLeft = style.paddingLeft != null ? parseFloat(style.paddingLeft) : 0;
    const paddingRight = style.paddingRight != null ? parseFloat(style.paddingRight) : 0;
    const paddingTop = style.paddingTop != null ? parseFloat(style.paddingTop) : 0;
    const paddingBottom = style.paddingBottom != null ? parseFloat(style.paddingBottom) : 0;

    const elementRect: ClientRect = Utils.assign(<ClientRect>{}, element.getBoundingClientRect());
    return Utils.clone(elementRect, p => {
      p.bottom = p.bottom - paddingBottom;
      p.right = p.right - paddingRight;
      // tslint:disable-next-line:no-any
      (<any>p).height = element.clientHeight - paddingTop - paddingBottom;
      // tslint:disable-next-line:no-any
      (<any>p).width = element.clientWidth - paddingLeft - paddingRight;
    });
  }

  public static getComputedStyle(el: HTMLElement, styleName: string): string {
    try {
      return window.getComputedStyle(el, undefined).getPropertyValue(styleName);
    } catch (e) {
      return el.style[styleName];
    }
  }

  public static isChildVisibleInParentViewPort(parentEl: HTMLElement, childEl: HTMLElement, checkFullVisible: boolean = false): boolean {
    if (childEl.clientHeight === 0 || childEl.clientWidth === 0) {
      return false;
    }
    const childElRect = childEl.getBoundingClientRect();
    const parentElRect = parentEl.getBoundingClientRect();

    if (checkFullVisible) {
      return childElRect.top >= parentElRect.top && childElRect.top + childElRect.height <= parentElRect.top + parentElRect.height;
    }

    return (
      (childElRect.top <= parentElRect.top && childElRect.top + childElRect.height > parentElRect.top) ||
      (childElRect.top >= parentElRect.top && childElRect.top < parentElRect.top + parentElRect.height)
    );
  }

  public static isChildVisibleFromTopInParentViewPort(parentEl: HTMLElement, childEl: HTMLElement): boolean {
    if (childEl.clientHeight === 0 || childEl.clientWidth === 0) {
      return false;
    }
    const childElRect = childEl.getBoundingClientRect();
    const parentElRect = parentEl.getBoundingClientRect();

    return childElRect.top >= parentElRect.top && childElRect.top < parentElRect.top + parentElRect.height;
  }

  public static getComputedPxSizeStyle(el: HTMLElement, styleName: string): number {
    const computedFontSizeString: string = DomUtils.getComputedStyle(el, styleName);
    if (computedFontSizeString === '0') {
      return 0;
    }
    if (computedFontSizeString.indexOf('px') < 0) {
      throw new Error('Style is invalid');
    }
    return parseFloat(computedFontSizeString.substr(0, computedFontSizeString.length - 2));
  }
}
