import { Component, Input } from '@angular/core';

@Component({
  selector: 'status-indicator',
  templateUrl: './status-indicator.component.html'
})
export class StatusIndicatorComponent {
  @Input() public currentStatus: string | string[];
  @Input() public prefix: string = '';
  @Input() public mapping: Dictionary<{ color: string; text: string }> | Dictionary<{ color: string; text: string }>[];
  @Input() public textOnly: boolean = false;
  @Input() public delimiter: string = ', ';

  public canDisplayStatusDot(): boolean {
    if (this.currentStatus instanceof Array) {
      return (
        this.currentStatus[0] && this.mapping[0][this.currentStatus[0]] && this.mapping[0][this.currentStatus[0]].color && !this.textOnly
      );
    } else {
      return this.currentStatus && this.mapping[this.currentStatus] && this.mapping[this.currentStatus].color && !this.textOnly;
    }
  }

  public displayStatusColorDot(): string {
    if (this.currentStatus instanceof Array) {
      return this.canDisplayStatusDot() ? this.mapping[0][this.currentStatus[0]].color : '';
    } else {
      return this.canDisplayStatusDot() ? this.mapping[this.currentStatus].color : '';
    }
  }

  public displayStatus(): string {
    if (this.currentStatus instanceof Array) {
      let displayText = '';
      for (let index = 0; index < this.currentStatus.length; index++) {
        const selectedStatus = this.currentStatus[index];
        const selectedDict = this.mapping[index];
        if (selectedStatus && selectedDict && selectedDict[selectedStatus] && selectedDict[selectedStatus].text) {
          displayText += selectedDict[selectedStatus].text + this.delimiter;
        }
      }
      displayText = displayText.substring(0, displayText.length - this.delimiter.length);
      return displayText;
    }
    return this.mapping[this.currentStatus] && this.mapping[this.currentStatus].text ? this.mapping[this.currentStatus].text : '';
  }
}
