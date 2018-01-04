import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../providers/api.service';
import { CurrentCharacterService } from '../services/current-character.service';
import 'rxjs/Rx';

declare var electron: any;

@Component({
	selector: 'app-login-page',
	templateUrl: './login-page.component.html',
	styleUrls: ['./login-page.component.scss']
})
export class LoginPageComponent implements OnInit {

	constructor(
		private readonly api: ApiService,
		private readonly currentCharacterService: CurrentCharacterService,
		private readonly router: Router) {
	}

	public ngOnInit(): void {
		this.currentCharacterService.loggedInCharacterIdChanged.subscribe(
			(loggedInCharacterId: number) => {
				if (loggedInCharacterId !== 0) {
					this.router.navigate(['/character-info']);
				}
			},
			error => console.error(`LLLL: ${error}`),
			() => console.info('LLLL Complited.'));
	}

	private login(): void {
		this.api.get('http://localhost:5000/api/authentication/getAuthenticationUri')
			.subscribe(authenticationUri => {
				electron.shell.openExternal(authenticationUri);
			});
	}
}
