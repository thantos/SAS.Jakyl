# SAS.Jakyl 
**(formerly SAS.UmbracoUnitTesting)**

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

###THIS FEATURE IS IN ALPHA. SUBJECT TO CHANGE AT ANY TIME
Always create a new instance before each test and dispose after each test 
(*see test project for examples*)

`
UmbracoUnitTestEngine _unitTestEngine = new UmbracoUnitTestEngine();
`

The general pattern currently is to declate what is needed for the current 
test by using the With* methods.

For example, to set the current page during the test, use `WithCurrentPage()`. 

`
_unitTestEngine.WithCurrentPage();
`

In many cases, important values such as names, aliases, or IDs will be 
automatically assigned either using AutoFixture, Random, or a Unique ID 
system which is not always being used (only content and media currently use it).
Due to this, leaving such fields empty might benifit the test. 
(*see test project for examples*)

To add content for TypedContent or DynamicContent look ups, use 
`WithPublishedContentPage()`. A similar method exists for media: `WithPublishedMedia()`;

In some cases (like when defining the `CurrentPage` object), the controller 
either needs to have values assigned or a safety check needs to be made.
Use `RegisterController` at any time (before the non-test code is run)
to make sure all controller based operations are considered. If the code
doesn't involve a MVC Controller or API Controller, no registration is
nessecary. 

The engine **SHOULD** resolve all dependancies for items which are 
requested and allow for little to no boilerplate code.

This is a work in progress and community involvment is appreciated. 
Together we can cover every possible situation with grace and poise.

## Nuget

#### Coming Soon

Please poke me if you are reading this in a month and it still says coming soon. 

