﻿namespace AskMate.Model;

public class Question
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? Author { get; set; }
    public DateTime SubmissionTime { get; set; }
    public List<Answer>? Answers { get; set; }
}