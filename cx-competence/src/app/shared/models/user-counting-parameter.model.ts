import { ArchetypeEnum } from 'app-enums/archetypeEnum';

export class UserCountingParameter {
  userTypeArchetypes?: ArchetypeEnum[];
  userGroupIds?: number[];
  userIds?: number[];
  constructor(data?: Partial<UserCountingParameter>) {
    if (!data) {
      return;
    }

    this.userTypeArchetypes = data.userTypeArchetypes;
    this.userGroupIds = data.userGroupIds;
    this.userIds = data.userIds;
  }
}
