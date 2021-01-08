import { Injectable } from '@angular/core';

@Injectable()
export class CommonHelpers {
  /**
   * Gets an enumeration given a case-insensitive key. For a numeric enum this uses
   * its members' names; for a string enum this searches the specific string values.
   * Logs a warning if the letter case was ignored to find a match, and logs an error
   * including the supported values if no match was found.
   */

  static freezeAgGridHeader(): any {
    const headerHeight = 128;
    const freezeFunction = () => {
      const userAccountDomainUrl = '/user-accounts';
      const agGridHeader = document.querySelector(
        'ag-grid-angular.ag-grid-header-floatable .ag-header'
      );

      const universalToolbarAction = document.querySelector(
        '.action-group-container'
      );

      const isOnUserAccountPage = window.location.href.includes(
        userAccountDomainUrl
      );

      if (!agGridHeader && !universalToolbarAction) {
        return;
      }

      const fixedAgGridHeaderClassName = 'ag-grid-header-fixed';
      const fixedAgGridHeaderPendingTabClassName =
        'ag-grid-header-pending-tab-fixed';
      const fixedAgGridUniversalToolbarClassName = 'pending-list-toolbar-fixed';
      const currentScrollHeight =
        window.pageYOffset ||
        document.documentElement.scrollTop ||
        document.body.scrollTop ||
        0;
      if (currentScrollHeight > headerHeight) {
        agGridHeader.classList.add(fixedAgGridHeaderClassName);

        const agGridToolbarActionBtns = document
          .querySelector('.action-toolbar')
          .getElementsByTagName('BUTTON');

        if (agGridToolbarActionBtns.length !== 0 && isOnUserAccountPage) {
          universalToolbarAction.classList.add(
            fixedAgGridUniversalToolbarClassName
          );

          agGridHeader.classList.add(fixedAgGridHeaderPendingTabClassName);
          agGridHeader.classList.remove(fixedAgGridHeaderClassName);
        } else {
          //   universalToolbarAction.classList.add(
          //     fixedAgGridUniversalToolbarClassName
          //   );
          universalToolbarAction.classList.remove(
            fixedAgGridUniversalToolbarClassName
          );

          agGridHeader.classList.remove(fixedAgGridHeaderPendingTabClassName);
          agGridHeader.classList.add(fixedAgGridHeaderClassName);
        }
      } else {
        agGridHeader.classList.remove(fixedAgGridHeaderClassName);
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
