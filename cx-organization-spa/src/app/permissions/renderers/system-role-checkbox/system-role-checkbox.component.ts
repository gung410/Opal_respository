import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output
} from '@angular/core';
import { Dictionary, Utils } from 'app-utilities/utils';
import { AssignableSystemRole } from 'app/core/dtos/assignable-system-role.dto';
import { Granted } from 'app/core/models/system-role-permission-subject.model';

@Component({
  selector: 'system-role-checkbox',
  templateUrl: './system-role-checkbox.component.html',
  styleUrls: ['./system-role-checkbox.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SystemRoleCheckboxComponent implements OnInit, AfterViewInit {
  @Input() systemRoleId: number;
  @Input() isCollapsed: boolean = false;
  grantedLabelDic: Dictionary<string> = {};
  fullGrantedLabel: string = 'View and assign';
  readGrantedLabel: string = 'View only';

  @Output()
  selectedValueChange: EventEmitter<AssignableSystemRole> = new EventEmitter<AssignableSystemRole>();

  get isSystemRoleChecked(): boolean {
    return this._isSystemRoleChecked;
  }
  set isSystemRoleChecked(isSystemRoleChecked: boolean) {
    if (isSystemRoleChecked == null) {
      return;
    }

    if (isSystemRoleChecked && this.grantedChoice === Granted.Deny) {
      this.grantedChoice = Granted.Full;
    }

    if (!isSystemRoleChecked) {
      this.resetGrantedChoice();
    }

    this._isSystemRoleChecked = isSystemRoleChecked;
  }

  get grantedChoice(): Granted {
    return this._grantedChoice;
  }
  // tslint:disable-next-line: no-unsafe-any
  @Input() set grantedChoice(newGrantedChoice: Granted) {
    if (
      !newGrantedChoice ||
      !Utils.isDifferent(this._grantedChoice, newGrantedChoice)
    ) {
      return;
    }

    if (newGrantedChoice !== Granted.Deny && !this.isSystemRoleChecked) {
      this._isSystemRoleChecked = true;
    }

    this._grantedChoice = newGrantedChoice;

    this.selectedValueChange.emit(
      new AssignableSystemRole({
        id: this.systemRoleId,
        granted: newGrantedChoice
      })
    );
  }

  private _grantedChoice: Granted = Granted.Deny;

  private _isSystemRoleChecked: boolean = false;

  constructor(private elementRef: ElementRef<HTMLElement>) {
    this.grantedLabelDic[Granted.Full] = this.fullGrantedLabel;
    this.grantedLabelDic[Granted.Read] = this.readGrantedLabel;
  }

  ngOnInit(): void {}

  ngAfterViewInit(): void {
    const imageElement = this.elementRef.nativeElement.querySelector(
      '.arrow-icon'
    );
    this.collapseSwitch(imageElement);
  }

  onArrowIconClicked(mouseEvent: MouseEvent): void {
    const imageElement = mouseEvent.target as HTMLElement;
    this.isCollapsed = !this.isCollapsed;
    this.collapseSwitch(imageElement);
  }

  private collapseSwitch(imageElement: HTMLElement | Element): void {
    if (this.isCollapsed) {
      imageElement.classList.add('role-arrow-icon-collapse');

      return;
    }

    imageElement.classList.remove('role-arrow-icon-collapse');
  }

  private resetGrantedChoice(): void {
    this.grantedChoice = Granted.Deny;
  }
}
