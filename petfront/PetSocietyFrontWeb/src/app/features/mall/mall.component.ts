import { CartService } from './services/cart.service';
import { Router } from '@angular/router';
import { MallService } from './services/mall.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import Swal from 'sweetalert2';
import { DialogService } from 'primeng/dynamicdialog';
import { ProductDetailComponent } from './components/product-detail/product-detail.component';
import { MessageService } from 'primeng/api';
import { PaginatorModule } from 'primeng/paginator';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-mall',
  imports: [CommonModule, PaginatorModule, FormsModule],
  templateUrl: './mall.component.html',
  styleUrl: './mall.component.css',
  providers: [DialogService, MessageService]
})
export class MallComponent implements OnInit {

  products: any[] = []; // 展示架 (隨時會變，比如被篩選過)
  currentCategory: string = '全部商品'; // 門口的電子看板 (顯示現在是哪一區)
  isLoading: boolean = true;
  keyword: string = '';

  // 新增：分頁相關變數
  currentPage: number = 1;     // 目前第幾頁 (後端是從 1 開始)
  pageSize: number = 15;       // 一頁幾筆
  totalRecords: number = 0;    // 總共有幾筆 (後端會告訴我們)
  first: number = 0;           // PrimeNG Paginator 用來控制「第幾筆開始」(UI用)

  // 新增：記錄目前的價格範圍
  currentMinPrice: number | undefined;
  currentMaxPrice: number | undefined;

  // 宣告兩個專門給 input 綁定用的變數
  inputMinPrice: number | null = null;
  inputMaxPrice: number | null = null;

  constructor(
    private mallService: MallService, // 進貨員 (負責去後端拿商品)
    private CartService: CartService, // 結帳員 (負責把東西送去購物車)
    private router: Router, // 導遊 (負責帶客人去登入頁)
    private dialogService: DialogService
  ) { }

  // 元件初始化(網頁載入)時，執行這個方法
  ngOnInit(): void {
    this.loadProducts(1);
  }
  // -- 載入商品 (支援分頁 + 分類)--
  loadProducts(page: number) {

    this.isLoading = true;

    // 呼叫 Service，傳入頁碼
    this.mallService.getProducts(
      page,
      this.pageSize,
      this.currentCategory,
      this.currentMinPrice,
      this.currentMaxPrice,
      this.keyword
    ).subscribe({
      next: (data) => {
        // data.items 就是這一頁的 15 筆商品
        this.products = data.items;
        // data.totalCount 是總筆數 (給分頁器用的)
        this.totalRecords = data.totalCount;
        this.isLoading = false;
        // 每次換頁，記得回到頂部，不然使用者會停在頁面下面
        window.scrollTo({ top: 0, behavior: 'smooth' });
        console.log(`第 ${page} 頁資料載入成功，共 ${this.products.length} 筆，總數 ${this.totalRecords}`);

      },
      error: (err) => {
        console.error('無法取得商品列表', err);
        this.isLoading = false;
      }
    });
  }
  // 新增：分頁器切換事件
  onPageChange(event: any) {
    // event.page 是從 0 開始的 (PrimeNG 的習慣)
    // 但我們的後端是從 1 開始的，所以要 +1
    this.currentPage = event.page + 1;
    this.first = event.first;
    // 重新載入那一頁的資料
    this.loadProducts(this.currentPage);
  }
  // -- 分類篩選功能 --
  filterCategory(categoryName: string): void {
    // 1. 設定新的分類
    this.currentCategory = categoryName;

    // 關鍵：清空綁定的變數，畫面上的數字就會瞬間消失！
    this.inputMinPrice = null;
    this.inputMaxPrice = null;
    this.currentMinPrice = undefined;
    this.currentMaxPrice = undefined;

    // ★ 建議：換分類時，把價格篩選清空，避免使用者困惑
    this.currentMinPrice = undefined;
    this.currentMaxPrice = undefined;

    // ★★★ 新增：切換分類時，把搜尋關鍵字也清空！ ★★★
    this.keyword = '';

    // 2. 頁碼重置回第 1 頁 (因為換分類了，當然要從頭看)
    this.currentPage = 1;
    this.first = 0; // 讓分頁器UI也回到第一頁

    // 3. 重新呼叫後端 (這裡會自動帶入 currentCategory)
    this.loadProducts(1);
  }

  // -- 價格篩選功能 --
  filterPrice(): void {
    // 1. 解析數字
    this.currentMinPrice = this.inputMinPrice || undefined;
    this.currentMaxPrice = this.inputMaxPrice || undefined;

    // 2. 頁碼重置回第 1 頁
    this.currentPage = 1;
    this.first = 0;

    // 3. 呼叫後端 (這裡會自動帶入目前的 Category + 新的 Price)
    // ★ 重點：這裡不需要再寫 this.currentCategory = '全部商品' 了！
    this.loadProducts(1);
  }

  // -- 商品排序功能 --
  onSortChange(event: any): void {
    const sortValue = event.target.value;
    switch (sortValue) {
      case 'price-asc':// 價格由低到高
        this.products.sort((a, b) => a.price - b.price);
        break;
      case 'price-desc': // 價格由高到低
        this.products.sort((a, b) => b.price - a.price);
        break;
      case 'newest': // 最新上架
        // ID 排序，ID 越大代表越晚新增
        // createDate 欄位，改成: new Date(b.createDate).getTime() - new Date(a.createDate).getTime()
        this.products.sort((a, b) => b.productId - a.productId);
        break;
      default: // 預設排序 (例如照 ID 順序)
        this.products.sort((a, b) => a.productId - b.productId);
        break;
    }
  }

  // -- 購物車加入功能 --
  addToCart(product: any): void {
    // 檢查 LocalStorage 裡面有沒有這把鑰匙
    const token = localStorage.getItem('jwtToken');

    if (token) {
      // --- 情境 A：有 Token (代表已登入) ---
      console.log('正在呼叫後端 API...', product)

      this.CartService.addToCart(product.productId, 1).subscribe({
        next: (response) => {
          console.log('後端回傳成功:', response)
          Swal.fire({
            title: '已加入購物車',
            text: `商品 : ${product.productName}`,
            icon: 'success',
            timer: 1500,
            showCancelButton: false
          });
          // ★ 通知右上角購物車 +1
          this.CartService.incrementCartCount(1);
        },
        error: (err) => {
          // 失敗！(可能是 Token 過期，或是網址錯了)
          console.error('後端回傳錯誤:', err);
          Swal.fire('加入失敗', '系統連線錯誤', 'error');
        }
      });
    } else {
      // --- 情境 B：沒 Token (代表未登入) ---
      Swal.fire({
        title: '請先登入',
        text: '登入會員後才能將商品加入購物車喔！',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: '前往登入',
        cancelButtonText: '先不要',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        reverseButtons: true
      }).then((result) => {
        if (result.isConfirmed) {
          // 使用者按「前往登入」，跳轉頁面
          this.router.navigate(['/login']);
        }
      });
    }
  }
  // -- 商品詳細頁面 --
  openProductDetail(product: any) {
    this.dialogService.open(ProductDetailComponent, {
      header: '商品詳細內容',
      width: '800px',       // 電腦版寬度
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000,
      maximizable: false,
      dismissableMask: true, // 點擊背景可以關閉
      modal: true,
      data: product,      // ★ 關鍵：把點到的商品資料傳進去！
      breakpoints: {
        '960px': '85%', // 平板
        '640px': '100%' // 手機全螢幕
      }
    });
  }

  // 判斷是否為 14 天內的新品
  isNew(dateString: string): boolean {
    if (!dateString) return false;
    const createDate = new Date(dateString);
    const today = new Date();
    const diffTime = Math.abs(today.getTime() - createDate.getTime());
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays <= 14; // 14天內算新品
  }
  // ★ 3. 新增搜尋觸發方法
  onSearch() {
    // 搜尋時，通常會想搜尋「全站商品」，所以建議把分類歸零 (看你的需求)
    // 這裡我們先保留分類，變成「在目前分類下搜尋」

    this.currentPage = 1; // 搜尋後一定要回到第 1 頁
    this.first = 0;       // 分頁器歸零
    this.loadProducts(1); // 重新載入
  }
}
