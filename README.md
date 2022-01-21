# Rainmeter HttpRequest Plugin

A simple plugin to make http calls.

Rainmeter already includes a feature to make remote http calls: [WebParser](https://docs.rainmeter.net/manual/measures/webparser) but this one do not allow making `POST` request.

This plugin aims to fill this gap and it makes a good match with [Rainmeter JsonParser](https://github.com/e2e8/rainmeter-jsonparser) to integrate with common Rest API services.

## How to install

1. Download the latest version release
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

[Temperature]
Measure=Plugin
Plugin=JsonParser
Source=[RestApiCall]
Query="body.devices[0].dashboard_data.Temperature"

### Options

* `URL`: The url to call. You can write the url without any Query Param, those will be specified with the `Param` option.
* `Method`: indicates the `HTTP_METHOD`, it can be any one of `PUT`, `GET`, `POST`, `DELETE`, `PATCH`. If omitted, default is `GET`. Tells `HttpRequestPlugin` to perform the corresponding http method call.
* `ParamNNN`: where NNN is a number from 1 to 99, eg: `Param1`, `Param2`... and so on. Be careful to not skip number: at the first missing param, the parser will stop to search for more params.
The value is in the form: `name=value`. You should not UrlEncode the `value` part, it will be accounted automatically by `HttpRequestPlugin`.
You can use the Option `Param` to specify both Query Params and Form Params to send along the request.
* `HeaderNNN`: same logic as `ParamNNN` but for request headers. You can write in the form of `name: value`, they will be written as-is in the http request.


