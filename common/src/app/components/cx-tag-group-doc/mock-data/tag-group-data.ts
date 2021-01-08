export const tagGroupData = {
    groups: [
        {
            name: 'Status',
            const: 'STATUS',
            listFilter: [
                {
                    name: 'Active',
                    isSelected: true,
                    value: 'Active'
                },
                {
                    name: 'Deactive',
                    isSelected: true,
                    value: 'Deactive'
                },
                {
                    name: 'Suspended',
                    isSelected: true,
                    value: 'Inactive'
                },
                {
                    name: 'Locked',
                    isSelected: false,
                    value: 'Locked'
                }
            ]
        },
        {
            name: 'Role',
            const: 'ROLE',
            listFilter: [
                // {
                //     name: 'Division Administrator',
                //     isSelected: false,
                //     value: {
                //         localizedData: [
                //             {
                //                 id: 2,
                //                 languageCode: 'en-US',
                //                 fields: [
                //                     {
                //                         name: 'Name',
                //                         localizedText: 'Division Administrator'
                //                     },
                //                     {
                //                         name: 'Description',
                //                         localizedText: ''
                //                     }
                //                 ]
                //             }
                //         ],
                //         identity: {
                //             extId: 'divisionadmin',
                //             ownerId: 3001,
                //             customerId: 0,
                //             archetype: 'SystemRole',
                //             id: 87
                //         },
                //         entityStatus: {
                //             externallyMastered: false,
                //             lastUpdated: '0001-01-01T00:00:00',
                //             lastUpdatedBy: 0,
                //             statusId: 'Active',
                //             statusReasonId: 'Unknown',
                //             deleted: false
                //         }
                //     }
                // },
                // {
                //     name: 'Branch Admin',
                //     isSelected: false,
                //     value: {
                //         localizedData: [
                //             {
                //                 id: 2,
                //                 languageCode: 'en-US',
                //                 fields: [
                //                     {
                //                         name: 'Name',
                //                         localizedText: 'Branch Admin'
                //                     },
                //                     {
                //                         name: 'Description',
                //                         localizedText: 'Branch Administrator'
                //                     }
                //                 ]
                //             }
                //         ],
                //         identity: {
                //             extId: 'branchadmin',
                //             ownerId: 3001,
                //             customerId: 0,
                //             archetype: 'SystemRole',
                //             id: 88
                //         },
                //         entityStatus: {
                //             externallyMastered: false,
                //             lastUpdated: '0001-01-01T00:00:00',
                //             lastUpdatedBy: 0,
                //             statusId: 'Active',
                //             statusReasonId: 'Unknown',
                //             deleted: false
                //         }
                //     }
                // },
                // {
                //     name: 'School Admin',
                //     isSelected: false,
                //     value: {
                //         localizedData: [
                //             {
                //                 id: 2,
                //                 languageCode: 'en-US',
                //                 fields: [
                //                     {
                //                         name: 'Name',
                //                         localizedText: 'School Admin'
                //                     },
                //                     {
                //                         name: 'Description',
                //                         localizedText: 'School Administrator'
                //                     }
                //                 ]
                //             }
                //         ],
                //         identity: {
                //             extId: 'schooladmin',
                //             ownerId: 3001,
                //             customerId: 0,
                //             archetype: 'SystemRole',
                //             id: 89
                //         },
                //         entityStatus: {
                //             externallyMastered: false,
                //             lastUpdated: '0001-01-01T00:00:00',
                //             lastUpdatedBy: 0,
                //             statusId: 'Active',
                //             statusReasonId: 'Unknown',
                //             deleted: false
                //         }
                //     }
                // },
                // {
                //     name: 'DLC',
                //     isSelected: false,
                //     value: {
                //         localizedData: [
                //             {
                //                 id: 2,
                //                 languageCode: 'en-US',
                //                 fields: [
                //                     {
                //                         name: 'Name',
                //                         localizedText: 'DLC'
                //                     },
                //                     {
                //                         name: 'Description',
                //                         localizedText: 'Divisional Learning Coordinator'
                //                     }
                //                 ]
                //             }
                //         ],
                //         identity: {
                //             extId: 'divisiontrainingcoordinator',
                //             ownerId: 3001,
                //             customerId: 0,
                //             archetype: 'SystemRole',
                //             id: 90
                //         },
                //         entityStatus: {
                //             externallyMastered: false,
                //             lastUpdated: '0001-01-01T00:00:00',
                //             lastUpdatedBy: 0,
                //             statusId: 'Active',
                //             statusReasonId: 'Unknown',
                //             deleted: false
                //         }
                //     }
                // }
            ]
        },
        {
            name: 'Service Scheme',
            const: 'SERVICE_SCHEME',
            listFilter: [
                {
                    name: 'EO',
                    isSelected: false,
                    value: '101'
                },
                {
                    name: 'MK',
                    isSelected: false,
                    value: '102'
                },
                {
                    name: 'AED',
                    isSelected: false,
                    value: '103'
                },
                {
                    name: 'EAS',
                    isSelected: false,
                    value: '104'
                }
            ]
        },
        {
            name: 'Growth Model',
            const: 'GROWTH_MODEL',
            listFilter: [
                {
                    name: 'TGM',
                    isSelected: false,
                    value: '105'
                },
                {
                    name: 'LGM',
                    isSelected: false,
                    value: '106'
                },
                {
                    name: 'SGM',
                    isSelected: false,
                    value: '107'
                }
            ]
        }
    ]
};


export const MockData = {
    applied_filter: [
        {
            filterOptions: {
                data: 'STATUS',
                text: 'Account Status'
            },
            data: {
                value: [
                    'Active',
                    'New'
                ],
                text: 'Active, New'
            }
        }
    ]
};
