import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { DividerModule } from 'primeng/divider';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CartService } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    InputNumberModule,
    DividerModule,
    RouterModule,
    FormsModule,
  ],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent implements OnInit {

  // 抓取 HTML 裡的隱藏表單
  @ViewChild('paymentForm') paymentForm!: ElementRef;

  cartItems: any[] = [];
  totalPrice: number = 0;
  isLoading: boolean = true;

  // 新增收件人資訊物件 (綁定到 HTML)
  receiver = {
    name: '',
    phone: '',
    email: '',
    address: ''
  };

  // 用來存後端回傳的加密資料
  paymentUrl: string = ''; // 刷卡網址 (NewebPay ServiceUrl)
  paymentData: any = {
    MerchantID: '',
    TradeInfo: '',
    TradeSha: '',
    Version: ''
  };

  constructor(
    private cartService: CartService,
    private orderService: OrderService,
    private router: Router) { }

  ngOnInit(): void {
    this.loadCart();
  }
  // 載入真實購物車資料
  loadCart() {

    // 開始搬貨前，先打開轉圈圈 (開燈)
    this.isLoading = true;

    this.cartService.getCartItems().subscribe({
      next: (items) => {
        this.cartItems = items;
        this.calculateTotal();

        // 算出「總商品數量」(把每一項的 quantity 加起來)
        // 預設是 0，然後一筆一筆把 quantity 加進去
        const totalQuantity = items.reduce((sum: number, item: any) => sum + item.quantity, 0);

        // 更新右上角的數字，確保同步
        this.cartService.updateCartCount(totalQuantity);

        // 資料全部處理好了！關掉轉圈圈 (關燈)
        this.isLoading = false;
      },
      error: (err) => {
        console.error('購物車載入失敗', err);

        // 就算失敗了也要關掉
        this.isLoading = false;
      }
    });
  }
  calculateTotal() {
    this.totalPrice = this.cartItems.reduce((acc, item) => acc + (item.price * item.quantity), 0);
  }
  // 處理數量變更
  updateQuantity(item: any) {
    // 1. 先算錢 (讓畫面金額馬上變，使用者體驗才好)
    this.calculateTotal();

    // 2. 呼叫後端 API 更新資料庫
    // (注意：這裡要傳 productId 和 新的 quantity)
    this.cartService.updateCartItem(item.productId, item.quantity).subscribe({
      next: () => {
        console.log(`商品 ${item.productName} 數量更新為 ${item.quantity}`);

        // 3. 更新右上角小鈴鐺 (重新加總所有商品的數量)
        const totalQuantity = this.cartItems.reduce((sum, i) => sum + i.quantity, 0);
        this.cartService.updateCartCount(totalQuantity);
      },
      error: (err) => {
        console.error('更新數量失敗', err);
        // 如果失敗，也可以考慮把數量改回來，或是跳 Alert
      }
    });
  }
  // 🚀 核心功能：結帳
  checkout() {
    // (A) 防呆：購物車是空的
    if (this.cartItems.length === 0) {
      Swal.fire({
        title: '購物車是空的',
        text: '還沒有選購商品喔！要去逛逛嗎？🐶',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: '去逛逛',
        cancelButtonText: '先不要',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        reverseButtons: true
      }).then((result) => {
        if (result.isConfirmed) {
          this.router.navigate(['/mall']); // 導回商城首頁
        }
      });
      return;
    }

    // (B) 防呆：收件人資料不完整
    if (!this.receiver.name || !this.receiver.phone || !this.receiver.address) {
      Swal.fire({
        title: '資料不完整',
        text: '請填寫完整的收件人資訊 (姓名、電話、地址) 🚚',
        icon: 'info',
        confirmButtonText: '好，我去填寫',
        confirmButtonColor: '#3085d6'
      });
      return;
    }

    // (C) 呼叫後端下單 (這部分邏輯不變，但 Error 換成 Swal)
    // 顯示 Loading...
    Swal.fire({
      title: '訂單處理中...',
      text: '正在將您導向付款頁面 ⏳',
      allowOutsideClick: false,
      didOpen: () => {
        Swal.showLoading();
      }
    });

    const orderRequest = {
      orderItems: this.cartItems.map(item => ({
        productId: item.productId,
        quantity: item.quantity,
        unitPrice: item.price
      })),
      totalAmount: this.totalPrice,
      paymentMethod: 'Credit',
      receiver: {
        name: this.receiver.name,
        phone: this.receiver.phone,
        email: this.receiver.email,
        address: this.receiver.address
      }
    };

    this.orderService.createOrder(orderRequest).subscribe({
      next: (res: any) => {
        console.log('訂單建立成功！', res);

        const payData = res.paymentData; // 先把這包拿出來

        if (payData) {
          this.paymentData.MerchantID = payData.merchantID;
          this.paymentData.TradeInfo = payData.tradeInfo;
          this.paymentData.TradeSha = payData.tradeSha;
          this.paymentData.Version = payData.version;

          // 如果後端有回傳網址，就更新網址；沒有就用預設的
          if (payData.newebPayUrl) {
            this.paymentUrl = payData.newebPayUrl;
          }
        }

        // 清空購物車
        this.cartItems = [];
        this.totalPrice = 0;
        this.cartService.clearCartState();

        // 關閉 Loading 視窗
        Swal.close();

        // 直接操作 DOM 元素
        setTimeout(() => {
          const formElement = this.paymentForm.nativeElement;

          // 1. 強制把網址塞進去 (不管 HTML 綁定更新了沒)
          // ⚠️ 注意：如果 res 裡的網址是 null，這裡要用預設網址
          formElement.action = this.paymentUrl;

          // 2. 送出！
          formElement.submit();
        }, 100);
      },
      error: (err) => {
        console.error('結帳失敗', err);
        Swal.fire({
          title: '結帳發生錯誤',
          text: '系統暫時無法建立訂單，請稍後再試 😢',
          icon: 'error',
          confirmButtonText: '確定',
          confirmButtonColor: '#d33'
        });
      }
    });
  }
  // -- 刪除商品功能 --
  deleteItem(item: any) {
    // 1. 先用 SweetAlert 問使用者確定嗎？(避免誤觸)
    Swal.fire({
      title: '確定要移除嗎？',
      text: `要把 ${item.productName} 趕出購物車嗎？🥺`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: '狠心移除',
      cancelButtonText: '再考慮一下',
      confirmButtonColor: '#d33', // 紅色按鈕表示危險
      cancelButtonColor: '#3085d6'
    }).then((result) => {

      if (result.isConfirmed) {
        // 2. 使用者按了「確定」，才呼叫 Service
        this.isLoading = true;

        this.cartService.removeCartItem(item.productId).subscribe({
          next: () => {
            // 3. 刪除成功！
            this.isLoading = false;

            // 顯示成功訊息
            Swal.fire({
              title: '已移除',
              text: '商品已離開購物車 👋',
              icon: 'success',
              timer: 1500,
              showConfirmButton: false
            });

            // 4. ★ 關鍵：重新載入購物車！
            // 這樣畫面會更新，總金額會重算，右上角數字也會更新！
            // 我們剛剛重構的 loadCart 在這裡派上用場了！✨
            this.loadCart();
          },
          error: (err) => {
            console.error('刪除失敗', err);
            this.isLoading = false;
            Swal.fire('刪除失敗', '系統發生錯誤，請稍後再試', 'error');
          }
        });
      }
    });
  }
  fillDemoData() {
    this.receiver.name = '測試員';
    this.receiver.phone = '0912345678';
    this.receiver.email = 'demo@petsociety.com';
    this.receiver.address = '106臺北市大安區復興南路一段390號2樓';
  }
}
