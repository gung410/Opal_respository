import { CxArrayUtil } from './array-util';

// @dynamic
export class CxHtmlUtil {
  public static addScript(src: string) {
    if (document.head === undefined) {
      throw new Error('Document head was undefined');
    }
    const scriptElement = document.createElement('script');
    scriptElement.setAttribute('src', src);
    document.head.appendChild(scriptElement);
  }

  public static getElementContentWidth(element: HTMLElement) {
    const style = window.getComputedStyle(element);
    const paddingLeft =
      style.paddingLeft !== undefined ? parseFloat(style.paddingLeft) : 0;
    const paddingRight =
      style.paddingRight !== undefined ? parseFloat(style.paddingRight) : 0;
    const result = element.clientWidth - paddingLeft - paddingRight;
    return result >= 0 ? result : 0;
  }

  public static getElementContentHeight(element: HTMLElement) {
    const style = window.getComputedStyle(element);
    const paddingTop =
      style.paddingTop !== undefined ? parseFloat(style.paddingTop) : 0;
    const paddingBottom =
      style.paddingBottom !== undefined ? parseFloat(style.paddingBottom) : 0;
    const result = element.clientHeight - paddingTop - paddingBottom;
    return result >= 0 ? result : 0;
  }

  public static addClass(element: HTMLElement, className: string) {
    if (className) {
      element.classList.remove(className);
      element.classList.add(className);
    }
  }

  public static removeClass(element: HTMLElement, className: string) {
    if (className) {
      element.classList.remove(className);
    }
  }

  public static replaceClass(
    element: HTMLElement,
    oldClass: string,
    newClass: string
  ) {
    if (oldClass) {
      element.classList.remove(oldClass);
    }
    if (newClass) {
      element.classList.add(newClass);
    }
  }

  public static createHtmlElWithContent(tagName: string, content: string) {
    const result = document.createElement(tagName);
    result.innerHTML = content;
    return result;
  }

  public static getElStyleNumberValue(
    value: string | undefined,
    unit: string = 'px'
  ) {
    if (value === undefined) {
      return undefined;
    }
    const intValueAsString = value.replace(unit, '');
    try {
      const result = parseFloat(intValueAsString);
      return result.toString() === 'NaN' ? undefined : result;
    } catch (e) {
      {
        return undefined;
      }
    }
  }

  public static contains(
    parent: HTMLElement,
    child: Node | Element | HTMLElement
  ) {
    if (child === undefined) {
      return false;
    }
    if (parent === child) {
      return true;
    }
    let childNodeParent = child.parentElement;
    while (childNodeParent !== undefined) {
      if (childNodeParent === parent) {
        return true;
      }
      childNodeParent = childNodeParent.parentElement;
    }
    {
      return false;
    }
  }

  public static buildNewMouseEventData(clickEvent: any) {
    const el = document.elementFromPoint(
      clickEvent.clientX,
      clickEvent.clientY
    );
    if (el === undefined) {
      return undefined;
    }
    return {
      bubbles: true,
      cancelable: true,
      view: window,
      sourceCapabilities: clickEvent.originalEvent
        ? clickEvent.originalEvent.sourceCapabilities
        : undefined,
      which: clickEvent.which,
      screenX: clickEvent.screenX,
      screenY: clickEvent.screenY,
      pageX: clickEvent.pageX,
      pageY: clickEvent.pageY,
      clientX: clickEvent.clientX,
      clientY: clickEvent.clientY,
      x: clickEvent.clientX,
      y: clickEvent.clientY,
      button: clickEvent.button,
      buttons: clickEvent.buttons,
      target: el,
      toElement: el,
      currentTarget: el
    };
  }

  public static getTextFromHtmlString(htmlString: string) {
    const element = document.createElement('div');
    element.innerHTML = htmlString;
    return this.getText(element as Node);
  }

  public static getText(element: Node | HTMLElement): string {
    let ret = '';
    const nodeType = element.nodeType;

    if (
      nodeType === DomNodeType.elementNode ||
      nodeType === DomNodeType.documentNode ||
      nodeType === DomNodeType.documentFragmentNode
    ) {
      if (typeof element.textContent === 'string') {
        return element.textContent;
      } else {
        let elementChild: Node | undefined;
        for (
          elementChild = element.firstChild as Node | undefined;
          elementChild !== undefined;
          elementChild = elementChild.nextSibling as Node | undefined
        ) {
          ret += this.getText(elementChild);
        }
      }
    } else if (nodeType === DomNodeType.textNode) {
      return element.nodeValue !== undefined
        ? (element.nodeValue as string)
        : '';
    }

    return ret;
  }

  public static findTextNodes(rootElement: HTMLElement) {
    return this.findNodes(
      rootElement,
      (node: Node) => node.nodeType === DomNodeType.textNode,
      (node: Node) =>
        node.nodeType in
        [
          DomNodeType.elementNode,
          DomNodeType.documentNode,
          DomNodeType.documentFragmentNode
        ]
    );
  }

  public static findFirstNode(
    rootElement: HTMLElement,
    nodeFilter: (node: Node | HTMLElement) => boolean,
    isFindRecursively?: (node: Node) => boolean
  ): Node | HTMLElement | undefined {
    if (rootElement === undefined || rootElement.childNodes === undefined) {
      return undefined;
    }
    rootElement.childNodes.forEach(item => {
      const node = item;
      if (nodeFilter(node)) {
        return node;
      } else if (
        node.childNodes.length &&
        (isFindRecursively === undefined || isFindRecursively(node))
      ) {
        const recursiveResult = this.findFirstNode(
          node as HTMLElement,
          nodeFilter,
          isFindRecursively
        );
        if (recursiveResult !== undefined) {
          return recursiveResult;
        }
      }
    });
    return undefined;
  }

  public static findFirstElement(
    rootElement: HTMLElement,
    elFilter: (node: HTMLElement | Element) => boolean,
    isFindRecursively?: (node: HTMLElement | Element) => boolean
  ): HTMLElement | Element | undefined {
    if (rootElement === undefined || rootElement.children === undefined) {
      return undefined;
    }
    // tslint:disable-next-line:prefer-for-of
    for (let i = 0; i < rootElement.children.length; i++) {
      const element = rootElement.children[i];
      if (elFilter(element)) {
        return element;
      } else if (
        element.childNodes.length &&
        (isFindRecursively === undefined || isFindRecursively(element))
      ) {
        const recursiveResult = this.findFirstElement(
          element as HTMLElement,
          elFilter,
          isFindRecursively
        );
        if (recursiveResult !== undefined) {
          return recursiveResult;
        }
      }
    }
    return undefined;
  }

  public static findFirstElementByClass(
    rootElement: HTMLElement,
    classString: string
  ) {
    const allClasses = classString.split(' ');
    return CxHtmlUtil.findFirstElement(rootElement, (el: HTMLElement) =>
      CxArrayUtil.all(allClasses, className => el.classList.contains(className))
    );
  }

  public static findNodes(
    rootElement: HTMLElement,
    nodeFilter: (node: Node) => boolean,
    isFindRecursively?: (node: Node) => boolean
  ) {
    if (rootElement === undefined) {
      return [];
    }
    const result: Node[] = [];
    buildResult(rootElement);
    return result;

    function buildResult(element: HTMLElement) {
      if (element.childNodes === undefined) {
        return;
      }
      element.childNodes.forEach(item => {
        const node = item;
        if (nodeFilter(node)) {
          result.push(node);
        } else if (
          node.childNodes !== undefined &&
          node.childNodes.length > 0 &&
          (isFindRecursively === undefined || isFindRecursively(node))
        ) {
          buildResult(node as HTMLElement);
        }
      });
    }
  }

  public static findElements(
    rootElement: HTMLElement,
    elementFilter: (element: HTMLElement) => boolean,
    isFindRecursively?: (element: HTMLElement) => boolean
  ) {
    if (rootElement === undefined) {
      return [];
    }
    const result: HTMLElement[] = [];
    buildResult(rootElement);
    return result;

    function buildResult(element: HTMLElement) {
      if (element.children === undefined) {
        {
          return;
        }
      }
      // tslint:disable-next-line:prefer-for-of
      for (let i = 0; i < element.children.length; i++) {
        const childElement = element.children[i] as HTMLElement;
        if (childElement === undefined) {
          continue;
        }
        if (elementFilter(childElement)) {
          result.push(childElement);
        } else if (
          childElement.children !== undefined &&
          childElement.children.length > 0 &&
          (isFindRecursively === undefined || isFindRecursively(element))
        ) {
          buildResult(childElement);
        }
      }
    }
  }

  public static findElementsByClass(
    rootElement: HTMLElement,
    className: string
  ) {
    return CxHtmlUtil.findElements(rootElement, el =>
      el.classList.contains(className)
    );
  }

  public static getChildTextNode(
    elementNode: Node,
    atLast: boolean = false
  ): Node | undefined {
    if (
      elementNode === undefined ||
      elementNode.childNodes === undefined ||
      elementNode.childNodes.length === 0
    ) {
      return elementNode;
    }

    const selectedChildNode =
      elementNode.childNodes[atLast ? elementNode.childNodes.length - 1 : 0];
    if (selectedChildNode.nodeType === DomNodeType.textNode) {
      return selectedChildNode;
    }
    if (
      selectedChildNode.childNodes === undefined ||
      selectedChildNode.childNodes.length === 0
    ) {
      return elementNode;
    }
    return this.getChildTextNode(
      selectedChildNode.childNodes[
        atLast ? selectedChildNode.childNodes.length - 1 : 0
      ],
      atLast
    );
  }

  public static indexInParent(element: HTMLElement | undefined) {
    if (
      element === undefined ||
      element.parentElement === undefined ||
      element.parentElement.children === undefined
    ) {
      return -1;
    }

    for (let i = 0; i < element.parentElement.children.length; i++) {
      if (element.parentElement.children[i] === element) {
        return i;
      }
    }

    return -1;
  }

  public static getParentElementWithClassName(
    element: HTMLElement,
    className: string
  ) {
    if (element === undefined || element.parentElement === undefined) {
      return undefined;
    }

    let parentElement: HTMLElement | undefined = element.parentElement;
    while (parentElement !== undefined) {
      if (parentElement.classList.contains(className)) {
        return parentElement;
      }
      parentElement = parentElement.parentElement as HTMLElement | undefined;
    }

    return parentElement;
  }
  public static getComputedStyle(el: HTMLElement, styleName: string) {
    try {
      return window.getComputedStyle(el, undefined).getPropertyValue(styleName);
    } catch (e) {
      return el.style[styleName];
    }
  }

  public static getComputedPxSizeStyle(
    el: HTMLElement,
    styleName: string
  ): number {
    const computedFontSizeString: string = CxHtmlUtil.getComputedStyle(
      el,
      styleName
    );
    if (computedFontSizeString === '0') {
      return 0;
    }
    if (computedFontSizeString.indexOf('px') < 0) {
      throw new Error('Style is invalid');
    }
    return parseFloat(
      computedFontSizeString.substr(0, computedFontSizeString.length - 2)
    );
  }

  public static getFirstChildElementInParentViewPort(
    parentEl: HTMLElement,
    childEls?: HTMLElement[],
    startCheckIndex: number = 0,
    isDirectChild: boolean = true
  ) {
    const childElements: any =
      childEls !== undefined ? childEls : parentEl.children;
    if (childElements === undefined) {
      return undefined;
    }
    for (let i = startCheckIndex; i < childElements.length; i++) {
      if (
        this.isChildInParentViewPort(parentEl, childElements[i], isDirectChild)
      ) {
        return childElements[i];
      }
    }
    {
      return undefined;
    }
  }

  public static isBottomPartOfChildInParentViewPort(
    parentEl: HTMLElement,
    childEl: HTMLElement,
    isDirectChild: boolean = true
  ) {
    if (childEl.clientHeight === 0) {
      return false;
    }

    if (isDirectChild) {
      return (
        childEl.offsetTop < parentEl.scrollTop &&
        childEl.offsetTop + childEl.clientHeight > parentEl.scrollTop
      );
    } else {
      const childElTop = childEl.getBoundingClientRect().top;
      const childElHeight = childEl.getBoundingClientRect().height;
      const parentElTop = parentEl.getBoundingClientRect().top;

      return (
        childElTop < parentElTop && childElTop + childElHeight > parentElTop
      );
    }
  }

  public static isChildInParentViewPort(
    parentEl: HTMLElement,
    childEl: HTMLElement,
    isDirectChild: boolean = true
  ) {
    if (childEl.clientHeight === 0) {
      return false;
    }

    if (isDirectChild) {
      return (
        (childEl.offsetTop <= parentEl.scrollTop &&
          childEl.offsetTop + childEl.clientHeight > parentEl.scrollTop) ||
        (childEl.offsetTop >= parentEl.scrollTop &&
          childEl.offsetTop < parentEl.scrollTop + parentEl.clientHeight)
      );
    } else {
      const childElTop = childEl.getBoundingClientRect().top;
      const childElHeight = childEl.getBoundingClientRect().height;
      const parentElTop = parentEl.getBoundingClientRect().top;
      const parentElHeight = parentEl.getBoundingClientRect().height;

      return (
        (childElTop <= parentElTop &&
          childElTop + childElHeight > parentElTop) ||
        (childElTop >= parentElTop && childElTop < parentElTop + parentElHeight)
      );
    }
  }

  public static insertAfter(newNode: Node, referenceNode: Node) {
    if (referenceNode.parentNode === undefined) {
      return;
    }
    if (referenceNode.nextSibling !== undefined) {
      referenceNode.parentNode.insertBefore(newNode, referenceNode.nextSibling);
    } else {
      referenceNode.parentNode.appendChild(newNode);
    }
  }

  public static focus(
    element: HTMLInputElement,
    isSelect: boolean = false,
    delay?: number
  ) {
    if (delay !== undefined) {
      setTimeout(() => {
        doFocus();
      }, delay);
    } else {
      doFocus();
    }

    function doFocus() {
      element.focus();
      if (isSelect) {
        element.select();
      }
    }
  }

  public static getElementById(id: string) {
    return document.getElementById(id);
  }

  public static elementFromPoint(x: number, y: number): Element | null {
    return document.elementFromPoint(x, y);
  }

  public static scrollToChild(
    parent: HTMLElement | Element,
    child: HTMLElement | Element | undefined
  ) {
    if (child === undefined) {
      return;
    }
    const childTopPosition = child.getBoundingClientRect().top;
    const parentTopPosition = parent.getBoundingClientRect().top;
    parent.scrollTop += childTopPosition - parentTopPosition;
  }

  public static encode(htmlString: string) {
    return htmlString
      .replace(new RegExp('<', 'g'), '&lt;')
      .replace(new RegExp('>', 'g'), '&gt;');
  }

  public static setElementFontSizeScale(
    element: HTMLElement,
    fontSizeScale: number,
    minFontSizeInPx?: number
  ) {
    element.style.fontSize = `${fontSizeScale}%`;
    const currentElementFontSizeInPx = CxHtmlUtil.getComputedPxSizeStyle(
      element,
      'font-size'
    );
    if (
      minFontSizeInPx !== undefined &&
      currentElementFontSizeInPx < minFontSizeInPx
    ) {
      element.style.fontSize = `${minFontSizeInPx}px`;
    }
  }

  public static popElementTextCharacters(
    element: HTMLElement,
    removeCharacterCount: number
  ) {
    if (removeCharacterCount < 0) {
      throw new Error('removeCharacterCount cannot less than 0');
    }
    if (element.childElementCount === 0) {
      element.innerHTML =
        removeCharacterCount >= element.innerHTML.length
          ? ''
          : (element.innerHTML = element.innerHTML.substr(
              0,
              element.innerHTML.length - removeCharacterCount
            ));
    } else {
      if (removeCharacterCount >= CxHtmlUtil.getText(element).length) {
        element.innerHTML = '';
      } else {
        const allReversedTextNodes = CxHtmlUtil.findTextNodes(
          element
        ).reverse();
        if (allReversedTextNodes.length === 0) {
          return;
        }
        let currentRemovingCharactersNodeIndex = 0;
        for (let i = 0; i < removeCharacterCount; i++) {
          const currentRemovingCharactersNode =
            allReversedTextNodes[currentRemovingCharactersNodeIndex];
          if (
            currentRemovingCharactersNode.nodeValue !== undefined &&
            currentRemovingCharactersNode.nodeValue.length > 0
          ) {
            currentRemovingCharactersNode.nodeValue = currentRemovingCharactersNode.nodeValue.substr(
              0,
              currentRemovingCharactersNode.nodeValue.length - 1
            );
          } else {
            if (
              currentRemovingCharactersNodeIndex ===
              allReversedTextNodes.length - 1
            ) {
              {
                return;
              }
            }
            currentRemovingCharactersNodeIndex++;
            i--;
          }
        }
      }
    }
    let ret = '';
    const nodeType = element.nodeType;

    if (
      nodeType === DomNodeType.elementNode ||
      nodeType === DomNodeType.documentNode ||
      nodeType === DomNodeType.documentFragmentNode
    ) {
      if (typeof element.textContent === 'string') {
        return element.textContent;
      } else {
        let elementChild: Node | undefined;
        for (
          elementChild = element.firstChild as Node | undefined;
          elementChild !== undefined;
          elementChild = elementChild.nextSibling as Node | undefined
        ) {
          ret += this.getText(elementChild);
        }
      }
    } else if (nodeType === DomNodeType.textNode) {
      return element.nodeValue !== undefined
        ? (element.nodeValue as string)
        : '';
    }

    return ret;
  }

  public static pxToRem(value: number, rootFontPx?: number) {
    const htmlEl = document.querySelector('html') as HTMLElement;
    return (
      (value * 1.0) /
        (rootFontPx !== undefined
          ? rootFontPx
          : this.getComputedPxSizeStyle(htmlEl, 'font-size')) +
      'rem'
    );
  }
}

export const DomNodeType = {
  elementNode: 1,
  textNode: 3,
  commentNode: 8,
  documentNode: 9,
  documentTypeNode: 10,
  documentFragmentNode: 11
};
