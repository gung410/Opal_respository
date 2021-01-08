import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { ContentApiService, DigitalContent, DigitalContentType } from '@opal20/domain-api';
import { DiffHelper, IDiffResult } from '@opal20/domain-components';

@Component({
  selector: 'digital-content-comparison',
  templateUrl: './digital-content-comparison.component.html'
})
export class DigitalContentComparisonComponent extends BaseComponent {
  @Input() public oldVersionTrackingId: string;
  @Input() public newVersionTrackingId: string;
  public oldDigitalContent: DigitalContent;
  public newDigitalContent: DigitalContent;

  public diffHtmlResult: IDiffResult = undefined;
  public isCompleted: boolean;
  public noDataToCompare: boolean = false;

  constructor(moduleFacadeService: ModuleFacadeService, public contentApiService: ContentApiService) {
    super(moduleFacadeService);
  }

  public ngOnInit(): void {
    this.getDigitalContentAndPocessCompare();
  }

  public trimEmptyHtml(html: string): string {
    const aux = document.createElement('div');
    aux.innerHTML = html; // parses the html
    for (let index = 0; index < aux.childNodes.length; index++) {
      const node = aux.childNodes[index];
      if (node && node.nodeName === 'IMG') {
        return html;
      }
      if (node && node.nodeName === 'IFRAME') {
        return html;
      }
    }
    if (aux.innerText.length > 1) {
      return html;
    }
    return '';
  }

  private getDigitalContentAndPocessCompare(): void {
    const getOldVersionTrackingData = this.contentApiService.getDigitalContentByVersionTrackingId(this.oldVersionTrackingId).then(data => {
      this.oldDigitalContent = data;
    });

    const getNewVersionTrackingData = this.contentApiService.getDigitalContentByVersionTrackingId(this.newVersionTrackingId).then(data => {
      this.newDigitalContent = data;
    });

    Promise.all([getOldVersionTrackingData, getNewVersionTrackingData]).then(() => {
      this.processCompare();
      this.isCompleted = true;
    });
  }

  private processCompare(): void {
    if (this.oldDigitalContent.type === DigitalContentType.LearningContent) {
      if (!this.oldDigitalContent.htmlContent && !this.newDigitalContent.htmlContent) {
        this.isCompleted = true;
        this.noDataToCompare = true;
        return;
      }

      this.compareHtml();
    }
  }

  private compareHtml(): void {
    const diffMatchPatch = new DiffHelper();
    this.diffHtmlResult = diffMatchPatch.diffTwoHtml(this.oldDigitalContent.htmlContent, this.newDigitalContent.htmlContent);
  }
}
