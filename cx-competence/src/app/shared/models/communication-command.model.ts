export class NotificationCommand {
  type: string;
  created: string;
  routing: CommandRouting;
  payload: CommandPayload;
  constructor(data?: Partial<NotificationCommand>) {
    if (!data) {
      return;
    }
    this.type = data.type;
    this.created = data.created;
    this.routing = data.routing;
    this.payload = data.payload;
  }
}

export class CommandRouting {
  action: string;
  actionVersion: string;
  entity: string;
  entityId: string | number;
  constructor(data?: Partial<CommandRouting>) {
    if (!data) {
      return;
    }
    this.action = data.action;
    this.actionVersion = data.actionVersion;
    this.entity = data.entity;
    this.entityId = data.entityId;
  }
}

export class CommandPayload {
  identity: CommandIdentity;
  body: CommandBody;
  constructor(data?: Partial<CommandPayload>) {
    if (!data) {
      return;
    }
    this.identity = data.identity;
    this.body = data.body;
  }
}

export class CommandIdentity {
  clientId: string;
  customerId: string;
  userId: string;
  constructor(data?: Partial<CommandIdentity>) {
    if (!data) {
      return;
    }
    this.clientId = data.clientId;
    this.customerId = data.customerId;
    this.userId = data.userId;
  }
}

export class CommandBody {
  NotificationId: string | number;
  constructor(data?: Partial<CommandBody>) {
    if (!data) {
      return;
    }
    this.NotificationId = data.NotificationId;
  }
}
