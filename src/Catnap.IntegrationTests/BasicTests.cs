using System;
using System.Collections.Generic;
using System.Linq;
using Catnap.IntegrationTests.Migrations;
using Catnap.IntegrationTests.Repositories;
using Catnap.Logging;
using Catnap.Tests.Core;
using Catnap.Tests.Core.Models;
using Machine.Specifications;
using Should.Fluent;

namespace Catnap.IntegrationTests
{
    public abstract class behaves_like_integration_test
    {
        Cleanup after_each = () => UnitOfWork.Current.Dispose();

        Establish context = () =>
        {
            Log.Level = LogLevel.Debug;
            Fluently.Configure
                .ConnectionString("Data source=:memory:")
                .DatabaseAdapter(DbAdapter.Sqlite)
                .Domain(DomainMapping.Get())
                .Done();
            UnitOfWork.Start(); //NOTE: Normally unit-of work-would be more fine grained; however the in-memory database is created blank with each connection
            DatabaseMigrator.Execute();
        };
    }

    public abstract class behaves_like_person_test_ints : behaves_like_integration_test
    {
        protected static Person person;
        protected static Person actualPerson;

        protected static void save_person()
        {
            person = new Person { FirstName = "Joe", LastName = "Smith" };
            Container.PersonRepository.Save(person);
        }

        Establish context = save_person;
    }

    public abstract class behaves_like_person_test_guids : behaves_like_integration_test
    {
        protected static PersonGuid person;
        protected static PersonGuid actualPerson;

        protected static void save_person()
        {
            person = new PersonGuid { FirstName = "Joe", LastName = "Smith" };
            ContainerGuid.PersonRepository.Save(person);
        }

        Establish context = save_person;
    }

    public class when_getting_person_ints : behaves_like_person_test_ints
    {
        Because of = () => actualPerson = Container.PersonRepository.Get(person.Id);
        It should_be_the_person = () => actualPerson.Should().Equal(person);
    }

    public class when_getting_person_guids : behaves_like_person_test_guids
    {
        Because of = () => actualPerson = ContainerGuid.PersonRepository.Get(person.Id);
        It should_be_the_person = () => actualPerson.Should().Equal(person);
    }

    public class when_updating_person_ints : behaves_like_person_test_ints
    {
        Because of = () =>
        {
            actualPerson = Container.PersonRepository.Get(person.Id);
            actualPerson.LastName = "Newlastname";
            Container.PersonRepository.Save(actualPerson);
            actualPerson = Container.PersonRepository.Get(person.Id);
        };
        It should_have_updates = () => actualPerson.LastName.Should().Equal("Newlastname");
    }

    public class when_updating_person_guids : behaves_like_person_test_guids
    {
        Because of = () => 
        {
            actualPerson = ContainerGuid.PersonRepository.Get(person.Id);
            actualPerson.LastName = "Newlastname";
            ContainerGuid.PersonRepository.Save(actualPerson);
            actualPerson = ContainerGuid.PersonRepository.Get(person.Id);
        };
        It should_have_updates = () => actualPerson.LastName.Should().Equal("Newlastname");
    }

    public class when_deleting_person_ints: behaves_like_person_test_ints
    {
        Because of = () => Container.PersonRepository.Delete(person.Id);
        It person_should_be_deleted = () =>
        {
            actualPerson = Container.PersonRepository.Get(person.Id);
            actualPerson.Should().Be.Null();
        };
    }

    public class when_deleting_person_guids : behaves_like_person_test_guids
    {
        Because of = () => ContainerGuid.PersonRepository.Delete(person.Id);
        It person_should_be_deleted = () =>
        {
            actualPerson = ContainerGuid.PersonRepository.Get(person.Id);
            actualPerson.Should().Be.Null();
        };
    }

    public class when_updating_a_person_ints : behaves_like_person_test_ints
    {
        Establish context =()=> person.FirstName = "NewFirstName";
        Because of = () => Container.PersonRepository.Save(person);
        It person_should_be_updated = () =>
        {
            actualPerson = Container.PersonRepository.Get(person.Id);
            actualPerson.FirstName.Should().Equal(person.FirstName);
        };
    }

    public class when_updating_a_person_guids : behaves_like_person_test_guids
    {
        Establish context = () => person.FirstName = "NewFirstName";
        Because of = () => ContainerGuid.PersonRepository.Save(person);
        It person_should_be_updated = () =>
        {
            actualPerson = ContainerGuid.PersonRepository.Get(person.Id);
            actualPerson.FirstName.Should().Equal(person.FirstName);
        };
    }

    public class when_getting_person_by_first_name_ints : behaves_like_person_test_ints
    {
        Because of = () => actualPerson = Container.PersonRepository.FindByFirstName(person.FirstName).FirstOrDefault();
        It should_be_the_person = () => actualPerson.Should().Equal(person);
    }

    public class when_getting_person_by_first_name_guids : behaves_like_person_test_guids
    {
        Because of = () => actualPerson = ContainerGuid.PersonRepository.FindByFirstName(person.FirstName).FirstOrDefault();
        It should_be_the_person = () => actualPerson.Should().Equal(person);
    }

    public abstract class behaves_like_post_test_ints : behaves_like_person_test_ints
    {
        protected static Forum forum;
        protected static Post post;
        protected static Post actualPost;
        protected static Forum actualForum;

        Establish context = () =>
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
                TimeOfDayLastUpdated = new TimeSpan(10, 9, 8, 7, 6),
                Posts = new List<Post> {post}
            };
            Container.ForumRepository.Save(forum);
        };

        protected static void get_forum()
        {
            actualForum = Container.ForumRepository.Get(forum.Id);
        }
    }

    public abstract class behaves_like_post_test_guids : behaves_like_person_test_guids
    {
        protected static ForumGuid forum;
        protected static PostGuid post;
        protected static PostGuid actualPost;
        protected static ForumGuid actualForum;

        private Establish context = () =>
        {
            save_person();
            post = new PostGuid
            {
                Title = "It Doesn't Work",
                Body = "Someone help, it doesnt work",
                Poster = person,
                DatePosted = new DateTime(2000, 1, 1)
            };
            forum = new ForumGuid
            {
                Name = "Annoying Complaints",
                TimeOfDayLastUpdated = new TimeSpan(10, 9, 8, 7, 6),
                Posts = new List<PostGuid> {post}
            };
            ContainerGuid.ForumRepository.Save(forum);
        };

        protected static void get_forum()
        {
            actualForum = ContainerGuid.ForumRepository.Get(forum.Id);
        }
    }

    public class when_getting_forum_ints : behaves_like_post_test_ints
    {
        Because of = get_forum;
        It should_be_the_forum = () => actualForum.Should().Equal(forum);
        It forum_should_contain_the_post = () => actualForum.Posts.First().Should().Equal(post);
        It poster_should_be_the_person = () => actualForum.Posts.First().Poster.Should().Equal(person);
        It forum_last_updated_should_be_same = () => actualForum.TimeOfDayLastUpdated.Should().Equal(forum.TimeOfDayLastUpdated);
    }

    public class when_getting_forum_guids : behaves_like_post_test_guids
    {
        Because of = get_forum;
        It should_be_the_forum = () => actualForum.Should().Equal(forum);
        It forum_should_contain_the_post = () => actualForum.Posts.First().Should().Equal(post);
        It poster_should_be_the_person = () => actualForum.Posts.First().Poster.Should().Equal(person);
        It forum_last_updated_should_be_same = () => actualForum.TimeOfDayLastUpdated.Should().Equal(forum.TimeOfDayLastUpdated);
    }

    public class when_getting_persons_who_have_posted_ints : behaves_like_post_test_ints
    {
        static IList<Person> personsWhoHavePosted;

        Establish context = () =>
        {
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

    public class when_getting_persons_who_have_posted_guids : behaves_like_post_test_guids
    {
        static IList<PersonGuid> personsWhoHavePosted;

        Establish context = () =>
        {
            var notPosted = new PersonGuid { FirstName = "HasNot", LastName = "Posted" };
            ContainerGuid.PersonRepository.Save(notPosted);
        };
        Because of = () => personsWhoHavePosted = ContainerGuid.PersonRepository.GetPesonsWhoHavePosted().ToList();
        It should_return_only_person_who_have_posted = () =>
        {
            personsWhoHavePosted.Should().Count.Exactly(1);
            personsWhoHavePosted.Should().Contain.One(x => x.Equals(person));
        };
    }

    public class when_getting_post_count_for_a_person_ints : behaves_like_post_test_ints
    {
        static long postCount;
        private const long expected = 1;

        Because of = () => postCount = Container.PersonRepository.GetTotalPostCount(person.Id);
        It should_return_post_count = () => postCount.Should().Equal(expected);
    }

    public class when_getting_post_count_for_a_person_guids : behaves_like_post_test_guids
    {
        static long postCount;
        private const long expected = 1;

        Because of = () => postCount = ContainerGuid.PersonRepository.GetTotalPostCount(person.Id);
        It should_return_post_count = () => postCount.Should().Equal(expected);
    }
}