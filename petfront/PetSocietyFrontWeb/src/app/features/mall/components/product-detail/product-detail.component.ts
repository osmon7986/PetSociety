import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { GalleriaModule } from 'primeng/galleria';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { InputNumberModule } from 'primeng/inputnumber';

import { CartService } from '../../services/cart.service';
import { MessageService } from 'primeng/api';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-product-detail',
  imports: [CommonModule, FormsModule, GalleriaModule, InputNumberModule, ButtonModule, TagModule],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.css'
})
export class ProductDetailComponent implements OnInit {
  product: any;          // 存整包商品資料
  quantity: number = 1;  // 存使用者選的數量
  images: any[] = [];    // 存處理過的圖片格式
  responsiveOptions = [
    { breakpoint: '1024px', numVisible: 5 },
    { breakpoint: '768px', numVisible: 3 },
    { breakpoint: '560px', numVisible: 1 }
  ];

  constructor(
    public config: DynamicDialogConfig, // 拿資料
    public ref: DynamicDialogRef,       // 關視窗
    private cartService: CartService,
    private messageService: MessageService, // 跳通知
    private router: Router
  ) { }

  ngOnInit() {
    // 從 config 拿到傳進來的 product 資料
    if (this.config.data) {
      this.product = this.config.data;
      // 處理圖片：把字串陣列 ['url1', 'url2'] 轉成物件陣列 [{itemImageSrc: 'url1'}, ...]
      // PrimeNG 的 Galleria 需要物件格式才能運作
      if (this.product.images && this.product.images.length > 0) {
        this.images = this.product.images.map((img: string) => ({
          itemImageSrc: img,
          alt: this.product.productName
        }));
      } else {
        // 如果完全沒圖，給一張預設圖
        this.images = [{ itemImageSrc: 'assets/images/no-image.png', alt: 'No Image' }];
      }
    }
  }
  // 加入購物車
  addToCart() {
    // 1. 取得目前的 token
    const token = localStorage.getItem('jwtToken');

    if (token) {
      // --- 情境 A：已登入 ---
      console.log('正在呼叫後端 API...', this.product); // ★ 改成 this.product

      // ★ 參數改成：this.product.productId 和 this.quantity
      this.cartService.addToCart(this.product.productId, this.quantity).subscribe({
        next: (response) => {
          console.log('後端回傳成功:', response);

          // 成功提示
          Swal.fire({
            title: '已加入購物車',
            text: `商品 : ${this.product.productName}`, // ★ 改成 this.product
            icon: 'success',
            timer: 1500,
            showCancelButton: false
          });

          // 通知購物車數字 +1 (或 +數量)
          this.cartService.incrementCartCount(this.quantity); // ★ 這裡建議加 this.quantity

          // ★★★ 關鍵：加入成功後，自動關閉彈窗！ ★★★
          this.ref.close();
        },
        error: (err) => {
          console.error('後端回傳錯誤:', err);
          // 如果是 401 錯誤，代表 Token 過期，也可以在這裡引導去登入
          Swal.fire('加入失敗', '系統連線錯誤', 'error');
        }
      });

    } else {
      // --- 情境 B：未登入 ---
      // 這裡直接用你提供的 SweetAlert 代碼，完美！
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
          // ★★★ 關鍵：跳轉前，先把目前的彈窗關掉，不然會擋在登入頁前面很醜 ★★★
          this.ref.close();

          // 跳轉去登入
          this.router.navigate(['/login']);
        }
      });
    }
  }

  // 判斷是否為 14 天內的新品
  isNew(dateString: string): boolean {
    if (!dateString) return false;
    const createDate = new Date(dateString);
    const today = new Date();
    const diffTime = Math.abs(today.getTime() - createDate.getTime());
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays <= 14;
  }
}
