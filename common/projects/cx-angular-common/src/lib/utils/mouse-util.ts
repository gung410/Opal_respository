import { fromEvent, Subscription } from 'rxjs';
import { CxWindowUtil } from './window-util';
import { CxDateUtil } from './date-util';
import { CxProcessUtil } from './process-util';

export class CxMouseUtil {
  private static minDistanceToStateThatMouseMoved = 2;
  public static commonWaitForDbClickDelayTime = 300;

  public static isMousePositionChanged(
    mousedownEvent: MouseEvent,
    mouseMoveEvent: MouseEvent
  ) {
    return (
      Math.abs(mousedownEvent.x - mouseMoveEvent.x) >
        CxMouseUtil.minDistanceToStateThatMouseMoved ||
      Math.abs(mousedownEvent.y - mouseMoveEvent.y) >
        CxMouseUtil.minDistanceToStateThatMouseMoved
    );
  }

  public static isTouchPositionChanged(
    touchStartEvent: TouchEvent,
    touchEndEvent: TouchEvent
  ) {
    return (
      Math.abs(
        touchStartEvent.touches[0].clientX -
          touchEndEvent.changedTouches[0].clientX
      ) >= CxMouseUtil.minDistanceToStateThatMouseMoved ||
      Math.abs(
        touchStartEvent.touches[0].clientY -
          touchEndEvent.changedTouches[0].clientY
      ) >= CxMouseUtil.minDistanceToStateThatMouseMoved
    );
  }

  public static createClickOnlyCheckContext(
    targetElement: () => HTMLElement | undefined
  ): CxMouseUtilClickOnlyCheckContext {
    return new CxMouseUtilClickOnlyCheckContext(targetElement);
  }
}

export enum MouseWhich {
  LeftClick = 1,
  MiddleClick = 2,
  RightClick = 3
}

export class CxMouseUtilClickOnlyCheckContext {
  constructor(
    public targetElement: () => HTMLElement | undefined,
    public onClickedOnly?: (e: Event) => any,
    public onDblClickedOnly?: (e: Event) => any
  ) {}

  private onDbClickSubs: Subscription = new Subscription();
  private onMouseDownSubs: Subscription = new Subscription();
  private onMouseUpSubs: Subscription = new Subscription();
  private onTouchStartSubs: Subscription = new Subscription();
  private onTouchEndSubs: Subscription = new Subscription();
  private waitForSecondDoubleTouchSubs: Subscription | undefined;
  private callOnDblClickedOnlyDelaySubs: Subscription | undefined;
  private lastMouseDownEvent?: MouseEvent;
  private lastMouseUpEvent?: MouseEvent;
  private lastTouchStartEvent?: TouchEvent;
  private lastTouchEndEvent?: TouchEvent;
  private lastMouseDownTime: Date | undefined;
  private lastTouchStartTime: Date | undefined;

  public subscribe() {
    this.unsubscribe();
    const targetElement = this.targetElement();
    if (targetElement !== undefined) {
      this.onMouseDownSubs = fromEvent(targetElement, 'mousedown').subscribe(
        (e: MouseEvent) => {
          this.lastTouchStartEvent = undefined;
          this.lastMouseDownEvent = e;
          this.lastMouseDownTime = new Date();
          if (this.callOnDblClickedOnlyDelaySubs !== undefined) {
            this.callOnDblClickedOnlyDelaySubs.unsubscribe();
          }
        }
      );
      this.onMouseUpSubs = fromEvent(targetElement, 'mouseup').subscribe(
        (e: MouseEvent) => {
          this.lastMouseUpEvent = e;
          if (
            this.lastMouseDownTime !== undefined &&
            CxDateUtil.diff(new Date(), this.lastMouseDownTime) <
              CxMouseUtil.commonWaitForDbClickDelayTime
          ) {
            this.callOnClickedOnly(e);
          }
        }
      );

      this.onDbClickSubs = fromEvent(targetElement, 'dblclick').subscribe(
        (e: MouseEvent) => {
          if (this.onDblClickedOnly !== undefined) {
            this.onDblClickedOnly(e);
          }
          if (this.callOnDblClickedOnlyDelaySubs !== undefined) {
            this.callOnDblClickedOnlyDelaySubs.unsubscribe();
          }
        }
      );

      this.onTouchStartSubs = fromEvent(targetElement, 'touchstart').subscribe(
        (e: TouchEvent) => {
          const wasLastTouchNotScrolling = this.wasLastClickedOrTouchOnly();
          this.lastMouseDownEvent = undefined;
          this.lastTouchStartEvent = e;
          const isThisTouchNotMovedComparedToLastTouch = this.wasLastClickedOrTouchOnly();
          this.lastTouchStartTime = new Date();
          if (this.callOnDblClickedOnlyDelaySubs !== undefined) {
            this.callOnDblClickedOnlyDelaySubs.unsubscribe();
          }

          if (this.waitForSecondDoubleTouchSubs === undefined) {
            this.waitForSecondDoubleTouchSubs = CxProcessUtil.delay(() => {
              this.waitForSecondDoubleTouchSubs = undefined;
            }, CxMouseUtil.commonWaitForDbClickDelayTime);
          } else {
            this.waitForSecondDoubleTouchSubs.unsubscribe();
            this.waitForSecondDoubleTouchSubs = undefined;
            if (
              this.onDblClickedOnly !== undefined &&
              wasLastTouchNotScrolling &&
              isThisTouchNotMovedComparedToLastTouch
            ) {
              this.onDblClickedOnly(e);
            }
          }
        }
      );
      this.onTouchEndSubs = fromEvent(targetElement, 'touchend').subscribe(
        (e: TouchEvent) => {
          this.lastMouseUpEvent = undefined;
          this.lastTouchEndEvent = e;
          if (
            this.lastTouchStartTime !== undefined &&
            CxDateUtil.diff(new Date(), this.lastTouchStartTime) <
              CxMouseUtil.commonWaitForDbClickDelayTime
          ) {
            this.callOnClickedOnly(e);
          }
        }
      );
    }
  }

  public unsubscribe() {
    this.onMouseDownSubs.unsubscribe();
    this.onMouseUpSubs.unsubscribe();
    this.onTouchStartSubs.unsubscribe();
    this.onTouchEndSubs.unsubscribe();
    this.onDbClickSubs.unsubscribe();
  }

  public wasLastClickedOrTouchOnly(): boolean {
    if (
      this.lastTouchStartEvent !== undefined &&
      this.lastTouchEndEvent !== undefined &&
      !CxMouseUtil.isTouchPositionChanged(
        this.lastTouchStartEvent,
        this.lastTouchEndEvent
      )
    ) {
      return true;
    }
    if (
      this.lastMouseDownEvent !== undefined &&
      this.lastMouseUpEvent !== undefined &&
      this.lastMouseDownEvent.which === MouseWhich.LeftClick &&
      !CxMouseUtil.isMousePositionChanged(
        this.lastMouseDownEvent,
        this.lastMouseUpEvent
      )
    ) {
      return true;
    }
    return false;
  }

  private callOnClickedOnly(e: Event) {
    if (this.callOnDblClickedOnlyDelaySubs !== undefined) {
      this.callOnDblClickedOnlyDelaySubs.unsubscribe();
    }
    if (this.onClickedOnly !== undefined && this.wasLastClickedOrTouchOnly()) {
      this.callOnDblClickedOnlyDelaySubs = CxProcessUtil.delay(() => {
        if (this.onClickedOnly !== undefined) {
          this.onClickedOnly(e);
        }
        this.callOnDblClickedOnlyDelaySubs = undefined;
      }, CxMouseUtil.commonWaitForDbClickDelayTime);
    }
  }
}
