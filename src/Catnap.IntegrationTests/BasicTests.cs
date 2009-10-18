using System;
using System.Collections.Generic;
using System.Linq;
using Catnap.Common.Logging;
using Catnap.IntegrationTests.Migrations;
using Catnap.IntegrationTests.Models;
using Catnap.IntegrationTests.Repositories;
using Catnap.Maps;
using Machine.Specifications;
using ShouldIt.Clr.Fluent;

namespace Catnap.IntegrationTests
{
    public abstract class behaves_like_integration_test
    {
        Establish context = initialize_app;

        static void initialize_app()
        {
            Log.Level = LogLevel.Debug;
            SessionFactory.Initialize(":memory:");
            Domain.Configure
            (
                Map.Entity<Person>()
                    .Property(x => x.Id)
                    .Property(x => x.FirstName)
                    .Property(x => x.LastName),
                Map.Entity<Forum>()
                    .Property(x => x.Id)
                    .List(x => x.Posts)
                    .Property(x => x.Name)
                    .Property(x => x.TimeOfDayLastUpdated),
                Map.Entity<Post>()
                    .ParentColumn("ForumId")
                    .Property(x => x.Id)
                    .Property(x => x.Title)
                    .Property(x => x.Body)
                    .Property(x => x.DatePosted)
                    .BelongsTo(x => x.Poster, "PosterId")
            );
            UnitOfWork.Start(); //NOTE: Normally unit-of work-would be more fine grained; however the in-memory database is created blank with each connection
            DatabaseMigrator.Execute();
        }

        Cleanup after_each = () => UnitOfWork.Current.Dispose();
    }

    public abstract class behaves_like_person_test : behaves_like_integration_test
    {
        protected static Person person;
        protected static Person actualPerson;

        protected static void save_person()
        {
            person = new Person { FirstName = "Joe", LastName = "Smith" };
            Container.PersonRepository.Save(person);
        }

        protected static void get_person()
        {
            actualPerson = Container.PersonRepository.Get(person.Id);
        }
    }

    public class when_getting_person : behaves_like_person_test
    {
        Establish context = save_person;
        
        Because of = get_person;
        
        It should_be_the_person = () => actualPerson.Should().Equal(person);
    }

    public class when_deleting_person : behaves_like_person_test
    {
        Establish context = save_person;

        Because of = () => Container.PersonRepository.Delete(person.Id);

        It person_should_be_deleted = () =>
        {
            actualPerson = Container.PersonRepository.Get(person.Id);
            actualPerson.Should().Be.Null();
        };
    }

    public class when_getting_person_by_first_name : behaves_like_person_test
    {
        Establish context = save_person;

        Because of = () => actualPerson = Container.PersonRepository.FindByFirstName(person.FirstName).FirstOrDefault();

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
                Poster = person,
                DatePosted = new DateTime(2000, 1, 1)
            };
            forum = new Forum
            {
                Name = "Annoying Complaints",
                TimeOfDayLastUpdated = new TimeSpan(10, 0, 0),
                Posts = new List<Post> { post }
            };
            Container.ForumRepository.Save(forum);
        }

        protected static void get_forum()
        {
            actualForum = Container.ForumRepository.Get(forum.Id);
        }
    }

    public class when_getting_forum : behaves_like_post_test
    {
        Establish context = save_forum;
       
        Because of = get_forum;
        
        It should_be_the_forum = () => actualForum.Should().Equal(forum);
        
        It forum_should_contain_the_post = () => actualForum.Posts.ToList()[0].Should().Equal(post);
        
        It poster_should_be_the_person = () => actualForum.Posts.ToList()[0].Poster.Should().Equal(person);

        It forum_last_updated_should_be_same = () => actualForum.TimeOfDayLastUpdated.Should().Equal(forum.TimeOfDayLastUpdated);
    }

    public class when_getting_persons_who_have_posted : behaves_like_post_test
    {
        static IList<Person> personsWhoHavePosted;

        Establish context = () =>
        {
            save_forum();
            var notPosted = new Person { FirstName = "HasNot", LastName = "Posted" };
            Container.PersonRepository.Save(notPosted);
        };

        Because of = () => personsWhoHavePosted = Container.PersonRepository.GetPesonsWhoHavePosted().ToList();

        It should_return_only_person_who_have_posted = () =>
        {
            personsWhoHavePosted.Should().Count.Exactly(1);
            personsWhoHavePosted.Should().Contain.One(x => x.Equals(person));
        };
    }

    public class when_getting_post_count_for_a_person : behaves_like_post_test
    {
        static int postCount;

        Establish context = save_forum;

        Because of = () => postCount = Container.PersonRepository.GetTotalPostCount(person.Id);

        It should_return_post_count = () => postCount.Should().Equal(1);
    }

}