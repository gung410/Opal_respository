import { AppInfoService, GlobalSpinnerService, Guid, ModalService } from '@opal20/infrastructure';
import { Component, EventEmitter, HostListener, Input, Output, SimpleChanges } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { IScormPlayerParameters, ScormPlayerMode } from '../../models/scorm-player.model';
@Component({
  selector: 'scorm-player',
  templateUrl: './scorm-player.component.html'
})
export class ScormPlayerComponent {
  @Input() public myLectureId: string;
  @Input() public digitalContentId: string;
  @Input() public myDigitalContentId: string;
  @Input() public displayMode: ScormPlayerMode = 'local';
  @Input() public fileLocation: string;
  @Input() public onScormFinishCallback: () => void;
  @Input() public isLectureNextButtonClicked: boolean = false;
  @Output() public onEnableFinishButtonEmitter: EventEmitter<boolean> = new EventEmitter<boolean>();

  public iframeId: string = Guid.create().toString();
  public safeUrl: SafeResourceUrl;
  private token: string;

  constructor(
    public domSanitizer: DomSanitizer,
    private appInfoService: AppInfoService,
    private globalSpinner: GlobalSpinnerService,
    private modalService: ModalService
  ) {
    const url = AppGlobal.environment.scormPlayerUrl;
    this.safeUrl = domSanitizer.bypassSecurityTrustResourceUrl(url);
    this.token = this.appInfoService.getAccessToken();
  }

  @HostListener('window:message', ['$event'])
  public onMessage(event: MessageEvent): void {
    try {
      const messageData = JSON.parse(event.data);
      if (messageData.key === 'ContentLoadedEvent') {
        this.hideSpinner();
      }
      if (messageData.key === 'FinishedToTheEnd') {
        this.enableFinishFunction();
      }
      if (messageData.key === 'ExitButtonPressed') {
        this.enableFinishFunction();
      }
      if (messageData.key === 'PackageInvalid') {
        this.modalService.showErrorMessage('The SCORM file is unavailable. Please contact the administration.');
      }
    } catch {
      return;
    }
  }

  public onLearningPackageLoaded(reinitializePlayer: boolean = false): void {
    this.globalSpinner.show();
    const scormElement = document.getElementById(this.iframeId) as HTMLIFrameElement;

    if (!scormElement) {
      return;
    }

    const initScormPlayerMessage: IScormPlayerParameters = {
      key: 'scorm-init',
      displayMode: this.displayMode,
      contentApiUrl: AppGlobal.environment.contentApiUrl,
      learnerApiUrl: AppGlobal.environment.learnerApiUrl,
      cloudFrontUrl: AppGlobal.environment.cloudfrontUrl,
      accessToken: this.token,
      digitalContentId: this.digitalContentId,
      myLectureId: this.myLectureId,
      myDigitalContentId: this.myDigitalContentId || undefined,
      reinitializePlayer: reinitializePlayer,
      fileLocation: this.fileLocation
    };

    scormElement.contentWindow.postMessage(JSON.stringify(initScormPlayerMessage), '*');
  }

  public enableFinishFunction(): void {
    if (this.onScormFinishCallback != null) {
      this.onScormFinishCallback();
    }
  }

  public ngOnChanges(changes: SimpleChanges): void {
    if (changes.isLectureNextButtonClicked && (changes.isLectureNextButtonClicked.currentValue as Boolean)) {
      const iWindow = (document.getElementById(this.iframeId) as HTMLIFrameElement).contentWindow;
      const iContentWindow = (document.getElementById('contentIFrame') as HTMLIFrameElement).contentWindow;
      const lectureNextButtonClickedEvent: IScormPlayerParameters = {
        key: 'lecture-next-button-clicked'
      };
      iWindow.postMessage(JSON.stringify(lectureNextButtonClickedEvent), '*');
      iContentWindow.postMessage(JSON.stringify(lectureNextButtonClickedEvent), '*');
      return;
    }

    // Check more: when user change an input (myLectureId) => we must be force re-init data for ScormPlayer via onLearningPackageLoaded function
    if (
      changes.myLectureId &&
      changes.myLectureId.firstChange === false &&
      changes.myLectureId.currentValue !== changes.myLectureId.previousValue
    ) {
      const reinitializePlayer: boolean = true;
      this.onLearningPackageLoaded(reinitializePlayer);
    }
  }

  private hideSpinner(): void {
    this.globalSpinner.hide(true);
  }
}
