define(['underscore',
		'backbone'
], function ( _, Backbone ) {
	"use strict";

	var ElementModel = Backbone.Model.extend({

		defaults: {
			"label": "Text",
			"type": "text",
			"name": "nametext"
		},

		//defaults: function () { 
		//	return { "label": "Text", "type": "text", "name": "nametext" };
		//},

		initialize: function () {
			console.log('ElementModel: initialize()');

			this.options = _.extend(this.defaults, this.options);
		}

	});

	return ElementModel;
});