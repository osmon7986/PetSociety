import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import Swal from 'sweetalert2';
import { MemberService } from '../../services/member.service';
import { FavoriteItemDto, FavoriteType } from '../../Interfaces/favorite.dto';

@Component({
  selector: 'app-wishlist',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './wishlist.component.html',
  styleUrls: ['./wishlist.component.css']
})
export class WishlistComponent implements OnInit {

  activeTab: string = 'product';
  favorites: FavoriteItemDto[] = [];
  isFavoriteLoading: boolean = false;
  getGoShopLink(): string {
    switch (this.activeTab) {

      // 1. 商城 -> 對應 path: 'mall'
      case 'product':
        return '/mall';

      // 2. 活動 -> 對應 path: 'activity' -> 'intro'
      case 'activity':
        return '/activity/intro';

      // 3. 課程 -> 對應 path: 'academy'
      case 'course':
        return '/academy';

      // 4. 論壇 -> 對應 path: 'forum'
      case 'article':
        return '/forum';

      // 預設回首頁
      default:
        return '/';
    }
  }

  // 為了在 HTML 使用 Enum (雖然後端回傳的是數字，但保持對照比較好維護)
  FavoriteType = FavoriteType;
  private readonly imgBaseUrl = 'https://localhost:7138/images/';
  constructor(
    private memberService: MemberService,
    private router: Router

  ) { }

  ngOnInit(): void {
    this.loadFavorites();
  }

  // --- 切換分頁 ---
  onTabChange(tab: string) {
    this.activeTab = tab;
    this.loadFavorites();
  }

  // --- 讀取收藏列表 ---
  loadFavorites() {
    this.isFavoriteLoading = true;
    this.favorites = [];

    let typeId: number;
    switch (this.activeTab) {
      case 'product': typeId = 1; break;
      case 'activity': typeId = 2; break;
      case 'course': typeId = 3; break;
      case 'article': typeId = 4; break;
      default: typeId = 1;
    }

    this.memberService.getFavorites(typeId).subscribe({
      next: (data) => {
        this.favorites = data;
        this.isFavoriteLoading = false;
      },
      error: (err) => {
        console.error('讀取收藏失敗', err);
        this.isFavoriteLoading = false;
      }
    });
  }

  // --- 移除收藏 (Wishlist 頁面專用) ---
  onRemoveFavorite(favoriteId: number, event: Event) {
    event.stopPropagation();

    Swal.fire({
      title: '確定要移除嗎？',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: '移除',
      cancelButtonText: '取消',
      confirmButtonColor: '#D8C3A5',
      cancelButtonColor: '#8E8375',
    }).then((result) => {
      if (result.isConfirmed) {

        // 先把目前的狀態備份起來
        const previousState = [...this.favorites];

        // 1. 不等後端，直接從畫面上移除
        this.favorites = this.favorites.filter(f => f.favoriteId !== favoriteId);

        // 2. 背景偷偷呼叫後端
        this.memberService.removeFavorite(favoriteId).subscribe({
          next: () => {
            console.log('後端同步刪除成功');
          },
          error: (err) => {
            // 3. 萬一後端失敗，把卡片救回來
            this.favorites = previousState;
            Swal.fire('移除失敗', '網路錯誤', 'error');
          }
        });
      }
    });
  }

  // --- 點擊卡片跳轉到詳情頁 ---
  goToDetail(item: FavoriteItemDto) {
    switch (item.targetType) {
      case 1: // 商城
        this.router.navigate(['/mall']); // 商城還沒做詳情頁，先回商城首頁
        // this.router.navigate(['/mall/product', item.targetId]);
        break;

      case 2: // 活動
        this.router.navigate(['/activity/intro']);
        break;

      case 3: // 課程
        this.router.navigate(['/academy/course', item.targetId]);
        break;

      case 4: // 文章
        this.router.navigate(['/forum']);
        break;

      default:
        console.warn('未知的收藏類型', item);
    }
  }

  getCourseImage(filename: string | undefined): string {
    // 1. 如果沒資料，回傳前端的預設圖
    if (!filename) {
      return 'assets/images/placeholder.png';
    }

    // 2. 如果資料庫已經存了完整網址 (http開頭)，直接用
    if (filename.startsWith('http')) {
      return filename;
    }

    // 3. 如果只是檔名 (course2.png)，拼接後端路徑
    return `${this.imgBaseUrl}${filename}`;
  }

}
