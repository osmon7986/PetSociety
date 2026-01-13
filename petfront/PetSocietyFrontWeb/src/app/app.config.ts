import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi, withJsonpSupport } from '@angular/common/http';
import { AuthInterceptor } from './core/interceptors/token.interceptor';
import { routes } from './app.routes';
import { providePrimeNG } from 'primeng/config';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { definePreset } from '@primeng/themes';
import Aura from '@primeng/themes/aura';
import { ConfirmationService, MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';

const myPreset = definePreset(Aura, {
  semantic: {
    primary: {
      50: '#fdfaf5',
      100: '#f7f1e6',
      200: '#efdfcc',
      300: '#e3c6a3',
      400: '#D8C3A5',  // 主色
      500: '#C9B29B', // 核心色
      600: '#F2EBE5', // Hover 色
      700: '#a38f7a',
      800: '#7d6d5d',
      900: '#53493e',
      950: '#2b2620'
    }
  }
});

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(withJsonpSupport()),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    // 註冊 Interceptor
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true // 允許有多個 Interceptor
    },
    provideAnimationsAsync(), // 使用動畫
    providePrimeNG({ // 套用 PrimeNG 主題
      theme: {
        preset: myPreset,
        options: {
          darkModeSelector: false || 'none' // 關閉深色模式
        }
      }
    }),
    ConfirmationService,
    MessageService,
    DialogService,

  ]
};
