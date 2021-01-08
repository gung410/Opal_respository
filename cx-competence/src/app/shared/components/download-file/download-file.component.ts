import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { AuthService } from 'app-auth/auth.service';
import { ReportsDataService } from 'app-services/reports-data.services';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'download-file',
  template: '<p></p>',
})
export class DownloadFileComponent implements OnInit {
  private authService: AuthService;
  constructor(
    private activatedRoute: ActivatedRoute,
    private toastrService: ToastrService,
    authService: AuthService,
    private reportsDataService: ReportsDataService,
    private translateAdapterService: TranslateAdapterService
  ) {
    this.authService = authService;
  }

  ngOnInit(): void {
    const userInfo = this.authService.userData().getValue();

    this.activatedRoute.queryParamMap.subscribe((queryParams: ParamMap) => {
      const filePath = queryParams.get('filepath');

      if (filePath) {
        this.reportsDataService
          .checkFileExists(filePath)
          .subscribe((fileExists) => {
            if (fileExists) {
              this.reportsDataService.downloadFile(filePath);
            } else {
              console.error(`Not found file path '${filePath}'`);
              this.toastrService.error(
                this.translateAdapterService.getValueImmediately(
                  'RequestErrorMessage.FileNotFound'
                )
              );
            }
          });
      }
    });
  }
}
