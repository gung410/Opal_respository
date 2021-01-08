export class PDCatalogueEnumerationDto {
  id: string;
  humanCodingScheme: string;
  fullStatement: string;
  note: string;
  type: string;
  typeId: string;
  associationItems: AssociationItemDto[];
  displayText: string;
  constructor(data?: any) {
    if (!data) {
      return;
    }
    this.id = data.id;
    this.humanCodingScheme = data.humanCodingScheme;
    this.fullStatement = data.fullStatement;
    this.note = data.note;
    this.type = data.type;
    this.typeId = data.typeId;
    this.associationItems = data.associationItems ? data.associationItems : [];
    this.displayText = data.displayText;
  }
}

// tslint:disable-next-line:max-classes-per-file
export class AssociationItemDto {
  association: string;
  items: AssociationItemDto[];
  constructor(data?: any) {
    if (!data) {
      return;
    }
    this.association = data.association;
    this.items = data.items ? data.items : [];
  }
}
