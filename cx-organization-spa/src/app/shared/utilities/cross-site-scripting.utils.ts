export class CrossSiteScriptingUtil {
  static encodeHtmlEntity(html: string): string {
    if (!html || !html.length) {
      return '';
    }

    const encodingMapping: Map<RegExp, string> = new Map([
      [/&/g, '&amp;'],
      [/</g, '&lt;'],
      [/>/g, '&gt;'],
      [/\"/g, '&quot;'],
      [/\'/g, '&#x27;'],
      [/\//g, '&#x2F;']
    ]);

    encodingMapping.forEach((value, regex) => {
      html = html.replace(regex, value);
    });

    return html;
  }
}
