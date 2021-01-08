export interface ICreateActionItemResultRequest {
  objectiveInfo: {
    identity: {
      extId: string; // extId of learner or you can put learner id instead
      archetype: string; // Employee
    };
  };
  answer: {
    learningOpportunity: {
      uri: string; // pattern: 'urn:opal2.moe.sg:coursepad-pdo:${courseId}'
      source: LearningOpportunitySourceType; // fixed string 'coursepad-pdo'
    };
    classRunId?: string; // Current class run unavailable, so we don't input this field
    addedToIDPDate: string; // Current date in ISO string format
  };
  forceCreateResult: boolean; // fixed true
  additionalProperties: {
    courseId?: string;
    type: string; // fixed 'self-registered'
    learningOpportunityUri: string; // same as answer.learningOpportunity.uri
  };
  timestamp: string; // Current date in ISO string format
  resultIdentity?: {
    extId?: string;
  };
}

export enum LearningOpportunitySourceType {
  CustomPdo = 'custom-pdo',
  CoursepadPdo = 'coursepad-pdo'
}
