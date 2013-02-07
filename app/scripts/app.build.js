({
	appDir: "../",
	baseUrl: "scripts",
	dir: "../../app-build",
	useStrict: true,
	optimizeCss: "standard",
	paths: {
		jquery: 'libs/jquery-1.9.1.min',
		underscore: 'libs/underscore-1.4.4.min',
		backbone: 'libs/backbone-0.9.10.min',
		text: 'libs/text',
		utils: 'libs/utils'
	},
	modules: [
		{
			name: 'main'
		}
	]
})