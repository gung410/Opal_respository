import { Directive, Input } from '@angular/core';

@Directive({
  selector: 'img',
  host: {
    '(error)': 'error($event)',
    '[src]': 'src'
  }
})

export class DefaultImageDirective {
  @Input() src: string;
  @Input() defaultSrc: string;
  error(event) {
    this.src = this.defaultSrc ? this.defaultSrc : 'assets/images/default-image.png';
  }
}
