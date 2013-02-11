define(['underscore',
		'backbone',
		'text!templates/textElement.html'
], function ( _, Backbone, textElementTemplate ) {
	"use strict";

	var ElementView = Backbone.View.extend({

		el: '#formEditor',

		template: _.template( textElementTemplate ),

		initialize: function () {
			console.log('ElementView: initialize()');

			this.render();
		},

		render: function () {
			console.log('ElementView: render()');

			//this.$el.html(this.template());

			this.$el.html('<p>TEST</p>');

			console.log(this.template());

			return this;
		}

	});

	return ElementView;
});