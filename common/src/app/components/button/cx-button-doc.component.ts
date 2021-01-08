import { Component, OnInit } from '@angular/core';
import { CxGlobalLoaderService } from 'projects/cx-angular-common/src/lib/components/cx-loader/cx-global-loader.service';
@Component({
    selector: 'cx-button-doc',
    templateUrl: './cx-button-doc.component.html',
    styleUrls: ['./cx-button-doc.component.scss']
})

export class CxButtonDocComponent implements OnInit {
    constructor(private globalLoaderService: CxGlobalLoaderService) { }

    ngOnInit() {
    }
    onSubmitted() {

    }
    onCollapsibleSubmitted() {

    }

    onBtnClicked() {
        this.globalLoaderService.showLoader();
        setTimeout(() => {
            this.globalLoaderService.hideLoader();
        }, 1000);
    }
}
