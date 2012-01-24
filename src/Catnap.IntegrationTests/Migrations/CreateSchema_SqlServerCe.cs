using Catnap.Migration;

namespace Catnap.IntegrationTests.Migrations
{
    public class CreateSchema_SqlServerCe : BaseMigration
    {
		public CreateSchema_SqlServerCe()
			: base(
            @"create table Forum (Id int primary key identity(1,1) not null, Name nvarchar(200), TimeOfDayLastUpdated bigint)",
			@"create table Post (Id int primary key identity(1,1) not null, Title nvarchar(200), Body nvarchar(200), DatePosted datetime, PosterId int, ForumId int not null)",
			@"create table Person (Id int primary key identity(1,1) not null, FirstName nvarchar(200), LastName nvarchar(200), Active int, MemberSince int)",
			@"create table ForumGuid (Id uniqueidentifier primary key not null, Name nvarchar(200), TimeOfDayLastUpdated bigint)",
            @"create table PostGuid (Id uniqueidentifier primary key not null, Title nvarchar(200), Body nvarchar(200), DatePosted datetime, PosterId uniqueidentifier, ForumId uniqueidentifier not null)",
            @"create table PersonGuid (Id uniqueidentifier primary key not null, FirstName nvarchar(200), LastName nvarchar(200), Active int, MemberSince int)"
        ) { }
    }
}