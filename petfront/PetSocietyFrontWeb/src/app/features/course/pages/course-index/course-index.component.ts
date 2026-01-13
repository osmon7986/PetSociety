import { Component, OnInit, signal } from '@angular/core';
import { CourseService } from '../../services/course.service';
import { CourseCardComponent } from "../../components/course-card/course-card.component";
import { BreadcrumbItem } from '../../../../shared/interfaces/breadcrumb-items';
import { CoursePaged } from '../../interfaces/course-paged';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { SelectModule } from 'primeng/select';
import { CourseQuery } from '../../interfaces/course-query';
import { InputIcon } from 'primeng/inputicon';
import { IconField } from 'primeng/iconfield';
import { InputTextModule } from 'primeng/inputtext';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, single } from 'rxjs';
import { CarouselModule } from 'primeng/carousel';
import { CourseCategory } from '../../interfaces/course-category';
import { DatePipe, DecimalPipe } from '@angular/common';
import { CourseIndex } from '../../interfaces/course-index';
import { Router, RouterModule } from '@angular/router';
import Swal from 'sweetalert2';
import { MemberService } from '../../../member/services/member.service';
import { AuthService } from '../../../../core/auth/auth.service';


@Component({
  selector: 'app-course-index',
  imports: [RouterModule, CourseCardComponent, PaginatorModule,
    SelectModule, InputIcon, IconField, InputTextModule, FormsModule,
    ReactiveFormsModule, DatePipe, CarouselModule, DecimalPipe],
  templateUrl: './course-index.component.html',
  styleUrl: './course-index.component.css'
})
export class CourseIndexComponent implements OnInit {

  hotImgLoaded = signal(false);
  courseImgLoaded = signal(false);

  breadcrumbData: BreadcrumbItem[] = [{ label: '學院', link: '/academy' }];
  responsiveOptions: any[] | undefined;
  categoryTitle: string = '全部';
  categories: CourseCategory[] = [];
  hotCourse: CourseIndex[] = [];
  pagedCourse: CoursePaged = { items: [], page: 0, pageSize: 0, totalCount: 0, totalPages: 0, lastUpdated: '' };
  query: CourseQuery = { pageIndex: 1, pageSize: 8, search: '', categoryId: 0 };
  searchControl = new FormControl('');
  first: number = 0;
  rows: number = 8;

  constructor(
    private courseService: CourseService,
    private memberService: MemberService,
    private authService: AuthService,
    private router: Router
  ) {
    this.responsiveOptions = [
      { breakpoint: '1400px', numVisible: 2, numScroll: 2 },
      { breakpoint: '1199px', numVisible: 2, numScroll: 2 },
      { breakpoint: '767px', numVisible: 1, numScroll: 1 }
    ];
  }

  ngOnInit(): void {
    this.courseService.getCourseCategory().subscribe((data) => {
      this.categories = data;
    })
    this.loadCourses();
    // 載入熱門課程
    this.courseService.getHotCourse().subscribe((data) => {
      if (data) {
        this.hotCourse = data;
      }
    })

    this.searchControl.valueChanges.pipe(
      debounceTime(400),
      distinctUntilChanged(),
    ).subscribe((val) => {
      this.query.search = val ?? '';
      this.query.pageIndex = 1;
      this.first = 0;
      this.loadCourses();
    });
  }

  selectCategory(id: number, name: string) {
    this.query.categoryId = id;
    if (id !== undefined) this.categoryTitle = name;
    this.loadCourses();
  }

  onPageChange(event: PaginatorState) {
    const newFirst = event.first ?? 0;
    if (newFirst !== this.first) {
      this.first = event.first ?? 0;
      this.rows = event.rows ?? 8;
      this.query.pageIndex = (this.first / this.rows) + 1;
      this.loadCourses();
    }
  }

  loadCourses() {
    this.courseService.getPagedCourse(this.query).subscribe((data) => {
      data.items = data.items.map((item: any) => ({ ...item, isFavorite: item.isFavorite || false }));
      this.pagedCourse = data;
    })
  }

  hotOnLoad() {
    this.hotImgLoaded.set(true);
  }




  // 收藏切換邏輯
  toggleFavorite(course: any) {
    // 1. 檢查登入
    if (!this.authService.isLoggedIn()) {
      Swal.fire({
        title: '請先登入',
        text: '登入後才能收藏課程喔！',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: '前往登入',
        cancelButtonText: '先不要',
      }).then((res) => {
        if (res.isConfirmed) this.router.navigate(['/member/login']);
      });
      return;
    }

    // 2. 樂觀 UI 更新 (不用等後端回傳)
    const originalStatus = course.isFavorite;
    course.isFavorite = !course.isFavorite;

    const TARGET_TYPE_COURSE = 3;

    if (course.isFavorite) {
      // --- 加入收藏 ---
      this.memberService.addFavorite(
        course.courseDetailId,
        TARGET_TYPE_COURSE,
        course.title,
        course.price || 0,
        course.imageUrl
      ).subscribe({
        next: () => {
          const Toast = Swal.mixin({
            toast: true, position: 'top-end', showConfirmButton: false, timer: 1500
          });
          Toast.fire({ icon: 'success', title: '已加入收藏' });
        },
        error: (err) => {
          course.isFavorite = originalStatus;
          console.error(err);
          Swal.fire('加入失敗', '系統繁忙請稍後再試', 'error');
        }
      });
    } else {
      // --- 取消收藏  ---
      this.memberService.removeFavoriteByTarget(
        course.courseDetailId,
        TARGET_TYPE_COURSE
      ).subscribe({
        next: () => { /* 成功 */ },
        error: (err) => {
          course.isFavorite = originalStatus;
          console.error(err);
        }
      });
    }
  }
}
