define(['underscore',
		'backbone'
], function ( _, Backbone ) {
	"use strict";

	var FormModel = Backbone.Model.extend({

		defaults: {
			content: 'P'
		}

	});

	return FormModel;
});