export class ClipboardUtil {
  public static copyTextToClipboard(text: string): void {
    const clipboard = navigator.clipboard;
    if (!clipboard) {
      fallbackCopyTextToClipboard(text);
      return;
    }
    clipboard.writeText(text);

    function fallbackCopyTextToClipboard(value: string): void {
      const textArea = document.createElement('textarea');
      textArea.value = value;
      textArea.style.position = 'absolute';
      textArea.style.left = '-99999px';
      document.body.appendChild(textArea);
      textArea.focus();
      if (navigator.userAgent.match(/ipad|ipod|iphone/i)) {
        const el = textArea;
        el.contentEditable = 'true';
        el.readOnly = false;
        const range = document.createRange();
        range.selectNodeContents(el);
        const sel = window.getSelection();
        sel.removeAllRanges();
        sel.addRange(range);
        el.setSelectionRange(0, 999999);
      } else {
        textArea.select();
      }
      document.execCommand('copy');
      document.body.removeChild(textArea);
    }
  }
}
