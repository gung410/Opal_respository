import { Injectable } from '@angular/core';
import { JwksValidationHandler } from './jwks-validation-handler';

// tslint:disable:all
/**
 * Abstraction for crypto algorithms
 */
/**
 * @deprecated The class backward compatible with idm, it cloned from cx-angular-common repo.
 */
@Injectable({ providedIn: 'root', useClass: JwksValidationHandler })
export abstract class CryptoHandler {
  abstract calcHash(valueToHash: string, algorithm: string): Promise<string>;
}
