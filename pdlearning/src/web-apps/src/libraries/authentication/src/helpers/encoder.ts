import { HttpParameterCodec } from '@angular/common/http';
// tslint:disable:all
/**
 * This custom encoder allows charactes like +, % and / to be used in passwords
 */
/**
 * @deprecated The class backward compatible with idm, it cloned from cx-angular-common repo.
 */
export class WebHttpUrlEncodingCodec implements HttpParameterCodec {
  encodeKey(k: string): string {
    return encodeURIComponent(k);
  }

  encodeValue(v: string): string {
    return encodeURIComponent(v);
  }

  decodeKey(k: string): string {
    return decodeURIComponent(k);
  }

  decodeValue(v: string) {
    return decodeURIComponent(v);
  }
}
