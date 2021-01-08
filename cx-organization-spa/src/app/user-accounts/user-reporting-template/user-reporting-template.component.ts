import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { ToastrAdapterService } from 'app-services/toastr-adapter.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { ReportsDataService } from 'app/reports/reports-data.service';
import { AppConstant } from 'app/shared/app.constant';

@Component({
  template: '<p></p>'
})
export class UserReportingTemplateComponent implements OnInit {
  constructor(
    private activatedRoute: ActivatedRoute,
    private toastr: ToastrAdapterService,
    private translateAdapterService: TranslateAdapterService,
    private reportsDataService: ReportsDataService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.activatedRoute.queryParamMap.subscribe((queryParams: ParamMap) => {
      const filePath = queryParams.get('filepath');

      if (filePath) {
        this.reportsDataService
          .checkFileExists(filePath)
          .subscribe((fileExists) => {
            if (fileExists) {
              this.router.navigate([AppConstant.siteURL.menus.userAccounts]);
              this.reportsDataService.downloadFile(filePath);
            } else {
              console.error(`Not found file path '${filePath}'`);
              this.toastr.error(
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
