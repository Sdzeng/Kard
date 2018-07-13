// Kard Web Gruntfile
module.exports = function (grunt) {

    'use strict';

    grunt.initConfig({

        watch: {
            // If any .less file changes in directory "../less/AdminLTE/" run the "less"-task.
            files: ["../less/*.less", "../js/*.js"],
            tasks: ["cssBuild", "jsBuild"]
        },
        // "less"-task configuration
        // This task will compile all less files upon saving to create both AdminLTE.css and AdminLTE.min.css
        less: {
            // Development not compressed
            toBuildCss: {
                options: {
                    // Whether to compress or not
                    compress: false
                },
                files: [{
                    // compilation.css  :  source.less

                    "../css/kard.css": "../less/kard.less",
                    "../css/m-kard.css": "../less/m-kard.less",


                }
                    //    ,
                    //    {
                    //    expand: true, 
                    //    cwd: '../less',//lessÄ¿Â¼ÏÂ
                    //    src: ['*.less', '**/*.less'],//ËùÓÐlessÎÄ¼þ
                    //    dest: '../builds/css',//Êä³öµ½´ËÄ¿Â¼ÏÂ
                    //    ext: '.css',
                    //    extDot: 'last'
                    //}
                ]
            },
            minBuildCss: {
                options: {
                    // Whether to compress or not
                    compress: true
                },
                files: [{
                    expand: true,
                    cwd: '../css',//cssÄ¿Â¼ÏÂ
                    src: ['*.css', '**/*.css'],//ËùÓÐcssÎÄ¼þ
                    dest: '../builds/css',//Êä³öµ½´ËÄ¿Â¼ÏÂ
                    ext: '.css',
                    extDot: 'last'
                }]
            }
        },
        // Uglify task info. Compress the js files.
        uglify: {
            options: {
                mangle: true,
                preserveComments: 'some'
            },
            toBuildJs: {
                files: [{
                    //'../builds/js/app.min.js': ['../js/app.js'],
                    //'../builds/js/demo.min.js': ['../js/demo.js'],
                    //'../builds/js/pages/*.min.js': ['../js/pages/*.js']
                    expand: true,
                    cwd: '../js',//jsÄ¿Â¼ÏÂ
                    src: ['*.js', '**/*.js'],//ËùÓÐjsÎÄ¼þ
                    dest: '../builds/js'//Êä³öµ½´ËÄ¿Â¼ÏÂ
                    //ext: '.min.js',
                    //extDot: 'last'
                }]
            }
        },
        htmlmin: {
            options: {
                removeComments: true, //移除注释
                removeCommentsFromCDATA: true,//移除来自字符数据的注释
                collapseWhitespace: true,//无用空格
                collapseBooleanAttributes: true,//失败的布尔属性
                //removeAttributeQuotes: true,//移除属性引号      有些属性不可移走引号
                removeRedundantAttributes: false,//移除多余的属性
                useShortDoctype: true,//使用短的跟元素
                removeEmptyAttributes: true//移除空的属性
                //removeOptionalTags: true//移除可选附加标签
            },
            yasuo: {
                files: [{
                    expand: true,
                    cwd: '../',
                    src: ['*.html'],
                    dest: '../builds'
                }, {
                        expand: true,
                        cwd: '../m/',
                        src: ['*.html'],
                        dest: '../builds/m'
                    }]
            }
        },
        // Copy the js min files.
        copy: {
            //toBuildCss:{
            //    files: [{
            //        expand: true,
            //        cwd: '../css',
            //        src: ['*'],
            //        dest: '../builds/css'
            //    }]
            //},
            //toBuildJs: {
            //    files: [{
            //        expand: true,
            //        cwd: '../js',//jsÄ¿Â¼ÏÂ
            //        src: ['*.js', '**/*.js'],//ËùÓÐmin.jsÎÄ¼þ
            //        dest: '../builds/js',//Êä³öµ½´ËÄ¿Â¼ÏÂ
            //    }]
            //},
            toBuildJson: {
                files: [{
                    expand: true,
                    cwd: '../json',
                    src: ['*'],
                    dest: '../builds/json'
                }]
            },
            toBuildPlugins: {
                files: [{
                    expand: true,
                    cwd: '../plugins',
                    src: ['*.*', '**/*.*'],
                    dest: '../builds/plugins'
                }]
            },
            toBuildFonts: {
                files: [{
                    expand: true,
                    cwd: '../fonts',
                    src: ['*.*', '**/*.*'],
                    dest: '../builds/fonts'
                }]
            },
            toBuildImage: {
                files: [{
                    expand: true,
                    cwd: '../image',
                    src: ['*.*', '**/*.*'],
                    dest: '../builds/image'
                }]
            }
        },
        // Build the documentation files
        includes: {
            build: {
                src: ['*.html'], // Source files
                dest: 'documentation/', // Destination directory
                flatten: true,
                cwd: 'documentation/build',
                options: {
                    silent: true,
                    includePath: 'documentation/builds/include'
                }
            }
        },

        // Optimize images
        image: {
            dynamic: {
                files: [{
                    expand: true,
                    cwd: '../image/',
                    src: ['*.*', '**/*.*'],
                    dest: '../builds/image/'
                }]
            }
        },

        //// Validate JS code
        //jshint: {
        //    options: {
        //        jshintrc: '../js/.jshintrc'
        //    },
        //    core: {
        //        src: '../js/app.js',
        //    },
        //    demo: {
        //        src: '../js/demo.js'
        //    }
        //    //pages: {
        //    //    src: '../js/pages/*.js'
        //    //}
        //},

        //// Validate CSS files
        //csslint: {
        //    options: {
        //        csslintrc: '../less/AdminLTE/.csslintrc'
        //    },
        //    dist: [
        //      '../builds/css/AdminLTE.min.css',
        //    ]
        //},

        //// Validate Bootstrap HTML
        //bootlint: {
        //    options: {
        //        relaxerror: ['W005']
        //    },
        //    files: ['../assest/less/bootstrap/pages/**/*.html', '../assest/less/bootstrap/pages/*.html']
        //},

        // Delete images in build directory
        // After compressing the images in the builds/img dir, there is no need
        // for them
        clean: {
            options: { force: true },
            css: ["../builds/css/*"],
            js: ["../builds/js/*"],
            html: ["../builds/*.html"],
            json: ["../builds/json/*"],
            plugins: ["../builds/plugins/*"],
            fonts: ["../builds/fonts/*"],
            //noMinCss: ["../builds/css/*.css", "../builds/css/**/*.css", "!../builds/css/*.min.css", "!../builds/css/**/*.min.css"],
            noMinJs: ["../builds/js/*.js", "../builds/js/**/*.js", "!../builds/js/*.min.js", "!../builds/js/**/*.min.js"]
        }
    });

    // Load all grunt tasks

    // LESS Compiler
    grunt.loadNpmTasks('grunt-contrib-less');
    // Watch File Changes
    grunt.loadNpmTasks('grunt-contrib-watch');
    // Compress JS Files
    grunt.loadNpmTasks('grunt-contrib-uglify');
    // Include Files Within HTML
    grunt.loadNpmTasks('grunt-includes');
    // Optimize images
    grunt.loadNpmTasks('grunt-image');
    // Validate JS code
    grunt.loadNpmTasks('grunt-contrib-jshint');
    // Delete not needed files
    grunt.loadNpmTasks('grunt-contrib-clean');
    // Lint CSS
    grunt.loadNpmTasks('grunt-contrib-csslint');
    // Mini Html
    grunt.loadNpmTasks('grunt-contrib-htmlmin');
    /* // Lint Bootstrap
     grunt.loadNpmTasks('grunt-bootlint');*/

    // Copy File or Folder
    grunt.loadNpmTasks('grunt-contrib-copy');

    // Clean File or Folder
    grunt.loadNpmTasks('grunt-contrib-clean');

    // Linting task
    //grunt.registerTask('lint', ['jshint', 'csslint', 'bootlint']);



    // css task
    grunt.registerTask('cssBuild', ["clean:css", "less:toBuildCss", "less:minBuildCss"]);

    // js task
    grunt.registerTask('jsBuild', ["clean:js", "uglify:toBuildJs"]);

    grunt.registerTask('htmlBuild', ["clean:html", "htmlmin"]);

    // json task
    grunt.registerTask('jsonBuild', ["clean:json", "copy:toBuildJson"]);

    // plugins task
    grunt.registerTask('pluginsBuild', ["clean:plugins", "copy:toBuildPlugins"]);

    // fonts task
    grunt.registerTask('fontsBuild', ["clean:fonts", "copy:toBuildFonts"]);

    // copy task
    grunt.registerTask('copyBuild', ["copy:toBuildImage"]);

    //main task
    grunt.registerTask('build', ["cssBuild", "jsBuild", "htmlBuild", "jsonBuild", "copyBuild"]);//, "pluginsBuild", "fontsBuild"

    // The default task (running "grunt" in console) is "build"
    grunt.registerTask('default', ['build']);
};
