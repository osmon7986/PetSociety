import { ShowMarker } from './../../interface/showMarker';
import { ActivityService } from './../../services/activity.service';
import { AfterViewInit, Component, CUSTOM_ELEMENTS_SCHEMA, effect, ElementRef, EventEmitter, inject, Input, NgZone, Output, SimpleChanges, ViewChild, ViewEncapsulation } from '@angular/core';
import { GoogleMapsModule, MapAdvancedMarker, MapInfoWindow } from '@angular/google-maps'
import { catchError, map, Observable, of } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { GoogleMapsLoaderService } from '../../services/google-maps-loader.service';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-activity-map',
  imports: [GoogleMapsModule,
    AsyncPipe,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './activity-map.component.html',
  styleUrl: './activity-map.component.css',
})
export class ActivityMapComponent implements AfterViewInit {
  //判斷是否顯示搜尋框
  @Input() showSearch: boolean = false;
  @Input() showInfoWindow: boolean = false;

  showMap: ShowMarker[] = [];


  private activityService = inject(ActivityService);
  private activatedRoute = inject(ActivatedRoute);


  @ViewChild(MapInfoWindow) infoWindow!: MapInfoWindow;

  @ViewChild(MapAdvancedMarker)
  set marker(markerInstance: MapAdvancedMarker) {
    if (markerInstance && this.infoWindow) {
      // 標記一出現，立刻開啟視窗
      this.infoWindow.open(markerInstance);
    }
  }

  //抓取 HTML 裡的輸入框
  @ViewChild('addressInput') addressInput?: ElementRef;

  //定義 Output：把結果傳給父層 (地址 + 經緯度)
  @Output() locationPicked = new EventEmitter<{ address: string, lat: number, lng: number }>();

  mapId = environment.googleMaps.mapId;
  center: google.maps.LatLngLiteral = {
    //起始位置台北101
    lat: 25.0339,
    lng: 121.5644
  };
  zoom = 15;
  selectedPos: google.maps.LatLngLiteral | null = null;
  //傳入活動資料
  RedMark =
    {
      title: '台北 101 活動',
      pos: { lat: 25.0339, lng: 121.5644 },
      label: '活動地點'
    }
  info = {
    title: this.RedMark.title,
    label: this.RedMark.label
  };

  //匯入資料
  private mapsLoader = inject(GoogleMapsLoaderService);

  apiLoaded: Observable<boolean> = this.mapsLoader.load();

  openInfoWindow(marker: MapAdvancedMarker) {

    this.infoWindow.open(marker);
  }

  // 用來存使用者點擊的座標 (給 HTML 顯示用)
  clickedPos: google.maps.LatLngLiteral | null = null;

  // --- 核心方法：抓取經緯度 ---
  onMapClick(event: google.maps.MapMouseEvent) {
    if (this.showInfoWindow) { return; } // 如果在簡章頁就不處理地圖點擊事件
    // event.latLng 包含了座標物件
    if (event.latLng) {
      const lat = event.latLng.lat();
      const lng = event.latLng.lng();

      const geocoder = new google.maps.Geocoder();

      geocoder.geocode({ location: { lat, lng } }, (results, status) => {

        if (status === 'OK' && results && results[0]) {
          const address = results[0].formatted_address;
          this.ngZone.run(() => {
            // 通知父層
            this.locationPicked.emit({ address, lat, lng });
            if (this.addressInput) {
              this.addressInput.nativeElement.value = address;
            }
          });
        } else {
          console.error('反向地理編碼失敗:', status);
          this.ngZone.run(() => {
            this.locationPicked.emit({ address: '未知地點', lat, lng });
          });
        }
      });
      // 將座標存起來，更新畫面上的暫時標記
      this.RedMark.pos = { lat, lng };
    }
  }
  ngAfterViewInit() {
    this.apiLoaded.subscribe(() => {
      // 確保 input 已經長出來了
      setTimeout(() => {
        if (this.showSearch && this.addressInput) {
          this.initLegacyAutocomplete();
        }
      }, 500);
    });
  }

  private ngZone = inject(NgZone);

  initLegacyAutocomplete() {
    const inputElement = this.addressInput!.nativeElement;


    const autocomplete = new google.maps.places.Autocomplete(inputElement, {
      componentRestrictions: { country: 'tw' },
      fields: ['formatted_address', 'geometry', 'name'],
      types: ['establishment', 'geocode']
    });

    // 監聽選擇事件
    autocomplete.addListener('place_changed', () => {
      this.ngZone.run(() => { // 還是要用 NgZone 確保畫面更新
        const place = autocomplete.getPlace();

        if (!place.geometry || !place.geometry.location) {
          window.alert("找不到該地點的詳細資訊: '" + place.name + "'");
          return;
        }

        const lat = place.geometry.location.lat();
        const lng = place.geometry.location.lng();
        const address = place.formatted_address || place.name || '';

        this.RedMark.pos = { lat, lng };
        // 更新地圖
        this.center = { lat, lng };
        this.selectedPos = { lat, lng };
        this.zoom = 17;

        // 通知父層
        this.locationPicked.emit({ address, lat, lng });
      });
    });
  }
  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((data: ParamMap) => {
      //自url獲得活動id
      const inputId = data.get('id')
      var activityId = inputId ? + inputId : 0

      if (this.showInfoWindow == true) {
        this.activityService.getMap(activityId)
          .subscribe(data => {
            console.log(data);
            this.RedMark.pos = { lat: data.latitude, lng: data.longitude };
            this.info.title = data.title;
            this.info.label = data.location;
            // 更新地圖
            this.center = { lat: data.latitude, lng: data.longitude };

          });
      }

    });

  }

}
