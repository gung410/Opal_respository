import {
    Component,
    ElementRef,
    ChangeDetectorRef,
    SimpleChanges,
    ViewEncapsulation,
    ChangeDetectionStrategy,
    Input,
    OnInit,
    OnChanges,
    Renderer2
} from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { MediaObserver } from '@angular/flex-layout';
import { CxAbstractTableComponent } from '../cx-abstract-table.component';
import { orderBy } from 'lodash';
import { CxColumnSortType } from '../cx-table.model';
import { CxAnimations } from '../../../constants/cx-animation.constant';

@Component({
    selector: 'cx-table',
    templateUrl: './cx-table.component.html',
    styleUrls: ['../cx-abstract-table.component.scss',
        './cx-table.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: [CxAnimations.smoothAppendRemove],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class CxTableComponent<RowData> extends CxAbstractTableComponent<RowData> implements OnInit, OnChanges {
    @Input() emptyStateText: string;
    constructor(
        public changeDetectorRef: ChangeDetectorRef,
        public elementRef: ElementRef,
        public media: MediaObserver,
        public ngbModal: NgbModal,
        renderer: Renderer2
    ) {
        super(changeDetectorRef, elementRef, media, ngbModal, renderer);
    }

    ngOnInit() {
        super.ngOnInit();
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes.items && changes.items.currentValue !== changes.items.previousValue) {
            this.itemsUnsorted = this.items;
            if (!this.isDataLazyLoad) {
                this.sortDataByHeader(this.currentFieldSort, this.currentSortType);
            }
            this.updateSelectedItemsMapBaseOnItemsList(this.items);
        }
    }

    public sortByFieldClick(fieldSort: string, currentSort: CxColumnSortType) {
        this.currentFieldSort = fieldSort;
        this.currentSortType = currentSort;
        let nextSortType: CxColumnSortType;
        if (!this.isDataLazyLoad) {
            this.sortDataByHeader(fieldSort, currentSort);
        } else {
            nextSortType = this.changeSortInHeaders(
                this.headers,
                fieldSort,
                currentSort
            );
        }
        this.sortTypeChange.emit({ fieldSort, sortType: nextSortType });
    }

    protected sortDataByHeader(fieldSort: string, currentSortType: CxColumnSortType) {
        const nextSortType = this.changeSortInHeaders(
            this.headers,
            fieldSort,
            currentSortType
        );
        if (nextSortType === CxColumnSortType.UNSORTED) {
            this.items = this.itemsUnsorted;
            return;
        }
        const sort = nextSortType === CxColumnSortType.ASCENDING;

        const itemData = this.itemsUnsorted;
        this.items = sort
            ? orderBy(itemData, [fieldSort], [sort])
            : orderBy(itemData, [fieldSort], [sort]).reverse();
    }
}
