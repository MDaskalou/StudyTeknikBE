IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AuditLogs] (
    [Id] uniqueidentifier NOT NULL,
    [TimestampUtc] datetime2 NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [EventType] nvarchar(max) NOT NULL,
    [PayloadJson] nvarchar(max) NOT NULL,
    [CorrelationId] nvarchar(max) NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Classes] (
    [Id] uniqueidentifier NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    [TeacherId] uniqueidentifier NULL,
    [MentorId] uniqueidentifier NULL,
    [SchoolName] nvarchar(200) NOT NULL,
    [Year] int NOT NULL,
    [ClassName] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_Classes] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [dbo].[DiaryEntries] (
    [Id] uniqueidentifier NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    [StudentId] uniqueidentifier NOT NULL,
    [EntryDate] date NOT NULL,
    [Text] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_DiaryEntries] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Enrollments] (
    [Id] uniqueidentifier NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    [StudentId] uniqueidentifier NOT NULL,
    [ClassId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Enrollments] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [MentorAssigments] (
    [Id] uniqueidentifier NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    [MentorId] uniqueidentifier NOT NULL,
    [StudentId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_MentorAssigments] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [dbo].[Users] (
    [Id] uniqueidentifier NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [SecurityNumber] nvarchar(max) NOT NULL,
    [Email] nvarchar(320) NOT NULL,
    [Role] int NOT NULL,
    [ExternalProvider] nvarchar(max) NULL,
    [ExternalSubject] nvarchar(max) NULL,
    [ConsentGiven] bit NOT NULL,
    [ConsentGivenAtUtc] datetime2 NULL,
    [ConsentSetBy] nvarchar(256) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [WeeklySummaries] (
    [Id] uniqueidentifier NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    [StudentId] uniqueidentifier NOT NULL,
    [YearWeek] nvarchar(10) NOT NULL,
    [Text] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_WeeklySummaries] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Decks] (
    [Id] uniqueidentifier NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    [Title] nvarchar(100) NOT NULL,
    [CourseName] nvarchar(100) NOT NULL,
    [SubjectName] nvarchar(100) NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Decks] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Decks_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [StudentProfiles] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [PlanningHorizonWeeks] int NOT NULL,
    [WakeUpTime] time NOT NULL,
    [BedTime] time NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_StudentProfiles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [FlashCards] (
    [Id] uniqueidentifier NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    [FrontText] nvarchar(20) NOT NULL,
    [BackText] nvarchar(100) NOT NULL,
    [NextReviewAtUtc] datetime2 NOT NULL,
    [DeckId] uniqueidentifier NOT NULL,
    [Interval] int NOT NULL,
    [EaseFactor] real NOT NULL,
    CONSTRAINT [PK_FlashCards] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FlashCards_Decks_DeckId] FOREIGN KEY ([DeckId]) REFERENCES [Decks] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Courses] (
    [Id] uniqueidentifier NOT NULL,
    [StudentProfileId] uniqueidentifier NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [Difficulty] int NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_Courses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Courses_StudentProfiles_StudentProfileId] FOREIGN KEY ([StudentProfileId]) REFERENCES [StudentProfiles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [StudyGoals] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [CourseId] uniqueidentifier NOT NULL,
    [GoalDescription] nvarchar(500) NOT NULL,
    [IsCompleted] bit NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_StudyGoals] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudyGoals_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StudyGoals_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);
GO

CREATE TABLE [StudySessions] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [CourseId] uniqueidentifier NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [TaskDescription] nvarchar(500) NOT NULL,
    [WorkDurationMinutes] int NOT NULL,
    [WorkFeedback] nvarchar(50) NOT NULL,
    [BreakFeedback] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_StudySessions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudySessions_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]),
    CONSTRAINT [FK_StudySessions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);
GO

CREATE TABLE [StudyPlanTasks] (
    [Id] uniqueidentifier NOT NULL,
    [StudyGoalId] uniqueidentifier NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    [SuggestedDuration] int NOT NULL,
    [IsCompleted] bit NOT NULL,
    CONSTRAINT [PK_StudyPlanTasks] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudyPlanTasks_StudyGoals_StudyGoalId] FOREIGN KEY ([StudyGoalId]) REFERENCES [StudyGoals] ([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_Classes_SchoolName_Year_ClassName] ON [Classes] ([SchoolName], [Year], [ClassName]);
GO

CREATE INDEX [IX_Classes_TeacherId] ON [Classes] ([TeacherId]);
GO

CREATE INDEX [IX_Courses_StudentProfileId] ON [Courses] ([StudentProfileId]);
GO

CREATE INDEX [IX_Decks_UserId] ON [Decks] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_DiaryEntries_StudentId_EntryDate] ON [dbo].[DiaryEntries] ([StudentId], [EntryDate]);
GO

CREATE UNIQUE INDEX [IX_Enrollments_StudentId_ClassId] ON [Enrollments] ([StudentId], [ClassId]);
GO

CREATE INDEX [IX_FlashCards_DeckId] ON [FlashCards] ([DeckId]);
GO

CREATE INDEX [IX_FlashCards_NextReviewAtUtc] ON [FlashCards] ([NextReviewAtUtc]);
GO

CREATE UNIQUE INDEX [IX_MentorAssigments_MentorId_StudentId] ON [MentorAssigments] ([MentorId], [StudentId]);
GO

CREATE UNIQUE INDEX [IX_StudentProfiles_UserId] ON [StudentProfiles] ([UserId]);
GO

CREATE INDEX [IX_StudyGoals_CourseId] ON [StudyGoals] ([CourseId]);
GO

CREATE INDEX [IX_StudyGoals_UserId] ON [StudyGoals] ([UserId]);
GO

CREATE INDEX [IX_StudyPlanTasks_StudyGoalId] ON [StudyPlanTasks] ([StudyGoalId]);
GO

CREATE INDEX [IX_StudySessions_CourseId] ON [StudySessions] ([CourseId]);
GO

CREATE INDEX [IX_StudySessions_UserId] ON [StudySessions] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]);
GO

CREATE INDEX [IX_Users_Role] ON [dbo].[Users] ([Role]);
GO

CREATE UNIQUE INDEX [IX_WeeklySummaries_StudentId_YearWeek] ON [WeeklySummaries] ([StudentId], [YearWeek]);
GO

INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251208204017_InitialCreate', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [StudentProfiles] DROP CONSTRAINT [FK_StudentProfiles_Users_UserId];
GO

EXEC sp_rename N'[StudentProfiles].[UserId]', N'StudentId', N'COLUMN';
GO

EXEC sp_rename N'[StudentProfiles].[IX_StudentProfiles_UserId]', N'IX_StudentProfiles_StudentId', N'INDEX';
GO

ALTER TABLE [StudentProfiles] ADD CONSTRAINT [FK_StudentProfiles_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251210114923_RenameUserIdToStudentId', N'8.0.0');
GO

COMMIT;
GO

