

var apiKey = '89n3t8j57at4eukx86j8sty3';
var baseAPI = 'http://api.rottentomatoes.com/api/public/v1.0/movies.json?apikey=' + apiKey;

$("#SearchBtn").click(function () {
    var value = $("#chkIt").val(); 
    var moviesSearchUrl = baseAPI + '=' + encodeURI(value);

    alert(moviesSearchUrl);

    $.ajax({
        url: moviesSearchUrl, //+ '=' + encodeURI(value),
        dataType: 'jsonp',
        success: function (data) {
            
            $(document.body).append('Found ' + data.total + ' results for ' + query);
            var movies = data.movies;

            console.writeln('success');
            
            $.each(movies, function (index, movie) {
                $(document.body).append('<h1>' + movie.title + '</h1>');
                $(document.body).append('<img src="' + movie.posters.thumbnail + '" />');
            });
        }
    });

});

// callback for when we get back the results
function searchCallback(data) {
    $(document.body).append('Found ' + data.total + ' results for ' + query);
    var movies = data.movies;
        
    $.each(movies, function (index, movie) {
        $(document.body).append('<h1>' + movie.title + '</h1>');
        $(document.body).append('<img src="' + movie.posters.thumbnail + '" />');
    });
}