export class PdPlanIframeMessage {
  public action: PdPlanIframeAction;

  constructor(data?: Partial<PdPlanIframeMessage>) {
    if (!data) {
      return;
    }
    this.action = data.action;
  }
}

export class PdPlanTokenPayload {
  public accessToken: string;
  constructor(accessToken: string) {
    this.accessToken = accessToken;
  }
}

export enum PdPlanIframeAction {
  WEBVIEW_READY = 'WEBVIEW_READY',
  SET_ACCESS_TOKEN = 'SET_ACCESS_TOKEN',
  CLICKED_OPEN_PDCATALOGUE = 'CLICKED_OPEN_PDCATALOGUE'
}
