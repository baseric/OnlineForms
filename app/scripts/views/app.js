define(['underscore',
		'backbone',
		'views/toolbar',
		'views/formeditor'
], function ( _, Backbone, ToolbarView, FormEditorView ) {
	"use strict";

	var AppView = Backbone.View.extend({

		initialize: function () {
			this.toolbarView = new ToolbarView({ el: '#toolbarView' });
			this.formEditorView = new FormEditorView({ el: '#formEditorView' });
		}

	});

	return AppView;
});