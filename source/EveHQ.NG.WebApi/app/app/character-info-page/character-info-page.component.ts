import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CurrentCharacterService } from '../services/current-character.service';
import { ApiService } from '../providers/api.service';
import { CharacterInfo } from '../models/character-info';

@Component({
	selector: 'app-character-info-page',
	templateUrl: './character-info-page.component.html',
	styleUrls: ['./character-info-page.component.scss']
})
export class CharacterInfoPageComponent implements OnInit {
	constructor(
		private readonly api: ApiService,
		private readonly currentCharacterService: CurrentCharacterService,
		private readonly router: Router) {
	}

	public ngOnInit(): void {
		this.currentCharacterService.loggedInCharacterIdChanged.subscribe(
			(loggedInCharacterId: number) => {
				if (loggedInCharacterId === 0) {
					this.navigateToLoginPage();
				}
				else {
					this.loadCharacterData(loggedInCharacterId);
				}
			},
			error => console.error(`MMMM: ${error}`),
			() => console.info('MMMM Complited.'));
	}

	private loadCharacterData(characterId: number): void {
		console.log(`Loading data for character with ID ${characterId}.`);
		this.api.get(`http://localhost:5000/api/characters/${characterId}/info`).subscribe(
			(characterInfo: CharacterInfo) => {
				console.log(`Gotten character info: ${JSON.stringify(characterInfo)}`);
				this.characterInfo = characterInfo;
			}
		);

		this.api.get(`http://localhost:5000/api/characters/${characterId}/portrait`).subscribe(
			(portraitUri: string) => {
				console.log(`Gotten character portrait: ${portraitUri}`);
				this.portraitUri = portraitUri;
			}
		);

	}

	private logout(): void {
		this.api.post('http://localhost:5000/api/authentication/logout').subscribe(() => this.navigateToLoginPage());
	}

	private navigateToLoginPage(): void {
		this.router.navigate(['/login']);
	}

	private portraitUri: string;
	private characterInfo: CharacterInfo = new CharacterInfo();
}
