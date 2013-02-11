define(['underscore',
		'backbone',
		'collections/formTools',
		'views/formTool'
], function ( _, Backbone, FormToolsCollection, FormToolView ) {
	"use strict";

	var FormToolsView = Backbone.View.extend({

		initialize: function() {
			//console.log('FormToolsView: initialize()');
			
			// triggers the render function when a reset occurs
			FormToolsCollection.on( 'reset', this.render, this );
			// fetch the collection
			FormToolsCollection.fetch();
		},

		render: function() {
			// create a new form element view for each form element model in the collection
			_.each(FormToolsCollection.models, function (formTool) {
				// create a new form element view
				var formToolView = new FormToolView({model: formTool});
				// attach the form element view to it's parent view ($el)
				this.$el.append(formToolView.render().el);
			}, this);

			// attach the draggable on the parent element
			$('li', this.$el).draggable({
				connectToSortable: "#formEditor",
				helper: "clone",
				revert: "invalid"
			});

			return this;
		}

	});

	return FormToolsView;
});