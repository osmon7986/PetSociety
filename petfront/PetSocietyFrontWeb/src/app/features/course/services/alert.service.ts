import { inject, Injectable } from '@angular/core';
import { ConfirmationService } from 'primeng/api';

@Injectable({
  providedIn: 'root'
})
export class AlertService {

  private confirmationService = inject(ConfirmationService)

  // PrimeNG 建立一個通用的 confirm 方法
  confirm(options: {
    header?: string,
    message?: string,
    icon?: string,
    acceptLabel?: string,
    rejectLabel?: string,
    rejectVisible?: boolean
  }): Promise<boolean> {
    return new Promise((resolve) => {
      this.confirmationService.confirm({
        header: options.header || '確認視窗',
        message: options.message || '您確定要執行此操作嗎？',
        icon: options.icon || 'pi pi-exclamation-triangle',
        acceptLabel: options.acceptLabel || '確定',
        rejectLabel: options.rejectLabel || '取消',
        rejectVisible: options.rejectVisible ?? true,

        rejectButtonProps: {
          severity: 'secondary',
          outlined: true,
        },

        accept: () => resolve(true),
        reject: () => resolve(false)
      });
    });
  }

}
