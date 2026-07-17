using Freelify.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Freelify.Models.ViewModels.Job
{
    public class JobBrowseViewModel
    {
        // Filters
        public string? SearchTerm { get; set; }

        public int? CategoryId { get; set; }

        public List<int> SkillIds { get; set; } = new();

        public decimal? MinBudget { get; set; }

        public decimal? MaxBudget { get; set; }

        public JobSortBy SortBy { get; set; }

        // Pagination
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalCount { get; set; }

        // Dropdown Lists
        public SelectList? Categories { get; set; }

        public MultiSelectList? Skills { get; set; }

        // Results
        public List<JobListItemViewModel> Results { get; set; } = new();
    }
}
