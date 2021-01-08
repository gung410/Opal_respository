import { CxStringUtil } from './../../utils/string.util';
import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { CxObjectUtil } from '../../utils/object.util';

@Component({
    selector: 'cx-breadcrumb',
    templateUrl: './cx-breadcrumb.component.html',
    styleUrls: ['./cx-breadcrumb.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush,
    encapsulation: ViewEncapsulation.None
})
export class CxBreadcrumbComponent implements OnInit {

    @Input() breadCrumbItems: any[];
    @Input() labelledPropertyName: string;
    @Input() truncatedLimit = 4;
    @Input() separatorSymbol = '/';
    @Input() overflowSuffix = '...';

    @Output() clickItem: EventEmitter<any> = new EventEmitter<any>();

    constructor() { }

    ngOnInit() {
    }

    public onClickedBreadCrumbItem(breadCrumbItem: any): void {
        this.clickItem.emit(breadCrumbItem);
    }

    public getPropertyValue(object: any, propertyRoute: string): any {
        return CxObjectUtil.getPropertyValue(object, propertyRoute);
    }

    public getPropertyValueWithTruncatedLabel(object: any, propertyRoute: string): any {
        const label = CxObjectUtil.getPropertyValue(object, propertyRoute);
        return CxStringUtil.truncateWordByWord(label, this.truncatedLimit, this.overflowSuffix);
    }
}
