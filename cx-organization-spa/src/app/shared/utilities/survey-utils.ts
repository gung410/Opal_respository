import { environment } from 'app-environments/environment';
import { Md5 } from 'md5-typescript';
import * as moment from 'moment';
import { boxAlternativeIcon } from '../constants/check-box-icon-ag-grid.enum';

export class SurveyUtils {
  static getAvatarFromEmail(
    email: string,
    imageSize: number = 80,
    gravatarType: string = 'mm'
  ): string {
    if (email === null || email === undefined) {
      return '';
    }
    const hash = Md5.init(email.toLowerCase()).toLowerCase();

    return `${environment.gravatarUrl}/${hash}.jpg?s=${imageSize}&d=${gravatarType}`;
  }

  static mapSurveyJSResultToObject(jsonObject: any): {} {
    const result = {};
    const allPropertiesInJson = Object.getOwnPropertyNames(jsonObject);
    allPropertiesInJson.forEach((property) => {
      this.setPropertyValue(result, property, jsonObject[property]);
    });

    return result;
  }

  static mapArrayLocalizedToArrayObject(
    data: any[],
    languageCode: string,
    field: string = 'Name'
  ): any {
    return data.map((item: any) => {
      return this.getPropLocalizedData(item.localizedData, field, languageCode);
    });
  }

  static getPropLocalizedData(
    localizedDatas: any,
    propName: string,
    languageCode: string
  ): string {
    const localizedDataItem = localizedDatas.find(
      (x) => x.languageCode === languageCode
    );
    const fields =
      localizedDataItem !== undefined
        ? localizedDataItem.fields
        : localizedDatas.find(
            (x) => x.languageCode === environment.fallbackLanguage
          );
    for (const field of fields) {
      if (field.name === propName) {
        return field.localizedText;
      }
    }

    return undefined;
  }

  static setPropertyValue(object: any, propertyRoute: string, value: any): any {
    if (typeof object !== 'object' || typeof propertyRoute !== 'string') {
      return;
    }
    const dotIndex = propertyRoute.indexOf('.');
    if (dotIndex >= 0) {
      const fieldName = propertyRoute.substring(0, dotIndex);
      if (!object[fieldName]) {
        object[fieldName] = {};
      }
      const remainingPropertyRoute = propertyRoute.substr(dotIndex + 1);

      return this.setPropertyValue(
        object[fieldName],
        remainingPropertyRoute,
        value
      );
    }
    object[propertyRoute] = value;
  }
}
