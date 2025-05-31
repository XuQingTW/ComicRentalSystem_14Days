using System;
using System.Linq;
using System.Windows.Forms;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Interfaces;

namespace ComicRentalSystem_14Days.Controls
{
    public partial class AdminDashboardUserControl : UserControl
    {
        private readonly ComicService _comicService;
        private readonly MemberService _memberService;
        private readonly ILogger _logger;

        internal System.Windows.Forms.Label lblTotalComicsValue;
        internal System.Windows.Forms.Label lblRentedComicsValue;
        internal System.Windows.Forms.Label lblAvailableComicsValue;
        internal System.Windows.Forms.Label lblActiveMembersValue;

        public AdminDashboardUserControl(ComicService comicService, MemberService memberService, ILogger logger)
        {
            _comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!this.DesignMode)
            {
                ApplyModernStyling();
                LoadDashboardData();
            }
        }

        private void ApplyModernStyling()
        {
            // This method can be expanded to style child controls if not done in Designer.cs
            // For example, if GroupBoxes were not styled using ModernBaseForm properties directly.
            // We are relying on Designer.cs for initial styling based on ModernBaseForm statics.
        }

        public void LoadDashboardData()
        {
            try
            {
                _logger.Log("AdminDashboardUserControl: Loading dashboard data.");
                int totalComics = _comicService.GetAllComics().Count;
                int rentedComics = _comicService.GetAllComics().Count(c => c.IsRented);
                int availableComics = totalComics - rentedComics;
                int activeMembers = _memberService.GetAllMembers().Count;

                if (lblTotalComicsValue != null) lblTotalComicsValue.Text = totalComics.ToString();
                if (lblRentedComicsValue != null) lblRentedComicsValue.Text = rentedComics.ToString();
                if (lblAvailableComicsValue != null) lblAvailableComicsValue.Text = availableComics.ToString();
                if (lblActiveMembersValue != null) lblActiveMembersValue.Text = activeMembers.ToString();
                _logger.Log("AdminDashboardUserControl: Dashboard data loaded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("AdminDashboardUserControl: Error loading dashboard data.", ex);
                if (lblTotalComicsValue != null) lblTotalComicsValue.Text = "N/A";
                if (lblRentedComicsValue != null) lblRentedComicsValue.Text = "N/A";
                if (lblAvailableComicsValue != null) lblAvailableComicsValue.Text = "N/A";
                if (lblActiveMembersValue != null) lblActiveMembersValue.Text = "N/A";
            }
        }
    }
}
