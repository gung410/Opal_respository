// tslint:disable:max-line-length
import { AppConstant } from 'app/shared/app.constant';

const reportBaseUrl = AppConstant.moduleLink.Report;
/**
 * This is the custom parameter using to set the background of the iframe to white.
 * TODO: Remove this if the background in SAM is synced with other module which #F9F9F9 is the background color.
 */
const customIframeBackgroundParameter = '&bgColor=white';

export const reportUrls = {
  /**
   * Relevant to https://www.development.opal2.conexus.net/report/dynamic/report?reportName=Officer%20PWC%20Meeting&schema=external&viewName=OfficerPWCMeeting&Select=Name,NumFinishedOnBoarding,NumSentEmailUnfinishedOnBoarding,TotalUser,PercentOfFinishedOnBoarding,PercentOfSentEmailAndUnfinishedOnBoarding&ShowTotal=true&colPatterns=4|{{PercentOfFinishedOnBoarding}}%&colPatterns=5|{{PercentOfSentEmailAndUnfinishedOnBoarding}}%&bgColor=white
   */
  OfficerPWCMeeting: `${reportBaseUrl}?q=BA+CglXWnXPtbHEA8O4GnfYet6sX9o/fR8SchDnvrHCiY3RC+gczhrs0iWq0hEUWEE0oC7WH3DafBytL/3kmaw==${customIframeBackgroundParameter}`,
  /**
   * Relevant to https://www.development.opal2.conexus.net/report/dynamic/report?reportName=User%20Account%20Details&schema=external&viewName=AllUserInOPAL2&bgColor=white&select=TypeOfOrganizationUnits,OrgName,FullName,JobDesignation,Email,SentWelcomeEmail,FinishOnBoarding,UserAccountStatus,DateOnboarded,DateOfLastLogin
   */
  AllUsers: `${reportBaseUrl}?q=BA+CglXWnXPtbHEA8O4Gndzz2kunaKdoqxi3i7DgyRWqULchxSpDBDRd3OVRK7AfeWz++n+CWu9ggHuEpQoiPw==${customIframeBackgroundParameter}`,
  /**
   * Relevant to https://www.development.opal2.conexus.net/report/report/dynamic/content/?reportName=UserAccountsStatuses
   */
  UserAccountStatus: `${reportBaseUrl}?q=BA+CglXWnXPtbHEA8O4Gndzz2kunaKdoqxi3i7DgyRUPE9axtXAc5HUg8Ml17pff2BBtPsgXg/A9Ph1x4OUmaQ==${customIframeBackgroundParameter}`
};
