import { ChapterPlayback } from "./chapter-playback";

export interface CoursePlayback {
  courseDetailId: number,
  courseTitle: string,
  currentChapter?: ChapterPlayback,
  chapters?: ChapterPlayback[],
}
