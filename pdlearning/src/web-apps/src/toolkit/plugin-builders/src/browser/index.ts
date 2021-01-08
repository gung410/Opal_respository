import { BrowserBuilderOutput, executeBrowserBuilder } from '@angular-devkit/build-angular';
import { BuilderContext, createBuilder } from '@angular-devkit/architect';

import { CustomWebpackBrowserSchema } from '@angular-builders/custom-webpack';
import { IPluginBuilderSchema } from './plugin-builder-schema';
import { Observable } from 'rxjs';
import { getTransforms } from '@angular-builders/custom-webpack/dist/common';
import { json } from '@angular-devkit/core';

export type PluginBuilderSchema = CustomWebpackBrowserSchema & IPluginBuilderSchema;

export function buildCustomWebpackBrowser(options: PluginBuilderSchema, context: BuilderContext): Observable<BrowserBuilderOutput> {
  return executeBrowserBuilder(options, context, getTransforms(options, context));
}

export default createBuilder<json.JsonObject & PluginBuilderSchema>(buildCustomWebpackBrowser);
