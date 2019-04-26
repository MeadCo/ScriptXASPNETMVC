# MeadCo ScriptX Helpers for ASP.NET MVC 
A library of helpers for ASP.NET MVC Razor views to simplify the use of The MeadCo ScriptX Add-on for Internet Explorer to deliver controlled printing on client PCs. This library is not appropriate for use with .NET Core.

## Current Version
2.4.1

v2.4 Added suport for async startup when using ScriptX Services.

v2.3 Added support for working with enhanced PDF printing in ScriptX Services

v2.2 Added support for working with ScriptX Services for Windows PC.

v2.1 Addedsupport for working with a ScriptX Services - it will output code for working with the service instead of working with the add-on.

v2.1 is source compatible with views written against v1. 

However, note that v2.1 is under a different naming scheme on Nuget because of incompatibilities in the dependencies. To upgrade, uninstall the v1 package and install this. 

v2 uses the new [Configuration handler][4] which is far more flexible than the v1 package and de-couples the help library from the binary bits for ScriptX itself.

## Nuget Gallery
[MeadCoScriptXASPNETMVC][1]
## Use
An [overview][5] based on the v1 version of the helpers but that still applies to v2 is available on the [ScriptX web site][5].

Note that the ability for the [configuration handler][4] to be configured with multiple versions of the ScriptX enables this version of the helpers to offer the best version of ScriptX to the user depending upon the version of Internet Explorer they are using.

## Copyright
Copyright © 2016, 2018 [Mead & Co Ltd][6].

## License 
**MeadCo ScriptX Helpers for ASP.NET MVC** is under MIT license - http://www.opensource.org/licenses/mit-license.php

[1]: https://www.nuget.org/packages/MeadCoScriptXASPNETMVC/
[2]: http://scriptxprintsamples.meadroid.com/
[3]: http://scriptx.meadroid.com
[4]: https://github.com/MeadCo/ScriptXConfig
[5]: http://scriptx.meadroid.com/knowledge-bank/guide-for-client-side-printing/quick-start-with-visual-studio/aspnet-mvc-web-application.aspx
[6]: http://scriptx.meadroid.com
