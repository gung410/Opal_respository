import {
  Component,
  OnInit,
  Input,
  Output,
  EventEmitter,
  ViewEncapsulation,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  ElementRef
} from "@angular/core";
import { BaseComponent } from "../../abstracts/base.component";
import { MediaObserver } from "@angular/flex-layout";
import { CxPagingIcon, CxPagingText } from "./cx-paging.model";

@Component({
  selector: "cx-paging",
  templateUrl: "./cx-paging.component.html",
  styleUrls: ["./cx-paging.component.scss"],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CxPagingComponent extends BaseComponent {
  pages: number[] = [];
  isCurrentLastPage: boolean = false;
  isCurrentFirstPage: boolean = false;
  isShowFirstOrLastNavigation: boolean = true;
  private _totalRecords: number;

  @Input() get totalRecords(): number {
    return this._totalRecords;
  }
  set totalRecords(val: number) {
    this._totalRecords = val;
    if (!this.canDetectChanges) {
      return;
    }
    this.detectChanges();
  }

  private _recordsPerPage: number;
  @Input() get recordsPerPage(): number {
    return this._recordsPerPage;
  }
  set recordsPerPage(val: number) {
    this._recordsPerPage = val;
    if (!this.canDetectChanges) {
      return;
    }
    this.detectChanges();
  }
  @Input() maxPages: number = 5;
  @Input() currentPage: number;
  @Input() currentPageSize: number;
  @Input() pageSizes: number[] = [10, 25, 50, 75, 100, 125, 150, 175, 200];
  @Input() icon: CxPagingIcon = new CxPagingIcon();
  @Input() text: CxPagingText = new CxPagingText();
  @Output() currentPageChange = new EventEmitter<number>();
  @Output() pageSizeChange = new EventEmitter<number>();

  public get pagesArray(): number[] {
    if (!this.recordsPerPage || !this.totalRecords) {
      return [];
    }
    const numberOfPages = Math.ceil(this.totalRecords / this.recordsPerPage);
    const pages: number[] = [];
    for (let page = 1; page <= numberOfPages; page++) {
      pages.push(page);
    }
    return pages;
  }
  constructor(
    public changeDetectorRef: ChangeDetectorRef,
    public elementRef: ElementRef,
    public media: MediaObserver
  ) {
    super(changeDetectorRef, elementRef, media);
  }

  ngOnInit() {
    super.ngOnInit();
    this.recordsPerPage = this.currentPageSize ? this.currentPageSize : this.pageSizes[0];
    this.pages = this.pagesArray.slice(0, this.maxPages);
    if (this.currentPage === this.pagesArray[0]) {
      this.isCurrentFirstPage = true;
    }
    if (this.currentPage === this.pagesArray[this.pagesArray.length - 1]) {
      this.isCurrentLastPage = true;
    }
  }

  ngOnChanges() {
    this.updatePageList(this.currentPage);
  }

  onPageClicked(page: number) {
    if(this.pagesArray.length > 1)
      this.currentPageChange.emit(page);
  }

  onNavigatePreviousClicked() {
    const navigateToPage = this.currentPage - 1;
    if (this.currentPage !== this.pagesArray[0]) {
      this.updatePageList(navigateToPage);
      if (this.pages.some(page => page === navigateToPage)) {
        this.currentPageChange.emit(navigateToPage);
      }
    }
  }

  onNavigateNextClicked() {
    const navigateToPage = this.currentPage + 1;
    if (this.currentPage !== this.pagesArray[this.pagesArray.length - 1]) {
      this.updatePageList(navigateToPage);
      if (this.pages.some(page => page === navigateToPage)) {
        this.currentPageChange.emit(navigateToPage);
      }
    }
  }

  onNavigateFirstClicked() {
    if (!this.isCurrentFirstPage) {
      const navigateToPage = this.pagesArray[0];
      this.updatePageList(navigateToPage);
      this.currentPageChange.emit(navigateToPage);
    }
  }

  onNavigateLastClicked() {
    if (!this.isCurrentLastPage) {
      const navigateToPage = this.pagesArray[this.pagesArray.length - 1];
      this.updatePageList(navigateToPage);
      this.currentPageChange.emit(navigateToPage);
    }
  }

  onChangePageSize(pageSize: number) {
    this.recordsPerPage = pageSize;
    this.pageSizeChange.emit(this.recordsPerPage);
  }

  updatePageList(currentPage: number) {
    const pages = this.pagesArray;
    this.isShowFirstOrLastNavigation = pages.length > 1;
    this.isCurrentFirstPage = currentPage === this.pagesArray[0];
    this.isCurrentLastPage = currentPage === this.pagesArray[this.pagesArray.length - 1];

    if (pages.some(page => page === currentPage)) {
      const endIndex = pages.indexOf(currentPage) + 3;
      const startIndex = endIndex - this.maxPages;
      this.pages = pages.slice(
        startIndex < 0 ? 0 : startIndex,
        endIndex < this.maxPages ? this.maxPages : endIndex
      );
      this.changeDetectorRef.detectChanges();
    }
  }
}
