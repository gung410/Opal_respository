// Navigation.
export const NAVIGATION_PARAMETERS_KEY: string = 'fw.navigation-parameters';
export const MODULE_INPUT_DATA: string = 'fw.module-input-data';

// App storage;
export const APP_LOCAL_STORAGE_KEY: string = 'fw.app-local-storage';
export const APP_SESSION_STORAGE_KEY: string = 'fw.app-session-storage';

// Interceptors
export const FW_INTERCEPTOR_KEYS: string = 'FW-Interceptor-Keys';
export const FW_DISPLAY_SPINNER: string = 'FW-Display-Spinner';

// Value of type variables
export const MAX_INT: number = 0x7fffffff; // equals: 2147483647;
export const TIME_HIDDEN_NOTIFICATION: number = 5000;

// z-index variables: mapping with _variables.scss
export const ZINDEX_LEVEL_1: number = 1;
export const ZINDEX_LEVEL_2: number = 1000;
export const ZINDEX_LEVEL_3: number = 2000;
export const ZINDEX_LEVEL_4: number = 3000;
export const ZINDEX_LEVEL_5: number = 4000;

// For issue: When kendo tooltip show in dialog => z-index of it is '100002'. If you open another dialog, you must set z-index of it equal this variable.
export const MAX_ZINDEX_OF_TOOLTIP: number = 100002;
