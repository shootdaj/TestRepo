module.exports = function (grunt) {
	grunt.initConfig({
		bower: {
			install: {
				options: {
					targetDir: "wwwroot/lib",
					layout: "byComponent",
					cleanTargetDir: true
				}
			}
		},
		// Add this JSON object:
		less: {
			development: {
				options: {
					paths: ["Assets"],
				},
				files: { "wwwroot/css/site.css": "assets/site.less" }
			},
		}
	});

	grunt.registerTask("default", ["bower:install"]);

	grunt.loadNpmTasks("grunt-bower-task");
	// Add this line:
	grunt.loadNpmTasks("grunt-contrib-less");
};