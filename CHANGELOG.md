# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0]

####  Changed

- updated some functions comments.
- updated the naming of the projects and namespaces from .NET to .Net.
- updated `WithHeaders()` input type from Dictionary to `IEnumerable<KeyValuePair<string, string>>`
- updated `WithHeaders()` method on the message composer to add the list of to append the values to internal list instead of overriding it,
- updated the visibility of the message constructor to internal, to make sure all message instances are created through the factory.

## [1.0.0]
 
the initial release