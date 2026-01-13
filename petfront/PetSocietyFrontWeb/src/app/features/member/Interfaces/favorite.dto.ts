// 定義收藏類型的 Enum (對應資料庫的 TargetType)
export enum FavoriteType {
  Product = 1,
  Activity = 2,
  Course = 3,
  Article = 4
}

// 收藏項目的 DTO (用來顯示列表)
export interface FavoriteItemDto {
  favoriteId: number;
  targetId: number;
  targetType: number;

  // 快照欄位
  title: string;
  imageUrl: string;
  description?: string;
  price?: number;
  date?: string;
}
