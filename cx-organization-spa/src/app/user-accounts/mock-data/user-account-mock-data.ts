import {
  PagingResponseModel,
  UserManagement
} from '../models/user-management.model';
import { EntityStatus } from 'app-models/entity-status.model';

export const UserAccountMockData = {
  mockDepartmentData: [
    {
      parentDepartmentId: 1,
      identity: {
        extId: 'HRMS001',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'DataOwner',
        id: 14350
      },
      departmentName: 'MOE',
      departmentDescription: ''
    },
    {
      parentDepartmentId: 14350,
      identity: {
        extId: 'HRMS002',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'OrganizationalUnit',
        id: 14351
      },
      departmentName: 'Professional Wing',
      departmentDescription: ''
    }
  ],
  mockEmployeesData: {
    pageIndex: 1,
    pageSize: 50,
    items: [
      {
        departmentId: 14350,
        firstName: 'Geir Fuhre Pettersen',
        mobileCountryCode: 65,
        emailAddress: 'gfp@conexus.no',
        gender: 0,
        tag: '',
        created: '2019-03-25T11:01:00Z',
        loginServiceClaims: [],
        forceLoginAgain: false,
        roles: [],
        identity: {
          extId: 'MOE000001',
          ownerId: 3001,
          customerId: 2052,
          archetype: 'Employee',
          id: 6
        },
        jsonDynamicAttributes: [],
        entityStatus: {
          externallyMastered: true,
          lastExternallySynchronized: '2019-03-25T19:01:26.2833333Z',
          entityVersion: 'AAAAAAAACGU=',
          lastUpdated: '2019-03-25T19:01:26.2833333Z',
          lastUpdatedBy: 3,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        departmentId: 14350,
        firstName: 'Jackie Nguyen',
        mobileCountryCode: 65,
        emailAddress: 'tu.nguyen@orientsoftware.com',
        gender: 0,
        tag: '',
        created: '2019-03-25T11:01:00Z',
        loginServiceClaims: [],
        forceLoginAgain: false,
        roles: [],
        identity: {
          extId: 'MOE000003',
          ownerId: 3001,
          customerId: 2052,
          archetype: 'Employee',
          id: 8
        },
        jsonDynamicAttributes: [],
        entityStatus: {
          externallyMastered: true,
          lastExternallySynchronized: '2019-03-25T19:01:26.3166667Z',
          entityVersion: 'AAAAAAAACHg=',
          lastUpdated: '2019-03-26T14:33:58.5Z',
          lastUpdatedBy: 3,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      }
    ],
    totalItems: 170
  },
  mockEmployeeData: {
    departmentId: 14350,
    firstName: 'Geir Fuhre Pettersen',
    mobileCountryCode: 65,
    emailAddress: 'gfp@conexus.no',
    sSN: 'S0590251G',
    gender: 0,
    tag: '',
    created: '2019-03-25T11:01:00Z',
    loginServiceClaims: [],
    forceLoginAgain: false,
    roles: [
      {
        localizedData: [
          {
            id: 1,
            languageCode: 'nb-NO',
            fields: [
              {
                name: 'Name',
                localizedText: 'Superadministrator'
              },
              {
                name: 'Description',
                localizedText: 'vip24 Super administrator'
              }
            ]
          },
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Super Administrator'
              },
              {
                name: 'Description',
                localizedText: 'vip24 Super administrator'
              }
            ]
          },
          {
            id: 7,
            languageCode: 'sv-SE',
            fields: [
              {
                name: 'Name',
                localizedText: 'Super administrator'
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          },
          {
            id: 10,
            languageCode: 'da-DK',
            fields: [
              {
                name: 'Name',
                localizedText: 'Super Administrator'
              },
              {
                name: 'Description',
                localizedText: 'vip24 Super administrator'
              }
            ]
          }
        ],
        identity: {
          extId: 'superadministrator',
          ownerId: 3001,
          customerId: 0,
          archetype: 'Role',
          id: 1
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00Z',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      }
    ],
    identity: {
      extId: 'S0590251G',
      ownerId: 3001,
      customerId: 2052,
      archetype: 'Employee',
      id: 6
    },
    jsonDynamicAttributes: [],
    entityStatus: {
      externallyMastered: false,
      lastExternallySynchronized: '2019-03-25T19:01:26.2833333Z',
      entityVersion: 'AAAAAAAACPU=',
      lastUpdated: '2019-04-16T11:21:28.87Z',
      lastUpdatedBy: 3,
      statusId: 'Active',
      statusReasonId: 'Unknown',
      deleted: false
    }
  },
  mockSystemRolesData: [
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'Division Administrator'
            },
            {
              name: 'Description',
              localizedText: ''
            }
          ]
        }
      ],
      identity: {
        extId: 'divisionadmin',
        ownerId: 3001,
        customerId: 0,
        archetype: 'SystemRole',
        id: 87
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'Branch Administrator'
            },
            {
              name: 'Description',
              localizedText: ''
            }
          ]
        }
      ],
      identity: {
        extId: 'branchadmin',
        ownerId: 3001,
        customerId: 0,
        archetype: 'SystemRole',
        id: 88
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'School Administrator'
            },
            {
              name: 'Description',
              localizedText: ''
            }
          ]
        }
      ],
      identity: {
        extId: 'schooladmin',
        ownerId: 3001,
        customerId: 0,
        archetype: 'SystemRole',
        id: 89
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'Division Training Coordinator'
            },
            {
              name: 'Description',
              localizedText:
                'Division Training Coordinator/\nDivisional Learning Coordinator/School Staff Developer'
            }
          ]
        }
      ],
      identity: {
        extId: 'divisiontrainingcoordinator',
        ownerId: 3001,
        customerId: 0,
        archetype: 'SystemRole',
        id: 90
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'Course Content Creator'
            },
            {
              name: 'Description',
              localizedText: ''
            }
          ]
        }
      ],
      identity: {
        extId: 'coursecontentcreator',
        ownerId: 3001,
        customerId: 0,
        archetype: 'SystemRole',
        id: 91
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'Course Administrator'
            },
            {
              name: 'Description',
              localizedText: ''
            }
          ]
        }
      ],
      identity: {
        extId: 'courseadmin',
        ownerId: 3001,
        customerId: 0,
        archetype: 'SystemRole',
        id: 92
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'Course Facilitator'
            },
            {
              name: 'Description',
              localizedText: ''
            }
          ]
        }
      ],
      identity: {
        extId: 'coursefacilitator',
        ownerId: 3001,
        customerId: 0,
        archetype: 'SystemRole',
        id: 93
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'WTTG Representative'
            },
            {
              name: 'Description',
              localizedText: ''
            }
          ]
        }
      ],
      identity: {
        extId: 'wttgrepresentative',
        ownerId: 3001,
        customerId: 0,
        archetype: 'SystemRole',
        id: 94
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'Digital Content Approving Officer'
            },
            {
              name: 'Description',
              localizedText: ''
            }
          ]
        }
      ],
      identity: {
        extId: 'digitalcontentreportingofficer',
        ownerId: 3001,
        customerId: 0,
        archetype: 'SystemRole',
        id: 95
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'Content Creator'
            },
            {
              name: 'Description',
              localizedText: ''
            }
          ]
        }
      ],
      identity: {
        extId: 'contentcreator',
        ownerId: 3001,
        customerId: 0,
        archetype: 'SystemRole',
        id: 96
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'Approving Officer'
            },
            {
              name: 'Description',
              localizedText: ''
            }
          ]
        }
      ],
      identity: {
        extId: 'reportingofficer',
        ownerId: 3001,
        customerId: 0,
        archetype: 'SystemRole',
        id: 97
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'Reporting Officer '
            },
            {
              name: 'Description',
              localizedText: ''
            }
          ]
        }
      ],
      identity: {
        extId: 'reportingofficer',
        ownerId: 3001,
        customerId: 0,
        archetype: 'SystemRole',
        id: 98
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'Learner'
            },
            {
              name: 'Description',
              localizedText: ''
            }
          ]
        }
      ],
      identity: {
        extId: 'learner',
        ownerId: 3001,
        customerId: 0,
        archetype: 'SystemRole',
        id: 99
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'Guest '
            },
            {
              name: 'Description',
              localizedText: ''
            }
          ]
        }
      ],
      identity: {
        extId: 'guest',
        ownerId: 3001,
        customerId: 0,
        archetype: 'SystemRole',
        id: 100
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    }
  ],
  mockPersonnelGroups: [
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'Educating Officer'
            },
            {
              name: 'Description',
              localizedText: 'Educating Officer'
            }
          ]
        }
      ],
      identity: {
        extId: 'Scheme-EO',
        ownerId: 3001,
        customerId: 0,
        archetype: 'PersonnelGroup',
        id: 101
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'MOE Kindergarten Educator'
            },
            {
              name: 'Description',
              localizedText: 'MOE Kindergarten Educator'
            }
          ]
        }
      ],
      identity: {
        extId: 'Scheme-MKE',
        ownerId: 3001,
        customerId: 0,
        archetype: 'PersonnelGroup',
        id: 102
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'Allied Educator'
            },
            {
              name: 'Description',
              localizedText: 'Service Scheme: Allied Educator'
            }
          ]
        }
      ],
      identity: {
        extId: 'Scheme-AED',
        ownerId: 3001,
        customerId: 0,
        archetype: 'PersonnelGroup',
        id: 103
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'Executive and Administrative Staff​'
            },
            {
              name: 'Description',
              localizedText:
                'Service Scheme: Executive and Administrative Staff​'
            }
          ]
        }
      ],
      identity: {
        extId: 'Scheme-EAS',
        ownerId: 3001,
        customerId: 0,
        archetype: 'PersonnelGroup',
        id: 104
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    }
  ],
  mockCarreerPaths: [
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'Teacher Growth Model'
            },
            {
              name: 'Description',
              localizedText: 'Growth Model: Teacher Growth Model'
            }
          ]
        }
      ],
      identity: {
        extId: 'GM-TGM',
        ownerId: 3001,
        customerId: 0,
        archetype: 'CareerPath',
        id: 105
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'Leadership Growth Model'
            },
            {
              name: 'Description',
              localizedText: 'Growth Model: Leadership Growth Model'
            }
          ]
        }
      ],
      identity: {
        extId: 'GM-LGM',
        ownerId: 3001,
        customerId: 0,
        archetype: 'CareerPath',
        id: 106
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      localizedData: [
        {
          id: 2,
          languageCode: 'en-US',
          fields: [
            {
              name: 'Name',
              localizedText: 'Specialist Growth Model'
            },
            {
              name: 'Description',
              localizedText: 'Growth Model: Specialist Growth Model'
            }
          ]
        }
      ],
      identity: {
        extId: 'GM-SGM',
        ownerId: 3001,
        customerId: 0,
        archetype: 'CareerPath',
        id: 107
      },
      entityStatus: {
        externallyMastered: false,
        lastUpdated: '0001-01-01T00:00:00',
        lastUpdatedBy: 0,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    }
  ],

  editUserMockData: {
    departmentName: 'MOE',
    groups: [],
    departmentId: 14350,
    firstName: 'Kai',
    mobileCountryCode: '65',
    mobileNumber: '1234567891',
    emailAddress: 'kiet.truong@orientsoftware.com',
    ssn: 'S1234557G',
    gender: 0,
    dateOfBirth: '2019-04-27T00:00:00',
    tag: '',
    created: '2019-03-28T02:47:00',
    forceLoginAgain: false,
    systemRoles: [
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Super Administrator'
              },
              {
                name: 'Description',
                localizedText: 'vip24 Super administrator'
              }
            ]
          }
        ],
        identity: {
          extId: 'superadministrator',
          ownerId: 3001,
          customerId: 0,
          archetype: 'Role',
          id: 1
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Candidate'
              },
              {
                name: 'Description',
                localizedText: 'vip24 Candidate'
              }
            ]
          }
        ],
        identity: {
          extId: 'candidate',
          ownerId: 3001,
          customerId: 0,
          archetype: 'Role',
          id: 39
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Leader'
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'leader',
          ownerId: 3001,
          customerId: 0,
          archetype: 'Role',
          id: 42
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Employee'
              },
              {
                name: 'Description',
                localizedText: '+Competence users only'
              }
            ]
          }
        ],
        identity: {
          extId: 'employee',
          ownerId: 3001,
          customerId: 0,
          archetype: 'Role',
          id: 49
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Dnb Sponsored Coaching'
              },
              {
                name: 'Description',
                localizedText: 'Dnb Sponsored Coaching'
              }
            ]
          }
        ],
        identity: {
          extId: 'dnbsponsoredcoaching',
          ownerId: 3001,
          customerId: 0,
          archetype: 'Role',
          id: 85
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Digital Coach'
              },
              {
                name: 'Description',
                localizedText: 'Digital Coach'
              }
            ]
          }
        ],
        identity: {
          extId: 'digitalcoach',
          ownerId: 3001,
          customerId: 0,
          archetype: 'Role',
          id: 86
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Division Administrator'
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'divisionadmin',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 87
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'School Administrator'
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'schooladmin',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 89
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Course Content Creator'
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'coursecontentcreator',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 91
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Course Administrator'
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'courseadmin',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 92
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Digital Content Approving Officer'
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'digitalcontentreportingofficer',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 95
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Content Creator'
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'contentcreator',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 96
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Reporting Officer '
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'reportingofficer',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 98
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Guest '
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'guest',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 100
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Educating Officer'
              },
              {
                name: 'Description',
                localizedText: 'Educating Officer'
              }
            ]
          }
        ],
        identity: {
          extId: 'Scheme-EO',
          ownerId: 3001,
          customerId: 0,
          archetype: 'PersonnelGroup',
          id: 101
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Specialist Growth Model'
              },
              {
                name: 'Description',
                localizedText: 'Growth Model: Specialist Growth Model'
              }
            ]
          }
        ],
        identity: {
          extId: 'GM-SGM',
          ownerId: 3001,
          customerId: 0,
          archetype: 'CareerPath',
          id: 107
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      }
    ],
    identity: {
      extId: 'e3fac4f2-bff2-42cb-8986-3c900ca67b28',
      ownerId: 3001,
      customerId: 2052,
      archetype: 'Employee',
      id: 18
    },
    jsonDynamicAttributes: [],
    entityStatus: {
      externallyMastered: false,
      lastExternallySynchronized: '2019-03-28T10:46:58.32',
      entityVersion: 'AAAAAAAACp4=',
      lastUpdated: '2019-04-22T09:22:18.3779626',
      lastUpdatedBy: 3,
      statusId: 'Active',
      statusReasonId: 'Unknown',
      deleted: false
    }
  },
  userAccountsMockData: [
    {
      departmentName: 'MOE',
      groups: [],
      currentDepartmentId: 14350,
      departmentId: 14350,
      firstName: 'Dupree - Phat Nguyen Vinh',
      mobileCountryCode: 65,
      mobileNumber: '1234567891',
      emailAddress: 'phat.nguyen@orientsoftware.com',
      ssn: 'S1234557G',
      gender: 0,
      tag: '',
      created: '2019-03-25T11:01:00',
      forceLoginAgain: false,
      roles: [
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Division Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'divisionadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 87
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Branch Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'branchadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 88
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Content Creator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'coursecontentcreator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 91
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Content Creator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'contentcreator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 96
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Executive and Administrative Staff​'
                },
                {
                  name: 'Description',
                  localizedText:
                    'Service Scheme: Executive and Administrative Staff​'
                }
              ]
            }
          ],
          identity: {
            extId: 'Scheme-EAS',
            ownerId: 3001,
            customerId: 0,
            archetype: 'PersonnelGroup',
            id: 104
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Leadership Growth Model'
                },
                {
                  name: 'Description',
                  localizedText: 'Growth Model: Leadership Growth Model'
                }
              ]
            }
          ],
          identity: {
            extId: 'GM-LGM',
            ownerId: 3001,
            customerId: 0,
            archetype: 'CareerPath',
            id: 106
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        }
      ],
      identity: {
        extId: '76f2b76d-f34f-4364-8627-935a79359fa9',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 11
      },
      jsonDynamicAttributes: [],
      entityStatus: {
        externallyMastered: false,
        lastExternallySynchronized: '2019-03-25T19:01:26.35',
        entityVersion: 'AAAAAAAACtQ=',
        lastUpdated: '2019-04-22T18:46:29.3966667',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      departmentName: 'MOE',
      groups: [],
      currentDepartmentId: 14350,
      jsonjsonDynamicAttributes: {
        isbn: '123-456-222',
        author: {
          lastname: 'test-test',
          firstname: 'Jane'
        },
        editor: {
          lastname: 'Smith',
          firstname: 'Jane'
        },
        title: 'The Ultimate Database Study Guide',
        category: ['Non-Fiction', 'Technology']
      },
      departmentId: 14350,
      firstName: 'Geir Fuhre Pettersen',
      mobileCountryCode: 65,
      mobileNumber: '123',
      emailAddress: 'gfp@conexus.no',
      ssn: 'S1234567G',
      gender: 0,
      dateOfBirth: '1955-06-10T00:00:00',
      tag: '',
      created: '2019-03-25T11:01:00',
      forceLoginAgain: false,
      roles: [
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Division Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'divisionadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 87
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Branch Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'branchadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 88
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'School Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'schooladmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 89
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'courseadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 92
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Educating Officer'
                },
                {
                  name: 'Description',
                  localizedText: 'Educating Officer'
                }
              ]
            }
          ],
          identity: {
            extId: 'Scheme-EO',
            ownerId: 3001,
            customerId: 0,
            archetype: 'PersonnelGroup',
            id: 101
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Leadership Growth Model'
                },
                {
                  name: 'Description',
                  localizedText: 'Growth Model: Leadership Growth Model'
                }
              ]
            }
          ],
          identity: {
            extId: 'GM-LGM',
            ownerId: 3001,
            customerId: 0,
            archetype: 'CareerPath',
            id: 106
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        }
      ],
      identity: {
        extId: 'S0590251G',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 6
      },
      jsonDynamicAttributes: [],
      entityStatus: {
        externallyMastered: false,
        lastExternallySynchronized: '2019-03-25T19:01:26.2833333',
        entityVersion: 'AAAAAAAACs4=',
        lastUpdated: '2019-04-22T10:32:59.8452001',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      departmentName: 'MOE',
      groups: [],
      currentDepartmentId: 14350,
      departmentId: 14350,
      firstName: 'Glenn Tran',
      mobileCountryCode: 47,
      emailAddress: 'wohu@mail-list.top',
      gender: 0,
      dateOfBirth: '2019-04-22T00:00:00',
      tag: '',
      created: '2019-04-22T03:55:00',
      forceLoginAgain: false,
      roles: [
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Employee'
                },
                {
                  name: 'Description',
                  localizedText: '+Competence users only'
                }
              ]
            }
          ],
          identity: {
            extId: 'employee',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 49
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Learner'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'learner',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 99
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Educating Officer'
                },
                {
                  name: 'Description',
                  localizedText: 'Educating Officer'
                }
              ]
            }
          ],
          identity: {
            extId: 'Scheme-EO',
            ownerId: 3001,
            customerId: 0,
            archetype: 'PersonnelGroup',
            id: 101
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Specialist Growth Model'
                },
                {
                  name: 'Description',
                  localizedText: 'Growth Model: Specialist Growth Model'
                }
              ]
            }
          ],
          identity: {
            extId: 'GM-SGM',
            ownerId: 3001,
            customerId: 0,
            archetype: 'CareerPath',
            id: 107
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        }
      ],
      identity: {
        extId: '86f5e4df-50a2-4021-9a81-9cde1d101e0b',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 23
      },
      jsonDynamicAttributes: [],
      entityStatus: {
        externallyMastered: false,
        lastExternallySynchronized: '2019-04-22T11:54:47.5533333',
        entityVersion: 'AAAAAAAACuA=',
        lastUpdated: '2019-04-23T03:32:13.384145',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      departmentName: 'MOE',
      groups: [],
      currentDepartmentId: 14350,
      departmentId: 14350,
      firstName: 'Ha Tang Thanh',
      mobileCountryCode: 65,
      mobileNumber: '988605109',
      emailAddress: 'idrawlife@gmail.com',
      ssn: 'G1481481T',
      gender: 0,
      dateOfBirth: '2019-04-22T00:00:00',
      tag: '',
      created: '2019-03-25T11:01:00',
      forceLoginAgain: false,
      roles: [
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Branch Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'branchadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 88
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'courseadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 92
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Facilitator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'coursefacilitator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 93
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Guest '
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'guest',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 100
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Executive and Administrative Staff​'
                },
                {
                  name: 'Description',
                  localizedText:
                    'Service Scheme: Executive and Administrative Staff​'
                }
              ]
            }
          ],
          identity: {
            extId: 'Scheme-EAS',
            ownerId: 3001,
            customerId: 0,
            archetype: 'PersonnelGroup',
            id: 104
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        }
      ],
      identity: {
        extId: 'd0de2846-6a9f-4089-82bb-977cd186d503',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 13
      },
      jsonDynamicAttributes: [],
      entityStatus: {
        externallyMastered: false,
        lastExternallySynchronized: '2019-03-25T19:01:26.3733333',
        entityVersion: 'AAAAAAAACs0=',
        lastUpdated: '2019-04-22T10:26:35.9975097',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      departmentName: 'MOE',
      groups: [],
      currentDepartmentId: 14350,
      departmentId: 14350,
      firstName: 'Jack2 Vo',
      mobileCountryCode: 47,
      emailAddress: 'cujib@blackbird.ws',
      gender: 0,
      dateOfBirth: '2019-04-22T00:00:00',
      tag: '',
      created: '2019-04-22T06:09:00',
      forceLoginAgain: false,
      roles: [
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Super Administrator'
                },
                {
                  name: 'Description',
                  localizedText: 'vip24 Super administrator'
                }
              ]
            }
          ],
          identity: {
            extId: 'superadministrator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 1
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Branch Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'branchadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 88
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Content Creator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'coursecontentcreator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 91
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'courseadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 92
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Facilitator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'coursefacilitator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 93
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Content Creator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'contentcreator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 96
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Guest '
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'guest',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 100
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        }
      ],
      identity: {
        extId: '5ea43cc2-63e3-4631-b127-1308fda9e7fd',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 31
      },
      jsonDynamicAttributes: [],
      entityStatus: {
        externallyMastered: false,
        lastExternallySynchronized: '2019-04-22T14:09:15.0533333',
        entityVersion: 'AAAAAAAACsE=',
        lastUpdated: '2019-04-22T10:01:55.2170859',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      departmentName: 'MOE',
      groups: [],
      currentDepartmentId: 14350,
      departmentId: 14350,
      firstName: 'Jackie - Tu Nguyen Ngoc',
      mobileCountryCode: 65,
      mobileNumber: '123',
      emailAddress: 'tu.nguyen@orientsoftware.com',
      ssn: 'S1481481G',
      gender: 1,
      dateOfBirth: '1955-06-10T00:00:00',
      tag: '',
      created: '2019-03-25T11:01:00',
      forceLoginAgain: false,
      roles: [
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Super Administrator'
                },
                {
                  name: 'Description',
                  localizedText: 'vip24 Super administrator'
                }
              ]
            }
          ],
          identity: {
            extId: 'superadministrator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 1
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Employee'
                },
                {
                  name: 'Description',
                  localizedText: '+Competence users only'
                }
              ]
            }
          ],
          identity: {
            extId: 'employee',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 49
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Learner'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'learner',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 99
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Educating Officer'
                },
                {
                  name: 'Description',
                  localizedText: 'Educating Officer'
                }
              ]
            }
          ],
          identity: {
            extId: 'Scheme-EO',
            ownerId: 3001,
            customerId: 0,
            archetype: 'PersonnelGroup',
            id: 101
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Leadership Growth Model'
                },
                {
                  name: 'Description',
                  localizedText: 'Growth Model: Leadership Growth Model'
                }
              ]
            }
          ],
          identity: {
            extId: 'GM-LGM',
            ownerId: 3001,
            customerId: 0,
            archetype: 'CareerPath',
            id: 106
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        }
      ],
      identity: {
        extId: '8b44d1a9-7a6b-48e7-8cf3-5df2d932c0ed',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 8
      },
      jsonDynamicAttributes: [],
      entityStatus: {
        externallyMastered: false,
        lastExternallySynchronized: '2019-03-25T19:01:26.3166667',
        entityVersion: 'AAAAAAAACo8=',
        lastUpdated: '2019-04-22T08:42:36.8076102',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      departmentName: 'MOE',
      groups: [],
      currentDepartmentId: 14350,
      departmentId: 14350,
      firstName: 'John',
      mobileCountryCode: 65,
      mobileNumber: '988605109',
      emailAddress: 'tung.dao@orientsoftware.com',
      ssn: 'F0234234Q',
      gender: 1,
      dateOfBirth: '1987-09-10T00:00:00',
      tag: '',
      created: '2019-03-25T11:01:00',
      forceLoginAgain: false,
      roles: [
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Branch Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'branchadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 88
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Division Training Coordinator'
                },
                {
                  name: 'Description',
                  localizedText:
                    'Division Training Coordinator/\nDivisional Learning Coordinator/School Staff Developer'
                }
              ]
            }
          ],
          identity: {
            extId: 'divisiontrainingcoordinator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 90
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Content Creator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'coursecontentcreator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 91
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Approving Officer'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'reportingofficer',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 97
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Guest '
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'guest',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 100
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Executive and Administrative Staff​'
                },
                {
                  name: 'Description',
                  localizedText:
                    'Service Scheme: Executive and Administrative Staff​'
                }
              ]
            }
          ],
          identity: {
            extId: 'Scheme-EAS',
            ownerId: 3001,
            customerId: 0,
            archetype: 'PersonnelGroup',
            id: 104
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Leadership Growth Model'
                },
                {
                  name: 'Description',
                  localizedText: 'Growth Model: Leadership Growth Model'
                }
              ]
            }
          ],
          identity: {
            extId: 'GM-LGM',
            ownerId: 3001,
            customerId: 0,
            archetype: 'CareerPath',
            id: 106
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        }
      ],
      identity: {
        extId: '5FC846F2-11F6-4799-A45A-B24B03106473',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 12
      },
      jsonDynamicAttributes: [],
      entityStatus: {
        externallyMastered: false,
        lastExternallySynchronized: '2019-03-25T19:01:26.36',
        entityVersion: 'AAAAAAAACqo=',
        lastUpdated: '2019-04-22T09:36:33.2063642',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      departmentName: 'MOE',
      groups: [],
      currentDepartmentId: 14350,
      departmentId: 14350,
      firstName: 'Kai',
      mobileCountryCode: 65,
      mobileNumber: '1234567891',
      emailAddress: 'kiet.truong@orientsoftware.com',
      ssn: 'S1234557G',
      gender: 0,
      dateOfBirth: '2019-04-27T00:00:00',
      tag: '',
      created: '2019-03-28T02:47:00',
      forceLoginAgain: false,
      roles: [
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Super Administrator'
                },
                {
                  name: 'Description',
                  localizedText: 'vip24 Super administrator'
                }
              ]
            }
          ],
          identity: {
            extId: 'superadministrator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 1
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Candidate'
                },
                {
                  name: 'Description',
                  localizedText: 'vip24 Candidate'
                }
              ]
            }
          ],
          identity: {
            extId: 'candidate',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 39
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Leader'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'leader',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 42
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Employee'
                },
                {
                  name: 'Description',
                  localizedText: '+Competence users only'
                }
              ]
            }
          ],
          identity: {
            extId: 'employee',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 49
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Dnb Sponsored Coaching'
                },
                {
                  name: 'Description',
                  localizedText: 'Dnb Sponsored Coaching'
                }
              ]
            }
          ],
          identity: {
            extId: 'dnbsponsoredcoaching',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 85
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Digital Coach'
                },
                {
                  name: 'Description',
                  localizedText: 'Digital Coach'
                }
              ]
            }
          ],
          identity: {
            extId: 'digitalcoach',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 86
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Division Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'divisionadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 87
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'School Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'schooladmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 89
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Content Creator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'coursecontentcreator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 91
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'courseadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 92
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Digital Content Approving Officer'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'digitalcontentreportingofficer',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 95
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Content Creator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'contentcreator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 96
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Reporting Officer '
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'reportingofficer',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 98
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Guest '
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'guest',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 100
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Educating Officer'
                },
                {
                  name: 'Description',
                  localizedText: 'Educating Officer'
                }
              ]
            }
          ],
          identity: {
            extId: 'Scheme-EO',
            ownerId: 3001,
            customerId: 0,
            archetype: 'PersonnelGroup',
            id: 101
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Specialist Growth Model'
                },
                {
                  name: 'Description',
                  localizedText: 'Growth Model: Specialist Growth Model'
                }
              ]
            }
          ],
          identity: {
            extId: 'GM-SGM',
            ownerId: 3001,
            customerId: 0,
            archetype: 'CareerPath',
            id: 107
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        }
      ],
      identity: {
        extId: 'e3fac4f2-bff2-42cb-8986-3c900ca67b28',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 18
      },
      jsonDynamicAttributes: [],
      entityStatus: {
        externallyMastered: false,
        lastExternallySynchronized: '2019-03-28T10:46:58.32',
        entityVersion: 'AAAAAAAACvQ=',
        lastUpdated: '2019-04-26T03:25:23.4616201',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      departmentName: 'MOE',
      groups: [],
      currentDepartmentId: 14350,
      departmentId: 14350,
      firstName: 'Ken 123 test',
      mobileCountryCode: 65,
      emailAddress: 'nghia.dao@orientsoftware.net',
      ssn: 'S1234567G',
      gender: 0,
      dateOfBirth: '2019-04-18T00:00:00',
      tag: '',
      created: '2019-04-12T10:45:00',
      forceLoginAgain: false,
      roles: [
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Super Administrator'
                },
                {
                  name: 'Description',
                  localizedText: 'vip24 Super administrator'
                }
              ]
            }
          ],
          identity: {
            extId: 'superadministrator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 1
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Employee'
                },
                {
                  name: 'Description',
                  localizedText: '+Competence users only'
                }
              ]
            }
          ],
          identity: {
            extId: 'employee',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 49
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Division Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'divisionadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 87
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Branch Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'branchadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 88
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Content Creator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'coursecontentcreator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 91
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'courseadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 92
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Facilitator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'coursefacilitator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 93
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Approving Officer'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'reportingofficer',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 97
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Learner'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'learner',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 99
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Specialist Growth Model'
                },
                {
                  name: 'Description',
                  localizedText: 'Growth Model: Specialist Growth Model'
                }
              ]
            }
          ],
          identity: {
            extId: 'GM-SGM',
            ownerId: 3001,
            customerId: 0,
            archetype: 'CareerPath',
            id: 107
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        }
      ],
      identity: {
        extId: 'e09ccc6b-7836-45d1-b55a-830e6c023992',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 20
      },
      jsonDynamicAttributes: [],
      entityStatus: {
        externallyMastered: false,
        lastExternallySynchronized: '2019-04-12T18:45:00.41',
        entityVersion: 'AAAAAAAACvI=',
        lastUpdated: '2019-04-25T10:49:42.2772836',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      departmentName: 'MOE',
      groups: [],
      currentDepartmentId: 14350,
      departmentId: 14350,
      firstName: 'Kyle Nguyen',
      mobileCountryCode: 47,
      emailAddress: 'sasih@mail-list.top',
      gender: 0,
      dateOfBirth: '2019-04-22T00:00:00',
      tag: '',
      created: '2019-04-22T04:09:00',
      forceLoginAgain: false,
      roles: [
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Super Administrator'
                },
                {
                  name: 'Description',
                  localizedText: 'vip24 Super administrator'
                }
              ]
            }
          ],
          identity: {
            extId: 'superadministrator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 1
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Employee'
                },
                {
                  name: 'Description',
                  localizedText: '+Competence users only'
                }
              ]
            }
          ],
          identity: {
            extId: 'employee',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 49
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Division Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'divisionadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 87
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Branch Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'branchadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 88
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Content Creator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'coursecontentcreator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 91
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Approving Officer'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'reportingofficer',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 97
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Learner'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'learner',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 99
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        }
      ],
      identity: {
        extId: 'ae5953fd-44f3-4ae2-b104-1b5e4bb802f5',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 25
      },
      jsonDynamicAttributes: [],
      entityStatus: {
        externallyMastered: false,
        lastExternallySynchronized: '2019-04-22T12:09:21.99',
        entityVersion: 'AAAAAAAACoU=',
        lastUpdated: '2019-04-22T15:41:05.6933333',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      departmentName: 'MOE',
      groups: [],
      currentDepartmentId: 14350,
      departmentId: 14350,
      firstName: 'Matthew 1 2',
      mobileCountryCode: 47,
      emailAddress: 'sufeyom@businessagent.email',
      gender: 0,
      dateOfBirth: '2019-04-22T00:00:00',
      tag: '',
      created: '2019-04-22T04:13:00',
      forceLoginAgain: false,
      roles: [
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Super Administrator'
                },
                {
                  name: 'Description',
                  localizedText: 'vip24 Super administrator'
                }
              ]
            }
          ],
          identity: {
            extId: 'superadministrator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 1
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Division Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'divisionadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 87
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Division Training Coordinator'
                },
                {
                  name: 'Description',
                  localizedText:
                    'Division Training Coordinator/\nDivisional Learning Coordinator/School Staff Developer'
                }
              ]
            }
          ],
          identity: {
            extId: 'divisiontrainingcoordinator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 90
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Facilitator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'coursefacilitator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 93
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Learner'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'learner',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 99
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Educating Officer'
                },
                {
                  name: 'Description',
                  localizedText: 'Educating Officer'
                }
              ]
            }
          ],
          identity: {
            extId: 'Scheme-EO',
            ownerId: 3001,
            customerId: 0,
            archetype: 'PersonnelGroup',
            id: 101
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Teacher Growth Model'
                },
                {
                  name: 'Description',
                  localizedText: 'Growth Model: Teacher Growth Model'
                }
              ]
            }
          ],
          identity: {
            extId: 'GM-TGM',
            ownerId: 3001,
            customerId: 0,
            archetype: 'CareerPath',
            id: 105
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        }
      ],
      identity: {
        extId: 'b5accd8a-d7b5-410f-809c-d0f627711d23',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 26
      },
      jsonDynamicAttributes: [],
      entityStatus: {
        externallyMastered: false,
        lastExternallySynchronized: '2019-04-22T12:13:02.24',
        entityVersion: 'AAAAAAAACtE=',
        lastUpdated: '2019-04-22T10:33:42.4150392',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      departmentName: 'MOE',
      groups: [],
      currentDepartmentId: 14350,
      departmentId: 14350,
      firstName: 'Persie Van',
      mobileCountryCode: 47,
      emailAddress: 'tofiha@max-mail.info',
      gender: 0,
      tag: '',
      created: '2019-04-22T06:35:00',
      forceLoginAgain: false,
      roles: [
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Super Administrator'
                },
                {
                  name: 'Description',
                  localizedText: 'vip24 Super administrator'
                }
              ]
            }
          ],
          identity: {
            extId: 'superadministrator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 1
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        }
      ],
      identity: {
        extId: 'e23577a2-f916-4131-848e-45d70839c103',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 34
      },
      jsonDynamicAttributes: [],
      entityStatus: {
        externallyMastered: false,
        lastExternallySynchronized: '2019-04-22T14:34:47.48',
        entityVersion: 'AAAAAAAACmA=',
        lastUpdated: '2019-04-22T14:36:14.25',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      departmentName: 'MOE',
      groups: [],
      currentDepartmentId: 14350,
      departmentId: 14350,
      firstName: 'Sam Nguyen',
      mobileCountryCode: 47,
      emailAddress: 'japan@businessagent.email',
      gender: 0,
      dateOfBirth: '1952-04-16T00:00:00',
      tag: '',
      created: '2019-04-22T06:59:00',
      forceLoginAgain: false,
      roles: [
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Super Administrator'
                },
                {
                  name: 'Description',
                  localizedText: 'vip24 Super administrator'
                }
              ]
            }
          ],
          identity: {
            extId: 'superadministrator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 1
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Content Creator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'coursecontentcreator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 91
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Facilitator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'coursefacilitator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 93
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Guest '
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'guest',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 100
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Executive and Administrative Staff​'
                },
                {
                  name: 'Description',
                  localizedText:
                    'Service Scheme: Executive and Administrative Staff​'
                }
              ]
            }
          ],
          identity: {
            extId: 'Scheme-EAS',
            ownerId: 3001,
            customerId: 0,
            archetype: 'PersonnelGroup',
            id: 104
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Leadership Growth Model'
                },
                {
                  name: 'Description',
                  localizedText: 'Growth Model: Leadership Growth Model'
                }
              ]
            }
          ],
          identity: {
            extId: 'GM-LGM',
            ownerId: 3001,
            customerId: 0,
            archetype: 'CareerPath',
            id: 106
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        }
      ],
      identity: {
        extId: '8b4799eb-11f1-44ed-a9b8-afbe64af66a8',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 40
      },
      jsonDynamicAttributes: [],
      entityStatus: {
        externallyMastered: false,
        lastExternallySynchronized: '2019-04-22T14:59:29.38',
        entityVersion: 'AAAAAAAACts=',
        lastUpdated: '2019-04-23T03:26:10.2458621',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      departmentName: 'MOE',
      groups: [],
      currentDepartmentId: 14350,
      departmentId: 14350,
      firstName: 'Shane - Sang Nguyen Thai',
      mobileCountryCode: 65,
      mobileNumber: '1234567891',
      emailAddress: 'sang.nguyen@orientsoftware.com',
      ssn: 'S1234557G',
      gender: 1,
      dateOfBirth: '1992-07-01T00:00:00',
      tag: '',
      created: '2019-03-25T11:01:00',
      forceLoginAgain: false,
      roles: [
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Content Creator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'coursecontentcreator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 91
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Learner'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'learner',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 99
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Executive and Administrative Staff​'
                },
                {
                  name: 'Description',
                  localizedText:
                    'Service Scheme: Executive and Administrative Staff​'
                }
              ]
            }
          ],
          identity: {
            extId: 'Scheme-EAS',
            ownerId: 3001,
            customerId: 0,
            archetype: 'PersonnelGroup',
            id: 104
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Leadership Growth Model'
                },
                {
                  name: 'Description',
                  localizedText: 'Growth Model: Leadership Growth Model'
                }
              ]
            }
          ],
          identity: {
            extId: 'GM-LGM',
            ownerId: 3001,
            customerId: 0,
            archetype: 'CareerPath',
            id: 106
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        }
      ],
      identity: {
        extId: '993ca6fc-e884-4c8b-8e64-61a44ade6134',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 10
      },
      jsonDynamicAttributes: [],
      entityStatus: {
        externallyMastered: true,
        lastExternallySynchronized: '2019-03-25T19:01:26.3333333',
        entityVersion: 'AAAAAAAACkY=',
        lastUpdated: '2019-04-22T04:22:25.1829918',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      departmentName: 'MOE',
      groups: [],
      currentDepartmentId: 14350,
      departmentId: 14350,
      firstName: 'Steffen Nguyen',
      mobileCountryCode: 47,
      emailAddress: 'homujefulo@dreamcatcher.email',
      gender: 0,
      tag: '',
      created: '2019-04-22T04:22:00',
      forceLoginAgain: false,
      roles: [
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Super Administrator'
                },
                {
                  name: 'Description',
                  localizedText: 'vip24 Super administrator'
                }
              ]
            }
          ],
          identity: {
            extId: 'superadministrator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 1
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Division Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'divisionadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 87
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Branch Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'branchadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 88
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'courseadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 92
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Approving Officer'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'reportingofficer',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 97
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Learner'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'learner',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 99
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        }
      ],
      identity: {
        extId: 'aaeec0a8-0b14-4fb5-8728-b12d2aee0d61',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 27
      },
      jsonDynamicAttributes: [],
      entityStatus: {
        externallyMastered: false,
        lastExternallySynchronized: '2019-04-22T12:22:18.2433333',
        entityVersion: 'AAAAAAAACno=',
        lastUpdated: '2019-04-22T15:05:16.6',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      departmentName: 'MOE',
      groups: [],
      currentDepartmentId: 14350,
      departmentId: 14350,
      firstName: 'Theo',
      mobileCountryCode: 65,
      mobileNumber: '123',
      emailAddress: 'thanh.nguyen@orientsoftware.com',
      ssn: 'S1234567G',
      gender: 0,
      dateOfBirth: '1955-06-10T00:00:00',
      tag: '',
      created: '2019-03-25T11:01:00',
      forceLoginAgain: false,
      roles: [
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Employee'
                },
                {
                  name: 'Description',
                  localizedText: '+Competence users only'
                }
              ]
            }
          ],
          identity: {
            extId: 'employee',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 49
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Division Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'divisionadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 87
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Branch Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'branchadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 88
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'School Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'schooladmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 89
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Division Training Coordinator'
                },
                {
                  name: 'Description',
                  localizedText:
                    'Division Training Coordinator/\nDivisional Learning Coordinator/School Staff Developer'
                }
              ]
            }
          ],
          identity: {
            extId: 'divisiontrainingcoordinator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 90
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Content Creator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'coursecontentcreator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 91
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'courseadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 92
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Facilitator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'coursefacilitator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 93
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'WTTG Representative'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'wttgrepresentative',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 94
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Digital Content Approving Officer'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'digitalcontentreportingofficer',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 95
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Content Creator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'contentcreator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 96
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Approving Officer'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'reportingofficer',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 97
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Reporting Officer '
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'reportingofficer',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 98
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Learner'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'learner',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 99
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Guest '
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'guest',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 100
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Educating Officer'
                },
                {
                  name: 'Description',
                  localizedText: 'Educating Officer'
                }
              ]
            }
          ],
          identity: {
            extId: 'Scheme-EO',
            ownerId: 3001,
            customerId: 0,
            archetype: 'PersonnelGroup',
            id: 101
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Leadership Growth Model'
                },
                {
                  name: 'Description',
                  localizedText: 'Growth Model: Leadership Growth Model'
                }
              ]
            }
          ],
          identity: {
            extId: 'GM-LGM',
            ownerId: 3001,
            customerId: 0,
            archetype: 'CareerPath',
            id: 106
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        }
      ],
      identity: {
        extId: 'df2aba95-7b2c-44d9-a00d-da79683d60f6',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 9
      },
      jsonDynamicAttributes: [],
      entityStatus: {
        externallyMastered: false,
        lastExternallySynchronized: '2019-03-25T19:01:26.3233333',
        entityVersion: 'AAAAAAAACnc=',
        lastUpdated: '2019-04-22T15:04:26.6833333',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      departmentName: 'MOE',
      groups: [],
      currentDepartmentId: 14350,
      departmentId: 14350,
      firstName: 'Tom Lee',
      mobileCountryCode: 47,
      emailAddress: 'pejalacet@maillink.live',
      gender: 0,
      tag: '',
      created: '2019-04-22T06:30:00',
      forceLoginAgain: false,
      roles: [
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Super Administrator'
                },
                {
                  name: 'Description',
                  localizedText: 'vip24 Super administrator'
                }
              ]
            }
          ],
          identity: {
            extId: 'superadministrator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 1
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        }
      ],
      identity: {
        extId: 'd1a9f27e-1bc9-427a-81e7-34277145711b',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 32
      },
      jsonDynamicAttributes: [],
      entityStatus: {
        externallyMastered: false,
        lastExternallySynchronized: '2019-04-22T14:30:04.7666667',
        entityVersion: 'AAAAAAAACls=',
        lastUpdated: '2019-04-22T14:31:31.8833333',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    },
    {
      departmentName: 'MOE',
      groups: [],
      currentDepartmentId: 14350,
      departmentId: 14350,
      firstName: 'Tung Nguyen',
      mobileCountryCode: 47,
      emailAddress: 'kogonoy@maillink.live',
      gender: 0,
      tag: '',
      created: '2019-04-22T06:50:00',
      forceLoginAgain: false,
      roles: [
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Super Administrator'
                },
                {
                  name: 'Description',
                  localizedText: 'vip24 Super administrator'
                }
              ]
            }
          ],
          identity: {
            extId: 'superadministrator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'Role',
            id: 1
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Branch Administrator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'branchadmin',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 88
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Course Facilitator'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'coursefacilitator',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 93
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'WTTG Representative'
                },
                {
                  name: 'Description',
                  localizedText: ''
                }
              ]
            }
          ],
          identity: {
            extId: 'wttgrepresentative',
            ownerId: 3001,
            customerId: 0,
            archetype: 'SystemRole',
            id: 94
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Educating Officer'
                },
                {
                  name: 'Description',
                  localizedText: 'Educating Officer'
                }
              ]
            }
          ],
          identity: {
            extId: 'Scheme-EO',
            ownerId: 3001,
            customerId: 0,
            archetype: 'PersonnelGroup',
            id: 101
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        },
        {
          localizedData: [
            {
              id: 2,
              languageCode: 'en-US',
              fields: [
                {
                  name: 'Name',
                  localizedText: 'Leadership Growth Model'
                },
                {
                  name: 'Description',
                  localizedText: 'Growth Model: Leadership Growth Model'
                }
              ]
            }
          ],
          identity: {
            extId: 'GM-LGM',
            ownerId: 3001,
            customerId: 0,
            archetype: 'CareerPath',
            id: 106
          },
          entityStatus: {
            externallyMastered: false,
            lastUpdated: '0001-01-01T00:00:00',
            lastUpdatedBy: 0,
            statusId: 'Active',
            statusReasonId: 'Unknown',
            deleted: false
          }
        }
      ],
      identity: {
        extId: '333d3466-92ea-4047-af41-08080529c176',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 39
      },
      jsonDynamicAttributes: [],
      entityStatus: {
        externallyMastered: false,
        lastExternallySynchronized: '2019-04-22T14:50:04.6533333',
        entityVersion: 'AAAAAAAACu4=',
        lastUpdated: '2019-04-23T04:18:34.0822881',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    }
  ],
  editFormJsonMockData: {
    title: 'Edit User',
    elements: [
      {
        type: 'html',
        name: 'avatar',
        readOnly: true
      },
      {
        type: 'text',
        name: 'firstName',
        title: 'Full Name',
        isRequired: true,
        requiredErrorText: 'Full name is required',
        maxLength: 50,
        readOnly: true
      },
      {
        type: 'text',
        name: 'emailAddress',
        title: 'Email',
        isRequired: true,
        requiredErrorText: 'Email is required',
        readOnly: true
      },
      {
        type: 'text',
        name: 'ssn',
        title: 'UIN/FIN',
        width: '50%',
        validators: [
          {
            type: 'regex',
            text: 'Invalid NRIC',
            regex: '^[STFG]\\d{7}[A-Z]$'
          }
        ],
        readOnly: true
      },
      {
        type: 'text',
        inputType: 'number',
        name: 'mobileCountryCode',
        title: 'Country Code',
        defaultValue: '65',
        startWithNewLine: false,
        width: '25%',
        readOnly: true
      },
      {
        type: 'text',
        inputType: 'number',
        name: 'mobileNumber',
        title: 'Mobile Number',
        startWithNewLine: false,
        width: '25%',
        readOnly: true
      },
      {
        name: 'dateOfBirth',
        type: 'datepicker',
        inputType: 'date',
        title: 'Date of birth',
        dateFormat: 'dd/mm/yy',
        config: {
          changeMonth: true,
          changeYear: true,
          yearRange: '1950:2019'
        },
        width: '50%',
        readOnly: true
      },
      {
        type: 'radiogroup',
        name: 'gender',
        title: 'Gender',
        defaultValue: 0,
        width: '50%',
        startWithNewLine: false,
        colCount: 3,
        choices: [
          {
            value: 0,
            text: 'Male'
          },
          {
            value: 1,
            text: 'Female'
          },
          {
            value: 2,
            text: 'Other'
          }
        ],
        readOnly: true
      },
      {
        type: 'dropdown',
        name: 'careerPaths',
        title: 'Growth Model',
        width: '50%',
        readOnly: true,
        choices: [
          {
            value: '105',
            text: 'Teacher Growth Model'
          },
          {
            value: '106',
            text: 'Leadership Growth Model'
          },
          {
            value: '107',
            text: 'Specialist Growth Model'
          }
        ]
      },
      {
        type: 'dropdown',
        name: 'personnelGroups',
        title: 'Service Scheme',
        startWithNewLine: false,
        width: '50%',
        readOnly: true,
        choices: [
          {
            value: '101',
            text: 'Educating Officer'
          },
          {
            value: '102',
            text: 'MOE Kindergarten Educator'
          },
          {
            value: '103',
            text: 'Allied Educator'
          },
          {
            value: '104',
            text: 'Executive and Administrative Staff​'
          }
        ]
      },
      {
        type: 'checkbox',
        name: 'systemRoles',
        title: 'Roles',
        colCount: 3,
        readOnly: true,
        choices: [
          {
            value: '87',
            text: 'Division Administrator'
          },
          {
            value: '88',
            text: 'Branch Administrator'
          },
          {
            value: '89',
            text: 'School Administrator'
          },
          {
            value: '90',
            text: 'Division Training Coordinator'
          },
          {
            value: '91',
            text: 'Course Content Creator'
          },
          {
            value: '92',
            text: 'Course Administrator'
          },
          {
            value: '93',
            text: 'Course Facilitator'
          },
          {
            value: '94',
            text: 'WTTG Representative'
          },
          {
            value: '95',
            text: 'Digital Content Approving Officer'
          },
          {
            value: '96',
            text: 'Content Creator'
          },
          {
            value: '97',
            text: 'Approving Officer'
          },
          {
            value: '98',
            text: 'Reporting Officer '
          },
          {
            value: '99',
            text: 'Learner'
          },
          {
            value: '100',
            text: 'Guest '
          }
        ]
      }
    ]
  },
  mockCreateUserFormData: {
    title: 'Create New User',
    elements: [
      {
        type: 'text',
        name: 'firstName',
        title: 'Full Name',
        isRequired: true,
        requiredErrorText: 'Full name is required',
        maxLength: 50,
        readOnly: true
      },
      {
        type: 'text',
        name: 'emailAddress',
        title: 'Email',
        isRequired: true,
        requiredErrorText: 'Email is required',
        readOnly: true
      },
      {
        type: 'text',
        name: 'ssn',
        title: 'UIN/FIN',
        width: '50%',
        validators: [
          {
            type: 'regex',
            text: 'Invalid NRIC',
            regex: '^[STFG]\\d{7}[A-Z]$'
          }
        ],
        readOnly: true
      },
      {
        type: 'text',
        inputType: 'number',
        name: 'mobileCountryCode',
        title: 'Country Code',
        defaultValue: '65',
        startWithNewLine: false,
        width: '25%',
        readOnly: true
      },
      {
        name: 'dateOfBirth',
        type: 'datepicker',
        inputType: 'date',
        title: 'Date of birth',
        dateFormat: 'dd/mm/yy',
        config: {
          changeMonth: true,
          changeYear: true,
          yearRange: '1950:2019'
        },
        width: '50%',
        readOnly: true
      },
      {
        type: 'radiogroup',
        name: 'gender',
        title: 'Gender',
        defaultValue: 0,
        width: '50%',
        startWithNewLine: false,
        colCount: 3,
        choices: [
          {
            value: 0,
            text: 'Male'
          },
          {
            value: 1,
            text: 'Female'
          },
          {
            value: 2,
            text: 'Other'
          }
        ],
        readOnly: true
      },
      {
        type: 'dropdown',
        name: 'careerPaths',
        title: 'Growth Model',
        width: '50%',
        readOnly: true,
        choices: [
          {
            value: '105',
            text: 'Teacher Growth Model'
          },
          {
            value: '106',
            text: 'Leadership Growth Model'
          },
          {
            value: '107',
            text: 'Specialist Growth Model'
          }
        ]
      },
      {
        type: 'dropdown',
        name: 'personnelGroups',
        title: 'Service Scheme',
        startWithNewLine: false,
        width: '50%',
        readOnly: true,
        choices: [
          {
            value: '101',
            text: 'Educating Officer'
          },
          {
            value: '102',
            text: 'MOE Kindergarten Educator'
          },
          {
            value: '103',
            text: 'Allied Educator'
          },
          {
            value: '104',
            text: 'Executive and Administrative Staff​'
          }
        ]
      },
      {
        type: 'checkbox',
        name: 'systemRoles',
        title: 'Roles',
        colCount: 3,
        readOnly: true,
        choices: [
          {
            value: '87',
            text: 'Division Administrator'
          },
          {
            value: '88',
            text: 'Branch Administrator'
          },
          {
            value: '89',
            text: 'School Administrator'
          },
          {
            value: '90',
            text: 'Division Training Coordinator'
          },
          {
            value: '91',
            text: 'Course Content Creator'
          },
          {
            value: '92',
            text: 'Course Administrator'
          },
          {
            value: '93',
            text: 'Course Facilitator'
          },
          {
            value: '94',
            text: 'WTTG Representative'
          },
          {
            value: '95',
            text: 'Digital Content Approving Officer'
          },
          {
            value: '96',
            text: 'Content Creator'
          },
          {
            value: '97',
            text: 'Approving Officer'
          },
          {
            value: '98',
            text: 'Reporting Officer '
          },
          {
            value: '99',
            text: 'Learner'
          },
          {
            value: '100',
            text: 'Guest '
          }
        ]
      }
    ]
  },
  mockCreatedUserData: {
    departmentId: 14350,
    firstName: 'Tester Tung',
    emailAddress: 'testertung@k.com',
    gender: 0,
    systemRoles: [
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Division Administrator'
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'divisionadmin',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 87
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Division Training Coordinator'
              },
              {
                name: 'Description',
                localizedText:
                  'Division Training Coordinator/\nDivisional Learning Coordinator/School Staff Developer'
              }
            ]
          }
        ],
        identity: {
          extId: 'divisiontrainingcoordinator',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 90
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Course Facilitator'
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'coursefacilitator',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 93
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Content Creator'
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'contentcreator',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 96
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Learner'
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'learner',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 99
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Guest '
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'guest',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 100
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Approving Officer'
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'reportingofficer',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 97
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'WTTG Representative'
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'wttgrepresentative',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 94
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Course Content Creator'
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'coursecontentcreator',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 91
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Branch Administrator'
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'branchadmin',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 88
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'School Administrator'
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'schooladmin',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 89
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Course Administrator'
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'courseadmin',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 92
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Digital Content Approving Officer'
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'digitalcontentreportingofficer',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 95
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      },
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Reporting Officer '
              },
              {
                name: 'Description',
                localizedText: ''
              }
            ]
          }
        ],
        identity: {
          extId: 'reportingofficer',
          ownerId: 3001,
          customerId: 0,
          archetype: 'SystemRole',
          id: 98
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      }
    ],
    personnelGroups: [
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Educating Officer'
              },
              {
                name: 'Description',
                localizedText: 'Educating Officer'
              }
            ]
          }
        ],
        identity: {
          extId: 'Scheme-EO',
          ownerId: 3001,
          customerId: 0,
          archetype: 'PersonnelGroup',
          id: 101
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      }
    ],
    careerPaths: [
      {
        localizedData: [
          {
            id: 2,
            languageCode: 'en-US',
            fields: [
              {
                name: 'Name',
                localizedText: 'Leadership Growth Model'
              },
              {
                name: 'Description',
                localizedText: 'Growth Model: Leadership Growth Model'
              }
            ]
          }
        ],
        identity: {
          extId: 'GM-LGM',
          ownerId: 3001,
          customerId: 0,
          archetype: 'CareerPath',
          id: 106
        },
        entityStatus: {
          externallyMastered: false,
          lastUpdated: '0001-01-01T00:00:00',
          lastUpdatedBy: 0,
          statusId: 'Active',
          statusReasonId: 'Unknown',
          deleted: false
        }
      }
    ],
    groups: [],
    dateOfBirth: '2019-05-07T00:00:00',
    identity: {
      ownerId: 3001,
      customerId: 2052,
      extId: '',
      archetype: 'Employee',
      id: 501
    },
    entityStatus: {
      statusId: 'Active',
      statusReasonId: 'Unknown',
      externallyMastered: false,
      lastExternallySynchronized: '0001-01-01T00:00:00',
      entityVersion: 'AAAAAAAAFtI=',
      lastUpdated: '2019-05-27T06:47:57.4028407+00:00',
      lastUpdatedBy: 0,
      deleted: false
    },
    departmentName: 'MOE',
    otpValue: '71G88T7Z',
    mobileCountryCode: 65,
    tag: '',
    created: '2019-05-27T06:47:57.4028396+00:00',
    forceLoginAgain: false,
    jsonDynamicAttributes: []
  }
};

export const pagingUserAccount = new PagingResponseModel<UserManagement>({
  pageIndex: 1,
  pageSize: 50,
  items: [
    {
      departmentId: 14350,
      firstName: 'Geir Fuhre Pettersen',
      mobileCountryCode: 65,
      emailAddress: 'gfp@conexus.no',
      gender: 0,
      tag: '',
      created: '2019-03-25T11:01:00Z',
      loginServiceClaims: [],
      forceLoginAgain: false,
      roles: [],
      identity: {
        extId: 'MOE000001',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 6
      },
      jsonDynamicAttributes: {},
      departmentName: '',
      groups: [],
      mobileNumber: '',
      ssn: '',
      dateOfBirth: '',
      resetOtp: '',
      otpValue: '',
      entityStatus: new EntityStatus({
        externallyMastered: true,
        lastExternallySynchronized: '2019-03-25T19:01:26.2833333Z',
        entityVersion: 'AAAAAAAACGU=',
        lastUpdated: '2019-03-25T19:01:26.2833333Z',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      })
    },
    {
      departmentId: 14350,
      firstName: 'Jackie Nguyen',
      mobileCountryCode: 65,
      emailAddress: 'tu.nguyen@orientsoftware.com',
      gender: 0,
      tag: '',
      created: '2019-03-25T11:01:00Z',
      loginServiceClaims: [],
      forceLoginAgain: false,
      roles: [],
      identity: {
        extId: 'MOE000003',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'Employee',
        id: 8
      },
      jsonDynamicAttributes: {},
      departmentName: '',
      groups: [],
      mobileNumber: '',
      ssn: '',
      dateOfBirth: '',
      resetOtp: '',
      otpValue: '',
      entityStatus: new EntityStatus({
        externallyMastered: true,
        lastExternallySynchronized: '2019-03-25T19:01:26.3166667Z',
        entityVersion: 'AAAAAAAACHg=',
        lastUpdated: '2019-03-26T14:33:58.5Z',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      })
    }
  ],
  totalItems: 170
});

export const DefaultSystemRoleData: any = {
  localizedData: [
    {
      id: 2,
      languageCode: 'en-US',
      fields: [
        {
          name: 'Name',
          localizedText: 'Learner'
        },
        {
          name: 'Description',
          localizedText: ''
        }
      ]
    }
  ],
  identity: {
    extId: 'learner',
    ownerId: 3001,
    customerId: 0,
    archetype: 'SystemRole',
    id: 99
  },
  entityStatus: {
    externallyMastered: false,
    lastUpdated: '0001-01-01T00:00:00Z',
    lastUpdatedBy: 0,
    statusId: 'Active',
    statusReasonId: 'Unknown',
    deleted: false
  }
};
