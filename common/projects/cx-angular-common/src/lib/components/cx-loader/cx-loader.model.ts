
export class CxLoaderUI {
    circleBackgroundColor?: string;
    overlayTopPositionInPx?: number;
    constructor(data?: Partial<CxLoaderUI>) {
        if (data) {
            this.circleBackgroundColor = data.circleBackgroundColor === undefined
                ? '' : data.circleBackgroundColor;
            this.overlayTopPositionInPx = data.overlayTopPositionInPx === undefined
                ? 0 : data.overlayTopPositionInPx;
            return;
        }

        this.circleBackgroundColor = '';
        this.overlayTopPositionInPx = 0;
    }
}

export class CxLoaderModuleConfig {
    loaderUi: CxLoaderUI;
}
