using System;
using System.Windows.Forms;
using System.Runtime.Versioning;
using ComicRentalSystem_14Days.Interfaces;

namespace ComicRentalSystem_14Days.Controls
{
    [SupportedOSPlatform("windows6.1")]
    public partial class AdminDashboardUserControl : UserControl
    {
        internal Label lblTotalComicsValue = new Label();
        internal Label lblRentedComicsValue = new Label();
        internal Label lblAvailableComicsValue = new Label();
        internal Label lblActiveMembersValue = new Label();

        private readonly IComicService _comicService;
        private readonly MemberService _memberService;
        private readonly ILogger _logger;

        public AdminDashboardUserControl(
            IComicService comicService,
            MemberService memberService,
            ILogger logger
        )
        {
            _comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            InitializeComponent();
        }

        public void LoadDashboardData()
        {
            try
            {
                int totalComicsCount = _comicService.GetAllComics()?.Count ?? 0;
                lblTotalComicsValue.Text = totalComicsCount.ToString();
                int rentedComicsCount = _comicService.GetAllComics().Count(c => c.IsRented);
                lblRentedComicsValue.Text = rentedComicsCount.ToString();

                int availableComicsCount = totalComicsCount - rentedComicsCount;
                lblAvailableComicsValue.Text = availableComicsCount.ToString();

                int activeMembersCount = _memberService.GetAllMembers()?.Count ?? 0;
                lblActiveMembersValue.Text = activeMembersCount.ToString();

                _logger.Log($"儀表板資料已載入: 總數={totalComicsCount}, 已租={rentedComicsCount}, 可借={availableComicsCount}, 活躍會員={activeMembersCount}");
            }
            catch (Exception ex)
            {
                _logger.LogError("AdminDashboardUserControl.LoadDashboardData: 取得指標時發生例外狀況。", ex);
            }
        }
    }
}
