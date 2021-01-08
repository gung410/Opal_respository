import { ArchetypeEnum } from 'app-enums/archetypeEnum';

export class UserCounting {
  archetype: ArchetypeEnum;
  archetypeId: number;
  userTypeId: number;
  userTypeExtId: string;
  userTypeName: string;
  /**
   * The number of users in the group of the user type.
   */
  count: number;
  constructor(data?: Partial<UserCounting>) {
    if (!data) {
      return;
    }

    this.archetype = data.archetype;
    this.archetypeId = data.archetypeId;
    this.userTypeId = data.userTypeId;
    this.userTypeExtId = data.userTypeExtId;
    this.userTypeName = this.userTypeName;
  }
}
