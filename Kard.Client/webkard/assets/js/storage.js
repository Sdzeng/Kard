/**
 * 存贮(storage) 
 * 		local, cookie and session
 */
var storage={
	// 本地存贮
	 local:{
		setItem: function(name, value) {
			if (this.isSupport()) {
				window.localStorage.setItem(name, value);
			}
		},
		getItem: function(name) {
			if (this.isSupport()) {
				return window.localStorage.getItem(name);
			}
		},
		removeItem: function(name) {
			if (this.isSupport()) {
				return window.localStorage.removeItem(name);
			}
		},
		hasItem: function(name) {
			if (this.isSupport()) {
				return window.localStorage.getItem(name) !== null;
			}
		},
		isSupport: function() {
			return ('localStorage' in window) && window.localStorage !== null;
		}
	},
	// session
	 session: {
		setItem: function(name, value) {
			if (this.isSupport()) {
				window.sessionStorage.setItem(name, value);
			}
		},
		getItem: function(name) {
			if (this.isSupport()) {
				return window.sessionStorage.getItem(name);
			}
		},
		removeItem: function(name) {
			if (this.isSupport()) {
				return window.sessionStorage.removeItem(name);
			}
		},
		hasItem: function(name) {
			if (this.isSupport()) {
				return window.sessionStorage.getItem(name) !== null;
			}
		},
		isSupport: function() {
			return ('sessionStorage' in window) && window.sessionStorage !== null;
		}
	},

	// cookie
	 cookie: {
		setItem: function() {
			
		},
		getItem: function() {

		}
	}

 
};