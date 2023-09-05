namespace AskMate.Model;

public class Question
{
    public int Id { get; set; }
    public string QuestionContent { get; set; }
    public string Username { get; set; }
    public DateTime PublishedDate { get; set; }
}