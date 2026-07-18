using System;
using System.Collections.Generic;

namespace Freelify.Models.ViewModels.Job
{
    public class JobDetailsViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Budget { get; set; }

        public DateTime Deadline { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string ClientName { get; set; } = string.Empty;

        public string ClientUserId { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public List<string> Skills { get; set; } = new();

        public List<string> AttachmentPaths { get; set; } = new();
    }
}