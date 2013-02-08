define(['underscore',
		'backbone'
], function ( _, Backbone ) {
	"use strict";

	var FormElementModel = Backbone.Model.extend({

		initialize: function() {
			console.log('new form element model created');
		}

	});

	return FormElementModel;
});