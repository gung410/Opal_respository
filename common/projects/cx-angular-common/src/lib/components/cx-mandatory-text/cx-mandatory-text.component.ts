import { Component, OnInit, Input } from '@angular/core';
import { CxCommonService } from '../../cx-angular-common.service';

@Component({
  selector: 'cx-mandatory-text',
  templateUrl: './cx-mandatory-text.component.html',
  styleUrls: ['./cx-mandatory-text.component.scss']
})
export class CxMandatoryTextComponent implements OnInit {
  textMandatoryNeedToShow: string;
  constructor(public cxCommonService: CxCommonService) { }
  ngOnInit() {
    this.textMandatoryNeedToShow = this.cxCommonService.textMandatoryNeedToShow;
  }

}
