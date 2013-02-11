define(['underscore',
		'backbone',
		'collections/formTools',
		'collections/elements',
		'views/element'
], function ( _, Backbone, FormToolsCollection, ElementsCollection, ElementView ) {
	"use strict";

	var FormEditorView = Backbone.View.extend({

		initialize: function () {

			this.elementView = new ElementView({ 'label': 'test' });

		},

		render: function () {

			return this;
		}

	});

	return FormEditorView;
});