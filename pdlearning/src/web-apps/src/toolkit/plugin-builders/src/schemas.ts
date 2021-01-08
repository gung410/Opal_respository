module.exports = [
  {
    originalSchemaPath: '@angular-builders/custom-webpack/dist/browser/schema.json',
    schemaExtensionPaths: [`${__dirname}/browser/schema.ext.json`],
    newSchemaPath: `${__dirname}/../lib/browser/schema.json`
  }
];
