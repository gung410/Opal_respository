import { BaseComponent, DomUtils, ModuleFacadeService, NotificationType, Utils } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ECertificateModel, MetadataTagModel } from '@opal20/domain-api';

import { CourseDataService } from '../services/course-data.service';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { DomSanitizer } from '@angular/platform-browser';
import { MyAchievementService } from '../services/my-achievement.service';
import { MyAchievementsDialogComponent } from '@opal20/domain-components';
import { Observable } from 'rxjs';
import { OpalDialogService } from '@opal20/common-components';
import { map } from 'rxjs/operators';

const itemsPerPage: number = 5;
@Component({
  selector: 'learning-ecertificates-list',
  templateUrl: './learning-ecertificates-list.component.html'
})
export class LearningECertificatesListComponent extends BaseComponent implements OnInit {
  @ViewChild('listSection', { static: false })
  public listSectionElement: ElementRef;

  @Input() public numberOfItemsOnPage: number = itemsPerPage;
  @Input() public isShowMore: boolean = false;

  @Output() public onGetECertificatesCallBack = new EventEmitter<number>();
  @Output() public onShowMoreECertificatesClick: EventEmitter<void> = new EventEmitter<void>();

  public totalCount: number = 0;
  public activePageNumber: number = 0;
  public totalPages: number = 0;
  public pages: Array<number | undefined> = [1];
  public scrollableParent: HTMLElement;
  public currentActiveSectionNumber: number = 1;
  public eCertificateItems: ECertificateModel[] = [];
  public metadata: Observable<Dictionary<MetadataTagModel>>;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private myAchievementService: MyAchievementService,
    private elementRef: ElementRef,
    private courseDataService: CourseDataService,
    private opalDialogService: OpalDialogService,
    private sanitizer: DomSanitizer
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
      this.numberOfItemsOnPage = 3;
    }

    this.activePageNumber = pageNumber;
    this.getECertificatesCallBack(this.numberOfItemsOnPage * (pageNumber - 1), this.numberOfItemsOnPage).then(() => {
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

  public getECertificatesCallBack(skipCount: number = 0, maxResultCount: number = 3): Promise<void> {
    return this.myAchievementService.getMyECertificates(skipCount, maxResultCount).then(result => {
      this.metadata = this.courseDataService.getCourseMetadata().pipe(map(tags => Utils.toDictionary(tags, p => p.id)));

      this.eCertificateItems = result.items;
      this.totalCount = result.totalCount;
      this.onGetECertificatesCallBack.emit(result.totalCount);
    });
  }

  public onECertificateClick(item: ECertificateModel): void {
    this.myAchievementService.getECertificateFromImg(item.id).subscribe(res => {
      const blobURL = window.URL.createObjectURL(res.body);
      const safeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(blobURL);

      const dialogRef: DialogRef = this.opalDialogService.openDialogRef(MyAchievementsDialogComponent, {
        safeUrl: safeUrl,
        onDownloadFn: () => {
          this.onDownloadECertificateClick(item);
        }
      });

      dialogRef.result.subscribe(() => {
        dialogRef.close();
      });
    });
  }

  public onDownloadECertificateClick(item: ECertificateModel): void | string {
    this.myAchievementService.downloadECertificate(item.id).subscribe(res => {
      saveAs(res.body, res.headers.get('Download-File-Name'));
      this.showNotification('The file is being downloaded.', NotificationType.Info);
    });
  }

  public onShowMoreClicked(): void {
    this.onShowMoreECertificatesClick.emit();
  }

  public getTagName(metadata: Dictionary<MetadataTagModel>, tagId: string): string {
    return metadata[tagId] && metadata[tagId].displayText;
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
