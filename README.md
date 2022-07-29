# Rainmeter HttpRequest Plugin

A simple plugin to make http calls.

Rainmeter already includes a feature to make remote http calls: [WebParser](https://docs.rainmeter.net/manual/measures/webparser) but this one do not allow making `POST` request.

This plugin aims to fill this gap and it makes a good match with [Rainmeter JsonParser](https://github.com/e2e8/rainmeter-jsonparser) to integrate with common Rest API services.

## How to install

1. Download the latest version from the [Release Page](https://github.com/ilsasdo/rainmeter-httprequest/releases/latest)
2. Put it in the `Plugin` directory of Rainmeter
3. Restart Rainmeter

## How to use

Measure=HttpRequestPlugin makes an http call to a site.

The measure returns the plain text response with no further modifications.

### Declare the measure

```
[RestApiCall]
Measure=Plugin
Plugin=HttpRequestPlugin
Method=POST
URL=http://yoururl.com
```

### Use the measure (eg with Jsonparser)

```
[Temperature]
Measure=Plugin
Plugin=JsonParser
Source=[RestApiCall]
Query="body.devices[0].dashboard_data.Temperature"
```

### Options

* `URL`: The url to call. You can write the url without any Query Param, those will be specified with the `Param` option.
* `Method`: indicates the `HTTP_METHOD`, it can be any one of `PUT`, `GET`, `POST`, `DELETE`, `PATCH`. If omitted, default is `GET`. Tells `HttpRequestPlugin` to perform the corresponding http method call.
* `ParamNNN`: where NNN is a number from 1 to 99, eg: `Param1`, `Param2`... and so on. Be careful to not skip number: at the first missing param, the parser will stop to search for more params.
The value is in the form: `name=value`. You should not UrlEncode the `value` part, it will be accounted automatically by `HttpRequestPlugin`.
You can use the Option `Param` to specify both Query Params and Form Params to send along the request.
* `HeaderNNN`: same logic as `ParamNNN` but for request headers. You can write in the form of `name: value`, they will be written as-is in the http request.
* `DownloadFile`:  path where to store the downloaded file (like images)
* `OnFinish`: Event triggered on Download Finished with success
* `OnError`: Event triggered on Download Error.

### Examples

Example for a Skin that performs a login to a Rest api service and retrieve the data:

```
[Rainmeter]
Update=120000
AccurateText=1

[Metadata]
Name=Aircare
Author=ilsasdo
Information=Show Netatmo Aircare Data
Version=1.0
License=Creative Commons Attribution - Non - Commercial - Share Alike 3.0

[Variables]
AuthToken=[HomeCoach.AuthToken]

[HomeCoach.Auth]
Measure=Plugin
Plugin=HttpRequestPlugin
Method=POST
UpdateDivider=-1
URL=https://api.netatmo.com/oauth2/token
Param1="client_id=THE_CLIENT_ID_OBTAINED_FROM_NETATMO_WEBSITE"
Param2="client_secret=THE_CLIENT_SECRET_OBTAINED_FROM_NETATMO_WEBSITE"
Param3="grant_type=password"
Param4="username=__USERNAME__"
Param5="password=__PASSWORD__"
Param6="scope=read_homecoach"

[HomeCoach.AuthToken]
Measure=Plugin
Plugin=JsonParser
Source=[HomeCoach.Auth]
Query="access_token"
DynamicVariables=1

[HomeCoach.GetData]
Measure=Plugin
MeasureName=HomeCoach.AuthToken
Plugin=HttpRequestPlugin
Method=GET
DynamicVariables=1
URL=https://api.netatmo.com/api/gethomecoachsdata
Header1="accept:application/json"
Header2="Authorization: Bearer #AuthToken#"

[HomeCoach.Temp]
Measure=Plugin
Plugin=JsonParser
Source=[HomeCoach.GetData]
DynamicVariables=1
Query="body.devices[0].dashboard_data.Temperature"

[HomeCoach.CO2]
Measure=Plugin
Plugin=JsonParser
Source=[HomeCoach.GetData]
DynamicVariables=1
Query="body.devices[0].dashboard_data.CO2"

[HomeCoach.Humidity]
Measure=Plugin
Plugin=JsonParser
Source=[HomeCoach.GetData]
DynamicVariables=1
Query="body.devices[0].dashboard_data.Humidity"

[HomeCoach.Noise]
Measure=Plugin
Plugin=JsonParser
Source=[HomeCoach.GetData]
DynamicVariables=1
Query="body.devices[0].dashboard_data.Noise"

[HomeCoach.HealthIndex]
Measure=Plugin
Plugin=JsonParser
Source=[HomeCoach.GetData]
DynamicVariables=1
Query="body.devices[0].dashboard_data.health_idx"

[MeterTemperature]
Meter=STRING
MeasureName=HomeCoach.Temp
Text="%1°"
Y=0
X=0
FontEffectColor=0,0,0,20
FontColor=255,255,255,204
FontFace=ITC Avant Garde Pro XLt
FontSize=36
AntiAlias=1

[MeterCO2]
Meter=STRING
MeasureName=HomeCoach.CO2
Text="%1ppm"
Y=56r
X=5
FontEffectColor=0,0,0,20
FontColor=255,255,255,204
FontFace=ITC Avant Garde Pro XLt
FontSize=24
AntiAlias=1

[MeterHumidity]
Meter=STRING
MeasureName=HomeCoach.Humidity
Text="%1%"
Y=36r
X=5
FontEffectColor=0,0,0,20
FontColor=255,255,255,204
FontFace=ITC Avant Garde Pro XLt
FontSize=24
AntiAlias=1
```
