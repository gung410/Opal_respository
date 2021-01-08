import { Input, TemplateRef, EventEmitter, Output, Directive } from '@angular/core';
import { CxTreeIcon } from '../cx-tree/models/cx-tree-icon.model';
import { CxTreeText } from '../cx-tree/models/cx-tree-text.model';
import { CxTreeButtonCondition } from '../cx-tree/models/cx-tree-button-condition.model';
import { CxObjectRoute } from '../cx-tree/models/cx-object-route.model';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CxObjectUtil } from '../../utils/object.util';

@Directive()
export abstract class CxAbstractTreeComponent<T> {
    @Input() dynamicIcon: ((object: T) => string)[];
    @Input() icon: CxTreeIcon = new CxTreeIcon();
    @Input() text: CxTreeText = new CxTreeText();
    @Input() idFieldRoute;
    @Input() parentIdFieldRoute = 'parentDepartmentId';
    @Input() displayFieldRoute;
    @Input() buttonCondition: CxTreeButtonCondition<T> = new CxTreeButtonCondition();
    @Output() editItem: EventEmitter<T> = new EventEmitter();
    @Output() deleteItem: EventEmitter<any> = new EventEmitter();
    @Output() addItem: EventEmitter<any> = new EventEmitter();
    @Output() clickItem: EventEmitter<any> = new EventEmitter();
    constructor(
    ) {
    }

    public getRootObjectFromArray(flatObjectsArray: T[]): T {
        const mapObjIds = {};
        flatObjectsArray.forEach(obj => {
            if (mapObjIds[CxObjectUtil.getPropertyValue(obj,
                this.idFieldRoute)] !== undefined) { return; }
            mapObjIds[CxObjectUtil.getPropertyValue(obj, this.idFieldRoute)] = obj;
        });

        return flatObjectsArray.find(
            obj => mapObjIds[CxObjectUtil.getPropertyValue(obj, this.parentIdFieldRoute)] === undefined
        );
    }

    public getRootObjectsFromArray(flatObjectsArray: T[]): T[] {
      const mapObjIds = {};
      flatObjectsArray.forEach(obj => {
          if (mapObjIds[CxObjectUtil.getPropertyValue(obj,
              this.idFieldRoute)] !== undefined) { return; }
          mapObjIds[CxObjectUtil.getPropertyValue(obj, this.idFieldRoute)] = obj;
      });

      return flatObjectsArray.filter(
          obj => mapObjIds[CxObjectUtil.getPropertyValue(obj, this.parentIdFieldRoute)] === undefined
      );
  }
}
