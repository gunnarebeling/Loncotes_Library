namespace LoncotesLibrary.Models.DTOs;
public class PatronLateFeeDTO
{
    public int Id {get; set;}
    public string FirstName {get; set;}
    public string LastName {get; set;}
    public string Address {get; set;}
    public string Email {get; set;}
    public bool IsActive {get; set;}

    public List<CheckoutLateFeeDTO> Checkouts {get; set;}

    public decimal? Balance {
        get
        {
            return Checkouts
            .Where(ch => ch.PatronId == Id && ch.Paid == false)
            .Aggregate(0m,(sum, ch) => {
                sum += ch.LateFee ?? 0;
                
                return sum;
            });
        }
    }


}