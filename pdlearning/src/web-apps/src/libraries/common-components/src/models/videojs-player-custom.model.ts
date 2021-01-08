import { VideojsChapter, VideojsChapterConfig } from '../components/videojs-player/videojs-chapter-plugin';

import { VideoJsPlayer } from 'video.js';

export interface VideojsPlayerCustom extends VideoJsPlayer {
  chapters: (config: VideojsChapterConfig) => void;
  chaptersFn: VideojsChapterFn;
}

export interface VideojsChapterFn {
  add(chapter: VideojsChapter): void;
  addMany(chapters: VideojsChapter[]): void;
  update(chapter: VideojsChapter): void;
  remove(id: string): void;
  removeAll(): void;
  refresh(newChapters: VideojsChapter[]): void;
}
