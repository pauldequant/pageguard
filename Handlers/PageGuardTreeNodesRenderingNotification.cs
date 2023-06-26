using Microsoft.Extensions.Configuration;

namespace PageGuard.Handlers
{
    using Microsoft.Extensions.Logging;
    using Controllers;
    using Umbraco.Cms.Core.Events;
    using Umbraco.Cms.Core.Notifications;
    using Umbraco.Cms.Infrastructure.Scoping;
    using Umbraco.Cms.Core.Mail;
    using Umbraco.Cms.Core.Services;
    using Umbraco.Cms.Core.Web;

    public class PageGuardTreeNodesRenderingNotification : INotificationHandler<TreeNodesRenderingNotification>
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly ILogger<PageGuardApiController> _logger;
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedTextService _localizedTextService;
        private readonly IConfiguration _configuration;

        public PageGuardTreeNodesRenderingNotification(IScopeProvider scopeProvider, ILogger<PageGuardApiController> logger, IUserService userService, IEmailSender emailSender, IUmbracoContextAccessor umbracoContextAccessor, ILocalizationService localizationService, ILocalizedTextService localizedTextService, IConfiguration configuration)
        {
            _scopeProvider = scopeProvider;
            _logger = logger;
            _userService = userService;
            _emailSender = emailSender;
            _localizationService = localizationService;
            _umbracoContextAccessor = umbracoContextAccessor;
            _localizedTextService = localizedTextService;
            _configuration = configuration;

        }

        public void Handle(TreeNodesRenderingNotification notification)
        {
            if (notification.TreeAlias == "content")
            {
                PageGuardApiController pageGuardController = new PageGuardApiController(_scopeProvider, _logger, _userService, _emailSender, _umbracoContextAccessor, _localizationService, _localizedTextService, _configuration);

                foreach (var node in notification.Nodes)
                {
                    if (int.TryParse(node.Id.ToString(), out int nodeId) && nodeId > 0)
                    {
                        if (pageGuardController.ShowCheckInMenuItem(nodeId))
                        {
                            node.Icon = "icon-lock color-red";
                        }
                    }
                }
            }
        }

    }
}
