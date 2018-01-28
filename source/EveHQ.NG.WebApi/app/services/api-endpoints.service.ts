import { Injectable } from '@angular/core';

@Injectable()
export class ApiEndpointsService {

	public get authenticationNotification() {
		return `${this.baseUri}/authentication-notification`;
	}

	public get characters() {
		return `${this.apiBaseUri}/characters`;
	}

	public get logging() {
		return `${this.apiBaseUri}/clientlogging`;
	}

	private readonly baseUri = 'http://localhost:5000';
	private readonly apiBaseUri = `${this.baseUri}/api`;
}
