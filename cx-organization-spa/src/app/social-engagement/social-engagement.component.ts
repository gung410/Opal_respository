import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CxGlobalLoaderService } from '@conexus/cx-angular-common';
import { AuthService } from 'app-auth/auth.service';
import { AppConstant } from 'app/shared/app.constant';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';

@Component({
  selector: 'social-engagement',
  templateUrl: './social-engagement.component.html',
  styleUrls: ['./social-engagement.component.scss']
})
export class SocialEngagementComponent
  extends BaseScreenComponent
  implements OnInit {
  private cslUrl: string = `${AppConstant.moduleLink.CSL}/dashboard/social/setting?embed=true`;

  constructor(
    private sanitizer: DomSanitizer,
    private globalLoader: CxGlobalLoaderService,
    public authService: AuthService,
    changeDetectorRef: ChangeDetectorRef
  ) {
    super(changeDetectorRef, authService);
  }

  ngOnInit(): void {
    super.ngOnInit();
    this.globalLoader.showLoader();
  }

  get iframeUrl(): SafeResourceUrl {
    return this.sanitizer.bypassSecurityTrustResourceUrl(this.cslUrl);
  }

  onIframeLoad(): void {
    // Delay for render DOM then hide loader
    const delayTime = 500; // ms
    setTimeout(() => {
      this.globalLoader.hideLoader();
    }, delayTime);
  }
}
