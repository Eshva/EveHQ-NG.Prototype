import { Injectable } from '@angular/core';
import { HubConnection } from '@aspnet/signalr-client';
import { BehaviorSubject } from 'rxjs/Rx';

@Injectable()
export class CurrentCharacterService {
	constructor() {
		this.loggedInCharacterIdChanged = new BehaviorSubject(0);
		this.authenticationNotificationHub = new HubConnection('http://localhost:5000/authentication-notification');
		this.authenticationNotificationHub
			.start()
			.then(() => console.info('Connection to authentication-notification hub established.'))
			.catch(error => console.error(`Error while establishing connection to authentication-notification hub: ${error}`));
		this.authenticationNotificationHub.on(
			'LoggedInCharacterIdChanged',
			(loggedInCharacterId: number) => {
				this._loggedInCharacterId = loggedInCharacterId;
				this.loggedInCharacterIdChanged.next(this._loggedInCharacterId);
			});
	}

	public loggedInCharacterIdChanged: BehaviorSubject<number>;

	public get loggedInCharacterId(): number {
		return this._loggedInCharacterId;
	}

	private _loggedInCharacterId: number = 0;
	private readonly authenticationNotificationHub: HubConnection;
}
