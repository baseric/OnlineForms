define(['underscore',
		'backbone',
		'models/element'
], function( _, Backbone, Element ) {
	"use strict";

	var ElementsCollection = Backbone.Collection.extend({

		model: Element,
		url: 'scripts/data/elements.json' // load default elements

	});

	return new ElementsCollection();
});