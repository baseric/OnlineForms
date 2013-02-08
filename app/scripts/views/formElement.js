define(['underscore',
		'backbone',
		'text!templates/formElement.html',
], function ( _, Backbone, formElementTemplate ) {
	"use strict";

	var FormElementView = Backbone.View.extend({

		tagName: 'li',

		template: _.template( formElementTemplate ),

		initialize: function () {
			this.model.on( 'change', this.render, this );
		},

		render: function () {
			this.$el.html( this.model.toJSON().content ).attr('data-cid', this.model.cid);

			return this;
		}

	});

	return FormElementView;
});