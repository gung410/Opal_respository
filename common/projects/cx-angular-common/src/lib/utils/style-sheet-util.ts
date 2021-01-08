import { CxStringUtil } from './string.util';

export class CxStyleSheetUtil {
    public static addCSSRule(styleSheetId: string | undefined, selector: string, rules: any, fromDocument?: Document): number {
        const sheet: CSSStyleSheet = this.getStyleSheetById(styleSheetId, fromDocument);
        const newRuleIndex: number = this.addRules(sheet, selector, rules);
        return newRuleIndex;
    }

    public static addRules(sheet: CSSStyleSheet, selector: string, rules: any, index: number = 0): number {
        const styles = typeof rules !== 'string' ? this.toStringStyle(rules) : rules;

        if (sheet.insertRule !== undefined && !CxStringUtil.isNullOrEmpty(styles)) {
            const rule: string = selector + ' {' + styles + '}';
            return sheet.insertRule(rule, sheet.cssRules !== undefined ? sheet.cssRules.length : 0);
        }

        if (sheet.addRule !== undefined && !CxStringUtil.isNullOrEmpty(styles)) {
            return sheet.addRule(selector, styles);
        }

        return -1;
    }

    public static updateCssRule(selector: string, rules: any, styleSheetId: string, fromDocument?: Document) {
        const sheet: CSSStyleSheet = this.getStyleSheetById(styleSheetId, fromDocument);
        this.removeExistedRule(sheet, selector);
        const newRuleIndex = this.addRules(sheet, selector, rules, sheet.cssRules.length + 1);
        return newRuleIndex;
    }

    public static removeAllRules(styleSheetId: string, fromDocument?: Document) {
        const sheet: CSSStyleSheet = this.getStyleSheetById(styleSheetId, fromDocument);
        for (let i = 0; i < sheet.cssRules.length; i++) {
            sheet.deleteRule(i);
        }
    }

    public static removeExistedRule(sheet: CSSStyleSheet | string, selector: string) {
        if (typeof sheet === 'string') {
            this.removeExistedRule(this.getStyleSheetById(sheet), selector);
        } else {
            for (let i = 0; i < sheet.cssRules.length; i++) {
                const cssStyleRule: CSSStyleRule = sheet.cssRules[i] as CSSStyleRule;
                if (cssStyleRule.selectorText === selector) {
                    sheet.deleteRule(i);
                    return;
                }
            }
        }
    }

    public static getStyleSheetById(styleSheetId?: string, fromDocument?: Document): CSSStyleSheet {
        if (fromDocument === undefined) {fromDocument = window.document;}
        if (styleSheetId === undefined) { return fromDocument.styleSheets.item(0) as CSSStyleSheet; }
        if (fromDocument.head === undefined) {throw new Error('Document head was undefined');}

        let style = fromDocument.getElementById(styleSheetId.toString()) as HTMLStyleElement;
        if (style === undefined) {
            style = fromDocument.createElement('style');
            style.setAttribute('type', 'text/css');
            style.setAttribute('id', styleSheetId.toString());
            style.appendChild(fromDocument.createTextNode(''));
            fromDocument.head.appendChild(style as Node);
        }

        return style.sheet as CSSStyleSheet;
    }

    public static getStyleSheetCssTextById(styleSheetId?: string, fromDocument?: Document) {
        const styleSheet = this.getStyleSheetById(styleSheetId, fromDocument);
        let cssText = '';
        // tslint:disable-next-line:prefer-for-of
        for (let i = 0; i < styleSheet.cssRules.length; i++) {
            cssText += `${styleSheet.cssRules[i].cssText} `;
        }
        return cssText;
    }

    public static toStringStyle(cssObj: any): string {
        let result = '';
        for (const key in cssObj) {
            if (cssObj.hasOwnProperty(key)) {
                if (typeof cssObj[key] === 'string' && !CxStringUtil.isNullOrEmpty(cssObj[key]))
                    {result += `${key}: ${cssObj[key]};`;}
            }
        }
        return result;
    }

    public static getCurrentDocumentStyleElementsAsString() {
        let result = '';
        const allStyleEls = document.getElementsByTagName('style');
        // tslint:disable-next-line:prefer-for-of
        for (let i = 0; i < allStyleEls.length; i++) {
            result += allStyleEls[i].outerHTML;
        }

        const allLinkEls = document.getElementsByTagName('link');
        // tslint:disable-next-line:prefer-for-of
        for (let i = 0; i < allLinkEls.length; i++) {
            const clonedLinkNode = <HTMLLinkElement>allLinkEls[i].cloneNode(true);
            clonedLinkNode.href = clonedLinkNode.href; // Reset the href again to update href contain the origin in outerHTML
            result += clonedLinkNode.outerHTML;
        }
        return result;
    }
}
