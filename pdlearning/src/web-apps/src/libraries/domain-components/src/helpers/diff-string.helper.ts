import { Diff, DiffMatchPatch, DiffOperation } from 'diff-match-patch-typescript';
export class DiffHelper extends DiffMatchPatch {
  public ignoreFormattedTags: string[] = ['VIDEO', 'AUDIO'];

  /**
   * Return the compare result of two HTML STRING
   *
   * @param htmlString1 The old resource;
   * @param htmlString2 The new resource to compare;
   *
   * @returns obj {result1: string; result2: string}
   *
   */
  public diffTwoHtml(htmlString1: string, htmlString2: string): IDiffResult {
    const diffs = this.getLineDiffs(htmlString1, htmlString2);

    const htmlRightSide: string[] = [];
    const htmlLeftSide: string[] = [];
    for (let x = 0; x < diffs.length; x++) {
      const op = diffs[x][0]; // Operation (insert, delete, equal)
      const data = diffs[x][1]; // Text of change.
      const text = data;
      switch (op) {
        case DiffOperation.DIFF_INSERT:
          htmlLeftSide[x] = '<div class="overlay-layer --add-new --blank">' + text + '</div>';
          htmlRightSide[x] = '<div class="overlay-layer --add-new">' + text + '</div>';
          break;
        case DiffOperation.DIFF_DELETE:
          htmlLeftSide[x] = '<div class="overlay-layer --remove">' + text + '</div>';
          htmlRightSide[x] = '<div class="overlay-layer --remove --blank">' + text + '</div>';
          break;
        case DiffOperation.DIFF_EQUAL:
          htmlRightSide[x] = text;
          htmlLeftSide[x] = text;
          break;
      }
    }

    return <IDiffResult>{
      oldResource: htmlRightSide.join(''),
      newResource: htmlLeftSide.join('')
    };
  }

  public getDiffString(string1: string, string2: string): IDiffResult {
    if (!string1 && string2) {
      return <IDiffResult>{
        oldResource: `<div class="overlay-layer --add-new --blank"> ${string2} </div>`,
        newResource: `<div class="overlay-layer --add-new> ${string2} </div>`
      };
    }

    if (string1 && !string2) {
      return <IDiffResult>{
        oldResource: `<div class="overlay-layer --remove"> ${string1} </div>`,
        newResource: `<div class="overlay-layer --remove --blank"> ${string1} </div>`
      };
    }

    const oldResource =
      `<div class="overlay-layer --remove"> ${string1} </div>` + '\n' + `<div class="overlay-layer --add-new --blank"> ${string2} </div>`;
    const newResource =
      `<div class="overlay-layer --remove --blank"> ${string1} </div>` + '\n' + `<div class="overlay-layer --add-new"> ${string2} </div>`;

    return <IDiffResult>{
      oldResource: oldResource,
      newResource: newResource
    };
  }

  public diffString(string1: string, string2: string): string {
    const diffs = this.diff_main(string1, string2, false);
    return this.diff_prettyHtml(diffs);
  }

  public getLineDiffs(text1: string, text2: string): Diff[] {
    text1 = this.getFormattedHtml(text1 ? text1.replace(/\n/gi, '') : text1);
    text2 = this.getFormattedHtml(text2 ? text2.replace(/\n/gi, '') : text2);

    const thisProto = Object.getPrototypeOf(this);
    const a = thisProto.diff_linesToChars_(text1, text2);
    const lineText1 = a.chars1;
    const lineText2 = a.chars2;
    const lineArray = a.lineArray;
    const diffs = this.diff_main(lineText1, lineText2, false);
    thisProto.diff_charsToLines_(diffs, lineArray);
    return diffs;
  }

  private getFormattedHtml(str: string): string {
    const div = document.createElement('div');
    if (str) {
      div.innerHTML = str.trim();
    } else {
      div.innerHTML = '';
    }

    return this.format(div, 0).innerHTML;
  }

  private format(node: Element, level: number): Element {
    const indentBefore = new Array(level++ + 1).join('  ');
    const indentAfter = new Array(level - 1).join('  ');

    let textNode;

    for (let i = 0; i < node.children.length; i++) {
      if (this.ignoreFormattedTags.includes(node.children[i].tagName)) {
        continue;
      }

      textNode = document.createTextNode('\n' + indentBefore);
      node.insertBefore(textNode, node.children[i]);
      this.format(node.children[i], level);

      if (node.lastElementChild === node.children[i]) {
        textNode = document.createTextNode('\n' + indentAfter);
        node.appendChild(textNode);
      }
    }
    return node;
  }
}

export interface IDiffIndex {
  index1: number;
  index2: number;
}

export interface IDiffResult {
  oldResource: string;
  newResource: string;
}
