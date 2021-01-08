export class PlayerHelpers {
  public static notAllowedRightClickTags = ['AUDIO', 'VIDEO', 'IMG'];

  public static preventRightClick(e: MouseEvent): void {
    if (PlayerHelpers.notAllowedRightClickTags.includes(((e.target as unknown) as { tagName: string }).tagName)) {
      e.preventDefault();
    }
  }
}
