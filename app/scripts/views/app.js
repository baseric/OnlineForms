define(['underscore',
		'backbone',
		'views/toolbar',
		'views/formEditor'
], function ( _, Backbone, ToolbarView, FormEditorView ) {
	"use strict";

	var AppView = Backbone.View.extend({

		initialize: function () {
			this.toolbarView = new ToolbarView({ el: '#toolbar' });
			this.formEditorView = new FormEditorView({ el: '#formEditor' });
		}

	});

	return AppView;
});