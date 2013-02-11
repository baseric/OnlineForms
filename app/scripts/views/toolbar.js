define(['underscore',
		'backbone',
		'text!templates/handle.html',
		'views/formTools'
], function ( _, Backbone, HandleTemplate, FormToolsView ) {
	"use strict";

	var ToolbarView = Backbone.View.extend({

		events: {
			"mouseenter": "showHandle",
			"mouseleave": "hideHandle"
		},

		tplHandle: _.template( HandleTemplate ),

		initialize: function() {
			//console.log('ToolbarView: initialize()');

			// create the formToolsView view
			this.formToolsView = new FormToolsView({ el: '#formTools' });

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