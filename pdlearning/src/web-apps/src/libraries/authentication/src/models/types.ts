import { Injectable } from '@angular/core';

// tslint:disable:all
/**
 * Additional options that can be passt to tryLogin.
 */
/**
 * @deprecated The class backward compatible with idm, it cloned from cx-angular-common repo.
 */
export class LoginOptions {
  /**
   * Is called, after a token has been received and
   * successfully validated.
   *
   * Deprecated:  Use property ``events`` on OAuthService instead.
   */
  onTokenReceived?: (receivedTokens: ReceivedTokens) => void;

  /**
   * Hook, to validate the received tokens.
   *
   * Deprecated:  Use property ``tokenValidationHandler`` on OAuthService instead.
   */
  validationHandler?: (receivedTokens: ReceivedTokens) => Promise<any>;

  /**
   * Called when tryLogin detects that the auth server
   * included an error message into the hash fragment.
   *
   * Deprecated:  Use property ``events`` on OAuthService instead.
   */
  onLoginError?: (params: object) => void;

  /**
   * A custom hash fragment to be used instead of the
   * actual one. This is used for silent refreshes, to
   * pass the iframes hash fragment to this method.
   */
  customHashFragment?: string;

  /**
   * Set this to true to disable the oauth2 state
   * check which is a best practice to avoid
   * security attacks.
   * As OIDC defines a nonce check that includes
   * this, this can be set to true when only doing
   * OIDC.
   */
  disableOAuth2StateCheck?: boolean;

  /**
   * Normally, you want to clear your hash fragment after
   * the lib read the token(s) so that they are not displayed
   * anymore in the url. If not, set this to true.
   */
  preventClearHashAfterLogin? = false;
}

/**
 * Defines the logging interface the OAuthService uses
 * internally. Is compatible with the `console` object,
 * but you can provide your own implementation as well
 * through dependency injection.
 */
/**
 * @deprecated The class backward compatible with idm, it cloned from cx-angular-common repo.
 */
export abstract class OAuthLogger {
  abstract debug(message?: any, ...optionalParams: any[]): void;
  abstract info(message?: any, ...optionalParams: any[]): void;
  abstract log(message?: any, ...optionalParams: any[]): void;
  abstract warn(message?: any, ...optionalParams: any[]): void;
  abstract error(message?: any, ...optionalParams: any[]): void;
}

/**
 * Defines a simple storage that can be used for
 * storing the tokens at client side.
 * Is compatible to localStorage and sessionStorage,
 * but you can also create your own implementations.
 */
/**
 * @deprecated The class backward compatible with idm, it cloned from cx-angular-common repo.
 */
@Injectable({ providedIn: 'root' })
export class OAuthStorage {
  public storageKeyWhitelist: string[] = [
    'access_token',
    'access_token_stored_at',
    'expires_at',
    'id_token',
    'id_token_claims_obj',
    'id_token_expires_at',
    'id_token_stored_at',
    'session_state',
    'nonce',
    'PKCI_verifier'
  ];
  public memoryStorage: { [key: string]: string } = {};
  public storageType: 'physic' | 'memory' = 'physic';
  public get storage() {
    return localStorage || sessionStorage;
  }

  public getItem(key: string): string | null {
    if (this.checkIfPhysicalStorage(key)) {
      return this.storage.getItem(key);
    } else {
      return this.memoryStorage[key] || null;
    }
  }

  public removeItem(key: string): void {
    if (this.checkIfPhysicalStorage(key)) {
      this.storage.removeItem(key);
    } else {
      delete this.memoryStorage[key];
    }
  }

  public setItem(key: string, data: string): void {
    if (this.checkIfPhysicalStorage(key)) {
      this.storage.setItem(key, data);
    } else {
      this.memoryStorage[key] = data;
    }
  }

  public switchToMemoryStorage(): void {
    this.storageType = 'memory';
    this.memoryStorage = Object.assign({}, localStorage);
    Object.keys(localStorage).forEach(key => this.storageKeyWhitelist.indexOf(key) > -1 && localStorage.removeItem(key));
  }

  private checkIfPhysicalStorage(key: string): boolean {
    return this.storageType === 'physic';
  }
}

/**
 * Represents the received tokens, the received state
 * and the parsed claims from the id-token.
 */
/**
 * @deprecated The class backward compatible with idm, it cloned from cx-angular-common repo.
 */
export class ReceivedTokens {
  idToken: string;
  accessToken: string;
  idClaims?: object;
  state?: string;
}

/**
 * Represents the parsed and validated id_token.
 */
/**
 * @deprecated The class backward compatible with idm, it cloned from cx-angular-common repo.
 */
export interface ParsedIdToken {
  idToken: string;
  idTokenClaims: object;
  idTokenHeader: object;
  idTokenClaimsJson: string;
  idTokenHeaderJson: string;
  idTokenExpiresAt: number;
}

/**
 * Represents the response from the token endpoint
 * http://openid.net/specs/openid-connect-core-1_0.html#TokenEndpoint
 */
/**
 * @deprecated The class backward compatible with idm, it cloned from cx-angular-common repo.
 */
export interface TokenResponse {
  access_token: string;
  id_token: string;
  token_type: string;
  expires_in: number;
  refresh_token: string;
  scope: string;
  state?: string;
}

/**
 * Represents the response from the user info endpoint
 * http://openid.net/specs/openid-connect-core-1_0.html#UserInfo
 */
/**
 * @deprecated The class backward compatible with idm, it cloned from cx-angular-common repo.
 */
export interface UserInfo {
  sub: string;
  [key: string]: any;
}

/**
 * Represents an OpenID Connect discovery document
 */
/**
 * @deprecated The class backward compatible with idm, it cloned from cx-angular-common repo.
 */
export interface OidcDiscoveryDoc {
  issuer: string;
  authorization_endpoint: string;
  token_endpoint: string;
  token_endpoint_auth_methods_supported: string[];
  token_endpoint_auth_signing_alg_values_supported: string[];
  userinfo_endpoint: string;
  check_session_iframe: string;
  end_session_endpoint: string;
  jwks_uri: string;
  registration_endpoint: string;
  scopes_supported: string[];
  response_types_supported: string[];
  acr_values_supported: string[];
  response_modes_supported: string[];
  grant_types_supported: string[];
  subject_types_supported: string[];
  userinfo_signing_alg_values_supported: string[];
  userinfo_encryption_alg_values_supported: string[];
  userinfo_encryption_enc_values_supported: string[];
  id_token_signing_alg_values_supported: string[];
  id_token_encryption_alg_values_supported: string[];
  id_token_encryption_enc_values_supported: string[];
  request_object_signing_alg_values_supported: string[];
  display_values_supported: string[];
  claim_types_supported: string[];
  claims_supported: string[];
  claims_parameter_supported: boolean;
  service_documentation: string;
  ui_locales_supported: string[];
}

export function createDefaultStorage(): Storage | null {
  return typeof localStorage !== 'undefined' ? localStorage : null;
}
