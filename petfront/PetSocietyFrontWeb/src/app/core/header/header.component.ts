import { Component, CUSTOM_ELEMENTS_SCHEMA, HostListener, inject, OnInit } from '@angular/core';
// 啟用RouterLink & RouterLinkActive
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { CartService } from '../../features/mall/services/cart.service';
import { CommonModule, NgIf } from '@angular/common';
import Swal from 'sweetalert2';
import { Subject, Subscription, of } from 'rxjs';
import { catchError, debounceTime, switchMap } from 'rxjs/operators';
import { AuthService } from '../auth/auth.service';


@Component({
  selector: 'app-header',
  imports: [RouterLink,
    RouterLinkActive, NgIf, CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ],
})
export class HeaderComponent implements OnInit {
  private authService = inject(AuthService); // 注入 AuthService，簡潔新主流寫法

  // 建立一個 Subject 通道，用來接收使用者的點擊
  private quantityUpdateSubject = new Subject<{ item: any, quantity: number }>();
  private updateSubscription: Subscription | undefined;

  cartCount: number = 0; // 用來顯示的變數
  cartItems: any[] = []; // 用來裝後端回傳的商品列表
  totalAmount: number = 0; // 用來顯示總金額

  isLogin = this.authService.isLogin; // 引用 AuthService 的 Signal

  // ★ 注入 Service
  constructor(private cartService: CartService,
    public router: Router) { }

  ngOnInit(): void {
    // ★ 只要「數量」有變 (例如剛登入、或剛加入商品)，我們就順便去更新「清單內容」
    this.cartService.cartCount$.subscribe(count => {
      this.cartCount = count;

      if (count > 0) {
        this.loadCartDetails();
      }
      else {
        this.cartItems = [];
        this.totalAmount = 0
      }
    });
    this.cartService.getCartCount();

    // 設定「防抖」監聽器
    this.updateSubscription = this.quantityUpdateSubject.pipe(
      debounceTime(500),
      switchMap(data => {
        // data 裡面的 quantity 是「目標值」，我們需要知道「變化量」來做回滾有點麻煩
        // 所以錯誤時我們選擇「重新載入整頁」比較保險
        return this.cartService.updateCartItem(data.item.productId, data.quantity).pipe(
          catchError(err => {
            this.handleUpdateError(); // 失敗處理
            return of(null);
          })
        );
      })
    ).subscribe({
      next: (result) => {
        if (result === null) return;
        // ★ 成功後什麼都不做！因為 UI 早就對了，不需要浪費流量重抓
        console.log('背景更新成功 (靜默)');
      }
    });
  }

  // 把錯誤處理抽出來
  handleUpdateError() {
    console.error('更新失敗，回滾數據');
    Swal.fire('哎呀', '網路不穩，數據同步失敗', 'error');
    // 只有出錯時，才強制跟後端要最新資料來校正
    this.loadCartDetails();
    this.cartService.getCartCount();
  }

  loadCartDetails() {
    this.cartService.getCartItems().subscribe({
      next: (items) => {
        this.cartItems = items;

        this.totalAmount = items.reduce((sum, item) => sum + (item.price * item.quantity), 0)
      },
      error: (err) => console.log('無法取得購物車列表', err)
    });
  }
  removeItem(productId: number) {
    // 確認按鈕有反應
    // console.log('準備刪除商品 ID:', productId);
    Swal.fire({
      title: '確定要移除嗎？',
      text: "這項商品將會從購物車中移除",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: '是的，移除！',
      cancelButtonText: '再想想',
      confirmButtonColor: '#d33',   // 刪除按鈕用紅色 (比較警示)
      cancelButtonColor: '#b0b0b0', // 取消按鈕用灰色
      reverseButtons: true          // 把確認按鈕放右邊 (符合習慣)
    }).then((result) => {
      if (result.isConfirmed) {
        this.cartService.removeCartItem(productId).subscribe({
          next: () => {
            // ★ 關鍵步驟：刪除後，必須立刻更新畫面！
            // 重新抓取「購物車數量」 (這會更新小紅點，也會觸發訂閱去更新列表)
            this.cartService.getCartCount();
            // 2. (保險起見) 也可以手動再抓一次列表，確保畫面是最新的
            // 因為有時候數量沒變(例如原本 0 個變 0 個?) 但內容變了，手動抓比較穩
            this.loadCartDetails();

            Swal.fire({
              icon: 'success',
              title: '已移除',
              text: '商品已成功移除',
              showConfirmButton: false,
              timer: 1500
            });
          }, error: (err) => {
            console.error('刪除失敗', err);
            Swal.fire('哎呀！', '移除失敗，請稍後再試', 'error');
          }
        });
      }
    });
  }
  // 記得在離開元件時取消訂閱，避免記憶體洩漏
  ngOnDestroy() {
    if (this.updateSubscription) {
      this.updateSubscription.unsubscribe();
    }
  }
  updateQuantity(item: any, change: number) {
    const oldQuantity = item.quantity;
    const newQuantity = oldQuantity + change;

    if (newQuantity < 1) {
      return;
    }
    // ★ 樂觀更新 (Optimistic UI)：不管後端，先改畫面！
    item.quantity = newQuantity;    // 1. 改商品數量
    this.calculateTotal();          // 2. 改總金額
    this.cartCount += change;       // 3. ★ 改小紅點 (直接加減)

    // ★ 不直接 Call API，而是把請求「丟進去」防抖通道
    // 通道會幫你擋住連續點擊，只留最後一次
    this.quantityUpdateSubject.next({ item: item, quantity: newQuantity });
  }
  // ★ 把計算總金額邏輯抽出來 (方便重複使用)
  calculateTotal() {
    this.totalAmount = this.cartItems.reduce((sum, item) => sum + (item.price * item.quantity), 0);
  }

  onWishlistClick() {
    // 1. 如果已經登入 -> 直接去收藏頁
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/member/wishlist']);
    }
    // 2. 如果沒登入 -> 彈出 Toast -> 按下確定才去登入頁
    else {
      Swal.fire({
        title: '請先登入',
        text: '登入後才能查看收藏清單喔！',
        icon: 'warning',
        confirmButtonText: '前往登入',
        showCancelButton: true,
        cancelButtonText: '先不要',
        confirmButtonColor: '#D8C3A5',
        cancelButtonColor: '#8E8375',
      }).then((res) => {
        if (res.isConfirmed) {
          this.router.navigate(['/member/login']);
        }
      });
    }
  }
}
