import { app, BrowserWindow, screen, dialog } from 'electron';
import * as path from 'path';


const os = require('os');
let webApiProcess: any;

const args = process.argv.slice(1);
let serve = args.some(val => val === '--serve');

if (serve) {
	require('electron-reload')(__dirname, {});
}

let win: Electron.BrowserWindow | null;
try {

	let isItSecondInstance = app.makeSingleInstance(
		(otherInstanceArguments: string[], workingDirectory: string) => {
			if (win) {
				if (win.isMinimized()) {
					win.restore();
				}

				win.focus();

				const message = `Arguments: ${otherInstanceArguments.join(', ')}`;
				dialog.showMessageBox(
					win,
					{
						type: 'info',
						message: message
					});
			}
		});

	if (isItSecondInstance) {
		app.quit();
	}

	// This method will be called when Electron has finished
	// initialization and is ready to create browser windows.
	// Some APIs can only be used after this event occurs.
	app.on('ready',
		() => {
			startApi();
		});

	// Quit when all windows are closed.
	app.on('window-all-closed',
		() => {
			// On OS X it is common for applications and their menu bar
			// to stay active until the user quits explicitly with Cmd + Q
			if (process.platform !== 'darwin') {
				app.quit();
			}

			console.log('exit...');
			webApiProcess.kill();
		});

	app.on('activate',
		() => {
			// On OS X it's common to re-create a window in the app when the
			// dock icon is clicked and there are no other windows open.
			if (win === null) {
				createWindow();
			}
		});

}
catch (e) {
	// TODO: Catch Error
	// throw e;
}
finally {
	win = null;
}

function startApi() {
	const childProcess = require('child_process').spawn;

	//  run server
	webApiProcess = childProcess(buildPathToWebApi());
	webApiProcess.stdout.on('data',
		(data: any) => {
			console.log(`stdout: ${data}`);

			if (win == null) {
				createWindow();
			}
		});
}

function buildPathToWebApi(): string {
	let pathToExecutable: string;
	switch (os.platform()) {
		case 'win32':
			pathToExecutable = 'publish//EveHQ.NG.WebApi.exe';
			break;
		case 'linux':
			pathToExecutable = 'publish//EveHQ.NG.WebApi';
			break;
		case 'darwin':
			pathToExecutable = 'publish//EveHQ.NG.WebApi';
			break;
		default:
			throw Error(`Unknown platform: ${os.platform()}`);
	}

	const appPath = app.getAppPath();
	const basePath = path.resolve(appPath, '..', '..', 'resources');
	const isAsar = !!appPath.match(/\.asar$/);
	const unpackedFolder = isAsar ? 'app.asar.unpacked' : 'app';

	return path.join(basePath, unpackedFolder, pathToExecutable);
}

function createWindow() {

	const electronScreen = screen;
	const size = electronScreen.getPrimaryDisplay().workAreaSize;

	// Create the browser window.
	win = new BrowserWindow({
		x: 0,
		y: 0,
		width: size.width,
		height: size.height
	});

	// and load the index.html of the app.
	win.loadURL(`file://${__dirname}/index.html`);

	// Open the DevTools.
	if (serve) {
		win.webContents.openDevTools();
	}

	// Emitted when the window is closed.
	win.on('closed',
		() => {
			// Dereference the window object, usually you would store window
			// in an array if your app supports multi windows, this is the time
			// when you should delete the corresponding element.
			win = null;
		});
}
