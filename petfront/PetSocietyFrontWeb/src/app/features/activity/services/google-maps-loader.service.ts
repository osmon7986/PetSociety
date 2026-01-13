import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, map, Observable, of, shareReplay, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class GoogleMapsLoaderService {

  private httpClient = inject(HttpClient);

  // 用來快取 API 的請求，確保只發送一次
  private api$: Observable<boolean> | undefined;

  load(): Observable<boolean> {
    // 檢查是否已經在下載中或下載過了 (Service 的記憶體)
    if (this.api$) {
      return this.api$;
    }

    // 檢查全域變數 (防止切換頁面時的殘留)
    if ((window as any).google && (window as any).google.maps) {
      // 雖然已經有了，但為了配合 Observable 格式，我們存一個已經完成的 Observable
      this.api$ = of(true);
      return this.api$;
    }

    const apiKey = environment.googleMaps.apiKey;


    this.api$ = this.httpClient.jsonp(
      `https://maps.googleapis.com/maps/api/js?key=${apiKey}&libraries=marker,places&v=weekly`,
      "callback"
    ).pipe(
      tap(() => console.log('Google Maps API 下載完成')),
      map(() => true),
      // 下一個 component 再呼叫 load() 時，會直接拿到結果，不會再發請求
      shareReplay(1),
      catchError((err) => {
        console.error('API 下載失敗', err);
        return of(false);
      })
    );

    return this.api$;
  }
}
