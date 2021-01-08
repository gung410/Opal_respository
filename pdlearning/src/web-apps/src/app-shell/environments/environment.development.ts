const appUrl: string = `${window.location.origin}/app`;

export const environment: IEnvironment = {
  envName: 'development',
  production: true,
  manifest: 'assets/manifest.json',
  appUrl,
  apiUrl: '',

  contentApiUrl: 'https://api.development.opal2.conexus.net/development-learnapp-opal-api-content/api',
  courseApiUrl: 'https://api.development.opal2.conexus.net/development-learnapp-opal-api-course/api',
  formApiUrl: 'https://api.development.opal2.conexus.net/development-learnapp-opal-api-form/api',
  learnerApiUrl: 'https://api.development.opal2.conexus.net/development-learnapp-opal-api-learner/api',
  uploaderApiUrl: 'https://api.development.opal2.conexus.net/development-learnapp-opal-api-uploader/api',
  taggingApiUrl: 'https://api.development.opal2.conexus.net/development-learnapp-opal-api-tagging/api',
  brokenLinkApiUrl: 'https://api.development.opal2.conexus.net/development-learnapp-opal-api-brokenlink/api',
  calendarApiUrl: 'https://api.development.opal2.conexus.net/development-learnapp-opal-api-calendar/api',
  newsfeedApiUrl: 'https://api.development.opal2.conexus.net/development-learnapp-opal-api-newsfeed/api',
  webinarApiUrl: 'https://api.development.opal2.conexus.net/development-learnapp-opal-api-webinar/api',
  lnaFormApiUrl: 'https://api.development.opal2.conexus.net/development-learnapp-opal-api-standalonesurvey/api',
  badgeApiUrl: 'https://api.development.opal2.conexus.net/development-learnapp-opal-api-badge/api',

  cslApiUrl: 'https://api.development.opal2.conexus.net/development-socialapp-opal-csl-api/api',
  cloudfrontApiUrl: 'https://api.development.opal2.conexus.net/development-learnapp-opal-api-cloudfront/api',
  cloudfrontUrl: 'https://dexzs7wx4278r.cloudfront.net',
  scormPlayerUrl: `https://dexzs7wx4278r.cloudfront.net/permanent/scorm-player/scorm-player.html`,
  userOnboardingUrl: `https://www.development.opal2.conexus.net/opal-account/opal-account/onboarding?returnUrl=${appUrl}/learner`,
  reportVulnerabilityUrl: 'https://www.tech.gov.sg/report_vulnerability',
  privacyPolicyUrl: 'https://idm.development.opal2.conexus.net/Home/PrivacyPolicy',
  termOfUse: 'https://idm.development.opal2.conexus.net/Home/TermsOfUse',
  competenceApiUrl: 'https://api.development.opal2.conexus.net/development-competence-opal-api',
  learningCatalogUrl: 'https://api.development.opal2.conexus.net/development-competence-opal-api-learningcatalog',
  reportUrl: 'https://www.development.opal2.conexus.net/report',
  AESSecretKey: '92AE31A79FEEB2A3',
  AESIv: 'H+MbQeThWmZq4t7w',
  ePortfolioUrl: 'https://www.development.opal2.conexus.net/eportfolio',
  pdplanUrl: 'https://www.development.opal2.conexus.net/pdplanner',
  cslUrl: 'https://www.development.opal2.conexus.net/csl',
  notificationUrl: 'https://www.development.opal2.conexus.net/opal-account/notifications/bell',
  broadcastMessageNotificationUrl: 'https://www.development.opal2.conexus.net/opal-account/notifications/alert',
  enableBroadcastMessage: true,
  enableBellNotification: true,
  authConfig: {
    strictDiscoveryDocumentValidation: false,
    requireHttps: false,
    responseType: 'code',
    issuer: 'https://idm.development.opal2.conexus.net',
    logoutUrl: 'https://idm.development.opal2.conexus.net/connect/endsession',
    profileUrl: 'https://www.development.opal2.conexus.net/opal-account/opal-account/profile',
    changePasswordUrl: 'https://idm.development.opal2.conexus.net/Manage/ChangePassword',
    sitesUrl: 'https://api.development.opal2.conexus.net/development-competence-opal-api-portal/sites',
    organizationUrl: 'https://api.development.opal2.conexus.net/development-competence-opal-api-organization',
    permissionUrl: 'https://api.development.opal2.conexus.net/development-competence-opal-api-portal/me/accessRights',
    postLogoutRedirectUri: appUrl,
    redirectUri: `${appUrl}/index.html`,
    silentRefreshRedirectUri: `${appUrl}/silent-refresh.html`,
    clientId: 'Opal2WebApp',
    scope: 'roles profile cxprofile openid cxDomainInternalApi',
    disablePKCE: false,
    showDebugInformation: true,
    sessionChecksEnabled: true,
    sessionCheckIntervall: 2000,
    skipIssuerCheck: true,
    ignoredPaths: [
      '/app/quiz-player',
      '/app/scorm-player',
      '/app/digital-content-player',
      '/app/video-annotation-player',
      '/app/community-metadata',
      '/app/assignment-player',
      '/app/calendar',
      '/app/form-standalone-player',
      '/app/assessment-player',
      '/app/standalone-survey'
    ]
  },
  idleConfig: {
    idleTimeoutInSecond: 1500,
    inActiveTimeoutInSecond: 300,
    userEvents: 'DOMMouseScroll keyup mousewheel mousedown touchstart touchmove scroll'
  },
  lastUpdateString: '12 June 2020',
  logoPath: 'assets/images/logos/opal.png'
};
