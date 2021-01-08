import * as path from 'path';

import { merge } from 'lodash';
import { writeFileSync } from 'fs';

interface ICustomSchema {
  originalSchemaPath: string;
  schemaExtensionPaths: string[];
  newSchemaPath: string;
}

// Merge schemas
const schemesToMerge: ICustomSchema[] = require(path.join(__dirname, 'src', 'schemas'));

for (const customSchema of schemesToMerge) {
  const originalSchema: object = require(customSchema.originalSchemaPath);
  const schemaExtensions: object[] = customSchema.schemaExtensionPaths.map((filePath: string) => require(filePath));
  const newSchema: object = schemaExtensions.reduce(
    (extendedSchema: object, currentExtension: object) => merge(extendedSchema, currentExtension),
    originalSchema
  );
  writeFileSync(customSchema.newSchemaPath, JSON.stringify(newSchema, null, 2), 'utf-8');
}
