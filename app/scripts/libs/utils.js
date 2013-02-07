define([ 'underscore' ], function( _ ) {
	"use strict";

	var utils = {
		// returns true if element has a class that matches the regex;
		// returns false otherwise
		hasClass: function (element, regex) {
			if (element.className) {
				return regex.test(element.className);
			} else {
				return false;
			}
		},

		isGridElement: function(element) {
			return this.hasClass(element, /span-/g);
		}
	};

	return utils;
});