import { Injectable } from '@angular/core';
import { Http, Headers, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Rx';
import 'rxjs/Rx';

@Injectable()
export class ApiService {
	constructor(private readonly http: Http) { }

	public get(url: string, params: URLSearchParams = new URLSearchParams()): Observable<any> {
		return this.http.get(url, { headers: this.headers, search: params })
			.map((res: Response) => res.json());
	}

	public post(url: string, body: Object = {}): Observable<any> {
		return this.http.post(url, JSON.stringify(body), { headers: this.headers });
	}

	public put(url: string, body: Object = {}): Observable<any> {
		return this.http.put(url, JSON.stringify(body), { headers: this.headers });
	}

	public delete(url: string): Observable<any> {
		return this.http.delete(url, { headers: this.headers });
	}

	private readonly headers = new Headers({ 'Content-Type': 'application/json' });
}
