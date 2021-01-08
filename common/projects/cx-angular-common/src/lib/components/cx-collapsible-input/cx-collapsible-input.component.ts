import {
  Component,
  OnInit,
  ElementRef,
  ChangeDetectorRef,
  ViewEncapsulation,
  ChangeDetectionStrategy,
  Input,
  ViewChild
} from "@angular/core";
import { CxInputComponent } from "../cx-input/cx-input.component";
import { MediaObserver } from "@angular/flex-layout";

@Component({
  selector: "cx-collapsible-input",
  templateUrl: "./cx-collapsible-input.component.html",
  styleUrls: ["./cx-collapsible-input.component.scss"],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CxCollapsibleInputComponent extends CxInputComponent
  implements OnInit {
  public isOpenInput: boolean = false;
  @Input() expandedInputWidth: string = "300px";
  @ViewChild(CxInputComponent) cxInputComponent: CxInputComponent;
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    elementRef: ElementRef,
    media: MediaObserver
  ) {
    super(changeDetectorRef, elementRef, media);
  }

  ngOnInit() {}

  public onClearClicked() {
    this.cxInputComponent.focusInput();
    this.isOpenInput = false;
  }

  public onSubmitted() {
      this.submit.emit(this.value);
  }

  public onOpenButtonClicked() {
    this.isOpenInput = true;
    this.cxInputComponent.focusInput();
  }
}
