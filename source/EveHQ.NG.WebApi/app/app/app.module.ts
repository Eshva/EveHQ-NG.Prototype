import 'zone.js/dist/zone-mix';
import 'reflect-metadata';
import 'polyfills';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { AppRoutingModule } from './app-routing.module';
import { ElectronService } from './providers/electron.service';
import { ApiService } from './providers/api.service';
import { LoginPageComponent } from './login-page/login-page.component';
import { CurrentCharacterService } from './services/current-character.service';
import { CharacterInfoPageComponent } from './character-info-page/character-info-page.component';

@NgModule({
	declarations: [
		AppComponent,
		HomeComponent,
		LoginPageComponent,
		CharacterInfoPageComponent
	],
	imports: [
		BrowserModule,
		FormsModule,
		HttpModule,
		AppRoutingModule
	],
	providers: [
		ElectronService,
		ApiService,
		CurrentCharacterService
	],
	bootstrap: [AppComponent]
})
export class AppModule {
}
