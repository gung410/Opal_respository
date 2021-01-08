import { FormDetailMode } from '@opal20/domain-components';
import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable()
export class FormEditModeService {
  public modeChanged: Subject<FormDetailMode> = new Subject();
  public initMode: FormDetailMode = FormDetailMode.View;
}
