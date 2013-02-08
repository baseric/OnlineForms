define(['underscore',
		'backbone'
], function ( _, Backbone ) {
	"use strict";

	var FormEditorView = Backbone.View.extend({

		initialize: function () {
			console.log('FormEditorView initialize');
			this.render();
		},

		render: function () {
			console.log('FormEditorView render');
		}

	});

	return FormEditorView;
});