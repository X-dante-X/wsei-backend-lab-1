using ApplicationCore.Models;

namespace WebApi.Dto;

public class QuizFeedbackDto
{
    public int UserId { get; set; }
    public int QuizId { get; set; }
    public int TotalQuestions { get; set; }
    public IEnumerable<QuizItemUserAnswerDto> Answers { get; set; }
}
