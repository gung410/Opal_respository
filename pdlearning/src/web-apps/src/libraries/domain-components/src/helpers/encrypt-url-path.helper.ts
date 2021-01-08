import { AES, enc, mode } from 'crypto-js';

export class EncryptUrlPathHelper {
  public static encrypt(value: string): string {
    const secretKey = enc.Utf8.parse(AppGlobal.environment.AESSecretKey);
    const _iv = enc.Utf8.parse(AppGlobal.environment.AESIv);
    return AES.encrypt(value, secretKey, {
      keySize: 128,
      iv: _iv,
      mode: mode.CBC
    }).toString();
  }
}
