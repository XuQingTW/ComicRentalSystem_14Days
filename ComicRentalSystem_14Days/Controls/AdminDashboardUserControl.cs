using System;
using System.Windows.Forms;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Interfaces; // ­Y»İ­n ILogger
using System.Threading.Tasks;

namespace ComicRentalSystem_14Days.Controls
{
    public partial class AdminDashboardUserControl : UserControl
    {
        // ¢w¢w (A) ¦b³o¸Ì«Å§i Label Äæ¦ì¡ADesigner ¤¤¥u¹ê¨Ò¤Æ¡A¤£­n­«½Æ«Å§i ¢w¢w
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
        /// °õ¦æ®É©I¥s¦¹¤èªk¡A±q Service ®³¸ê®Æ¨Ã§ó·s¥|­Ó Label ªº Text
        /// </summary>
        public void LoadDashboardData()
        {
            try
            {
                // (1) Á`º©µe¼Æ
                int totalComicsCount = _comicService.GetAllComics()?.Count ?? 0;
                lblTotalComicsValue.Text = totalComicsCount.ToString();

                // (2) ¤w¯²­Éº©µe¼Æ
                int rentedComicsCount = _comicService.GetAllComics().Count(c => c.IsRented);
                lblRentedComicsValue.Text = rentedComicsCount.ToString();

                // (3) ¥i­Éº©µe¼Æ
                int availableComicsCount = totalComicsCount - rentedComicsCount;
                lblAvailableComicsValue.Text = availableComicsCount.ToString();

                // (4) ¬¡ÅD·|­û¼Æ¡]½d¨Ò¡G°²³] MemberService ¦^¶Ç¥u¦³¬¡ÅD·|­û¡^
                int activeMembersCount = _memberService.GetAllMembers()?.Count ?? 0;
                lblActiveMembersValue.Text = activeMembersCount.ToString();

                _logger.Log($"å„€è¡¨æ¿è³‡æ–™å·²è¼‰å…¥: ç¸½æ•¸={totalComicsCount}, å·²ç§Ÿ={rentedComicsCount}, å¯å€Ÿ={availableComicsCount}, æ´»èºæœƒå“¡={activeMembersCount}");
            }
            catch (Exception ex)
            {
                _logger.LogError("AdminDashboardUserControl.LoadDashboardData: è®€å–æŒ‡æ¨™æ™‚ç™¼ç”Ÿä¾‹å¤–ç‹€æ³ã€‚", ex);
            }
        }
    }
}
