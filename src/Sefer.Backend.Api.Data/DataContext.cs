using VersionInfo = Sefer.Backend.Api.Data.Models.VersionInfo;

namespace Sefer.Backend.Api.Data;

// ReSharper disable UnusedAutoPropertyAccessor.Global
public class DataContext(DbContextOptions options) : DbContext(options)
{
    #region Database Sets Courses

    public DbSet<Course> Courses { get; set; }

    public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }

    public DbSet<CourseRevision> CourseRevisions { get; set; }

    public DbSet<Series> Series { get; set; }

    public DbSet<SeriesCourse> SeriesCourses { get; set; }

    public DbSet<CourseRating> CourseRatings { get; set; }

    #endregion

    #region Database Sets Courses.Curricula

    public DbSet<Curriculum> Curricula { get; set; }

    public DbSet<CurriculumBlock> CurriculumBlocks { get; set; }

    public DbSet<CurriculumBlockCourse> CurriculumBlockCourses { get; set; }

    public DbSet<CurriculumRevision> CurriculumRevisions { get; set; }

    #endregion

    #region Database Sets Courses.Lessons

    public DbSet<BoolQuestion> LessonBoolQuestions { get; set; }

    public DbSet<LessonTextElement> LessonTextElements { get; set; }

    public DbSet<MediaElement> LessonMediaElements { get; set; }

    public DbSet<Lesson> Lessons { get; set; }

    public DbSet<MultipleChoiceQuestion> LessonMultipleChoiceQuestions { get; set; }

    public DbSet<MultipleChoiceQuestionChoice> LessonMultipleChoiceQuestionChoices { get; set; }

    public DbSet<OpenQuestion> LessonOpenQuestions { get; set; }

    #endregion

    #region Database Sets Courses.Reward

    public DbSet<Reward> Rewards { get; set; }

    public DbSet<RewardEnrollment> RewardEnrollments { get; set; }

    public DbSet<RewardGrant> RewardGrants { get; set; }

    public DbSet<RewardTarget> RewardTargets { get; set; }

    #endregion

    #region Database Sets Courses.Surveys

    public DbSet<SurveyBoolQuestion> SurveyBoolQuestions { get; set; }

    public DbSet<Survey> Surveys { get; set; }

    public DbSet<SurveyMultipleChoiceQuestion> SurveyMultipleChoiceQuestions { get; set; }

    public DbSet<SurveyMultipleChoiceQuestionChoice> SurveyMultipleChoiceQuestionChoices { get; set; }

    public DbSet<SurveyOpenQuestion> SurveyOpenQuestions { get; set; }

    #endregion

    #region Database Sets Users

    public DbSet<User> Users { get; set; }

    public DbSet<MentorCourse> MentorCourses { get; set; }

    public DbSet<MentorRegion> MentorRegions { get; set; }

    public DbSet<MentorRating> MentorRatings { get; set; }

    public DbSet<MentorSettings> MentorSettings { get; set; }

    public DbSet<StudentSettings> StudentSettings { get; set; }

    public DbSet<AdminSettings> AdminSettings { get; set; }

    public DbSet<UserBackupKey> UserBackupKeys { get; set; }

    public DbSet<MentorStudentData> MentorStudentData { get; set; }

    public DbSet<PushNotificationToken> PushNotificationTokens { get; set; }
    
    public DbSet<UserSetting> UserSettings { get; set; }

    #endregion

    #region Databases Sets About Chatting between users

    public DbSet<Channel> ChatChannels { get; set; }

    public DbSet<ChannelMessage> ChatChannelMessages { get; set; }

    public DbSet<ChannelReceiver> ChatChannelReceivers { get; set; }

    public DbSet<Message> ChatMessages { get; set; }

    #endregion

    #region Database Sets Enrollments

    public DbSet<Enrollment> Enrollments { get; set; }

    public DbSet<LessonSubmission> LessonSubmissions { get; set; }

    public DbSet<QuestionAnswer> Answers { get; set; }

    public DbSet<SurveyResult> SurveyResults { get; set; }

    #endregion

    #region Database Sets Resources
    
    public DbSet<Testimony> Testimonies { get; set; }

    public DbSet<ContentPage> ContentPages { get; set; }

    public DbSet<SiteSpecificContentPage> SiteSpecificContentPages { get; set; }
    
    public DbSet<Blog> Blogs { get; set; }
    
    public DbSet<GeoIPInfo> IpAddressLookups { get; set; }
    
    public DbSet<VersionInfo> Versions { get; set; }

    public DbSet<ShortUrl> ShortUrls { get; set; }
    
    public DbSet<Template> Templates { get; set; }
    
    public DbSet<NotificationLocalization> NotificationLocalizations { get; set; }

    #endregion

    #region Database Sets Logging

    public DbSet<LoginLogEntry> LoginLogEntries { get; set; }

    public DbSet<ApiRequestLogEntry> ApiRequestLogEntries { get; set; }

    public DbSet<ClientPageRequestLogEntry> ClientPageRequestLogEntries { get; set; }

    public DbSet<Log> Logs { get; set; }

    #endregion

    #region Database Sets Settings

    public DbSet<Settings> Settings { get; set; }

    #endregion

    #region Views

    public DbSet<UserLastActivity> UserLastActivities { get; set; }

    public DbSet<MentorPerformance> MentorPerformance { get; set; }

    public DbSet<CourseStudentCount> CourseStudentCount { get; set; }

    public DbSet<EnrollmentSummary> EnrollmentSummaries { get; set; }

    #endregion

    #region Methods

    public string GetDatabaseVersion()
    {
        var version = Versions.OrderByDescending(v => v.Number).First();
        return $"{version.Number} - {version.Name}";
    }

    public string GetDatabaseConnection()
    {
        var info = Database.GetConnectionString()?.Split(';');
        if (info == null) return null;

        var connection = string.Empty;

        foreach (var property in info)
        {
            if (property.StartsWith("Host=")) connection += property + ";";
            if (property.StartsWith("Database=")) connection += property + ";";
            if (property.StartsWith("Port=")) connection += property + ";";
        }

        return connection;
    }

    public string GetDatabaseProvider() => Database.ProviderName;

    public void UpdateSingleProperty<T>(T entity, string property) where T : class, IEntity
    {
        Attach(entity);
        Entry(entity).Property(property).IsModified = true;
        SaveChanges();
    }

    public void Collection<TEntity, TCollection>(TEntity entity, Expression<Func<TEntity, IEnumerable<TCollection>>> collection)
        where TCollection : class, IEntity
        where TEntity : class, IEntity
    {
        Entry(entity).Collection(collection).Load();
    }

    public void Reference<TEntity, TReference>(TEntity entity, Expression<Func<TEntity, TReference>> reference)
        where TEntity : class, IEntity
        where TReference : class, IEntity
    {
        Entry(entity).Reference(reference).Load();
    }

    #endregion

    #region Fluent API Methods

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Please note: a database-first approach is used, so there is no need for a full setup here
        // only that what is required for ef core to be able to deal with database but no migrations

        // Deal with triggers in the database
        modelBuilder.Entity<User>().ToTable(tb => tb.HasTrigger("TG_InsertUser"));
        modelBuilder.Entity<LoginLogEntry>().ToTable(tb => tb.HasTrigger("TG_InsertLoginLogEntries"));
        modelBuilder.Entity<Message>().ToTable(tb => tb.HasTrigger("TG_InsertTextMessage"));
        modelBuilder.Entity<Enrollment>().ToTable(tb => tb.HasTrigger("TG_InsertEnrollments"));
        modelBuilder.Entity<LessonSubmission>().ToTable(tb => tb.HasTrigger("TG_InsertLessonSubmissions"));

        // Dealing with views
        modelBuilder.Entity<MentorPerformance>().HasNoKey().ToView("mentor_performance");
        modelBuilder.Entity<CourseStudentCount>().HasNoKey().ToView("course_student_count");
        modelBuilder.Entity<EnrollmentSummary>().HasNoKey().ToView("enrollment_summaries");
        modelBuilder.Entity<CourseReadingTime>().HasNoKey().ToView("course_reading_time");
        modelBuilder.Entity<ActiveStudentsPerMentor>().HasNoKey().ToView("active_students_per_mentor");
        modelBuilder.Entity<ContentPageOverride>().HasNoKey().ToView("content_page_overrides");

        // Special tables that don't have primary keys
        modelBuilder.Entity<VersionInfo>().HasNoKey();
        modelBuilder.Entity<UserLastActivity>().HasNoKey();
        modelBuilder.Entity<PushNotificationToken>().HasNoKey();
        
        // Let ef core now about the relations
        modelBuilder.Entity<User>().HasOne(u => u.MentorSettings).WithOne(s => s.Mentor);
        modelBuilder.Entity<Enrollment>().HasOne(s => s.CourseRevision).WithMany(c => c.Enrollments).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrollment>().HasOne(s => s.AccountabilityPartner).WithMany(c => c.PartnerEnrollments).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrollment>().HasOne(s => s.Student).WithMany(c => c.Enrollments).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrollment>().HasOne(s => s.Mentor).WithMany(c => c.Mentoring).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<MentorCourse>().HasOne(m => m.Course).WithMany(u => u.Mentors).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<MentorCourse>().HasOne(m => m.Mentor).WithMany(c => c.MentorCourses).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Course>().HasMany(c => c.CourseRevisions).WithOne(r => r.Course);
        modelBuilder.Entity<CoursePrerequisite>().HasOne(c => c.Course).WithMany(c => c.Prerequisites).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<CoursePrerequisite>().HasOne(c => c.RequiredCourse).WithMany(c => c.RequiredFor).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SeriesCourse>().HasOne(c => c.Course).WithMany(c => c.SeriesCourses).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SeriesCourse>().HasOne(c => c.Series).WithMany(c => c.SeriesCourses).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ChannelMessage>().HasOne(m => m.Receiver).WithMany(c => c.ReceivedChannelMessages).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Message>().HasOne(m => m.Sender).WithMany(c => c.SendMessages).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Blog>().HasOne(m => m.Author).WithMany(c => c.Blogs).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RewardGrant>().HasOne(m => m.User).WithMany(c => c.Grants).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RewardGrant>().HasOne(m => m.Target).WithMany(c => c.Grants).OnDelete(DeleteBehavior.Restrict);

        // Let ef core also know about the unique indices (for testing purposes)
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Survey>().HasIndex(s => s.CourseRevisionId).IsUnique();
    }

    #endregion
}