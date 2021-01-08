import { Component, ChangeDetectionStrategy } from '@angular/core';
import { CxGlobalLoaderService } from 'projects/cx-angular-common/src';

@Component({
  selector: 'cxlib-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent {
  title = 'Conexus Angular Common';
  constructor(loader: CxGlobalLoaderService) {
    loader.showLoader();
    setTimeout(() => {
      loader.hideLoader();
    }, 1000);
  }
}
