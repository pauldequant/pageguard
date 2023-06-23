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
        public PageGuardTreeNodesRenderingNotification(IScopeProvider scopeProvider, ILogger<PageGuardApiController> logger, IUserService userService, IEmailSender emailSender, IUmbracoContextAccessor umbracoContextAccessor, ILocalizationService localizationService)
        {
            _scopeProvider = scopeProvider;
            _logger = logger;
            _userService = userService;
            _emailSender = emailSender;
            _localizationService = localizationService;
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public void Handle(TreeNodesRenderingNotification notification)
        {
            if (notification.TreeAlias == "content")
            {
                PageGuardApiController pageGuardController = new PageGuardApiController(_scopeProvider, _logger, _userService, _emailSender, _umbracoContextAccessor, _localizationService);

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
