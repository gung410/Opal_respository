import {
  animate,
  state,
  style,
  transition,
  trigger
} from '@angular/animations';
import { Component, OnInit } from '@angular/core';
import { Language } from 'app-models/language.model';
import { AppSettingService } from 'app-services/app-setting.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';

@Component({
  selector: 'language-selector',
  templateUrl: './language-selector.component.html',
  styleUrls: ['./language-selector.component.scss'],
  animations: [
    trigger('openSelector', [
      state(
        'inactive',
        style({
          height: 0,
          padding: 0,
          opacity: 0
        })
      ),
      state(
        'active',
        style({
          height: 'auto',
          padding: '10px',
          opacity: 1
        })
      ),
      transition('inactive => active', animate('0.3s ease-in')),
      transition('active => inactive', animate('0.3s ease-out'))
    ])
  ]
})
export class LanguageSelectorComponent implements OnInit {
  state: string = 'inactive';
  selectedLanguageCode: string;
  selectedLanguageName: string;
  languageList: Language[];

  constructor(
    private translate: TranslateAdapterService,
    private appSettingService: AppSettingService
  ) {}

  ngOnInit(): void {
    this.appSettingService.getLanguages().subscribe((result) => {
      this.languageList = result;
      this.selectedLanguageCode = this.translate.getCurrentLanguage();
      this.selectedLanguageName = this.translate.getCurrentLanguageName();
    });
  }

  openSelector(): void {
    this.state = this.state === 'inactive' ? 'active' : 'inactive';
  }

  changeLanguage(code: string, name: string): void {
    this.selectedLanguageCode = code;
    this.selectedLanguageName = name;
    this.translate.switchLanguage(code, name);
    localStorage.setItem('language-code', code);
    localStorage.setItem('language-name', name);
    this.openSelector();
  }
}
