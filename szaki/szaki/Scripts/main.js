$(document).ready(function () {
    $(window).scroll(function () { // check if scroll event happened
        if ($(document).scrollTop() > 50) { // check if user scrolled more than 50 from top of the browser window
            $("#nav-home.navbar-fixed-top").css("background-color", "#f8f8f8"); // if yes, then change the color of class "navbar-fixed-top" to white (#f8f8f8)
            $("#nav-home.navbar ul li a").css("color", "#333");
            $("#nav-home.navbar ul li ul li a").css("color", "#fff");
            $("#nav-home .navbar-brand").css("color", "#333");
        } else {
            $("#nav-home.navbar-fixed-top").css("background-color", "transparent"); // if not, change it back to transparent
            $("#nav-home.navbar ul li a").css("color", "#fff");
            $("#nav-home .navbar-brand").css("color", "#fff");
        }
	});

});