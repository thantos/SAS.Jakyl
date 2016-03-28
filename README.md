# SAS.UmbracoUnitTesting

## Introduction

In an attempt to match our Umbraco development to our other CMS development, 
I set off to understand what it would take to reliably unit test Umbraco. 
The attempt of this project is the reduce the headache it would take to unit 
test the cached Umbraco Objects (IPublishedContent, UmbracoHelper) which 
should lead to more often unit tested Umbraco projects.

## Umbraco Versions

I hope to keep up with new Umbraco releases. I welcome additional branches ported
to older Umbraco versions, but it is my intention to backport at last to 7.3.8.
As of 3/24/16 the project is using 7.4.2.

## Examples

The UmbracoUnitTest.TestWeb.Test project has examples of testing the simple
code found in the UmbracoUnitTesting.TestWeb project. If you do not with to
use the included helper or the future planned abstracted system, the examples
should be a good starting point.

## UmbracoUnitTestHelper

This is intended to cover 95% of the possible inputs and allow for overriding 
and mocking where possible. It is very possible it will be extended in the 
future, but hopefully in a backwards compatable way only.

This helper is stateless and will only aid in writing shorter code with
less boiler plate.

## UmbracoUnitTest Engine

#### Coming Soon. Name Pending.

## Nuget

#### Coming Soon

Please poke me if you are reading this in a month and it still says coming soon. 

