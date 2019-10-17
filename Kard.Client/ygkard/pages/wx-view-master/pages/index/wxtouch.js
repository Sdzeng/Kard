
(function() {
  var abs = Math.abs;

  var DIRECTION_NONE = 0;
  var DIRECTION_UP = 2;
  var DIRECTION_DOWN = 4;
  var DIRECTION_LEFT = 8;
  var DIRECTION_RIGHT = 16;
  var DIRECTION_VERTICAL = DIRECTION_DOWN | DIRECTION_UP;
  var DIRECTION_HORIZONTAL = DIRECTION_LEFT | DIRECTION_RIGHT;
  var DIRECTION_ALL = DIRECTION_VERTICAL | DIRECTION_HORIZONTAL;

  function each(obj, iterator, context) {
    if (!obj) {
      return;
    }
    if (obj.forEach) {
      obj.forEach(iterator, context);
    }
    else if (obj.length !== undefined) {
      var i = 0;
      while (i < obj.length) {
        iterator.call(context, obj[i], i, obj);
        i++;
      }
    }
    else {
      var i = 0;
      for (i in obj) {
        obj.hasOwnProperty(i) && iterator.call(context, obj[i], i, obj);
      }
    }
  }

  function buildTouchInput(e) {
    var input = {};
    input.type = e.type;
    input.timeStamp = e.timeStamp;
    input.pointers = [];
    input.changedPointers = [];
    for (var i = 0; i < e.touches.length; i++)
    {
      var pointer = {};
      pointer.id = e.touches[i].identifier;
      pointer.x = e.touches[i].clientX || e.touches[i].x;
      pointer.y = e.touches[i].clientY || e.touches[i].y;
      input.pointers.push(pointer);

      var changePointer = {};
      changePointer.id = e.changedTouches[i].identifier;
      changePointer.x = e.changedTouches[i].clientX || e.changedTouches[i].x;
      changePointer.y = e.changedTouches[i].clientY || e.changedTouches[i].y;
      input.changedPointers.push(changePointer);
    }
    input.center = getCenter(input.pointers);
    return input;
  }

  function setTimeoutContext(fn, timeout, context) {
    return setTimeOut(bindFn(fn, context), timeout);
  }

  function bindFn(fn, context) {
    return function () {
      return fn.apply(context, arguments);
    }
  }

  function splitStrWithWhiteSpace(str) {
    return str.trim().split(/\s+/g);
  }

  function inArray(array, find) {
    if (array.indexOf) {
      return array.indexOf(find);
    }
    return -1;
  }

  function toArray(obj) {
    return Array.prototype.slice.call(obj, 0);
  }

  function getCenter(pointers) {
    var len = pointers.length;

    var center = {};
    if (len === 1) {
      center.x = pointers[0].x;
      center.y = pointers[0].y;
    }

    var totalX = 0;
    var totalY = 0;
    for (var i = 0; i < len; i++) {
      totalX += pointers[i].x;
      totalY += pointers[i].y;
    }

    center.x = Math.round(totalX / len);
    center.y = Math.round(totalY / len);

    return center;
  }

  function getDirection(x, y) {
    if (x === y) {
      return DIRECTION_NONE;
    }
    if (abs(x) >= abs(y)) {
      return x < 0 ? DIRECTION_LEFT : DIRECTION_RIGHT;
    }
    return y < 0 ? DIRECTION_UP : DIRECTION_DOWN;
  }

  function getDis(p1, p2) {
    var x = p1.x - p2.x;
    var y = p1.y - p2.y;
    return Math.sqrt((x * x) + (y * y));
  }

  function getScale(start, end) {
    return getDis(end[0], end[1]) / getDis(start[0], start[1]);
  }

  function getAngle(p1, p2) {
    var x = p2.x - p1.x;
    var y = p2.y - p1.y;

    return Math.atan2(y, x) * 180 / Math.PI;
  }

  function getRotation(start, end) {
    return getAngle(end[1], end[0]) + getAngle(start[1], start[0]);
  }

  var WXTouch = {
    ontouch: function(e) {
      if (!e) {
        return;
      }
      var id = e.currentTarget.id;
      var handler = this.handlers[id];
      if (handler) {
        handler.onTouch(e);
      }
    },
    init: function(page, opt) {
      var self = this;
      page.ontouch = function(e) {
        self.ontouch(e);
      };
      this.opt = opt || {};
      this.handlers = {};
    },
    createHandler: function(elem, gesture, cb) {
      var handler = this.handlers[elem];
      if (handler) {
        handler.on(gesture, cb);
      }
      else {
        handler = Object.create(TouchHandler);
        handler.init(elem, gesture, cb, this);
        this.handlers[elem] = handler;
      }
      return handler;
    },
    istouchstart: function(input) {
      return input.event === 'touchstart';
    },
    istouchmove: function(input) {
      return input.event === 'touchmove';
    },
    istouchend: function(input) {
      return input.event === 'touchend';
    },
    istouchcancel: function(input) {
      return input.event === 'touchcancel';
    },
    ismultitouchstart: function(input) {
      return input.event === 'multitouchstart';
    },
    ismultitouchend: function(input) {
      return input.event === 'multitouchend';
    },
    istap: function(input) {
      return input.event === 'tap';
    },
    islongtap: function(input) {
      return input.event === 'longtap';
    },
    isdoubletap: function(input) {
      return input.event === 'doubletap';
    }
  };

  function calTouchmoveData(session, e) {
    var input = buildTouchInput(e);
    var firstInput = session.firstInput;
    var firstMultiple = session.firstMultiple;
    var firstCenter = firstMultiple ? firstMultiple.center : firstInput.center;
    var prevInput = session.prevInput;
    var prevCenter = prevInput.center;
    var prevTotalDeltaX = prevInput.totalDeltaX || 0;
    var prevTotalDeltaY = prevInput.totalDeltaY || 0;
    var prevTimeStamp = prevInput.timeStamp;

    var center = input.center;
    input.totalDeltaTime = input.timeStamp - firstInput.timeStamp;
    input.deltaTime = input.timeStamp - prevTimeStamp;
    input.deltaX = center.x - prevCenter.x;
    input.deltaY = center.y - prevCenter.y;
    input.totalDeltaX = input.deltaX + prevTotalDeltaX;
    input.totalDeltaY = input.deltaY + prevTotalDeltaY;
    input.totalDirection = getDirection(input.totalDeltaX, input.totalDeltaY);
    input.direction = getDirection(input.deltaX, input.deltaY);

    input.scale = firstMultiple ? getScale(firstMultiple.pointers, pointers) : 1;
    input.rotation = firstMultiple ? getRotation(firstMultiple.pointers, pointers) : 0;

    return input;
  }

  function calTouchendData(session, e) {
    var input = {};
    input.timeStamp = e.timeStamp;
    var firstInput = session.firstInput;
    var prevInput = session.prevInput;

    var center = input.center = prevInput.center;
    input.totalDeltaTime = input.timeStamp - firstInput.timeStamp;
    input.totalDeltaX = prevInput.totalDeltaX || 0;
    input.totalDeltaY = prevInput.totalDeltaY || 0;
    input.totalDirection = prevInput.totalDirection || DIRECTION_NONE;
    input.direction = prevInput.direction || DIRECTION_NONE;
    input.deltaTime = input.timeStamp - prevInput.timeStamp;
    input.deltaX = prevInput.deltaX || 0;
    input.deltaY = prevInput.deltaY || 0;

    input.speedX = input.deltaX / input.deltaTime;
    input.speedY = input.deltaY / input.deltaTime;

    input.scale = prevInput.scale;
    input.rotation = prevInput.rotation;

    return input;
  }

  var TouchHandler = {
    init: function(elem, gesture, cb, touchMgr) {
      this.session = {};
      this.elem = elem;
      this.touchMgr = touchMgr;
      this.handlers = {};
      this.on(gesture, cb);
    },
    _on: function(gesture, cb) {
      var handlers = this.handlers;
      handlers[gesture] = handlers[gesture] || [];
      handlers[gesture].push(cb);
    },
    on: function(gesture, cb) {
      if (typeof gesture !== "string") {
        return;
      }
      var s = this;
      each(splitStrWithWhiteSpace(gesture), function(gesture) {
        if (gesture === 'touch') {
          s._on('touchstart', cb);
          s._on('touchmove', cb);
          s._on('touchend', cb);
          s._on('touchcancel', cb);
        }
        else if (gesture === 'multitouch') {
          s._on('multitouchstart', cb);
          s._on('multitouchend', cb);
        }
        else {
          s._on(gesture, cb);
        }
      });
    },
    off: function(gesture, cb) {
      if (typeof gesture !== "string") {
        return;
      }
      var handlers = this.handlers;
      each(splitStrWithWhiteSpace(gesture), function(gesture) {
        if (!cb) {
          handlers[gesture] = null;
        }
        else {
          handlers[gesture] && handlers[gesture].splice(inArray(handlers[gesture], cb), 1);
        }
      });
    },
    emit: function(gesture, e, input) {
      var self = this;
      each(splitStrWithWhiteSpace(gesture), function(item) {
        self._emit(item, e, input);
      });
    },
    _emit: function(gesture, e, input) {
      input.event = gesture;
      var handlers = this.handlers[gesture] && this.handlers[gesture].slice();
      if (!handlers || !handlers.length) {
        return;
      }
      for (var i = 0; i < handlers.length; i++) {
        (typeof handlers[i] === "function") && handlers[i](e, input);
      }
    },
    enable: function(gesture) {
    },
    disable: function(gesture) {
    },
    onTouch: function(e) {
      var type = e.type;
      (type === "tap") && this.onTap(e);
      (type === "longtap") && this.onLongtap(e);
      (type === "touchstart") && this.onTouchstart(e);
      (type === "touchmove") && this.onTouchmove(e);
      (type === "touchend") && this.onTouchend(e);
      (type === "touchcancel") && this.onTouchcancel(e);
    },
    onTouchstart: function(e) {
      var input = buildTouchInput(e);
      if (e.touches.length > 1 && !this.session.firstMultiple) {
        this.session.firstMultiple = input;
        this.emit("multitouchstart", e, this.session.firstMultiple);
      }
      else if (e.touches.length === 1) {
        this.session.firstMultiple = null;
      }
      if (!this.session.firstInput) {
        this.session.firstInput = input;
        this.session.prevInput = this.session.firstInput;
        this.emit("touchstart", e, this.session.firstInput);
        return;
      }
      var prevInput = this.session.prevInput;
      input.totalDeltaX = prevInput.totalDeltaX;
      input.totalDeltaY = prevInput.totalDeltaY;
      this.session.prevInput = input;
    },
    onTouchmove: function(e) {
      var input = calTouchmoveData(this.session, e);
      this.emit("touchmove", e, input);
      if (this.session.firstMultiple) {
        this.emit("pinch", e, input);
        this.emit("rotate", e, input);
      }
      this.session.prevInput = input;
    },
    onTouchend: function(e) {
      var input = calTouchendData(this.session, e);
      if (this.session.firstMultiple && e.touches.length < 2) {
        this.session.firstMultiple = null;
        this.emit("multitouchend", e, input);
      }
      if (e.touches.length === 0) {
        this.emit("touchend", e, input);
        this.reset();
      }
      this.session.prevInput = input;
    },
    onTouchcancel: function(e) {
      this.reset();
    },
    onTap: function(e) {
      this.emit("tap", e, this.session.prevInput);
    },
    onLongtap: function(e) {
      this.emit("longtap", e, this.session.prevInput);
    },
    reset: function() {
      this.session.firstInput = null;
      this.session.firstMultiple = null;
      this.session.prevInput = null;
    }
  };

  module.exports = WXTouch;
})();
