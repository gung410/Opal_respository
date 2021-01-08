import { TranslationMessage } from '@opal20/infrastructure';
import { Type } from '@angular/core';

export interface INavigationMenuItem {
  id: string;
  parentId?: string;
  component?: Type<unknown>;
  name?: string | TranslationMessage;
  isActivated?: boolean;
  data?: unknown | INavigationMenuItem[];
  subData?: INavigationMenuItem[];
  menuType?: 'learner-catalog';
  onClick?: (item: INavigationMenuItem) => void;
}
