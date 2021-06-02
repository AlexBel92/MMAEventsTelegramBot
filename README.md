## MMA events telegram bot

https://t.me/MMAEvents_Bot

Telegram bot can tell you about the planned and past MMA events.
And also report to a separate channel about changes in scheduled MMA events.

The implementation used:
* Web api for telegram webhook
* gRpc as entry point for changed events
* Client for events web api based on OpenApi specification
* For caching calls to the events web api the proxy pattern is used
