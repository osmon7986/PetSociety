import { ActivityApply } from "./activityApply";
import { CreateGuideData } from "./activityGuideData";

export interface CreateActivityPayload {
  applyData: ActivityApply;
  guideData: CreateGuideData;
}
