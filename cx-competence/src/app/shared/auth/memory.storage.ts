import { OAuthStorage } from '@conexus/cx-angular-common';

export class InMemoryStorage extends OAuthStorage {
  getItem(key: string): string {
    switch (key) {
      case 'nonce':
        return localStorage.getItem('nonce');
      case 'PKCI_verifier':
        return localStorage.getItem('PKCI_verifier');
      default:
        return this[key];
    }
  }

  removeItem(key: string): void {
    switch (key) {
      case 'nonce':
        localStorage.removeItem('nonce');
        break;
      case 'PKCI_verifier':
        localStorage.removeItem('PKCI_verifier');
        break;
      default:
        delete this[key];
        break;
    }
  }

  setItem(key: string, data: string): void {
    switch (key) {
      case 'nonce':
        localStorage.setItem('nonce', data);
        break;
      case 'PKCI_verifier':
        localStorage.setItem('PKCI_verifier', data);
        break;
      default:
        this[key] = data;
        break;
    }
  }
}
