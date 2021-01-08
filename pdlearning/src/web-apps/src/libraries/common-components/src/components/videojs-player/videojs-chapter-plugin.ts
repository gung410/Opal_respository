import * as videoJS from 'video.js';

import { Utils } from '@opal20/infrastructure';
import { VideojsPlayerCustom } from '../../models/videojs-player-custom.model';

const videojs = videoJS.default;
export interface VideojsChapterConfig {
  chapters: VideojsChapter[];
  onChapterChange?: (newChapter: VideojsChapter, oldChapter: VideojsChapter) => void;
}

export interface VideojsChapter {
  id: string;
  startTime: number;
  endTime: number;
  title: string;
}

// default setting
const defaultSetting: VideojsChapterConfig = {
  chapters: [],
  onChapterChange: () => {
    // no implementation
  }
};

const NULL_INDEX = -1;

export function registerVideoJsChaptersPlugin(options: VideojsChapterConfig): void {
  /**
   * register the chapters plugin
   */
  let currentChapter: VideojsChapter;
  const setting = videojs.mergeOptions(defaultSetting, options);
  const chaptersList: VideojsChapter[] = []; // list of chapters sorted by time
  const player: VideojsPlayerCustom = this;

  function getPosition(chapter: VideojsChapter): number {
    return (chapter.startTime / player.duration()) * 100;
  }

  function getWidth(chapter: VideojsChapter): number {
    if (chapter.endTime === Math.floor(player.duration())) {
      return 100 - getPosition(chapter);
    }
    return ((chapter.endTime - chapter.startTime) / player.duration()) * 100;
  }

  function sortChaptersList(): void {
    // sort the list by time in asc order
    chaptersList.sort((a, b) => {
      return a.startTime - b.startTime;
    });
  }

  function setChapterDivStyle(chapter: VideojsChapter, chapterDiv: HTMLElement): void {
    chapterDiv.className = 'vjs-chapter';
    chapterDiv.style.left = getPosition(chapter) + '%';
    chapterDiv.style.width = getWidth(chapter) + '%';

    const chapterProgressDiv = videojs.dom.createEl('div', {});
    chapterProgressDiv.className = 'vjs-chapter-progress';
    chapterDiv.appendChild(chapterProgressDiv);

    const chapterNameDiv = videojs.dom.createEl('div', {});
    chapterNameDiv.className = 'vjs-chapter-name';
    chapterNameDiv.innerHTML = chapter.title;
    chapterDiv.appendChild(chapterNameDiv);
  }

  function createChapterDiv(chapter: VideojsChapter): Element {
    const chapterDiv = videojs.dom.createEl(
      'div',
      {},
      {
        'data-chapter-id': chapter.id
      }
    ) as HTMLElement;

    setChapterDivStyle(chapter, chapterDiv);

    return chapterDiv;
  }

  function addChapter(chapter: VideojsChapter, index?: number): void {
    player
      .el()
      .querySelector('.vjs-chapters')
      .appendChild(createChapterDiv(chapter));

    if (index) {
      chaptersList.splice(index, 0, chapter);
    } else {
      chaptersList.push(chapter);
    }
  }

  function addChapters(newChapters: VideojsChapter[]): void {
    newChapters.forEach((chapter: VideojsChapter) => {
      addChapter(chapter);
    });

    sortChaptersList();
  }

  function updateChapter(newChapter: VideojsChapter): void {
    const oldChapterIndex = chaptersList.findIndex(p => p.id === newChapter.id);
    if (oldChapterIndex > -1 && Utils.isDifferent(chaptersList[oldChapterIndex], newChapter)) {
      removeChapter(chaptersList[oldChapterIndex].id);
      addChapter(newChapter, oldChapterIndex);
    }
  }

  function removeChapter(id: string): void {
    const chapterIndex = chaptersList.findIndex(p => p.id === id);
    if (chapterIndex !== NULL_INDEX) {
      chaptersList.splice(chapterIndex, 1);
    }

    const el = player.el().querySelector(".vjs-chapter[data-chapter-id='" + id + "']");
    if (el) {
      el.parentNode.removeChild(el);
    }
  }

  function removeAll(): void {
    chaptersList.forEach(p => {
      removeChapter(p.id);
    });
  }

  function refresh(newChapters: VideojsChapter[]): void {
    removeAll();
    addChapters(newChapters);
  }

  function onTimeUpdate(): void {
    const timestamp = player.currentTime();
    updateCurrentChapter(timestamp);
  }

  function updateCurrentChapter(timestamp: number): void {
    if (currentChapter && isTimestampInChapter(timestamp, currentChapter)) {
      return;
    }
    const newCurrentChapter = chaptersList.find(p => isTimestampInChapter(timestamp, p));
    if (newCurrentChapter != null) {
      setting.onChapterChange(newCurrentChapter, currentChapter);
      currentChapter = newCurrentChapter;
    }
  }

  function isTimestampInChapter(timestamp: number, chapter: VideojsChapter): boolean {
    return chapter.startTime <= timestamp && chapter.endTime > timestamp;
  }

  // setup the whole thing
  function initialize(): void {
    // remove existing chapters if already initialized
    removeAll();
    const chapterDiv = videojs.dom.createEl('div', {}, {});
    chapterDiv.className = 'vjs-chapters';
    player
      .el()
      .querySelector('.vjs-progress-control')
      .appendChild(chapterDiv);

    addChapters(setting.chapters);

    onTimeUpdate();
    player.on('timeupdate', onTimeUpdate);
    player.off('loadedmetadata');
  }

  // setup the plugin after we loaded video's meta data
  player.on('loadedmetadata', () => {
    initialize();
  });

  // exposed plugin API
  player.chaptersFn = {
    add(chapter: VideojsChapter): void {
      addChapter(chapter);
    },
    addMany(newChapters: VideojsChapter[]): void {
      addChapters(newChapters);
    },
    update(chapter: VideojsChapter): void {
      updateChapter(chapter);
    },
    remove(id: string): void {
      removeChapter(id);
    },
    removeAll(): void {
      removeAll();
    },
    refresh(newChapters: VideojsChapter[]): void {
      refresh(newChapters);
    }
  };
}
