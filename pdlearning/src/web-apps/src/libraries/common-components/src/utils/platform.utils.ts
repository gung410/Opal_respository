export const IOS_PLATFORM_REGEX: RegExp = /iPad|iPhone|iPod/;

export class PlatformHelper {
  public static isIOSDevice(): boolean {
    return (
      !!navigator.platform && (IOS_PLATFORM_REGEX.test(navigator.platform) || /^((?!chrome|android).)*safari/i.test(navigator.userAgent))
    );
  }

  public static setMobileViewport(): void {
    const viewPortEl = document.querySelector('meta[name=viewport]');
    viewPortEl.setAttribute(
      'content',
      'width=device-width, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0, user-scalable=no, shrink-to-fit=no, viewport-fit=cover'
    );
  }
}
