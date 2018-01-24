import { Component } from '@angular/core';
//import { ElectronService } from './providers/electron.service';

@Component({
	selector: 'app-root',
	templateUrl: './app.component.html'
})
export class AppComponent {
/*
	constructor(private readonly electronService: ElectronService) {

		if (electronService.isElectron()) {
			console.log('Mode electron');
			// Check if electron is correctly injected (see externals in webpack.config.js)
			console.log('ipcRenderer available', electronService.ipcRenderer);
			// Check if nodeJs childProcess is correctly injected (see externals in webpack.config.js)
			console.log('childProcess available', electronService.childProcess);
		}
		else {
			console.log('Mode web');
		}
	}
*/
}
