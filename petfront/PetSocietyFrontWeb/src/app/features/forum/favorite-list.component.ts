import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ForumService, Article } from './forum.service';

@Component({
  selector: 'app-favorite-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './favorite-list.component.html',
  styleUrl: './favorite-list.component.css'
})
export class FavoriteListComponent implements OnInit {
  favorites: Article[] = [];
  currentMemberId = 20; // 測試用 ID

  constructor(private forumService: ForumService) { }

  ngOnInit(): void {
    this.loadFavorites();
  }

  loadFavorites() {
    this.forumService.getUserFavorites(this.currentMemberId).subscribe(data => {
      this.favorites = data;
    });
  }

  // 取消收藏邏輯
  toggleFavorite(articleId: number) {
    this.forumService.toggleFavorite(articleId, this.currentMemberId).subscribe(res => {
      if (!res.isFavorited) {
        // 如果取消成功，直接從目前的陣列中濾掉，不需要重新刷頁面
        this.favorites = this.favorites.filter(a => a.articleId !== articleId);
      }
    });
  }
  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString();
  }
}
