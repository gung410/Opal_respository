import { throttle } from 'lodash';

export function debounce(delay: number = 300) {
  return (target: any, propertyKey: string, descriptor: PropertyDescriptor) => {
    let timeout = null;

    const original = descriptor.value;

    descriptor.value = function(...args) {
      clearTimeout(timeout);
      timeout = setTimeout(() => original.apply(this, args), delay);
    };

    return descriptor;
  };
}

export function cxThrottle(milliseconds: number = 0, options = {}): any {
  return (target: any, propertyKey: string, descriptor: PropertyDescriptor) => {
    const originalMethod = descriptor.value;
    descriptor.value = throttle(originalMethod, milliseconds, options);
    return descriptor;
  };
}
