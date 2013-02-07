define(['underscore',
		'backbone',
		'views/formeditor'
], function( _, Backbone, FormEditorView ) {
	"use strict";

	var AppView = Backbone.View.extend({

		initialize: function() {
			this.formEditorView = new FormEditorView({ el: '#formEditorView' });
		}

	});

	return AppView;
});