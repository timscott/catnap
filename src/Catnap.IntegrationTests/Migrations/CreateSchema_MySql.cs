using Catnap.Migration;

namespace Catnap.IntegrationTests.Migrations
{
    public class CreateSchema_MySql : BaseMigration
    {
        public CreateSchema_MySql() : base(
    		@"CREATE TABLE `Forum` (`Id` INTEGER NOT NULL AUTO_INCREMENT,`Name` VARCHAR(200),`TimeOfDayLastUpdated` BIGINT, PRIMARY KEY (`Id`))",
			@"create table Post (Id INTEGER NOT NULL AUTO_INCREMENT, Title varchar(200), Body varchar(200), DatePosted datetime, PosterId int, ForumId int not null, PRIMARY KEY (`Id`))",
			@"create table Person (Id INTEGER NOT NULL AUTO_INCREMENT, FirstName varchar(200), LastName varchar(200), Active int, MemberSince int, PRIMARY KEY (`Id`))",
			@"create table ForumGuid (Id VARCHAR(200) NOT NULL, Name varchar(200), TimeOfDayLastUpdated bigint, PRIMARY KEY (`Id`))",
            @"create table PostGuid (Id VARCHAR(200) NOT NULL, Title varchar(200), Body varchar(200), DatePosted datetime, PosterId VARCHAR(200), ForumId VARCHAR(200), PRIMARY KEY (`Id`),  UNIQUE INDEX `ForumId`(`ForumId`),  UNIQUE INDEX `PosterId`(`PosterId`))",
            @"create table PersonGuid (Id VARCHAR(200) NOT NULL, FirstName varchar(200), LastName varchar(200), Active int, MemberSince int, PRIMARY KEY (`Id`))"
        ) { }
    }
}
