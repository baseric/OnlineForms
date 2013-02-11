define(['underscore',
		'backbone',
		'models/formTool'
], function ( _, Backbone, FormTool ) {
	"use strict";

	var FormToolsCollection = Backbone.Collection.extend({

		model: FormTool,
		url: 'scripts/data/formTools.json' // load form tools

	});

	return new FormToolsCollection();
});