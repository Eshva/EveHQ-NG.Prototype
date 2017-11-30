/* SystemJS module definition */
declare var nodeModule: NodeModule;

interface NodeModule {
	id: string;
}

declare var window: Window;

interface Window {
	process: any;
	require: any;
	fs: any;
	os: any;
}
