import { BaseComponent } from '../../abstracts/base.component';
import { Input, Output, EventEmitter, TemplateRef, ChangeDetectorRef, ElementRef, AfterViewInit, ViewChild, OnInit, ViewChildren, QueryList, Renderer2, OnDestroy, AfterViewChecked, DoCheck, Directive } from '@angular/core';
import { CxItemTableHeaderModel } from './cx-table-containers/cx-table-containers.model';
import {
    CxTableIcon,
    CxColumnSortType,
    CxTableLabelModel
} from './cx-table.model';
import { uniqueId } from 'lodash';
import { CxObjectUtil } from '../../utils/object.util';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { MediaObserver } from '@angular/flex-layout';
import { CxRemoveItemsConfirmDialogComponent } from './cx-remove-items-confirm-dialog/cx-remove-items-confirm-dialog.component';

@Directive()
export abstract class CxAbstractTableComponent<RowData>
    extends BaseComponent implements AfterViewInit, OnInit, OnDestroy, AfterViewChecked {
    public masterSelected: boolean;
    public isHeaderFixed = false;
    public itemTrackByFn: (index, item) => {};
    private tableScrollTimeout;
    private lastUpdatedHeaderTimeStamp = Date.now();
    protected itemsUnsorted: RowData[] = [];
    public selectedItemsMap = {};
    public tableId: string = uniqueId();
    protected _items: RowData[];
    @Output() removeItems = new EventEmitter<RowData[]>();
    @Input() isDataLazyLoad = true;
    @Input() checkboxDisabled = false;
    @Input() headers: CxItemTableHeaderModel[] = [];
    @Input() itemTemplate: TemplateRef<RowData>;
    @Input() itemCustomActionTemplate: TemplateRef<RowData>;
    @Input() itemIdRoutes = [];
    @Input() icon: CxTableIcon = new CxTableIcon();
    @Input() currentSortType: CxColumnSortType = CxColumnSortType.UNSORTED;
    @Input() currentFieldSort: string;
    @Input() showActionsButtonInLastRow = true;
    @Input() lastColumnCustomTemplate: TemplateRef<RowData>;
    @Input() removeDialogConfimationText = 'Confirm';
    @Input() showMassDefaultAction = true;
    @Input() fixedHeaderTopPositionWhenWindowScrollInPx = 50;
    @Input() cxTableLabel = new CxTableLabelModel();
    @Input() widthOfLastColumnHeader = 50;
    @Input() itemActionNgbBootstrapPlacement: (itemIndex: number) => string = () => 'left-top';
    @Input() get items(): RowData[] {
        return this._items;
    }
    set items(items: RowData[]) {
        this._items = items;
    }
    @Output() sortTypeChange = new EventEmitter<{ fieldSort: string, sortType: CxColumnSortType }>();
    @Output() selectItem = new EventEmitter<{ selected: boolean, value: RowData }>();
    @ViewChild('tableHeadPlaceholder') tableHeadPlaceholderRef: ElementRef;
    @ViewChild('tableHead') tableHeadRef: ElementRef;
    @ViewChild('table') tableRef: ElementRef;
    @ViewChildren('tableHeadPlaceholderColumn') tableHeadPlaceholderColumnRefs: QueryList<ElementRef>;
    @ViewChildren('tableHeadColumn') tableHeadColumnRefs: QueryList<ElementRef>;
    public get selectedRow(): number {
        return Object.values(this.selectedItemsMap).length;
    }
    public get showMassActions(): boolean {
        const anySelectedItems = CxObjectUtil.hasAnyProperty(this.selectedItemsMap);
        return anySelectedItems;
    }
    public get masterCheckboxGroupName(): string {
        return `${this.tableId}_master`;
    }

    public get childrenCheckboxesGroupName(): string {
        return `${this.tableId}_item`;
    }

    constructor(
        public changeDetectorRef: ChangeDetectorRef,
        public elementRef: ElementRef,
        public media: MediaObserver,
        public ngbModal: NgbModal,
        private renderer: Renderer2
    ) {
        super(changeDetectorRef, elementRef, media);
    }

    ngOnInit() {
        super.ngOnInit();
        this.itemTrackByFn = (index, item) => {
            return CxObjectUtil.getUniqueId(item, this.itemIdRoutes);
        };
    }

    ngAfterViewInit() {
        super.ngAfterViewInit();
        window.addEventListener('scroll', this.updateTableHeadPosition);
        window.addEventListener('resize', this.resizeHandler);
    }

    ngAfterViewChecked(): void {
        setTimeout(() => {
            this.updateHeaderColumnsWidth(this.tableHeadPlaceholderColumnRefs,
                this.tableHeadColumnRefs);
        });
    }

    ngOnDestroy() {
        window.removeEventListener('scroll', this.updateTableHeadPosition);
        window.removeEventListener('resize', this.resizeHandler);
    }

    private resizeHandler = () => {
        this.updateHeaderColumnsWidth(this.tableHeadPlaceholderColumnRefs, this.tableHeadColumnRefs);
    }

    private updateTableHeadPosition = () => {
        this.tableScrollTimeout = setTimeout(() => {
            this.lastUpdatedHeaderTimeStamp = Date.now();
            if (this.tableHeadPlaceholderRef === undefined) { return; }
            const distanceFromTopTableBottomToViewportTop = this.tableRef.nativeElement.getBoundingClientRect().bottom;
            const isTopLimitLineCoveredTableBottom = distanceFromTopTableBottomToViewportTop
                - this.tableHeadRef.nativeElement.offsetHeight * 2
                < this.fixedHeaderTopPositionWhenWindowScrollInPx;
            const isTopLimitLineCoveredTableHeadBottom =
                this.tableRef.nativeElement.getBoundingClientRect().top
                + this.tableHeadPlaceholderRef.nativeElement.offsetHeight
                < this.fixedHeaderTopPositionWhenWindowScrollInPx;
            if (isTopLimitLineCoveredTableBottom
                || !isTopLimitLineCoveredTableHeadBottom) {
                this.isHeaderFixed = false;
                this.detectChanges();
                return;
            }

            const distanceFromTopTableTopToViewportTop = this.tableHeadPlaceholderRef.nativeElement.getBoundingClientRect().top;
            this.isHeaderFixed = distanceFromTopTableTopToViewportTop < this.fixedHeaderTopPositionWhenWindowScrollInPx;
            this.detectChanges();
        });
        if (Date.now() - this.lastUpdatedHeaderTimeStamp < 10) {
            clearTimeout(this.tableScrollTimeout);
        }
    }

    private updateHeaderColumnsWidth = (
        tableHeadPlaceholderColumnRefs: QueryList<ElementRef>,
        tableHeadColumnRefs: QueryList<ElementRef>) => {
        if (tableHeadPlaceholderColumnRefs === undefined || tableHeadColumnRefs === undefined) { return; }
        const tableHeadColumnRefsArray = tableHeadColumnRefs.toArray();
        const isAnyColumnChangeWidth = tableHeadPlaceholderColumnRefs.some((placeholderColRef, index) =>
            tableHeadColumnRefsArray[index].nativeElement.offsetWidth !== placeholderColRef.nativeElement.offsetWidth);
        if (!isAnyColumnChangeWidth) { return; }
        tableHeadPlaceholderColumnRefs.forEach((eleRef, index) => {
            this.renderer.setStyle(tableHeadColumnRefsArray[index].nativeElement,
                'width', `${eleRef.nativeElement.offsetWidth}px`);
        });
        this.detectChanges();
    }

    public abstract sortByFieldClick(
        fieldSort: string,
        currentSort: CxColumnSortType
    );
    protected abstract sortDataByHeader(
        fieldSort: string,
        currentSort: CxColumnSortType
    );

    public onSelectedItemChange(changedData: {
        selected: boolean;
        value: RowData;
    }) {
        if (changedData.selected === true) {
            this.selectedItemsMap[
                CxObjectUtil.getUniqueId(changedData.value, this.itemIdRoutes)
            ] = changedData.value;
        } else {
            delete this.selectedItemsMap[
                CxObjectUtil.getUniqueId(changedData.value, this.itemIdRoutes)
            ];
        }
        this.selectItem.emit(changedData);
        this.detectChanges();
    }

    protected changeSortInHeaders(
        headers: CxItemTableHeaderModel[],
        fieldSort: string,
        currentSortType: CxColumnSortType
    ): CxColumnSortType {
        const sortIndex = headers.findIndex(item => item.fieldSort === fieldSort);
        let nextSortType: CxColumnSortType;
        if (sortIndex !== -1) {
            switch (currentSortType) {
                case CxColumnSortType.UNSORTED:
                    nextSortType = CxColumnSortType.ASCENDING;
                    break;
                case CxColumnSortType.ASCENDING:
                    nextSortType = CxColumnSortType.DESCENDING;
                    break;
                case CxColumnSortType.DESCENDING:
                    nextSortType = CxColumnSortType.UNSORTED;
                    break;
                default:
                    nextSortType = CxColumnSortType.UNSORTED;
                    break;
            }
            headers[sortIndex].sortType = nextSortType;
        }

        headers.forEach(item => {
            if (item.fieldSort !== fieldSort && item.fieldSort !== '') {
                item.sortType = CxColumnSortType.UNSORTED;
            }
        });
        return nextSortType;
    }

    public getSortIcon(header: CxItemTableHeaderModel) {
        if (!header.fieldSort) {
            return '';
        }

        switch (header.sortType) {
            case CxColumnSortType.UNSORTED:
                return 'icon icon-sortable';
            case CxColumnSortType.ASCENDING:
                return 'icon icon-sort-up';
            case CxColumnSortType.DESCENDING:
                return 'icon icon-sort-down';
            default:
                return '';
        }
    }

    public onDeleteItemClicked(item: RowData) {
        const modalRef = this.ngbModal.open(CxRemoveItemsConfirmDialogComponent, {
            size: 'lg',
            centered: true
        });
        (modalRef.componentInstance as CxRemoveItemsConfirmDialogComponent<RowData>).items = [item];
        (modalRef.componentInstance as CxRemoveItemsConfirmDialogComponent<RowData>).removeItems.subscribe(data => {
            if (data) {
                this.removeItems.emit([item]);
            }
            modalRef.close();
        });
    }

    public onRemoveSelectedItems() {
        const selectedItems = Object.values(this.selectedItemsMap);
        const modalRef = this.ngbModal.open(CxRemoveItemsConfirmDialogComponent, {
            size: 'lg',
            centered: true
        });
        (modalRef.componentInstance as CxRemoveItemsConfirmDialogComponent<RowData>).items = selectedItems as RowData[];
        (modalRef.componentInstance as CxRemoveItemsConfirmDialogComponent<RowData>).dialogHeaderText = this.removeDialogConfimationText;
        (modalRef.componentInstance as CxRemoveItemsConfirmDialogComponent<RowData>).removeItems.subscribe(data => {
            if (data) {
                this.removeItems.emit(selectedItems as RowData[]);
            }
            modalRef.close();
        });
    }

    public executeRemoveItems(items: RowData[]) {
        const removedItemsMap = {};

        items.forEach(item => {
            const itemId = CxObjectUtil.getUniqueId(item, this.itemIdRoutes);
            removedItemsMap[itemId] = item;
            delete this.selectedItemsMap[itemId];
        });
        this.items = this.items.filter(currItem => {
            const currItemId = CxObjectUtil.getUniqueId(currItem, this.itemIdRoutes);
            return removedItemsMap[currItemId] === undefined;
        });
        this.itemsUnsorted = this.itemsUnsorted.filter(i => {
            const currItemId = CxObjectUtil.getUniqueId(i, this.itemIdRoutes);
            return removedItemsMap[currItemId] === undefined;
        });
        this.detectChanges();
    }

    public getSelectedItemsMap() {
        return this.selectedItemsMap;
    }

    protected updateSelectedItemsMapBaseOnItemsList(items: RowData[]): void {
        const itemUniqueIdsMap = {};

        items.forEach(i => {
            itemUniqueIdsMap[CxObjectUtil.getUniqueId(i, this.itemIdRoutes)] = i;
        });

        for (const key in this.selectedItemsMap) {
            const itemsListContainsSelectedItem = itemUniqueIdsMap[key] !== undefined;
            if (itemsListContainsSelectedItem) {
                continue;
            }
            delete this.selectedItemsMap[key];
        }
    }

    public getItemIdFromRoute(item: RowData) {
        return CxObjectUtil.getUniqueId(item, this.itemIdRoutes);
    }

    public clearSelectedItemsMap() {
        this.selectedItemsMap = {};
        this.changeDetectorRef.detectChanges();
    }
}

