export class CxClipboardUtil {
    public static copyTextToClipboard(text: string) {
        const clipboard = (navigator as any).clipboard;
        if (!clipboard) {
            fallbackCopyTextToClipboard(text);
            return;
        }
        clipboard.writeText(text);

        function fallbackCopyTextToClipboard(textInput: string) {
            const textArea = document.createElement('textarea');
            textArea.value = textInput;
            textArea.style.display = 'none';
            document.body.appendChild(textArea);
            textArea.focus();
            textArea.select();
            document.execCommand('copy');
            document.body.removeChild(textArea);
        }
    }
}