export const environment = {
  production: true,
  fallbackLanguage: 'en-US',
  fallbackLanguageName: 'English',
  clientId: 'OrganizationSpa',
  issuer: '${CXID_IDP_URL}',
  apiGatewayOrigin: '${API_GATEWAY}',
  apiGatewayResource: {
    organization: '${ENVIRONMENT_NAME}-competence-opal-api-organization',
    assessment: '${ENVIRONMENT_NAME}-competence-opal-api-assessment',
    cxId: '${ENVIRONMENT_NAME}-cxid-opal-idp',
    portal: '${ENVIRONMENT_NAME}-competence-opal-api-portal',
    report: '${ENVIRONMENT_NAME}-competence-opal-api-report',
    event: '${ENVIRONMENT_NAME}-competence-opal-api-event',
    communication: '${ENVIRONMENT_NAME}-datahub-opal-api-communication',
    dataHub: '${ENVIRONMENT_NAME}-datahub-opal-api-query',
    learningCatalog: '${ENVIRONMENT_NAME}-competence-opal-api-learningcatalog'
  },
  site: {
    title: 'System Admin',
    logo: {
      imageUrl: 'assets/images/opal-logo-slogan.png',
      imageAlt: 'System Admin',
      routeLink: '',
      text: 'System Admin'
    },
    footer: {
      vulnerabilityUrl: 'https://tech.gov.sg/report_vulnerability',
      privacyStatementUrl: '${CXID_IDP_URL}/Home/PrivacyPolicy',
      termsOfUseUrl: '${CXID_IDP_URL}/Home/TermsOfUse'
    },
    dateRelease: '12 June 2020'
  },
  applications: {
    pdPlanner: {
      mainUrl: 'https://${ENVIRONMENT_NAME}-competence-opal-spa.csc.cxs.cloud',
      logoUrl: ''
    }
  },
  gravatarUrl: 'https://secure.gravatar.com/avatar',
  buildVersion: '${GO_PIPELINE_LABEL}',
  autoNavigateToIDP: true,
  UserIdleTimeOut: 1500, // seconds
  SessionTimeoutCountdown: 300, // seconds
  userIdleTimeoutLogout: true,
  OwnerId: 3001,
  CustomerId: 2052,
  ParentDepartmentId: 14350,
  OtherDepartmentId: 20747,
  topDepartmentId: 1,
  firebase: {
    fcmConfig: {
      apiKey: '${FIREBASE_API_KEY}',
      authDomain: 'moe-opal-${FIREBASE_AUTH_DOMAIN}.firebaseapp.com',
      databaseURL: 'https://moe-opal-${FIREBASE_AUTH_DOMAIN}.firebaseio.com',
      projectId: 'moe-opal-${FIREBASE_AUTH_DOMAIN}',
      storageBucket: '',
      messagingSenderId: '${ENVIRONMENT_FCM_MESSAGING_SENDER_ID}',
      appId: '${FIREBASE_APP_ID}'
    },
    publicVapidKey: '${FIREBASE_PUBLIC_VAPID_KEY}'
  },
  notification: {
    enableToggleToFireBase: false,
    enableShowBellIcon: true,
    enableBroadCast: true,
    bellUrl: 'https://www.opal2.moe.edu.sg/opal-account/notifications/bell',
    alertUrl: 'https://www.opal2.moe.edu.sg/opal-account/notifications/alert'
  },
  moduleLink: {
    PDPM: 'https://www.uat.opal2.conexus.net/pdplanner/',
    SAM: 'https://admin.uat.opal2.conexus.net/',
    LearnerAppAndroid: 'https://www.uat.opal2.conexus.net/pdplanner/',
    LearnerAppIOS: 'https://www.uat.opal2.conexus.net/pdplanner/',
    LearnerWeb: 'https://www.prod.opal2.conexus.net/app/learner',
    Report: 'https://www.prod.opal2.conexus.net/report'
  },
  userAccounts: {
    enableCreateUserAccountRequest: true,
    enableMassCreateUserAccountRequest: true
  }
};
