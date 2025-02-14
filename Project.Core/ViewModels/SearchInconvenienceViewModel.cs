using Project.Core.Entities;

namespace Project.Core.ViewModels;

public class SearchInconvenienceViewModel : InconvenienceViewModel 
{
    public string? SearchKey { get; set; }

    public SearchInconvenienceViewModel(Inconvenience inconvenience, string searchKey) : base(inconvenience)
    {
        SearchKey = searchKey;
    }
}
