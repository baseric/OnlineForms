define(['underscore',
		'backbone',
		'text!templates/handle.html',
		'views/formElements'
], function ( _, Backbone, HandleTemplate, FormElementsView ) {
	"use strict";

	var ToolbarView = Backbone.View.extend({

		events: {
			"mouseenter": "showHandle",
			"mouseleave": "hideHandle"
		},

		tplHandle: _.template( HandleTemplate ),

		initialize: function() {
			// create the formElements view
			this.formElementsView = new FormElementsView({ el: '#formElements' });

			// append handle to toolbar
			this.$el.append(this.tplHandle());

			// makes the toolbar draggable
			this.$el.draggable({
				handle: '.handle'
			});
		},

		showHandle: function () {
			// only show the toolbar handle when it is being dragged
			if (!this.$el.hasClass('ui-draggable-dragging')) {
				// show the toolbar handle
				$('.handle', this.$el).show();
			}
		},

		hideHandle: function () {
			// only hide the toolbar handle when it is NOT being dragged
			if (!this.$el.hasClass('ui-draggable-dragging')) {
				// hide the toolbar handle
				$('.handle', this.$el).hide();
			}
		}

	});

	return ToolbarView;
});