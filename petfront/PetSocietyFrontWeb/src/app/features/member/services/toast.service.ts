import { Injectable } from '@angular/core';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class ToastService {

  constructor() { }

  // 成功通知 (綠色勾勾)
  success(message: string, title: string = '成功') {
    const Toast = Swal.mixin({
      toast: true,
      position: 'top-end',
      showConfirmButton: false,
      timer: 2000,
      timerProgressBar: false,

      didOpen: (toast) => {
        toast.onmouseenter = Swal.stopTimer;
        toast.onmouseleave = Swal.resumeTimer;
      }
    });

    Toast.fire({
      icon: 'success',
      title: message
    });
  }

  // 錯誤通知 (紅色叉叉)
  error(message: string, title: string = '錯誤') {
    const Toast = Swal.mixin({
      toast: true,
      position: 'top-end',
      showConfirmButton: false,
      timer: 2000,
      timerProgressBar: false,
    });

    Toast.fire({
      icon: 'error',
      title: message
    });
  }

  warning(message: string) {
    const Toast = Swal.mixin({
      toast: true,
      position: 'top-end',
      showConfirmButton: false,
      timer: 2000,
      timerProgressBar: false,
    });


    Toast.fire({
      icon: 'warning',
      title: message
    });
  }

  info(message: string) {
    const Toast = Swal.mixin({
      toast: true,
      position: 'top-end',
      showConfirmButton: false,
      timer: 2000,
      timerProgressBar: false,
    });

    Toast.fire({
      icon: 'info',
      title: message
    });
  }
}



