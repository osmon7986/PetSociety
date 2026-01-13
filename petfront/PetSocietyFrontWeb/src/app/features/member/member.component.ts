import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router, RouterModule, RouterOutlet, NavigationEnd, Event } from '@angular/router'; // 1. 引入 NavigationEnd, Event
import { AuthService } from '../../core/auth/auth.service';
import { MemberCardComponent } from './components/member-card/member-card.component';
import { MemberDto } from './Interfaces/member.dto';
import { MemberService } from './services/member.service';
import { Subscription, filter } from 'rxjs';
import { ToastService } from './services/toast.service';
import { CartService } from '../mall/services/cart.service';

@Component({
  selector: 'app-member',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterModule, MemberCardComponent],
  templateUrl: './member.component.html',
  styleUrls: ['./member.component.css']
})
export class MemberComponent implements OnInit, OnDestroy {
  isLoggedIn: boolean = false;
  currentMember: MemberDto | null = null;
  currentAvatarUrl: string = 'images/member/default-avatar.png';

  private refreshSubscription!: Subscription;
  private routerSubscription!: Subscription;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    public authService: AuthService,
    private memberService: MemberService,
    private toast: ToastService,
    private cartService: CartService
  ) { }

  ngOnInit(): void {
    this.updateUIStatus();

    this.routerSubscription = this.router.events.pipe(
      filter((event: Event): event is NavigationEnd => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.updateUIStatus();
    });

    this.handleDefaultRedirect();

    this.refreshSubscription = this.memberService.onRefreshProfile.subscribe(() => {
      if (this.isLoggedIn) {
        this.fetchMemberData();
      }
    });
  }

  ngOnDestroy(): void {
    if (this.refreshSubscription) this.refreshSubscription.unsubscribe();
    if (this.routerSubscription) this.routerSubscription.unsubscribe();
  }

  updateUIStatus() {
    if (this.router.url.includes('/member/login')) {
      this.isLoggedIn = false;
      this.currentMember = null;
    } else {
      const wasLoggedIn = this.isLoggedIn;
      this.isLoggedIn = this.authService.isLoggedIn();

      if (this.isLoggedIn && !this.currentMember) {
        this.fetchMemberData();
      }
    }
  }

  handleDefaultRedirect() {
    if (this.router.url === '/member' && this.authService.isLoggedIn()) {
      this.router.navigate(['/member/profile']);
    }
  }

  checkLoginStatus() {
    this.updateUIStatus();
  }

  fetchMemberData() {
    if (this.router.url.includes('/login')) return;

    this.memberService.getProfile().subscribe({
      next: (res) => {
        this.currentMember = res;
      },
      error: (err) => console.error('無法取得會員資料', err)
    });

    this.memberService.getAvatar().subscribe({
      next: (blob) => {
        if (this.currentAvatarUrl && this.currentAvatarUrl.startsWith('blob:')) {
          URL.revokeObjectURL(this.currentAvatarUrl);
        }
        this.currentAvatarUrl = URL.createObjectURL(blob);
      },
      error: () => {
        this.currentAvatarUrl = 'images/member/default-avatar.png';
      }
    });
  }

  logout() {
    this.authService.logout();
    this.isLoggedIn = false;
    this.currentMember = null;
    this.currentAvatarUrl = 'images/member/default-avatar.png';
    this.toast.success('您已安全登出');
    this.cartService.clearCartState();

    this.router.navigate(['/member/login']);
  }
}
