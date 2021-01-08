import { BaseComponent, DomUtils, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ILearningItemModel, LearningItemModel, LearningType } from '../models/learning-item.model';

import { DigitalContentItemModel } from '../models/digital-content-item.model';
import { LEARNER_PERMISSIONS } from '@opal20/domain-components';
import { Observable } from 'rxjs';
import { StandaloneFormItemModel } from '../models/standalone-form-item.model';
import { UserInfoModel } from '@opal20/domain-api';

const itemsPerPage: number = 20;
@Component({
  selector: 'learning-card-list',
  templateUrl: './learning-card-list.component.html'
})
export class LearningCardListComponent extends BaseComponent implements OnInit {
  @ViewChild('listSection', { static: false })
  public listSectionElement: ElementRef;
  @Input()
  public getPagedLearningItemsCallback: (
    maxResultCount: number,
    skipCount: number
  ) => Observable<{
    total: number;
    items: ILearningItemModel[];
  }>;

  @Input()
  public hasBackButton: boolean = false;
  @Input()
  public title: string | undefined;
  @Input()
  public titleHtml: string | undefined;
  @Input()
  public totalTextClass: string | undefined;
  @Input()
  public numberOfItemsOnPage: number = itemsPerPage;
  @Input()
  public showTitle: boolean = false;
  @Input()
  public showLongCard: boolean = false;
  @Input()
  public toolbarCustomClass?: string;
  @Input()
  public detailUrl?: string;
  @Input()
  public showCopyPermalink?: boolean;
  @Input()
  public isBookmark?: boolean = false;
  @Input()
  public showBookmark?: boolean = false;

  @Output()
  public backButtonClick: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output()
  public learningCardClick: EventEmitter<ILearningItemModel> = new EventEmitter<ILearningItemModel>();
  @Output()
  public iconPermalinkClick: EventEmitter<Event> = new EventEmitter<Event>();
  @Output()
  public iconBookmarkClick: EventEmitter<Event> = new EventEmitter<Event>();

  public learningItems: ILearningItemModel[];
  public total: number;
  public anyBookmarkChanged: boolean = false;
  public activePageNumber: number = 0;
  public totalPages: number = 0;
  public pages: Array<number | undefined> = [1];

  public learningType: typeof LearningType = LearningType;
  public scrollableParent: HTMLElement;
  public currentActiveSectionNumber: number = 1;

  constructor(protected moduleFacadeService: ModuleFacadeService, private elementRef: ElementRef) {
    super(moduleFacadeService);
  }

  public triggerDataChange(fromPage1: boolean = false): void {
    if (fromPage1 === true || this.activePageNumber === 0) {
      this.loadPageItems(1, true);
    } else {
      this.loadPageItems(this.activePageNumber, true);
    }
  }

  public ngOnInit(): void {
    super.ngOnInit();
    this.loadPageItems(1);
  }

  public onBackButtonClick(): void {
    this.backButtonClick.emit(this.anyBookmarkChanged);
  }

  public onLearningCardClick(event: LearningItemModel): void {
    this.learningCardClick.emit(event);
  }

  public onDigitalContentCardClick(event: DigitalContentItemModel): void {
    this.learningCardClick.emit(event);
  }

  public onStandaloneFormCardClick(event: StandaloneFormItemModel): void {
    this.learningCardClick.emit(event);
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

    this.activePageNumber = pageNumber;
    if (this.getPagedLearningItemsCallback !== undefined) {
      this.getPagedLearningItemsCallback(this.numberOfItemsOnPage, this.numberOfItemsOnPage * (pageNumber - 1))
        .pipe(this.untilDestroy())
        .subscribe(result => {
          this.total = result.total;
          this.learningItems = result.items;
          this.totalPages = this.calculateTotalPages(this.total);
          this.pages = this.generatePageNumberArray(this.totalPages);
          this.currentActiveSectionNumber = 1;
          if (this.scrollableParent !== undefined) {
            this.scrollableParent.scrollTop = 0;
          }
        });
    }
  }

  public loadPreviousPage(): void {
    this.loadPageItems(this.activePageNumber - 1);
  }

  public loadNextPage(): void {
    this.loadPageItems(this.activePageNumber + 1);
  }

  protected currentUserPermissionDic(): IPermissionDictionary {
    return UserInfoModel.getMyUserInfo().permissionDic;
  }

  public get hasPermissionToBookmark(): boolean {
    return this.hasPermission(LEARNER_PERMISSIONS.Action_Bookmark);
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
