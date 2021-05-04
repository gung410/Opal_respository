export const environment = {
  production: true,
  showAuthDebugInformation: true,
  fallbackLanguage: 'en-US',
  fallbackLanguageName: 'English',
  clientId: 'OrganizationSpa',
  issuer: 'https://idm.opal2.uat.cxs.cloud',
  apiGatewayOrigin: 'https://apigw.opal2.uat.cxs.cloud',
  apiGatewayResource: {
    organization: 'uat-competence-opal-api-organization',
    assessment: 'uat-competence-opal-api-assessment',
    cxId: 'uat-cxid-opal-idp',
    portal: 'uat-competence-opal-api-portal',
    report: 'uat-competence-opal-api-report',
    event: 'uat-competence-opal-api-event',
    communication: 'uat-datahub-opal-api-communication',
    dataHub: 'uat-datahub-opal-api-query'
  },
  applications: {
    pdPlanner: {
      mainUrl: 'https://system-admin.opal2.uat.cxs.cloud',
      logoUrl: ''
    }
  },
  gravatarUrl: 'https://secure.gravatar.com/avatar',
  buildVersion: '1.0.1',
  autoNavigateToIDP: true,
  UserIdleTimeOut: 1500, // seconds
  SessionTimeoutCountdown: 300, // seconds
  userIdleTimeoutLogout: true,
  OwnerId: 3001,
  CustomerId: 2052,
  ParentDepartmentId: 14350,
  OtherDepartmentId: 20747,
  topDepartmentId: 1,
  showTechnicalInfo: true,
  firebase: {
    fcmConfig: {
      apiKey: 'AIzaSyAQYaxjtyRu8_BEnG83lyKNbAJo9eNVRRw',
      authDomain: 'moe-opal-uat.firebaseapp.com',
      databaseURL: 'https://moe-opal-uat.firebaseio.com',
      projectId: 'moe-opal-uat',
      storageBucket: '',
      messagingSenderId: '718367215580',
      appId: '1:718367215580:web:0536314cc4263190'
    },
    publicVapidKey:
      'BDSteR2mKaYolfsqltZz662WMFTj4IODM_BngAWrYHbMYXQXxBWxJHYG_OErSf9wffPIbTOI7JrT5OnlszOPfr8'
  },
  notification: {
    enableToggleToFireBase: false,
    enableShowBellIcon: true,
    enableBroadCast: true,
    bellUrl:
      'https://www.uat.opal2.conexus.net/opal-account/notifications/bell',
    alertUrl:
      'https://www.uat.opal2.conexus.net/opal-account/notifications/alert'
  },
  moduleLink: {
    PDPM: 'https://www.uat.opal2.conexus.net/pdplanner/',
    SAM: 'https://admin.uat.opal2.conexus.net/',
    LearnerAppAndroid: 'https://www.uat.opal2.conexus.net/pdplanner/',
    LearnerAppIOS: 'https://www.uat.opal2.conexus.net/pdplanner/',
    LearnerWeb: 'https://www.uat.opal2.conexus.net/app/learner',
    Report: 'https://www.uat.opal2.conexus.net/report',
    CSL: 'https://www.uat.opal2.conexus.net/csl'
  },
  userAccounts: {
    enableCreateUserAccountRequest: true,
    enableMassCreateUserAccountRequest: true
  }
};
