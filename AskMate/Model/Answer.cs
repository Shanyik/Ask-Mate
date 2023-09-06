namespace AskMate.Model;

public class Answer
{
    public int Id { get; set; }
    public string Message { get; set; }
    public int QuestionId { get; set; }
    public bool Accepted { get; set; }
    public DateTime SubmissionTime { get; set; }
}