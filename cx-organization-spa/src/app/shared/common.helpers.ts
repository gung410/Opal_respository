import { Injectable } from '@angular/core';

import { UserAccountsDataService } from 'app/user-accounts/user-accounts-data.service';

import { UserAccountTabEnum } from 'app/user-accounts/user-accounts.helper';

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

      const userAccountCurrentTab = UserAccountsDataService.getCurrentTabLabel();

      const isOnUserAccountPendingTab =
        userAccountCurrentTab === UserAccountTabEnum.Pending1st ||
        userAccountCurrentTab === UserAccountTabEnum.Pending2nd ||
        userAccountCurrentTab === UserAccountTabEnum.Pending3rd;

      const isOnUserAccountOtherPlaceOfWorkTab =
        userAccountCurrentTab === UserAccountTabEnum.UserOtherPlace;

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

      const fixedAgGridUniversalToolbarPendingClassName =
        'pending-list-toolbar-fixed';

      const fixedAgGridHeaderOtherPlaceOfWorkTabClassName =
        'ag-grid-header-other-place-of-work-tab-fixed';

      const fixedAgGridUniversalToolbarOtherPlaceOfWorkClassName =
        'other-place-of-work-list-toolbar-fixed';

      const currentScrollHeight =
        window.pageYOffset ||
        document.documentElement.scrollTop ||
        document.body.scrollTop ||
        0;

      if (currentScrollHeight > headerHeight) {
        if (isOnUserAccountPage && isOnUserAccountPendingTab) {
          universalToolbarAction.classList.add(
            fixedAgGridUniversalToolbarPendingClassName
          );
          universalToolbarAction.classList.remove(
            fixedAgGridUniversalToolbarOtherPlaceOfWorkClassName
          );

          agGridHeader.classList.add(fixedAgGridHeaderPendingTabClassName);
          agGridHeader.classList.remove(fixedAgGridHeaderClassName);
          agGridHeader.classList.remove(
            fixedAgGridHeaderOtherPlaceOfWorkTabClassName
          );
        } else if (isOnUserAccountPage && isOnUserAccountOtherPlaceOfWorkTab) {
          universalToolbarAction.classList.add(
            fixedAgGridUniversalToolbarOtherPlaceOfWorkClassName
          );
          universalToolbarAction.classList.remove(
            fixedAgGridUniversalToolbarPendingClassName
          );

          agGridHeader.classList.add(
            fixedAgGridHeaderOtherPlaceOfWorkTabClassName
          );
          agGridHeader.classList.remove(fixedAgGridHeaderClassName);
          agGridHeader.classList.remove(fixedAgGridHeaderPendingTabClassName);
        } else {
          universalToolbarAction.classList.remove(
            fixedAgGridUniversalToolbarPendingClassName
          );

          universalToolbarAction.classList.remove(
            fixedAgGridUniversalToolbarOtherPlaceOfWorkClassName
          );

          agGridHeader.classList.remove(fixedAgGridHeaderPendingTabClassName);
          agGridHeader.classList.add(fixedAgGridHeaderClassName);
        }
      } else {
        universalToolbarAction.classList.remove(
          fixedAgGridUniversalToolbarPendingClassName
        );

        universalToolbarAction.classList.remove(
          fixedAgGridUniversalToolbarOtherPlaceOfWorkClassName
        );

        agGridHeader.classList.remove(fixedAgGridHeaderPendingTabClassName);
        agGridHeader.classList.remove(
          fixedAgGridHeaderOtherPlaceOfWorkTabClassName
        );
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
