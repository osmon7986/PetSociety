import { Router } from '@angular/router';
import { AuthService } from './../../../../core/auth/auth.service';
import { ActivityService } from './../../services/activity.service';
import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivityApply } from '../../interface/activityApply';
import { EditorComponent } from "../../../../shared/editor/editor.component";
import { CreateGuideData } from '../../interface/activityGuideData';
import Swal from 'sweetalert2';
import { debounceTime, merge, Subject } from 'rxjs';
import { ToastService } from '../../../member/services/toast.service';
import { ActivityMapComponent } from "../activity-map/activity-map.component";
import { testContent } from '../../../../../../testcontent';

@Component({
  selector: 'app-activity-applyform',
  imports: [ReactiveFormsModule, EditorComponent, ActivityMapComponent],
  templateUrl: './activity-applyform.component.html',
  styleUrl: './activity-applyform.component.css'
})
export class ActivityApplyformComponent implements OnInit {
  activityForm!: FormGroup;
  memberId!: number;
  parentHtmlContent = '請輸入簡章內容...';
  readonly Storage_form_key = 'activity-applyform'
  private editorChange$ = new Subject<string>();
  isLogged = signal(false);


  constructor(private fb: FormBuilder,
    private activityService: ActivityService,
    private authService: AuthService,
    private router: Router,
    private toast: ToastService
  ) { }


  ngOnInit(): void {

    this.activityForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      organizerName: ['', Validators.required],
      description: ['', Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required],
      location: ['', Validators.required],
      maxCapacity: [1, [Validators.required, Validators.min(1)]],
      registrationStartTime: ['', Validators.required],
      registrationEndTime: ['', Validators.required],
      lat: [0, Validators.required],
      lng: [0, Validators.required]
    });

    this.loadDraft();

    merge(
      this.editorChange$,
      this.activityForm.valueChanges
    ).pipe(debounceTime(1000)).subscribe(() => {
      this.saveDraft();
    });
  }
  //HTML 綁定 (onContentChange) 的方法
  onReceiveEditorContent(content: string) {
    // 更新變數 (讓畫面同步)
    this.parentHtmlContent = content;
    //手動把收到的值，推入 RxJS
    this.editorChange$.next(content);
  }


  example() {
    const setActivity = {
      title: "2026 毛孩春季運動會",
      organizerName: "寵物社會企業有限公司",
      description: "春暖花開的季節，帶著你家毛孩一同前往運動會盡情運動。",
      startTime: "2026-03-20T09:00:00",
      endTime: "2026-03-20T17:00:00",
      location: "台中市民廣場",
      maxCapacity: 100,
      registrationStartTime: "2026-03-01T10:00:00",
      registrationEndTime: "2026-03-15T23:59:59",
      lat: 24.1477,
      lng: 120.6736,
    };

    // 使用 setValue 填入，必須包含以上所有欄位
    this.activityForm.setValue(setActivity);
    this.parentHtmlContent = testContent;
  }

  back() {
    //todo:增加退出詢問
    Swal.fire({
      title: '退出活動建立',
      text: '退出後所有資料將會消失，確定要離開嗎?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: '確定',
      cancelButtonText: '取消',
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      reverseButtons: true,
      allowOutsideClick: false, // 禁止點擊外部關閉
      allowEscapeKey: false     // 禁止按 ESC 關閉
    }).then((result) => {
      if (result.isConfirmed) {
        this.clearDraft();
        this.router.navigate(['activity', 'intro'])
      }
    })
  }



  //儲存暫存
  saveDraft() {
    const draft = {
      form: this.activityForm.value,       // 左邊表單的資料
      context: this.parentHtmlContent  // 右邊簡章的內容
    };
    sessionStorage.setItem(this.Storage_form_key, JSON.stringify(draft));
    console.log('草稿已暫存', draft); // Debug 用
  }

  //讀取暫存
  loadDraft() {
    const draftJson = sessionStorage.getItem(this.Storage_form_key);
    if (draftJson) {
      const draft = JSON.parse(draftJson);

      // 回填表單 (patchValue 會自動對應欄位名稱)
      // 修正：分開判斷，避免 context 為空字串 (falsy) 時導致表單資料無法回填
      if (draft.form) {
        this.activityForm.patchValue(draft.form);
      }
      if (draft.context !== undefined) {
        this.parentHtmlContent = draft.context;
      }
    }


  }

  clearDraft() {
    sessionStorage.removeItem(this.Storage_form_key);
  }

  onLocationSelected(data: { address: string, lat: number, lng: number }) {

    // patchValue 可以一次更新多個欄位
    this.activityForm.patchValue({
      location: data.address,
      lat: data.lat,
      lng: data.lng
    });
  }

  onSubmit(): void {

    //檢查會員登入
    if (!this.authService.isLoggedIn()) {
      Swal.fire({
        icon: 'warning',
        title: '請先登入',
        text: '系統已暫存您填寫的內容。請先登入會員，以繼續報名流程。',
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 6000,
        timerProgressBar: true
      });
      this.router.navigate(['member', 'login'])
      return;
    }

    //檢查簡章內容輸入
    if (!this.parentHtmlContent) {
      this.toast.warning('簡章內容沒有被撰寫或變更')
      return;
    }
    //檢查表單有效性
    if (this.activityForm.valid) {
      const formValue = this.activityForm.value;
      const newActivity: ActivityApply = {
        title: formValue.title,
        organizerName: formValue.organizerName,
        description: formValue.description,
        startTime: formValue.startTime,
        endTime: formValue.endTime,
        location: formValue.location,
        maxCapacity: formValue.maxCapacity,
        registrationStartTime: formValue.registrationStartTime,
        registrationEndTime: formValue.registrationEndTime,
        latitude: formValue.lat,
        longitude: formValue.lng
      }

      const newActivityGuide: CreateGuideData = {
        activityEditorHtml: this.parentHtmlContent
      }

      this.activityService.createActivity(newActivity, newActivityGuide).subscribe({
        next: (res) => {
          this.clearDraft();
          console.log('活動建立成功！', res);
          //修改成toast
          this.toast.success('建立成功，活動名稱為: ' + res.title + '\n'
            + '請等待審核'
          );
          Swal.fire({
            icon: 'success',
            title: '建立成功',
            text: '活動名稱為: ' + res.title + '\n' + '請等待審核',
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 6000, // 設定顯示時間為 6 秒
            timerProgressBar: true
          });
          this.clearDraft();
          this.router.navigate(['activity', 'intro'])
        },
        error: (err) => {
          this.toast.error('建立失敗，請聯絡工作人員')
          console.error('發生錯誤：', err);
        }
      });
    } else {
      //標記所有欄位為Touched以顯示錯誤訊息
      //todo:錯誤訊息
      this.toast.warning('空值判斷')
      this.activityForm.markAllAsTouched();
    }
  }

}
