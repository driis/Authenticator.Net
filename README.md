Authenticator.Net
=================

_Hobby project in progress - you should not attempt to use this for anything security related unless you know what you are doing and know how to verify the implementation._

Two factor authentication have become common on the web today. This library attempts to implement the open standards used in many places in .NET for easy consumption by .NET developers.
These standards are:

* [RFC 4226](https://tools.ietf.org/html/rfc4226): HMAC-Based One-time Password (HOTP) algorithm 
* [RFC 6238](https://tools.ietf.org/html/rfc6238): Time-based One-time Password (TOTP) algorithm

The initial goal will be to provide the client side parts of the two RFCs, thus making it easy to implement a Google Authenticator style app in .NET, or embed one-time password generation in existing apps.

Serverside functionality (Validating passwords) might be added later.