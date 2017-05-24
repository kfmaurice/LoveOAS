using System.Collections.Generic;
using System;

using Microsoft.AspNetCore.Mvc;

using Dynamite.LoveOAS.Attributes;
using Dynamite.LoveOAS.NetCore.Filters;

namespace TestWebApiCore.Controllers
{
  [LoveOASFilter] 
  [Route("api/[controller]")]
  public class PeopleController : Controller
  {
    public class Person
    {
      public int Id { get; set; }
      public string Firstname { get; set; }
      public string Lastname { get; set; }
    }

    public class Start
    {
      public string Language { get; set; }
      public string CustomProperty { get; set; }
    }


    // The current implementation doesn't support "Settings.CheckAuthorization" yet
    //[Entry("Auth")]
    //[Route("auth")]
    //public Person GetPerson()
    //{
    //  return new Person { Id = 0, Firstname = "Authorization", Lastname = "Passed" };
    //}

    [HttpGet("void")]
    public Person NotAffectedByPlugin() // change "Settings.HandleOnlyMarkedApis" in Startup.cs and see what happens
    {
      return new Person { Id = 0, Firstname = "Not", Lastname = "Affected" };
    }

    [Base]
    [Route("~/base")]
    public Start GetBase()
    {
      return new Start { Language = "en-us", CustomProperty = "mine" };
    }

    [Route("{id}")]
    [Entry("Read")]
    [Exit(nameof(Get), typeof(PeopleController), "Self", new Type[] { typeof(int) })]
    [Exit(nameof(Put), typeof(PeopleController), "Update")]
    [Exit(nameof(Delete), typeof(PeopleController), "Delete")]
    public Person Get(int id)
    {
      return new Person { Id = 0, Firstname = "Get", Lastname = "Person" };
    }

    [AcceptVerbs("Get", Route = "")]
    [Entry("All")]
    [Exit(nameof(Post), typeof(PeopleController), "Create")]
    [Exit(nameof(Get), typeof(PeopleController), "Read", new Type[] { typeof(int) })]
    [Exit(nameof(Put), typeof(PeopleController), "Update")]
    [Exit(nameof(Delete), typeof(PeopleController), "Delete")]
    [Exit(nameof(Search), typeof(PeopleController), "Search")]
    public IEnumerable<Person> Get()
    {
      // change "Settings.TreatCollectionAsPayload" in Startup.cs and see what happens
      return new List<Person>() { new Person { Id = 0, Firstname = "Collection", Lastname = "As Payload" } };
    }

    [HttpPost("search")]
    [Entry("Search")]
    [Exit(nameof(Post), typeof(PeopleController), "Create")]
    [Exit(nameof(Get), typeof(PeopleController), "Read", new Type[] { typeof(int) })]
    [Exit(nameof(Put), typeof(PeopleController), "Update")]
    [Exit(nameof(Delete), typeof(PeopleController), "Delete")]
    [Exit(nameof(Get), typeof(PeopleController), "All")]
    public IEnumerable<Person> Search(object Parameters)
    {
      // change "Settings.TreatCollectionAsPayload" in Startup.cs and see what happens
      return new List<Person>() { new Person { Id = 0, Firstname = "Collection", Lastname = "As Payload" } };
    }

    [Route("")]
    [Entry("Create")]
    [Exit(nameof(Get), typeof(PeopleController), "Self", new Type[] { typeof(int) })]
    [Exit(nameof(Put), typeof(PeopleController), "Update")]
    [Exit(nameof(Delete), typeof(PeopleController), "Delete")]
    public Person Post([FromBody]Person person)
    {
      return person;
    }

    [HttpPut("{id}")]
    [Exit(nameof(Get), typeof(PeopleController), "Self", new Type[] { typeof(int) })]
    [Exit(nameof(Put), typeof(PeopleController), "Update")]
    [Exit(nameof(Delete), typeof(PeopleController), "Delete")]
    public Person Put(int id, [FromBody]Person person)
    {
      return person;
    }

    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
  }
}
