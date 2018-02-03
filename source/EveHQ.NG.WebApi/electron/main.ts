import { app, BrowserWindow, screen, net } from 'electron';
import * as path from 'path';

const os = require('os');
let webApiProcess: any;

const args = process.argv.slice(1);
let serve = args.some(val => val === '--serve');
const serviceBaseUrl = 'http://localhost:5000/api';

require('electron-unhandled')({ logger: logExceptionToApi, showDialog: false });

if (serve) {
	require('electron-reload')(__dirname);
}

let mainWindow: Electron.BrowserWindow | null;
try {
	let isItSecondInstance = app.makeSingleInstance(
		(otherInstanceArguments: string[], workingDirectory: string) => {
			if (mainWindow) {
				if (mainWindow.isMinimized()) {
					mainWindow.restore();
				}

				mainWindow.focus();
				processArguments(otherInstanceArguments);
			}
		});

	if (isItSecondInstance) {
		app.exit();
	}

	// This method will be called when Electron has finished
	// initialization and is ready to create browser windows.
	// Some APIs can only be used after this event occurs.
	app.on(
		'ready',
		() => {
			startApi();

			if (mainWindow == null) {
				createMainWindow();
				mainWindow.webContents.openDevTools();
			}

			setServiceDefaults();
		});

	// Quit when all windows are closed.
	app.on(
		'window-all-closed',
		() => {
			// On OS X it is common for applications and their menu bar
			// to stay active until the user quits explicitly with Cmd + Q
			if (process.platform !== 'darwin') {
				app.quit();
			}

			console.log('exit...');
			stopApi();
		});

	app.on(
		'activate',
		() => {
			// On OS X it's common to re-create a window in the app when the
			// dock icon is clicked and there are no other windows open.
			if (mainWindow === null) {
				createMainWindow();
			}
		});

	app.on(
		'open-url',
		(event: Event, url: string) => {
			logInformation(`open-url event with url: ${url}`)
			processArguments(['todo', url]);
		}
	)
}
finally {
	mainWindow = null;
}

function startApi() {
	if (serve) {
		return;
	}

	try {
		const childProcess = require('child_process').spawn;

		//  run server
		webApiProcess = childProcess(buildPathToWebApi());
	}
	catch (error) {
		console.error(`An error occured: ${error}`);
	}
}

function stopApi() {
	if (serve) {
		return;
	}

	webApiProcess.kill();
}

function buildPathToWebApi(): string {
	let pathToExecutable: string;
	switch (os.platform()) {
		case 'win32':
			pathToExecutable = 'publish//EveHQ.NG.WebApi.exe';
			break;
		case 'linux':
		case 'darwin':
			pathToExecutable = 'publish//EveHQ.NG.WebApi';
			break;
		default:
			throw Error(`Unknown platform: ${os.platform()}`);
	}

	const appPath = app.getAppPath();

	if (serve) {
		return path.join(appPath, pathToExecutable);
	}

	const basePath = path.resolve(appPath, '..', '..', 'resources');
	const isAsar = !!appPath.match(/\.asar$/);
	const unpackedFolder = isAsar ? 'app.asar.unpacked' : 'app';

	return path.join(basePath, unpackedFolder, pathToExecutable);
}

function createMainWindow() {

	const electronScreen = screen;
	const size = electronScreen.getPrimaryDisplay().workAreaSize;

	// Create the browser window.
	mainWindow = new BrowserWindow({
		x: 0,
		y: 0,
		width: size.width,
		height: size.height
	});

	// and load the index.html of the app.
	mainWindow.loadURL(`file://${__dirname}/index.html`);

	// Open the DevTools.
	if (serve) {
		mainWindow.webContents.openDevTools();
	}

	// Emitted when the window is closed.
	mainWindow.on('closed',
		() => {
			// Dereference the window object, usually you would store window
			// in an array if your app supports multi windows, this is the time
			// when you should delete the corresponding element.
			mainWindow = null;
		});

	mainWindow.on('unresponsive', () => logExceptionToApi(new Error('Main window is unresponsive.')));
	mainWindow.webContents.on('crashed', (event: any, killed: boolean) => logExceptionToApi(new Error(`Renderer process crashed. Killed: ${killed}.`)));
}

function processArguments(otherInstanceArguments: string[]) {
	logInformation(`processArguments: ${otherInstanceArguments.join(', ')}`);
	if (otherInstanceArguments.length !== 2) {
		throw new Error('Number of arguments is invalid.');
	}

	const payload = otherInstanceArguments[1];
	const match = /^eveauth-evehq-ng:\/\/sso-auth\/\?code=(.+?)&state=(.+?)$/.exec(payload);

	if (match != null) {
		const [code, state] = match;
		const localAuthenticationServiceUrl =
			`${serviceBaseUrl}/authentication/authenticatioWithCode?codeUri=${code}&state=${state}`;
		const authenticationRequest = net.request({
			url: localAuthenticationServiceUrl,
			method: 'POST'
		});
		authenticationRequest.on(
			'response',
			response => {
				console.warn(`Status of authentication call: ${response.statusCode}`);
			});
		authenticationRequest.end();
	}
	else {
		throw new Error('Bad format of authentication code replay.');
	}
}

function setServiceDefaults() {
	const settings = JSON.stringify({
		applicationFolder: app.getAppPath(),
		applicationDataFolder: app.getPath('appData'),
		temporaryDataFolder: app.getPath('temp')
	});

	const setServiceDefaultsRequest = net.request({
		url: `${serviceBaseUrl}/settings/setDefaults`,
		method: 'POST'
	});

	setServiceDefaultsRequest.setHeader('Content-Type', 'application/json');
	setServiceDefaultsRequest.write(settings);
	setServiceDefaultsRequest.on(
		'response',
		response => {
			console.warn(`Status of set defaults call: ${response.statusCode}`);
		});
	setServiceDefaultsRequest.end();

}

function logInformation(message: string) {
	log(message, 'information');
}

function logExceptionToApi(error: Error) {
	const message = `${JSON.stringify(`Uncaught exception in the ${process.type} Electron process:\n${error.stack}`)}`;
	log(message, 'error');
}

function log(message: string, messageKind: string) {
	const clientLoggingUrl = `${serviceBaseUrl}/clientlogging/${messageKind}`;
	const logRequest = net.request({
		url: clientLoggingUrl,
		method: 'POST'
	});

	logRequest.setHeader('Content-Type', 'application/json');
	logRequest.write(message);
	logRequest.on(
		'response',
		response => {
			console.warn(`Status of log error call: ${response.statusCode}`);
		});
	logRequest.on(
		'error',
		error1 => {
			console.warn(`Error on log error call: ${error1.message}`);
		});
	logRequest.end();
}