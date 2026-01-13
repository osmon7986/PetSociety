import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

// 文章資料介面 (確保欄位與後端 DTO 對齊)
export interface Article {
  articleId: number;
  categoryName: string;
  tagName: string;
  memberName: string;
  categoryId: number; // 保留 ID 供發文/編輯使用
  tagId: number;
  memberId: number;
  title: string;
  content: string;
  postDate: Date;
  lastUpdate: Date;
  like: number;
  disLike: number;
  popular: number;
  commentCount: number;
  picture: string | null;
  userVote?: 'like' | 'dislike' | null;  // 記錄當前使用者是否按過讚或倒讚
  isFavorited: boolean;  // 記錄當前使用者是否收藏
}

// 定義留言資料結構 (新增此介面供 Service 使用)
export interface Comment {
  commentId: number;
  articleId: number;
  memberId: number;
  memberName?: string;
  content: string;
  postDate: Date;
}
// 分類與標籤介面
export interface Category {
  categoryId: number;
  categoryName: string;
}

export interface Tag {
  tagId: number;
  tagName: string

}

// 分頁回傳結果的包裝介面
export interface PagedResult<T> {
  totalCount: number;
  pageSize: number;
  currentPage: number;
  items: T[]; // 實際的資料陣列
}

@Injectable({
  providedIn: 'root' // 讓service在全域可以使用
})

export class ForumService {
  // 設定後端 API 基礎路徑
  private readonly apiUrl = 'https://localhost:7138/api/Articles';
  private readonly commentUrl = 'https://localhost:7138/api/Comments';

  constructor(private httpClient: HttpClient) { } // 注入 HttpClient 以便發送 HTTP 請求

  // 取得所有會員清單，用於下拉選單或搜尋建議
  getMembers(): Observable<any[]> {
    return this.httpClient.get<any[]>(`https://localhost:7138/api/Articles/Members`);
  }

  // 1. 取得文章列表 (支援分頁、分類、標籤、關鍵字搜尋)
  getRecentArticles(page: number = 1, pageSize: number = 5, categoryId: number | null = null, tagId: number | null = null,
    keyword: string = '', memberId: number = 20): Observable<PagedResult<Article>> {
    // 建立 HttpParams 物件來組合查詢參數
    let params = new HttpParams()
      .set('page', page.toString()) // 建立請求參數，轉成字串
      .set('pageSize', pageSize.toString())
      .set('memberId', memberId.toString());

    // 如果有選擇分類、標籤，就把 categoryId、tagId 串上去
    if (categoryId && categoryId > 0) {
      params = params.set('categoryId', categoryId.toString());
    }
    if (tagId && tagId > 0) {
      params = params.set('tagId', tagId.toString());
    }
    // 加入關鍵字查詢參數
    if (keyword && keyword.trim() !== '') {
      params = params.set('keyword', keyword.trim()); // 去除多餘空白
    }
    // 發送請求時，將 params 物件傳入 options 裡
    return this.httpClient.get<PagedResult<Article>>(this.apiUrl, { params });
  }

  // 2. 取得單一文章細節，傳入文章 ID
  // getArticleById(id: number): Observable<Article> {
  //   return this.httpClient.get<Article>(`${this.apiUrl}/${id}`);
  // }
  getArticleById(id: number, memberId: number = 20): Observable<Article> {
    return this.httpClient.get<Article>(`${this.apiUrl}/${id}?memberId=${memberId}`);
  }

  // 3. 建立新文章，傳入部分 Article 物件
  createNewArticle(payload: Partial<Article>): Observable<Article> {
    return this.httpClient.post<Article>(this.apiUrl, payload);
  }

  // 4. 修改文章，傳入完整的 Article 物件
  editArticle(payload: Article): Observable<void> {
    return this.httpClient.put<void>(`${this.apiUrl}/${payload.articleId}`, payload);
  }

  // 5. 刪除文章，傳入文章 ID
  deleteArticle(id: number): Observable<void> {
    return this.httpClient.delete<void>(`${this.apiUrl}/${id}`);
  }

  // 6. 按讚 (呼叫後端 API，回傳更新後的讚數與人氣)
  likeArticle(id: number, action: 'add' | 'remove' | 'switch'): Observable<any> {
    return this.httpClient.post(`${this.apiUrl}/${id}/like`, { action });
  }

  // 7. 倒讚 (呼叫後端 API，回傳更新後的倒讚數與人氣)
  dislikeArticle(id: number, action: 'add' | 'remove' | 'switch'): Observable<any> {
    return this.httpClient.post(`${this.apiUrl}/${id}/dislike`, { action });
  }

  // 8. 取得特定文章留言，傳入文章 ID
  getCommentsForArticle(articleId: number): Observable<Comment[]> {
    // 必須對應到 api/Comments/Article/{id}
    return this.httpClient.get<Comment[]>(`${this.commentUrl}/Article/${articleId}`);
  }

  // 9. 新增留言，傳入留言內容
  addComment(payload: { articleId: number; memberId: number; content: string }): Observable<Comment> {
    return this.httpClient.post<Comment>(this.commentUrl, payload);
  }

  // 取得會員收藏清單，傳入會員 ID
  getUserFavorites(memberId: number): Observable<Article[]> {
    // 對應後端 [HttpGet("FavoriteArticles/{memberId}")]
    return this.httpClient.get<Article[]>(`${this.apiUrl}/FavoriteArticles/${memberId}`);
  }

  // 收藏/取消收藏 切換方法，傳入文章 ID 與會員 ID
  toggleFavorite(articleId: number, memberId: number): Observable<any/*{ isFavorited: boolean }*/> {
    // 對應後端修正後的 [HttpPost("{id}/FavoriteArticle/{memberId}")]
    // return this.httpClient.post<any/*{ isFavorited: boolean }*/>(`${this.apiUrl}/${articleId}/FavoriteArticle/${memberId}`, {});

    // 為了除錯，先把 URL 印出來看看
    const url = `${this.apiUrl}/${articleId}/FavoriteArticle/${memberId}`;
    console.log('正在請求 URL:', url); // 加上這行，在瀏覽器 F12 看看網址對不對
    return this.httpClient.post<any>(url, {}); // 這裡先用 any，等後端確定回傳格式再改
  }
}
