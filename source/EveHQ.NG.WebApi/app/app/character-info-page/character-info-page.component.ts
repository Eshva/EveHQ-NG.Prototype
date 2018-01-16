import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs/Rx';
import { CurrentCharacterService } from '../services/current-character.service';
import { ApiService } from '../providers/api.service';
import { CharacterInfo } from '../models/character-info';
import * as _ from 'lodash';

@Component({
	selector: 'app-character-info-page',
	templateUrl: './character-info-page.component.html',
	styleUrls: ['./character-info-page.component.scss']
})
export class CharacterInfoPageComponent implements OnInit, OnDestroy {
	constructor(
		private readonly api: ApiService,
		private readonly currentCharacterService: CurrentCharacterService,
		private readonly router: Router) {
		console.warn(`CharacterInfoPageComponent.ngOnInit Current: ${JSON.stringify(this.currentCharacter)}`);
		this.currentCharacter = this.currentCharacterService.currentCharacter;
		if (this.currentCharacter) {
			this.portraitUri = this.currentCharacter.portraitUris['Image512x512'];
		}
		else {
			this.navigateToLoginPage();
		}

		this.loggedInCharacterListChangedSubscription =
			this.currentCharacterService.loggedInCharacterListChanged.subscribe(
				(characters: CharacterInfo[]) => {
					console.warn(`Character list changed. ${characters.length}`);
					this.currentCharacter = this.currentCharacterService.currentCharacter;
					if (!this.currentCharacter) {
						this.navigateToLoginPage();
					}
					else {
						this.portraitUri = this.currentCharacterService.currentCharacter
											? this.currentCharacter.portraitUris['image512x512']
											: '';
					}
				},
				error => console.error(`MMMM: ${error}`),
				() => console.info('MMMM Complited.'));
	}

	public ngOnInit(): void {
	}

	public ngOnDestroy(): void {
		console.warn(`CharacterInfoPageComponent.ngOnDestroy Current: ${JSON.stringify(this.currentCharacter)}`);
		this.loggedInCharacterListChangedSubscription.unsubscribe();
	}


	private logout(): void {
		if (!this.currentCharacter) {
			return;
		}

		this.api.post(`http://localhost:5000/api/authentication/${this.currentCharacter.id}/logout/`)
			.subscribe(() => this.navigateToLoginPage());
	}

	private navigateToLoginPage(): void {
		this.router.navigate(['/login']);
	}

	private portraitUri: string;
	private currentCharacter: CharacterInfo | undefined;
	private readonly loggedInCharacterListChangedSubscription: Subscription;
}
