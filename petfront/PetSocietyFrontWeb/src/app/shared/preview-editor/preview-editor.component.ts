import { Component, Input, SimpleChanges, ViewEncapsulation } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
  selector: 'app-preview-editor',
  encapsulation: ViewEncapsulation.ShadowDom,
  imports: [],
  templateUrl: './preview-editor.component.html',
  styleUrl: './preview-editor.component.css'
})
export class PreviewEditorComponent {
  @Input() innerHTML: string = '';
  useRawCss: boolean = true;
  constructor(private sanitizer: DomSanitizer) { }

  // 1. 預設的 HTML 內容 (你可以視需求改成輸入框讓使用者改)


  // 2. 預設的 CSS 內容
  rawCss2: string = `#cms-content {
  --primary-color: #007bff;
  --accent-color: #0f0;    /* 螢光綠 */
  --highlight-color: #f0f; /* 螢光粉 */
  --dark-bg: #000;         /* 純黑 */
  --text-color: #fff;      /* 純白 */

  font-family: 'Segoe UI', sans-serif;
  color: #333; /* 預設文字色 */
  line-height: 1.6;
  display: block;
}

/* H3：標題 */
#cms-content h3 {
  position: relative;
  font-size: 1.8rem;
  font-weight: 900;
  text-transform: uppercase;
  letter-spacing: 2px;

  /* 工業風配色 */
  color: var(--accent-color);
  background-color: var(--dark-bg);

  margin-top: 2rem;
  margin-bottom: 1rem;
  padding: 1rem;

  border: 4px solid var(--accent-color);
  box-shadow: 5px 5px 0px var(--highlight-color); /* 硬陰影 */
}

/* UL：清單容器 */
#cms-content ul {
  padding: 15px;
  list-style: none;
  background-color: #222;
  border: 2px solid #555;
  margin-bottom: 2rem;
}

/* LI：清單項目 */
#cms-content ul li {
  position: relative;
  background: #333;
  color: #fff;
  padding: 12px 15px;
  margin-bottom: 8px;
  border-bottom: 1px dashed #555;
  font-family: monospace;

  /* Flex 佈局讓 Icon 對齊 */
  display: flex;
  align-items: center;
  transition: all 0.2s;
}

/* LI Hover */
#cms-content ul li:hover {
  background-color: var(--highlight-color);
  color: var(--dark-bg);
  font-weight: bold;
}

/* Icon (原本的 ::before) */
#cms-content ul li::before {
  content: '→';
  color: var(--accent-color);
  font-size: 1.5em;
  font-weight: bold;
  margin-right: 15px;
  line-height: 1;
}

/* Hover 時 Icon 變色 */
#cms-content ul li:hover::before {
  color: var(--dark-bg);
}

/* 其他自訂 Class (如果有用到) */
#cms-content .title-primary {
  background-color: var(--primary-color);
  color: white;
  padding: 10px;
  margin-bottom: 20px;
}`;

  rawCss: string = `

:host {
  --primary-color: #007bff;
  --secondary-color: #00a0e9;
  --accent-color: #d4a373; /* 奶茶色 */
  --dark-color: #4a4a4a;   /* 深灰色 */
  --light-dark-color: #666;
  --shadow-sm: 0 2px 4px rgba(0,0,0,0.1);
  --shadow-hover: 0 4px 8px rgba(0,0,0,0.15);
  --transition-speed: 0.3s;

  /* 確保基本字體設定 */
  display: block;
  font-family: 'Segoe UI', sans-serif;
  color: var(--dark-color);
  background: #fdfdfd; /* 讓背景稍微有點顏色 */
}

/* 2. 直接寫 class，不用加 :host */
.title-primary {
  background-color: var(--primary-color);
  color: white;
  padding: 10px 15px;
  margin-bottom: 20px;
  border-radius: 4px;
}

.title-secondary {
  background-color: var(--secondary-color);
  color: white;
  padding: 8px 15px;
  margin-top: 30px;
  margin-bottom: 15px;
  border-left: 5px solid #ffcc00;
}

.content-list-item {
  list-style: none;
  margin-bottom: 10px;
}

/* 3. 移除 ::ng-deep，直接選取標籤 */
/* 因為在 Shadow DOM 裡，這裡的 h3 不會影響到外面的 h3 */
h3 {
  position: relative;
  font-size: 1.6rem;
  font-weight: 800;
  color: var(--dark-color);
  margin-top: 2.5rem;
  margin-bottom: 1.5rem;
  padding-left: 1rem;
  z-index: 1;
}

/* H3 背景裝飾 */
h3::after {
  content: '';
  position: absolute;
  bottom: 2px;
  left: 0;
  width: 120px;
  height: 8px;
  background: linear-gradient(90deg, var(--accent-color), rgba(255, 255, 255, 0));
  z-index: -1;
  border-radius: 4px;
}

/* H3 左側立體標記 */
h3::before {
  content: '';
  position: absolute;
  left: 0;
  top: 50%;
  transform: translateY(-50%);
  width: 6px;
  height: 24px;
  background: var(--dark-color);
  border-radius: 3px;
  box-shadow: 2px 2px 0px var(--accent-color);
}

/* UL 設定 */
ul {
  padding-left: 0;
  list-style: none;
  margin-bottom: 2rem;
}

/* LI 卡片樣式 */
ul li {
  position: relative;
  background: #fff;
  padding: 12px 16px 12px 42px; /* 左邊留給 icon */
  margin-bottom: 10px;
  border-radius: 8px;
  border: 1px solid rgba(0, 0, 0, 0.05);
  box-shadow: var(--shadow-sm);
  color: var(--light-dark-color);
  font-weight: 500;
  cursor: default;
  transition: all var(--transition-speed);
}

/* LI Hover 效果 */
ul li:hover {
  transform: translateX(8px) translateY(-2px);
  box-shadow: var(--shadow-hover);
  border-left: 4px solid var(--accent-color);
  color: var(--dark-color);
}

/* LI Icon */
ul li::before {
  content: '✦';
  position: absolute;
  left: 16px;
  top: 12px;
  color: var(--accent-color);
  font-size: 1.1em;
  font-weight: bold;
}
  `.trim();


  // 3. 取得「信任後的 HTML」
  get trustedHtml(): SafeHtml {
    // 為了配合 CSS 中的 #cms-content 選擇器，這裡手動包覆一層 ID
    return this.sanitizer.bypassSecurityTrustHtml(`<div id="cms-content">${this.innerHTML}</div>`);
  }

  setStyle(useRaw: boolean) {
    this.useRawCss = useRaw;
  }
  // 4. 取得「信任後的 CSS」 (將 rawCss 包裝成 style 標籤)
  get trustedCss(): SafeHtml {
    const cssContent = this.useRawCss ? this.rawCss : this.rawCss2;
    // 這裡我們把使用者輸入的 CSS 包在 <style> 標籤裡
    // 因為整個元件都在 Shadow DOM 裡，這個 style 標籤只會影響這個元件內部
    return this.sanitizer.bypassSecurityTrustHtml(`<style>${cssContent}</style>`);
  }
}
