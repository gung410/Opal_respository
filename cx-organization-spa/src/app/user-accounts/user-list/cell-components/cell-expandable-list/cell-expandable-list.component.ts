import { Component, ElementRef, ViewChild } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { SurveyUtils } from '../../../../shared/utilities/survey-utils';

@Component({
  selector: 'cell-expandable-list',
  templateUrl: './cell-expandable-list.component.html',
  styleUrls: ['./cell-expandable-list.component.scss']
})
export class CellExpandableListComponent implements ICellRendererAngularComp {
  @ViewChild('itemExpand', { read: ElementRef }) itemExpand: ElementRef;
  items: any[];
  description: string;
  private params: any;
  private defaultHeight: number = 100;
  private defaultPadding: number = 10;

  agInit(params: any): void {
    this.params = params;
    this.getDataToShowUp();
  }

  refresh(params: any): boolean {
    this.params = params;
    this.getDataToShowUp();

    return true;
  }

  getDataToShowUp(): void {
    if (this.params.value) {
      this.items = this.mappingDataFromLocalize(this.params.value);
      this.description = this.mappingDataFromLocalize(this.params.value, true);
    }
  }

  mappingDataFromLocalize(arrData: any[], isJoin: boolean = false): any {
    if (isJoin) {
      return arrData
        ? SurveyUtils.mapArrayLocalizedToArrayObject(
            arrData,
            localStorage.getItem('language-code')
          ).join(', ')
        : [];
    }

    return arrData
      ? SurveyUtils.mapArrayLocalizedToArrayObject(
          arrData,
          localStorage.getItem('language-code')
        )
      : [];
  }
}
