import { MallService } from './../../services/mall.service';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../services/order.service';
import { Router, RouterModule } from '@angular/router';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { AccordionModule } from 'primeng/accordion';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-order-history',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    TableModule,
    TagModule,
    ButtonModule,
    CardModule,
    AccordionModule
  ],
  templateUrl: './order-history.component.html',
  styleUrl: './order-history.component.css'
})
export class OrderHistoryComponent implements OnInit {

  orders: any[] = [];
  isLoading: boolean = true;

  constructor(
    private orderService: OrderService,
    private router: Router,
    private MallService: MallService
  ) { }

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders() {
    this.isLoading = true;

    this.orderService.getMyOrders().subscribe({
      next: (data) => {
        console.log('訂單資料回來囉:', data);
        // 為了讓畫面好看，我們可以依照時間排序 (新的在上面)
        this.orders = data.sort((a, b) => new Date(b.createDate).getTime() - new Date(a.createDate).getTime());

        this.isLoading = false;
      },
      error: (err) => {
        console.error('拿不到訂單 QQ', err);
        this.isLoading = false;
      }
    });
  }

  // -- 狀態翻譯機 --
  getStatusName(status: string): string {
    switch (status) {
      case 'Pending': return '待付款';   // 或 Unpaid
      case 'Paid': return '已付款';
      case 'Processing': return '已付款';
      case 'Shipped': return '已出貨';
      case 'Completed': return '已完成';
      case 'Cancelled': return '已取消';
      default: return status; // 如果沒對應到，就顯示原本的英文
    }
  }

  // -- 狀態顏色設定 --
  getStatusSeverity(status: string): "success" | "info" | "warning" | "danger" | "secondary" | "contrast" | undefined {
    switch (status) {
      case 'Pending': return 'warning';  // 待付款用黃色警告
      case 'Paid': return 'success';     // 已付款用綠色
      case 'Processing': return 'success';
      case 'Shipped': return 'info';
      case 'Completed': return 'success';
      case 'Cancelled': return 'danger';
      default: return 'secondary';
    }
  }
  // -- 立即付款 (補救措施) --
  payNow(orderId: number) {
    this.isLoading = true; // 開啟遮罩

    this.MallService.getPaymentInfo(orderId).subscribe({
      next: (paymentData) => {
        console.log('後端回傳的金流資料:', paymentData);
        // paymentData 應該包含: MerchantID, TradeInfo, TradeSha, Version, ActionUrl

        // ★ 動態建立一個 form 來送出資料給藍新
        const form = document.createElement('form');
        form.method = 'post';
        form.action = paymentData.actionUrl || paymentData.ActionUrl || paymentData.NewebPayUrl; // 藍新網址
        // 如果還是沒抓到，就跳錯誤避免白畫面
        if (!form.action) {
          console.error('金流網址是空的！請檢查後端回傳的欄位名稱');
          this.isLoading = false;
          return;
        }
        // 塞入參數
        const inputs = [
          { name: 'MerchantID', value: paymentData.merchantID || paymentData.MerchantID },
          { name: 'TradeInfo', value: paymentData.tradeInfo || paymentData.TradeInfo },
          { name: 'TradeSha', value: paymentData.tradeSha || paymentData.TradeSha },
          { name: 'Version', value: paymentData.version || paymentData.Version }
        ];

        inputs.forEach(param => {
          const input = document.createElement('input');
          input.type = 'hidden';
          input.name = param.name;
          input.value = param.value;
          form.appendChild(input);
        });

        document.body.appendChild(form);
        form.submit();
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
        Swal.fire('錯誤', '無法取得付款資訊，請稍後再試', 'error');
      }
    });
  }
  // -- 取消訂單 --
  onCancelOrder(orderId: number): void {
    Swal.fire({
      title: '確定要取消訂單嗎？',
      text: '取消後，商品將會重新釋出給其他顧客購買喔！', // 提示庫存會釋出
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: '確定取消',
      cancelButtonText: '我再想想',
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6'
    }).then((result) => {
      if (result.isConfirmed) {

        this.isLoading = true; // 開啟遮罩

        this.MallService.cancelOrder(orderId).subscribe({
          next: (res) => {
            this.isLoading = false;

            // 成功提示
            Swal.fire('已取消', '訂單已取消，庫存已恢復！', 'success');

            // 🔥 重點：重新撈一次資料，畫面上的狀態才會變更！
            // 如果你是直接寫在 ngOnInit，建議抽成一個 this.loadOrders() 方法方便呼叫
            this.loadOrders();
          },
          error: (err) => {
            console.error(err);
            this.isLoading = false;
            // 顯示後端傳回來的錯誤訊息 (例如：不是 Pending 狀態)
            Swal.fire('失敗', err.error?.message || '取消訂單時發生錯誤', 'error');
          }
        });
      }
    });
  }
  // -- 刪除訂單紀錄 (軟刪除) --
  onDeleteOrder(orderId: number): void {
    Swal.fire({
      title: '刪除訂單紀錄？',
      text: '這只會從您的列表中隱藏，不會真的刪除資料庫紀錄喔！',
      icon: 'question',
      showCancelButton: true,
      confirmButtonText: '隱藏它',
      cancelButtonText: '留著吧',
      confirmButtonColor: '#6c757d', // 灰色
      cancelButtonColor: '#3085d6'
    }).then((result) => {
      if (result.isConfirmed) {
        this.isLoading = true;
        this.MallService.deleteOrder(orderId).subscribe({
          next: () => {
            this.isLoading = false;
            Swal.fire('已刪除', '訂單紀錄已隱藏', 'success');
            this.loadOrders(); // 重新撈資料，該筆訂單就會消失了
          },
          error: (err) => {
            this.isLoading = false;
            Swal.fire('失敗', err.error?.message || '刪除失敗', 'error');
          }
        });
      }
    });
  }
}

