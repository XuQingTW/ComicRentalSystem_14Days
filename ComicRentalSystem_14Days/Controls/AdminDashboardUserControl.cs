using System;
using System.Windows.Forms;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Interfaces; // 若需要 ILogger
using System.Threading.Tasks;

namespace ComicRentalSystem_14Days.Controls
{
    public partial class AdminDashboardUserControl : UserControl
    {
        // ── (A) 在這裡宣告 Label 欄位，Designer 中只實例化，不要重複宣告 ──
        internal Label lblTotalComicsValue;
        internal Label lblRentedComicsValue;
        internal Label lblAvailableComicsValue;
        internal Label lblActiveMembersValue;

        private readonly ComicService _comicService;
        private readonly MemberService _memberService;
        private readonly ILogger _logger;

        public AdminDashboardUserControl(
            ComicService comicService,
            MemberService memberService,
            ILogger logger
        )
        {
            _comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            InitializeComponent();
        }

        /// <summary>
        /// 執行時呼叫此方法，從 Service 拿資料並更新四個 Label 的 Text
        /// </summary>
        public void LoadDashboardData()
        {
            try
            {
                // (1) 總漫畫數
                int totalComicsCount = _comicService.GetAllComics()?.Count ?? 0;
                lblTotalComicsValue.Text = totalComicsCount.ToString();

                // (2) 已租借漫畫數
                int rentedComicsCount = _comicService.GetAllComics().Count(c => c.IsRented);
                lblRentedComicsValue.Text = rentedComicsCount.ToString();

                // (3) 可借漫畫數
                int availableComicsCount = totalComicsCount - rentedComicsCount;
                lblAvailableComicsValue.Text = availableComicsCount.ToString();

                // (4) 活躍會員數（範例：假設 MemberService 回傳只有活躍會員）
                int activeMembersCount = _memberService.GetAllMembers()?.Count ?? 0;
                lblActiveMembersValue.Text = activeMembersCount.ToString();

                _logger.Log($"Dashboard Data loaded: Total={totalComicsCount}, Rented={rentedComicsCount}, Available={availableComicsCount}, ActiveMembers={activeMembersCount}");
            }
            catch (Exception ex)
            {
                _logger.LogError("AdminDashboardUserControl.LoadDashboardData: Exception while fetching metrics.", ex);
            }
        }
    }
}
