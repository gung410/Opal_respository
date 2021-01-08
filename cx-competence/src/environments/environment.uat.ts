export const environment = {
  production: true,
  fallbackLanguage: 'en-US',
  fallbackLanguageName: 'English',
  clientId: 'CompetenceSpa',
  issuer: 'https://idm.opal2.uat.cxs.cloud',
  apiGatewayOrigin: 'https://apigw.opal2.uat.cxs.cloud',
  systemAdminUrl: 'https://system-admin.opal2.uat.cxs.cloud',
  apiGatewayResource: {
    organization: 'uat-competence-opal-api-organization',
    assessment: 'uat-competence-opal-api-assessment',
    cxId: 'uat-cxid-opal-idp',
    portal: 'uat-competence-opal-api-portal',
    report: 'uat-competence-opal-api-report',
    competence: 'uat-competence-opal-api',
    communication: 'uat-datahub-opal-api-communication',
  },
  site: {
    title: 'PD Planner',
    logo: {
      imageUrl: 'assets/images/opal.png',
      imageAlt: 'PD Planner',
      routeLink: '',
      text: 'PD Planner',
    },
    footer: {
      vulnerabilityUrl: 'https://tech.gov.sg/report_vulnerability',
      privacyStatementUrl:
        'https://idm.uat.opal2.conexus.net/Home/PrivacyPolicy',
      termsOfUseUrl: 'https://idm.uat.opal2.conexus.net/Home/TermsOfUse',
    },
    dateRelease: '12 June 2020',
  },
  ItemPerPage: 50,
  OwnerId: 3001,
  CustomerId: 2052,
  ParentDepartmentId: 14350,
  buildVersion: '1.1.1',
  autoNavigateToIDP: true,
  pdplanConfig: {
    permissionConfig: {
      createLearningDirection: {
        roleExtIds: [
          'schooltrainingcoordinator',
          'divisiontrainingcoordinator',
          'overallsystemadministrator',
          'schooladmin',
          'divisionadmin',
          'approvingofficer',
        ],
      },
      createKeyLearningProgram: {
        roleExtIds: [
          'schooltrainingcoordinator',
          'divisiontrainingcoordinator',
          'overallsystemadministrator',
          'schooladmin',
          'divisionadmin',
          'approvingofficer',
        ],
      },
      startNewLearningPlan: {
        roleExtIds: [
          'schooltrainingcoordinator',
          'divisiontrainingcoordinator',
          'overallsystemadministrator',
          'schooladmin',
          'divisionadmin',
          'approvingofficer',
        ],
      },
      deleteDraftVersion: {
        roleExtIds: [
          'schooltrainingcoordinator',
          'divisiontrainingcoordinator',
          'overallsystemadministrator',
          'schooladmin',
          'divisionadmin',
          'approvingofficer',
        ],
      },
      edit: {
        roleExtIds: [
          'schooltrainingcoordinator',
          'divisiontrainingcoordinator',
          'overallsystemadministrator',
          'schooladmin',
          'divisionadmin',
          'approvingofficer',
        ],
      },
      submit: {
        roleExtIds: [
          'schooltrainingcoordinator',
          'divisiontrainingcoordinator',
          'overallsystemadministrator',
          'schooladmin',
          'divisionadmin',
          'approvingofficer',
        ],
      },
      approveOrReject: {
        roleExtIds: [
          'approvingofficer',
          'overallsystemadministrator',
          'schooladmin',
          'divisionadmin',
        ],
      },
      commentOnOPJ: [
        {
          opjStatus: ['NotStarted', 'Started'],
          roleExtIds: ['*'],
        },
        {
          opjStatus: [
            'PendingForApproval',
            'Approved',
            'Rejected',
            'Completed',
          ],
          roleExtIds: [
            'schooltrainingcoordinator',
            'divisiontrainingcoordinator',
            'overallsystemadministrator',
            'schooladmin',
            'divisionadmin',
            'approvingofficer',
          ],
        },
      ],
      departmentHierarchyBrowser: {
        includeChildren: {
          roleExtIds: [
            'overallsystemadministrator',
            'divisionadmin',
            'branchadmin',
            'schooladmin',
            'divisiontrainingcoordinator',
            'schooltrainingcoordinator',
            'approvingofficer',
          ],
        },
      },
      approvePendingRequest: {
        ODP: {
          learningPlan: [
            'overallsystemadministrator',
            'divisionadmin',
            'branchadmin',
            'schooladmin',
            'approvingofficer',
            'divisionallearningcoordinator',
            'schooltrainingcoordinator',
          ],
          learningDirection: [
            'overallsystemadministrator',
            'divisionadmin',
            'branchadmin',
            'schooladmin',
            'approvingofficer',
            'divisionallearningcoordinator',
            'schooltrainingcoordinator',
          ],
          klpNomination: [
            'overallsystemadministrator',
            'divisionadmin',
            'branchadmin',
            'schooladmin',
            'reportingofficer',
          ],
        },
        IDP: {
          classRegistration: [
            'overallsystemadministrator',
            'divisionadmin',
            'branchadmin',
            'schooladmin',
            'reportingofficer',
          ],
          classWithdrawal: [
            'overallsystemadministrator',
            'divisionadmin',
            'branchadmin',
            'schooladmin',
            'reportingofficer',
          ],
          classChangeRequest: [
            'overallsystemadministrator',
            'divisionadmin',
            'branchadmin',
            'schooladmin',
            'reportingofficer',
          ],
          adhocNominations: [
            'overallsystemadministrator',
            'divisionadmin',
            'branchadmin',
            'schooladmin',
            'reportingofficer',
          ],
        },
      },
    },
    organizationalUnitTypesHavingOPJ: ['school', 'division', 'ministry'],
  },
  lnaResult: {
    roleCanDecideApprovalExtIds: [
      'overallsystemadministrator',
      'schooladmin',
      'branchadmin',
      'divisionadmin',
    ],
    allowToEditLNAApprovedXTimes: 1,
  },
  userIdleTimeOut: 1500, // seconds
  sessionTimeoutCountdown: 300, // seconds
  userIdleTimeoutLogout: true,
  gravatarUrl: 'https://secure.gravatar.com/avatar',
  uriLearningOpportunityFieldName: 'uriLearningOpportunity',
  myPdJourneyConfig: {
    pdPlanTabName: 'PdPlanTab',
  },
  firebase: {
    fcmConfig: {
      apiKey: 'AIzaSyAQYaxjtyRu8_BEnG83lyKNbAJo9eNVRRw',
      authDomain: 'moe-opal-uat.firebaseapp.com',
      databaseURL: 'https://moe-opal-uat.firebaseio.com',
      projectId: 'moe-opal-uat',
      storageBucket: '',
      messagingSenderId: '718367215580',
      appId: '1:718367215580:web:0536314cc4263190',
    },
    publicVapidKey:
      'BDSteR2mKaYolfsqltZz662WMFTj4IODM_BngAWrYHbMYXQXxBWxJHYG_OErSf9wffPIbTOI7JrT5OnlszOPfr8',
  },
  VirtualPath: 'pdplanner',
  notification: {
    enableToggleToFireBase: false,
    enableShowBellIcon: true,
    enableBroadCast: true,
    bellUrl:
      'https://www.uat.opal2.conexus.net/opal-account/notifications/bell',
    alertUrl:
      'https://www.uat.opal2.conexus.net/opal-account/notifications/alert',
  },
  moduleLink: {
    report: 'https://www.uat.opal2.conexus.net/report',
  },
};
