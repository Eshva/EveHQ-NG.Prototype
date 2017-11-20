import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Rx';
import { ApiService } from '../../providers/api.service';
import { Contact } from '../../contact';
import 'rxjs/Rx';

@Component({
	selector: 'app-home',
	templateUrl: './home.component.html',
	styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
	title = `App works !`;

	constructor(private readonly api: ApiService) {
	}

	ngOnInit() {
		this.getTasks();
	}

	private getTasks(): void {
		this.api.get('http://localhost:5000/api/contacts').subscribe(data => {
			console.warn(data);
			this.contacts = data
			console.warn(this.contacts);
		});
	}

	private contacts: Contact[];
}
