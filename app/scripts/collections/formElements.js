define(['underscore',
		'backbone',
		'models/formElement'
], function ( _, Backbone, FormElement ) {
	"use strict";

	var FormElementsCollection = Backbone.Collection.extend({

		model: FormElement,
		url: 'scripts/data/formElements.json' // load default form elements

	});

	return new FormElementsCollection();
});