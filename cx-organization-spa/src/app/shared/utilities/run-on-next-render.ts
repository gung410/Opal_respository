export function RunOnNextRender(callBack: () => void, timer: number = 0): void {
  setTimeout(callBack, timer);
}
