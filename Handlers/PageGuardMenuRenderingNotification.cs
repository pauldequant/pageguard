using Microsoft.Extensions.Configuration;

namespace PageGuard.Handlers
{
    using Umbraco.Cms.Core.Notifications;
    using Umbraco.Cms.Core.Events;
    using Umbraco.Cms.Core.Models.Trees;
    using Umbraco.Cms.Core.Services;
    using Microsoft.Extensions.Logging;
    using Controllers;
    using Umbraco.Cms.Infrastructure.Scoping;
    using Umbraco.Cms.Core.Mail;
    using Umbraco.Cms.Core.Web;
    using Umbraco.Extensions;

    public class PageGuardMenuRenderingNotification : INotificationHandler<MenuRenderingNotification>
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly ILogger<PageGuardApiController> _logger;
        private readonly IContentService _contentService;
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedTextService _localizedTextService;
        private readonly IConfiguration _configuration;

        public PageGuardMenuRenderingNotification(IScopeProvider scopeProvider, ILogger<PageGuardApiController> logger, IContentService contentService, IUserService userService, IEmailSender emailSender, IUmbracoContextAccessor umbracoContextAccessor, ILocalizationService localizationService, ILocalizedTextService localizedTextService, IConfiguration configuration)
        {
            _scopeProvider = scopeProvider;
            _logger = logger;
            _contentService = contentService;
            _userService = userService;
            _emailSender = emailSender;
            _umbracoContextAccessor = umbracoContextAccessor;
            _localizationService = localizationService;
            _localizedTextService = localizedTextService;
            _configuration = configuration;
        }

        public void Handle(MenuRenderingNotification notification)
        {
            if (notification.TreeAlias == "content")
            {
                var nodeId = notification.NodeId;
                bool convertNodeId = int.TryParse(nodeId, out int pageId);

                var nodeItem = _contentService.GetById(pageId);

                if (nodeItem != null)
                {
                    var m = new MenuItem("checkInPage", _localizedTextService.Localize("pageGuardMenu", "checkInPage"));
                    m.Icon = "icon icon-traffic";
                    m.SeparatorBefore = true;
                    m.AdditionalData.Add("actionView", "/App_Plugins/PageGuard/backoffice/views/checkin.html");

                    PageGuardApiController pageGuardController = new PageGuardApiController(_scopeProvider, _logger, _userService, _emailSender, _umbracoContextAccessor, _localizationService, _localizedTextService, _configuration);

                    if (convertNodeId)
                    {
                        bool showMenuItem = pageGuardController.ShowCheckInMenuItem(pageId);

                        if (showMenuItem)
                        {
                            notification.Menu.Items.Add(m);
                        }
                    }

                }
            }
        }
    }
}
