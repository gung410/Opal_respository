import { BaseComponent, DomUtils, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { DigitalBadgeModel } from '@opal20/domain-api';
import { MyAchievementService } from '../services/my-achievement.service';
import { MyAchievementsDialogComponent } from '@opal20/domain-components';
import { OpalDialogService } from '@opal20/common-components';

const itemsPerPage: number = 12;
@Component({
  selector: 'learning-digital-badges-list',
  templateUrl: './learning-digital-badges-list.component.html'
})
export class LearningDigitalBadgesListComponent extends BaseComponent implements OnInit {
  @ViewChild('scrollElement', { static: false })
  public scrollElement: ElementRef;
  @ViewChild('listSection', { static: false })
  public listSectionElement: ElementRef;

  @Input() public numberOfItemsOnPage: number = itemsPerPage;
  @Input() public isShowMore: boolean = false;

  @Output() public onGetDigitalBadgesCallBack = new EventEmitter<number>();
  @Output() public onShowMoreDigitalBadgesClick: EventEmitter<void> = new EventEmitter<void>();

  public totalCount: number = 0;
  public activePageNumber: number = 0;
  public totalPages: number = 0;
  public pages: Array<number | undefined> = [1];
  public scrollableParent: HTMLElement;
  public currentActiveSectionNumber: number = 1;
  public digitalBadgeItems: DigitalBadgeModel[] = [];

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private myAchievementService: MyAchievementService,
    private elementRef: ElementRef,
    private opalDialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
  }

  public ngOnInit(): void {
    super.ngOnInit();
    this.loadPageItems(1);
  }

  public onAfterViewInit(): void {
    this.scrollableParent = DomUtils.findClosestVerticalScrollableParent(this.elementRef.nativeElement);
    if (this.scrollableParent === undefined) {
      return;
    }
  }

  public onScroll(): void {
    if (this.scrollableParent.scrollTop === 0) {
      this.currentActiveSectionNumber = 1;
      return;
    }

    const currentParentScrollPosition = this.scrollableParent.scrollTop;
    const sections = [this.listSectionElement];
    let currentActiveSection: number = 0;
    sections.forEach((p, i) => {
      if (p !== undefined && p.nativeElement.offsetTop - 350 <= currentParentScrollPosition) {
        currentActiveSection = i + 1;
      }
    });
    this.currentActiveSectionNumber = currentActiveSection;
  }

  public scrollTo(el: HTMLElement, sectionNumber: number): void {
    if (el === undefined || this.scrollableParent === undefined) {
      return;
    }
    this.scrollableParent.scrollTop = el.offsetTop - 300;
    setTimeout(() => (this.currentActiveSectionNumber = sectionNumber), 55);
  }

  public generatePagesArray(totalPages: number): number[] {
    const result: number[] = [];
    for (let i = 1; i <= totalPages; i++) {
      result.push(i);
    }
    return result;
  }

  public loadPageItems(pageNumber: number | undefined, forceLoad: boolean = false): void {
    if (!forceLoad && (pageNumber === undefined || pageNumber === this.activePageNumber)) {
      return;
    }

    // At My Achievement Page, isShowMore = true
    if (this.isShowMore) {
      this.numberOfItemsOnPage = 6;
    }

    this.activePageNumber = pageNumber;
    this.getDigitalBadgesCallBack(this.numberOfItemsOnPage * (pageNumber - 1), this.numberOfItemsOnPage).then(() => {
      this.totalPages = this.calculateTotalPages(this.totalCount);
      this.pages = this.generatePageNumberArray(this.totalPages);
      this.currentActiveSectionNumber = 1;
      if (this.scrollableParent !== undefined) {
        this.scrollableParent.scrollTop = 0;
      }
    });
  }

  public loadPreviousPage(): void {
    this.loadPageItems(this.activePageNumber - 1);
  }

  public loadNextPage(): void {
    this.loadPageItems(this.activePageNumber + 1);
  }

  public getDigitalBadgesCallBack(skipCount: number = 0, maxResultCount: number = 3): Promise<void> {
    return this.myAchievementService.getMyDigitalBadges(skipCount, maxResultCount).then(result => {
      this.digitalBadgeItems = result.items;
      this.totalCount = result.totalCount;
      this.onGetDigitalBadgesCallBack.emit(result.totalCount);
    });
  }

  public onDigitalBadgeClick(): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(MyAchievementsDialogComponent, {
      onDownloadFn: () => {
        this.onDownloadDigitalBadge();
      }
    });

    dialogRef.result.subscribe(() => {
      dialogRef.close();
    });
  }

  public onDownloadDigitalBadge(): void {
    alert('Not Implement!!!');
    const msg = this.moduleFacadeService.translator.translateCommon('Download successfully');
    this.showNotification(msg);
    //   this.uploaderService.getFile(this.fileLocation).then(url => {
    //     const isIOSDevice = !!navigator.platform && /iPad|iPhone|iPod/.test(navigator.platform);
    //     if (isIOSDevice) {
    //       fetch(url).then(res => {
    //         res.blob().then(blob => {
    //           this.showNotification('The file is being downloaded.', NotificationType.Info);
    //           Utils.downloadFileByFileReader(blob, `${this.digitalContent.fileName}`);
    //         });
    //       });
    //       return;
    //     }
    //     if (this.isImageFile(this.digitalContent.fileExtension)) {
    //       // some image files like svg can't download by saveAs so using fetch to get file
    //       // but this method get all data before create file download therefore with
    //       // large file user can't see the progress of downloading before it has completed.
    //       // Using this method only for small file such as images.
    //       fetch(url).then(res => {
    //         res.blob().then(blob => {
    //           this.showNotification('The file is being downloaded.', NotificationType.Info);
    //           Utils.downloadFile(blob, `${this.digitalContent.fileName}`);
    //         });
    //       });
    //       return;
    //     }
    //     saveAs(url, this.digitalContent.fileName);
    //     this.showNotification('The file is being downloaded.', NotificationType.Info);
    //   });
  }

  public onShowMoreClicked(): void {
    this.onShowMoreDigitalBadgesClick.emit();
  }

  public scrollLeft(): void {
    if (this.scrollElement === undefined || this.scrollElement.nativeElement === undefined) {
      return;
    }
    const nativeElement = <HTMLElement>this.scrollElement.nativeElement;
    this.smoothScroll(-340);
  }

  public scrollRight(): void {
    if (this.scrollElement === undefined || this.scrollElement.nativeElement === undefined) {
      return;
    }
    this.smoothScroll(340);
  }

  private smoothScroll(totalScroll: number): void {
    const step = totalScroll / 10;
    let i: number = 0;
    const initalPosition = this.scrollElement.nativeElement.scrollLeft;
    const interval = setInterval(() => {
      this.scrollElement.nativeElement.scrollLeft = initalPosition + (i + 1) * step;
      i++;
      if (i * step === totalScroll) {
        clearInterval(interval);
      }
    }, 10);
  }

  private generatePageNumberArray(totalPages: number): Array<number | undefined> {
    const result: Array<number | undefined> = [1];
    if (totalPages === 1) {
      return result;
    }

    if (this.activePageNumber >= 5) {
      result.push(undefined);
    }

    for (let i = this.activePageNumber - 2; i <= this.activePageNumber; i++) {
      if (i > 1) {
        result.push(i);
      }
    }

    if (totalPages - 1 > this.activePageNumber + 2) {
      result.push(this.activePageNumber + 1);
      result.push(this.activePageNumber + 2);
      result.push(undefined);
      result.push(totalPages);
    } else {
      for (let i = this.activePageNumber + 1; i <= totalPages; i++) {
        result.push(i);
      }
    }

    return result;
  }

  private calculateTotalPages(total: number): number {
    const minPages = Math.floor(total / this.numberOfItemsOnPage);
    const spareItems = total - this.numberOfItemsOnPage * minPages;
    return spareItems > 0 ? minPages + 1 : minPages;
  }
}
