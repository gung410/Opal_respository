import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy,
  Output,
  SimpleChanges,
} from '@angular/core';
import { CxSurveyjsEventModel } from '@conexus/cx-angular-common';
import { CxLazySurveyjsService } from 'app-services/cx-lazy-surveyjs.service';

@Component({
  selector: 'cx-lazy-surveyjs',
  templateUrl: './cx-lazy-surveyjs.component.html',
})
export class CxLazySurveyjsComponent implements OnChanges, OnDestroy {
  @Input() id: string;
  @Input() json: any;
  @Input() data: any;
  @Input() hideFooter: boolean;

  @Output() afterQuestionsRender: EventEmitter<
    CxSurveyjsEventModel
  > = new EventEmitter<CxSurveyjsEventModel>();

  loadingData: boolean = true;
  timer: any;

  constructor(
    private changeDetectorRef: ChangeDetectorRef,
    private cxLazySurveyjsService: CxLazySurveyjsService
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    this.timer = setTimeout(() => {
      if (this.json) {
        this.loadingData = false;
        this.changeDetectorRef.detectChanges();
      }
    }, this.cxLazySurveyjsService.addNewForm(this.id));
  }

  ngOnDestroy(): void {
    // Ensure the timeout is cleared when the component is destroyed.
    clearTimeout(this.timer);
  }

  afterQuestionsRendered(event: CxSurveyjsEventModel): void {
    this.afterQuestionsRender.emit(event);
  }
}
