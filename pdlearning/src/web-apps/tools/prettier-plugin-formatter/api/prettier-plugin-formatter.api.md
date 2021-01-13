## API Report File for "@thunder/prettier-plugin-formatter"

> Do not edit this file. It is a report generated by [API Extractor](https://api-extractor.com/).

```ts

import { Parser } from 'prettier';

// @public (undocumented)
export interface IPrettierParser {
    // (undocumented)
    [parserName: string]: {
        preprocess: (text: string) => string;
    } & Parser;
}

// @public (undocumented)
export const parsers: IPrettierParser;

// @public (undocumented)
export class PrettierImportPlugin {
    // (undocumented)
    readonly parsers: IPrettierParser;
    }


// (No @packageDocumentation comment for this package)

```