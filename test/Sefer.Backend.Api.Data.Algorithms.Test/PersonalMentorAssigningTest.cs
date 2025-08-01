namespace Sefer.Backend.Api.Data.Algorithms.Test;

[TestClass]
public class PersonalMentorAssigningTest
{
    [TestMethod]
    public void Test_OneMentor()
    {
        // Arrange
        var builder = new PersonalMentorAssigningBuilder(Genders.Female, 2000);
        builder.AddMentor(1, 1900, 1, 10, 20, Genders.Female);
        var algo = builder.GetAssigner(true, 10, 0.5);
        
        // Assert
        ActAndAssert(algo, 1);
    }
    
    [TestMethod]
    public void Test_OneMentorNotSameGender()
    {
        // Arrange
        var builder = new PersonalMentorAssigningBuilder(Genders.Female, 2000);
        builder.AddMentor(1, 1900, 1, 10, 20, Genders.Male);
        var algo = builder.GetAssigner(true, 10, 0.5);
        
        // Assert
        ActAndAssert(algo, null);
    }
    
    [TestMethod]
    public void Test_PreferAvailability()
    {
        // Arrange
        var builder = new PersonalMentorAssigningBuilder(Genders.Female, 2000);
        builder.AddMentor(4, 1900, 2, 10, 20, Genders.Female);
        builder.AddMentor(3, 1901, 1, 10, 20, Genders.Female);
        var algo = builder.GetAssigner(false, 10, 0.25);
        
        // Assert - Mentor 3 ranks 1st on age, Mentor 4 ranks on 1st on availability, given the age-factor 3 should be selected 
        ActAndAssert(algo, 3);
    }
    
    [TestMethod]
    public void Test_PreferAge()
    {
        // Arrange
        var builder = new PersonalMentorAssigningBuilder(Genders.Female, 2000);
        builder.AddMentor(4, 1900, 2, 10, 20, Genders.Female);
        builder.AddMentor(3, 1901, 1, 10, 20, Genders.Female);
        var algo = builder.GetAssigner(false, 10, 0.75);
        
        // Assert - Mentor 3 ranks 1st on age, Mentor 4 ranks on 1st on availability, given the age-factor 4 should be selected 
        ActAndAssert(algo, 4);
    }
    
    [TestMethod]
    public void Test_StrictGender()
    {
        // Arrange
        var builder = new PersonalMentorAssigningBuilder(Genders.Female, 2000);
        builder.AddMentor(4, 1900, 2, 10, 20, Genders.Male);
        builder.AddMentor(3, 1901, 1, 10, 20, Genders.Female);
        var algo = builder.GetAssigner(true, 10, 0.75);
        
        // Assert - Mentor 3 ranks 1st on age, Mentor 4 ranks on 1st on availability, given age-factor 4 should be selected.
        //          However, strict-age is set, so since 3 is female, 3 should be selected
        ActAndAssert(algo, 3);
    }
    
    [TestMethod]
    public void Test_NotStrict()
    {
        // Arrange
        var builder = new PersonalMentorAssigningBuilder(Genders.Female, 2000);
        builder.AddMentor(1, 1900, 1, 10, 20, Genders.Male);
        var algo = builder.GetAssigner(false, 10, 0.5);
        
        // Assert
        ActAndAssert(algo, 1);
    }
    
    [TestMethod]
    public void Test_Strict_ToBackupMentor()
    {
        // Arrange
        var builder = new PersonalMentorAssigningBuilder(Genders.Female, 2000);
        builder.AddMentor(1, 1900, 1, 10, 20, Genders.Male);
        builder.AddBackupMentor(3, Genders.Female);
        var algo = builder.GetAssigner(true, 10, 0.5);
        
        // Assert
        ActAndAssert(algo, 3);
    }
    
    [TestMethod]
    public void Test_Strict_ToBackupMentor_No_Available()
    {
        // Arrange
        var builder = new PersonalMentorAssigningBuilder(Genders.Female, 2000);
        builder.AddMentor(1, 1900, 21, 10, 20, Genders.Male);
        builder.AddBackupMentor(3, Genders.Male);
        var algo = builder.GetAssigner(false, 10, 0.5);
        
        // Assert
        ActAndAssert(algo, 3);
    }
    
    [TestMethod]
    public void Test_Strict_NoBackupMentor()
    {
        // Arrange
        var builder = new PersonalMentorAssigningBuilder(Genders.Female, 2000);
        builder.AddMentor(1, 1900, 1, 10, 20, Genders.Male);
        builder.AddBackupMentor(3, Genders.Male);
        var algo = builder.GetAssigner(true, 10, 0.5);
        
        // Assert
        ActAndAssert(algo, null);
    }

    private static void ActAndAssert(PersonalMentorAssigning algo, int? mentorId)
    {
        // Act
        var mentor = algo.GetMentor();

        // Assert
        mentor?.Id.Should().Be(mentorId);
    }
}