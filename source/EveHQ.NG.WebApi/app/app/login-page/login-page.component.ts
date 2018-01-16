import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs/Rx';
import { ApiService } from '../providers/api.service';
import { CharacterInfo } from '../models/character-info';
import { CurrentCharacterService } from '../services/current-character.service';

declare var electron: any;

@Component({
	selector: 'app-login-page',
	templateUrl: './login-page.component.html',
	styleUrls: ['./login-page.component.scss']
})
export class LoginPageComponent implements OnInit, OnDestroy {

	constructor(
		private readonly api: ApiService,
		private readonly currentCharacterService: CurrentCharacterService,
		private readonly router: Router) {
		console.warn('LoginPageComponent.ngOnInit');
		this.loggedInCharacterListChangedSubscription =
			this.currentCharacterService.loggedInCharacterListChanged.subscribe(
			(characters: CharacterInfo[]) => {
				if (characters.length > 0) {
					console.warn(`Navigate to character page. ${JSON.stringify(characters)}`);

					this.router.navigate(['/character-info']);
				}
			},
			error => console.error(`LLLL: ${error}`),
			() => console.info('LLLL Complited.'));
	}

	public ngOnInit(): void {
	}

	public ngOnDestroy(): void {
		console.warn(`LoginPageComponent.ngOnDestroy`);
		this.loggedInCharacterListChangedSubscription.unsubscribe();
	}

	private login(): void {
		this.api.get('http://localhost:5000/api/authentication/getAuthenticationUri')
			.subscribe(authenticationUri => {
				electron.shell.openExternal(authenticationUri);
			});
	}

	private readonly loggedInCharacterListChangedSubscription: Subscription;
}
