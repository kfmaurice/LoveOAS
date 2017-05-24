[![Build status](https://ci.appveyor.com/api/projects/status/3x3mydoni0j12tsr/branch/master?svg=true)](https://ci.appveyor.com/project/kfmaurice/loveoas/branch/master)
# LoveOAS
A library to bring you closer to HATEOAS without hating it :-)

## Abstract
Implementing HATEOAS for a web application mostly boils down to the fact that contextual links are embedded in server responses to allow clients to navigate throughout the application. Most of the time, these links are included per entity. So, if the response of a REST endpoint is an array of object then each object contains hypermedia links for acting on the object.

```c#
{
  "payload": [{
    "id": 1,
    "firtname": "maurice",
    "lastname": "kf",
    "links": [{ "rel": "self", "href": "http://url.com/api/people/1", "method": "GET" }]
  }, {
    "id": 2,
    "firstname": "git",
    "lastname": "hub",
    "links": [{ "rel": "self", "href": "http://url.com/api/people/2", "method": "GET" }]
  }],
  "links": [{ "rel": "search", "href": "http://url.com/api/people/search", "method": "POST" }]
}
```

## This Plugin
The idea behind LoveOAS is to implement something similar to HATEOAS... with a subtle difference. Links will not be injected into individual entities but instead to the response object as a whole. So, if the response of a REST endpoint is an array then it will be modified to contain links pointing to all other logical REST endpoints from there.

```c#
{
  "payload": [{
    "id": 1,
    "firtname": "maurice",
    "lastname": "kf"
  }, {
    "id": 2,
    "firstname": "git",
    "lastname": "hub"
  }],
  "links": [{ 
    "rel": "search",
    "href": "http://url.com/api/people/search" ,
    "method": "POST"
  }, { 
    "rel": "entity", 
    "href": "http://url.com/api/people/{id}",
    "method": "GET"
  }]
}
```

In other terms, the idea is to extend responses with links without having to tweak on entities. With this, adding the HATEOAS constraint on an existing REST API should (hopefully) feel like a piece of cake.

## Installation
First things first, this plugin targets .NET 4.6.1+ and has been developed for Web APi 2 and .NET Core (only full .NET Framework is supported at the moment). Web application built with ASP.NET MVC and Web Api 2 require the nuget package ``Dynamite.LoveOAS``. For .NET Core the package ``Dynamite.LoveOAS.NETCore`` is additionally required.

## How To Use
The plugin usage is based on attributes applied to controller actions, filters which act on these attributes and a bit of setup to boot up the plugin so you need to do the 3 following steps in the order you wish:

1. Use attributes on action which shold be extended to return an object with the ``payload`` and ``links`` properties.
2. Register a filter or filters which will actually modify the response of actions marked with the attributes explained later.
3. Boot the plugin to allow the graph to be build once when the application starts.


### Attributes
The plugin relies on the fact that all your REST endpoints could be connected between each other into a structure similar to a graph. No doubt, there might be isolated endpoints but they are part of the structure anyway. So when you make a request to a specific endpoint, the plugin makes a lookup in the graph to find all the possible exits and write these in the ``links`` property of the result. Therefoe, to help build the graph, you should be aware of three attributes:
- Entry
```c#
[Entry("All People")]
[Route("api/people")]
Public MyObject GetAllPeople()
{
  ...
}

...

[Entry("Search")]
[Route("api/people/search")]
Public MyResult SearchPeople(MySearchParameters parameters)
{
  ...
}
```

With this attribute you mark the endpoint to be part of the graph. Additionally it will be considered as an entry point to the graph i.e. a starting point for navigation. For example, when you open an online shop you might be first interested on which categories of products are sold there before browsing through them.

- Base

Having all the entry points, it only makes sense to have one controller action return all these as an object. With the ``Base`` attribute, you can mark an endpoint to be part of the graph and to return all entry points in the ``links`` property. Nothing prevents you from adding an object of your own through the ``payload`` property of the result;

```c#
[Base]
Public MyStartObject GetEntries()
{
  ...
  
  return myResult;
}
```
A call to the endpoint above results in the following json response.
```c#
{
  "payload": json_of_myResult, 
  "links": [{
    "rel": "search",
    "href": "http://url.com/api/people/search",
    "method": "POST"
   }, {
    "rel": "all people",
    "href": "http://url.com/api/people",
    "method": "GET"
   }, 
   ...]
}
```

- Exit

With the entries in place, you need a way to let api users discover where they can navigate to. With the ``Exit`` attributes, you mark an endpoint to be part of the graph and specify an exit of the endpoint at the same time which results in an entry in the ``links`` property. Endpoints marked as ``Entry`` might be (and maybe) should also specify exits.

```c#
[Entry("All People")]
[Exit(nameof(SearchController.Search), typeof(SearchController), "Search")]
[Route("api/people")]
Public MyObject GetAllPeople()
{
  ...
}

[Route("api/people/{id}")]
[Exit(nameof(GetPeople), typeof(PeopleController), "Self")]
[Exit(nameof(DeletePeople), typeof(PeopleController), "Delete")]
[Exit(nameof(EditPeople), typeof(PeopleController), "Modify")]
Public MyPeople GetPeople(int id)
{
  ...
  
  return myPeopleResult;
}

[Route("api/people/{id}")]
[HttpDelete]
Public void DeletePeople(int id)
{
  ...
}

[Route("api/people/{id}")]
[HttpPut]
Public MyPeople EditPeople(MyPeople people)
{
  ...
}

...

[Entry("Search")]
[Route("api/people/search")]
Public MyResult SearchPeople(MySearchParameters parameters)
{
  ...
}
```
A call to the endpoint "GetPeople" results in the following json response.
```c#
{
  "payload": json_of_myPeopleResult, 
  "links": [{ 
    "rel": "self",
    "href": "http://url.com/api/people/{id}",
    "method": "GET"
  }, { 
    "rel": "delete",
    "href": "http://url.com/api/people/{id}",
    "method": "DELETE"
  }, { 
    "rel": "modify",
    "href": "http://url.com/api/people/{id}",
    "method": "PUT"
  }]
}
```

### Filters
With attributes in places, you'll need to register filters which would act on the endpoints marked with the attributes above. Depending on which kind of project you have, there are 2 ways to register the filter. For Web Api 2, you'll probably do it in ``WebApiConfig.cs``:

```c#
using LoveOAS.Discovery;
using LoveOAS.Filters;
...
public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      ...
      var orchestrator = new Orchestrator(settings: WebApiApplication.Settings);

      config.Filters.Add(new LoveOasFilter(orchestrator));

      ...
    }
  }
```
We'll get to the settings later in the next chapter.

If your project is an ASP.NET Core Web Application targeting the fll .NET Framework then you should rather register the filter in the startup class ``Startup.cs``:
```c#
using LoveOAS.NetCore.Filters;
...
public void ConfigureServices(IServiceCollection services)
{
  ...
  services.AddMvc(options =>
  {
    options.Filters.Add(typeof(LoveOASFilterAttribute));
  });
  ...
}
```
Note that the namespace for the filters is now ``LoveOAS.NetCore.Filters``.

### Setup And Settings
Before the filters are able to extend controller actions marked with the attributes presented above, you need to boot the plugin. During theis step a graph of endpoints will be constructed and cached so that each request do not need to do it again. With ASP.NET MVC and Web APi 2, you might call the following code in ``WebApiConfig``:
```c#
public static class WebApiConfig
{
  public static void Register(HttpConfiguration config)
  {
    ...
    var processor = new Processor(mySettings);
    ...
    processor.Setup(namespaces: nameof(MyProjectNamespace));
    ...
  }
}

```
The plugin heavily relies on reflection to construct the graph which means you have to submit a namespace in which all the relevant endpoint can be found.

With ASP.NET Core, you would rely on dependency injection to get an ``IProcessor`` instance. Anyway, this requires you to have configured how an instance of ``IProcessor`` should be configured. See the sample in [Startup.cs](TestWebApiCore/Startup.cs) for a clue. Note that settings are also injected in this sample:

```c#
public void ConfigureServices(IServiceCollection services)
{
  ...
  services.BuildServiceProvider().GetService<IProcessor>().Setup(namespaces: nameof(MyProjectNamespace));
  ...
}
```

The primary use of the settings is to configure how the graph is constructed. See [ISettings.cs](LoveOAS/Interfaces/ISettings.cs) for all the properties to care about to tweak the endpoint discovery. Concerning the ``Mode`` property, only ``Boot`` is currently supported
 and means that the graph is built once when the application starts and never changed on runtime.
 
### Authorization
There might the case that you do not wish to publish links for which the user is not authorized. Normally, with Web Api 2 you would use the ``Authorize`` attribute to require authorization on such links. So, for the plugin to detect that authorization is required on a link, you have to use either ``ExtendedAuthorizeAttribute``. This attribute inherits from ``System.Web.Http.AuthorizeAttribute``. In addition, it only has a method which provide the authorization value of the endpoint. See for yourself in [ExtendedAuthorizeAttribute.cs](LoveOAS/Attributes/ExtendedAuthorizeAttribute.cs). 

Authorization for ASP.NET Core is not quite handled by the plugin yet since authorization has been rewritten by the ASP.NET Core team.

### Sample Projects
You can sneak into the code to discover further usage of the attributes presented above. Clone this project and open the solution file with Visual Studio 2015+. You can also just create your own and install the nuget package ``Dynamite.LoveOAS`` and ``Dynamite.LoveOAS.NetCore``. After that start your solution see for yourself the api response. Feel free to experiment and tweak with the attributes and interfaces.

## Implementation Details
Knowing how the plugin works in the back might a good idea if you plan to contribute or need to fit it to your usage. All you need to know about is the interfaces which have been defined for it and how they connect to each others. Basically, they play all together to construct a graph of interconnected endpoints so that building links for each request results in looking up the graph and performing some serialization. 7 interfaces intervene sequentially in the graph construction:
1. [IExtractor](LoveOAS/Interfaces/IExtractor.cs) defines methods to extract specific attributes from controller actions. Reflection is expected to be used.
2. [IDiscoverer](LoveOAS/Interfaces/IDiscoverer.cs) is used to convert controller actions to interconnected [endpoints](LoveOAS/Discovery/Endpoint.cs) in order to build a network.
3. [IParser](LoveOAS/Interfaces/IParser.cs) converts endpoints to [nodes](LoveOAS/Discovery/Node.cs) ready to be serialized as a matter of links.
   * [IAuthorization](LoveOAS/Interfaces/IAuthorization.cs) is used by IParser to discover whether a link should be published given the authorization settings.
   * [IRouteSelector](LoveOAS/Interfaces/IRouteSelector.cs) is also used by IParser to uniquely select a route on endpoints with multiple routes.
6. [ISerializer](LoveOAS/Interfaces/ISerializer.cs) serializes nodes into an object with payload and links. For instance the default implmentation [JsonOutputSerializer](LoveOAS/Discovery/JsonOutputSerializer.cs) serializes to JSON.
7. [IOrchestrator](LoveOAS/Interfaces/IOrchestrator.cs) uses all the other interfaces to make the process work from reflection to serialization. 

## Code Documentation

## Future Work

## License

Licensed under MIT.
