import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Directive, HostListener } from '@angular/core';

@Directive({
  selector: '[opalNumeric]'
})
export class OpalNumericDirective extends BaseComponent {
  public numericList: string[] = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];

  public textOperateList: string[] = ['Delete', 'Backspace', 'Home', 'End', 'ArrowLeft', 'ArrowRight', 'Tab', 'Escape', 'Enter'];

  public selectCopyCutPaste: string[] = ['a', 'A', 'c', 'C', 'x', 'X', 'v', 'V'];

  constructor(moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  @HostListener('keydown', ['$event']) public onKeyDown(e: KeyboardEvent): void {
    if (!this.validInput(e)) {
      e.preventDefault();
    }
  }

  public validInput(e: KeyboardEvent): boolean {
    const isTextOperate: boolean = this.textOperateList.includes(e.key);
    const isNumeric: boolean = this.numericList.includes(e.key);
    const isSelectCopyCutPaste: boolean = this.selectCopyCutPaste.includes(e.key) && e.ctrlKey;
    if (isTextOperate || isNumeric || isSelectCopyCutPaste) {
      return true;
    } else {
      return false;
    }
  }
}
