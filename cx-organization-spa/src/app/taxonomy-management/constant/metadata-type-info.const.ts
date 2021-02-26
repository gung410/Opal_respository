import { IMetadataTypeNode } from '../models/metadata-type-node.model';

export const EO_METADATA_TYPE: IMetadataTypeNode[] = [
  {
    nodeId: 1,
    metadataTypeId: 'ddbeb6aa-b42b-11e9-b923-0242ac120004',
    code: 'COURSES-OF-STUDY',
    name: 'Teaching Course of Study'
  },
  {
    nodeId: 2,
    metadataTypeId: '1822d1fa-b42c-11e9-b3e6-0242ac120004',
    code: 'LEARNING-FXS',
    name: 'Learning Frameworks',
    children: [
      {
        nodeId: 3,
        metadataTypeId: '953bbd08-9e2b-11e9-98de-0242ac120003',
        code: 'DEVROLES',
        parentNodeId: 2,
        name: 'Developmental roles'
      }
    ]
  },
  {
    nodeId: 4,
    metadataTypeId: 'b44edc3c-b42b-11e9-9a3d-0242ac120004',
    code: 'TRACKS',
    name: 'Tracks'
  },
  {
    nodeId: 5,
    metadataTypeId: '720f99c8-b427-11e9-9399-0242ac120004',
    code: 'RSPS',
    name: 'Role-Specific Proficiencies'
  },
  {
    nodeId: 6,
    metadataTypeId: '6dc2268e-2c67-11ea-8dea-0242ac120003',
    code: 'TEACHING-LEVELS',
    name: 'Teaching Levels',
    children: [
      {
        nodeId: 7,
        metadataTypeId: '08c08108-b42c-11e9-a613-0242ac120004',
        parentNodeId: 6,
        code: 'CCAS',
        name: 'Co-curricular Activities'
      },
      {
        nodeId: 8,
        metadataTypeId: 'c06cc0ce-b42b-11e9-b1db-0242ac120004',
        parentNodeId: 6,
        code: 'TEACHING-SUBJECTS',
        name: 'Teaching Subjects'
      },
      {
        nodeId: 9,
        metadataTypeId: '3ccc65ac-db91-11e9-84d4-0242ac120004',
        parentNodeId: 6,
        code: 'PORTFOLIOS',
        name: 'Portfolios'
      }
    ]
  },
  {
    nodeId: 10,
    metadataTypeId: '18a10f86-1032-11ea-8c6a-0242ac120003',
    code: 'DESIGNATION',
    name: 'Designation',
    children: [
      {
        nodeId: 11,
        metadataTypeId: '3ccc65ac-db91-11e9-84d4-0242ac120004',
        parentNodeId: 10,
        code: 'PORTFOLIOS',
        name: 'Portfolios'
      }
    ]
  },
  {
    nodeId: 12,
    metadataTypeId: '',
    code: '',
    name: 'Areas of Professional Interest'
  }
];

export const AED_METADATA_TYPE: IMetadataTypeNode[] = [
  {
    nodeId: 1,
    metadataTypeId: 'ddbeb6aa-b42b-11e9-b923-0242ac120004',
    code: 'COURSES-OF-STUDY',
    name: 'Teaching Course of Study'
  },
  {
    nodeId: 2,
    metadataTypeId: '1822d1fa-b42c-11e9-b3e6-0242ac120004',
    code: 'LEARNING-FXS',
    name: 'Learning Frameworks',
    children: [
      {
        nodeId: 3,
        metadataTypeId: '953bbd08-9e2b-11e9-98de-0242ac120003',
        code: 'DEVROLES',
        parentNodeId: 2,
        name: 'Developmental roles'
      }
    ]
  },
  {
    nodeId: 4,
    metadataTypeId: 'b44edc3c-b42b-11e9-9a3d-0242ac120004',
    code: 'TRACKS',
    name: 'Tracks'
  },
  {
    nodeId: 5,
    metadataTypeId: '6dc2268e-2c67-11ea-8dea-0242ac120003',
    code: 'TEACHING-LEVELS',
    name: 'Teaching Levels',
    children: [
      {
        nodeId: 6,
        metadataTypeId: 'c06cc0ce-b42b-11e9-b1db-0242ac120004',
        parentNodeId: 5,
        code: 'TEACHING-SUBJECTS',
        name: 'Teaching Subjects'
      }
    ]
  },
  {
    nodeId: 7,
    metadataTypeId: '18a10f86-1032-11ea-8c6a-0242ac120003',
    code: 'DESIGNATION',
    name: 'Designation'
  },
  {
    nodeId: 8,
    metadataTypeId: '',
    code: '',
    name: 'Areas of Professional Interest'
  }
];

export const MKE_METADATA_TYPE: IMetadataTypeNode[] = [
  {
    nodeId: 1,
    metadataTypeId: '1822d1fa-b42c-11e9-b3e6-0242ac120004',
    code: 'LEARNING-FXS',
    name: 'Learning Frameworks',
    children: [
      {
        nodeId: 2,
        metadataTypeId: '953bbd08-9e2b-11e9-98de-0242ac120003',
        code: 'DEVROLES',
        parentNodeId: 1,
        name: 'Developmental roles'
      }
    ]
  },
  {
    nodeId: 3,
    metadataTypeId: 'b44edc3c-b42b-11e9-9a3d-0242ac120004',
    code: 'TRACKS',
    name: 'Tracks'
  },
  {
    nodeId: 4,
    metadataTypeId: '6dc2268e-2c67-11ea-8dea-0242ac120003',
    code: 'TEACHING-LEVELS',
    name: 'Teaching Levels'
  },
  {
    nodeId: 5,
    metadataTypeId: '18a10f86-1032-11ea-8c6a-0242ac120003',
    code: 'DESIGNATION',
    name: 'Designation'
  },
  {
    nodeId: 6,
    metadataTypeId: '',
    code: '',
    name: 'Areas of Professional Interest'
  }
];

export const EAS_METADATA_TYPE: IMetadataTypeNode[] = [
  {
    nodeId: 1,
    metadataTypeId: '1822d1fa-b42c-11e9-b3e6-0242ac120004',
    code: 'LEARNING-FXS',
    name: 'Learning Frameworks',
    children: [
      {
        nodeId: 2,
        metadataTypeId: '953bbd08-9e2b-11e9-98de-0242ac120003',
        code: 'DEVROLES',
        parentNodeId: 1,
        name: 'Developmental roles'
      }
    ]
  },
  {
    nodeId: 3,
    metadataTypeId: 'c67f2c18-b42b-11e9-9148-0242ac120004',
    code: 'JOB-FAMILIES',
    name: 'Job Families'
  },
  {
    nodeId: 4,
    metadataTypeId: '18a10f86-1032-11ea-8c6a-0242ac120003',
    code: 'DESIGNATION',
    name: 'Designation'
  },
  {
    nodeId: 5,
    metadataTypeId: '',
    code: '',
    name: 'Areas of Professional Interest'
  }
];
