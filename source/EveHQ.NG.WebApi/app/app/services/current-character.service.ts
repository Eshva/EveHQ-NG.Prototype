import { Injectable } from '@angular/core';
import { HubConnection } from '@aspnet/signalr-client';
import { Observable, Subject } from 'rxjs/Rx';
import { ApiService } from '../providers/api.service';
import { CharacterInfo } from '../models/character-info';
import { SkillQueueItem } from '../models/skill-queue-item';

@Injectable()
export class CurrentCharacterService {
	constructor(private readonly api: ApiService) {
		this.createNotificator();
		this.getLoggedInCharacters();
	}

	public get characters(): CharacterInfo[] {
		return this._characters;
	}

	public currentCharacter: CharacterInfo | undefined;

	public loggedInCharacterListChanged: Subject<CharacterInfo[]>;

	public getSkillQueue(id: number): Observable<SkillQueueItem[]> {
		return this.api.get(`http://localhost:5000/api/characters/${id}/skillqueue`).map(data => data as SkillQueueItem[]);
	}

	private createNotificator(): void {
		this.loggedInCharacterListChanged = new Subject();
		this.authenticationNotificationHub = new HubConnection('http://localhost:5000/authentication-notification');
		this.authenticationNotificationHub
			.start()
			.then(() => console.info('Connection to authentication-notification hub established.'))
			.catch(error => console.error(`Error while establishing connection to authentication-notification hub: ${error}`));
		this.authenticationNotificationHub.on(
			'LoggedInCharacterListChanged',
			(loggedInCharacters: CharacterInfo[]) => {
				this._characters = loggedInCharacters;
				const foundCharacters = this._characters.filter(
					(character: CharacterInfo) => {
						return this.currentCharacter && character.id === this.currentCharacter.id;
					});

				if (foundCharacters.length === 0) {
					this.currentCharacter = this._characters.length > 0 ? this._characters[0] : undefined;
				}

				this.loggedInCharacterListChanged.next(this._characters);
			});
	}

	private getLoggedInCharacters(): void {
		this.api.get('http://localhost:5000/api/characters/').subscribe(
			(characters: CharacterInfo[]) => {
				this._characters = characters;
				if (characters.length > 0) {
					this.currentCharacter = characters[0];
					this.loggedInCharacterListChanged.next(this._characters);
				}
			}
		);
	}

	private _characters: CharacterInfo[] = [];
	private authenticationNotificationHub: HubConnection;
}
