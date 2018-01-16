import { Injectable } from '@angular/core';
import { HubConnection } from '@aspnet/signalr-client';
import { Subject } from 'rxjs/Rx';
import { ApiService } from '../providers/api.service';
import { CharacterInfo } from '../models/character-info';
import * as _ from 'lodash';

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
				console.warn(`characters: ${JSON.stringify(loggedInCharacters)}`);
				console.warn('1');
				const foundCharacters = this._characters.filter(
					(character: CharacterInfo) => {
						return this.currentCharacter && character.id === this.currentCharacter.id;
					});
				console.warn('2');

				if (foundCharacters.length === 0) {
					this.currentCharacter = this._characters.length > 0 ? this._characters[0] : undefined;
				}
				console.warn('3');

				this.loggedInCharacterListChanged.next(this._characters);
			});
	}

	private getLoggedInCharacters(): void {
		console.log('Loading logged in characters.');
		this.api.get('http://localhost:5000/api/characters/').subscribe(
			(characters: CharacterInfo[]) => {
				console.log(`Gotten logged in characters: ${JSON.stringify(characters)}`);

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
