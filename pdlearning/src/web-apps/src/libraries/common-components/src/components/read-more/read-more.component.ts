import { Component, Input } from '@angular/core';

@Component({
  selector: 'read-more',
  templateUrl: './read-more.component.html',
  styleUrls: ['./read-more.component.scss']
})
export class ReadMoreComponent {
  @Input() public content: string = '';
  @Input() public maxDisplayChar: number = 200;
  @Input() public buttonText: string = 'Read more';
  public isShowReadMoreButton: boolean;
  public displayChar: number;

  public ngOnChanges(): void {
    if (this.content) {
      this.isShowReadMoreButton = this.content.length > this.maxDisplayChar - 1;
      this.displayChar = this.isShowReadMoreButton ? this.maxDisplayChar - 1 : this.content.length;
    }
  }

  public readMore(): void {
    this.displayChar = this.content.length;
    this.isShowReadMoreButton = false;
  }
}
