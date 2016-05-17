# BrowserExtensionFinder

BrowserExtensionFinder (BEF) is a tool for running on a live system to get a list of installed plugins for Firefox and Chrome.  It parses the Firefox sqlite database for information about installed extensions and parses the manifest.json file for each installed Chrome extension.

By default, the output displays in a list format.  CSV output can be specified using the --type=CSV flag.

Newtonsoft.Json.dll must be included in the directory with the BrowserExtensionFinder.exe executable when running.

The exception.chrome file is not required, but can be used to specify Chrome extensions that can be excluded from the output of BEF.  Note that these files are fully excluded in the CSV output, but in the default output they are instead moved to the bottom of the report and the manifest.json files are not parsed. 
