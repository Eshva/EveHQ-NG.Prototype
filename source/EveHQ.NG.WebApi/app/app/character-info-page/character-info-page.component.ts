import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs/Rx';
import { CurrentCharacterService } from '../services/current-character.service';
import { ApiService } from '../services/api.service';
import { CharacterInfo } from '../models/character-info';
import { SkillQueueItem } from '../models/skill-queue-item';

@Component({
	selector: 'app-character-info-page',
	templateUrl: './character-info-page.component.html',
	styleUrls: ['./character-info-page.component.scss']
})
export class CharacterInfoPageComponent implements OnDestroy {
	constructor(
		private readonly api: ApiService,
		private readonly currentCharacterService: CurrentCharacterService,
		private readonly router: Router) {
		this.setCurrentAndGoToLoginIfNotItNotPresent();

		this.loggedInCharacterListChangedSubscription =
			this.currentCharacterService.loggedInCharacterListChanged.subscribe(
				(characters: CharacterInfo[]) => {
					this.setCurrentAndGoToLoginIfNotItNotPresent();
				},
				error => console.error(`MMMM: ${error}`),
				() => console.info('MMMM Complited.'));
	}

	public ngOnDestroy(): void {
		this.loggedInCharacterListChangedSubscription.unsubscribe();
	}

	private skills: SkillQueueItem[] = [];

	private setCurrentAndGoToLoginIfNotItNotPresent(): void {
		this.currentCharacter = this.currentCharacterService.currentCharacter;
		if (!this.currentCharacter) {
			this.navigateToLoginPage();
			return;
		}

		const id = ((this.currentCharacter) as CharacterInfo).id;
		this.currentCharacterService.getSkillQueue(id).subscribe((skillQueueItems: SkillQueueItem[]) => {
			return this.skills = skillQueueItems;
		});
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
