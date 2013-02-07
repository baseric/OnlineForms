define(['underscore',
		'backbone'
], function ( _, Backbone ) {
	"use strict";

	var FieldsetModel = Backbone.Model.extend({

		defaults: {
			content: 'P'
		}

	});

	return FieldsetModel;
});