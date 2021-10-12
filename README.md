# Open Bins Server
The server component of Open Bins, one of the most used apps in UK local government. Written in C# and ASP.NET. Over 30 million days delivered for well under £10,000.

## Summary
Open Bins is the shared code of Leeds Bins, Luton Bins, Fenland Bins, and more local government bin collection apps in the UK. Open Bins tells citizens what to put in their bins, integrates with their device calendar to remind them when to put them outside their property for collection, provides information 

## Components of Open Bins
The example containing the most features is the Leeds Bins app, available on both [Android](https://play.google.com/store/apps/details?id=com.imactivate.bins&hl=en_GB) and [iOS](https://itunes.apple.com/gb/app/leeds-bins/id1013036432?mt=8). The core app code is written in HTML5 and [available on the web](https://imactivate.com/leedsbins/newexample/). A [usage dashboard](https://imactivate.com/leedsbins/usageexample/) is also provided.

## Why this server won't work
The UK's Postcode Address File, linking the UK's addresses with the UK's postcode is not open data. Since bin collections timetables rely on this dataset, bin collection timetables cannot be shared openly on the internet. So you can't run this server and run your own bin server and your own bin apps. Sorry.
