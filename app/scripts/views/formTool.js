define(['underscore',
		'backbone',
		'text!templates/formTool.html'
], function ( _, Backbone, formToolTemplate ) {
	"use strict";

	var FormToolView = Backbone.View.extend({

		tagName: 'li',

		template: _.template( formToolTemplate ),

		initialize: function () {
			this.model.on( 'change', this.render, this );
		},

		render: function () {
			this.$el.html( this.model.toJSON().content ).attr('data-cid', this.model.cid);

			return this;
		}

	});

	return FormToolView;
});