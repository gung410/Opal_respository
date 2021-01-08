import * as Survey from 'survey-angular';

export class CxSurveyJsUtil {
    public static addProperty(surveyJson: any, fieldName: string, propertyName: string, value: any) {
        if (surveyJson.pages) {
            surveyJson.pages.forEach(page => {
                this.addProperty(page, fieldName, propertyName, value);
            });
        } else if (surveyJson.elements) {
            surveyJson.elements.forEach(element => {
                if (element.elements) {
                    this.addProperty(element, fieldName, propertyName, value);
                } else {
                    if (element.name === fieldName) {
                        element[propertyName] = value;
                        return;
                    }
                }
            });
        }
    }
    public static initDefaultValue(surveyJson: any, dataJson: any) {
        if (surveyJson.pages) {
            surveyJson.pages.forEach(page => {
                this.initDefaultValue(page, dataJson);
            });
        } else if (surveyJson.elements) {
            surveyJson.elements.forEach(element => {
                if (element.elements) {
                    this.initDefaultValue(element, dataJson);
                } else {
                    if (dataJson.hasOwnProperty(element.name)) {
                        element.defaultValue = dataJson[element.name];
                    }
                }
            });
        }
    }
}
