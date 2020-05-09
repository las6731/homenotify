# HomeNotify

Basic RESTful web API built with ASP.NET Core to allow push notifications to be sent via API call to Android devices with the accompanying Xamarin app installed. Firebase Cloud Messaging is used to send notifications to the devices, and MongoDB is used for logging and for keeping track of notification topics that have been created through the API.

## About

This is intended for personal use as part of a home automation setup I hope to have one day, to allow other services to send notifications to my phone without needing to implement this ability in every application. Possible usages include (but aren't limited to) status alerts, activity detected by motion detectors, and basically anything else that could feasibly be automated - I'm sure your imagination is much better than mine. Many home automation appliances do offer notifications as an included feature, but a DIY solution just sounds more fun to me.

Feel free to use or modify this yourself, just keep in mind that I make no guarantees about whether this solution is actually any good. If you'd like to contribute, PRs are most definitely welcome.

## Usage

TODO