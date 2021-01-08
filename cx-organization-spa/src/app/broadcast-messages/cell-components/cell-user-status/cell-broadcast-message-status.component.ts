import {
  ChangeDetectionStrategy,
  Component,
  ViewEncapsulation
} from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { BroadcastMessagesDto } from './../../models/broadcast-messages.model';

@Component({
  selector: 'cell-broadcast-message-status',
  templateUrl: './cell-broadcast-message-status.component.html',
  styleUrls: ['./cell-broadcast-message-status.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CellBroadcastMessageStatusComponent
  implements ICellRendererAngularComp {
  broadcastMessage: BroadcastMessagesDto;

  agInit(params: any): void {
    this.broadcastMessage = params.data;
  }

  refresh(params?: any): boolean {
    return true;
  }
}
