// src/app/features/forum/forum.component.ts
// 匯入必要的模組與服務
import { Component, OnInit } from '@angular/core';
import { ForumService, Article, PagedResult, Comment } from './forum.service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';
import { RouterModule } from '@angular/router';
// 定義元件
@Component({
  selector: 'app-forum', // 元件選擇器
  standalone: true, // 使用獨立元件
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterModule], // 匯入所需模組
  templateUrl: './forum.component.html', // HTML 模板路徑
  styleUrl: './forum.component.css' // CSS 樣式路徑
})
// 定義元件類別
export class ForumComponent implements OnInit {
  // --- 列表數據 ---
  recentPosts: Article[] = []; // 畫面實際渲染的陣列

  // --- 狀態控制 ---
  selectedCategory: number | null = null; // 預設不篩選分類
  selectedTag: number | null = null; // 預設不篩選標籤
  currentSortField: keyof Article = 'postDate'; // 預設排序欄位
  sortDirection: 'asc' | 'desc' = 'desc'; // 預設排序方向

  // --- 圖片上傳 ---
  selectedFile: File | null = null; // 圖片檔案

  // --- 分頁狀態 ---
  currentPage: number = 1; // 目前頁碼
  pageSize: number = 5; // 每頁顯示筆數
  totalCount: number = 0; // 總筆數
  totalPages: number = 1; // 總頁數

  // --- Modal & Form ---
  showModal = false; // 控制彈窗顯示
  modalMode: 'create' | 'edit' | 'view' = 'create'; // 彈窗3種模式
  editingArticle: Article | null = null; // 正在編輯或檢視的文章
  articleForm!: FormGroup; // 文章表單
  commentForm!: FormGroup; // 留言表單
  articleComments: Comment[] = []; // 目前文章的留言列表
  members: any[] = []; // 會員清單
  imagePreview: string | null = null; // 圖片預覽 URL
  searchKeyword: string = ''; // 搜尋關鍵字
  currentMemberId: number = 20; // 假設的當前登入會員 ID

  // 建構子注入 ForumService 和 FormBuilder
  constructor(private forumService: ForumService, private formBuilder: FormBuilder) { }

  // 元件初始化
  ngOnInit(): void {
    this.initArticleForm(); // 初始化文章表單
    this.initCommentForm(); // 初始化留言表單
    this.loadData(); // 初始化讀取
    this.loadMembers(); // 2. 初始化時載入會員
  }
  // 載入會員清單
  loadMembers(): void {
    // 呼叫 ForumService 取得會員清單
    this.forumService.getMembers().subscribe({
      next: (data) => { this.members = data; console.log('會員清單已載入', data); },
      error: (err) => console.error('會員清單載入失敗', err)
    });
  }
  // 處理會員名字輸入轉 ID
  onMemberInput(event: any): void {
    // 取得輸入的名字
    const inputName = event.target.value;

    // 從會員清單中尋找匹配的名字
    const matchedMember = this.members.find(m => m.name === inputName);

    if (matchedMember) {
      // 如果找到了，就把 ID 填入表單真正要送出的 memberId 欄位
      this.articleForm.patchValue({ memberId: matchedMember.memberId });
      console.log('匹配成功，ID 為:', matchedMember.memberId);
    } else {
      // 如果沒找到（或使用者正在打字中），清空 ID 欄位以觸發驗證報錯
      this.articleForm.patchValue({ memberId: null });
    }
  }

  // 核心加載數據：當分頁、分類、標籤改變時都呼叫此方法
  loadData(): void {
    // 呼叫 API 取回資料
    this.forumService.getRecentArticles(this.currentPage, this.pageSize, this.selectedCategory, this.selectedTag, this.searchKeyword, this.currentMemberId).subscribe({
      // 使用 any 接收回傳，避免屬性大小寫不一致報錯
      next: (res: any /*PagedResult<Article>*/) => {
        console.log('後端回傳原始資料:', res); // 除錯用
        this.recentPosts = res.items || res.Items; // 處理大小寫不一致問題
        this.totalCount = res.totalCount || res.TotalCount; // 處理大小寫不一致問題
        this.totalPages = Math.ceil(res.totalCount / this.pageSize); // 計算總頁數
      },
      error: (err) => console.error('無法載入文章', err)
    });
  }

  // 跳頁函式 (對應 HTML 裡的 goToPage)
  goToPage(page: number): void {
    // 確保頁碼在有效範圍內
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page; // 更新目前頁碼
      this.loadData(); // 重新載入資料
    }
  }
  // 分類篩選觸發方法
  selectCategory(id: number | null): void {
    this.selectedCategory = id; // 設定選中的分類 ID
    // this.selectedTag = null; // 切換大分類時，把標籤重設為 null，
    this.currentPage = 1; // 篩選時回到第一頁
    // 注意：若 API 支援過濾，應將 id 帶入 API 請求
    this.loadData(); // 重新讀取資料
  }

  // 搜尋觸發方法
  onSearch(): void {
    this.currentPage = 1; // 搜尋時重置回第一頁
    this.loadData(); // 重新載入資料
  }

  // 清除搜尋
  clearSearch(): void {
    this.searchKeyword = ''; // 清空搜尋關鍵字
    this.onSearch(); // 觸發搜尋以重新載入資料
  }

  // --- 表單初始化 ---
  initArticleForm(): void {
    // 使用 FormBuilder 建立文章表單
    this.articleForm = this.formBuilder.group({
      articleId: [0], // 文章ID預設為 0，表示新文章
      categoryId: [null, Validators.required], // 必填
      tagId: [null, Validators.required], // 必填
      memberId: [null, Validators.required], // 必填且為會員 ID
      title: ['', [Validators.required, Validators.maxLength(20)]], // 必填且最多20字
      content: ['', [Validators.required, Validators.maxLength(500)]], // 必填且最多500字
      picture: [''] // 圖片 URL，可選填
    });
  }
  // 留言表單初始化
  initCommentForm(): void {
    // 使用 FormBuilder 建立留言表單
    this.commentForm = this.formBuilder.group({
      articleId: [0], // 文章ID，預設為0
      memberId: [20], // 假設當前會員ID為20
      content: ['', Validators.required] // 留言內容，必填
    });
  }

  // --- CRUD 操作 ---
  submitForm(): void {
    // 基本驗證
    if (this.articleForm.invalid) {
      // 這裡可以幫助你 debug 為什麼沒反應
      Object.keys(this.articleForm.controls).forEach(key => {
        // 列出所有欄位的錯誤
        const controlErrors = this.articleForm.get(key)?.errors;
        if (controlErrors != null) console.log('欄位 ' + key + ' 驗證失敗:', controlErrors);
      });
      return;
    }

    const payload = this.articleForm.value; // 取得表單值
    // 根據模式呼叫不同的 Service 方法
    if (this.modalMode === 'create') {
      // 建立新文章
      this.forumService.createNewArticle(payload).subscribe({
        next: () => { this.loadData(); this.closeModal(); }, // 成功後重新載入並關閉彈窗
        error: (err) => console.error('新增失敗', err)
      });
    } else if (this.modalMode === 'edit') {
      // 編輯文章
      this.forumService.editArticle(payload).subscribe({
        next: () => {
          // 編輯成功後的操作
          console.log('編輯成功');
          this.loadData(); // 重新整理列表，確保數據最新
          this.closeModal(); // 關閉彈窗
        },
        error: (err) => {
          console.error('編輯失敗', err);
          alert(err.error || '更新文章時發生錯誤');
        }
      });
    }
  }

  // 發佈新文章按鈕觸發
  createNewArticle(): void {
    this.modalMode = 'create'; // 設定為建立模式
    this.articleForm.reset({ // 重置表單
      articleId: 0,
      categoryId: 1,
      tagId: 1,
      memberId: null,
      title: '',
      content: '',
      picture: ''
    });
    // 重要：清空會員輸入欄位顯示
    const memberInput = document.getElementById('member') as HTMLInputElement;
    if (memberInput) memberInput.value = '';

    this.imagePreview = null; // 清空圖片預覽
    this.showModal = true; // 顯示彈窗
  }
  // 一鍵帶入資料
  fillDemoData(): void {
    // 1. 定義會員資訊
    const myId = 20;
    const myName = 'Norah';

    // 1. 使用 patchValue 填充響應式表單
    this.articleForm.patchValue({
      categoryId: 1, // 預設狗狗專區
      tagId: 1,      // 預設飲食標籤
      memberId: myId,
      title: '我家狗狗最近超愛吃這款！',
      content: '最近換了新的天然糧，狗狗適口性非常好，每天都期待放飯時間。想問問大家還有推薦其他適合幼犬的零食嗎？',
    });
    // 2. 由於 HTML 上的會員輸入框是獨立的 input + datalist，需要手動補上顯示文字
    const memberInput = document.getElementById('member') as HTMLInputElement;
    if (memberInput) {
      memberInput.value = myName;
    }

    console.log('已自動填入測試資料');
  }

  // 修改文章按鈕觸發
  editArticle(post: Article): void {
    this.modalMode = 'edit'; // 設定為編輯模式
    this.editingArticle = post; // 設定正在編輯的文章
    this.showModal = true; // 顯示彈窗
    // 檢查 article.categoryId 是否為 0，若是則強制預設為 1
    // const safeCategoryId = post.categoryId > 0 ? post.categoryId : 1;
    // this.articleForm.patchValue({
    //   ...post,
    //   categoryId: safeCategoryId
    // });
    // 使用 patchValue 填入表單，並確保 categoryId 有合理值
    this.articleForm.patchValue({
      articleId: post.articleId,
      title: post.title,
      content: post.content,
      // 關鍵：確保傳入的是數值類型，且不為 null
      categoryId: post.categoryId || 1,
      tagId: Number(post.tagId) || 1,
      memberId: post.memberId,
      picture: post.picture
    });
    // this.articleForm.patchValue({
    //   articleId: post.articleId,
    //   title: post.title,
    //   content: post.content,
    //   categoryId: post.categoryId,
    //   tagId: post.tagId,
    //   memberId: post.memberId,
    //   picture: post.picture
    // });
    this.imagePreview = post.picture;
    // this.showModal = true;


  }
  // 刪除文章按鈕觸發
  deleteArticle(id: number): void {
    if (!confirm('確定刪除？')) return;
    this.forumService.deleteArticle(id).subscribe(() => this.loadData());
  }

  // 排序功能
  sortArticles(field: keyof Article): void {
    // 切換排序方向或設定新的排序欄位
    if (this.currentSortField === field) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    }
    // 設定新的排序欄位，預設為降冪
    else {
      this.currentSortField = field;
      this.sortDirection = 'desc';
    }
    this.loadData(); // 重新載入資料
  }

  // 取得排序圖示類別 (沒用到)
  getSortIcon(field: keyof Article): any {
    return {
      // 為 asc 和 desc 圖示設定 CSS 類別
      'asc-icon': this.currentSortField === field && this.sortDirection === 'asc',
      'desc-icon': this.currentSortField === field && this.sortDirection === 'desc'
    };
  }

  /** 圖片選擇處理 */
  onFileSelected(event: any): void {
    // 檢查是否有選擇檔案
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file; // 儲存選擇的檔案
      const reader = new FileReader(); // 建立 FileReader 物件
      // 設定讀取完成的回呼函式
      reader.onload = () => {
        // 取得圖片的 Data URL
        this.imagePreview = reader.result as string;
        // 將圖片預覽的 Data URL 設定到表單的 picture 欄位
        this.articleForm.patchValue({ picture: this.imagePreview });
      };
      reader.readAsDataURL(file); // 讀取檔案為 Data URL
    }
  }

  // 取得標籤清單 (供 HTML @for 使用)
  getAvailableTags() {
    return [
      { id: 1, name: '飲食' }, { id: 2, name: '保健醫療' }, { id: 3, name: '孕事配種' },
      { id: 4, name: '梳洗美容' }, { id: 5, name: '生活起居' }, { id: 6, name: '訓練' },
      { id: 7, name: '新聞' }, { id: 8, name: '討論' }, { id: 9, name: '網聚' },
      { id: 10, name: '品種' }, { id: 11, name: '分享' }
    ];
  }

  /** 提交留言 */
  submitComment(): void {
    // 1. 基本驗證
    if (this.commentForm.invalid || !this.editingArticle) return;

    // 2. 準備 payload (memberId 20 假設為當前登入者)
    const currentUserId = 20;
    const payload = {
      articleId: this.editingArticle.articleId, // 目前文章 ID
      memberId: currentUserId, // 假設的當前會員 ID
      content: this.commentForm.value.content // 留言內容
    };
    // 3. 呼叫 Service 送出留言
    this.forumService.addComment(payload).subscribe({
      next: (newComment: any) => {
        // 4. 從現有的成員清單中「抓」出名字，補齊給 UI 渲染
        const memberInfo = this.members.find(m => m.memberId === currentUserId);
        // 組合要顯示在 UI 的物件
        const uiComment = {
          ...newComment,
          memberName: memberInfo ? memberInfo.name : '系統會員'
        };
        // 5. 更新本地留言陣列 (讓畫面立即出現新留言)
        this.articleComments = [...this.articleComments, uiComment];

        // 6. 同步更新「詳情視窗」與「主列表」的回覆次數
        this.editingArticle!.commentCount++;
        const postInList = this.recentPosts.find(p => p.articleId === this.editingArticle?.articleId);

        // 7. 重置表單
        this.commentForm.reset();
        if (postInList) {
          postInList.commentCount++;
        }
        console.log('回覆成功！');
      },
      error: (err) => {
        console.error('留言失敗，請檢查 API 回傳格式', err);
        // 備案：API 報錯但資料庫若有進去，強迫重整一次
        this.loadComments(this.editingArticle!.articleId);
      }
    });
  }

  /** 標籤篩選觸發 */
  onTagFilterChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    const value = target.value;
    // 處理 string 轉 number 的問題
    if (value === 'all' || value === '') {
      this.selectedTag = null;
    } else {
      // 使用 Number() 轉換，並確保賦值給 selectedTag
      this.selectedTag = Number(value);
    }
    this.currentPage = 1;
    this.loadData();
  }

  /** 每頁筆數改變觸發 */
  onPageSizeChange(): void {
    this.currentPage = 1;
    this.loadData();
  }

  // --- 互動功能 (Like/Dislike) ---
  // 按讚
  likeArticle(id: number): void {
    const art = this.editingArticle;
    if (!art) return;
    // 1. 根據目前 userVote 決定 action
    let action: 'add' | 'remove' | 'switch';
    if (art.userVote === 'like') action = 'remove';
    else if (art.userVote === 'dislike') action = 'switch';
    else action = 'add';
    // 2. 呼叫 API，不要在外面先改 art.userVote，等成功再改
    this.forumService.likeArticle(id, action).subscribe({
      next: (res: any) => {
        console.log('後端回傳內容:', res);
        // 3. 處理大小寫相容性 (res.like 或 res.Like)
        art.like = res.like !== undefined ? res.like : res.Like;
        art.disLike = res.disLike !== undefined ? res.disLike : res.DisLike;
        art.popular = res.popular !== undefined ? res.popular : res.Popular;
        // 4. 根據剛才的 action 更新 UI 狀態
        if (action === 'remove') {
          art.userVote = null;
        } else {
          art.userVote = 'like';
        }
        this.updateListState(id, art); // 同步回主表
      },
      error: (err) => {
        console.error('API 請求失敗:', err);
        alert('操作失敗，請稍後再試');
      }
    });
  }
  // 倒讚
  dislikeArticle(id: number): void {
    const art = this.editingArticle;
    if (!art) return;
    let action: 'add' | 'remove' | 'switch';
    if (art.userVote === 'dislike') action = 'remove';
    else if (art.userVote === 'like') action = 'switch';
    else action = 'add';
    this.forumService.dislikeArticle(id, action).subscribe({
      next: (res: any) => {
        art.like = res.like !== undefined ? res.like : res.Like;
        art.disLike = res.disLike !== undefined ? res.disLike : res.DisLike;
        art.popular = res.popular !== undefined ? res.popular : res.Popular;
        if (action === 'remove') {
          art.userVote = null;
        } else {
          art.userVote = 'dislike';
        }
        this.updateListState(id, art);
      },
      error: (err) => console.error(err)
    });
  }
  // 私有輔助方法：當彈窗數據變動時，同步更新表格列表中的數據
  private updateListState(id: number, updatedArt: Article) {
    const postInList = this.recentPosts.find(a => a.articleId === id);
    if (postInList) {
      postInList.like = updatedArt.like;
      postInList.disLike = updatedArt.disLike;
      postInList.popular = updatedArt.popular;
      postInList.userVote = updatedArt.userVote;
    }
  }


  // --- 留言處理 ---
  viewDetails(article: Article): void {
    this.modalMode = 'view';
    this.editingArticle = { ...article };
    this.articleComments = []; // 先清空舊留言，避免畫面閃爍

    this.articleForm.patchValue(article);
    // 重要：確保檢視時，imagePreview 有值可以顯示
    this.imagePreview = article.picture;

    // 必須手動設定 input 的顯示值，否則會員欄位會是空的
    setTimeout(() => {
      const memberInput = document.getElementById('member') as HTMLInputElement;
      if (memberInput) {
        memberInput.value = article.memberName || '';
      }
    });

    // 這裡應呼叫後端 API 取得留言，而非從 local 找
    this.loadComments(article.articleId); // 2. 重新抓取
    this.showModal = true;
  }

  loadComments(articleId: number): void {
    this.forumService.getCommentsForArticle(articleId).subscribe({
      next: (data) => {
        console.log('抓到的留言：', data);
        this.articleComments = data;
      },
      error: (err) => {
        console.error('抓取留言失敗，請確認 API 路由是否存在', err);
        // 建議失敗時清空，避免顯示上一篇文章的留言
        this.articleComments = [];
      }
    });
  }


  openEditModal(article: Article) {
    this.modalMode = 'edit';
    this.editingArticle = article;
    this.imagePreview = article.picture; // 設定圖片預覽

    // 確保 article 物件裡確實有 categoryId (1 或 2)
    console.log('當前文章資料:', article);

    // 使用 patchValue 填入表單
    this.articleForm.patchValue({
      articleId: article.articleId,
      title: article.title,
      content: article.content,
      categoryId: article.categoryId, // 這裡如果 article.categoryId 是 undefined，就會變 0
      tagId: Number(article.tagId),
      memberId: article.memberId,
      picture: article.picture
    });
    setTimeout(() => {
      const memberInput = document.getElementById('member') as HTMLInputElement;
      if (memberInput) {
        memberInput.value = article.memberName || '';
      }
    });

    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.imagePreview = null;
    this.selectedFile = null;
    this.articleForm.reset({ articleId: 0 });
  }

  // --- 工具方法 ---
  formatDate(date: any): string {
    // return date ? new Date(date).toLocaleDateString() : '';
    if (!date) return '';
    const d = new Date(date);

    // 格式化為：2025/10/9 11:36 (24小時制)
    // 你也可以根據需求調整格式
    return d.getFullYear() + '/' +
      (d.getMonth() + 1).toString().padStart(2, '0') + '/' +
      d.getDate().toString().padStart(2, '0') + ' ' +
      d.getHours().toString().padStart(2, '0') + ':' +
      d.getMinutes().toString().padStart(2, '0');
  }
  // 收藏文章
  toggleFavorite(post: Article) {
    // const memberId = 20; // 這裡應該動態取得目前登入的會員 ID
    if (!this.currentMemberId) {
      alert('請先登入！');
      return;
    }

    this.forumService.toggleFavorite(post.articleId, this.currentMemberId).subscribe({
      // 使用 any 接收 res 避免 res.isFavorited 報錯
      next: (res: any) => {
        // 直接更新該文章物件的收藏狀態
        // 強制轉型解決 'Article' 沒有屬性報錯
        post.isFavorited = res.isFavorited;
        console.log(res.isFavorited ? '已加入收藏' : '已取消收藏');
      },
      error: (err) => console.error('API 錯誤詳情:', err)
    });
  }
}

