import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { PreviewEditorComponent } from "../preview-editor/preview-editor.component";

@Component({
  selector: 'app-editor',
  imports: [CommonModule, FormsModule, AngularEditorModule, PreviewEditorComponent],
  templateUrl: './editor.component.html',
  styleUrl: './editor.component.css'
})
export class EditorComponent {
  @Input() htmlContent: string = '請輸入簡章內容...';

  @Input() title: string = '編輯內容';


  @Input() config: AngularEditorConfig = {
    editable: true,
    spellcheck: true,
    height: '10rem',
    minHeight: '5rem',
    placeholder: 'Enter text in this rich text editor....',
    defaultParagraphSeparator: 'p',
    defaultFontName: 'Arial',
    toolbarHiddenButtons: [
      ['subscript', 'superscript']
    ],

  };

  @Output() onContentChange = new EventEmitter<string>();

  onEditorChange() {
    this.onContentChange.emit(this.htmlContent);
    console.log(this.htmlContent);
  }

}
