## SharpMirror
### A smart mirror project built on .NET 6 & Blazor

![screenshot example](https://github.com/senj/SharpMirror/blob/master/Documentation/example_2.png "Screenhot example")

This is a blazor (server-side) web application.
It's a development project and no out of the box product.
There is heavy use of (experimental) browser APIs, best experience is with Chrome.

## Modules right now:
- Clock & Timer
- Calendar
- Weather (current and next seven days)
- News (choose your own RSS feed)
- Fuel prices
- Fitbit
- Spotify
- Shopping List Bring
- Hue Lights (if hosted in local network)
- VVS (german local traffic around stuttgart)
- German football results (1. Bundesliga)
- Space (moon phases, ISS live feed)
- Chuck norris jokes
- Google FIT Api
- Route/Traffic information

## API Access
You need some ApiKeys or accounts to use all of the features.
You can set these secrets as environment variables. (see docker-compose.yml)

- ASPNETCORE_ENVIRONMENT=
- CalendarConfiguration__CalendarUrl=
- WeatherConfiguration__ApiKey=
- FuelConfiguration__ApiKey=
- SpotifyConfiguration__ClientSecret=
- FitbitConfiguration__ClientSecret=
- RedisConfiguration__Configuration=
- BringConfiguration__Email=
- BringConfiguration__Password=
- RouteConfiguration__ApiKey=
- SpeechRecognitionConfiguration__SpeechApiSubscriptionKey
- SpeechRecognitionConfiguration__LuisAppId
- SpeechRecognitionConfiguration__LuisSubscriptionKey
- SpeechRecognitionConfiguration__LuisEndpoint
- GoogleApiConfiguration__ClientId
- GoogleApiConfiguration__ClientSecret

### Get access
For most of the APIs, request limits apply. Therefore this project makes heavy use of caching using a redis cache. 
- Calendar: e.g. use a private google calendar ics link.
- Weather: this project uses the *One Call API* of https://openweathermap.org/api
- fuel prices: check out this website: https://creativecommons.tankerkoenig.de/
- Spotify: login here and create a client application: https://developer.spotify.com/dashboard/ (Spotify uses OAuth2)
- Fitbit: login here and create a client application: https://dev.fitbit.com/login (Fitbit uses OAuth2)
- Route/Traffic: Azure Map Service
- Google FIT: Google Developer Console

## Build & Run
- This can be built and run as a docker container.
- Use the *Dockerfile* to build the docker container and *docker-compose.yml* to run the container.
- You can also run the project with ASP.NET Core tools (https://dotnet.microsoft.com/download)

This project can be hosted in a docker container on a raspberry pi. It should run within your local network for features like Hue Lights to work.
Speech input / output only works using HTTPS, right now the container must be deployed behind a nginx reverse proxy terminating TLS. 

There is also no concept of different users/logins right now, so your network should not be reachable from the outside.

## Software Design
- Each component must work on it's own and can be (de)activated at every time using speech input
- There is no user input except speech. Therefore components must refresh themselfes on a regular basis or support speech commands.

## Interface Design
- Inspired by: https://material.io/design/
- Because this application is used behind a mirror glass the background should be dark with high contrast to the content. 
