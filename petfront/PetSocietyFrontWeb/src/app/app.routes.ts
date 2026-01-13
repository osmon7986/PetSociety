import { ChapterQuizComponent } from './features/course/pages/chapter-quiz/chapter-quiz.component';
import { Routes } from '@angular/router';
import { TestTokenComponent } from './core/test/test-token/test-token.component';
import { AuthGuard } from './core/guards/auth.guard';


export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/home/home.component').then(m => m.HomeComponent)
  },
  // 當有人(或是Guard) 呼叫 '/login' 時，自動轉去 '/member/login'
  { path: 'login', redirectTo: 'member/login', pathMatch: 'full' },

  {
    path: 'academy',
    children: [
      {
        path: '', // 課程主頁
        loadComponent: () => import('./features/course/pages/course-index/course-index.component').then(m => m.CourseIndexComponent)
      },
      {
        path: 'course/:courseDetailId', // 課程明細頁
        loadComponent: () => import('./features/course/pages/course-detail/course-detail.component').then(m => m.CourseDetailComponent)
      },
      {
        path: 'payment/success', // 課程付款成功頁
        canActivate: [AuthGuard],
        loadComponent: () => import('./features/course/pages/course-payment-success/course-payment-success.component').then(m => m.CoursePaymentSuccessComponent)
      },
      {
        path: 'my-course',
        canActivate: [AuthGuard],
        children: [
          {
            path: '', // 我的課程
            loadComponent: () => import('./features/course/pages/my-course/my-course.component').then(m => m.MyCourseComponent),
          },
          {
            path: ':courseDetailId',
            children: [
              {
                path: 'playback', // 課程播放頁
                loadComponent: () => import('./features/course/pages/course-playback/course-playback.component').then(m => m.CoursePlaybackComponent),
              },
              {
                path: 'quiz/:chapterId', // 章節測驗
                loadComponent: () => import('./features/course/pages/chapter-quiz/chapter-quiz.component').then(m => m.ChapterQuizComponent)
              },
            ]
          },
        ]
      },
    ]
  },
  {
    path: 'forum',
    // loadComponent: () => import('./features/forum/forum.component').then(m => m.ForumComponent)
    children: [
      {
        path: '', // 這對應 /forum
        loadComponent: () => import('./features/forum/forum.component').then(m => m.ForumComponent)
      },
      {
        path: 'favorites', // 這對應 /forum/favorites
        loadComponent: () => import('./features/forum/favorite-list.component').then(m => m.FavoriteListComponent)
      }
    ]
  },
  {
    path: 'mall',
    children: [
      // 商城首頁
      {
        path: '',
        loadComponent: () => import('./features/mall/mall.component').then(m => m.MallComponent)
      },
      // 購物車頁面
      {
        path: 'cart',
        canActivate: [AuthGuard],
        loadComponent: () => import('./features/mall/pages/cart/cart.component').then(m => m.CartComponent)
      },
      {
        path: 'payment-success',
        canActivate: [AuthGuard],
        loadComponent: () => import('./features/mall/pages/payment-success/payment-success.component').then(m => m.PaymentSuccessComponent)
      }
      , {
        path: 'orders',
        canActivate: [AuthGuard],
        loadComponent: () => import('./features/mall/pages/order-history/order-history.component').then(m => m.OrderHistoryComponent)
      }
    ]
  },
  {
    path: 'activity',
    // 子路由邏輯
    children: [
      {
        path: 'intro',
        loadComponent: () => import('./features/activity/pages/intro/intro.component').then(m => m.IntroComponent)
      },
      {
        path: 'guide/:id',
        loadComponent: () => import('./features/activity/pages/registration-guide/registration-guide.component').then(m => m.RegistrationGuideComponent)
      },
      {
        path: 'registration',
        loadComponent: () => import('./features/activity/pages/registration/registration.component').then(m => m.RegistrationComponent)
      },
      {
        path: 'calender',
        loadComponent: () => import('./features/activity/pages/calender/calender.component').then(m => m.CalenderComponent)

      },
      {
        path: 'apply',
        children: [

        ],
        loadComponent: () => import('./features/activity/pages/apply/apply.component').then(m => m.ApplyComponent)

      },
    ],
    loadComponent: () => import('./features/activity/activity.component').then(m => m.ActivityComponent)
  },
  {
    path: 'member',
    loadChildren: () => import('./features/member/member.routing.module').then(m => m.MemberRoutingModule)
  },
  { path: 'test-token', component: TestTokenComponent },
  {
    path: '**',
    loadComponent: () => import('./features/home/home.component').then(m => m.HomeComponent)
  }, // 測試用
];
