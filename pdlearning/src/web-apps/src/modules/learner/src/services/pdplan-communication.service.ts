import { PdPlanIframeAction, PdPlanIframeMessage, PdPlanTokenPayload } from '../models/pdplan-communication.model';

import { Injectable } from '@angular/core';

@Injectable()
export class PdPlanCommunicationService {
  public iframeWindow: Window;

  public getIframeMessage(event: MessageEvent): PdPlanIframeMessage {
    const pdplanIframeMessage = event.data;
    if (!pdplanIframeMessage) {
      return;
    }

    return this.processIframeMessage(event.data);
  }

  public sendAccessToken(accessToken: string): void {
    if (!this.iframeWindow || !accessToken) {
      return;
    }
    const messageData = { action: PdPlanIframeAction.SET_ACCESS_TOKEN, payload: new PdPlanTokenPayload(accessToken) };
    const message = JSON.stringify(messageData);
    this.iframeWindow.postMessage(message, '*');
  }

  private processIframeMessage(messageData: string | object): PdPlanIframeMessage {
    let message: PdPlanIframeMessage;

    switch (typeof messageData) {
      case 'string':
        try {
          message = JSON.parse(messageData) as PdPlanIframeMessage;
        } catch (e) {
          return;
        }
        break;
      case 'object':
        message = messageData as PdPlanIframeMessage;
        break;
    }

    if (!message || !message.action) {
      return;
    }

    return message;
  }
}
