export class RegisterNotification {
  userId: string;
  clientId: string;
  instanceIdToken?: string;
  deviceId?: string;
  platform?: string;
}

// Check model definition at https://cxtech.atlassian.net/wiki/spaces/CP/pages/572194896/Datahub+Event
export class CleanUpRegistrationPayload {
  type?: string;
  version?: string;
  id?: string;
  created?: Date;
  routing: {
    action: string; //  <short application name>.<event type>.<action name>.<object>.<status>
    actionVersion: string; // 1.0
    entity: any; // User <short application name>.<scope/schema>.<entity>
    entityId: string; // external id of user
  };
  payload: {
    identity: {
      clientId: string;
      customerId: string;
      userId: string;
    };
    references: {
      correlationId: string;
    };
    body: {
      userId: string;
      registrationToken: string;
    };
  };

  constructor() {
    this.created = new Date();
  }
}
