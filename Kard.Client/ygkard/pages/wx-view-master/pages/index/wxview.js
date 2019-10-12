(function() {
    var wxView = {
        elasticity:0.45,
        upBound:0,
        downBound:0,
        diff:.5,
        friction:.98,
        speedMultiplier:16,
        maxSpeed:35,
        stopSpeed:.1,
        upRatio:10,
        downRatio:.9,
        loopInterval:4,
        isMove:false,
        stopMove:false,
        checkY:true,

        y:0,
        render:null,
        width:0,
        height:0,

        setWH: function(width, height) {
            this.width = width;
            this.height = height;
        },
        setBound: function(upBound, downBound) {
            this.upBound = upBound;
            this.downBound = downBound || 0;
        },
        setCheckY: function(checkY) {
            this.checkY = checkY;
        },
        setRender: function(render) {
            this.render = render;
        },

        setY: function(y) {
            this.y = y;
            this.isMove = false;
            this.render(y);
        },
        moveY: function(y) {
            this.y = y;
            this.render(this.y);
        },
        ontouchstart: function() {
            if (this.isMove) {
                this.stopMove = true;
            }
        },
        ontouchmove: function(dy) {
            if (!this.checkY) {
                return;
            }
            if (this.y + dy < this.upBound || this.y + dy > this.downBound) {
                dy = dy * this.elasticity;
            }
            this.moveY(this.y + dy);
        },
        ontouchend: function(speed) {
            if (!this.checkY) {
                return;
            }
            speed = Math.max(-this.maxSpeed, Math.min(this.maxSpeed, speed * this.speedMultiplier));
            this.isMove = true;
            this.stopMove = false;
            var s = this;
            loop();

            function loop() {
                if (s.stopMove) {
                    s.isMove = false;
                    return;
                }
                var y = 0;
                var isLoop = true;
                speed *= s.friction;
                if (s.y > s.diff + s.downBound) {
                    s.y *= s.downRatio;
                    speed *= s.friction;
                    if (s.y > s.diff + s.downBound) {
                        y = s.y + speed;
                    }
                    else {
                        y = s.downBound;
                        isLoop = false;
                    }  
                }
                else if (s.y < s.upBound - s.diff) {
                    s.y += (s.upBound - s.y) / s.upRatio;
                    speed *= s.friction;
                    if (s.y < s.upBound - s.diff) {
                        y = s.y + speed;
                    }
                    else {
                        y = s.upBound;
                        isLoop = false;
                    }
                }
                else if (Math.abs(speed) > 0.1) {
                    y = s.y + speed;
                }
                else {
                    y = s.y + speed;
                    isLoop = false;
                }
                s.moveY(y);
                s.isMove = isLoop;
                if (isLoop) {
                    setTimeout(loop, s.loopInterval);
                }
            }
        },
    };

    function createWXView() {
        return Object.create(wxView);
    }

    module.exports = {createWXView:createWXView};
})();