namespace Sefer.Backend.Api.Data.Handlers.Test.Mentors;

[TestClass]
public class IsStudentOfMentorHandlerTest : SubmissionUnitTest
{
    [TestInitialize]
    public override async Task Initialize()
    {
        var context = GetDataContext();
        await InitializeUsers(context);
    }

    [TestMethod]
    public async Task Handle_NoStudentOfMentor()
    {
        var context = GetDataContext();
        var mentor = await context.GetMentor();
        var student = await context.GetStudent();
        var isStudent = await Handle(student.Id, mentor.Id);
        isStudent.Should().BeFalse();
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        await InitializeSubmission(context);
        var mentor = await context.GetMentor();
        var student = await context.GetStudent();
        var isStudent = await Handle(student.Id, mentor.Id);
        isStudent.Should().BeTrue();
    }

    private async Task<bool> Handle(int studentId, int mentorId)
    {
        var request = new IsStudentOfMentorRequest(mentorId, studentId);
        var handler = new IsStudentOfMentorHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}