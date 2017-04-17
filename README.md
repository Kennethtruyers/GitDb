# GitDb

GitDb is a proof of concept to use Git as a NoSql database

It consists of 6 projects:

## GitDb.Core

Defines the core interfaces any GitDb implementation should implement as well as some DTO's for data transport

## GitDb.Local

Provides an implementation to use a local GIT repository as a data store

## GitDb.Remote

Provides an implementation to access a GIT repository over HTTP as a data store (it talks to a GitDb.Server instance)

## GitDb.Server

An ASP.NET Web API that exposes a GitDb.Local's method over an HTTP interfaces

## GitDb.Tests

Unit and Integration tests for all other projects

## GitDb.Watcher

Class library that can be used to watch a local Git repository for changes and expose events whenever something changes. 
This can be used to keep a secondary data store in sync with the data in Git.
The raised events will contain information about all modifications:
- Added / removed branches
- Added / modified / deleted items
- Current / previous commits

