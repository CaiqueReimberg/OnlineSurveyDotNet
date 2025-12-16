using Infnet.OnlineSurveys.Domain.Entities;
using Infnet.OnlineSurveys.Domain.Enums;

namespace Infnet.OnlineSurveys.Domain.Tests
{
    public class SurveyTest
    {
        [Fact]
        public void CreateSurveyTest()
        {
            // Arrange
            var title = "Teste de Survey";

            // Act
            Survey survey = new Survey(title);

            // Assert
            Assert.NotNull(survey);
            Assert.Equal(title, survey.Title);
            Assert.NotEqual(Guid.Empty, survey.Id);
            Assert.Equal(SurveyStatus.Draft, survey.Status);
            Assert.True(survey.CreatedAt <= DateTime.UtcNow);
            Assert.True(survey.CreatedAt >= DateTime.UtcNow.AddSeconds(-1));
            Assert.Empty(survey.Questions);
        }

        [Fact]
        public void AddQuestion_WhenSurveyIsDraft_ShouldAddQuestion()
        {
            // Arrange
            var survey = new Survey("Teste Survey");
            var question = new Question("Qual sua opinião?", 1);
            question.AddOption(new Option("Opção 1", 1));
            question.AddOption(new Option("Opção 2", 2));

            // Act
            survey.AddQuestion(question);

            // Assert
            Assert.Single(survey.Questions);
            Assert.Contains(question, survey.Questions);
        }

        [Fact]
        public void AddQuestion_WhenSurveyIsPublished_ShouldThrowException()
        {
            // Arrange
            var survey = new Survey("Teste Survey");
            var question1 = new Question("Questão 1", 1);
            question1.AddOption(new Option("Opção 1", 1));
            question1.AddOption(new Option("Opção 2", 2));
            survey.AddQuestion(question1);
            survey.Publish();

            var question2 = new Question("Questão 2", 2);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => survey.AddQuestion(question2));
            Assert.Equal("Cannot add questions after publication.", exception.Message);
        }

        [Fact]
        public void AddQuestion_WhenSurveyIsClosed_ShouldThrowException()
        {
            // Arrange
            var survey = new Survey("Teste Survey");
            var question1 = new Question("Questão 1", 1);
            question1.AddOption(new Option("Opção 1", 1));
            question1.AddOption(new Option("Opção 2", 2));
            survey.AddQuestion(question1);
            survey.Publish();
            survey.Close();

            var question2 = new Question("Questão 2", 2);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => survey.AddQuestion(question2));
            Assert.Equal("Cannot add questions after publication.", exception.Message);
        }

        [Fact]
        public void Publish_WhenSurveyHasValidQuestions_ShouldChangeStatusToPublished()
        {
            // Arrange
            var survey = new Survey("Teste Survey");
            var question = new Question("Qual sua opinião?", 1);
            question.AddOption(new Option("Opção 1", 1));
            question.AddOption(new Option("Opção 2", 2));
            survey.AddQuestion(question);

            // Act
            survey.Publish();

            // Assert
            Assert.Equal(SurveyStatus.Published, survey.Status);
        }

        [Fact]
        public void Publish_WhenSurveyHasNoQuestions_ShouldThrowException()
        {
            // Arrange
            var survey = new Survey("Teste Survey");

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => survey.Publish());
            Assert.Equal("Survey must have at least one question.", exception.Message);
        }

        [Fact]
        public void Publish_WhenQuestionHasLessThanTwoOptions_ShouldThrowException()
        {
            // Arrange
            var survey = new Survey("Teste Survey");
            var question = new Question("Qual sua opinião?", 1);
            question.AddOption(new Option("Única opção", 1));
            survey.AddQuestion(question);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => survey.Publish());
            Assert.Equal("Each question must have at least two options.", exception.Message);
        }

        [Fact]
        public void Publish_WhenQuestionHasNoOptions_ShouldThrowException()
        {
            // Arrange
            var survey = new Survey("Teste Survey");
            var question = new Question("Qual sua opinião?", 1);
            survey.AddQuestion(question);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => survey.Publish());
            Assert.Equal("Each question must have at least two options.", exception.Message);
        }

        [Fact]
        public void Close_WhenSurveyIsPublished_ShouldChangeStatusToClosed()
        {
            // Arrange
            var survey = new Survey("Teste Survey");
            var question = new Question("Qual sua opinião?", 1);
            question.AddOption(new Option("Opção 1", 1));
            question.AddOption(new Option("Opção 2", 2));
            survey.AddQuestion(question);
            survey.Publish();

            // Act
            survey.Close();

            // Assert
            Assert.Equal(SurveyStatus.Closed, survey.Status);
        }

        [Fact]
        public void AddQuestion_MultipleQuestions_ShouldAddAllQuestions()
        {
            // Arrange
            var survey = new Survey("Teste Survey");
            var question1 = new Question("Questão 1", 1);
            question1.AddOption(new Option("Opção 1", 1));
            question1.AddOption(new Option("Opção 2", 2));
            
            var question2 = new Question("Questão 2", 2);
            question2.AddOption(new Option("Opção A", 1));
            question2.AddOption(new Option("Opção B", 2));

            // Act
            survey.AddQuestion(question1);
            survey.AddQuestion(question2);

            // Assert
            Assert.Equal(2, survey.Questions.Count);
            Assert.Contains(question1, survey.Questions);
            Assert.Contains(question2, survey.Questions);
        }
    }
}
