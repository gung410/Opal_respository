export class SystemError extends Error {
  constructor(message?: string) {
    super(message);

    // We only support IE11+
    // https://stackoverflow.com/questions/41102060/typescript-extending-error-class
    // https://www.typescriptlang.org/docs/handbook/release-notes/typescript-2-2.html
    Object.setPrototypeOf(this, new.target.prototype);
  }
}

export class ModuleNotFoundError extends SystemError {
  constructor(message?: string) {
    super(message);
  }
}

export class NavigationError extends SystemError {
  constructor(message?: string) {
    super(message);
  }
}

export class AuthenticationError extends SystemError {
  constructor(message?: string) {
    super(message);
  }
}
