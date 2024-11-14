namespace LoncotesLibrary.Models.DTOs;

public class CheckoutDTO
{
    public int Id {get; set;}
    public int MaterialId {get; set;}
    public int PatronId {get; set;}
    public DateTime CheckoutDate {get; set;}
    public DateTime? ReturnDate {get; set;}
}