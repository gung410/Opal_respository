import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy, ViewEncapsulation, SimpleChanges } from '@angular/core';
import { CxBreadCrumbItem } from './model/breadcrumb.model';
import { CxStringUtil } from './../../utils/string.util';
@Component({
    selector: 'cx-breadcrumb-simple',
    templateUrl: './cx-breadcrumb-simple.component.html',
    styleUrls: ['./cx-breadcrumb-simple.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush,
    encapsulation: ViewEncapsulation.None
})
export class CxBreadcrumbSimpleComponent {
    @Input() simpleStyle = false;
    @Input() breadCrumbItems: CxBreadCrumbItem[];
    @Output() clickedItem: EventEmitter<any> = new EventEmitter<any>();
    @Input() truncatedLimit = 4;
    @Input() breadcrumbItemsLimit = 3;
    @Input() overflowSuffix = '...';
    isExpandedBreadcrumb = true;
    expandIconClick = ' / ... / ';
    constructor() {}

    // tslint:disable-next-line:use-life-cycle-interface
    ngOnChanges(changes: SimpleChanges): void {
        this.isExpandedBreadcrumb = this.breadCrumbItems.length <= this.breadcrumbItemsLimit;
    }

    public onClickedBreadCrumbItem(value, index): void {
        this.isExpandedBreadcrumb = index < this.breadcrumbItemsLimit;
        this.clickedItem.emit(value);
    }

    public onClickExpandBreadcrumb(): void {
        this.isExpandedBreadcrumb = true;
    }

    public getPropertyValueWithTruncatedLabel(nameDepartment: any, isFirstBreadcrumbItem: boolean, isLastBreadcrumbItem: boolean): any {
        return isLastBreadcrumbItem || isFirstBreadcrumbItem ? nameDepartment :
          CxStringUtil.truncateWordByWord(nameDepartment, this.truncatedLimit, this.overflowSuffix);
    }
}
