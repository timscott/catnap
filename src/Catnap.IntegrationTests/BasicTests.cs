using System;
using System.Collections.Generic;
using System.Linq;
using Catnap.Tests.Core.Models;
using Machine.Specifications;
using Should.Fluent;

namespace Catnap.IntegrationTests
{
    public abstract class behaves_like_integration_test
    {
        protected static ISessionFactory sessionFactory;

        Establish context = () =>
        {
            sessionFactory = Bootstrapper.BootstrapSqlite();
            //sessionFactory = Bootstrapper.BootstrapSqlServerCe();
        };

        protected static void InSession(Action<ISession> action)
        {
            using (var s = sessionFactory.Create())
            {
                s.Open();
                action(s);
            }
        }

        protected static T InSession<T>(Func<ISession, T> func)
        {
            using (var s = sessionFactory.Create())
            {
                s.Open();
                return func(s);
            }
        }
    }

    public abstract class behaves_like_person_test_ints : behaves_like_integration_test
    {
        protected static Person person;
        protected static Person actualPerson;

        protected static void save_person()
        {
            person = new Person { FirstName = "Joe", LastName = "Smith" };
            save_person(person);
        }

        Establish context = save_person;

        protected static void save_person(Person person)
        {
            InSession(s => s.SaveOrUpdate(person));
        }

        protected static Person get_person(int id)
        {
            return InSession(s => s.Get<Person>(id));
        }
    }

    public abstract class behaves_like_person_test_guids : behaves_like_integration_test
    {
        protected static PersonGuid person;
        protected static PersonGuid actualPerson;

        Establish context = save_person;

        protected static void save_person()
        {
            person = new PersonGuid { FirstName = "Joe", LastName = "Smith" };
            save_person(person);
        }

        protected static void save_person(PersonGuid person)
        {
            InSession(s => s.SaveOrUpdate(person));
        }

        protected static PersonGuid get_person(Guid id)
        {
            return InSession(s => s.Get<PersonGuid>(id));
        }
    }

    public class when_getting_person_ints : behaves_like_person_test_ints
    {
        Because of = () => actualPerson = get_person(person.Id);

        It should_be_the_person = () => actualPerson.Should().Equal(person);
    }

    public class when_getting_person_guids : behaves_like_person_test_guids
    {
        Because of = () => actualPerson = get_person(person.Id);

        It should_be_the_person = () => actualPerson.Should().Equal(person);
    }

    public class when_updating_person_ints : behaves_like_person_test_ints
    {
        Because of = () =>
        {
            actualPerson = get_person(person.Id);
            actualPerson.LastName = "Newlastname";
            save_person(actualPerson);
            actualPerson = get_person(person.Id);
        };
        It should_have_updates = () => actualPerson.LastName.Should().Equal("Newlastname");
    }

    public class when_updating_person_guids : behaves_like_person_test_guids
    {
        Because of = () => 
        {
            actualPerson = get_person(person.Id);
            actualPerson.LastName = "Newlastname";
            save_person(actualPerson);
            actualPerson = get_person(person.Id);
        };
        It should_have_updates = () => actualPerson.LastName.Should().Equal("Newlastname");
    }

    public class when_deleting_person_ints: behaves_like_person_test_ints
    {
        Because of = () => InSession(s => s.Delete<Person>(person.Id));
        It person_should_be_deleted = () =>
        {
            actualPerson = get_person(person.Id);
            actualPerson.Should().Be.Null();
        };
    }

    public class when_deleting_person_guids : behaves_like_person_test_guids
    {
        Because of = () => InSession(s => s.Delete<PersonGuid>(person.Id));
        It person_should_be_deleted = () =>
        {
            actualPerson = get_person(person.Id);
            actualPerson.Should().Be.Null();
        };
    }

    public class when_updating_a_person_ints : behaves_like_person_test_ints
    {
        Establish context =()=> person.FirstName = "NewFirstName";
        Because of = () => save_person(person);
        It person_should_be_updated = () =>
        {
            actualPerson = get_person(person.Id);
            actualPerson.FirstName.Should().Equal(person.FirstName);
        };
    }

    public class when_updating_a_person_guids : behaves_like_person_test_guids
    {
        Establish context = () => person.FirstName = "NewFirstName";
        Because of = () => save_person(person);
        It person_should_be_updated = () =>
        {
            actualPerson = get_person(person.Id);
            actualPerson.FirstName.Should().Equal(person.FirstName);
        };
    }

    public class when_getting_person_by_first_name_ints : behaves_like_person_test_ints
    {
        Because of = () =>
        {
            var criteria = Criteria.For<Person>().Equal(x => x.FirstName, person.FirstName);
            actualPerson = InSession(s => s.List(criteria).FirstOrDefault());
        };
        It should_be_the_person = () => actualPerson.Should().Equal(person);
    }

    public class when_getting_person_by_first_name_guids : behaves_like_person_test_guids
    {
        Because of = () =>
        {
            var criteria = Criteria.For<PersonGuid>()
                .Equal(x => x.FirstName, person.FirstName);
            actualPerson = InSession(s => s.List(criteria).FirstOrDefault());
        };
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
            InSession(s => s.SaveOrUpdate(forum));
        };

        protected static void get_forum(ISession session)
        {
            actualForum = session.Get<Forum>(forum.Id);
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
            InSession(s => s.SaveOrUpdate(forum));
        };

        protected static void get_forum(ISession session)
        {
            actualForum = session.Get<ForumGuid>(forum.Id);
        }
    }

    public class when_getting_forum_ints : behaves_like_post_test_ints
    {
        static ISession session;

        Establish context = () =>
        {
            session = sessionFactory.Create();
            session.Open();
        };
        
        Because of = () => get_forum(session);
        
        It should_be_the_forum = () => actualForum.Should().Equal(forum);
        It forum_should_contain_the_post = () => actualForum.Posts.First().Should().Equal(post);
        It poster_should_be_the_person = () => actualForum.Posts.First().Poster.Should().Equal(person);
        It forum_last_updated_should_be_same = () => actualForum.TimeOfDayLastUpdated.Should().Equal(forum.TimeOfDayLastUpdated);
        
        Cleanup after_all = () => session.Dispose();
    }

    public class when_getting_forum_guids : behaves_like_post_test_guids
    {
        static ISession session;

        Establish context = () =>
        {
            session = sessionFactory.Create();
            session.Open();
        };

        Because of = () => get_forum(session);
        It should_be_the_forum = () => actualForum.Should().Equal(forum);
        It forum_should_contain_the_post = () => actualForum.Posts.First().Should().Equal(post);
        It poster_should_be_the_person = () => actualForum.Posts.First().Poster.Should().Equal(person);
        It forum_last_updated_should_be_same = () => actualForum.TimeOfDayLastUpdated.Should().Equal(forum.TimeOfDayLastUpdated);

        Cleanup after_all = () => session.Dispose();
    }

    public class when_getting_persons_who_have_posted_ints : behaves_like_post_test_ints
    {
        static IEnumerable<Person> personsWhoHavePosted;

        Establish context = () =>
        {
            var notPosted = new Person { FirstName = "HasNot", LastName = "Posted" };
            save_person(notPosted);
        };

        Because of = () =>
        {
            var command = new DbCommandSpec()
                .SetCommandText("select p.* from Person p inner join Post on Post.PosterId = p.Id");
            personsWhoHavePosted = InSession(s => s.List<Person>(command));
        };
        It should_return_only_person_who_have_posted = () =>
        {
            personsWhoHavePosted.Should().Count.Exactly(1);
            personsWhoHavePosted.Should().Contain.One(x => x.Equals(person));
        };
    }

    public class when_getting_persons_who_have_posted_guids : behaves_like_post_test_guids
    {
        static IEnumerable<PersonGuid> personsWhoHavePosted;

        Establish context = () =>
        {
            var notPosted = new PersonGuid { FirstName = "HasNot", LastName = "Posted" };
            save_person(notPosted);
        };
        Because of = () =>
        {
            var command = new DbCommandSpec()
                .SetCommandText("select p.* from PersonGuid p inner join PostGuid on PostGuid.PosterId = p.Id");
            personsWhoHavePosted = InSession(s => s.List<PersonGuid>(command));
        };
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

        Because of = () =>
        {
            var commandSpec = new DbCommandSpec()
                .SetCommandText("select count(*) from Post p where p.PosterId = @personId")
                .AddParameter("@personId", person.Id);
            postCount = InSession(s => Convert.ToInt64(s.ExecuteScalar(commandSpec)));
        };
        It should_return_post_count = () => postCount.Should().Equal(expected);
    }

    public class when_getting_post_count_for_a_person_guids : behaves_like_post_test_guids
    {
        static long postCount;
        private const long expected = 1;

        Because of = () =>
        {
            var command = new DbCommandSpec()
                .SetCommandText("select count(*) from PostGuid p where p.PosterId = @personId")
                .AddParameter("@personId", person.Id);
            postCount = InSession(s => Convert.ToInt64(s.ExecuteScalar(command)));
        };
        It should_return_post_count = () => postCount.Should().Equal(expected);
    }
    
    public class when_getting_person_with_null_first_name : behaves_like_integration_test
    {
        static Person person;
        static Person actualPerson;

        Establish context = () =>
        {
            person = new Person { LastName = "Smith" };
            InSession(s => s.SaveOrUpdate(person));
        };

        Because of = () =>
        {
            var criteria = Criteria.For<Person>().Null(x => x.FirstName);
            actualPerson = InSession(s => s.List(criteria).FirstOrDefault());
        };
        It should_be_the_person = () => actualPerson.Should().Equal(person);
    }

    public class when_getting_person_with_not_null_first_name : behaves_like_person_test_ints
    {
        Because of = () =>
        {
            var criteria = Criteria.For<Person>().NotNull("FirstName");
            actualPerson = InSession(s => s.List(criteria).FirstOrDefault());
        };
        It should_be_the_person = () => actualPerson.Should().Equal(person);
    }

    public class when_getting_person_with_first_name_in : behaves_like_person_test_ints
    {
        Because of = () =>
        {
            var criteria = Criteria.For<Person>().In(x => x.FirstName, person.FirstName);
            actualPerson = InSession(s => s.List(criteria).FirstOrDefault());
        };
        It should_be_the_person = () => actualPerson.Should().Equal(person);
    }

    public class when_getting_person_with_first_name_not_in : behaves_like_person_test_ints
    {
        Because of = () =>
        {
            var criteria = Criteria.For<Person>().In("FirstName", "NotAnyonesFirstName");
            actualPerson = InSession(s => s.List(criteria).FirstOrDefault());
        };
        It should_be_the_person = () => actualPerson.Should().Be.Null();
    }

    public class when_getting_an_entity_saved_in_the_same_session : behaves_like_integration_test
    {
        static ISession session;
        static Person person;
        static Person actualPerson;

        Establish context = () =>
        {
            person = new Person {FirstName = "Tim", LastName = "Scott"};
            session = sessionFactory.Create();
            session.Open();
            session.SaveOrUpdate(person);
        };

        Because of = () => actualPerson = session.Get<Person>(person.Id);

        It should_be_the_same_instance = () => actualPerson.Should().Be.SameAs(person);

        Cleanup after_all = () => session.Dispose();
    }

    public class when_getting_an_entity_saved_in_the_a_different_session : behaves_like_integration_test
    {
        static Person person;
        static Person actualPerson;

        Establish context = () =>
        {
            person = new Person { FirstName = "Tim", LastName = "Scott" };
            InSession(s => s.SaveOrUpdate(person));
        };

        Because of = () => actualPerson = InSession(s => s.Get<Person>(person.Id));

        It should_not_be_the_same_instance = () => actualPerson.Should().Not.Be.SameAs(person);
    }

    public class when_getting_an_entity_previously_fetched_in_the_same_session : behaves_like_person_test_ints
    {
        static IEnumerable<Person> actualPersons;

        Because of = () => InSession(s =>
        {
            actualPersons = s.List(Criteria.For<Person>().Equal(x => x.Id, person.Id));
            actualPerson = s.Get<Person>(person.Id);
        });

        It should_be_the_same_instance = () => actualPersons.First().Should().Be.SameAs(actualPerson);
    }

    public class when_getting_an_entity_previously_got_in_the_same_session : behaves_like_person_test_ints
    {
        static Person actualPerson2;

        Because of = () => InSession(s =>
        {
            actualPerson2 = s.Get<Person>(person.Id);
            actualPerson = s.Get<Person>(person.Id);
        });

        It should_be_the_same_instance = () => actualPerson2.Should().Be.SameAs(actualPerson);
    }

    public class when_getting_an_entity_saved_twice_in_the_same_session : behaves_like_person_test_ints
    {
        static Person actualPerson2;

        Because of = () => InSession(s =>
        {
            actualPerson = s.Get<Person>(person.Id);
            actualPerson.LastName = "NewLastName";
            s.SaveOrUpdate(actualPerson);
            actualPerson2 = s.Get<Person>(person.Id);
        });

        It should_get_lastest_updates = () => actualPerson2.LastName.Should().Equal("NewLastName");
    }
}