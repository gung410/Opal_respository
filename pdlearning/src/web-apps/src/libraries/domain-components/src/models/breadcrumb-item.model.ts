export interface BreadcrumbItem {
  text?: string;
  iconClass?: string;
  navigationFn?: () => void;
  textFn?: () => string;
}
