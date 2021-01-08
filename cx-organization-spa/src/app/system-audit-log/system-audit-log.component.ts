import { Component } from '@angular/core';
import { environment } from 'app-environments/environment';

@Component({
  selector: 'system-audit-log',
  templateUrl: './system-audit-log.component.html',
  styleUrls: ['./system-audit-log.component.scss']
})
export class SystemAuditLogComponent {
  baseProfileUrl: string = environment.auditLogAppUrl;
}
