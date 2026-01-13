// 用於 "註冊" 時傳給後端的資料格式
export interface RegisterDto {
  name: string;
  email: string;
  password: string;
  // 這個欄位後端不需要，但前端表單驗證需要用到，所以設為可選(?)或是包含在內
  confirmPassword?: string;
}

// 用於 "登入" 時傳給後端的資料格式
export interface LoginDto {
  email: string;
  password: string;
}

// 用於 "接收" 後端回傳的會員資料 (不包含密碼)
export interface MemberDto {
  memberId: number;
  name: string;
  email: string;
  phone?: string;
  token?: string; // 預留給未來 JWT 驗證用的欄位
  role: number;
  registrationDate: string;
  lastLoginDate?: string;
  isActive: boolean;
}

export interface ChangePasswordDto {
  oldPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}


