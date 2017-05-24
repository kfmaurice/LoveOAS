using System.Collections.Generic;
using System.Web.Http;
using System;

using Dynamite.LoveOAS.Attributes;

namespace TestWebApi.Controllers
{
  [RoutePrefix("api/people")]
  public class PeopleController : ApiController
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

    [Entry("Auth")]
    [Route("auth")]
    [ExtendedAuthorize] // change "Settings.CheckAuthorization" in Global.asax.cs and see what happens when you call "api/people/base"
    public Person GetPerson()
    {
      return new Person { Id = 0, Firstname = "Authorization", Lastname = "Passed" };
    }

    [Route("void")]
    [HttpGet]
    public Person NotAffectedByPlugin() // change "Settings.HandleOnlyMarkedApis" in Global.asax.cs and see what happens
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

    [Route("")]
    [Entry("All")]
    [Exit(nameof(Post), typeof(PeopleController), "Create")]
    [Exit(nameof(Get), typeof(PeopleController), "Read", new Type[] { typeof(int) })]
    [Exit(nameof(Put), typeof(PeopleController), "Update")]
    [Exit(nameof(Delete), typeof(PeopleController), "Delete")]
    [Exit(nameof(Search), typeof(PeopleController), "Search")]
    public IEnumerable<Person> Get()
    {
      // change "Settings.TreatCollectionAsPayload" in Global.asax.cs and see what happens
      return new List<Person>() { new Person { Id = 0, Firstname = "Collection", Lastname = "As Payload" } };
    }

    [HttpPost]
    [Route("search")]
    [Entry("Search")]
    [Exit(nameof(Post), typeof(PeopleController), "Create")]
    [Exit(nameof(Get), typeof(PeopleController), "Read", new Type[] { typeof(int) })]
    [Exit(nameof(Put), typeof(PeopleController), "Update")]
    [Exit(nameof(Delete), typeof(PeopleController), "Delete")]
    [Exit(nameof(Get), typeof(PeopleController), "All")]
    public IEnumerable<Person> Search(object Parameters)
    {
      // change "Settings.TreatCollectionAsPayload" in Global.asax.cs and see what happens
      return new List<Person>() { new Person { Id = 0, Firstname = "Collection", Lastname = "As Payload" } };
    }

    [Route("")]
    [Entry("Create")]
    [Exit(nameof(Get), typeof(PeopleController), "Self", new Type[] { typeof(int) })]
    [Exit(nameof(Put), typeof(PeopleController), "Update")]
    [Exit(nameof(Delete), typeof(PeopleController), "Delete")]
    public Person Post(Person person)
    {
      return person;
    }

    [Route("{id}")]
    [Exit(nameof(Get), typeof(PeopleController), "Self", new Type[] { typeof(int) })]
    [Exit(nameof(Put), typeof(PeopleController), "Update")]
    [Exit(nameof(Delete), typeof(PeopleController), "Delete")]
    public Person Put(int id, Person person)
    {
      return person;
    }

    [Route("{id}")]
    public void Delete(int id)
    {
    }
  }
}
