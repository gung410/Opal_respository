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
      case 'PKCI_verifier':
        localStorage.removeItem('PKCI_verifier');
      default:
        delete this[key];
    }
  }
  setItem(key: string, data: string): void {
    switch (key) {
      case 'nonce':
        localStorage.setItem('nonce', data);
      case 'PKCI_verifier':
        localStorage.setItem('PKCI_verifier', data);
      default:
        this[key] = data;
    }
  }
}
