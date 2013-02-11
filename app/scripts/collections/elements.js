define(['underscore',
		'backbone',
		'models/element'
], function ( _, Backbone, Element ) {
	"use strict";

	var ElementsCollection = Backbone.Collection.extend({

		model: Element,
		url: 'scripts/data/form.json' // load default elements

	});

	return new ElementsCollection();
});