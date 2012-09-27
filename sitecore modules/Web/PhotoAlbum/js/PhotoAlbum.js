// -------------------------------------------------------------
// -    Photo Album Slider
// -------------------------------------------------------------

PhotoAlbum = {
    vars: {
        countSeconds: 3,
        controlId: null,
        globalSelf: null,
        isPlay: false,
        timeoutNext: null,
        countSecondsMax: 10,
        isFirstLoad: false
    }
};


var PhotoAlbumSlider = function (id) {
    PhotoAlbum.vars.controlId = id;
    this.parentNode = jQuery("#" + id);
    this.currentNode = this.parentNode.children("div:last");
    this.play = function () {
        PhotoAlbum.vars.timeoutNext = setTimeout(function () { PhotoAlbum.vars.globalSelf.fadeNext(); }, PhotoAlbum.vars.countSeconds * 1000);
    };

    this.fadeNext = function () {
        PhotoAlbum.vars.timeoutNext = null;
        var next = this.currentNode.next('div:first').next('div:first');
        if (typeof next[0] === "undefined") {
            this.currentNode = this.parentNode.children("div:first");
        } else {
            this.currentNode = this.currentNode.next('div:first');
        }
        this.parentNode.children("div").hide();
        this.currentNode.show();

        PhotoAlbum.vars.globalSelf = this;
        if (PhotoAlbum.vars.isPlay)
            PhotoAlbum.vars.timeoutNext = setTimeout(function () {
                if (PhotoAlbum.vars.isPlay)
                    PhotoAlbum.vars.globalSelf.fadeNext();
            }, PhotoAlbum.vars.countSeconds * 1000);
    };

    this.fadePrevious = function () {
        var prev = this.currentNode.prev('div:last');
        if (typeof prev[0] === "undefined") {
            this.currentNode = this.parentNode.children("div:last").prev('div:last');
        } else {
            this.currentNode = prev;
        }
        this.parentNode.children("div").hide();
        this.currentNode.show();
        PhotoAlbum.vars.globalSelf = this;

    };
};

// -------------------------------------------------------------
// -
// -------------------------------------------------------------
jQuery(document).ready(function () {

    jQuery('.photo-album #photo-slideshow .controls .minus').unbind().bind('click', function () {
        PhotoAlbum.vars.countSeconds = parseInt(jQuery(".photo-album #photo-slideshow .controls .interval span").text()) - 1;
        if (PhotoAlbum.vars.countSeconds <= 0)
            PhotoAlbum.vars.countSeconds = PhotoAlbum.vars.countSecondsMax;
        jQuery(" .photo-album #photo-slideshow  .controls .interval span").text(PhotoAlbum.vars.countSeconds);
    });

    jQuery('.photo-album #photo-slideshow .controls .plus').unbind().bind('click', function () {
        PhotoAlbum.vars.countSeconds = parseInt(jQuery(".photo-album #photo-slideshow .controls .interval span").text()) + 1;
        if (PhotoAlbum.vars.countSeconds > PhotoAlbum.vars.countSecondsMax)
            PhotoAlbum.vars.countSeconds = 1;
        jQuery(" .photo-album #photo-slideshow  .controls .interval span").text(PhotoAlbum.vars.countSeconds);

    });

    jQuery('.photo-album #photo-slideshow .controls .back').unbind().bind('click', function () {
        PhotoAlbum.vars.globalSelf.fadePrevious();
    });
    jQuery('.photo-album #photo-slideshow .controls .play').unbind().bind('click', function () {
        jQuery('.photo-album #photo-slideshow .controls .pause').css("display", "block");
        jQuery(this).css("display", "none");

        PhotoAlbum.vars.globalSelf.play();
        PhotoAlbum.vars.isPlay = true;
    });
    jQuery('.photo-album #photo-slideshow .controls .pause').unbind().bind('click', function () {
        jQuery('.photo-album #photo-slideshow .controls .play').css("display", "block");
        jQuery(this).css("display", "none");
        PhotoAlbum.vars.timeoutNext = null;
        PhotoAlbum.vars.isPlay = false;
    });
    jQuery('.photo-album #photo-slideshow .controls .forward').unbind().bind('click', function () {
        PhotoAlbum.vars.globalSelf.fadeNext();
    });
    jQuery(' .photo-album #photo-slideshow .controls .hide-captions').unbind().bind('click', function () {
        var showOrHide = jQuery(".photo-album #photo-slideshow .overlay ").toggle();
        jQuery(this).text(jQuery(this).text() == 'hide captions' ? 'show captions' : 'hide captions');

    });


    function changeRate(value) {
        jQuery(value).parents('ul.stars').find('li a').removeClass('current');
        jQuery(value).addClass('current');
        jQuery('#photo_rate_rate').val(value.id.charAt(4));
    }

    jQuery(".rate1").click(function () {
        changeRate(this);
    });
    jQuery(".rate2").click(function () {
        changeRate(this);
    });
    jQuery(".rate3").click(function () {
        changeRate(this);
    });
    jQuery(".rate4").click(function () {
        changeRate(this);
    });
    jQuery(".rate5").click(function () {
        changeRate(this);
    });

});



  