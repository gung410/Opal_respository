import { Pipe, PipeTransform } from '@angular/core';
/*
 * Generate the key which is transformed from a title. This could be usefull when we need to render the tab component.
 * Usage:
 *   value
 * Example:
 *   {{ 'Professional Development (PD) Plan' | titleAsKey }}
 *   formats to: 'professional-development--pd--plan'
 */
@Pipe({ name: 'titleAsKey' })
export class TitleAsKeyPipe implements PipeTransform {
  transform(value: string): string {
    const separator = '-';
    return (
      value
        // .replace(/\s/g, separator)  // Replace with spaces between words.
        .replace(/\W/g, separator) // Replace non-word characters by the separator.
        .toLowerCase() // Lower case the key.
    );
  }
}
