export function createDefaultLogger() {
    return console;
}

export function createDefaultStorage() {
    return typeof localStorage !== 'undefined' ? localStorage : null;
}
