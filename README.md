# KBucket
[![build status](https://ci.appveyor.com/api/projects/status/github/richardschneider/k-bucket?branch=master&svg=true)](https://ci.appveyor.com/project/richardschneider/k-bucket) 
[![CircleCI](https://circleci.com/gh/richardschneider/k-bucket.svg?style=svg)](https://circleci.com/gh/richardschneider/k-bucket)
[![Coverage Status](https://coveralls.io/repos/richardschneider/k-bucket/badge.svg?branch=master&service=github)](https://coveralls.io/github/richardschneider/k-bucket?branch=master)
[![Version](https://img.shields.io/nuget/v/Makaretu.svg)](https://www.nuget.org/packages/Makaretu.KBucket)
[![docs](https://cdn.rawgit.com/richardschneider/k-bucket/master/doc/images/docs-latest-green.svg)](https://richardschneider.github.io/k-bucket/articles/intro.html)

A [Distributed Hash Table](http://en.wikipedia.org/wiki/Distributed_hash_table)(DHT) is a 
decentralized distributed system that provides a lookup table similar to a hash table. 
*k-bucket* is an implementation of a storage mechanism for keys within a DHT. 

It manages `IContact` objects which represent locations and addresses of nodes in the system. 
`contact` objects have an ID, which is typically a SHA-1 hash.

## Getting started

Published releases are available on [NuGet](https://www.nuget.org/packages/Makaretu.KBucker).  To install, 
run the following command in the [Package Manager Console](https://docs.nuget.org/docs/start-here/using-the-package-manager-console).

    PM> Install-Package Makaretu.Mdns

## Usage

todo

## Credits

This is largely based on the javascript library by [tristanls](https://github.com/tristanls/k-bucket).
