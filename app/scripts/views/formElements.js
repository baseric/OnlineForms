define(['underscore',
		'backbone',
		'collections/formElements',
		'views/formElement'
], function ( _, Backbone, FormElementsCollection, FormElementView ) {
	"use strict";

	var FormElementsView = Backbone.View.extend({

		initialize: function() {
			// triggers the render function when a reset occurs
			FormElementsCollection.on( 'reset', this.render, this );
			// fetch the collection
			FormElementsCollection.fetch();
		},

		render: function() {
			// create a new form element view for each form element model in the collection
			_.each(FormElementsCollection.models, function (formElement) {
				// create a new form element view
				var formElementView = new FormElementView({model: formElement});
				// attach the form element view to it's parent view ($el)
				this.$el.append(formElementView.render().el);
			}, this);

			// attach the draggable on the parent element
			$('li', this.$el).draggable({
				connectToSortable: "#formEditorView",
				helper: "clone",
				revert: "invalid"
			});

			return this;
		}

	});

	return FormElementsView;
});