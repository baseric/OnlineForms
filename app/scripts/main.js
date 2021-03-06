require.config({
	paths: {
		jquery: 'libs/jquery-1.9.1.min',
		jqueryui: 'libs/jquery-ui-1.10.0.min',
		underscore: 'libs/underscore-1.4.4.min',
		backbone: 'libs/backbone-0.9.10.min',
		text: 'libs/text',
		utils: 'libs/utils'
	},
	shim: {
		'underscore': {
			exports: '_'
		},
		'backbone': {
			deps: ['underscore'],
			exports: 'Backbone'
		}
	}
});

require(['jquery', 'jqueryui', 'views/app', 'backbone'],
	function( $, jqueryui, AppView, Backbone ) {

		// Initialize the application view
		new AppView({ el: '#app' });
	}
);