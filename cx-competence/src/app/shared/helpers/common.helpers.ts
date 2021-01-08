import { Injectable } from '@angular/core';

@Injectable()
export class CommonHelpers {
  /**
   * Gets an enumeration given a case-insensitive key. For a numeric enum this uses
   * its members' names; for a string enum this searches the specific string values.
   * Logs a warning if the letter case was ignored to find a match, and logs an error
   * including the supported values if no match was found.
   */
  static toEnumIgnoreCase<T>(
    target: T,
    caseInsensitiveKey: string
  ): T[keyof T] {
    const needle = caseInsensitiveKey.toLowerCase();

    // If the enum Object does not have a key "0", then assume a string enum
    const key = Object.keys(target).find(
      (k) => (target['0'] ? k : target[k]).toLowerCase() === needle
    );

    if (!key) {
      const expected = Object.keys(target)
        .map((k) => (target['0'] ? k : target[k]))
        // tslint:disable-next-line:radix
        .filter((k) => isNaN(Number.parseInt(k)))
        .join(', ');
      console.error(
        `Could not map '${caseInsensitiveKey}' to values ${expected}`
      );

      return undefined;
    }

    const name = target['0'] ? key : target[key];
    if (name !== caseInsensitiveKey) {
      console.warn(
        `Ignored case to map ${caseInsensitiveKey} to value ${name}`
      );
    }

    return target[key];
  }

  static freezeAgGridHeader(): any {
    const headerHeight = 128;
    const freezeFunction = () => {
      const agGridHeader = document.querySelector(
        'ag-grid-angular.ag-grid-header-floatable .ag-header'
      );

      if (!agGridHeader) {
        return;
      }

      const fixedClassName = 'ag-grid-header-fixed';
      const currentScrollHeight =
        window.pageYOffset ||
        document.documentElement.scrollTop ||
        document.body.scrollTop ||
        0;
      if (currentScrollHeight > headerHeight) {
        agGridHeader.classList.add(fixedClassName);
      } else {
        agGridHeader.classList.remove(fixedClassName);
      }
    };

    return freezeFunction;
  }

  static freezeAgGridScroll(): any {
    const bottomOffset = 200;
    const freezeFunction = () => {
      const agGridScroll = document.querySelector(
        'ag-grid-angular.ag-grid-scroll-floatable .ag-body-horizontal-scroll'
      );

      if (!agGridScroll) {
        return;
      }

      const unfixedClassName = 'ag-body-horizontal-scroll-unfixed';

      if (
        window.innerHeight + window.scrollY >=
        document.body.offsetHeight - bottomOffset
      ) {
        agGridScroll.classList.add(unfixedClassName);
      } else {
        agGridScroll.classList.remove(unfixedClassName);
      }
    };

    return freezeFunction;
  }
}
