var apikey = "apikey";
        var baseUrl = "http://api.rottentomatoes.com/api/public/v1.0";

        // construct the uri with our apikey
        var moviesSearchUrl = baseUrl + '/movies.json?apikey=' + apikey;

        $("#SearchBtn").click(function () {
            var value = $("#chkIt").val();

            $.ajax({
                url: moviesSearchUrl + '&q=' + encodeURI(value),
                dataType: "jsonp",
                success: searchCallback
            });
            
        });

 

        // callback for when we get back the results
        function searchCallback(data) {
            $(document.body).append('Found ' + data.total + ' results for ' + query);
            var movies = data.movies;

            $.each(movies, function(index, movie) {
                $(document.body).append('<h1>' + movie.title + '</h1>');
                $(document.body).append('<img src="' + movie.posters.thumbnail + '" />');
            });
        }