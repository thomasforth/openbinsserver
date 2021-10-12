# Open Bins Server
The server component of Open Bins, one of the most used apps in UK local government. Written in C# and ASP.NET using an SQLite database. Over 30 million bin days delivered for under £10,000. Deployed with 100% uptime for over five years on a £50/month Azure Web App.

## Summary
Open Bins is the shared codebase of Leeds Bins, Luton Bins, Fenland Bins, and more local government bin collection apps in the UK. Open Bins tells citizens what to put in their bins, integrates with their device calendar to remind them when to put them outside their property for collection, provides information on other waste services, and provides a notification service to communicate with app users.

## Components of Open Bins
The example containing the most features is the Leeds Bins app, available on both [Android](https://play.google.com/store/apps/details?id=com.imactivate.bins&hl=en_GB) and [iOS](https://itunes.apple.com/gb/app/leeds-bins/id1013036432?mt=8). The core app code is written in HTML5 and a version without native device features is [available on the web](https://imactivate.com/leedsbins/newexample/). App versions are written in Apache Cordova to provide calendar information and notification features. An example [usage dashboard](https://imactivate.com/leedsbins/usageexample/) for the apps is also provided.

## Why this server won't work
The UK's Postcode Address File, linking the UK's addresses with the UK's postcode is not open data. Since bin collection timetables rely on this dataset, bin collection timetables compatible with Open Bins cannot be shared openly on the internet. Bin collection timetables that can be shared under an open licence exist, for example Leeds City Council publishes their timetables, but these do not allow the creation of an app that meets the user expectation of using their postcode to identify their address.
So you run this server yourself, but you probably won't be able to get the data that would let it serve bin days and power a bin app. This obstacle to wider deployment is the result of decisions by the UK government and you may wish to contact your MP if you are a UK resident.

## Accessibility
The app meets most of the [UK government's accessibility requirements for public sector bodies](https://www.gov.uk/guidance/accessibility-requirements-for-public-sector-websites-and-apps) but the cost of proving compliance is currently too high for me to provide proof. This obstacle to wider deployment is the result of decisions by the UK government and you may wish to contact your MP if you are a UK resident.
