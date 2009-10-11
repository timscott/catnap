using System.Collections.Generic;
using System.Linq;
using Catnap.IntegrationTests.Models;
using Catnap.Maps;
using Machine.Specifications;
using ShouldIt.Clr.Fluent;

namespace Catnap.IntegrationTests
{
    public class behaves_like_integration_test
    {
        Establish context = () =>
        {
            SessionFactory.Initialize(":memory:");
            DomainMap.Configure
                (
                new EntityMap<Person>()
                    .Property(x => x.Id)
                    .Property(x => x.FirstName)
                    .Property(x => x.LastName),
                new EntityMap<Forum>()
                    .Property(x => x.Id)
                    .List(x => x.Posts)
                    .Property(x => x.Name),
                new EntityMap<Post>("ForumId")
                    .Property(x => x.Id)
                    .Property(x => x.Title)
                    .Property(x => x.Body)
                    .BelongsTo(x => x.Poster, "PosterId")
                );
            UnitOfWork.Start();
            DatabaseMigrator.Execute();
        };

        //TODO: Why is this running beofer Establish?
        //Cleanup after_each = UnitOfWork.Current.Dispose;
    }

    public abstract class behaves_like_person_test : behaves_like_integration_test
    {
        protected static Person person;
        protected static Person actualPerson;

        protected static void save_person()
        {
            person = new Person { FirstName = "Joe", LastName = "Smith" };
            UnitOfWork.Current.Session.SaveOrUpdate(person);
        }

        protected static void get_person()
        {
            actualPerson = UnitOfWork.Current.Session.Get<Person>(person.Id);
        }
    }

    public class when_getting_person : behaves_like_person_test
    {
        Establish context = save_person;
        
        Because of = get_person;
        
        It should_be_the_person = () => actualPerson.Should().Equal(person);
    }

    public abstract class behaves_like_post_test : behaves_like_person_test
    {
        protected static Forum forum;
        protected static Post post;
        protected static Post actualPost;
        protected static Forum actualForum;

        protected static void save_forum()
        {
            save_person();
            post = new Post
            {
                Title = "It Doesn't Work",
                Body = "Someone help, it doesnt work",
                Poster = person
            };
            forum = new Forum { Name = "Annoying Complaints", Posts = new List<Post> { post } };
            UnitOfWork.Current.Session.SaveOrUpdate(forum);
        }

        protected static void get_forum()
        {
            actualForum = UnitOfWork.Current.Session.Get<Forum>(forum.Id);
        }
    }

    public class when_getting_forum : behaves_like_post_test
    {
        Establish context = save_forum;
       
        Because of = get_forum;
        
        It should_be_the_forum = () => actualForum.Should().Equal(forum);
        
        It forum_should_contain_the_post = () => actualForum.Posts.ToList()[0].Should().Equal(post);
        
        It poster_should_be_the_person = () => actualForum.Posts.ToList()[0].Poster.Should().Equal(person);
    }
}