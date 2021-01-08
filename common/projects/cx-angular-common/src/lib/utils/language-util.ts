import { Dictionary } from '../../typings';

export class CxLanguageUtil {
    public static getBrowserLang() {
        return window.navigator.language.substr(0, 2);
    }

    public static getBrowserLangTranslatedText(value: Dictionary<string>, defaultLang: string) {
        const browserLang = CxLanguageUtil.getBrowserLang();
        return value[browserLang] !== undefined
            ? value[browserLang]
            : value[defaultLang] !== undefined
                ? value[defaultLang]
                : '';
    }
}
