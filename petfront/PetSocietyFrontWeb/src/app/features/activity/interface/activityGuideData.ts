export interface ActivityGuideData {
  activtyId: number;
  guideId: number;
  activityEditorHtml: string;
}

export interface CreateGuideData {
  activityId?: number;
  activityEditorHtml: string;
}
