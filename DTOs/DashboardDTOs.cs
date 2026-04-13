namespace HerbalMedicalCare.DTOs
{
    public class DashboardDTO
    {
        public DashboardTopStatsDTO TopStats { get; set; } = new();
        public DashboardJourneyDTO Journey { get; set; } = new();
        public DashboardSuggestionDTO Suggested { get; set; } = new();
        public List<DashboardFavoritePreviewDTO> FavoritePreview { get; set; } = new();
        public List<DashboardNotePreviewDTO> NotesPreview { get; set; } = new();
        public DashboardChartsDTO Charts { get; set; } = new();
    }

    public class DashboardTopStatsDTO
    {
        public int Favorites { get; set; }
        public int FavoritesThisWeek { get; set; }
        public int Notes { get; set; }
        public int NotesThisWeek { get; set; }
        public string TopPlant { get; set; } = "N/A";
    }

    public class DashboardJourneyDTO
    {
        public int PlantsExplored { get; set; }
        public int PlantsExploredThisMonth { get; set; }
        public int DaysActive { get; set; }
        public int DaysActiveThisWeek { get; set; }
        public int NotesCreated { get; set; }
        public int NotesCreatedThisWeek { get; set; }
        public string Quote { get; set; } = string.Empty;
        public string QuoteAuthor { get; set; } = string.Empty;
    }

    public class DashboardSuggestionDTO
    {
        public int Id { get; set; }
        public string ItemType { get; set; } = "plant";
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class DashboardFavoritePreviewDTO
    {
        public int Id { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class DashboardNotePreviewDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class DashboardChartsDTO
    {
        public List<DashboardBarChartDTO> BarData { get; set; } = new();
        public List<DashboardPieChartDTO> PieData { get; set; } = new();
        public List<DashboardLineChartDTO> LineData { get; set; } = new();
    }

    public class DashboardBarChartDTO
    {
        public string Name { get; set; } = string.Empty;
        public int Favorites { get; set; }
        public int Notes { get; set; }
    }

    public class DashboardPieChartDTO
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    public class DashboardLineChartDTO
    {
        public string Day { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}