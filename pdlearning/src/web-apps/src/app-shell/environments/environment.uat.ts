const appUrl: string = `${window.location.origin}/app`;

export const environment: IEnvironment = {
  envName: 'uat',
  production: true,
  manifest: 'assets/manifest.json',
  appUrl,
  apiUrl: '',

  contentApiUrl: 'https://apigw.uat.opal2.conexus.net/uat-learnapp-opal-api-content/api',
  courseApiUrl: 'https://apigw.uat.opal2.conexus.net/uat-learnapp-opal-api-course/api',
  formApiUrl: 'https://apigw.uat.opal2.conexus.net/uat-learnapp-opal-api-form/api',
  learnerApiUrl: 'https://apigw.uat.opal2.conexus.net/uat-learnapp-opal-api-learner/api',
  uploaderApiUrl: 'https://apigw.uat.opal2.conexus.net/uat-learnapp-opal-api-uploader/api',
  cloudfrontApiUrl: 'https://apigw.uat.opal2.conexus.net/uat-learnapp-opal-api-cloudfront/api',
  taggingApiUrl: 'https://apigw.uat.opal2.conexus.net/uat-learnapp-opal-api-tagging/api',
  brokenLinkApiUrl: 'https://apigw.uat.opal2.conexus.net/uat-learnapp-opal-api-brokenlink/api',
  calendarApiUrl: 'https://apigw.uat.opal2.conexus.net/uat-learnapp-opal-api-calendar/api',
  newsfeedApiUrl: 'https://apigw.uat.opal2.conexus.net/uat-learnapp-opal-api-newsfeed/api',
  webinarApiUrl: 'https://apigw.uat.opal2.conexus.net/uat-learnapp-opal-api-webinar/api',
  lnaFormApiUrl: 'https://apigw.uat.opal2.conexus.net/uat-learnapp-opal-api-lnaform/api',
  badgeApiUrl: 'https://apigw.uat.opal2.conexus.net/uat-learnapp-opal-api-badge/api',

  cslApiUrl: 'https://apigw.uat.opal2.conexus.net/uat-socialapp-opal-csl-api/api',
  cloudfrontUrl: 'https://d26mvlwqbzkmnu.cloudfront.net',
  scormPlayerUrl: `https://d26mvlwqbzkmnu.cloudfront.net/permanent/scorm-player/scorm-player.html`,
  userOnboardingUrl: `https://www.uat.opal2.conexus.net/opal-account/opal-account/onboarding?returnUrl=${appUrl}/learner`,
  reportVulnerabilityUrl: 'https://www.tech.gov.sg/report_vulnerability',
  privacyPolicyUrl: 'https://idm.uat.opal2.conexus.net/Home/PrivacyPolicy',
  termOfUse: 'https://idm.uat.opal2.conexus.net/Home/TermsOfUse',
  competenceApiUrl: 'https://apigw.uat.opal2.conexus.net/uat-competence-opal-api',
  learningCatalogUrl: 'https://apigw.uat.opal2.conexus.net/uat-competence-opal-api-learningcatalog',
  reportUrl: 'https://www.uat.opal2.conexus.net/report',
  AESSecretKey: '92AE31A79FEEB2A3',
  AESIv: 'H+MbQeThWmZq4t7w',
  ePortfolioUrl: 'https://www.uat.opal2.conexus.net/eportfolio',
  pdplanUrl: 'https://www.uat.opal2.conexus.net/pdplanner',
  cslUrl: 'https://www.uat.opal2.conexus.net/csl',
  notificationUrl: 'https://www.uat.opal2.conexus.net/opal-account/notifications/bell',
  broadcastMessageNotificationUrl: 'https://www.uat.opal2.conexus.net/opal-account/notifications/alert',
  enableBroadcastMessage: false,
  enableBellNotification: true,
  authConfig: {
    strictDiscoveryDocumentValidation: false,
    requireHttps: false,
    responseType: 'code',
    issuer: 'https://idm.uat.opal2.conexus.net',
    logoutUrl: 'https://idm.uat.opal2.conexus.net/connect/endsession',
    profileUrl: 'https://www.uat.opal2.conexus.net/opal-account/opal-account/profile',
    changePasswordUrl: 'https://idm.uat.opal2.conexus.net/Manage/ChangePassword',
    sitesUrl: 'https://apigw.uat.opal2.conexus.net/uat-competence-opal-api-portal/sites',
    organizationUrl: 'https://apigw.uat.opal2.conexus.net/uat-competence-opal-api-organization',
    permissionUrl: 'https://apigw.uat.opal2.conexus.net/uat-competence-opal-api-portal/me/accessRights',
    postLogoutRedirectUri: appUrl,
    redirectUri: `${appUrl}/index.html`,
    silentRefreshRedirectUri: `${appUrl}//silent-refresh.html`,
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
  logoPath: 'assets/images/logos/opal_uat.png'
};