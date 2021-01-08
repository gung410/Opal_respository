export enum PDPlanActionEnum {
  WEBVIEW_READY = 'WEBVIEW_READY',
  SET_ACCESS_TOKEN = 'SET_ACCESS_TOKEN',
  VIEW_SIZE_CHANGE = 'VIEW_SIZE_CHANGE',
  CLICKED_OPEN_PDCATALOGUE = 'CLICKED_OPEN_PDCATALOGUE',
}

export class WebViewMessage {
  action: PDPlanActionEnum;
  payload: any;

  constructor(message?: Partial<WebViewMessage>) {
    if (!message) {
      return;
    }
    this.action = message.action;
    this.payload = message.payload;
  }
}

export class PDPlannerSetTokenPayload {
  accessToken: string;
  constructor(accessToken: string) {
    this.accessToken = accessToken;
  }
}

export class PDPlannerViewSizeChangePayload {
  width: number;
  height: number;
  constructor(width: number, height: number) {
    this.width = width;
    this.height = height;
  }
}
